using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioPeriodoFormaPagto.cs (Assimilar genérico e
// por MovimentoFinanceiro, ExecutarPersistir, ExecutarExcluir, GetDominioPeriodo) + SaldoDominioFormaHelper.
// Cobre: OP-E2-12 montagens (a) genérica e (b) movimento financeiro — as montagens (c) AdtoAcerto (E10) e
// (d)/(e) cheques (E9) chamam MontarAsync com os próprios dados.
//
// Chamado pelos processos que participam do período (IDadosPeriodoFormaPagto — OP-E1-01), dentro da
// transação deles.
public class DominioPeriodoFormaPagtoService(
    IDominioPeriodoRepository repository,
    IPeriodosAbertosService periodosAbertos,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    IContextoExecucao contexto)
{
    // [AutoSequencial("IdDominioPeriodoFormaPagto", SequencialTipo.Filial)].
    public const string NomeSequencial = "DominioPeriodoFormaPagto";
    public const string ParametroDominioPadrao = "DOMINIOPADRAO";

    /// <summary>
    /// Assimilar (302-316 / 321-336): crédito se a natureza é Credora, senão débito. Sem
    /// <paramref name="idDominioPeriodo"/>, usa o período aberto do usuário logado (GetDominioPeriodo).
    /// </summary>
    public async Task<Result<DominioPeriodoFormaPagto>> MontarAsync(int idFormaPagamento, decimal valor, NaturezaTipo natureza,
        int idOrigem, ProcessoOrigem processoOrigem, int? idDominioPeriodo = null, CancellationToken cancellationToken = default)
    {
        var idPeriodo = idDominioPeriodo;
        if (idPeriodo is null)
        {
            var aberto = await periodosAbertos.RetornarIdPeriodoAsync(cancellationToken);
            if (!aberto.IsSuccess)
                return Result<DominioPeriodoFormaPagto>.Fail([.. aberto.Errors]);
            idPeriodo = aberto.Value;
        }

        var agora = DateTime.Now;
        var forma = new DominioPeriodoFormaPagto
        {
            IdFilial = contexto.IdFilial,
            IdFormaPagamento = idFormaPagamento,
            Data = DateTime.Today,
            Hora = agora.ToString("HH:mm:ss"),
            // Convert.ToInt32(null) = 0 no legado.
            IdDominioPeriodo = idPeriodo ?? 0,
            IdOrigem = idOrigem,
            IdProcessoOrigem = processoOrigem,
        };

        if (natureza == NaturezaTipo.Credora)
            forma.Credito = valor;
        else
            forma.Debito = valor;

        return Result<DominioPeriodoFormaPagto>.Ok(forma);
    }

    /// <summary>ExecutarPersistir (63-68): grava e soma o saldo do domínio (CALC-E2-06).</summary>
    public async Task GravarAsync(DominioPeriodoFormaPagto forma, FormaPagtoTipo tipoForma, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(forma);

        forma.IdDominioPeriodoFormaPagto = await geradorSequencial.ProximoAsync(NomeSequencial, SequencialTipo.Filial, cancellationToken);
        await repository.AdicionarFormaPagtoAsync(forma, cancellationToken);
        await AplicarSaldoAsync(forma, tipoForma, somar: true, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    /// <summary>ExecutarExcluir (73-78): subtrai o saldo e remove.</summary>
    public async Task ExcluirAsync(DominioPeriodoFormaPagto forma, FormaPagtoTipo tipoForma, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(forma);

        await AplicarSaldoAsync(forma, tipoForma, somar: false, cancellationToken);
        repository.RemoverFormaPagto(forma);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task AplicarSaldoAsync(DominioPeriodoFormaPagto forma, FormaPagtoTipo tipoForma, bool somar, CancellationToken cancellationToken)
    {
        var periodo = await repository.ObterPeriodoAsync(forma.IdDominioPeriodo, contexto.IdFilial, cancellationToken);
        if (periodo is null)
            return;

        var dominio = await repository.ObterDominioAsync(periodo.IdDominio, contexto.IdFilial, cancellationToken);
        var valorPadrao = await parametroRepository.GetParametroValorAsync(ParametroDominioPadrao, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        var idPadrao = int.TryParse(valorPadrao, out var id) ? id : 0;

        var info = new PeriodoSaldoInfo(periodo.IdDominioPeriodo, periodo.IdDominio, dominio?.IdDominioPeriodo, idPadrao > 0 && idPadrao == periodo.IdDominio);
        await repository.AplicarSaldoAsync([SaldoDominioCalculadora.CalcularFormaPagto(tipoForma, forma.Credito, forma.Debito, info, somar)],
            contexto.IdFilial, cancellationToken);
    }
}

using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioPeriodoLacto.cs (OnBeforeExecutarPersistir,
// ExecutarPersistir, PersistirFinanceiro, GerarMovimentoFinanceiro, GetDominioMovimentoFinanceiro,
// ValidarOperacaoDominio, AtualizarTalaoCheque, ExecutarExcluir) + SaldoDominioLactoHelper.
// Cobre: OP-E2-10, OP-E2-11, VAL-E2-38..41 (matriz-rot.md#E2 / matriz-rtv.md#E2).
//
// Roda DENTRO da transação do handler de período (o legado marca IsTransactioned = true).
public class DominioPeriodoLactoService(
    IDominioPeriodoRepository repository,
    IGeradorMovimentoPeriodo geradorMovimento,
    IChequePeriodoPorta cheques,
    IOperacaoCaixaConsulta operacoes,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    IContextoExecucao contexto)
{
    public const string ParametroOperacaoSuprimento = "OPERACAOPADRAOSUPRIMENTO";
    public const string ParametroOperacaoSangria = "OPERACAOPADRAOSANGRIA";
    public const string ParametroDominioPadrao = "DOMINIOPADRAO";

    // [AutoSequencial("IdDominioPeriodoLacto", SequencialTipo.Filial)].
    public const string NomeSequencial = "DominioPeriodoLacto";

    // DominioPeriodoLacto.cs:216-266 (acentuação restaurada; textos literais — Q4).
    public const string MsgOperacoesNaoConfiguradas =
        "Para o lançamento do domínio período, deve ser configurado os parâmetros de operação de suprimento e sangria."; // VAL-E2-38
    public const string MsgSuprimentoNaoCaixa = "Para o lançamento do domínio período, a operação de suprimento configurada, deve ser do tipo 'Caixa'."; // VAL-E2-39
    public const string MsgSangriaNaoCaixa = "Para o lançamento do domínio período, a operação de sangria configurada, deve ser do tipo 'Caixa'.";       // VAL-E2-39
    public const string MsgSuprimentoMovtoEntidade =
        "Para o lançamento do domínio período, a operação de suprimento configurada, deve estar\r\ncom a opção 'Gera movimento de entidade' MARCADA."; // VAL-E2-40
    public const string MsgSangriaMovtoEntidade =
        "Para o lançamento do domínio período, a operação de sangria configurada, deve estar\r\ncom a opção 'Gera movimento de entidade' MARCADA.";   // VAL-E2-40
    public const string MsgSuprimentoNaoCredora =
        "Para o lançamento do domínio período, a operação de suprimento configurada, a natureza deve ser do tipo 'Credora'."; // VAL-E2-41
    public const string MsgSangriaNaoDevedora =
        "Para o lançamento do domínio período, a operação de sangria configurada, a natureza deve ser do tipo 'Devedora'.";   // VAL-E2-41

    /// <summary>OP-E2-10 — grava o lançamento, os efeitos financeiros e o saldo do domínio.</summary>
    public async Task<ValidationResult> GravarAsync(DominioPeriodoLacto lancto, StatusDominioFinanceiro status, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lancto);

        lancto.IdFilial = contexto.IdFilial;
        lancto.IdDominioPeriodoLacto = await geradorSequencial.ProximoAsync(NomeSequencial, SequencialTipo.Filial, cancellationToken);
        lancto.IdUsuarioInclusao = contexto.IdUsuario;
        lancto.DataInclusao = DateTime.Today;
        lancto.HoraInclusao = DateTime.Now;

        await repository.AdicionarLanctoAsync(lancto, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var financeiro = await PersistirFinanceiroAsync(lancto, cancellationToken);
        if (!financeiro.IsValid)
            return financeiro;

        await repository.AplicarSaldoAsync(await CalcularSaldoAsync(lancto, status, somar: true, cancellationToken), contexto.IdFilial, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ValidationResult.Ok();
    }

    /// <summary>OP-E2-11 — subtrai o saldo e remove o lançamento.</summary>
    public async Task ExcluirAsync(DominioPeriodoLacto lancto, StatusDominioFinanceiro status, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lancto);

        await repository.AplicarSaldoAsync(await CalcularSaldoAsync(lancto, status, somar: false, cancellationToken), contexto.IdFilial, cancellationToken);
        repository.RemoverLancto(lancto);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task<ValidationResult> PersistirFinanceiroAsync(DominioPeriodoLacto lancto, CancellationToken cancellationToken)
    {
        // DominioPeriodoLacto.cs:137-153.
        if (lancto.TipoForma == FormaPagtoTipo.Dinheiro)
            return await GerarMovimentoFinanceiroAsync(lancto, cancellationToken);

        if (lancto.TipoForma == FormaPagtoTipo.ChequeEmpresa)
        {
            if (lancto.IdCaixaBanco is not int idCaixa || lancto.Cheque is not int cheque)
                return ValidationResult.Ok();

            return await cheques.AtribuirPeriodoAFolhaTalaoAsync(idCaixa, cheque, lancto.IdPeriodoDestino, cancellationToken);
        }

        if (lancto.TipoForma == FormaPagtoTipo.ChequeCliente && lancto.IdChequeRecebido is int idCheque)
            return await cheques.AtribuirPeriodoAChequeRecebidoAsync(idCheque, lancto.IdPeriodoDestino, cancellationToken);

        return ValidationResult.Ok();
    }

    private async Task<ValidationResult> GerarMovimentoFinanceiroAsync(DominioPeriodoLacto lancto, CancellationToken cancellationToken)
    {
        // DominioPeriodoLacto.cs:158-190.
        var idSuprimento = await ParametroIntAsync(ParametroOperacaoSuprimento, cancellationToken);
        var idSangria = await ParametroIntAsync(ParametroOperacaoSangria, cancellationToken);

        var operacao = await ValidarOperacaoDominioAsync(idSuprimento, idSangria, cancellationToken);
        if (!operacao.IsValid)
            return operacao;

        var origem = lancto.IdPeriodoOrigem is int o ? await repository.ObterPeriodoAsync(o, contexto.IdFilial, cancellationToken) : null;
        var destino = lancto.IdPeriodoDestino is int d ? await repository.ObterPeriodoAsync(d, contexto.IdFilial, cancellationToken) : null;

        // GetDominioMovimentoFinanceiro: sangria → domínio da origem (ou do destino na abertura do padrão);
        // suprimento → domínio do destino (ou da origem no fechamento do padrão).
        for (var i = 0; i < 2; i++)
        {
            var sangria = i == 0;
            var periodo = sangria ? origem ?? destino : destino ?? origem;
            var dominio = periodo is null ? null : await repository.ObterDominioAsync(periodo.IdDominio, contexto.IdFilial, cancellationToken);
            if (dominio is null)
                continue;

            var movimento = new MovimentoPeriodo(
                dominio.IdCaixaBanco,
                sangria ? idSangria : idSuprimento,
                Math.Abs(lancto.Valor ?? 0m),
                sangria ? lancto.IdPeriodoOrigem : lancto.IdPeriodoDestino,
                DateTime.Today,
                lancto.IdDominioPeriodoLacto);

            var gerado = await geradorMovimento.GerarAsync(movimento, cancellationToken);
            if (!gerado.IsValid)
                return gerado;
        }

        return ValidationResult.Ok();
    }

    private async Task<ValidationResult> ValidarOperacaoDominioAsync(int idSuprimento, int idSangria, CancellationToken cancellationToken)
    {
        // DominioPeriodoLacto.cs:216-266. Operação inexistente: no legado Factory.Retornar falhava; aqui
        // conta como "não é do tipo Caixa".
        if (idSuprimento == 0 || idSangria == 0)
            return Falha(MsgOperacoesNaoConfiguradas);

        var suprimento = await operacoes.ObterAsync(idSuprimento, cancellationToken);
        if (suprimento is null || suprimento.IdTipoOperacao != (int)OperacaoTipo.Caixa)
            return Falha(MsgSuprimentoNaoCaixa);
        if (suprimento.LancaMovtoEntidade)
            return Falha(MsgSuprimentoMovtoEntidade);
        if (suprimento.IdNatureza != NaturezaTipo.Credora)
            return Falha(MsgSuprimentoNaoCredora);

        var sangria = await operacoes.ObterAsync(idSangria, cancellationToken);
        if (sangria is null || sangria.IdTipoOperacao != (int)OperacaoTipo.Caixa)
            return Falha(MsgSangriaNaoCaixa);
        if (sangria.LancaMovtoEntidade)
            return Falha(MsgSangriaMovtoEntidade);
        if (sangria.IdNatureza != NaturezaTipo.Devedora)
            return Falha(MsgSangriaNaoDevedora);

        return ValidationResult.Ok();
    }

    private async Task<IReadOnlyList<AjusteSaldoDominio>> CalcularSaldoAsync(DominioPeriodoLacto lancto, StatusDominioFinanceiro status, bool somar,
        CancellationToken cancellationToken)
    {
        var idPadrao = await ParametroIntAsync(ParametroDominioPadrao, cancellationToken);
        var origem = await InfoAsync(lancto.IdPeriodoOrigem, idPadrao, cancellationToken);
        var destino = await InfoAsync(lancto.IdPeriodoDestino, idPadrao, cancellationToken);
        return SaldoDominioCalculadora.CalcularLancto(lancto.TipoLancto, lancto.TipoForma, lancto.Valor, status, origem, destino, somar);
    }

    private async Task<PeriodoSaldoInfo?> InfoAsync(int? idPeriodo, int idPadrao, CancellationToken cancellationToken)
    {
        if (idPeriodo is not int id)
            return null;

        var periodo = await repository.ObterPeriodoAsync(id, contexto.IdFilial, cancellationToken);
        if (periodo is null)
            return null;

        var dominio = await repository.ObterDominioAsync(periodo.IdDominio, contexto.IdFilial, cancellationToken);
        return new PeriodoSaldoInfo(id, periodo.IdDominio, dominio?.IdDominioPeriodo, idPadrao > 0 && idPadrao == periodo.IdDominio);
    }

    private async Task<int> ParametroIntAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return int.TryParse(valor, out var numero) ? numero : 0;
    }

    private static ValidationResult Falha(string mensagem) => ValidationResult.Fail(new ValidationError("Operacao", mensagem));
}

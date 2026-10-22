using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Services;

// Origem: DominioPeriodo.cs (FormasPagamento/CarregarListaFormaPagto 124-146, Divergentes 102-118,
// CalcularTotalFormasPagto 972-989, RetornarChequesFechamentoPeriodo 995-1005), DominioUtil.ListarFormasPagto,
// ResumoPeriodoFormaPagtoConsulta.cs (Saldo, ValorCalculado, ValorConferencia, GetValorInformado) e
// FechamentoCaixaBase.Diferenca (308-317). Transcrito sem refatorar (Regra 5).
// Cobre: OP-E2-15 · CALC-E2-01, CALC-E2-02, CALC-E2-03 (golden de paridade: E2-T08).

/// <summary>Quantidade/valor informados pelo usuário para uma forma, na conferência do fechamento.</summary>
public sealed record FormaInformada(FormaPagtoTipo TipoForma, int Quantidade, decimal ValorInformado);

/// <summary>Formas do fechamento, divergentes e totais (sem arredondamento — Q1).</summary>
public sealed record FechamentoPeriodo(IReadOnlyList<FormaFechamentoPeriodo> Formas, IReadOnlyList<FormaFechamentoPeriodo> Divergentes,
    decimal TotalCalculado, decimal TotalConferencia);

public class FechamentoPeriodoService(IResumoFormaPagtoConsulta resumos, IChequePeriodoPorta cheques)
{
    /// <summary>OP-E2-15 — carrega as formas do período e aplica o que foi informado.</summary>
    public async Task<Result<FechamentoPeriodo>> CarregarAsync(int idDominioPeriodo, IReadOnlyList<FormaInformada>? informadas = null,
        CancellationToken cancellationToken = default)
    {
        var linhas = await resumos.ListarAsync(idDominioPeriodo, cancellationToken);

        // ChequeEmpresa: o ValorCalculado é a QUANTIDADE de folhas de talão do período (Q8).
        var folhas = 0;
        if (linhas.Any(l => l.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa))
        {
            var lista = await cheques.ListarFolhasTalaoDoPeriodoAsync(idDominioPeriodo, cancellationToken);
            if (!lista.IsSuccess)
                return Result<FechamentoPeriodo>.Fail([.. lista.Errors]);
            folhas = lista.Value!.Count;
        }

        var porForma = (informadas ?? []).GroupBy(i => i.TipoForma).ToDictionary(g => g.Key, g => g.First());
        var formas = new List<FormaFechamentoPeriodo>();

        foreach (var l in linhas)
        {
            var forma = new FormaFechamentoPeriodo
            {
                IdDominioPeriodo = l.IdDominioPeriodo,
                Descricao = l.Descricao,
                TipoFormaPagto = l.TipoFormaPagto,
                Credito = l.Credito,
                Debito = l.Debito,
                Saldo = ArredondamentoFinanceiro.Arredondar((l.Credito ?? 0m) - (l.Debito ?? 0m), 2),
            };
            forma.ValorCalculado = forma.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa ? folhas : forma.Saldo;

            // CarregarListaFormaPagto: sem saldo (exceto cheque emitido) ou cheque emitido sem folhas → fora.
            if (forma.Saldo == 0 && forma.TipoFormaPagto != FormaPagtoTipo.ChequeEmpresa)
                continue;
            if (forma.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa && forma.ValorCalculado == 0)
                continue;

            if (porForma.TryGetValue(forma.TipoFormaPagto, out var informada))
            {
                forma.Quantidade = informada.Quantidade;
                forma.ValorInformado = informada.ValorInformado;
            }

            forma.ValorConferencia = CalcularValorConferencia(forma);
            AplicarDiferenca(forma);
            formas.Add(forma);
        }

        var (totalCalculado, totalConferencia) = CalcularTotal(formas);
        var divergentes = formas.Where(f => ListarFormasPagto(f.TipoFormaPagto) && f.Diferenca != 0).ToList();

        return Result<FechamentoPeriodo>.Ok(new FechamentoPeriodo(formas, divergentes, totalCalculado, totalConferencia));
    }

    /// <summary>DominioUtil.ListarFormasPagto — formas que entram na conferência.</summary>
    public static bool ListarFormasPagto(FormaPagtoTipo tipo)
        => tipo is not (FormaPagtoTipo.Abatimento or FormaPagtoTipo.Credito or FormaPagtoTipo.CreditoPortador or FormaPagtoTipo.Outros
            or FormaPagtoTipo.ParcelamentoProprio or FormaPagtoTipo.ParcelamentoFinanceira or FormaPagtoTipo.Deposito);

    /// <summary>CALC-E2-02 — ValorConferencia / GetValorInformado.</summary>
    public static decimal CalcularValorConferencia(FormaFechamentoPeriodo forma)
    {
        if (forma.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa)
            return forma.Quantidade;

        return ListarFormasPagto(forma.TipoFormaPagto) ? forma.ValorInformado : forma.ValorCalculado;
    }

    /// <summary>CALC-E2-03 — Situacao sobre a diferença ANTES do arredondamento; Diferenca arredondada a 2.</summary>
    public static void AplicarDiferenca(FormaFechamentoPeriodo forma)
    {
        var dif = forma.ValorConferencia - forma.ValorCalculado;
        forma.Situacao = dif != 0 ? 1 : 0;
        forma.Diferenca = ArredondamentoFinanceiro.Arredondar(dif, 2);
    }

    /// <summary>
    /// CALC-E2-01 — soma sem ChequeEmpresa. O legado chama Funcoes.Arredondar e descarta o retorno
    /// (DominioPeriodo.cs:987-988): o total NÃO é arredondado (Q1).
    /// </summary>
    public static (decimal TotalCalculado, decimal TotalConferencia) CalcularTotal(IEnumerable<FormaFechamentoPeriodo> formas)
    {
        decimal calculado = 0m, conferencia = 0m;
        foreach (var f in formas)
        {
            if (f.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa)
                continue;

            calculado += f.ValorCalculado;
            conferencia += f.ValorConferencia;
        }

        return (calculado, conferencia);
    }
}

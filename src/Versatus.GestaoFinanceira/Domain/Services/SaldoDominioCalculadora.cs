using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: acesso.global/Saldo/Helpers/SaldoDominioLactoHelper.cs (GetObjetoIds, AdicionarValores,
// SubtrairValores, PermiteAtualizarSaldo, GetSaldoProp), SaldoDominioFormaHelper.cs (GetValor) e
// SaldoHelperBase.FormasPagtoPermitidaSaldo. Transcrito sem refatorar (Regra 5).
// Cobre: CALC-E2-05, CALC-E2-06 (matriz-rot.md#E2). Golden de paridade: E2-T08.

/// <summary>Coluna de FINDOMINIO que acompanha o SALDO geral.</summary>
public enum SaldoDominioColuna
{
    Dinheiro,
    ChequeRecebido,
    Cartao,
}

/// <summary>Período envolvido no lançamento, com o domínio dono.</summary>
/// <param name="IdPeriodoAtualDoDominio">FINDOMINIO.IDFINDOMINIOPERIODO do domínio dono.</param>
/// <param name="DominioPadrao">O domínio dono é o domínio padrão (parâmetro DominioPadrao).</param>
public sealed record PeriodoSaldoInfo(int IdDominioPeriodo, int IdDominio, int? IdPeriodoAtualDoDominio, bool DominioPadrao);

/// <summary>Ajuste a somar em FINDOMINIO: SALDO e, se houver, a coluna da forma.</summary>
public sealed record AjusteSaldoDominio(int IdDominio, decimal Valor, SaldoDominioColuna? Coluna);

public static class SaldoDominioCalculadora
{
    /// <summary>SaldoHelperBase.FormasPagtoPermitidaSaldo (355-378).</summary>
    public static bool FormaContaSaldo(FormaPagtoTipo tipo, bool dominioPadrao)
    {
        if (tipo is FormaPagtoTipo.Abatimento or FormaPagtoTipo.Credito or FormaPagtoTipo.CreditoPortador
            or FormaPagtoTipo.Outros or FormaPagtoTipo.ParcelamentoProprio or FormaPagtoTipo.ParcelamentoFinanceira
            or FormaPagtoTipo.Deposito or FormaPagtoTipo.ChequeEmpresa)
            return false;

        if (!dominioPadrao)
            return true;

        return tipo is not (FormaPagtoTipo.CartaoCredito or FormaPagtoTipo.CartaoDebito);
    }

    /// <summary>GetSaldoProp (SaldoDominioLactoHelper.cs:99-111).</summary>
    public static SaldoDominioColuna? Coluna(FormaPagtoTipo tipo) => tipo switch
    {
        FormaPagtoTipo.Dinheiro => SaldoDominioColuna.Dinheiro,
        FormaPagtoTipo.ChequeCliente => SaldoDominioColuna.ChequeRecebido,
        FormaPagtoTipo.CartaoCredito or FormaPagtoTipo.CartaoDebito => SaldoDominioColuna.Cartao,
        _ => null,
    };

    /// <summary>
    /// CALC-E2-05 — saldo do lançamento do período. <paramref name="somar"/> = gravação
    /// (AdicionarValores, com as guardas); false = exclusão (SubtrairValores, sem guardas).
    /// </summary>
    public static IReadOnlyList<AjusteSaldoDominio> CalcularLancto(LanctoDominioPeriodoTipo tipoLancto, FormaPagtoTipo tipoForma,
        decimal? valor, StatusDominioFinanceiro status, PeriodoSaldoInfo? origem, PeriodoSaldoInfo? destino, bool somar)
    {
        var ajustes = new List<AjusteSaldoDominio>();
        var v = valor ?? 0m;
        var coluna = Coluna(tipoForma);

        // GetObjetoIds (230-253): com origem e destino → os dois (a comparação do legado é entre object,
        // por referência, e sempre dá "diferentes"); senão Abertura → destino, Fechamento → origem.
        var alvos = new List<(PeriodoSaldoInfo Periodo, bool Origem)>();
        if (origem is not null && destino is not null)
        {
            alvos.Add((origem, true));
            alvos.Add((destino, false));
        }
        else if (tipoLancto == LanctoDominioPeriodoTipo.Abertura && destino is not null)
            alvos.Add((destino, false));
        else if (tipoLancto == LanctoDominioPeriodoTipo.Fechamento && origem is not null)
            alvos.Add((origem, true));

        foreach (var (periodo, ehOrigem) in alvos)
        {
            decimal saldo;
            if (somar)
            {
                saldo = 0m;
                if (PermiteAtualizarSaldo(periodo, status))
                {
                    saldo = ehOrigem ? -v : v;
                    if (!FormaContaSaldo(tipoForma, periodo.DominioPadrao))
                        saldo = 0m;
                }
            }
            else
                saldo = ehOrigem ? v : -v;

            ajustes.Add(new AjusteSaldoDominio(periodo.IdDominio, saldo, coluna));
        }

        return ajustes;
    }

    /// <summary>CALC-E2-06 — saldo da forma de pagamento do período (SaldoDominioFormaHelper.GetValor).</summary>
    public static AjusteSaldoDominio CalcularFormaPagto(FormaPagtoTipo tipoForma, decimal? credito, decimal? debito, PeriodoSaldoInfo periodo,
        bool somar)
    {
        var valor = 0m;
        if (periodo.IdPeriodoAtualDoDominio == periodo.IdDominioPeriodo && FormaContaSaldo(tipoForma, periodo.DominioPadrao))
        {
            if (credito > 0)
                valor += credito.Value;
            if (debito > 0)
                valor -= debito.Value;
        }

        return new AjusteSaldoDominio(periodo.IdDominio, somar ? valor : -valor, Coluna(tipoForma));
    }

    // PermiteAtualizarSaldo (78-93).
    private static bool PermiteAtualizarSaldo(PeriodoSaldoInfo periodo, StatusDominioFinanceiro status)
        => periodo.IdPeriodoAtualDoDominio is null
           || periodo.IdPeriodoAtualDoDominio == periodo.IdDominioPeriodo
           || status is StatusDominioFinanceiro.Abertura or StatusDominioFinanceiro.AberturaPadrao;
}

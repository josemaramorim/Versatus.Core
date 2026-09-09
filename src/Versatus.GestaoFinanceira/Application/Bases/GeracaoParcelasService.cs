using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bases;

namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/ParcelaBase.cs (legado) —
// validações e rebalanceamento de parcelas.
// Cobre (matriz-rtv.md#E1 / matriz-rot.md#E1): VAL-E1-13..28 · OP-E1-10 · CALC-E1-07/08.
//
// E1-T03 = esqueleto + contratos (CLR-04). A lógica concreta que depende da forma da
// Condição de Pagamento (`ICondicaoPagamento` do AcessoGlobal, consumida por `int`) e da
// coleção de parcelas do documento entra no épico dono (E4). Aqui ficam:
//  - as fórmulas puras (CALC-E1-07/08), transcritas sem refatorar (Regra 5);
//  - as assinaturas de validação (`ValidationResult`) e de rebalanceamento (`abstract`).
public abstract class GeracaoParcelasService
{
    // ---- Fórmulas puras (CALC) ----------------------------------------------------------

    /// <summary>
    /// CALC-E1-07 — valor mínimo de uma parcela:
    /// <c>arredondar( (valorParcelamento * percentualDivisao / 100) * percentualValorMinimo / 100 , 2)</c>.
    /// </summary>
    protected static decimal CalcularValorMinimo(decimal valorParcelamento, decimal percentualDivisao, decimal percentualValorMinimo)
    {
        decimal valorParcela = valorParcelamento * percentualDivisao / 100m;
        return ArredondamentoFinanceiro.Arredondar(valorParcela * percentualValorMinimo / 100m, 2);
    }

    /// <summary>
    /// CALC-E1-08 — percentual correspondente ao valor novo da parcela:
    /// <c>0</c> se o total for 0; senão <c>arredondar(valorParcela * 100 / total, 2)</c>.
    /// </summary>
    protected static decimal CalcularPercentualNovo(decimal valorParcela, decimal valorParcelamentoTotal)
        => valorParcelamentoTotal == 0m
            ? 0m
            : ArredondamentoFinanceiro.Arredondar(valorParcela * 100m / valorParcelamentoTotal, 2);

    // ---- Validações (contratos — VAL-E1-13..28) ----------------------------------------

    /// <summary>VAL-E1-13..15 — regras de alteração de vencimento em função da condição de pagamento.</summary>
    protected abstract ValidationResult ValidarVencimento(ParcelaBase parcela, DateTime novoVencimento);

    /// <summary>VAL-E1-16 — vencimento ≥ emissão (1ª) / &gt; anterior (demais) quando a condição permite alterar.</summary>
    protected abstract ValidationResult ValidarVencimentoParcelaAlterada(ParcelaBase parcela, int index, DateTime novoVencimento);

    /// <summary>VAL-E1-17..21 — número da parcela (&gt; 0, permitido alterar, 0..999, crescente, único) + renumeração.</summary>
    protected abstract ValidationResult ValidarNumeroParcela(ParcelaBase parcela, int novoNumero);

    /// <summary>VAL-E1-22 — valor da parcela ≥ valor mínimo (ver <see cref="CalcularValorMinimo"/>).</summary>
    protected abstract ValidationResult ValidarValorMinimo(ParcelaBase parcela, decimal novoValor);

    /// <summary>VAL-E1-23..26 — valor ≥ 0 e soma das parcelas ≤ valor parcelado.</summary>
    protected abstract ValidationResult ValidarValor(ParcelaBase parcela, decimal novoValor);

    /// <summary>VAL-E1-27 — se o Caixa/Banco é do tipo Banco, a conta bancária deve ser Conta corrente (depende do E3).</summary>
    protected abstract ValidationResult ValidarCaixaBanco(ParcelaBase parcela);

    // ---- Rebalanceamento (OP-E1-10) --------------------------------------------------

    /// <summary>
    /// OP-E1-10 — ao alterar valor/vencimento de uma parcela, recalcula as demais e joga a
    /// diferença de arredondamento na 1ª ou na última parcela conforme
    /// <c>ParcelamentoArredondamento</c> da condição.
    /// </summary>
    protected abstract IReadOnlyList<ParcelaBase> Rebalancear(IReadOnlyList<ParcelaBase> parcelas, int indiceAlterado, decimal valorParcelamentoTotal);
}

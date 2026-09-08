namespace Versatus.GestaoFinanceira.Domain.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/ParcelaBase.cs (legado,
//         `abstract class ParcelaBase : ParcelaGeral`)
// Base de: parcelas geradas por condição de pagamento (DocumentoParcela — E4).
//
// POCO abstrata SÓ DE DADOS (Artigo III). Herda os dados de ParcelaGeralBase. As
// validações (ValidarVencimento/ValidarNumeroParcela/ValidarValor/ValidarValorMinimo/
// CalcularTotalParcelas/ValidarCaixaBanco — VAL-E1-13..28) e o rebalanceamento de parcelas
// (RecalcularParcelas/CorrigirDiferenca/CalcularValorMinimo/CalcularPercentualNovo —
// OP-E1-10 / CALC-E1-07..08) vivem em Application/Bases/GeracaoParcelasService (E1-T03).
//
// A flag `validar` do legado é controle de fluxo do serviço — NÃO é dado persistido.
public abstract class ParcelaBase : ParcelaGeralBase
{
    /// <summary>Número da parcela (1..999).</summary>
    public int NumeroParcela { get; set; }

    /// <summary>Valor da parcela.</summary>
    public decimal Valor { get; set; }

    /// <summary>Data de vencimento.</summary>
    public DateTime DataVencimento { get; set; }

    /// <summary>Data de vencimento originalmente definida pela condição (para checar tolerância).</summary>
    public DateTime? DataVencimentoDefinida { get; set; }

    /// <summary>Percentual da parcela sobre o valor parcelado.</summary>
    public decimal PercentualDivisao { get; set; }

    /// <summary>Percentual recalculado quando o valor da parcela é alterado manualmente.</summary>
    public decimal PercentualDivisaoNovo { get; set; }

    /// <summary>Caixa/Banco vinculado à parcela para boleto (E3). Nulo = nenhum.</summary>
    public int? IdCaixaBancoParcela { get; set; }
}

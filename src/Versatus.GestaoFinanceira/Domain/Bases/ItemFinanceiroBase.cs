using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/ItemFinanceiroBase.cs (legado)
// Base de: itens financeiros aplicados a documento/parcela/liquidação (juros, multa,
// desconto, abatimento, acréscimo).
//
// POCO abstrata SÓ DE DADOS (Artigo III). O cálculo (ExecutarCalculo/CalcularComposto/
// CalcularDias/ConsiderarDiasParaCalculo/RetornarIndiceConvertido/DefinirValor — ver
// matriz-rot.md#E1 CALC-E1-01..06,10) e ValidarItemFinanceiro (VAL-E1-11) vivem em
// Application/Bases/CalculadoraItemFinanceiroBase (E1-T03/E1-T04).
public abstract class ItemFinanceiroBase
{
    /// <summary>Item financeiro cadastrado (E5). 0 = não informado.</summary>
    public int IdItemFinanceiro { get; set; }

    /// <summary>Nº de dias considerado no cálculo.</summary>
    public int Dias { get; set; }

    /// <summary>Valor/percentual do item, conforme <see cref="IdCalculo"/>.</summary>
    public decimal Valor { get; set; }

    /// <summary>Quando o item se aplica em relação ao vencimento.</summary>
    public ItemFinanceiroAplicar IdAplicar { get; set; }

    /// <summary>Sobre qual base o valor é calculado (calculado x parcelado).</summary>
    public ItemFinanceiroAplicacao IdAplicacao { get; set; }

    /// <summary>Operação aritmética do item (somar %, multiplicar, dividir, percentual…).</summary>
    public CalculoItemFinanceiro IdCalculo { get; set; }

    /// <summary>Se o item entra na base de cálculo de comissão.</summary>
    public bool AplicarComissao { get; set; }

    /// <summary>Índice econômico para conversão do valor do item (MOD-02). Nulo = índice padrão.</summary>
    public int? IdIndiceEconomico { get; set; }
}

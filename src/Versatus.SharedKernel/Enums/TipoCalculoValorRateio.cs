namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Modo de cálculo do valor rateado (valor fixo, percentual, invertido pela natureza...).
/// Núcleo do RateioContainer (E0-T03).
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoCalculoValorRateio (legado) — não-persistido
/// (comportamento em memória, nunca vira coluna).
/// </summary>
public enum TipoCalculoValorRateio
{
    Valor = 1,
    Percentual = 2,
    NaoCalculo = 3,
    NaoCalculoPercentual = 4,
    ValorNaturezaInvertida = 5,
    PercentualDevedor = 6,
    PercentualNatureza = 7,
    PercentualNaturezaInvertida = 8
}

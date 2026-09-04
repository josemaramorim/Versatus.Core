namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Regra de validação da soma do rateio (não valida, valida percentual, valida valor).
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoRateioItemValidacao (legado, sem valor
/// explícito — 0,1,2 por posição) — não-persistido (comportamento em memória, nunca vira
/// coluna).
/// </summary>
public enum TipoRateioItemValidacao
{
    NaoValidar = 0,
    ValidarPercentual = 1,
    ValidarValor = 2
}

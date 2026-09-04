namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Modo de correção do índice econômico (usa valor do dia, data anterior, data posterior).
/// Origem: Projeto.Geral.Enumerado.IndiceModoCorrecao (legado, [TipoEnumerado(105)]) —
/// persistido em FININDICECONVERSOR.
/// </summary>
public enum IndiceModoCorrecao
{
    UsaValorDia = 210,
    UsarDataAnterior = 106,
    UsarDataPosterior = 107
}

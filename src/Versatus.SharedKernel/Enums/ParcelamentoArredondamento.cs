namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Em qual parcela sobra o arredondamento de valor no parcelamento (primeira/última).
/// Origem: Projeto.Geral.Enumerado.ParcelamentoArredondamento (legado, [TipoEnumerado(45)]) —
/// persistido. Movido de Versatus.AcessoGlobal para cá por DEC-007 (usado por 2+ módulos:
/// AcessoGlobal e GestaoFinanceira). Valores inteiros preservados do legado — não renumerar.
/// </summary>
public enum ParcelamentoArredondamento
{
    Primeira = 46,
    Ultima = 47
}

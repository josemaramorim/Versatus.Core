namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Modo de geração das datas de vencimento do parcelamento.
/// Origem: Projeto.Geral.Enumerado.ParcelamentoTipo (legado, [TipoEnumerado(118)]) — persistido.
/// Movido de Versatus.AcessoGlobal para cá por DEC-007 (usado por 2+ módulos: AcessoGlobal e
/// GestaoFinanceira). Valores inteiros preservados do legado — não renumerar.
/// </summary>
public enum ParcelamentoTipo
{
    DiaFixo = 119,
    DiasEntreParcela = 120,
    DiasUteis = 693
}

namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Tipo de condição de pagamento (parcelada, por faixa de dias, semanal).
/// Origem: Projeto.Geral.Enumerado.CondicaoPagtoTipo (legado, [TipoEnumerado(35)]) — persistido.
/// Movido de Versatus.AcessoGlobal para cá por DEC-007 (usado por 2+ módulos: AcessoGlobal e
/// GestaoFinanceira). Valores inteiros preservados do legado — não renumerar.
/// </summary>
public enum CondicaoPagtoTipo
{
    Parcelada = 36,
    FaixaDias = 37,
    Semanal = 38
}

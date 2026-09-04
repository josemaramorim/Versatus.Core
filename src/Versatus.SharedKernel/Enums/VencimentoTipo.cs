namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Ajuste de vencimento em relação a dia útil (normal, antecipa, prorroga).
/// Origem: Projeto.Geral.Enumerado.VencimentoTipo (legado, [TipoEnumerado(58)]) — persistido.
/// Movido de Versatus.AcessoGlobal para cá por DEC-007 (usado por 2+ módulos: AcessoGlobal e
/// GestaoFinanceira). Valores inteiros preservados do legado — não renumerar.
/// </summary>
public enum VencimentoTipo
{
    Normal = 59,
    AntecipaDiaUtil = 60,
    ProrrogaDiaUtil = 61
}

namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Subtipo de conta de caixa (normal/cofre) — só se aplica quando ContaTipo = Caixa.
/// Origem: Projeto.Geral.Enumerado.TipoContaCaixa (legado, [TipoEnumerado(1481)]) —
/// persistido em FINCAIXABANCO.IDTIPOCONTACAIXA (anulável — 0/nulo quando a conta é Banco).
/// </summary>
public enum TipoContaCaixa
{
    Normal = 1482,
    Cofre = 1483
}

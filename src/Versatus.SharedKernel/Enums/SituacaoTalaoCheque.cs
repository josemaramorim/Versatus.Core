namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Situação de uma folha de talão de cheque emitido.
/// Origem: Projeto.Geral.Enumerado.SituacaoTalaoCheque (legado, [TipoEnumerado(187)]) —
/// persistido em FINTALAOCHEQUE.IDSITUACAO. Épico E9.
/// </summary>
public enum SituacaoTalaoCheque
{
    Disponivel = 188,
    Cancelado = 189,
    Emitido = 190,
    Devolvido = 498,
    Negociado = 499,
    Sacado = 740
}

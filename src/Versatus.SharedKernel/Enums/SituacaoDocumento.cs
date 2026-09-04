namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Situação de um documento financeiro (título a pagar/receber) e de suas parcelas.
/// Origem: Projeto.Geral.Enumerado.SituacaoDocumento (legado, [TipoEnumerado(82)]) —
/// persistido em FINDOCUMENTO.IDSITUACAO e FINDOCTOPARCELA.IDSITUACAO (a parcela reusa
/// este mesmo enum — não existe "SituacaoParcela" no legado; ver enums.md §2 Tabela A).
/// </summary>
public enum SituacaoDocumento
{
    Aberto = 83,
    Liquidado = 85,
    LiquidadoParcial = 221,
    Cancelado = 281
}

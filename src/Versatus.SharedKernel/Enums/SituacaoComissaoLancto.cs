namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Situação de um lançamento de comissão gerado por Documento/Reversão.
/// Origem: Projeto.Geral.Enumerado.SituacaoComissaoLancto (legado, [TipoEnumerado(431)]) —
/// persistido em FINDOCUMENTOCOMISSIONADO (coluna a confirmar no analysis de E4/E8).
/// </summary>
public enum SituacaoComissaoLancto
{
    Previsto = 432,
    Efetivado = 433,
    Revertido = 434,
    Cancelado = 435,
    EfetivadoParcial = 532
}

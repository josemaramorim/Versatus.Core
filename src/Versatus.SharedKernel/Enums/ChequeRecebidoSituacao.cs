namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Máquina de estados do cheque recebido (custódia, depósito, devolução, repasse...).
/// Origem: Projeto.Geral.Enumerado.ChequeRecebidoSituacao (legado, [TipoEnumerado(215)]) —
/// persistido em FINCHEQUERECEBIDO.IDSITUACAO / FINCHEQUERECEBIDOMOVTO. Épico E9.
/// </summary>
public enum ChequeRecebidoSituacao
{
    Aberto = 216,
    Devolvido = 217,
    Baixado = 218,
    Negociado = 235,
    Cancelado = 286,
    Repassado = 501,
    DevolucaoRepasse = 605,
    Sacado = 739
}

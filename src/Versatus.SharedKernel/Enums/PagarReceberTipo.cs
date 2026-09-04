namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Se o documento/movimento é a pagar ou a receber.
/// Origem: Projeto.Geral.Enumerado.PagarReceberTipo (legado, [TipoEnumerado(171)]) —
/// persistido em FINDOCTOMOVTO.IDRECEBERPAGAR, FINDOCUMENTO, FINPROJECAOFLUXOCAIXA.
/// </summary>
public enum PagarReceberTipo
{
    Pagar = 172,
    Receber = 173,
    MovimentoCartao = 1192
}

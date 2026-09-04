namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Tipo de registro de uma parcela (normal, juro capitalizado, boleto agrupado, reversão...).
/// Origem: Projeto.Geral.Enumerado.RegistroDocumentoTipo (legado) — SEM [TipoEnumerado] no
/// código-fonte, mas o valor é gravado (persistido) em FINDOCTOPARCELA.IDTIPOREGISTRO;
/// por não ter atributo, o label não vem de GloTipoEnumerado.
/// </summary>
public enum RegistroDocumentoTipo
{
    Normal = 223,
    JuroCapitalizado = 224,
    BoletoAgrupado = 225,
    Reversao = 226,
    RevertidoAgrupado = 227,
    Descontado = 228
}

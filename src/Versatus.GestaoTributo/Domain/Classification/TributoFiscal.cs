namespace Versatus.GestaoTributo.Domain.Classification;

public class TributoFiscal
{
    public int IdTributoFiscal { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public int IdTipoTributo { get; set; }
}

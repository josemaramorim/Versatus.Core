namespace Versatus.GestaoTributo.Domain.Classification;

public class CestSegmento
{
    public int IdCestSegmento { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public int? IdCestOutros { get; set; }
}

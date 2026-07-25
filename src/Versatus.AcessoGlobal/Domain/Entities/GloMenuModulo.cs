namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloMenuModulo (banco de dados)
// Tabela: GloMenuModulo
public class GloMenuModulo
{
    public int IdMenu { get; set; }
    public int IdModulo { get; set; }
    public int Ordem { get; set; }

    public GloMenu? Menu { get; set; }
    public GloModulo? Modulo { get; set; }
}

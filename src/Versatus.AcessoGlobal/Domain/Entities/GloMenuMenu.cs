namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloMenuMenu (banco de dados)
// Tabela: GloMenuMenu
public class GloMenuMenu
{
    public int IdMenu { get; set; }
    public int IdMenuPai { get; set; }
    public int Ordem { get; set; }

    public GloMenu? Menu { get; set; }
    public GloMenu? MenuPai { get; set; }
}

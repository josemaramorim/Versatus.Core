namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloMenu (banco de dados)
// Tabela: GloMenu
public class GloMenu
{
    public int IdMenu { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Ordem { get; set; }
}

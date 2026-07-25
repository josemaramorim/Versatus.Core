namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloRotina (banco de dados)
// Tabela: GloRotina
public class GloRotina
{
    public int IdRotina { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TipoRotina { get; set; }
    public string? Objeto { get; set; }
}

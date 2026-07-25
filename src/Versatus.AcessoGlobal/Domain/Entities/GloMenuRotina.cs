namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloMenuRotina (banco de dados)
// Tabela: GloMenuRotina
public class GloMenuRotina
{
    public int IdMenu { get; set; }
    public int IdRotina { get; set; }
    public int Ordem { get; set; }

    public GloMenu? Menu { get; set; }
    public GloRotina? Rotina { get; set; }
}

namespace Versatus.AcessoGlobal.Domain.Entities;

// Origem: GloFavorito (banco de dados)
// Tabela: GloFavorito
public class GloFavorito
{
    public int IdFavorito { get; set; }
    public int IdUsuario { get; set; }
    public int IdRotina { get; set; }
    public int Ordem { get; set; }

    public GloRotina? Rotina { get; set; }
}

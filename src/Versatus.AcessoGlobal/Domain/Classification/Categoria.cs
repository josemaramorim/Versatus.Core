namespace Versatus.AcessoGlobal.Domain.Classification;

/// <summary>
/// Entidade que representa uma Categoria (Classificação Geral).
/// Mapeada da tabela legada GloCategoria.
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador único (IdCategoria).
    /// </summary>
    public int IdCategoria { get; set; }

    /// <summary>
    /// Chave estrangeira para a Categoria pai (autorrelacionamento).
    /// </summary>
    public int? IdCategoriaPai { get; set; }

    /// <summary>
    /// Descrição da categoria.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Define se é Sintética (Pai) ou Analítica (Folha).
    /// </summary>
    public int IdSinteticoAnalitico { get; set; }

    /// <summary>
    /// Define se a categoria está ativa.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public Categoria? CategoriaPai { get; set; }
    public ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

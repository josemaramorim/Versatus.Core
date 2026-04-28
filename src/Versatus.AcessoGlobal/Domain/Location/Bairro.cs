namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa um Bairro.
/// Mapeada da tabela legada GloBairro.
/// </summary>
public class Bairro
{
    /// <summary>
    /// Identificador único (IdBairro).
    /// </summary>
    public int IdBairro { get; set; }

    /// <summary>
    /// Chave estrangeira para a Cidade à qual este bairro pertence.
    /// </summary>
    public int IdCidade { get; set; }

    /// <summary>
    /// Nome/Descrição do bairro (Descricao no legado).
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Define se o bairro está ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Propriedade de Navegação
    public Cidade? Cidade { get; set; }

    // Metadados de Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa o Tipo de Logradouro (Rua, Avenida, Praça, etc.).
/// Mapeada da tabela legada GloTipoLogradouro.
/// </summary>
public class TipoLogradouro
{
    /// <summary>
    /// Identificador único (IdTipoLogradouro).
    /// </summary>
    public int IdTipoLogradouro { get; set; }

    /// <summary>
    /// Abreviatura do tipo de logradouro (ex: R., Av., Pç.).
    /// </summary>
    public string Abreviacao { get; set; } = string.Empty;

    /// <summary>
    /// Nome descritivo (Descricao no legado).
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Define se o tipo de logradouro está ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Metadados de Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

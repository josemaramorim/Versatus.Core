namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa um Município/Cidade.
/// Mapeada da tabela legada GloCidade.
/// </summary>
public class Cidade
{
    /// <summary>
    /// Identificador único (IdCidade no legado).
    /// </summary>
    public int IdCidade { get; set; }

    /// <summary>
    /// Sigla do Estado ao qual esta cidade pertence (FK via string Uf no legado).
    /// </summary>
    public string SiglaEstado { get; set; } = string.Empty;

    /// <summary>
    /// Chave estrangeira para o País. 
    /// O legado possui essa redundância (IdPais na cidade e Estado).
    /// </summary>
    public int IdPais { get; set; }

    /// <summary>
    /// Nome da cidade (Descricao no legado).
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CEP da cidade (para cidades com CEP único).
    /// </summary>
    public string CEP { get; set; } = string.Empty;

    /// <summary>
    /// Código do IBGE (7 dígitos para municípios brasileiros).
    /// </summary>
    public string CodigoIBGE { get; set; } = string.Empty;

    /// <summary>
    /// Código interno da cidade no sistema legado.
    /// </summary>
    public int CodigoCidade { get; set; }

    /// <summary>
    /// Define se a cidade está ativa.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Latitude para geolocalização.
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// Longitude para geolocalização.
    /// </summary>
    public decimal? Longitude { get; set; }

    // Propriedades de Navegação
    public Estado? Estado { get; set; }
    public Pais? Pais { get; set; }

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

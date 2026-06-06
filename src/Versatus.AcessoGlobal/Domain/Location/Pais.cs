namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa um País para fins geográficos e fiscais.
/// Mapeada da tabela legada GloPais.
/// </summary>
public class Pais
{
    /// <summary>
    /// Identificador único (IdGloPais no legado).
    /// </summary>
    public int IdPais { get; set; }

    /// <summary>
    /// Código do Banco Central (BACEN) com 4 dígitos.
    /// Ex: 1058 para Brasil.
    /// </summary>
    public string CodigoBACEN { get; set; } = string.Empty;

    /// <summary>
    /// Abreviatura ou Sigla do país.
    /// </summary>
    public string Abreviacao { get; set; } = string.Empty;

    /// <summary>
    /// Nome descritivo do país.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Define se o país está ativo para uso no sistema.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Metadados de Auditoria (preservando estrutura do legado)
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

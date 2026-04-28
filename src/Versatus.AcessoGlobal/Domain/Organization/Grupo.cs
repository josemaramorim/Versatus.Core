namespace Versatus.AcessoGlobal.Domain.Organization;

/// <summary>
/// Entidade que representa um Grupo Empresarial.
/// Mapeada da tabela legada GloGrupo.
/// </summary>
public class Grupo
{
    /// <summary>
    /// Identificador único do grupo (IdGrupo).
    /// </summary>
    public int IdGrupo { get; set; }

    /// <summary>
    /// Nome do Grupo Empresarial.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Define se o grupo está ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();

    // Metadados de Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

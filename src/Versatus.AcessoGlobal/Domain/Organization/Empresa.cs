namespace Versatus.AcessoGlobal.Domain.Organization;

/// <summary>
/// Entidade que representa uma Empresa dentro de um grupo empresarial.
/// Mapeada da tabela legada GloEmpresa.
/// </summary>
public class Empresa
{
    /// <summary>
    /// Identificador único da empresa (IdEmpresa).
    /// </summary>
    public int IdEmpresa { get; set; }

    /// <summary>
    /// Chave estrangeira para o Grupo ao qual esta empresa pertence.
    /// </summary>
    public int IdGrupo { get; set; }

    /// <summary>
    /// Nome Fantasia ou Razão Social reduzida da empresa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Especialização tributária da empresa.
    /// </summary>
    public int IdTributacaoEspecial { get; set; }

    /// <summary>
    /// Define se a empresa está ativa.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Mascaras de Codificação (ERP)
    public string? MascaraClasse { get; set; }
    public string? MascaraCentroCusto { get; set; }
    public string? MascaraProjeto { get; set; }
    public string? MascaraPlanoContabil { get; set; }

    // Mensagens Padrão
    public string? MsgInicial { get; set; }
    public string? MsgFinal { get; set; }

    // Relacionamentos
    public Grupo? Grupo { get; set; }
    public ICollection<Filial> Filiais { get; set; } = new List<Filial>();

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

namespace Versatus.AcessoGlobal.Domain.Security;

/// <summary>
/// Representa as informações de um Usuário do sistema.
/// Origem: servidor/objeto de negócio/acesso.global/Usuario.cs (legado)
/// Tabela: GloUsuario
/// </summary>
public class Usuario
{
    public int IdUsuario { get; set; }
    
    /// <summary>
    /// Identificador do Funcionário associado (se aplicável).
    /// </summary>
    public int? IdFuncionario { get; set; }

    /// <summary>
    /// Nome de Acesso (Login). Mapeia para NomeAcesso no legado.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha do usuário. Mapeia para Senha no legado.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public int IdPerfil { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime? UltimoLogon { get; set; }

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public Perfil? Perfil { get; set; }
}

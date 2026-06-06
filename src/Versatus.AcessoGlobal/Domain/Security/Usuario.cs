using System;
using System.Collections.Generic;

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
    /// Nome de Acesso (Login). Mapeia para NomeAcesso no legado.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha do usuário (varbinary no banco). Mapeia para Senha no legado.
    /// </summary>
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public bool Ativo { get; set; } = true;

    // Auditoria (Nullables para suportar registros legados)
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos - Perfil (Mapeamento Many-to-Many via GloPerfilUsuario)
    public ICollection<Perfil> Perfis { get; set; } = new List<Perfil>();
}

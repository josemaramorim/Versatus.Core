namespace Versatus.AcessoGlobal.Domain.Security;

/// <summary>
/// Representa as informações de um Perfil de Acesso do usuário.
/// Origem: servidor/objeto de negócio/acesso.global/Perfil.cs (legado)
/// Tabela: GloPerfil
/// </summary>
public class Perfil
{
    public int IdPerfil { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Administrador { get; set; }
    public bool UsaDominioFinanceiro { get; set; }

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

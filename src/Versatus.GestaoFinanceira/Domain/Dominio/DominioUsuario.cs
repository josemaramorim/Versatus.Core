namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioUsuario.cs (legado)
// Tabela: FINDOMINIOUSUARIO (10 colunas) — PK (IDFINDOMINIO, IDGLOUSUARIO, IDGLOFILIAL),
// filial em 3º na ordem física. Mapa: analysis/E2-dominio.md §2.6. Item do agregado Dominio.
//
// POCO SÓ DE DADOS. VAL-E2-29..34 (unicidade, não altera usuário salvo, perfil, outro
// domínio ativo, 1º novo vira principal) são do DominioService (E2-T04).
public class DominioUsuario
{
    public int IdDominio { get; set; }

    /// <summary>FK para GLOUSUARIO (IDGLOUSUARIO).</summary>
    public int IdUsuario { get; set; }

    public int IdFilial { get; set; }

    public bool UsuarioPrincipal { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

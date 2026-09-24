namespace Versatus.GestaoFinanceira.Domain.Bancos;

// Origem: servidor/objeto de negócio/gestao.financeira/Cobrador.cs (legado)
// Tabela: FINCOBRADOR (13 colunas) — PK (IDFINCOBRADOR, IDGLOFILIAL).
// Mapa: analysis/E3-caixa-banco.md §2.6. CRUD simples (CobradorService — E3-T05).
public class Cobrador
{
    public int IdCobrador { get; set; }
    public int IdFilial { get; set; }

    /// <summary>FK lógica para GLOENTIDADE (MOD-02) — IDGLOENTIDADE.</summary>
    public int IdEntidade { get; set; }

    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }

    /// <summary>FK lógica para GLOUSUARIO — IDGLOUSUARIO.</summary>
    public int? IdUsuario { get; set; }

    /// <summary>FK lógica para o meio de contato (MOD-02) — IDGLOMEIOCONTATO.</summary>
    public int? IdMeioContato { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

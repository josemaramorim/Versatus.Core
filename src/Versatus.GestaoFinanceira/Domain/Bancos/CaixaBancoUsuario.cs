namespace Versatus.GestaoFinanceira.Domain.Bancos;

// Origem: servidor/objeto de negócio/gestao.financeira/CaixaBancoUsuario.cs (legado)
// Tabela: FINCAIXABANCOUSUARIO (9 colunas) — PK (IDFINCAIXABANCO, IDGLOFILIAL, IDGLOUSUARIO).
// Mapa: analysis/E3-caixa-banco.md §2.2. Item do agregado CaixaBanco.
//
// POCO SÓ DE DADOS. VAL-E3-11 (usuário único no caixa) e VAL-E3-12 (não altera usuário já
// salvo) são do CaixaBancoService (E3-T05), dentro da sincronização em lote OP-E3-07.
public class CaixaBancoUsuario
{
    public int IdCaixaBanco { get; set; }
    public int IdFilial { get; set; }

    /// <summary>FK lógica para GLOUSUARIO (IDGLOUSUARIO).</summary>
    public int IdUsuario { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

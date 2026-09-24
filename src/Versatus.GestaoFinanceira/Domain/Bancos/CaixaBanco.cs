using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Bancos;

// Origem: servidor/objeto de negócio/gestao.financeira/CaixaBanco.cs (legado, ObjectPersist)
// Tabela: FINCAIXABANCO (17 colunas) — PK (IDFINCAIXABANCO, IDGLOFILIAL).
// Mapa coluna→propriedade: specs/modulos/MOD-05/analysis/E3-caixa-banco.md §2.1.
//
// POCO SÓ DE DADOS (Artigo III). Validações VAL-E3-01..10 e a persistência OP-E3-01..04/07
// vivem em CaixaBancoService (E3-T05). Os dados bancários da conta (quando TipoConta = Banco)
// estão em ContaBancaria (FINCONTABANCARIA, 1:1).
public class CaixaBanco
{
    public int IdCaixaBanco { get; set; }
    public int IdFilial { get; set; }
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Caixa ou Banco (IDTIPOCONTA).</summary>
    public ContaTipo TipoConta { get; set; }

    public bool Ativo { get; set; }
    public bool EntraFluxoCaixa { get; set; }
    public DateTime? UltimaDataConferida { get; set; }
    public decimal? Saldo { get; set; }
    public string? ContaContabil { get; set; }

    /// <summary>FK lógica para o plano de contas do módulo contábil (IDCONPLANOCONTABIL).</summary>
    public int? IdPlanoContabil { get; set; }

    /// <summary>Normal/Cofre — só quando TipoConta = Caixa; nulo/0 quando Banco (IDTIPOCONTACAIXA).</summary>
    public TipoContaCaixa? TipoContaCaixa { get; set; }

    // Agregado: usuários vinculados ao caixa (FINCAIXABANCOUSUARIO) — Artigo V.
    private readonly List<CaixaBancoUsuario> _usuarios = [];
    public IReadOnlyList<CaixaBancoUsuario> Usuarios => _usuarios.AsReadOnly();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    public void AdicionarUsuario(CaixaBancoUsuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        _usuarios.Add(usuario);
    }

    public bool RemoverUsuario(CaixaBancoUsuario usuario) => _usuarios.Remove(usuario);
}

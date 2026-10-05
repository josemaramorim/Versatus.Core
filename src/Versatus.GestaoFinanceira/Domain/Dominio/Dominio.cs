namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/Dominio.cs (legado, ObjectMaster)
// Tabela: FINDOMINIO (23 colunas) — PK (IDFINDOMINIO, IDGLOFILIAL).
// Mapa coluna→propriedade: specs/modulos/MOD-05/analysis/E2-dominio.md §2.1.
//
// POCO SÓ DE DADOS (Artigo III). Validações VAL-E2-01..15/28..37 ficam no DominioService
// (E2-T04); as operações de período (abrir, fechar, suprir, padrão — OP-E2-03..09) ficam nos
// handlers de Application/Handlers/Periodo (E2-T05). O estado transitório do legado
// (Status, SetarSaldoCaixaBanco, ValorAbertura, SaldoCaixaBanco, FormasFechamento) não é
// persistido: vira parâmetro das operações.
public class Dominio
{
    public int IdDominio { get; set; }
    public int IdFilial { get; set; }

    /// <summary>Caixa do domínio — FINCAIXABANCO tipo Caixa/Normal (VAL-E2-14).</summary>
    public int IdCaixaBanco { get; set; }

    /// <summary>
    /// Ponteiro para o período corrente (aberto ou o último fechado). Só muda na abertura
    /// (OP-E2-03/06); nulo enquanto o domínio nunca abriu período.
    /// </summary>
    public int? IdDominioPeriodo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    // Construtor legado: Ativo = true; demais flags = false.
    public bool Ativo { get; set; } = true;
    public bool Tesouraria { get; set; }
    public bool AbrirPeriodo { get; set; }
    public bool AbrirOutrosPeriodos { get; set; }
    public bool FecharPeriodo { get; set; }
    public bool FecharOutrosPeriodos { get; set; }
    public bool MovimentoBanco { get; set; }
    public bool ConsultaTodosPeriodos { get; set; }

    // Saldos — gravados só pelo saldo do domínio (CALC-E2-05..07), nunca pelo cadastro.
    public decimal? Saldo { get; set; }
    public decimal? SaldoDinheiro { get; set; }
    public decimal? SaldoChequeRecebido { get; set; }
    public decimal? SaldoCartao { get; set; }

    // Agregados: usuários (FINDOMINIOUSUARIO) e responsáveis (FINDOMINIORESPONSAVEL) — Artigo V.
    private readonly List<DominioUsuario> _usuarios = [];
    public IReadOnlyList<DominioUsuario> Usuarios => _usuarios.AsReadOnly();

    private readonly List<DominioResponsavel> _responsaveis = [];
    public IReadOnlyList<DominioResponsavel> Responsaveis => _responsaveis.AsReadOnly();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    public void AdicionarUsuario(DominioUsuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        _usuarios.Add(usuario);
    }

    public bool RemoverUsuario(DominioUsuario usuario) => _usuarios.Remove(usuario);

    public void AdicionarResponsavel(DominioResponsavel responsavel)
    {
        ArgumentNullException.ThrowIfNull(responsavel);
        _responsaveis.Add(responsavel);
    }

    public bool RemoverResponsavel(DominioResponsavel responsavel) => _responsaveis.Remove(responsavel);
}

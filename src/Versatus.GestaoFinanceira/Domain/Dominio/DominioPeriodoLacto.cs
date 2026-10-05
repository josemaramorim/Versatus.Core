using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioPeriodoLacto.cs (legado)
// Tabela: FINDOMINIOPERIODOLANCTO (17 colunas) — PK (IDFINDOMINIOPERIODOLANCTO, IDGLOFILIAL).
// Mapa: analysis/E2-dominio.md §2.3. Nome da classe mantido como no legado ("Lacto").
//
// POCO SÓ DE DADOS. A gravação com movimento financeiro, cheques e saldo do domínio é
// OP-E2-10 (DominioPeriodoLactoService, E2-T05). ValorInformado e Status do legado não são
// colunas — ficam nos parâmetros da operação.
public class DominioPeriodoLacto
{
    public int IdDominioPeriodoLacto { get; set; }
    public int IdFilial { get; set; }

    /// <summary>Período que entrega; nulo na abertura do domínio padrão.</summary>
    public int? IdPeriodoOrigem { get; set; }

    /// <summary>Período que recebe; nulo no fechamento do domínio padrão.</summary>
    public int? IdPeriodoDestino { get; set; }

    /// <summary>Abertura / Fechamento / Suprimento (IDTIPOLANCAMENTO).</summary>
    public LanctoDominioPeriodoTipo TipoLancto { get; set; }

    /// <summary>Forma do lançamento (IDTIPOFORMA).</summary>
    public FormaPagtoTipo TipoForma { get; set; }

    /// <summary>FK para FINCHEQUERECEBIDO (E9).</summary>
    public int? IdChequeRecebido { get; set; }

    /// <summary>
    /// Em dinheiro: caixa do lançamento. Em cheque de empresa: caixa do talão — junto com
    /// <see cref="Cheque"/> forma a FK para FINTALAOCHEQUE (E9), não para FINCAIXABANCO.
    /// </summary>
    public int? IdCaixaBanco { get; set; }

    /// <summary>Número da folha do talão (cheque de empresa).</summary>
    public int? Cheque { get; set; }

    /// <summary>Data e hora do lançamento (DATAHORALANCTO).</summary>
    public DateTime DataLancto { get; set; }

    public decimal? Valor { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

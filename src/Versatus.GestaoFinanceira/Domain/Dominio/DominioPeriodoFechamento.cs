using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioPeriodoFechamento.cs (legado)
// Tabela: FINDOMINIOPERIODOFECHAMENTO (12 colunas) — PK (IDFINDOMINIOPERIODOFECHAMENTO,
// IDGLOFILIAL); índice UNIQUE (IDGLOFILIAL, IDFINDOMINIOPERIODO, IDTIPOFORMA): uma linha por
// forma por período. Mapa: analysis/E2-dominio.md §2.4. Item do agregado DominioPeriodo.
//
// POCO SÓ DE DADOS. Valores vêm do resumo do fechamento (OP-E2-04, CALC-E2-02).
public class DominioPeriodoFechamento
{
    public int IdDominioPeriodoFechamento { get; set; }
    public int IdFilial { get; set; }
    public int IdDominioPeriodo { get; set; }

    /// <summary>Forma de pagamento (IDTIPOFORMA).</summary>
    public FormaPagtoTipo TipoForma { get; set; }

    /// <summary>Valor calculado pelo sistema (coluna CALCULADO).</summary>
    public decimal ValorCalculado { get; set; }

    /// <summary>Valor conferido/informado pelo usuário (coluna INFORMADO).</summary>
    public decimal ValorInformado { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

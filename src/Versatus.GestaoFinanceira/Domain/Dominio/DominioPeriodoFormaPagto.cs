using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/DominioPeriodoFormaPagto.cs (legado,
// ObjectGenerated). Tabela: FINDOMINIOPERIODOFORMAPAGTO (10 colunas) — PK
// (IDFINDOMINIOPERIODOFORMAPAGTO, IDGLOFILIAL). Mapa: analysis/E2-dominio.md §2.5.
//
// POCO SÓ DE DADOS. Gerado por outros processos (IDadosPeriodoFormaPagto — OP-E1-01); montagem
// e saldo do domínio em OP-E2-12 / CALC-E2-06. Sem colunas de auditoria.
public class DominioPeriodoFormaPagto
{
    public int IdDominioPeriodoFormaPagto { get; set; }
    public int IdFilial { get; set; }
    public int IdDominioPeriodo { get; set; }

    /// <summary>FK para GLOFORMAPAGAMENTO (MOD-02).</summary>
    public int IdFormaPagamento { get; set; }

    public decimal? Credito { get; set; }
    public decimal? Debito { get; set; }

    public DateTime Data { get; set; }

    /// <summary>Hora em texto (varchar(8)), como o legado grava (DateTime.Now.ToLongTimeString()).</summary>
    public string Hora { get; set; } = string.Empty;

    // Origem geradora (ObjectGenerated)
    public int IdOrigem { get; set; }
    public ProcessoOrigem IdProcessoOrigem { get; set; }
}

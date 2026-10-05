using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/Consultas/ResumoPeriodoFormaPagtoConsulta.cs
// (ResumoDominioPeriodoFormaPagtoConsulta). View: VWRESUMODOMINIOPERIODOFORMAPAGTO (6 colunas)
// — somente leitura, sem chave. Mapa: analysis/E2-dominio.md §2.8.
//
// Só os dados da view. Saldo, ValorCalculado, ValorConferencia e Diferenca (CALC-E2-02/03) e a
// contagem do fechamento (herdada de FechamentoCaixaBase no legado) ficam nos serviços do
// fechamento (E2-T05/E2-T08).
public class ResumoDominioPeriodoFormaPagto
{
    public int IdFilial { get; set; }
    public int IdDominioPeriodo { get; set; }
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Tipo da forma de pagamento (IDTIPOFORMAPAGAMENTO).</summary>
    public FormaPagtoTipo TipoFormaPagto { get; set; }

    /// <summary>SUM(CREDITO) — a view expõe numeric(38,2).</summary>
    public decimal? Credito { get; set; }

    /// <summary>SUM(DEBITO) — a view expõe numeric(38,2).</summary>
    public decimal? Debito { get; set; }
}

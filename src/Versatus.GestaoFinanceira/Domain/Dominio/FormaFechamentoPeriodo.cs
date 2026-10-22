using Versatus.GestaoFinanceira.Domain.Bases;

namespace Versatus.GestaoFinanceira.Domain.Dominio;

// Origem: servidor/objeto de negócio/gestao.financeira/Consultas/ResumoPeriodoFormaPagtoConsulta.cs
// (ResumoDominioPeriodoFormaPagtoConsulta : FechamentoCaixaBase). Linha do fechamento do período por
// forma de pagamento — NÃO é persistida (vem da view VWRESUMODOMINIOPERIODOFORMAPAGTO e do que o usuário
// informa). Ao fechar, vira FINDOMINIOPERIODOFECHAMENTO + lançamentos (OP-E2-04).
//
// POCO SÓ DE DADOS. Saldo, ValorCalculado, ValorConferencia e Diferenca são calculados pelo
// FechamentoPeriodoService (CALC-E2-02/03).
public class FormaFechamentoPeriodo : FechamentoCaixaBase
{
    public int IdDominioPeriodo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal? Credito { get; set; }
    public decimal? Debito { get; set; }

    /// <summary>Arredondar(Credito − Debito, 2) — CALC-E2-02.</summary>
    public decimal Saldo { get; set; }

    /// <summary>Arredondar(ValorConferencia − ValorCalculado, 2) — CALC-E2-03.</summary>
    public decimal Diferenca { get; set; }
}

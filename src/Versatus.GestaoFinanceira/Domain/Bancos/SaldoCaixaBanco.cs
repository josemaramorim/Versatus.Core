namespace Versatus.GestaoFinanceira.Domain.Bancos;

// Origem: servidor/objeto de negócio/gestao.financeira/SaldoCaixaBanco.cs (legado)
// Tabela: FINSALDOCAIXABANCO (10 colunas) — PK (IDFINCAIXABANCO, IDGLOFILIAL, DATASALDO).
// Mapa: analysis/E3-caixa-banco.md §2.4.
//
// POCO SÓ DE DADOS. Saldo / SaldoConciliado do legado são propriedades calculadas, não
// colunas (CALC-E3-01/02); a consulta por data/tipo é OP-E3-08 / CALC-E3-03. Ficam no
// serviço do E3-T05, com paridade no E3-T09.
public class SaldoCaixaBanco
{
    public int IdCaixaBanco { get; set; }
    public int IdFilial { get; set; }
    public DateTime DataSaldo { get; set; }

    public decimal? SaldoAnterior { get; set; }
    public decimal? TotalDebito { get; set; }
    public decimal? TotalCredito { get; set; }
    public bool Conferido { get; set; }

    public decimal? SaldoAnteriorConciliado { get; set; }
    public decimal? TotalDebitoConciliado { get; set; }
    public decimal? TotalCreditoConciliado { get; set; }
}

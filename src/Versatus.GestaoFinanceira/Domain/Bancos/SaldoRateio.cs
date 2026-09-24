namespace Versatus.GestaoFinanceira.Domain.Bancos;

// Origem: servidor/objeto de negócio/gestao.financeira/SaldoRateio.cs (legado)
// Tabela: FINSALDORATEIO (11 colunas) — PK (IDGLOFILIAL, IDFINCLASSE, IDGLOCENTROCUSTO,
// IDGLOPROJETOS, DATASALDO). Mapa: analysis/E3-caixa-banco.md §2.5.
//
// As 3 dimensões de rateio fazem parte da PK mas são anuláveis no schema — preservadas como
// int? (Regra 4). O tratamento no EF é a DÚVIDA-E3-1, resolvida no mapping (E3-T03).
//
// POCO SÓ DE DADOS. SaldoEconomico / SaldoFinanceiro do legado são calculados (8 casas —
// CALC-E3-04), não colunas; ficam no serviço (E3-T05) com paridade no E3-T09.
public class SaldoRateio
{
    public int IdFilial { get; set; }

    /// <summary>FK lógica para FINCLASSE (IDFINCLASSE).</summary>
    public int? IdClasse { get; set; }

    /// <summary>FK lógica para o centro de custo (MOD-02) — IDGLOCENTROCUSTO.</summary>
    public int? IdCentroCusto { get; set; }

    /// <summary>FK lógica para o projeto (MOD-02) — IDGLOPROJETOS.</summary>
    public int? IdProjeto { get; set; }

    public DateTime DataSaldo { get; set; }

    public decimal? SaldoAnteriorEconomico { get; set; }
    public decimal? TotalCreditoEconomico { get; set; }
    public decimal? TotalDebitoEconomico { get; set; }

    public decimal? SaldoAnteriorFinanceiro { get; set; }
    public decimal? TotalCreditoFinanceiro { get; set; }
    public decimal? TotalDebitoFinanceiro { get; set; }
}

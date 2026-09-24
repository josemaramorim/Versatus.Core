using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINSALDORATEIO (11 colunas) — analysis/E3-caixa-banco.md §2.5.
//
// DÚVIDA-E3-1 resolvida (E3-T03, consulta a sys.indexes / INFORMATION_SCHEMA em 2026-09-24):
// a tabela NÃO tem PRIMARY KEY — só o índice UNIQUE IDX_FINSALDORATEIO (IDGLOFILIAL,
// IDFINCLASSE, IDGLOCENTROCUSTO, IDGLOPROJETOS, DATASALDO), com as 3 dimensões anuláveis.
// Linhas com dimensão NULL são esperadas (o legado compara "IS NULL AND IS NULL" em
// acesso.global/Saldo/Helpers/SaldoRateioHelper.cs). Uma chave EF sobre colunas anuláveis
// falharia ao materializar essas linhas, então a entidade é mapeada SEM CHAVE (só leitura).
// Fiel ao legado: quem grava FINSALDORATEIO é o recálculo de saldo (SQL direto no
// SaldoRateioHelper); a classe SaldoRateio do financeiro apenas lê (ObjectPersist.Retornar).
public class SaldoRateioMapping : IEntityTypeConfiguration<SaldoRateio>
{
    public void Configure(EntityTypeBuilder<SaldoRateio> builder)
    {
        builder.ToTable("FINSALDORATEIO");

        builder.HasNoKey();

        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").IsRequired();
        builder.Property(x => x.IdClasse).HasColumnName("IDFINCLASSE");
        builder.Property(x => x.IdCentroCusto).HasColumnName("IDGLOCENTROCUSTO");
        builder.Property(x => x.IdProjeto).HasColumnName("IDGLOPROJETOS");
        builder.Property(x => x.DataSaldo).HasColumnName("DATASALDO").HasColumnType("datetime").IsRequired();

        builder.Property(x => x.SaldoAnteriorEconomico).HasColumnName("SALDOANTERIORECONOMICO").HasPrecision(23, 8);
        builder.Property(x => x.TotalCreditoEconomico).HasColumnName("TOTALCREDITOECONOMICO").HasPrecision(23, 8);
        builder.Property(x => x.TotalDebitoEconomico).HasColumnName("TOTALDEBITOECONOMICO").HasPrecision(23, 8);
        builder.Property(x => x.SaldoAnteriorFinanceiro).HasColumnName("SALDOANTERIORFINANCEIRO").HasPrecision(23, 8);
        builder.Property(x => x.TotalCreditoFinanceiro).HasColumnName("TOTALCREDITOFINANCEIRO").HasPrecision(23, 8);
        builder.Property(x => x.TotalDebitoFinanceiro).HasColumnName("TOTALDEBITOFINANCEIRO").HasPrecision(23, 8);
    }
}

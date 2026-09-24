using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINSALDOCAIXABANCO (10 colunas) — analysis/E3-caixa-banco.md §2.4.
public class SaldoCaixaBancoMapping : IEntityTypeConfiguration<SaldoCaixaBanco>
{
    public void Configure(EntityTypeBuilder<SaldoCaixaBanco> builder)
    {
        builder.ToTable("FINSALDOCAIXABANCO");

        // PK_FINSALDOCAIXABANCO (IDFINCAIXABANCO, IDGLOFILIAL, DATASALDO)
        builder.HasKey(x => new { x.IdCaixaBanco, x.IdFilial, x.DataSaldo });

        builder.Property(x => x.IdCaixaBanco).HasColumnName("IDFINCAIXABANCO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();
        builder.Property(x => x.DataSaldo).HasColumnName("DATASALDO").HasColumnType("datetime").ValueGeneratedNever();

        // FK_FINSALDOCAIXABANCO_FINCAIXABANCO
        builder.HasOne<CaixaBanco>()
            .WithMany()
            .HasForeignKey(x => new { x.IdCaixaBanco, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SaldoAnterior).HasColumnName("SALDOANTERIOR").HasPrecision(23, 8);
        builder.Property(x => x.TotalDebito).HasColumnName("TOTALDEBITO").HasPrecision(23, 8);
        builder.Property(x => x.TotalCredito).HasColumnName("TOTALCREDITO").HasPrecision(23, 8);
        builder.Property(x => x.Conferido).HasColumnName("CONFERIDO").IsRequired();
        builder.Property(x => x.SaldoAnteriorConciliado).HasColumnName("SALDOANTERIORCONCILIADO").HasPrecision(23, 8);
        builder.Property(x => x.TotalDebitoConciliado).HasColumnName("TOTALDEBITOCONCILIADO").HasPrecision(23, 8);
        builder.Property(x => x.TotalCreditoConciliado).HasColumnName("TOTALCREDITOCONCILIADO").HasPrecision(23, 8);
    }
}

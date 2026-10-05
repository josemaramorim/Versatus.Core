using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIOPERIODOFORMAPAGTO (10 colunas) — analysis/E2-dominio.md §2.5.
public class DominioPeriodoFormaPagtoMapping : IEntityTypeConfiguration<DominioPeriodoFormaPagto>
{
    public void Configure(EntityTypeBuilder<DominioPeriodoFormaPagto> builder)
    {
        builder.ToTable("FINDOMINIOPERIODOFORMAPAGTO");

        // PK_FINDOMINIOPERIODOFORMAPAGTO (IDFINDOMINIOPERIODOFORMAPAGTO, IDGLOFILIAL)
        builder.HasKey(x => new { x.IdDominioPeriodoFormaPagto, x.IdFilial });

        builder.Property(x => x.IdDominioPeriodoFormaPagto).HasColumnName("IDFINDOMINIOPERIODOFORMAPAGTO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        // FK_FINDOMINIOPERIODOFORMAPAGTO_FINDOMINIOPERIODO
        builder.Property(x => x.IdDominioPeriodo).HasColumnName("IDFINDOMINIOPERIODO").IsRequired();
        builder.HasOne<DominioPeriodo>()
            .WithMany()
            .HasForeignKey(x => new { x.IdDominioPeriodo, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        // FK_FINDOMINIOPERIODOFORMAPAGTO_GLOFORMAPAGAMENTO — MOD-02, FK lógica por int (Artigo VIII).
        builder.Property(x => x.IdFormaPagamento).HasColumnName("IDGLOFORMAPAGAMENTO").IsRequired();

        builder.Property(x => x.Credito).HasColumnName("CREDITO").HasPrecision(23, 8);
        builder.Property(x => x.Debito).HasColumnName("DEBITO").HasPrecision(23, 8);
        builder.Property(x => x.Data).HasColumnName("DATA").HasColumnType("datetime").IsRequired();

        builder.Property(x => x.Hora)
            .HasColumnName("HORA")
            .HasMaxLength(8)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.IdOrigem).HasColumnName("IDORIGEM").IsRequired();
        builder.Property(x => x.IdProcessoOrigem)
            .HasColumnName("IDPROCESSOORIGEM")
            .HasConversion<int>()
            .IsRequired();
    }
}

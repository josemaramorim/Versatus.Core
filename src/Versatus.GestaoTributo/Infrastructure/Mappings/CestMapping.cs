using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class CestMapping : IEntityTypeConfiguration<Cest>
{
    public void Configure(EntityTypeBuilder<Cest> builder)
    {
        builder.ToTable("TRBCEST");

        builder.HasKey(c => c.IdCest);

        builder.Property(c => c.IdCest)
            .HasColumnName("IDTRBCEST")
            .ValueGeneratedNever();

        builder.Property(c => c.IdCestSegmento)
            .HasColumnName("IDTRBCESTSEGMENTO")
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(c => c.Codigo)
            .HasColumnName("CODIGO")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(c => c.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.PercentualMva)
            .HasColumnName("PERCENTUALMVA")
            .HasColumnType("numeric(18,4)");

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(c => c.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(c => c.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(c => c.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(c => c.Segmento)
            .WithMany()
            .HasForeignKey(c => c.IdCestSegmento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

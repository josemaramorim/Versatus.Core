using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class CestSegmentoMapping : IEntityTypeConfiguration<CestSegmento>
{
    public void Configure(EntityTypeBuilder<CestSegmento> builder)
    {
        builder.ToTable("TRBCESTSEGMENTO");

        builder.HasKey(c => c.IdCestSegmento);

        builder.Property(c => c.IdCestSegmento)
            .HasColumnName("IDTRBCESTSEGMENTO")
            .ValueGeneratedNever();

        builder.Property(c => c.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(150)
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

        builder.Property(c => c.IdCestOutros)
            .HasColumnName("IDTRBCESTOUTROS");
    }
}

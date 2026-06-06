using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class PartilhaICMSVigenciaMapping : IEntityTypeConfiguration<PartilhaICMSVigencia>
{
    public void Configure(EntityTypeBuilder<PartilhaICMSVigencia> builder)
    {
        builder.ToTable("TRBPARTILHAICMSVIGENCIA");

        builder.HasKey(p => p.IdPartilhaICMSVigencia);

        builder.Property(p => p.IdPartilhaICMSVigencia)
            .HasColumnName("IDTRBPARTILHAICMSVIGENCIA")
            .ValueGeneratedNever();

        builder.Property(p => p.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.VigenciaInicio)
            .HasColumnName("VIGENCIAINICIO")
            .IsRequired();

        builder.Property(p => p.VigenciaFim)
            .HasColumnName("VIGENCIAFIM");

        builder.Property(p => p.PercentualPartilhaICMS)
            .HasColumnName("PERCENTUALPARTILHAICMS")
            .HasPrecision(8, 4)
            .IsRequired();
    }
}

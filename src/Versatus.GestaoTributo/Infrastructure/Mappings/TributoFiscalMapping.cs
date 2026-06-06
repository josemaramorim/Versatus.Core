using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class TributoFiscalMapping : IEntityTypeConfiguration<TributoFiscal>
{
    public void Configure(EntityTypeBuilder<TributoFiscal> builder)
    {
        builder.ToTable("TRBTRIBUTOFISCAL");

        builder.HasKey(t => t.IdTributoFiscal);

        builder.Property(t => t.IdTributoFiscal)
            .HasColumnName("IDTRBTRIBUTOFISCAL")
            .ValueGeneratedNever();

        builder.Property(t => t.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Sigla)
            .HasColumnName("SIGLA")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(t => t.IdTipoTributo)
            .HasColumnName("IDTIPOTRIBUTO")
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class TipoEnumeradoMapping : IEntityTypeConfiguration<TipoEnumerado>
{
    public void Configure(EntityTypeBuilder<TipoEnumerado> builder)
    {
        builder.ToTable("GloTipoEnumerado");

        builder.HasKey(t => t.IdTipoEnumerado);

        builder.Property(t => t.IdTipoEnumerado)
            .HasColumnName("IdGloTipoEnumerado")
            .ValueGeneratedNever();

        builder.Property(t => t.IdTipoEnumeradoPai)
            .HasColumnName("IdGloTipoEnumeradoPai");

        builder.Property(t => t.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.Ordem)
            .HasColumnName("Ordem")
            .IsRequired();
    }
}

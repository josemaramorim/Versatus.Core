using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloFavoritoMapping : IEntityTypeConfiguration<GloFavorito>
{
    public void Configure(EntityTypeBuilder<GloFavorito> builder)
    {
        builder.ToTable("GloFavorito");
        builder.HasKey(x => x.IdFavorito);
        builder.Property(x => x.IdFavorito).HasColumnName("IdGloFavorito");
        builder.Property(x => x.IdRotina).HasColumnName("IdGloRotina");
        builder.HasOne(x => x.Rotina)
            .WithMany()
            .HasForeignKey(x => x.IdRotina)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

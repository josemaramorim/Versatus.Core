using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloMenuMenuMapping : IEntityTypeConfiguration<GloMenuMenu>
{
    public void Configure(EntityTypeBuilder<GloMenuMenu> builder)
    {
        builder.ToTable("GloMenuMenu");
        builder.HasKey(x => new { x.IdMenu, x.IdMenuPai });
        builder.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
        builder.Property(x => x.IdMenuPai).HasColumnName("IdGloMenuPai");
        builder.HasOne(x => x.Menu)
            .WithMany()
            .HasForeignKey(x => x.IdMenu)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.MenuPai)
            .WithMany()
            .HasForeignKey(x => x.IdMenuPai)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

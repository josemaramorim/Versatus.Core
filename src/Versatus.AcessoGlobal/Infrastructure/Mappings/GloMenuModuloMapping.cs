using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloMenuModuloMapping : IEntityTypeConfiguration<GloMenuModulo>
{
    public void Configure(EntityTypeBuilder<GloMenuModulo> builder)
    {
        builder.ToTable("GloMenuModulo");
        builder.HasKey(x => new { x.IdMenu, x.IdModulo });
        builder.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
        builder.Property(x => x.IdModulo).HasColumnName("IdGloModulo");
        builder.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.IdMenu);
        builder.HasOne(x => x.Modulo).WithMany().HasForeignKey(x => x.IdModulo);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloMenuRotinaMapping : IEntityTypeConfiguration<GloMenuRotina>
{
    public void Configure(EntityTypeBuilder<GloMenuRotina> builder)
    {
        builder.ToTable("GloMenuRotina");
        builder.HasKey(x => new { x.IdMenu, x.IdRotina });
        builder.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
        builder.Property(x => x.IdRotina).HasColumnName("IdGloRotina");
        builder.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.IdMenu);
        builder.HasOne(x => x.Rotina).WithMany().HasForeignKey(x => x.IdRotina);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloMenuMapping : IEntityTypeConfiguration<GloMenu>
{
    public void Configure(EntityTypeBuilder<GloMenu> builder)
    {
        builder.ToTable("GloMenu");
        builder.HasKey(x => x.IdMenu);
        builder.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
        builder.Property(x => x.Descricao).HasMaxLength(200).IsRequired();
    }
}

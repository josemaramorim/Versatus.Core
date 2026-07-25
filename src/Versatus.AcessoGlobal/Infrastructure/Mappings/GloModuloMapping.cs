using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloModuloMapping : IEntityTypeConfiguration<GloModulo>
{
    public void Configure(EntityTypeBuilder<GloModulo> builder)
    {
        builder.ToTable("GloModulo");
        builder.HasKey(x => x.IdModulo);
        builder.Property(x => x.IdModulo).HasColumnName("IdGloModulo");
        builder.Property(x => x.Nome).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TipoModulo).HasColumnName("IdTipoModulo");
        builder.Property(x => x.PrefixoRota).HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.CorHex).HasMaxLength(7).IsRequired(false);
        builder.Property(x => x.IconeMui).HasMaxLength(50).IsRequired(false);
    }
}

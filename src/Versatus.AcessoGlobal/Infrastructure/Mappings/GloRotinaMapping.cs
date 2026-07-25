using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GloRotinaMapping : IEntityTypeConfiguration<GloRotina>
{
    public void Configure(EntityTypeBuilder<GloRotina> builder)
    {
        builder.ToTable("GloRotina");
        builder.HasKey(x => x.IdRotina);
        builder.Property(x => x.IdRotina).HasColumnName("IdGloRotina");
        builder.Property(x => x.TipoRotina).HasColumnName("IdTipoRotina");
        builder.Property(x => x.Objeto).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.RotaWeb).HasMaxLength(100).IsRequired(false);
    }
}

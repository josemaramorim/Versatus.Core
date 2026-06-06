using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class GrupoTributarioInventarioICMSMapping : IEntityTypeConfiguration<GrupoTributarioInventarioICMS>
{
    public void Configure(EntityTypeBuilder<GrupoTributarioInventarioICMS> builder)
    {
        builder.ToTable("TRBGRUPOTRIBUTARIOINVENTARIOICMS");

        builder.HasKey(g => g.IdGrupoTributarioInventarioICMS);

        builder.Property(g => g.IdGrupoTributarioInventarioICMS)
            .HasColumnName("IDTRBGRUPOTRIBUTARIOINVENTARIOICMS")
            .ValueGeneratedNever();

        builder.Property(g => g.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(g => g.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(g => g.Substituicao)
            .HasColumnName("SUBSTITUICAO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        // Auditoria
        builder.Property(g => g.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(g => g.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(g => g.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(g => g.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(g => g.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(g => g.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasMany(g => g.Grupos)
            .WithOne(g => g.GrupoTributarioInventario)
            .HasForeignKey(g => g.IdGrupoTributarioInventarioICMS)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

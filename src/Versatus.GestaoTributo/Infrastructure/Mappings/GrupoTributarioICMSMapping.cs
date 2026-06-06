using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class GrupoTributarioICMSMapping : IEntityTypeConfiguration<GrupoTributarioICMS>
{
    public void Configure(EntityTypeBuilder<GrupoTributarioICMS> builder)
    {
        builder.ToTable("TRBGRUPOTRIBUTARIOICMS");

        builder.HasKey(g => g.IdGrupoTributarioICMS);

        builder.Property(g => g.IdGrupoTributarioICMS)
            .HasColumnName("IDTRBGRUPOTRIBUTARIOICMS")
            .ValueGeneratedNever();

        builder.Property(g => g.IdGrupoTributarioInventarioICMS)
            .HasColumnName("IDTRBGRUPOTRIBUTARIOINVENTARIOICMS")
            .IsRequired();

        builder.Property(g => g.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(g => g.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(g => g.IdDefinicaoTributaria)
            .HasColumnName("IDDEFINICAOTRIBUTARIA")
            .HasConversion<int>()
            .IsRequired();

        // Auditoria
        builder.Property(g => g.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(g => g.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(g => g.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(g => g.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(g => g.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(g => g.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(g => g.GrupoTributarioInventario)
            .WithMany(gi => gi.Grupos)
            .HasForeignKey(g => g.IdGrupoTributarioInventarioICMS)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

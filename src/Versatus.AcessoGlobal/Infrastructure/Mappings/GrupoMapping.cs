using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Organization;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GrupoMapping : IEntityTypeConfiguration<Grupo>
{
    public void Configure(EntityTypeBuilder<Grupo> builder)
    {
        builder.ToTable("GloGrupo");

        builder.HasKey(g => g.IdGrupo);

        builder.Property(g => g.IdGrupo)
            .HasColumnName("IdGloGrupo")
            .ValueGeneratedNever();

        builder.Property(g => g.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.Ativo).HasColumnName("Ativo");

        // Auditoria
        builder.Property(g => g.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(g => g.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(g => g.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(g => g.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(g => g.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(g => g.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

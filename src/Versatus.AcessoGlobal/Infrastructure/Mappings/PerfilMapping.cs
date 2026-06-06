using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Security;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class PerfilMapping : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("GloPerfil");

        builder.HasKey(p => p.IdPerfil);

        builder.Property(p => p.IdPerfil)
            .HasColumnName("IdGloPerfil")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(p => p.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Administrador)
            .HasColumnName("Administrador")
            .IsRequired();

        builder.Property(p => p.UsaDominioFinanceiro)
            .HasColumnName("UsaDominioFinanceiro")
            .IsRequired();

        // Auditoria
        builder.Property(p => p.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(p => p.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(p => p.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(p => p.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(p => p.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(p => p.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

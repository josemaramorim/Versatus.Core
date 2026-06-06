using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Security;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("GloUsuario");

        builder.HasKey(u => u.IdUsuario);

        builder.Property(u => u.IdUsuario)
            .HasColumnName("IdGloUsuario")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(u => u.Login)
            .HasColumnName("NomeAcesso")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("Senha")
            .IsRequired();

        builder.Property(u => u.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(u => u.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(u => u.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(u => u.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(u => u.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(u => u.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(u => u.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos Many-to-Many via GloPerfilUsuario
        builder.HasMany(u => u.Perfis)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "GloPerfilUsuario",
                j => j.HasOne<Perfil>()
                      .WithMany()
                      .HasForeignKey("IdGloPerfil")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey("IdGloUsuario")
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("GloPerfilUsuario");
                    j.HasKey("IdGloPerfil", "IdGloUsuario");
                });
    }
}

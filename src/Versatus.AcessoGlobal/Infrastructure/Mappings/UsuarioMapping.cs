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

        builder.Property(u => u.IdFuncionario)
            .HasColumnName("IdGloFuncionario")
            .IsRequired(false);

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
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.IdPerfil)
            .HasColumnName("IdGloPerfil")
            .IsRequired();

        builder.Property(u => u.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        builder.Property(u => u.UltimoLogon)
            .HasColumnName("DataUltimoLogon")
            .IsRequired(false);

        // Auditoria
        builder.Property(u => u.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(u => u.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(u => u.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(u => u.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(u => u.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(u => u.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos
        builder.HasOne(u => u.Perfil)
            .WithMany()
            .HasForeignKey(u => u.IdPerfil)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

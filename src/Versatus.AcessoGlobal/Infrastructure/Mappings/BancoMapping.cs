using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class BancoMapping : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.ToTable("GloBanco");

        builder.HasKey(b => b.IdBanco);

        builder.Property(b => b.IdBanco)
            .HasColumnName("IdGloBanco")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(b => b.Codigo)
            .HasColumnName("CodigoBancoCobranca")
            .IsRequired();

        builder.Property(b => b.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(b => b.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(b => b.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(b => b.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(b => b.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(b => b.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(b => b.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

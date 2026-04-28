using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class PaisMapping : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("GloPais");

        builder.HasKey(p => p.IdPais);

        builder.Property(p => p.IdPais)
            .HasColumnName("IdGloPais")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(p => p.CodigoBACEN)
            .HasColumnName("CodigoBACEN")
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(p => p.Abreviacao)
            .HasColumnName("Abreviacao")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Ativo)
            .HasColumnName("Ativo")
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

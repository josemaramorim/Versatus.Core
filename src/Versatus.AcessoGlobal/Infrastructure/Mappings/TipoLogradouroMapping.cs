using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class TipoLogradouroMapping : IEntityTypeConfiguration<TipoLogradouro>
{
    public void Configure(EntityTypeBuilder<TipoLogradouro> builder)
    {
        builder.ToTable("GloTipoLogradouro");

        builder.HasKey(t => t.IdTipoLogradouro);

        builder.Property(t => t.IdTipoLogradouro)
            .HasColumnName("IdGloTipoLogradouro")
            .ValueGeneratedNever();

        builder.Property(t => t.Abreviacao)
            .HasColumnName("Abreviacao")
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(t => t.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Ativo).HasColumnName("Ativo");

        // Auditoria
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(e => e.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(e => e.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(p => p.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(p => p.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(p => p.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

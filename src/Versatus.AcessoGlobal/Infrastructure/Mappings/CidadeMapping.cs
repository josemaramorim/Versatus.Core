using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class CidadeMapping : IEntityTypeConfiguration<Cidade>
{
    public void Configure(EntityTypeBuilder<Cidade> builder)
    {
        builder.ToTable("GloCidade");

        builder.HasKey(c => c.IdCidade);

        builder.Property(c => c.IdCidade)
            .HasColumnName("IdGloCidade")
            .ValueGeneratedNever();

        builder.Property(c => c.IdPais)
            .HasColumnName("IdGloPais")
            .IsRequired();

        builder.Property(c => c.SiglaEstado)
            .HasColumnName("Uf")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CEP)
            .HasColumnName("Cep")
            .HasMaxLength(8);

        builder.Property(c => c.CodigoIBGE)
            .HasColumnName("CodigoIBGE")
            .HasMaxLength(7);

        builder.Property(c => c.CodigoCidade).HasColumnName("CodigoCidade");
        builder.Property(c => c.Ativo).HasColumnName("Ativo");
        builder.Property(c => c.Latitude).HasColumnName("Latitude");
        builder.Property(c => c.Longitude).HasColumnName("Longitude");

        // Relacionamentos
        builder.HasOne(c => c.Pais)
            .WithMany()
            .HasForeignKey(c => c.IdPais)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com Estado via Sigla (Uf)
        // Nota: Assumimos que Uf é único em GloEstado para esta relação funcionar.
        builder.HasOne(c => c.Estado)
            .WithMany()
            .HasForeignKey(c => c.SiglaEstado)
            .HasPrincipalKey(e => e.Sigla)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoria
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(e => e.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(e => e.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(p => p.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(p => p.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(p => p.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

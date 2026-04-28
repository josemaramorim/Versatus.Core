using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class BairroMapping : IEntityTypeConfiguration<Bairro>
{
    public void Configure(EntityTypeBuilder<Bairro> builder)
    {
        builder.ToTable("GloBairro");

        builder.HasKey(b => b.IdBairro);

        builder.Property(b => b.IdBairro)
            .HasColumnName("IdBairro")
            .ValueGeneratedNever();

        builder.Property(b => b.IdCidade)
            .HasColumnName("IdCidade")
            .IsRequired();

        builder.Property(b => b.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Ativo).HasColumnName("Ativo");

        // Relacionamentos
        builder.HasOne(b => b.Cidade)
            .WithMany()
            .HasForeignKey(b => b.IdCidade)
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

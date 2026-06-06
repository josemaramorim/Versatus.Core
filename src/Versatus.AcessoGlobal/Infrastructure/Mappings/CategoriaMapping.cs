using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Classification;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class CategoriaMapping : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("GloCategoria");

        builder.HasKey(c => c.IdCategoria);

        builder.Property(c => c.IdCategoria)
            .HasColumnName("IdGloCategoria")
            .ValueGeneratedNever();

        builder.Property(c => c.IdCategoriaPai)
            .HasColumnName("IdGloCategoriaPai");

        builder.Property(c => c.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.IdSinteticoAnalitico)
            .HasColumnName("IdAnaliticoSintetico")
            .IsRequired();

        builder.Property(c => c.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(c => c.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(c => c.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos
        builder.HasOne(c => c.CategoriaPai)
            .WithMany(c => c.SubCategorias)
            .HasForeignKey(c => c.IdCategoriaPai)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class FornecedorMapping : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("GloFornecedor");

        builder.HasKey(f => f.IdFornecedor);

        builder.Property(f => f.IdFornecedor)
            .HasColumnName("IdGloFornecedor")
            .ValueGeneratedNever();

        builder.Property(f => f.CodigoAlternativo).HasColumnName("CodigoAlternativo").HasMaxLength(20);
        builder.Property(f => f.ContaContabil).HasColumnName("ContaContabil").HasMaxLength(20);
        builder.Property(f => f.Ativo).HasColumnName("Ativo").IsRequired();
        builder.Property(f => f.IsFornecedorCotacao).HasColumnName("FornecedorCotacao").IsRequired();

        builder.Property(f => f.IdCategoria).HasColumnName("IdGloCategoria");
        builder.Property(f => f.IdCondicaoPagamento).HasColumnName("IdGloCondicaoPagamento");

        // Auditoria
        builder.Property(f => f.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(f => f.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(f => f.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(f => f.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(f => f.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(f => f.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamento 1:1 com Entidade
        builder.HasOne(f => f.Entidade)
            .WithOne()
            .HasForeignKey<Fornecedor>(f => f.IdFornecedor);

        builder.HasOne(f => f.Categoria)
            .WithMany()
            .HasForeignKey(f => f.IdCategoria);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class EntidadeEnderecoMapping : IEntityTypeConfiguration<EntidadeEndereco>
{
    public void Configure(EntityTypeBuilder<EntidadeEndereco> builder)
    {
        builder.ToTable("GloEntidadeEndereco");

        builder.HasKey(e => e.IdEntidadeEndereco);

        builder.Property(e => e.IdEntidadeEndereco)
            .HasColumnName("IdGloEntidadeEndereco")
            .ValueGeneratedNever();

        builder.Property(e => e.IdEntidade)
            .HasColumnName("IdGloEntidade")
            .IsRequired();

        builder.Property(e => e.IdCidade)
            .HasColumnName("IdGloCidade")
            .IsRequired();

        builder.Property(e => e.IdTipoLogradouro)
            .HasColumnName("IdGloTipoLogradouro")
            .IsRequired();

        builder.Property(e => e.IdBairro)
            .HasColumnName("IdGloBairro");

        builder.Property(e => e.Numero).HasColumnName("Numero");
        builder.Property(e => e.Cep).HasColumnName("Cep").HasMaxLength(8);
        builder.Property(e => e.Logradouro).HasColumnName("Logradouro").HasMaxLength(100);
        builder.Property(e => e.Complemento).HasColumnName("Complemento").HasMaxLength(100);
        builder.Property(e => e.CaixaPostal).HasColumnName("CaixaPostal").HasMaxLength(20);
        
        builder.Property(e => e.Ativo).HasColumnName("Ativo").IsRequired();
        builder.Property(e => e.Padrao).HasColumnName("Padrao").IsRequired();
        builder.Property(e => e.TipoEndereco).HasColumnName("IdTipoEndereco").IsRequired();

        // Relacionamentos
        builder.HasOne(e => e.Entidade)
            .WithMany(ent => ent.Enderecos)
            .HasForeignKey(e => e.IdEntidade)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Cidade)
            .WithMany()
            .HasForeignKey(e => e.IdCidade)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Bairro)
            .WithMany()
            .HasForeignKey(e => e.IdBairro)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TipoLogradouro)
            .WithMany()
            .HasForeignKey(e => e.IdTipoLogradouro)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

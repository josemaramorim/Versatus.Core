using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class AplicacaoProdutoTributoMapping : IEntityTypeConfiguration<AplicacaoProdutoTributo>
{
    public void Configure(EntityTypeBuilder<AplicacaoProdutoTributo> builder)
    {
        builder.ToTable("TRBAPLICACAOPRODUTOTRIBUTO");

        // Chave primária composta
        builder.HasKey(apt => new { apt.IdAplicacaoProduto, apt.IdTributo });

        builder.Property(apt => apt.IdAplicacaoProduto)
            .HasColumnName("IDTRBAPLICACAOPRODUTO")
            .ValueGeneratedNever();

        builder.Property(apt => apt.IdTributo)
            .HasColumnName("IDGLOTRIBUTO")
            .ValueGeneratedNever();

        builder.Property(apt => apt.IdTributacao)
            .HasColumnName("IDTRBTRIBUTACAO")
            .IsRequired();

        // Auditoria
        builder.Property(apt => apt.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(apt => apt.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(apt => apt.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(apt => apt.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(apt => apt.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(apt => apt.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(apt => apt.AplicacaoProduto)
            .WithMany(ap => ap.Tributos)
            .HasForeignKey(apt => apt.IdAplicacaoProduto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(apt => apt.Tributo)
            .WithMany()
            .HasForeignKey(apt => apt.IdTributo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(apt => apt.Tributacao)
            .WithMany()
            .HasForeignKey(apt => apt.IdTributacao)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

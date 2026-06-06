using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class AplicacaoEspecialMapping : IEntityTypeConfiguration<AplicacaoEspecial>
{
    public void Configure(EntityTypeBuilder<AplicacaoEspecial> builder)
    {
        builder.ToTable("TRBAPLICACAOESPECIAL");

        // Chave primária composta
        builder.HasKey(ae => new { ae.IdAplicacaoProduto, ae.IdTributoFormula });

        builder.Property(ae => ae.IdAplicacaoProduto)
            .HasColumnName("IDTRBAPLICACAOPRODUTO")
            .ValueGeneratedNever();

        builder.Property(ae => ae.IdTributoFormula)
            .HasColumnName("IDTRBFORMULA")
            .ValueGeneratedNever();

        builder.Property(ae => ae.SufixoEspecial)
            .HasColumnName("SUFIXOESPECIAL")
            .HasMaxLength(4);

        builder.Property(ae => ae.IdAplicacaoProdutoEspecial)
            .HasColumnName("IDTRBAPLICACAOPRODUTOESPECIAL")
            .IsRequired();

        // Auditoria
        builder.Property(ae => ae.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(ae => ae.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(ae => ae.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(ae => ae.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(ae => ae.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(ae => ae.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(ae => ae.AplicacaoProduto)
            .WithMany(ap => ap.Especiais)
            .HasForeignKey(ae => ae.IdAplicacaoProduto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ae => ae.AplicacaoProdutoEspecial)
            .WithMany()
            .HasForeignKey(ae => ae.IdAplicacaoProdutoEspecial)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

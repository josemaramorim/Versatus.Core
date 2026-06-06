using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class ClassificacaoFiscalTributoFilialMapping : IEntityTypeConfiguration<ClassificacaoFiscalTributoFilial>
{
    public void Configure(EntityTypeBuilder<ClassificacaoFiscalTributoFilial> builder)
    {
        builder.ToTable("TRBCLASSIFICACAOFISCALTRIBUTOFILIAL");

        // Chave composta
        builder.HasKey(tf => new { tf.IdClassificacaoFiscalTributo, tf.IdFilial });

        builder.Property(tf => tf.IdClassificacaoFiscalTributo)
            .HasColumnName("IDTRBCLASSIFICACAOFISCALTRIBUTO")
            .ValueGeneratedNever();

        builder.Property(tf => tf.IdFilial)
            .HasColumnName("IDGLOFILIAL")
            .ValueGeneratedNever();

        builder.Property(tf => tf.Aliquota)
            .HasColumnName("ALIQUOTA")
            .HasColumnType("numeric(18,4)");

        // Relacionamentos
        builder.HasOne(tf => tf.ClassificacaoFiscalTributo)
            .WithMany(t => t.AliquotasFilial)
            .HasForeignKey(tf => tf.IdClassificacaoFiscalTributo)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

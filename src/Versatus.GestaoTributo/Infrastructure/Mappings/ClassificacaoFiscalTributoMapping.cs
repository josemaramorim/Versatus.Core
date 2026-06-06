using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class ClassificacaoFiscalTributoMapping : IEntityTypeConfiguration<ClassificacaoFiscalTributo>
{
    public void Configure(EntityTypeBuilder<ClassificacaoFiscalTributo> builder)
    {
        builder.ToTable("TRBCLASSIFICACAOFISCALTRIBUTO");

        builder.HasKey(t => t.IdClassificacaoFiscalTributo);

        builder.Property(t => t.IdClassificacaoFiscalTributo)
            .HasColumnName("IDTRBCLASSIFICACAOFISCALTRIBUTO")
            .ValueGeneratedNever();

        builder.Property(t => t.IdClassificacaoFiscal)
            .HasColumnName("IDTRBCLASSIFICACAOFISCAL")
            .IsRequired();

        builder.Property(t => t.IdTributoFiscal)
            .HasColumnName("IDTRBTRIBUTOFISCAL")
            .IsRequired();

        builder.Property(t => t.UsaAliquota)
            .HasColumnName("USAALIQUOTA")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(t => t.Aliquota)
            .HasColumnName("ALIQUOTA")
            .HasColumnType("numeric(18,4)");

        builder.Property(t => t.PossuiAliquotaFilial)
            .HasColumnName("POSSUIALIQUOTAFILIAL")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        // Relacionamentos
        builder.HasOne(t => t.ClassificacaoFiscal)
            .WithMany(c => c.Tributos)
            .HasForeignKey(t => t.IdClassificacaoFiscal)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.TributoFiscal)
            .WithMany()
            .HasForeignKey(t => t.IdTributoFiscal)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

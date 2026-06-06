using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class SimplesNacionalMapping : IEntityTypeConfiguration<SimplesNacional>
{
    public void Configure(EntityTypeBuilder<SimplesNacional> builder)
    {
        builder.ToTable("TRBSIMPLESNACIONAL");

        builder.HasKey(s => s.IdSimplesNacional);

        builder.Property(s => s.IdSimplesNacional)
            .HasColumnName("IDTRBSIMPLESNACIONAL")
            .ValueGeneratedNever();

        builder.Property(s => s.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Aliquota)
            .HasColumnName("ALIQUOTA")
            .HasPrecision(8, 4)
            .IsRequired();

        builder.Property(s => s.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        // Auditoria
        builder.Property(s => s.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(s => s.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(s => s.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(s => s.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(s => s.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(s => s.HoraAlteracao).HasColumnName("HORAALTERACAO");
    }
}

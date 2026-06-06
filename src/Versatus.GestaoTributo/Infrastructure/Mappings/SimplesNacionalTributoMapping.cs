using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class SimplesNacionalTributoMapping : IEntityTypeConfiguration<SimplesNacionalTributo>
{
    public void Configure(EntityTypeBuilder<SimplesNacionalTributo> builder)
    {
        builder.ToTable("TRBSIMPLESNACIONALTRIBUTO");

        builder.HasKey(t => t.IdSimplesNacionalTributo);

        builder.Property(t => t.IdSimplesNacionalTributo)
            .HasColumnName("IDTRBSIMPLESNACIONALTRIBUTO")
            .ValueGeneratedNever();

        builder.Property(t => t.IdSimplesNacional)
            .HasColumnName("IDTRBSIMPLESNACIONAL")
            .IsRequired();

        builder.Property(t => t.IdTributoFiscal)
            .HasColumnName("IDTRBTRIBUTOFISCAL")
            .IsRequired();

        builder.Property(t => t.Aliquota)
            .HasColumnName("ALIQUOTA")
            .HasPrecision(8, 4);

        // Auditoria
        builder.Property(t => t.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(t => t.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(t => t.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(t => t.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(t => t.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(t => t.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(t => t.SimplesNacional)
            .WithMany(s => s.Tributos)
            .HasForeignKey(t => t.IdSimplesNacional)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TributoFiscal)
            .WithMany()
            .HasForeignKey(t => t.IdTributoFiscal)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

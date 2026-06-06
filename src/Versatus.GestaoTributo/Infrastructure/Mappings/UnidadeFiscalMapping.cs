using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class UnidadeFiscalMapping : IEntityTypeConfiguration<UnidadeFiscal>
{
    public void Configure(EntityTypeBuilder<UnidadeFiscal> builder)
    {
        builder.ToTable("TRBUNIDADEFISCAL");

        builder.HasKey(u => u.IdUnidadeFiscal);

        builder.Property(u => u.IdUnidadeFiscal)
            .HasColumnName("IDTRBUNIDADEFISCAL")
            .ValueGeneratedNever();

        builder.Property(u => u.SiglaFiscal)
            .HasColumnName("SIGLAFISCAL")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(u => u.DecimaisQuantidade)
            .HasColumnName("DECIMAISQUANTIDADE");

        builder.Property(u => u.DecimaisValor)
            .HasColumnName("DECIMAISVALOR");

        builder.Property(u => u.SiglaFiscalEcf)
            .HasColumnName("SIGLAFISCALECF")
            .HasMaxLength(10);

        // Auditoria
        builder.Property(u => u.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(u => u.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(u => u.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(u => u.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(u => u.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(u => u.HoraAlteracao).HasColumnName("HORAALTERACAO");
    }
}

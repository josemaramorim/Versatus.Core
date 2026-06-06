using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class SituacaoTributariaMapping : IEntityTypeConfiguration<SituacaoTributaria>
{
    public void Configure(EntityTypeBuilder<SituacaoTributaria> builder)
    {
        builder.ToTable("TRBSITUACAOTRIBUTARIA");

        builder.HasKey(s => s.IdSituacaoTributaria);

        builder.Property(s => s.IdSituacaoTributaria)
            .HasColumnName("IDTRBSITUACAOTRIBUTARIA")
            .ValueGeneratedNever();

        builder.Property(s => s.IdTributoFiscal)
            .HasColumnName("IDTRBTRIBUTOFISCAL")
            .IsRequired();

        builder.Property(s => s.IdSituacaoTributariaFiscal)
            .HasColumnName("IDTRBSITUACAOTRIBUTARIAFISCAL");

        builder.Property(s => s.IdSituacaoTributariaFiscalSped)
            .HasColumnName("IDTRBSITUACAOTRIBUTARIAFISCALSPED");

        builder.Property(s => s.IdSituacaoTributariaFiscalSN)
            .HasColumnName("IDTRBSITUACAOTRIBUTARIAFISCALSN");

        builder.Property(s => s.IdSituacaoTributariaFiscalSNSped)
            .HasColumnName("IDTRBSITUACAOTRIBUTARIAFISCALSNSPED");

        builder.Property(s => s.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.SituacaoTributariaCodigo)
            .HasColumnName("SITUACAOTRIBUTARIA")
            .HasMaxLength(3);

        builder.Property(s => s.SituacaoSimplesNacional)
            .HasColumnName("SITUACAOSIMPLESNACIONAL")
            .HasMaxLength(4);

        builder.Property(s => s.SituacaoSped)
            .HasColumnName("SITUACAOSPED")
            .HasMaxLength(3);

        builder.Property(s => s.SituacaoSimplesNacionalSped)
            .HasColumnName("SITUACAOSIMPLESNACIONALSPED")
            .HasMaxLength(4);

        builder.Property(s => s.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(s => s.Substituicao)
            .HasColumnName("SUBSTITUICAO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(s => s.Monofasico)
            .HasColumnName("MONOFASICO")
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

        // Relacionamentos
        builder.HasOne(s => s.TributoFiscal)
            .WithMany()
            .HasForeignKey(s => s.IdTributoFiscal)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

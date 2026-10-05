using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIOPERIODOFECHAMENTO (12 colunas) — analysis/E2-dominio.md §2.4.
// O relacionamento com DominioPeriodo (agregado) é configurado em DominioPeriodoMapping.
public class DominioPeriodoFechamentoMapping : IEntityTypeConfiguration<DominioPeriodoFechamento>
{
    public void Configure(EntityTypeBuilder<DominioPeriodoFechamento> builder)
    {
        builder.ToTable("FINDOMINIOPERIODOFECHAMENTO");

        // PK_FINDOMINIOPERIODOFECHAMENTO (IDFINDOMINIOPERIODOFECHAMENTO, IDGLOFILIAL)
        builder.HasKey(x => new { x.IdDominioPeriodoFechamento, x.IdFilial });

        builder.Property(x => x.IdDominioPeriodoFechamento).HasColumnName("IDFINDOMINIOPERIODOFECHAMENTO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();
        builder.Property(x => x.IdDominioPeriodo).HasColumnName("IDFINDOMINIOPERIODO").IsRequired();

        builder.Property(x => x.TipoForma)
            .HasColumnName("IDTIPOFORMA")
            .HasConversion<int>()
            .IsRequired();

        // IDX_FINDOMINIOPERIODOFECHAMENTO01 — UNIQUE: uma linha por forma por período.
        builder.HasIndex(x => new { x.IdFilial, x.IdDominioPeriodo, x.TipoForma })
            .IsUnique()
            .HasDatabaseName("IDX_FINDOMINIOPERIODOFECHAMENTO01");

        builder.Property(x => x.ValorCalculado).HasColumnName("CALCULADO").HasPrecision(23, 8).IsRequired();
        builder.Property(x => x.ValorInformado).HasColumnName("INFORMADO").HasPrecision(23, 8).IsRequired();

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

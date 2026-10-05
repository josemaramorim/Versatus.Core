using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIOPERIODOLANCTO (17 colunas) — analysis/E2-dominio.md §2.3.
public class DominioPeriodoLactoMapping : IEntityTypeConfiguration<DominioPeriodoLacto>
{
    public void Configure(EntityTypeBuilder<DominioPeriodoLacto> builder)
    {
        builder.ToTable("FINDOMINIOPERIODOLANCTO");

        // PK_FINDOMINIOPERIODOLANCTO (IDFINDOMINIOPERIODOLANCTO, IDGLOFILIAL)
        builder.HasKey(x => new { x.IdDominioPeriodoLacto, x.IdFilial });

        builder.Property(x => x.IdDominioPeriodoLacto).HasColumnName("IDFINDOMINIOPERIODOLANCTO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        // FK_FINDOMINIOPERIODOSUPRIMENTO_FINDOMINIOPERIODO_ORIGEM / _DESTINO
        builder.Property(x => x.IdPeriodoOrigem).HasColumnName("IDFINPERIODOORIGEM");
        builder.HasOne<DominioPeriodo>()
            .WithMany()
            .HasForeignKey(x => new { x.IdPeriodoOrigem, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IdPeriodoDestino).HasColumnName("IDFINPERIODODESTINO");
        builder.HasOne<DominioPeriodo>()
            .WithMany()
            .HasForeignKey(x => new { x.IdPeriodoDestino, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TipoLancto)
            .HasColumnName("IDTIPOLANCAMENTO")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.TipoForma)
            .HasColumnName("IDTIPOFORMA")
            .HasConversion<int>()
            .IsRequired();

        // FK_FINDOMINIOPERIODOSUPRIMENTO_FINCHEQUERECEBIDO e FK_FINDOMINIOPERIODOLANCTO_FINTALAOCHEQUE
        // (IDFINCAIXABANCO, IDGLOFILIAL, CHEQUE): as entidades de cheque são do E9 — até lá,
        // FKs lógicas por int (relacionamento EF entra no E9).
        builder.Property(x => x.IdChequeRecebido).HasColumnName("IDFINCHEQUERECEBIDO");
        builder.Property(x => x.IdCaixaBanco).HasColumnName("IDFINCAIXABANCO");
        builder.Property(x => x.Cheque).HasColumnName("CHEQUE");

        builder.Property(x => x.DataLancto).HasColumnName("DATAHORALANCTO").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.Valor).HasColumnName("VALOR").HasPrecision(23, 8);

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

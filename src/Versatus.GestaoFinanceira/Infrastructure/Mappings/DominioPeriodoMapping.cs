using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIOPERIODO (16 colunas) — analysis/E2-dominio.md §2.2.
public class DominioPeriodoMapping : IEntityTypeConfiguration<DominioPeriodo>
{
    public void Configure(EntityTypeBuilder<DominioPeriodo> builder)
    {
        builder.ToTable("FINDOMINIOPERIODO");

        // PK_FINDOMINIOPERIODO (IDFINDOMINIOPERIODO, IDGLOFILIAL) — sequencial via GeradorSequencialService.
        builder.HasKey(x => new { x.IdDominioPeriodo, x.IdFilial });

        builder.Property(x => x.IdDominioPeriodo).HasColumnName("IDFINDOMINIOPERIODO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        // FK_FINDOMINIOPERIODO_FINDOMINIO (IDFINDOMINIO, IDGLOFILIAL)
        builder.Property(x => x.IdDominio).HasColumnName("IDFINDOMINIO").IsRequired();
        builder.HasOne<Dominio>()
            .WithMany()
            .HasForeignKey(x => new { x.IdDominio, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IdUsuarioFechamento).HasColumnName("IDGLOUSUARIOFECHAMENTO");

        builder.Property(x => x.DataAbertura).HasColumnName("DATAABERTURA").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.HoraAbertura).HasColumnName("DATAHORAABERTURA").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.DataFechamento).HasColumnName("DATAFECHAMENTO").HasColumnType("datetime");
        builder.Property(x => x.HoraFechamento).HasColumnName("DATAHORAFECHAMENTO").HasColumnType("datetime");
        builder.Property(x => x.DataFechamentoTesouraria).HasColumnName("DATAFECHAMENTOTESOURARIA").HasColumnType("datetime");
        builder.Property(x => x.HoraFechamentoTesouraria).HasColumnName("DATAHORAFECHAMENTOTESOURARIA").HasColumnType("datetime");

        // Agregado: FK_FINDOMINIOPERIODOFECHAMENTO_FINDOMINIOPERIODO — Artigo V.
        builder.HasMany(x => x.Fechamentos)
            .WithOne()
            .HasForeignKey(f => new { f.IdDominioPeriodo, f.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Fechamentos)
            .HasField("_fechamentos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

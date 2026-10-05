using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIORESPONSAVEL (10 colunas) — analysis/E2-dominio.md §2.7.
// O lado filho (agregado de Dominio) é configurado em DominioMapping.
public class DominioResponsavelMapping : IEntityTypeConfiguration<DominioResponsavel>
{
    public void Configure(EntityTypeBuilder<DominioResponsavel> builder)
    {
        builder.ToTable("FINDOMINIORESPONSAVEL");

        // PK_FINDOMINIORESPONSAVEL (IDFINDOMINIO, IDGLOFILIAL, IDFINDOMINIOPAI)
        builder.HasKey(x => new { x.IdDominio, x.IdFilial, x.IdDominioPai });

        builder.Property(x => x.IdDominio).HasColumnName("IDFINDOMINIO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();
        builder.Property(x => x.IdDominioPai).HasColumnName("IDFINDOMINIOPAI").ValueGeneratedNever();

        // FK_FINDOMINIORESPONSAVEL_FINDOMINIO_PAI (IDFINDOMINIOPAI, IDGLOFILIAL)
        builder.HasOne<Dominio>()
            .WithMany()
            .HasForeignKey(x => new { x.IdDominioPai, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DominioPaiPrincipal).HasColumnName("DOMINIOPAIPRINCIPAL").IsRequired();

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

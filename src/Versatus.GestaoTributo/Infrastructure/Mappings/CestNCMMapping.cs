using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class CestNCMMapping : IEntityTypeConfiguration<CestNCM>
{
    public void Configure(EntityTypeBuilder<CestNCM> builder)
    {
        builder.ToTable("TRBCESTNCM");

        builder.HasKey(c => c.IdCestNcm);

        builder.Property(c => c.IdCestNcm)
            .HasColumnName("IDTRBCESTNCM")
            .ValueGeneratedNever();

        builder.Property(c => c.IdCest)
            .HasColumnName("IDTRBCEST")
            .IsRequired();

        builder.Property(c => c.Ncm)
            .HasColumnName("NCM")
            .HasMaxLength(10)
            .IsRequired();

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(c => c.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(c => c.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(c => c.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(c => c.Cest)
            .WithMany()
            .HasForeignKey(c => c.IdCest)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

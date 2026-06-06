using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class RegraTributoMapping : IEntityTypeConfiguration<RegraTributo>
{
    public void Configure(EntityTypeBuilder<RegraTributo> builder)
    {
        builder.ToTable("TRBREGRATRIBUTO");

        builder.HasKey(r => r.IdRegraTributo);

        builder.Property(r => r.IdRegraTributo)
            .HasColumnName("IDTRBREGRATRIBUTO")
            .ValueGeneratedNever();

        builder.Property(r => r.IdRegraTributoPai)
            .HasColumnName("IDTRBREGRATRIBUTOPAI");

        builder.Property(r => r.IdTributo)
            .HasColumnName("IDGLOTRIBUTO")
            .IsRequired();

        builder.Property(r => r.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(r => r.IdSinteticoAnalitico)
            .HasColumnName("IDSINTETICOANALITICO")
            .IsRequired();

        // Auditoria
        builder.Property(r => r.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(r => r.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(r => r.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(r => r.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(r => r.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(r => r.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(r => r.Pai)
            .WithMany(r => r.Filhos)
            .HasForeignKey(r => r.IdRegraTributoPai)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Tributo)
            .WithMany()
            .HasForeignKey(r => r.IdTributo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

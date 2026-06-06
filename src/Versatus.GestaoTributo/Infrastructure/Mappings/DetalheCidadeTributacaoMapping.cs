using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class DetalheCidadeTributacaoMapping : IEntityTypeConfiguration<DetalheCidadeTributacao>
{
    public void Configure(EntityTypeBuilder<DetalheCidadeTributacao> builder)
    {
        builder.ToTable("TRBDETALHECIDADETRIBUTACAO");

        builder.HasKey(d => d.IdDetalheCidadeTributacao);

        builder.Property(d => d.IdDetalheCidadeTributacao)
            .HasColumnName("IDTRBDETALHECIDADETRIBUTACAO")
            .ValueGeneratedNever();

        builder.Property(d => d.IdRegraEntrada)
            .HasColumnName("IDTRBREGRAENTRADA");

        builder.Property(d => d.IdRegraSaida)
            .HasColumnName("IDTRBREGRASAIDA");

        builder.Property(d => d.IdTributacao)
            .HasColumnName("IDTRBTRIBUTACAO")
            .IsRequired();

        builder.Property(d => d.IdCidade)
            .HasColumnName("IDGLOCIDADE")
            .IsRequired();

        builder.Property(d => d.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(d => d.VigenciaInicio)
            .HasColumnName("VIGENCIAINICIO")
            .IsRequired();

        builder.Property(d => d.VigenciaFim)
            .HasColumnName("VIGENCIAFIM");

        builder.Property(d => d.IdTipoBaseCalculo)
            .HasColumnName("IDTIPOBASECALCULO")
            .HasConversion<int?>();

        // Relacionamentos
        builder.HasOne(d => d.Tributacao)
            .WithMany(t => t.DetalhesCidade)
            .HasForeignKey(d => d.IdTributacao)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.RegraEntrada)
            .WithMany()
            .HasForeignKey(d => d.IdRegraEntrada)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.RegraSaida)
            .WithMany()
            .HasForeignKey(d => d.IdRegraSaida)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

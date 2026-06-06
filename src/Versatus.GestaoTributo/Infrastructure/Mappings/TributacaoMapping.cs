using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class TributacaoMapping : IEntityTypeConfiguration<Tributacao>
{
    public void Configure(EntityTypeBuilder<Tributacao> builder)
    {
        builder.ToTable("TRBTRIBUTACAO");

        builder.HasKey(t => t.IdTributacao);

        builder.Property(t => t.IdTributacao)
            .HasColumnName("IDTRBTRIBUTACAO")
            .ValueGeneratedNever();

        builder.Property(t => t.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(t => t.AplicarAliquotaNcmUf)
            .HasColumnName("APLICARALIQUOTANCMUF")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(t => t.RegraTributacaoTipo)
            .HasColumnName("IDDETALHEUF")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.BaseCalculoTipo)
            .HasColumnName("IDTIPOBASECALCULO")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.IdTributo)
            .HasColumnName("IDGLOTRIBUTO")
            .IsRequired();

        builder.Property(t => t.IdRegraEntrada)
            .HasColumnName("IDTRBREGRAENTRADA");

        builder.Property(t => t.IdRegraSaida)
            .HasColumnName("IDTRBREGRASAIDA");

        // Auditoria
        builder.Property(t => t.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(t => t.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(t => t.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(t => t.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(t => t.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(t => t.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(t => t.Tributo)
            .WithMany()
            .HasForeignKey(t => t.IdTributo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.RegraEntrada)
            .WithMany()
            .HasForeignKey(t => t.IdRegraEntrada)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.RegraSaida)
            .WithMany()
            .HasForeignKey(t => t.IdRegraSaida)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

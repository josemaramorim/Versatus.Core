using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class CondicaoPagtoRegraMapping : IEntityTypeConfiguration<CondicaoPagtoRegra>
{
    public void Configure(EntityTypeBuilder<CondicaoPagtoRegra> builder)
    {
        builder.ToTable("GloCondicaoPagtoRegra");

        builder.HasKey(r => r.IdCondicaoPagtoParcela);

        builder.Property(r => r.IdCondicaoPagtoParcela)
            .HasColumnName("IdGloCondicaoPagamentoRegra")
            .ValueGeneratedNever();

        builder.Property(r => r.IdCondicaoPagamento)
            .HasColumnName("IdGloCondicaoPagamento")
            .IsRequired();

        builder.Property(r => r.NumeroDias)
            .HasColumnName("NumeroDias")
            .IsRequired();

        builder.Property(r => r.NumeroParcela)
            .HasColumnName("NumeroParcela")
            .IsRequired();

        builder.Property(r => r.PercentualDivisao)
            .HasColumnName("PercentualDivisao")
            .HasPrecision(5, 2)
            .IsRequired();

        // Faixas
        builder.Property(r => r.DiaInicial)
            .HasColumnName("DiaInicial");

        builder.Property(r => r.DiaFinal)
            .HasColumnName("DiaFinal");

        // Parcelas
        builder.Property(r => r.DiasLiberado)
            .HasColumnName("DiasLiberado");

        builder.Property(r => r.PercentualValorMinimo)
            .HasColumnName("PercentualValorMinimo")
            .HasPrecision(5, 2);

        // Relacionamento com CondicaoPagamento
        builder.HasOne(r => r.CondicaoPagamento)
            .WithMany(cp => cp.Regras)
            .HasForeignKey(r => r.IdCondicaoPagamento)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

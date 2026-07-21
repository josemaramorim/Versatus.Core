using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class CondicaoPagamentoMapping : IEntityTypeConfiguration<CondicaoPagamento>
{
    public void Configure(EntityTypeBuilder<CondicaoPagamento> builder)
    {
        builder.ToTable("GloCondicaoPagamento");

        builder.HasKey(cp => cp.IdCondicaoPagamento);

        builder.Property(cp => cp.IdCondicaoPagamento)
            .HasColumnName("IdGloCondicaoPagamento")
            .ValueGeneratedNever();

        builder.Property(cp => cp.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cp => cp.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        builder.Property(cp => cp.IdTipoCondicaoPagto)
            .HasColumnName("IdTipoCondicaoPagto")
            .HasConversion<int>();

        builder.Property(cp => cp.IdDisponibilidade)
            .HasColumnName("IdDisponibilidade")
            .HasConversion<int>();

        builder.Property(cp => cp.IdTipoVencimento)
            .HasColumnName("IdTipoVencimento")
            .HasConversion<int>();

        builder.Property(cp => cp.IdGrupoCondicaoPagamento)
            .HasColumnName("IdGloGrupoCondicaoPagamento");

        builder.Property(cp => cp.IdFormaCobranca)
            .HasColumnName("IdGloFormaCobranca");

        builder.Property(cp => cp.IdFormaPagamento)
            .HasColumnName("IdGloFormaPagamento");

        builder.Property(cp => cp.IdFormaPagamentoVista)
            .HasColumnName("IdGloFormaPagamentoAVista");

        builder.Property(cp => cp.OrdemConsulta)
            .HasColumnName("OrdemConsulta");

        builder.Property(cp => cp.UtilizarPdv)
            .HasColumnName("UtilizarPdv")
            .IsRequired();

        // Acréscimo / Desconto
        builder.Property(cp => cp.RecebeAcrescimo)
            .HasColumnName("RecebeAcrescimo")
            .IsRequired();

        builder.Property(cp => cp.Acrescimo)
            .HasColumnName("Acrescimo")
            .HasPrecision(5, 2);

        builder.Property(cp => cp.RecebeDesconto)
            .HasColumnName("RecebeDesconto")
            .IsRequired();

        builder.Property(cp => cp.Desconto)
            .HasColumnName("Desconto")
            .HasPrecision(5, 2);

        // Configurações de Parcelamento
        builder.Property(cp => cp.AlteraParcelas)
            .HasColumnName("AlteraParcelas")
            .IsRequired();

        builder.Property(cp => cp.AlteraNroParcela)
            .HasColumnName("AlteraNroParcela");

        builder.Property(cp => cp.IdParcelamentoTipo)
            .HasColumnName("IdTipoParcelamento")
            .HasConversion<int>();

        builder.Property(cp => cp.TipoDivisaoParcelamento)
            .HasColumnName("IdTipoDivisaoParcelamento")
            .HasConversion<int>();

        builder.Property(cp => cp.QuantidadeParcela)
            .HasColumnName("QuantidadeParcela");

        builder.Property(cp => cp.DiasParcelamento)
            .HasColumnName("DiasParcelamento");

        builder.Property(cp => cp.UsarMesComercial)
            .HasColumnName("UsarMesComercial")
            .IsRequired();

        builder.Property(cp => cp.DiasMinimoProximoMes)
            .HasColumnName("DiasMinimoProximoMes");

        builder.Property(cp => cp.PrimeiraParcelaAVista)
            .HasColumnName("PrimeiraParcelaAVista");

        builder.Property(cp => cp.ObrigatorioFormaPagamento)
            .HasColumnName("ObrigatorioFormaPagamento");

        builder.Property(cp => cp.IdParcelaArredondamento)
            .HasColumnName("IdParcelaArredondamento")
            .HasConversion<int>();

        // Configurações de Faixa
        builder.Property(cp => cp.QuantidadeFaixa)
            .HasColumnName("QuantidadeFaixa");

        // Configurações de Semanal
        builder.Property(cp => cp.IdDiaSemana)
            .HasColumnName("IdDiaSemana")
            .HasConversion<int>();

        // Relacionamentos
        builder.HasMany(cp => cp.Regras)
            .WithOne(r => r.CondicaoPagamento)
            .HasForeignKey(r => r.IdCondicaoPagamento)
            .OnDelete(DeleteBehavior.Cascade);

        // Auditoria
        builder.Property(cp => cp.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(cp => cp.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(cp => cp.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(cp => cp.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(cp => cp.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(cp => cp.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

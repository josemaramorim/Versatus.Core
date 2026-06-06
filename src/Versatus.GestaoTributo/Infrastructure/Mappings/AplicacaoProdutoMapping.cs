using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class AplicacaoProdutoMapping : IEntityTypeConfiguration<AplicacaoProduto>
{
    public void Configure(EntityTypeBuilder<AplicacaoProduto> builder)
    {
        builder.ToTable("TRBAPLICACAOPRODUTO");

        builder.HasKey(ap => ap.IdAplicacaoProduto);

        builder.Property(ap => ap.IdAplicacaoProduto)
            .HasColumnName("IDTRBAPLICACAOPRODUTO")
            .ValueGeneratedNever();

        builder.Property(ap => ap.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(ap => ap.IdSinteticoAnalitico)
            .HasColumnName("IDSINTETICOANALITICO")
            .IsRequired();

        builder.Property(ap => ap.IdAplicacaoProdutoPai)
            .HasColumnName("IDTRBAPLICACAOPRODUTOPAI");

        builder.Property(ap => ap.MovimentaEstoque)
            .HasColumnName("MOVIMENTAESTOQUE")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(ap => ap.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(ap => ap.SufixoCFOP)
            .HasColumnName("SUFIXOCFOP")
            .HasMaxLength(4);

        builder.Property(ap => ap.SubstituicaoICMS)
            .HasColumnName("SUBSTITUICAOICMS")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(ap => ap.IdTipoNaturezaOperacao)
            .HasColumnName("IDTIPONATUREZAOPERACAO");

        builder.Property(ap => ap.AplicarSubstituicaoExterna)
            .HasColumnName("APLICARSUBSTITUICAOEXTERNA")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(ap => ap.UsaRegraTributoIvaProduto)
            .HasColumnName("USAREGRATRIBUTOIVAPRODUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(ap => ap.IdRegraTributoIva)
            .HasColumnName("IDTRBREGRATRIBUTOIVA");

        // Auditoria
        builder.Property(ap => ap.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(ap => ap.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(ap => ap.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(ap => ap.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(ap => ap.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(ap => ap.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(ap => ap.Pai)
            .WithMany(ap => ap.Filhos)
            .HasForeignKey(ap => ap.IdAplicacaoProdutoPai)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ap => ap.RegraTributoIva)
            .WithMany()
            .HasForeignKey(ap => ap.IdRegraTributoIva)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

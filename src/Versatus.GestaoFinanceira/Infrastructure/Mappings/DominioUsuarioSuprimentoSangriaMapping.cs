using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA (12 colunas) — view, somente leitura, sem chave.
// analysis/E2-dominio.md §2.8; tipos conferidos em INFORMATION_SCHEMA.COLUMNS (2026-10-05).
// DOMINIOPAIPRINCIPAL, DOMINIOPAI e CAIXA são int na view (constantes 0/1 do UNION), não
// smallint — conversão bool↔int explícita, sobrepondo a convenção bool↔short do DbContext.
public class DominioUsuarioSuprimentoSangriaMapping : IEntityTypeConfiguration<DominioUsuarioSuprimentoSangria>
{
    public void Configure(EntityTypeBuilder<DominioUsuarioSuprimentoSangria> builder)
    {
        builder.ToView("VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA");
        builder.HasNoKey();

        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL");
        builder.Property(x => x.IdDominio).HasColumnName("IDDOMINIO");
        builder.Property(x => x.DominioPaiPrincipal).HasColumnName("DOMINIOPAIPRINCIPAL").HasConversion<int>();
        builder.Property(x => x.IdDominioFilho).HasColumnName("IDDOMINIOFILHO");
        builder.Property(x => x.IdDominioRetorno).HasColumnName("IDDOMINIORETORNO");
        builder.Property(x => x.DescricaoRetorno).HasColumnName("DESCRICAORETORNO").HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.IdUsuarioRetorno).HasColumnName("IDDOMINIOUSUARIORETORNO");
        builder.Property(x => x.UsuarioPrincipalDominio).HasColumnName("USUARIOPRINCIPALDOMINIO");
        builder.Property(x => x.NomeUsuarioRetorno).HasColumnName("DOMINIOUSUARIONOMERETORNO").HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.Ativo).HasColumnName("ATIVO");
        builder.Property(x => x.DominioPai).HasColumnName("DOMINIOPAI").HasConversion<int>();
        builder.Property(x => x.Caixa).HasColumnName("CAIXA").HasConversion<int>();
    }
}

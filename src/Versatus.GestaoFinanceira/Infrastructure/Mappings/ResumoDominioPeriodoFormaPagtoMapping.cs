using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// VWRESUMODOMINIOPERIODOFORMAPAGTO (6 colunas) — view, somente leitura, sem chave.
// analysis/E2-dominio.md §2.8. CREDITO/DEBITO saem da view como numeric(38,2) (conferido em
// INFORMATION_SCHEMA.COLUMNS em 2026-10-05), não numeric(23,8) como nas tabelas.
public class ResumoDominioPeriodoFormaPagtoMapping : IEntityTypeConfiguration<ResumoDominioPeriodoFormaPagto>
{
    public void Configure(EntityTypeBuilder<ResumoDominioPeriodoFormaPagto> builder)
    {
        builder.ToView("VWRESUMODOMINIOPERIODOFORMAPAGTO");
        builder.HasNoKey();

        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL");
        builder.Property(x => x.IdDominioPeriodo).HasColumnName("IDFINDOMINIOPERIODO");
        builder.Property(x => x.Descricao).HasColumnName("DESCRICAO").HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.TipoFormaPagto).HasColumnName("IDTIPOFORMAPAGAMENTO").HasConversion<int>();
        builder.Property(x => x.Credito).HasColumnName("CREDITO").HasPrecision(38, 2);
        builder.Property(x => x.Debito).HasColumnName("DEBITO").HasPrecision(38, 2);
    }
}

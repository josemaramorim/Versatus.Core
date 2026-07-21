using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class GrupoCondicaoPagamentoMapping : IEntityTypeConfiguration<GrupoCondicaoPagamento>
{
    public void Configure(EntityTypeBuilder<GrupoCondicaoPagamento> builder)
    {
        builder.ToTable("GloGrupoCondicaoPagamento");

        builder.HasKey(g => g.IdGrupoCondicaoPagamento);

        builder.Property(g => g.IdGrupoCondicaoPagamento)
            .HasColumnName("IdGloGrupoCondicaoPagamento")
            .ValueGeneratedNever();

        builder.Property(g => g.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(g => g.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(g => g.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(g => g.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(g => g.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(g => g.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(g => g.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class FormaPagamentoMapping : IEntityTypeConfiguration<FormaPagamento>
{
    public void Configure(EntityTypeBuilder<FormaPagamento> builder)
    {
        builder.ToTable("GloFormaPagamento");

        builder.HasKey(fp => fp.IdForma);

        builder.Property(fp => fp.IdForma)
            .HasColumnName("IdGloFormaPagamento")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(fp => fp.Codigo)
            .HasColumnName("SiglaForma")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(fp => fp.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(fp => fp.Tipo)
            .HasColumnName("IdTipoFormaPagamento")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fp => fp.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(fp => fp.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(fp => fp.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(fp => fp.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(fp => fp.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(fp => fp.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(fp => fp.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

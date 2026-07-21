using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class FormaCobrancaMapping : IEntityTypeConfiguration<FormaCobranca>
{
    public void Configure(EntityTypeBuilder<FormaCobranca> builder)
    {
        builder.ToTable("GloFormaCobranca");

        builder.HasKey(fc => fc.IdFormaCobranca);

        builder.Property(fc => fc.IdFormaCobranca)
            .HasColumnName("IdGloFormaCobranca")
            .ValueGeneratedNever();

        builder.Property(fc => fc.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(fc => fc.SiglaForma)
            .HasColumnName("SiglaForma")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(fc => fc.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(fc => fc.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(fc => fc.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(fc => fc.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(fc => fc.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(fc => fc.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(fc => fc.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

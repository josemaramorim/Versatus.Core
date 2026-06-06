using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Classification;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class CentroCustoMapping : IEntityTypeConfiguration<CentroCusto>
{
    public void Configure(EntityTypeBuilder<CentroCusto> builder)
    {
        builder.ToTable("GloCentroCusto");

        builder.HasKey(c => c.IdCentroCusto);

        builder.Property(c => c.IdCentroCusto)
            .HasColumnName("IdGloCentroCusto")
            .ValueGeneratedNever();

        builder.Property(c => c.IdFilial)
            .HasColumnName("IdGloFilial")
            .IsRequired();

        builder.Property(c => c.IdCentroCustoPai)
            .HasColumnName("IdGloCentroCustoPai");

        builder.Property(c => c.Nome)
            .HasColumnName("Descricao") // No legado costuma ser Descricao para nomes
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CodigoFormatado)
            .HasColumnName("Extenso")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Nivel)
            .HasColumnName("Nivel")
            .IsRequired();

        builder.Property(c => c.IdSinteticoAnalitico)
            .HasColumnName("IdTipo")
            .IsRequired();

        builder.Property(c => c.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(c => c.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(c => c.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos
        builder.HasOne(c => c.Filial)
            .WithMany()
            .HasForeignKey(c => c.IdFilial)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CentroCustoPai)
            .WithMany(c => c.SubCentros)
            .HasForeignKey(c => c.IdCentroCustoPai)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

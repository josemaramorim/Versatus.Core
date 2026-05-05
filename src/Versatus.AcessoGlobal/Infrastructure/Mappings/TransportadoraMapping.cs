using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class TransportadoraMapping : IEntityTypeConfiguration<Transportadora>
{
    public void Configure(EntityTypeBuilder<Transportadora> builder)
    {
        builder.ToTable("GloTransportadora");

        builder.HasKey(t => t.IdTransportadora);

        builder.Property(t => t.IdTransportadora)
            .HasColumnName("IdGloTransportadora")
            .ValueGeneratedNever();

        builder.Property(t => t.Rntrc).HasColumnName("Rntrc").HasMaxLength(20);
        builder.Property(t => t.Ativo).HasColumnName("Ativo").IsRequired();

        builder.Property(t => t.ProprietarioTipo).HasColumnName("IdTipoProprietario");
        builder.Property(t => t.TransportadorTipo).HasColumnName("IdTipoTransportador");

        builder.Property(t => t.IdCategoria).HasColumnName("IdGloCategoria");

        // Auditoria
        builder.Property(t => t.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(t => t.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(t => t.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(t => t.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(t => t.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(f => f.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamento 1:1 com Entidade
        builder.HasOne(t => t.Entidade)
            .WithOne()
            .HasForeignKey<Transportadora>(t => t.IdTransportadora);

        builder.HasOne(t => t.Categoria)
            .WithMany()
            .HasForeignKey(t => t.IdCategoria);
    }
}

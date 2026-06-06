using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class SerieDocumentoFilialMapping : IEntityTypeConfiguration<SerieDocumentoFilial>
{
    public void Configure(EntityTypeBuilder<SerieDocumentoFilial> builder)
    {
        builder.ToTable("GloSerieDocumentoFilial");

        builder.HasKey(sf => new { sf.CodigoSerie, sf.IdFilial });

        builder.Property(sf => sf.CodigoSerie)
            .HasColumnName("IdGloSerieDocumento")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(sf => sf.IdFilial)
            .HasColumnName("IdGloFilial")
            .IsRequired();

        builder.Ignore(sf => sf.ProximoNumero);

        // Auditoria
        builder.Property(sf => sf.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(sf => sf.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(sf => sf.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(sf => sf.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(sf => sf.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(sf => sf.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos
        builder.HasOne(sf => sf.SerieDocumento)
            .WithMany()
            .HasForeignKey(sf => sf.CodigoSerie)
            .HasPrincipalKey(s => s.Codigo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sf => sf.Filial)
            .WithMany()
            .HasForeignKey(sf => sf.IdFilial)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

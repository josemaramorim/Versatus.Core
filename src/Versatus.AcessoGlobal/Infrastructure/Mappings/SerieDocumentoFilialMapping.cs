using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class SerieDocumentoFilialMapping : IEntityTypeConfiguration<SerieDocumentoFilial>
{
    public void Configure(EntityTypeBuilder<SerieDocumentoFilial> builder)
    {
        builder.ToTable("GloSerieDocumentoFilial");

        builder.HasKey(sf => sf.IdRelacao);

        builder.Property(sf => sf.IdRelacao)
            .HasColumnName("IdGloSerieDocumentoFilial") // surrogate PK para o novo sistema
            .ValueGeneratedOnAdd(); // auto-incremento para facilidade do relacionamento

        builder.Property(sf => sf.IdSerie)
            .HasColumnName("IdSequencialSerieDocto")
            .IsRequired();

        builder.Property(sf => sf.IdFilial)
            .HasColumnName("IdGloFilial")
            .IsRequired();

        builder.Property(sf => sf.ProximoNumero)
            .HasColumnName("ProximoNumero") // Se for gerado como campo customizado ou temporário
            .IsRequired();

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
            .HasForeignKey(sf => sf.IdSerie)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sf => sf.Filial)
            .WithMany()
            .HasForeignKey(sf => sf.IdFilial)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

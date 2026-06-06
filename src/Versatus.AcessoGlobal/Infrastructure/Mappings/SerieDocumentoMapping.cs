using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class SerieDocumentoMapping : IEntityTypeConfiguration<SerieDocumento>
{
    public void Configure(EntityTypeBuilder<SerieDocumento> builder)
    {
        builder.ToTable("GloSerieDocumento");

        builder.HasKey(s => s.IdSerie);

        builder.Property(s => s.IdSerie)
            .HasColumnName("IdSequencialSerieDocto")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(s => s.Codigo)
            .HasColumnName("IdGloSerieDocumento")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Prefixo)
            .HasColumnName("ModeloFiscal")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.ProximoNumero)
            .HasColumnName("ProximoNumero") // Se for gerado como campo customizado ou temporário
            .IsRequired();

        builder.Property(s => s.Ativa)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(s => s.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(s => s.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(s => s.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(s => s.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(s => s.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(s => s.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

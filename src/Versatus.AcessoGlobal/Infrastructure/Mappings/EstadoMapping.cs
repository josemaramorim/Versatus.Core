using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class EstadoMapping : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        builder.ToTable("GloEstado");

        builder.HasKey(e => e.IdEstado);

        builder.Property(e => e.IdEstado)
            .HasColumnName("IdSequencialEstado")
            .ValueGeneratedNever();

        builder.Property(e => e.IdPais)
            .HasColumnName("IdGloPais") // Adicionado conforme SPEC (embora ausente no legado direto)
            .IsRequired();

        builder.Property(e => e.Sigla)
            .HasColumnName("Uf")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(e => e.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.CodigoIBGE)
            .HasColumnName("CodigoIBGE")
            .HasMaxLength(2);

        builder.Property(e => e.Ativo).HasColumnName("Ativo");
        builder.Property(e => e.ExigeIdentificacaoTecnico).HasColumnName("ExigeIdentificacaoTecnico");
        builder.Property(e => e.ExigeRegistroSistema).HasColumnName("ExigeRegistroSistema");

        // Relacionamentos
        builder.HasOne(e => e.Pais)
            .WithMany()
            .HasForeignKey(e => e.IdPais)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoria
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(e => e.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(e => e.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(p => p.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(p => p.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(p => p.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

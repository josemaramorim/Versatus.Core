using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class CfopMapping : IEntityTypeConfiguration<Cfop>
{
    public void Configure(EntityTypeBuilder<Cfop> builder)
    {
        builder.ToTable("TRBCFOP");

        builder.HasKey(c => c.IdCfop);

        builder.Property(c => c.IdCfop)
            .HasColumnName("IDTRBCFOP")
            .ValueGeneratedNever();

        builder.Property(c => c.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.IdCfopPai)
            .HasColumnName("IDTRBCFOPPAI");

        builder.Property(c => c.IdSinteticoAnalitico)
            .HasColumnName("IDSINTETICOANALITICO")
            .IsRequired();

        builder.Property(c => c.Sufixo)
            .HasColumnName("SUFIXO");

        builder.Property(c => c.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IdSequencial)
            .HasColumnName("IDSEQUENCIAL")
            .IsRequired();

        builder.Property(c => c.CodigoAcumuladorAVista)
            .HasColumnName("CODIGOACUMULADORAVISTA")
            .HasMaxLength(20);

        builder.Property(c => c.CodigoAcumuladorAPrazo)
            .HasColumnName("CODIGOACUMULADORAPRAZO")
            .HasMaxLength(20);

        builder.Property(c => c.DescricaoCompleta)
            .HasColumnName("DESCRICAOCOMPLETA");

        builder.Property(c => c.SubstituicaoIcms)
            .HasColumnName("SUBSTITUICAOICMS")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IdTipoNaturezaOperacao)
            .HasColumnName("IDTIPONATUREZAOPERACAO");

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(c => c.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(c => c.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(c => c.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamento Auto-relacionamento
        builder.HasOne(c => c.Pai)
            .WithMany(c => c.Filhos)
            .HasForeignKey(c => c.IdCfopPai)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

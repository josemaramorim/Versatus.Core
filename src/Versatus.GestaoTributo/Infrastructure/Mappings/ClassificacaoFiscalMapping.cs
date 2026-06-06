using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class ClassificacaoFiscalMapping : IEntityTypeConfiguration<ClassificacaoFiscal>
{
    public void Configure(EntityTypeBuilder<ClassificacaoFiscal> builder)
    {
        builder.ToTable("TRBCLASSIFICACAOFISCAL");

        builder.HasKey(c => c.IdClassificacaoFiscal);

        builder.Property(c => c.IdClassificacaoFiscal)
            .HasColumnName("IDTRBCLASSIFICACAOFISCAL")
            .ValueGeneratedNever();

        builder.Property(c => c.IdClassificacaoFiscalPai)
            .HasColumnName("IDTRBCLASSIFICACAOFISCALPAI");

        builder.Property(c => c.IdSinteticoAnalitico)
            .HasColumnName("IDSINTETICOANALITICO")
            .IsRequired();

        builder.Property(c => c.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Ncm)
            .HasColumnName("NCM")
            .HasMaxLength(10);

        builder.Property(c => c.Nbm)
            .HasColumnName("NBM")
            .HasMaxLength(15);

        builder.Property(c => c.ExtIpi)
            .HasColumnName("EXTIPI")
            .HasMaxLength(3);

        builder.Property(c => c.GeneroIpi)
            .HasColumnName("GENEROTIPI")
            .HasMaxLength(2);

        builder.Property(c => c.Observacao)
            .HasColumnName("OBSERVACAO");

        builder.Property(c => c.Ativo)
            .HasColumnName("ATIVO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.CodigoAtividadeCP)
            .HasColumnName("CODIGOATIVIDADECP")
            .HasMaxLength(20);

        builder.Property(c => c.IdTipoEscalaRelevante)
            .HasColumnName("IDTIPOESCALARELEVANTE");

        builder.Property(c => c.VigenciaFinalNcm)
            .HasColumnName("VIGENCIAFINALNCM");

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(c => c.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(c => c.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(c => c.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamento Auto-relacionamento (Pai-Filho)
        builder.HasOne(c => c.Pai)
            .WithMany(c => c.Filhos)
            .HasForeignKey(c => c.IdClassificacaoFiscalPai)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

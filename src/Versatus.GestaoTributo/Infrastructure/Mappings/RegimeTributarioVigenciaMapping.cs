using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class RegimeTributarioVigenciaMapping : IEntityTypeConfiguration<RegimeTributarioVigencia>
{
    public void Configure(EntityTypeBuilder<RegimeTributarioVigencia> builder)
    {
        builder.ToTable("TRBREGIMETRIBUTARIOVIGENCIA");

        builder.HasKey(r => r.IdRegimeTributarioVigencia);

        builder.Property(r => r.IdRegimeTributarioVigencia)
            .HasColumnName("IDTRBREGIMETRIBUTARIOVIGENCIA")
            .ValueGeneratedNever();

        builder.Property(r => r.IdFilial)
            .HasColumnName("IDGLOFILIAL")
            .IsRequired();

        builder.Property(r => r.IdIncidenciaTributaria)
            .HasColumnName("IDINCIDENCIATRIBUTARIA");

        builder.Property(r => r.IdApropriacaoCredito)
            .HasColumnName("IDAPROPRIACAOCREDITO");

        builder.Property(r => r.IdTipoContribuicaoApurada)
            .HasColumnName("IDTIPOCONTRIBUICAOAPURADA");

        builder.Property(r => r.IdRegimeEscrituracaoApuracaoPresumido)
            .HasColumnName("IDREGIMEESCRITURACAOAPURACAOPRESUMIDO");

        builder.Property(r => r.VigenciaInicio)
            .HasColumnName("VIGENCIAINICIO")
            .IsRequired();

        builder.Property(r => r.VigenciaFim)
            .HasColumnName("VIGENCIAFIM");

        builder.Property(r => r.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.IdEscrituracaoNfeEcf)
            .HasColumnName("IDESCRITURACAONFEECF");

        builder.Property(r => r.EnviarPlanoContabil)
            .HasColumnName("ENVIARPLANOCONTABIL")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(r => r.IdApuracaoContribuicaoPrevidenciaria)
            .HasColumnName("IDAPURACAOCONTRIBUICAOPREVIDENCIARIA")
            .IsRequired();
    }
}

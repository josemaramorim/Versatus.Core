using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class ParametroValorMapping : IEntityTypeConfiguration<ParametroValor>
{
    public void Configure(EntityTypeBuilder<ParametroValor> builder)
    {
        builder.ToTable("GloParametroValor");

        builder.HasKey(pv => pv.IdParametroValor);

        builder.Property(pv => pv.IdParametroValor)
            .HasColumnName("IDGLOPARAMETROVALOR")
            .ValueGeneratedNever();

        builder.Property(pv => pv.IdParametro)
            .HasColumnName("IDGLOPARAMETRO");

        builder.Property(pv => pv.IdFilial)
            .HasColumnName("IDGLOFILIAL");

        builder.Property(pv => pv.IdPerfil)
            .HasColumnName("IDGLOPERFIL");

        builder.Property(pv => pv.Valor)
            .HasColumnName("VALOR")
            .HasMaxLength(255);

        builder.Property(pv => pv.IdEmpresa)
            .HasColumnName("IDGLOEMPRESA");

        builder.Property(pv => pv.IdGrupo)
            .HasColumnName("IDGLOGRUPO");

        // Relacionamentos
        builder.HasOne(pv => pv.Parametro)
            .WithMany()
            .HasForeignKey(pv => pv.IdParametro)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

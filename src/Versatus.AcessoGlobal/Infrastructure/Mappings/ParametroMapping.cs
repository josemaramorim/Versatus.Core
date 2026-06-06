using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class ParametroMapping : IEntityTypeConfiguration<Parametro>
{
    public void Configure(EntityTypeBuilder<Parametro> builder)
    {
        builder.ToTable("GloParametro");

        builder.HasKey(p => p.IdParam);

        builder.Property(p => p.IdParam)
            .HasColumnName("IdGloParametro")
            .ValueGeneratedNever(); // Controlado pelo GeradorSequencial

        builder.Property(p => p.Chave)
            .HasColumnName("Nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(p => p.Valor)
            .HasColumnName("Objeto")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(p => p.Tipo)
            .HasColumnName("IdTipoValor")
            .IsRequired(false);
    }
}

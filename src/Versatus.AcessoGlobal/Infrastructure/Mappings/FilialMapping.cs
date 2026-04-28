using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Organization;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class FilialMapping : IEntityTypeConfiguration<Filial>
{
    public void Configure(EntityTypeBuilder<Filial> builder)
    {
        builder.ToTable("GloFilial");

        builder.HasKey(f => f.IdFilial);

        builder.Property(f => f.IdFilial)
            .HasColumnName("IdGloFilial")
            .ValueGeneratedNever();

        builder.Property(f => f.IdEmpresa)
            .HasColumnName("IdGloEmpresa")
            .IsRequired();

        builder.Property(f => f.Ativo).HasColumnName("Ativo");

        builder.Property(f => f.Logomarca).HasColumnName("Logomarca");
        builder.Property(f => f.LogomarcaMedia).HasColumnName("LogomarcaMedia");
        builder.Property(f => f.LogomarcaGrande).HasColumnName("LogomarcaGrande");

        // Propriedades Ignoradas (Ficam na tabela GloEntidade no legado)
        builder.Ignore(f => f.Nome);
        builder.Ignore(f => f.CNPJ);
        builder.Ignore(f => f.InscricaoEstadual);

        // Relacionamentos
        builder.HasOne(f => f.Empresa)
            .WithMany(e => e.Filiais)
            .HasForeignKey(f => f.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

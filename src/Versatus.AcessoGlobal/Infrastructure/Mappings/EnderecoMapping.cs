using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("GloEndereco");

        builder.HasKey(e => e.IdEndereco);

        builder.Property(e => e.IdEndereco)
            .HasColumnName("IdGloEndereco")
            .ValueGeneratedNever();

        builder.Property(e => e.IdCidade).HasColumnName("IdGloCidade");
        builder.Property(e => e.IdBairroInicial).HasColumnName("IdGloBairroInicial");
        builder.Property(e => e.IdBairroFinal).HasColumnName("IdGloBairroFinal");
        builder.Property(e => e.IdTipoLogradouro).HasColumnName("IdGloTipoLogradouro");

        builder.Property(e => e.Logradouro).HasColumnName("Logradouro").HasMaxLength(150);
        builder.Property(e => e.CEP).HasColumnName("Cep").HasMaxLength(8);
        builder.Property(e => e.Complemento).HasColumnName("Complemento").HasMaxLength(150);
        builder.Property(e => e.Latitude).HasColumnName("LATITUDE");
        builder.Property(e => e.Longitude).HasColumnName("LONGITUDE");

        // Relacionamentos
        builder.HasOne(e => e.Cidade).WithMany().HasForeignKey(e => e.IdCidade).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.BairroInicial).WithMany().HasForeignKey(e => e.IdBairroInicial).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.BairroFinal).WithMany().HasForeignKey(e => e.IdBairroFinal).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.TipoLogradouro).WithMany().HasForeignKey(e => e.IdTipoLogradouro).OnDelete(DeleteBehavior.Restrict);
    }
}

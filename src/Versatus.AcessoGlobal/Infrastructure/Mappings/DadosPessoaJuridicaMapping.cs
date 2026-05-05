using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class DadosPessoaJuridicaMapping : IEntityTypeConfiguration<DadosPessoaJuridica>
{
    public void Configure(EntityTypeBuilder<DadosPessoaJuridica> builder)
    {
        builder.ToTable("GloEntidadeJuridica");

        builder.HasKey(p => p.IdEntidade);

        builder.Property(p => p.IdEntidade)
            .HasColumnName("IdGloEntidade")
            .ValueGeneratedNever();

        builder.Property(p => p.Cnpj)
            .HasColumnName("CNPJ")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(p => p.RazaoSocial)
            .HasColumnName("RazaoSocial")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.RegimeTributario).HasColumnName("IdRegimeTributario");
        builder.Property(p => p.Enquadramento).HasColumnName("IdEnquadramento");
        builder.Property(p => p.IdCnaePrincipal).HasColumnName("IdGloCnaePrincipal");
    }
}

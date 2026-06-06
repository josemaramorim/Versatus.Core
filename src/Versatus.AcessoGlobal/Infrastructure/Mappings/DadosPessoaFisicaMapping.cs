using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class DadosPessoaFisicaMapping : IEntityTypeConfiguration<DadosPessoaFisica>
{
    public void Configure(EntityTypeBuilder<DadosPessoaFisica> builder)
    {
        builder.ToTable("GloEntidadeFisica");

        builder.HasKey(p => p.IdEntidade);

        builder.Property(p => p.IdEntidade)
            .HasColumnName("IdGloEntidade")
            .ValueGeneratedNever();

        builder.Property(p => p.Cpf)
            .HasColumnName("CPF")
            .HasMaxLength(11)
            .IsRequired(false);

        builder.Property(p => p.Rg).HasColumnName("Rg").HasMaxLength(20);
        builder.Property(p => p.OrgaoEmissorRg).HasColumnName("OrgaoEmissorRg").HasMaxLength(20);
        builder.Property(p => p.DataEmissaoRg).HasColumnName("DataEmissaoRg");
        builder.Property(p => p.DataNascimento).HasColumnName("DataNascimento");
        builder.Property(p => p.Sexo).HasColumnName("IdSexo");
        builder.Property(p => p.EstadoCivil).HasColumnName("IdEstadoCivil");
        builder.Property(p => p.FisicaTipoJuridica).HasColumnName("FisicaTipoJuridica");
    }
}

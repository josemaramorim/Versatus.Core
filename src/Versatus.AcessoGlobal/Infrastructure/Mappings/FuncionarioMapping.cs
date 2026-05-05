using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class FuncionarioMapping : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> builder)
    {
        builder.ToTable("GloFuncionario");

        builder.HasKey(f => f.IdFuncionario);

        builder.Property(f => f.IdFuncionario)
            .HasColumnName("IdGloFuncionario")
            .ValueGeneratedNever();

        builder.Property(f => f.Ctps).HasColumnName("Ctps").HasMaxLength(20);
        builder.Property(f => f.SerieCtps).HasColumnName("SerieCtps").HasMaxLength(10);
        builder.Property(f => f.UfCtps).HasColumnName("UfCtps").HasMaxLength(2);
        builder.Property(f => f.DataEmissaoCtps).HasColumnName("DataEmissaoCtps");

        builder.Property(f => f.NumeroCnh).HasColumnName("NumeroCnh").HasMaxLength(20);
        builder.Property(f => f.CategoriaCnh).HasColumnName("CategoriaCnh").HasMaxLength(2);
        builder.Property(f => f.DataVencimentoCnh).HasColumnName("DataVencimentoCnh");

        builder.Property(f => f.InscricaoPis).HasColumnName("InscricaoPis").HasMaxLength(20);
        builder.Property(f => f.IdBancoPis).HasColumnName("IdGloBancoPis");
        builder.Property(f => f.NumeroAgenciaPis).HasColumnName("NumeroAgenciaPis").HasMaxLength(20);
        builder.Property(f => f.NomeAgenciaPis).HasColumnName("NomeAgenciaPis").HasMaxLength(100);
        builder.Property(f => f.DataInscricaoPis).HasColumnName("DataInscricaoPis");

        builder.Property(f => f.NomePai).HasColumnName("NomePai").HasMaxLength(100);
        builder.Property(f => f.NomeMae).HasColumnName("NomeMae").HasMaxLength(100);
        builder.Property(f => f.Observacao).HasColumnName("Observacao");
        builder.Property(f => f.Ativo).HasColumnName("Ativo").IsRequired();

        builder.Property(f => f.IdRaca).HasColumnName("IdRhRaca");
        builder.Property(f => f.IdTipoDeficiencia).HasColumnName("IdRhTipoDeficiencia");
        builder.Property(f => f.IdPais).HasColumnName("IdGloPais");

        // Auditoria
        builder.Property(f => f.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(f => f.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(f => f.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(f => f.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(f => f.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(f => f.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamento 1:1 com Entidade
        builder.HasOne(f => f.Entidade)
            .WithOne()
            .HasForeignKey<Funcionario>(f => f.IdFuncionario);
    }
}

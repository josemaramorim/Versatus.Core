using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class EntidadeMapping : IEntityTypeConfiguration<Entidade>
{
    public void Configure(EntityTypeBuilder<Entidade> builder)
    {
        builder.ToTable("GloEntidade");

        builder.HasKey(e => e.IdEntidade);

        builder.Property(e => e.IdEntidade)
            .HasColumnName("IdGloEntidade")
            .ValueGeneratedNever();

        builder.Property(e => e.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Email).HasColumnName("Email").HasMaxLength(100);
        builder.Property(e => e.EmailNFE).HasColumnName("EmailNFE").HasMaxLength(100);
        builder.Property(e => e.EmailFinanceiro).HasColumnName("EmailFinanceiro").HasMaxLength(100);
        builder.Property(e => e.EmailVenda).HasColumnName("EmailVenda").HasMaxLength(100);
        builder.Property(e => e.EmailCompra).HasColumnName("EmailCompra").HasMaxLength(100);
        builder.Property(e => e.HomePage).HasColumnName("HomePage").HasMaxLength(200);
        builder.Property(e => e.Observacao).HasColumnName("Observacao");
        builder.Property(e => e.InscricaoEstadual).HasColumnName("InscricaoEstadual").HasMaxLength(20);
        builder.Property(e => e.InscricaoMunicipal).HasColumnName("InscricaoMunicipal").HasMaxLength(20);
        builder.Property(e => e.InscricaoSuframa).HasColumnName("InscricaoSuframa").HasMaxLength(20);
        builder.Property(e => e.Ativo).HasColumnName("Ativo").IsRequired();

        // Papéis (Roles)
        builder.Property(e => e.IsCliente).HasColumnName("Cliente").IsRequired();
        builder.Property(e => e.IsFornecedor).HasColumnName("Fornecedor").IsRequired();
        builder.Property(e => e.IsTransportadora).HasColumnName("Transportadora").IsRequired();
        builder.Property(e => e.IsComissionado).HasColumnName("Comissionado").IsRequired();
        builder.Property(e => e.IsAgenciaBancaria).HasColumnName("AgenciaBancaria").IsRequired();
        builder.Property(e => e.IsInstituicaoFinanceira).HasColumnName("InstituicaoFinanceira").IsRequired();
        builder.Property(e => e.IsFilial).HasColumnName("Filial").IsRequired();
        builder.Property(e => e.IsFuncionario).HasColumnName("Funcionario").IsRequired();
        builder.Property(e => e.IsObra).HasColumnName("Obra").IsRequired();
        builder.Property(e => e.IsRepresentante).HasColumnName("Representante").IsRequired();
        builder.Property(e => e.IsOutro).HasColumnName("Outro").IsRequired();
        builder.Property(e => e.IsProspecto).HasColumnName("Prospecto").IsRequired();
        builder.Property(e => e.IsContador).HasColumnName("Contador").IsRequired();
        builder.Property(e => e.IsAluno).HasColumnName("Aluno").IsRequired();
        builder.Property(e => e.IsProfessor).HasColumnName("Professor").IsRequired();
        builder.Property(e => e.IsIntermediadorComercial).HasColumnName("IntermediadorComercial").IsRequired();

        builder.Property(e => e.TipoPessoa).HasColumnName("IdFisicaJuridica").IsRequired();
        builder.Property(e => e.ContribuinteICMS).HasColumnName("IDINDICADORCONTRIBUINTEICMS").IsRequired();
        builder.Property(e => e.StatusCnpjCpf).HasColumnName("IdStatusCnpjCpf").IsRequired();
        builder.Property(e => e.IdTipoPlataforma).HasColumnName("IDTIPOPLATAFORMA").IsRequired();

        // Auditoria
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(e => e.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(e => e.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(e => e.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(e => e.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(e => e.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos 1:1
        builder.HasOne(e => e.PessoaFisica)
            .WithOne(p => p.Entidade)
            .HasForeignKey<DadosPessoaFisica>(p => p.IdEntidade);

        builder.HasOne(e => e.PessoaJuridica)
            .WithOne(p => p.Entidade)
            .HasForeignKey<DadosPessoaJuridica>(p => p.IdEntidade);
    }
}

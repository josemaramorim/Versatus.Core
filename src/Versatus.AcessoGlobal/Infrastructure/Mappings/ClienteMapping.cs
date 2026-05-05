using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("GloCliente");

        builder.HasKey(c => c.IdCliente);

        builder.Property(c => c.IdCliente)
            .HasColumnName("IdGloCliente")
            .ValueGeneratedNever();

        builder.Property(c => c.LocalTrabalho).HasColumnName("LocalTrabalho").HasMaxLength(100);
        builder.Property(c => c.TelefoneTrabalho).HasColumnName("TelefoneTrabalho").HasMaxLength(20);
        builder.Property(c => c.Profissao).HasColumnName("Profissao").HasMaxLength(100);
        builder.Property(c => c.InscricaoProdutor).HasColumnName("InscricaoProdutor").HasMaxLength(20);
        builder.Property(c => c.CodigoAlternativo).HasColumnName("CodigoAlternativo").HasMaxLength(20);

        builder.Property(c => c.Ativo).HasColumnName("Ativo").IsRequired();
        builder.Property(c => c.Bloqueado).HasColumnName("Bloqueado").IsRequired();
        builder.Property(c => c.ItemFinanceiroPadrao).HasColumnName("ItemFinanceiroPadrao").IsRequired();
        builder.Property(c => c.EnviarCNDNFe).HasColumnName("EnviarCNDNFe").IsRequired();

        builder.Property(c => c.RendaMensal).HasColumnName("RendaMensal");
        builder.Property(c => c.LimiteCredito).HasColumnName("LimiteCredito");
        builder.Property(c => c.ValorAluguel).HasColumnName("ValorAluguel");

        builder.Property(c => c.DataAdmissao).HasColumnName("DataAdmissao");
        builder.Property(c => c.HoraCobranca).HasColumnName("HoraCobranca");

        builder.Property(c => c.ImovelTipo).HasColumnName("IdImovel");
        builder.Property(c => c.SituacaoSPC).HasColumnName("SituacaoClienteSPC");

        builder.Property(c => c.IdCategoria).HasColumnName("IdGloCategoria");
        builder.Property(c => c.IdClienteConceito).HasColumnName("IdGloClienteConceito");
        builder.Property(c => c.IdDiaSemanaCobranca).HasColumnName("IdDiaSemanaCobranca");

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(c => c.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(c => c.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamento 1:1 com Entidade
        builder.HasOne(c => c.Entidade)
            .WithOne()
            .HasForeignKey<Cliente>(c => c.IdCliente);

        builder.HasOne(c => c.Categoria)
            .WithMany()
            .HasForeignKey(c => c.IdCategoria);
    }
}

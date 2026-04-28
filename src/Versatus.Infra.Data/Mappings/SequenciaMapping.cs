using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.Framework.Domain.Entities;

namespace Versatus.Infra.Data.Mappings;

/// <summary>
/// Mapeamento da entidade Sequencia para a tabela GloSequencialItem.
/// </summary>
public class SequenciaMapping : IEntityTypeConfiguration<Sequencia>
{
    public void Configure(EntityTypeBuilder<Sequencia> builder)
    {
        // Mapeia para a tabela que armazena os valores atuais por filial/empresa
        builder.ToTable("GloSequencialItem");

        // Chave composta conforma legado
        builder.HasKey(s => new { s.Tabela, s.IdFilial });

        builder.Property(s => s.Tabela)
            .HasColumnName("Tabela")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.IdFilial)
            .HasColumnName("IdGloFilial")
            .IsRequired();

        builder.Property(s => s.ValorAtual)
            .HasColumnName("Numero")
            .IsRequired();
    }
}

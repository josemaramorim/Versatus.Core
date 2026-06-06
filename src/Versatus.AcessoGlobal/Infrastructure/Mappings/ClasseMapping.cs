using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Classification;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class ClasseMapping : IEntityTypeConfiguration<Classe>
{
    public void Configure(EntityTypeBuilder<Classe> builder)
    {
        builder.ToTable("FinClasse");

        builder.HasKey(c => c.IdClasse);

        builder.Property(c => c.IdClasse)
            .HasColumnName("IdFinClasse")
            .ValueGeneratedNever();

        builder.Property(c => c.IdFilial)
            .HasColumnName("IdGloFilial")
            .IsRequired();

        builder.Property(c => c.IdClassePai)
            .HasColumnName("IdFinClassePai");

        builder.Property(c => c.Nome)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CodigoFormatado)
            .HasColumnName("Extenso")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Nivel)
            .HasColumnName("Nivel")
            .IsRequired();

        builder.Property(c => c.IdTipoNatureza)
            .HasColumnName("IdNatureza")
            .IsRequired();

        builder.Property(c => c.IdSinteticoAnalitico)
            .HasColumnName("IdTipo")
            .IsRequired();

        builder.Property(c => c.Ativo)
            .HasColumnName("Ativo")
            .IsRequired();

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(c => c.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(c => c.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HoraAlteracao");

        // Relacionamentos
        builder.HasOne(c => c.Filial)
            .WithMany()
            .HasForeignKey(c => c.IdFilial)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ClassePai)
            .WithMany(c => c.SubClasses)
            .HasForeignKey(c => c.IdClassePai)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

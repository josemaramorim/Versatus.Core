using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class RegraTributacaoEspecialMapping : IEntityTypeConfiguration<RegraTributacaoEspecial>
{
    public void Configure(EntityTypeBuilder<RegraTributacaoEspecial> builder)
    {
        builder.ToTable("TRBREGRATRIBUTACAOESPECIAL");

        // Chave primária composta
        builder.HasKey(e => new { e.IdRegraTributoConfiguracao, e.IdTributoFormula, e.IdRegraTributoEspecial });

        builder.Property(e => e.IdRegraTributoConfiguracao)
            .HasColumnName("IDTRBREGRATRIBUTOCONFIGURACAO")
            .ValueGeneratedNever();

        builder.Property(e => e.IdTributoFormula)
            .HasColumnName("IDTRBFORMULA")
            .ValueGeneratedNever();

        builder.Property(e => e.IdRegraTributoEspecial)
            .HasColumnName("IDTRBREGRATRIBUTOESPECIAL")
            .ValueGeneratedNever();

        builder.Property(e => e.Ordem)
            .HasColumnName("ORDEM")
            .IsRequired();

        // Auditoria
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(e => e.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(e => e.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(e => e.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(e => e.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(e => e.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(e => e.Configuracao)
            .WithMany(c => c.Itens)
            .HasForeignKey(e => e.IdRegraTributoConfiguracao)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.RegraEspecial)
            .WithMany()
            .HasForeignKey(e => e.IdRegraTributoEspecial)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

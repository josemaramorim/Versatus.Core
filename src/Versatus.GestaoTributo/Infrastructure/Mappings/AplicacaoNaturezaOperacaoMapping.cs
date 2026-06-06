using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class AplicacaoNaturezaOperacaoMapping : IEntityTypeConfiguration<AplicacaoNaturezaOperacao>
{
    public void Configure(EntityTypeBuilder<AplicacaoNaturezaOperacao> builder)
    {
        builder.ToTable("TRBAPLICACAONATUREZAOPERACAO");

        builder.HasKey(ano => ano.IdAplicacaoNaturezaOperacao);

        builder.Property(ano => ano.IdAplicacaoNaturezaOperacao)
            .HasColumnName("IDTRBAPLICACAONATUREZAOPERACAO")
            .ValueGeneratedNever();

        builder.Property(ano => ano.IdAplicacaoProduto)
            .HasColumnName("IDTRBAPLICACAOPRODUTO")
            .IsRequired();

        builder.Property(ano => ano.IdNaturezaOperacao)
            .HasColumnName("IDTRBNATUREZAOPERACAO")
            .IsRequired();

        // Auditoria
        builder.Property(ano => ano.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(ano => ano.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(ano => ano.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(ano => ano.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(ano => ano.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(ano => ano.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(ano => ano.AplicacaoProduto)
            .WithMany(ap => ap.Naturezas)
            .HasForeignKey(ano => ano.IdAplicacaoProduto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

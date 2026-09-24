using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINCOBRADOR (13 colunas) — analysis/E3-caixa-banco.md §2.6.
public class CobradorMapping : IEntityTypeConfiguration<Cobrador>
{
    public void Configure(EntityTypeBuilder<Cobrador> builder)
    {
        builder.ToTable("FINCOBRADOR");

        // PK_FINCOBRADOR (IDFINCOBRADOR, IDGLOFILIAL) — sequencial via GeradorSequencialService.
        builder.HasKey(x => new { x.IdCobrador, x.IdFilial });

        builder.Property(x => x.IdCobrador).HasColumnName("IDFINCOBRADOR").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();
        builder.Property(x => x.IdEntidade).HasColumnName("IDGLOENTIDADE").IsRequired();

        builder.Property(x => x.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Ativo).HasColumnName("ATIVO").IsRequired();
        builder.Property(x => x.IdUsuario).HasColumnName("IDGLOUSUARIO");
        builder.Property(x => x.IdMeioContato).HasColumnName("IDGLOMEIOCONTATO");

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

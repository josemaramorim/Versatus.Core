using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINCAIXABANCO (17 colunas) — analysis/E3-caixa-banco.md §2.1; legacy-schema/fin_columns.txt.
public class CaixaBancoMapping : IEntityTypeConfiguration<CaixaBanco>
{
    public void Configure(EntityTypeBuilder<CaixaBanco> builder)
    {
        builder.ToTable("FINCAIXABANCO");

        // PK_FINCAIXABANCO (IDFINCAIXABANCO, IDGLOFILIAL) — sequencial via GeradorSequencialService.
        builder.HasKey(x => new { x.IdCaixaBanco, x.IdFilial });

        builder.Property(x => x.IdCaixaBanco).HasColumnName("IDFINCAIXABANCO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        builder.Property(x => x.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.TipoConta)
            .HasColumnName("IDTIPOCONTA")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Ativo).HasColumnName("ATIVO").IsRequired();
        builder.Property(x => x.EntraFluxoCaixa).HasColumnName("ENTRAFLUXOCAIXA").IsRequired();
        builder.Property(x => x.UltimaDataConferida).HasColumnName("ULTIMADATACONFERIDA").HasColumnType("datetime");
        builder.Property(x => x.Saldo).HasColumnName("SALDO").HasPrecision(23, 8);

        builder.Property(x => x.ContaContabil)
            .HasColumnName("CONTACONTABIL")
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(x => x.IdPlanoContabil).HasColumnName("IDCONPLANOCONTABIL");

        builder.Property(x => x.TipoContaCaixa)
            .HasColumnName("IDTIPOCONTACAIXA")
            .HasConversion<int>();

        // Agregado: usuários do caixa (FK_FINCAIXABANCOUSUARIO_FINCAIXABANCO) — Artigo V.
        builder.HasMany(x => x.Usuarios)
            .WithOne()
            .HasForeignKey(u => new { u.IdCaixaBanco, u.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Usuarios)
            .HasField("_usuarios")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

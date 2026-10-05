using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIO (23 colunas) — analysis/E2-dominio.md §2.1; legacy-schema/fin_columns.txt.
public class DominioMapping : IEntityTypeConfiguration<Dominio>
{
    public void Configure(EntityTypeBuilder<Dominio> builder)
    {
        builder.ToTable("FINDOMINIO");

        // PK_FINDOMINIO (IDFINDOMINIO, IDGLOFILIAL) — sequencial via GeradorSequencialService.
        builder.HasKey(x => new { x.IdDominio, x.IdFilial });

        builder.Property(x => x.IdDominio).HasColumnName("IDFINDOMINIO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        // FK_FINDOMINIO_FINCAIXABANCO (IDFINCAIXABANCO, IDGLOFILIAL)
        builder.Property(x => x.IdCaixaBanco).HasColumnName("IDFINCAIXABANCO").IsRequired();
        builder.HasOne<CaixaBanco>()
            .WithMany()
            .HasForeignKey(x => new { x.IdCaixaBanco, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        // FK_FINDOMINIO_FINDOMINIOPERIODO (IDGLOFILIAL, IDFINDOMINIOPERIODO) — ponteiro para o
        // período corrente. Junto com FK_FINDOMINIOPERIODO_FINDOMINIO forma um ciclo; o legado
        // grava o período primeiro e depois faz UPDATE do ponteiro (OP-E2-03/06).
        builder.Property(x => x.IdDominioPeriodo).HasColumnName("IDFINDOMINIOPERIODO");
        builder.HasOne<DominioPeriodo>()
            .WithMany()
            .HasForeignKey(x => new { x.IdDominioPeriodo, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Descricao)
            .HasColumnName("DESCRICAO")
            .HasMaxLength(100)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.Ativo).HasColumnName("ATIVO").IsRequired();
        builder.Property(x => x.Tesouraria).HasColumnName("TESOURARIA").IsRequired();
        builder.Property(x => x.AbrirPeriodo).HasColumnName("ABRIRPERIODO").IsRequired();
        builder.Property(x => x.FecharPeriodo).HasColumnName("FECHARPERIODO").IsRequired();
        builder.Property(x => x.AbrirOutrosPeriodos).HasColumnName("ABRIROUTROSPERIODOS").IsRequired();
        builder.Property(x => x.FecharOutrosPeriodos).HasColumnName("FECHAROUTROSPERIODOS").IsRequired();
        builder.Property(x => x.MovimentoBanco).HasColumnName("MOVIMENTOBANCO").IsRequired();
        builder.Property(x => x.ConsultaTodosPeriodos).HasColumnName("CONSULTATODOSPERIODOS").IsRequired();

        builder.Property(x => x.Saldo).HasColumnName("SALDO").HasPrecision(23, 8);
        builder.Property(x => x.SaldoDinheiro).HasColumnName("SALDODINHEIRO").HasPrecision(23, 8);
        builder.Property(x => x.SaldoChequeRecebido).HasColumnName("SALDOCHEQUERECEBIDO").HasPrecision(23, 8);
        builder.Property(x => x.SaldoCartao).HasColumnName("SALDOCARTAO").HasPrecision(23, 8);

        // Agregados — Artigo V. FK_FINDOMINIOUSUARIO_FINDOMINIO e FK_FINDOMINIORESPONSAVEL_FINDOMINIO
        // (o lado "pai" do responsável é configurado em DominioResponsavelMapping).
        builder.HasMany(x => x.Usuarios)
            .WithOne()
            .HasForeignKey(u => new { u.IdDominio, u.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Usuarios)
            .HasField("_usuarios")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Responsaveis)
            .WithOne()
            .HasForeignKey(r => new { r.IdDominio, r.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Responsaveis)
            .HasField("_responsaveis")
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

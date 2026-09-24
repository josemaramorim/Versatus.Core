using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINCONTABANCARIA (55 colunas) — analysis/E3-caixa-banco.md §2.3. Escopo E3 = núcleo
// (12 colunas + auditoria) + ENVIARSPED/CPFCNPJ (E3-T05); as colunas [E14] entram no épico E14.
public class ContaBancariaMapping : IEntityTypeConfiguration<ContaBancaria>
{
    public void Configure(EntityTypeBuilder<ContaBancaria> builder)
    {
        builder.ToTable("FINCONTABANCARIA");

        // PK_FINCONTABANCARIA (IDFINCAIXABANCO, IDGLOFILIAL) — 1:1 com FINCAIXABANCO
        // (FK_FINCONTABANCARIA_FINCAIXABANCO).
        builder.HasKey(x => new { x.IdCaixaBanco, x.IdFilial });

        builder.Property(x => x.IdCaixaBanco).HasColumnName("IDFINCAIXABANCO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        builder.HasOne<CaixaBanco>()
            .WithOne()
            .HasForeignKey<ContaBancaria>(x => new { x.IdCaixaBanco, x.IdFilial })
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IdAgencia).HasColumnName("IDGLOAGENCIA").IsRequired();

        builder.Property(x => x.Titular)
            .HasColumnName("TITULAR")
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(x => x.NumeroConta)
            .HasColumnName("NUMEROCONTA")
            .HasMaxLength(15)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(x => x.DigitoConta)
            .HasColumnName("DIGITOCONTA")
            .HasMaxLength(2)
            .IsUnicode(false);

        builder.Property(x => x.Limite).HasColumnName("LIMITE").HasPrecision(23, 8);
        builder.Property(x => x.CreditoPendente).HasColumnName("CREDITOPENDENTE").HasPrecision(23, 8);
        builder.Property(x => x.DebitoPendente).HasColumnName("DEBITOPENDENTE").HasPrecision(23, 8);
        builder.Property(x => x.ChequePendente).HasColumnName("CHEQUEPENDENTE").HasPrecision(23, 8);

        builder.Property(x => x.ContaTerceiro).HasColumnName("CONTATERCEIRO").IsRequired();
        builder.Property(x => x.PermiteEmitirCheque).HasColumnName("PERMITEEMITIRCHEQUE").IsRequired();

        // FK_FINCONTABANCARIA_CONTAVINCULADA (IDFINCONTABANCARIAVINCULADA, IDGLOFILIAL) — só o int
        // lógico; sem navegação.
        builder.Property(x => x.IdContaBancariaVinculada).HasColumnName("IDFINCONTABANCARIAVINCULADA");

        builder.Property(x => x.ContaBancariaTipo)
            .HasColumnName("IDTIPOCONTABANCARIA")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IdInstituicaoFinanceira).HasColumnName("IDGLOINSTITUICAOFINANCEIRA");

        builder.Property(x => x.EnviarSped).HasColumnName("ENVIARSPED").IsRequired();

        builder.Property(x => x.CpfCnpj)
            .HasColumnName("CPFCNPJ")
            .HasMaxLength(14)
            .IsUnicode(false);

        // Colunas [E14] NOT NULL fora da entidade: shadow properties para o INSERT não violar a
        // restrição. Valor = default do construtor legado (ContaBancaria.cs:88-94, todas false).
        // No UPDATE o EF só grava propriedades modificadas — valores existentes são preservados.
        // Substituídas por propriedades reais no épico E14.
        builder.Property<bool>("GeraBoleto").HasColumnName("GERABOLETO").IsRequired();
        builder.Property<bool>("GeraRemessa").HasColumnName("GERAREMESSA").IsRequired();
        builder.Property<bool>("ProcessaRetorno").HasColumnName("PROCESSARETORNO").IsRequired();
        builder.Property<bool>("BoletoBeneficiarioDiferente").HasColumnName("BOLETOBENEFICIARIODIFERENTE").IsRequired();
        builder.Property<bool>("BoletoSacadoAvalista").HasColumnName("BOLETOSACADOAVALISTA").IsRequired();

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

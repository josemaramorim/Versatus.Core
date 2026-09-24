using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINCAIXABANCOUSUARIO (9 colunas) — analysis/E3-caixa-banco.md §2.2.
// O relacionamento com CaixaBanco (agregado) é configurado em CaixaBancoMapping.
public class CaixaBancoUsuarioMapping : IEntityTypeConfiguration<CaixaBancoUsuario>
{
    public void Configure(EntityTypeBuilder<CaixaBancoUsuario> builder)
    {
        builder.ToTable("FINCAIXABANCOUSUARIO");

        // PK_FINCAIXABANCOUSUARIO (IDFINCAIXABANCO, IDGLOFILIAL, IDGLOUSUARIO)
        builder.HasKey(x => new { x.IdCaixaBanco, x.IdFilial, x.IdUsuario });

        builder.Property(x => x.IdCaixaBanco).HasColumnName("IDFINCAIXABANCO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();
        builder.Property(x => x.IdUsuario).HasColumnName("IDGLOUSUARIO").ValueGeneratedNever();

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

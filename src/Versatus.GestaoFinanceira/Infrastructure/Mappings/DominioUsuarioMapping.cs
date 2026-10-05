using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure.Mappings;

// FINDOMINIOUSUARIO (10 colunas) — analysis/E2-dominio.md §2.6.
// O relacionamento com Dominio (agregado) é configurado em DominioMapping.
public class DominioUsuarioMapping : IEntityTypeConfiguration<DominioUsuario>
{
    public void Configure(EntityTypeBuilder<DominioUsuario> builder)
    {
        builder.ToTable("FINDOMINIOUSUARIO");

        // PK_FINDOMINIOUSUARIO (IDFINDOMINIO, IDGLOUSUARIO, IDGLOFILIAL) — filial em 3º.
        builder.HasKey(x => new { x.IdDominio, x.IdUsuario, x.IdFilial });

        builder.Property(x => x.IdDominio).HasColumnName("IDFINDOMINIO").ValueGeneratedNever();
        builder.Property(x => x.IdUsuario).HasColumnName("IDGLOUSUARIO").ValueGeneratedNever();
        builder.Property(x => x.IdFilial).HasColumnName("IDGLOFILIAL").ValueGeneratedNever();

        builder.Property(x => x.UsuarioPrincipal).HasColumnName("USUARIOPRINCIPAL").IsRequired();

        // Auditoria
        builder.Property(x => x.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(x => x.DataInclusao).HasColumnName("DATAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.HoraInclusao).HasColumnName("HORAINCLUSAO").HasColumnType("datetime");
        builder.Property(x => x.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(x => x.DataAlteracao).HasColumnName("DATAALTERACAO").HasColumnType("datetime");
        builder.Property(x => x.HoraAlteracao).HasColumnName("HORAALTERACAO").HasColumnType("datetime");
    }
}

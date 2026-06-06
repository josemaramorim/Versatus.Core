using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class TributoIcmsSubstituicaoEstoqueMapping : IEntityTypeConfiguration<TributoIcmsSubstituicaoEstoque>
{
    public void Configure(EntityTypeBuilder<TributoIcmsSubstituicaoEstoque> builder)
    {
        builder.ToTable("TRBICMSSUBSTITUICAOESTOQUE");

        builder.HasKey(t => t.IdTributoIcmsSubstituicaoEstoque);

        builder.Property(t => t.IdTributoIcmsSubstituicaoEstoque)
            .HasColumnName("IDTRBICMSSUBSTITUICAOESTOQUE")
            .ValueGeneratedNever();

        builder.Property(t => t.IdFilial)
            .HasColumnName("IDGLOFILIAL")
            .IsRequired();

        builder.Property(t => t.IdEstEstoque)
            .HasColumnName("IDESTESTOQUE")
            .IsRequired();

        builder.Property(t => t.IdEmpresa)
            .HasColumnName("IDGLOEMPRESA")
            .IsRequired();

        builder.Property(t => t.DataDocumento)
            .HasColumnName("DATADOCUMENTO")
            .IsRequired();

        builder.Property(t => t.Quantidade)
            .HasColumnName("QUANTIDADE")
            .HasPrecision(18, 8)
            .IsRequired();

        builder.Property(t => t.ValorTotal)
            .HasColumnName("VALORTOTAL")
            .HasPrecision(23, 8)
            .IsRequired();

        builder.Property(t => t.BaseCalculo)
            .HasColumnName("BASECALCULO")
            .HasPrecision(23, 8);

        builder.Property(t => t.Aliquota)
            .HasColumnName("ALIQUOTA")
            .HasPrecision(15, 8);

        builder.Property(t => t.ValorTributo)
            .HasColumnName("VALORTRIBUTO")
            .HasPrecision(23, 8);

        builder.Property(t => t.AliquotaFcp)
            .HasColumnName("ALIQUOTAFCP")
            .HasPrecision(15, 8);

        builder.Property(t => t.ValorFcp)
            .HasColumnName("VALORFCP")
            .HasPrecision(23, 8);

        builder.Property(t => t.BaseCalculoSt)
            .HasColumnName("BASECALCULOST")
            .HasPrecision(23, 8);

        builder.Property(t => t.AliquotaSt)
            .HasColumnName("ALIQUOTAST")
            .HasPrecision(15, 8);

        builder.Property(t => t.ValorSt)
            .HasColumnName("VALORST")
            .HasPrecision(23, 8);

        builder.Property(t => t.AliquotaFcpSt)
            .HasColumnName("ALIQUOTAFCPST")
            .HasPrecision(15, 8);

        builder.Property(t => t.ValorFcpSt)
            .HasColumnName("VALORFCPST")
            .HasPrecision(23, 8);

        builder.Property(t => t.IdOrigem)
            .HasColumnName("IDORIGEM")
            .IsRequired();

        builder.Property(t => t.IdProcessoOrigem)
            .HasColumnName("IDPROCESSOORIGEM")
            .IsRequired();

        builder.Property(t => t.Monofasico)
            .HasColumnName("MONOFASICO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(t => t.IcmsSubstituidoAnterior)
            .HasColumnName("ICMSSUBSTITUIDOANTERIOR")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        // Auditoria
        builder.Property(t => t.IdUsuarioInclusao)
            .HasColumnName("IDGLOUSUARIOINCLUSAO")
            .IsRequired();

        builder.Property(t => t.DataInclusao)
            .HasColumnName("DATAINCLUSAO")
            .IsRequired();

        builder.Property(t => t.HoraInclusao)
            .HasColumnName("HORAINCLUSAO")
            .IsRequired();

        builder.Property(t => t.IdUsuarioAlteracao)
            .HasColumnName("IDGLOUSUARIOALTERACAO");

        builder.Property(t => t.DataAlteracao)
            .HasColumnName("DATAALTERACAO");

        builder.Property(t => t.HoraAlteracao)
            .HasColumnName("HORAALTERACAO");
    }
}

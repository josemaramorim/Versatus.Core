using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Organization;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class EmpresaMapping : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("GloEmpresa");

        builder.HasKey(e => e.IdEmpresa);

        builder.Property(e => e.IdEmpresa)
            .HasColumnName("IdGloEmpresa")
            .ValueGeneratedNever();

        builder.Property(e => e.IdGrupo)
            .HasColumnName("IdGloGrupo")
            .IsRequired();

        builder.Property(e => e.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.IdTributacaoEspecial).HasColumnName("IdTributacaoEspecial");
        builder.Property(e => e.Ativo).HasColumnName("Ativo");

        builder.Property(e => e.MascaraClasse).HasColumnName("MascaraClasse").HasMaxLength(50);
        builder.Property(e => e.MascaraCentroCusto).HasColumnName("MascaraCentroCusto").HasMaxLength(50);
        builder.Property(e => e.MascaraProjeto).HasColumnName("MascaraProjeto").HasMaxLength(50);
        builder.Property(e => e.MascaraPlanoContabil).HasColumnName("MascaraPlanoContabil").HasMaxLength(50);

        builder.Property(e => e.MsgInicial).HasColumnName("MsgInicial");
        builder.Property(e => e.MsgFinal).HasColumnName("MsgFinal");

        // Relacionamentos
        builder.HasOne(e => e.Grupo)
            .WithMany(g => g.Empresas)
            .HasForeignKey(e => e.IdGrupo)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoria
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(e => e.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(e => e.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(e => e.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(e => e.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(e => e.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}

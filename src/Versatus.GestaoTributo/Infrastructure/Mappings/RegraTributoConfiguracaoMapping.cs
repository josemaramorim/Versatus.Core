using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Infrastructure.Mappings;

public class RegraTributoConfiguracaoMapping : IEntityTypeConfiguration<RegraTributoConfiguracao>
{
    public void Configure(EntityTypeBuilder<RegraTributoConfiguracao> builder)
    {
        builder.ToTable("TRBREGREATRIBUTOCONFIGURACAO");

        builder.HasKey(c => c.IdRegraTributoConfiguracao);

        builder.Property(c => c.IdRegraTributoConfiguracao)
            .HasColumnName("IDTRBREGRATRIBUTOCONFIGURACAO")
            .ValueGeneratedNever();

        builder.Property(c => c.VigenciaInicio)
            .HasColumnName("VIGENCIAINICIO")
            .IsRequired();

        builder.Property(c => c.VigenciaFim)
            .HasColumnName("VIGENCIAFIM")
            .IsRequired();

        builder.Property(c => c.IdDefinicaoTributaria)
            .HasColumnName("IDDEFINICAOTRIBUTARIA")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.IdTipoPauta)
            .HasColumnName("IDTIPOPAUTA")
            .HasConversion<int>();

        builder.Property(c => c.IdBaseSubstituicao)
            .HasColumnName("IDBASESUBSTITUICAO")
            .HasConversion<int>();

        builder.Property(c => c.IdValorNaoTributado)
            .HasColumnName("IDVALORNAOTRIBUTADO")
            .HasConversion<int>();

        builder.Property(c => c.IdOperadorValorRetencao)
            .HasColumnName("IDOPERADORVALORRETENCAO")
            .HasConversion<int>();

        builder.Property(c => c.IdRegraTributo)
            .HasColumnName("IDTRBREGRATRIBUTO")
            .IsRequired();

        builder.Property(c => c.Aliquota)
            .HasColumnName("ALIQUOTA")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.PercentualBaseReduzida)
            .HasColumnName("PERCENTUALBASEREDUZIDA")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.TributacaoEspecial)
            .HasColumnName("TRIBUTACAOESPECIAL")
            .HasColumnType("smallint")
            .HasConversion(
                v => v == null ? null : (short?)(v.Value ? 1 : 0),
                v => v == null ? null : (bool?)(v.Value == 1)
            );

        builder.Property(c => c.BasePauta)
            .HasColumnName("BASEPAUTA")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.PercentualLucroIva)
            .HasColumnName("PERCENTUALLUCROIVA")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.DispositivoLegal)
            .HasColumnName("DISPOSITIVOLEGAL")
            .HasMaxLength(2000);

        builder.Property(c => c.ConsideraPessoaFisica)
            .HasColumnName("CONSIDERAPESSOAFISICA")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.ConsideraRegimeSimples)
            .HasColumnName("CONSIDERAREGIMESIMPLES")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.ValorRetencao)
            .HasColumnName("VALORRETENCAO")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.IdComportamentoTributo)
            .HasColumnName("IDCOMPORTAMENTOTRIBUTO")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(c => c.AliquotaCupomFiscal)
            .HasColumnName("ALIQUOTACUPOMFISCAL")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.IdSituacaoTributaria)
            .HasColumnName("IDTRBSITUACAOTRIBUTARIA");

        builder.Property(c => c.CodigoNaturezaReceita)
            .HasColumnName("CODIGONATUREZARECEITA")
            .HasMaxLength(20);

        builder.Property(c => c.IncideBaseSubstituicaoICMS)
            .HasColumnName("INCIDEBASESUBSTITUICAOICMS")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IdTipoBaseCalculoCargaMedia)
            .HasColumnName("IDTIPOBASECALCULOCARGAMEDIA")
            .HasConversion<int>();

        builder.Property(c => c.PercentualCargaMedia)
            .HasColumnName("PERCENTUALCARGAMEDIA")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.IdMotivoDesoneracao)
            .HasColumnName("IDTRBMOTIVODESONERACAO");

        builder.Property(c => c.PartilhaTributo)
            .HasColumnName("PARTILHATRIBUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.AliquotaInterestadual)
            .HasColumnName("ALIQUOTAINTERESTADUAL")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.AplicarFCP)
            .HasColumnName("APLICARFCP")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.AliquotaFCP)
            .HasColumnName("ALIQUOTAFCP")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.AplicarCreditoSn)
            .HasColumnName("APLICARCREDITOSN")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IdObservacaoFiscal)
            .HasColumnName("IDTRBOBSERVACAOFISCAL");

        builder.Property(c => c.IdBeneficioFiscal)
            .HasColumnName("IDTRBBENEFICIOFISCAL");

        builder.Property(c => c.AplicarRepasseST)
            .HasColumnName("APLICARREPASSEST")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.SomarFreteBaseTributo)
            .HasColumnName("SOMARFRETEBASETRIBUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.SomarSeguroBaseTributo)
            .HasColumnName("SOMARSEGUROBASETRIBUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.SomarOutrasDespesasBaseTributo)
            .HasColumnName("SOMAROUTRASDESPESASBASETRIBUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.UsaPautaFiscal)
            .HasColumnName("USAPAUTAFISCAL")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IdRestituicaoICMSST)
            .HasColumnName("IDTRBRESTITUICAOICMSST");

        builder.Property(c => c.AliquotaDiferido)
            .HasColumnName("ALIQUOTADIFERIDO")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.AplicarAliquotaNcm)
            .HasColumnName("APLICARALIQUOTANCM")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.AplicarAliquotaFCPNcm)
            .HasColumnName("APLICARALIQUOTAFCPNCM")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.UsaLimiteCreditoST)
            .HasColumnName("USALIMITECREDITOST")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.PercentualLimiteCreditoST)
            .HasColumnName("PERCENTUALLIMITECREDITOST")
            .HasColumnType("numeric(18,4)")
            .IsRequired();

        builder.Property(c => c.IcmsDescontaBasePisCofins)
            .HasColumnName("ICMSDESCONTABASEPISCOFINS")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IncideBaseIcms)
            .HasColumnName("INCIDEBASEICMS")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.SomarAcrescimoBaseTributo)
            .HasColumnName("SOMARACRESCIMOBASETRIBUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.SubtrairDescontoBaseTributo)
            .HasColumnName("SUBTRAIRDESCONTOBASETRIBUTO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IdTipoExigibilidadeISS)
            .HasColumnName("IDTIPOEXIGIBILIDADEISS")
            .HasConversion<int>();

        builder.Property(c => c.AplicarReducaoAposIncidenciaBaseCalculo)
            .HasColumnName("APLICARREDUCAOAPOSINCIDENCIABASECALCULO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.SubtrairDestacadoValorSubstituicao)
            .HasColumnName("SUBTRAIRDESTACADOVALORSUBSTITUICAO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        builder.Property(c => c.IncideBaseIPIDevolucao)
            .HasColumnName("INCIDEBASEIPIDEVOLUCAO")
            .HasColumnType("smallint")
            .HasConversion<short>()
            .IsRequired();

        // Auditoria
        builder.Property(c => c.IdUsuarioInclusao).HasColumnName("IDGLOUSUARIOINCLUSAO");
        builder.Property(c => c.DataInclusao).HasColumnName("DATAINCLUSAO");
        builder.Property(c => c.HoraInclusao).HasColumnName("HORAINCLUSAO");
        builder.Property(c => c.IdUsuarioAlteracao).HasColumnName("IDGLOUSUARIOALTERACAO");
        builder.Property(c => c.DataAlteracao).HasColumnName("DATAALTERACAO");
        builder.Property(c => c.HoraAlteracao).HasColumnName("HORAALTERACAO");

        // Relacionamentos
        builder.HasOne(c => c.RegraTributo)
            .WithMany(r => r.Configuracoes)
            .HasForeignKey(c => c.IdRegraTributo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.SituacaoTributaria)
            .WithMany()
            .HasForeignKey(c => c.IdSituacaoTributaria)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

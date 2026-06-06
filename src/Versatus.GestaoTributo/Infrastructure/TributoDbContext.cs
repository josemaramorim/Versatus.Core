using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.Rules;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Infrastructure;

public class TributoDbContext : DbContext
{
    public TributoDbContext(DbContextOptions<TributoDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClassificacaoFiscal> ClassificacoesFiscais => Set<ClassificacaoFiscal>();
    public DbSet<ClassificacaoFiscalTributo> ClassificacoesFiscaisTributos => Set<ClassificacaoFiscalTributo>();
    public DbSet<ClassificacaoFiscalTributoFilial> ClassificacoesFiscaisTributosFiliais => Set<ClassificacaoFiscalTributoFilial>();
    public DbSet<Cfop> Cfops => Set<Cfop>();
    public DbSet<Cest> Cests => Set<Cest>();
    public DbSet<CestNCM> CestNcms => Set<CestNCM>();
    public DbSet<CestSegmento> CestSegmentos => Set<CestSegmento>();
    public DbSet<TributoFiscal> TributosFiscais => Set<TributoFiscal>();
    public DbSet<UnidadeFiscal> UnidadesFiscais => Set<UnidadeFiscal>();
    
    public DbSet<SituacaoTributaria> SituacoesTributarias => Set<SituacaoTributaria>();
    public DbSet<Tributacao> Tributacoes => Set<Tributacao>();
    public DbSet<RegraTributo> RegrasTributos => Set<RegraTributo>();
    public DbSet<RegraTributoConfiguracao> RegrasTributosConfiguracoes => Set<RegraTributoConfiguracao>();
    public DbSet<RegraTributacaoEspecial> RegrasTributacoesEspeciais => Set<RegraTributacaoEspecial>();
    public DbSet<AplicacaoProduto> AplicacoesProdutos => Set<AplicacaoProduto>();
    public DbSet<AplicacaoProdutoTributo> AplicacoesProdutosTributos => Set<AplicacaoProdutoTributo>();
    public DbSet<AplicacaoNaturezaOperacao> AplicacoesNaturezasOperacoes => Set<AplicacaoNaturezaOperacao>();
    public DbSet<AplicacaoEspecial> AplicacoesEspeciais => Set<AplicacaoEspecial>();

    public DbSet<GrupoTributarioICMS> GruposTributariosICMS => Set<GrupoTributarioICMS>();
    public DbSet<GrupoTributarioInventarioICMS> GruposTributariosInventarioICMS => Set<GrupoTributarioInventarioICMS>();
    public DbSet<TributoIcmsSubstituicaoEstoque> TributosIcmsSubstituicoesEstoque => Set<TributoIcmsSubstituicaoEstoque>();
    public DbSet<DetalheUfTributacao> DetalhesUfTributacao => Set<DetalheUfTributacao>();
    public DbSet<DetalheCidadeTributacao> DetalhesCidadeTributacao => Set<DetalheCidadeTributacao>();
    public DbSet<RegimeTributarioVigencia> RegimesTributariosVigencias => Set<RegimeTributarioVigencia>();
    public DbSet<SimplesNacional> SimplesNacional => Set<SimplesNacional>();
    public DbSet<SimplesNacionalTributo> SimplesNacionalTributos => Set<SimplesNacionalTributo>();
    public DbSet<PartilhaICMSVigencia> PartilhasICMSVigencias => Set<PartilhaICMSVigencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações Fluent API declaradas neste assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TributoDbContext).Assembly);
    }
}

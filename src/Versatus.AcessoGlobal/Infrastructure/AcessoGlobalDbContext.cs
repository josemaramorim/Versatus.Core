using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Location;
using Versatus.AcessoGlobal.Domain.Organization;
using Versatus.AcessoGlobal.Domain.Classification;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Security;
using Versatus.AcessoGlobal.Domain.Finance;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Infrastructure;

/// <summary>
/// Contexto do Entity Framework Core para o módulo de Acesso Global.
/// </summary>
public class AcessoGlobalDbContext : DbContext
{
    public AcessoGlobalDbContext(DbContextOptions<AcessoGlobalDbContext> options)
        : base(options)
    {
    }

    protected AcessoGlobalDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Estado> Estados => Set<Estado>();
    public DbSet<Cidade> Cidades => Set<Cidade>();
    public DbSet<Bairro> Bairros => Set<Bairro>();
    public DbSet<TipoLogradouro> TiposLogradouro => Set<TipoLogradouro>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();

    public DbSet<Grupo> Grupos => Set<Grupo>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Filial> Filiais => Set<Filial>();

    public DbSet<CentroCusto> CentrosCusto => Set<CentroCusto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Classe> Classes => Set<Classe>();

    public DbSet<Entidade> Entidades => Set<Entidade>();
    public DbSet<DadosPessoaFisica> PessoasFisicas => Set<DadosPessoaFisica>();
    public DbSet<DadosPessoaJuridica> PessoasJuridicas => Set<DadosPessoaJuridica>();
    public DbSet<EntidadeEndereco> EntidadeEnderecos => Set<EntidadeEndereco>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<Transportadora> Transportadoras => Set<Transportadora>();

    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<FormaPagamento> FormasPagamento => Set<FormaPagamento>();
    public DbSet<FormaCobranca> FormasCobranca => Set<FormaCobranca>();
    public DbSet<GrupoCondicaoPagamento> GruposCondicaoPagamento => Set<GrupoCondicaoPagamento>();
    public DbSet<CondicaoPagamento> CondicoesPagamento => Set<CondicaoPagamento>();
    public DbSet<CondicaoPagtoRegra> CondicoesPagtoRegra => Set<CondicaoPagtoRegra>();

    public DbSet<SerieDocumento> SeriesDocumento => Set<SerieDocumento>();
    public DbSet<SerieDocumentoFilial> SeriesDocumentoFilial => Set<SerieDocumentoFilial>();
    public DbSet<Parametro> Parametros => Set<Parametro>();
    public DbSet<ParametroValor> ParametroValores => Set<ParametroValor>();
    public DbSet<TipoEnumerado> TiposEnumerados => Set<TipoEnumerado>();

    public DbSet<GloModulo> Modulos => Set<GloModulo>();
    public DbSet<GloMenu> Menus => Set<GloMenu>();
    public DbSet<GloMenuModulo> MenuModulos => Set<GloMenuModulo>();
    public DbSet<GloMenuMenu> MenuMenus => Set<GloMenuMenu>();
    public DbSet<GloMenuRotina> MenuRotinas => Set<GloMenuRotina>();
    public DbSet<GloRotina> Rotinas => Set<GloRotina>();
    public DbSet<GloFavorito> Favoritos => Set<GloFavorito>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplica todos os mapeamentos definidos neste assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcessoGlobalDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Converte propriedades booleanas para short (smallint no SQL Server legado)
        configurationBuilder.Properties<bool>()
            .HaveConversion<short>();
    }
}

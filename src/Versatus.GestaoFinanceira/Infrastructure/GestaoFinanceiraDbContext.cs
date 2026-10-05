using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Infrastructure;

/// <summary>
/// Contexto de escrita (WriteConnection) do módulo de Gestão Financeira.
/// Mutações (POST/PUT/DELETE) passam por aqui — Artigo VII.4 da constituição.
/// </summary>
public class GestaoFinanceiraDbContext : DbContext
{
    /// <summary>
    /// Histórico de migrations próprio do módulo (decisão do usuário em 2026-09-27, E3-T08): não
    /// mistura com o <c>__EFMigrationsHistory</c> compartilhado. Usado na WebAPI e na fábrica de design.
    /// </summary>
    public const string TabelaHistoricoMigrations = "__EFMigrationsHistory_GestaoFinanceira";

    public GestaoFinanceiraDbContext(DbContextOptions<GestaoFinanceiraDbContext> options)
        : base(options)
    {
    }

    protected GestaoFinanceiraDbContext(DbContextOptions options)
        : base(options)
    {
    }

    // Os DbSet<T> das entidades são registrados por épico (E1/E3 em diante),
    // nas tarefas E?-T04 "DbSets + teste de mapeamento".

    // E3 — Caixa e Banco (E3-T04)
    public DbSet<CaixaBanco> CaixasBanco => Set<CaixaBanco>();
    public DbSet<CaixaBancoUsuario> CaixaBancoUsuarios => Set<CaixaBancoUsuario>();
    public DbSet<ContaBancaria> ContasBancarias => Set<ContaBancaria>();
    public DbSet<SaldoCaixaBanco> SaldosCaixaBanco => Set<SaldoCaixaBanco>();
    public DbSet<SaldoRateio> SaldosRateio => Set<SaldoRateio>(); // sem chave — só leitura
    public DbSet<Cobrador> Cobradores => Set<Cobrador>();

    // E2 — Domínio e Período (E2-T03)
    public DbSet<Dominio> Dominios => Set<Dominio>();
    public DbSet<DominioUsuario> DominioUsuarios => Set<DominioUsuario>();
    public DbSet<DominioResponsavel> DominioResponsaveis => Set<DominioResponsavel>();
    public DbSet<DominioPeriodo> DominioPeriodos => Set<DominioPeriodo>();
    public DbSet<DominioPeriodoFechamento> DominioPeriodoFechamentos => Set<DominioPeriodoFechamento>();
    public DbSet<DominioPeriodoLacto> DominioPeriodoLactos => Set<DominioPeriodoLacto>();
    public DbSet<DominioPeriodoFormaPagto> DominioPeriodoFormasPagto => Set<DominioPeriodoFormaPagto>();
    public DbSet<DominioUsuarioSuprimentoSangria> DominioUsuariosSuprimentoSangria => Set<DominioUsuarioSuprimentoSangria>(); // view — só leitura
    public DbSet<ResumoDominioPeriodoFormaPagto> ResumosDominioPeriodoFormaPagto => Set<ResumoDominioPeriodoFormaPagto>(); // view — só leitura

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações Fluent API (IEntityTypeConfiguration) deste assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestaoFinanceiraDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // bool no domínio -> short (smallint no SQL Server legado) — Artigo III.5.
        configurationBuilder.Properties<bool>()
            .HaveConversion<short>();
    }
}

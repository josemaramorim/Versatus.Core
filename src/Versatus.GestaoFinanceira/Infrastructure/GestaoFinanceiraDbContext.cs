using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Infrastructure;

/// <summary>
/// Contexto de escrita (WriteConnection) do módulo de Gestão Financeira.
/// Mutações (POST/PUT/DELETE) passam por aqui — Artigo VII.4 da constituição.
/// </summary>
public class GestaoFinanceiraDbContext : DbContext
{
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Versatus.GestaoFinanceira.Infrastructure;

/// <summary>
/// Fábrica usada só pelas ferramentas <c>dotnet ef</c> (design-time) — E3-T08. Lê a
/// <c>WriteConnection</c> do appsettings da WebAPI (ou da variável
/// <c>ConnectionStrings__WriteConnection</c>) e usa a tabela de histórico própria do módulo.
/// </summary>
/// <example>
/// dotnet ef migrations add Nome --project src/Versatus.GestaoFinanceira
///     --context GestaoFinanceiraDbContext --output-dir Infrastructure/Migrations
/// </example>
public class GestaoFinanceiraDbContextFactory : IDesignTimeDbContextFactory<GestaoFinanceiraDbContext>
{
    public GestaoFinanceiraDbContext CreateDbContext(string[] args)
    {
        var pastaWebApi = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Versatus.WebAPI"));

        var configuracao = new ConfigurationBuilder()
            .SetBasePath(Directory.Exists(pastaWebApi) ? pastaWebApi : Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var conexao = configuracao.GetConnectionString("WriteConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:WriteConnection não encontrada (appsettings da WebAPI ou variável de ambiente).");

        var opcoes = new DbContextOptionsBuilder<GestaoFinanceiraDbContext>()
            .UseSqlServer(conexao, sql => sql.MigrationsHistoryTable(GestaoFinanceiraDbContext.TabelaHistoricoMigrations))
            .Options;

        return new GestaoFinanceiraDbContext(opcoes);
    }
}

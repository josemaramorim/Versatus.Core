using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Demo;

/// <summary>
/// Fábrica de tempo de design para o AcessoGlobalDbContext, localizada no projeto de inicialização (Startup).
/// </summary>
public class AcessoGlobalDbContextFactory : IDesignTimeDbContextFactory<AcessoGlobalDbContext>
{
    public AcessoGlobalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AcessoGlobalDbContext>();
        
        const string connectionString = "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new AcessoGlobalDbContext(optionsBuilder.Options);
    }
}

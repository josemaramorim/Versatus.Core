using Microsoft.EntityFrameworkCore;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurações de mapeamento serão adicionadas conforme as entidades forem criadas
    }
}

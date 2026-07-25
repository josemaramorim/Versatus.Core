using Microsoft.EntityFrameworkCore;

namespace Versatus.AcessoGlobal.Infrastructure;

/// <summary>
/// Contexto de leitura (Read Replica) do módulo de Acesso Global.
/// Aponta para a réplica de leitura do banco de dados com QueryTrackingBehavior.NoTracking.
/// </summary>
public class AcessoGlobalReadDbContext : AcessoGlobalDbContext
{
    public AcessoGlobalReadDbContext(DbContextOptions<AcessoGlobalReadDbContext> options)
        : base(options)
    {
    }
}

using Microsoft.EntityFrameworkCore;

namespace Versatus.GestaoTributo.Infrastructure;

/// <summary>
/// Contexto de leitura (Read Replica) do módulo de Gestão de Tributos.
/// Aponta para a réplica de leitura do banco de dados com QueryTrackingBehavior.NoTracking.
/// </summary>
public class TributoReadDbContext : TributoDbContext
{
    public TributoReadDbContext(DbContextOptions<TributoReadDbContext> options)
        : base(options)
    {
    }
}

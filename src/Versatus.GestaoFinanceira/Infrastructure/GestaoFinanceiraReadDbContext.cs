using Microsoft.EntityFrameworkCore;

namespace Versatus.GestaoFinanceira.Infrastructure;

/// <summary>
/// Contexto de leitura (ReadConnection) do módulo de Gestão Financeira.
/// Consultas/paginações/lookups (GET) passam por aqui, sempre com
/// QueryTrackingBehavior.NoTracking — Artigo VII.4 da constituição.
/// </summary>
public class GestaoFinanceiraReadDbContext : GestaoFinanceiraDbContext
{
    public GestaoFinanceiraReadDbContext(DbContextOptions<GestaoFinanceiraReadDbContext> options)
        : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
}

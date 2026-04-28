using Microsoft.EntityFrameworkCore;
using Versatus.Framework.Domain.Entities;
using Versatus.Infra.Data.Mappings;

namespace Versatus.Infra.Data.Context;

/// <summary>
/// Contexto principal do Entity Framework Core para o ecossistema Versatus.
/// </summary>
public class VersatusDbContext : DbContext
{
    public VersatusDbContext(DbContextOptions<VersatusDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sequencia> Sequencias => Set<Sequencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica as configurações de mapeamento (Fluent API)
        modelBuilder.ApplyConfiguration(new SequenciaMapping());
    }
}

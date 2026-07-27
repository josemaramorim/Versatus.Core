using Microsoft.EntityFrameworkCore;

namespace Versatus.[Modulo].Infrastructure;

/// <summary>
/// Contexto principal de escrita do módulo [Modulo].
/// </summary>
public class [Modulo]DbContext : DbContext
{
    public [Modulo]DbContext(DbContextOptions<[Modulo]DbContext> options)
        : base(options)
    {
    }

    protected [Modulo]DbContext(DbContextOptions options)
        : base(options)
    {
    }

    // DbSets
    // public DbSet<[Entidade]> [Entidades] => Set<[Entidade]>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof([Modulo]DbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<bool>().HaveConversion<short>();
    }
}

/// <summary>
/// Contexto de leitura (Read Replica) desabilitado de tracking.
/// </summary>
public class [Modulo]ReadDbContext : [Modulo]DbContext
{
    public [Modulo]ReadDbContext(DbContextOptions<[Modulo]ReadDbContext> options)
        : base(options)
    {
    }
}

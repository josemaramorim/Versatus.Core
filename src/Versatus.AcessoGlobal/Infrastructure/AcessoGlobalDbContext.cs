using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Location;

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

    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Estado> Estados => Set<Estado>();
    public DbSet<Cidade> Cidades => Set<Cidade>();
    public DbSet<Bairro> Bairros => Set<Bairro>();
    public DbSet<TipoLogradouro> TiposLogradouro => Set<TipoLogradouro>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplica todos os mapeamentos definidos neste assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcessoGlobalDbContext).Assembly);
    }
}

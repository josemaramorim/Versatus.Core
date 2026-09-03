using Microsoft.EntityFrameworkCore;
using Versatus.Framework.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

/// <summary>
/// Implementação base para repositórios do módulo Gestão Financeira.
/// CQRS leve (Artigo VII.4): escrita via <see cref="Context"/>/<see cref="DbSet"/>;
/// leitura via <see cref="ReadContext"/>/<see cref="ReadDbSet"/> (NoTracking).
/// </summary>
public abstract class GestaoFinanceiraRepositorioBase<TEntity> : IRepositorio<TEntity>
    where TEntity : class
{
    protected readonly GestaoFinanceiraDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected readonly GestaoFinanceiraReadDbContext ReadContext;
    protected readonly DbSet<TEntity> ReadDbSet;

    protected GestaoFinanceiraRepositorioBase(
        GestaoFinanceiraDbContext context,
        GestaoFinanceiraReadDbContext readContext)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        ReadContext = readContext ?? throw new ArgumentNullException(nameof(readContext));
        DbSet = context.Set<TEntity>();
        ReadDbSet = readContext.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await ReadDbSet.FindAsync(new[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await ReadDbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Versatus.Framework.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

/// <summary>
/// Implementação base para repositórios do módulo AcessoGlobal.
/// </summary>
public abstract class AcessoGlobalRepositorioBase<TEntity> : IRepositorio<TEntity> where TEntity : class
{
    protected readonly AcessoGlobalDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected AcessoGlobalRepositorioBase(AcessoGlobalDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
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

using Microsoft.EntityFrameworkCore;
using Versatus.Framework.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

/// <summary>
/// Implementação base para repositórios do módulo AcessoGlobal com suporte a segregação de conexões (CQRS):
/// - Context / DbSet: Escrita na instância principal (Write Connection).
/// - ReadContext / ReadDbSet: Leitura na réplica desabilitada de tracking (Read Connection).
/// </summary>
public abstract class AcessoGlobalRepositorioBase<TEntity> : IRepositorio<TEntity> where TEntity : class
{
    protected readonly AcessoGlobalDbContext Context;
    protected readonly AcessoGlobalReadDbContext ReadContext;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly DbSet<TEntity> ReadDbSet;

    protected AcessoGlobalRepositorioBase(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext)
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
        var entry = Context.Entry(entity);
        var keyValues = entry.Metadata.FindPrimaryKey()?
            .Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();

        if (keyValues != null && keyValues.Length > 0)
        {
            var existingTracked = Context.ChangeTracker.Entries<TEntity>()
                .FirstOrDefault(e => {
                    var entryKeyValues = e.Metadata.FindPrimaryKey()?
                        .Properties.Select(p => e.Property(p.Name).CurrentValue).ToArray();
                    return entryKeyValues != null && entryKeyValues.SequenceEqual(keyValues);
                });

            if (existingTracked != null)
            {
                if (existingTracked.Entity == entity)
                {
                    existingTracked.State = EntityState.Modified;
                    return Task.CompletedTask;
                }
                existingTracked.State = EntityState.Detached;
            }
        }

        entry.State = EntityState.Modified;
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

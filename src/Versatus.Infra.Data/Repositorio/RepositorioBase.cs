using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Versatus.Framework.Repositorio;

namespace Versatus.Infra.Data.Repositorio;

/// <summary>
/// Implementação base para o padrão Repositório usando Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
public abstract class RepositorioBase<TEntity> : IRepositorio<TEntity> where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected RepositorioBase(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }
}

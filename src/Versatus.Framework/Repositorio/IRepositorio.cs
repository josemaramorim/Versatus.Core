using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Versatus.Framework.Repositorio;

/// <summary>
/// Contrato genérico para o padrão Repositório, utilizando EF Core.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
public interface IRepositorio<TEntity> where TEntity : class
{
    /// <summary>
    /// Obtém uma entidade pela sua chave primária.
    /// </summary>
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as entidades do tipo.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma entidade.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste as alterações no banco de dados.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

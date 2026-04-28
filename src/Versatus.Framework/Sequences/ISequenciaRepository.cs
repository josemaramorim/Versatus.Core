using Versatus.Framework.Domain.Entities;

namespace Versatus.Framework.Sequences;

/// <summary>
/// Repositório para Sequencia (a ser implementado com EF Core).
/// </summary>
public interface ISequenciaRepository
{
    Task<Sequencia?> GetByTabelaFilialAsync(string tabela, int idFilial, CancellationToken cancellationToken = default);
    Task AddAsync(Sequencia sequencia, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sequencia sequencia, CancellationToken cancellationToken = default);
}

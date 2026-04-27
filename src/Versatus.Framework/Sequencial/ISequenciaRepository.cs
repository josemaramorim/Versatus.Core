using Versatus.Framework.Domain.Entities;

namespace Versatus.Framework.Sequencial;

/// <summary>
/// Repositório para Sequencia (a ser implementado com EF Core).
/// </summary>
public interface ISequenciaRepository
{
    Task<Sequencia?> GetByTabelaFilialAsync(string tabela, int idFilial);
    Task AddAsync(Sequencia sequencia);
    Task UpdateAsync(Sequencia sequencia);
}
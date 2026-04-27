using Versatus.NFe.Domain.Entities;

namespace Versatus.NFe.Infrastructure.Repositories;

public interface INFeRepository
{
    Task AddAsync(NFeDocumento documento);
    Task<NFeDocumento?> GetByChaveAsync(string chave);
    Task UpdateAsync(NFeDocumento documento);
    Task<IEnumerable<NFeDocumento>> GetPendentesAsync();
}
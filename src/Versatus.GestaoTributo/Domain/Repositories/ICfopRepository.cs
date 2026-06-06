using Versatus.Framework.Repositories;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Domain.Repositories;

public interface ICfopRepository : IRepositorio<Cfop>
{
    Task<IEnumerable<Cfop>> ListarCfopsAsync(int limit = 50, CancellationToken cancellationToken = default);
}

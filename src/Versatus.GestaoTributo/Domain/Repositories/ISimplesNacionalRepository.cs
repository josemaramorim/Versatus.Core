using Versatus.Framework.Repositories;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Domain.Repositories;

public interface ISimplesNacionalRepository : IRepositorio<SimplesNacional>
{
    Task<IEnumerable<SimplesNacional>> ListarSimplesNacionalComTributosAsync(CancellationToken cancellationToken = default);
}

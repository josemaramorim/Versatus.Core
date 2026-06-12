using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface ILocalizacaoService
{
    Task<IEnumerable<Pais>> ListarPaisesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Estado>> ListarEstadosAsync(int? idPais = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cidade>> ListarCidadesAsync(int? idEstado = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bairro>> ListarBairrosAsync(int? idCidade = null, CancellationToken cancellationToken = default);
}

using Versatus.Framework.Repositories;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Domain.Repositories;

public interface IRegraTributoConfiguracaoRepository : IRepositorio<RegraTributoConfiguracao>
{
    Task<RegraTributoConfiguracao?> ObterComDetalhesAsync(int id, CancellationToken cancellationToken = default);
}

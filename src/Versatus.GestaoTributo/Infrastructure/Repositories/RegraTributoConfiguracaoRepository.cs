using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.Rules;
using Versatus.GestaoTributo.Domain.Repositories;

namespace Versatus.GestaoTributo.Infrastructure.Repositories;

public class RegraTributoConfiguracaoRepository : GestaoTributoRepositorioBase<RegraTributoConfiguracao>, IRegraTributoConfiguracaoRepository
{
    public RegraTributoConfiguracaoRepository(TributoDbContext context) : base(context)
    {
    }

    public async Task<RegraTributoConfiguracao?> ObterComDetalhesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.RegrasTributosConfiguracoes
            .Include(c => c.RegraTributo)
            .Include(c => c.SituacaoTributaria)
            .FirstOrDefaultAsync(c => c.IdRegraTributoConfiguracao == id, cancellationToken);
    }
}

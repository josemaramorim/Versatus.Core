using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.Repositories;

namespace Versatus.GestaoTributo.Infrastructure.Repositories;

public class CfopRepository : GestaoTributoRepositorioBase<Cfop>, ICfopRepository
{
    public CfopRepository(TributoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Cfop>> ListarCfopsAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await Context.Cfops
            .OrderBy(c => c.IdCfop)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}

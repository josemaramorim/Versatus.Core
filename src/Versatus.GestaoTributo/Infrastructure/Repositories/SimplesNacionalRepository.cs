using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.ICMS;
using Versatus.GestaoTributo.Domain.Repositories;

namespace Versatus.GestaoTributo.Infrastructure.Repositories;

public class SimplesNacionalRepository : GestaoTributoRepositorioBase<SimplesNacional>, ISimplesNacionalRepository
{
    public SimplesNacionalRepository(TributoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SimplesNacional>> ListarSimplesNacionalComTributosAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SimplesNacional
            .Include(s => s.Tributos)
            .OrderBy(s => s.IdSimplesNacional)
            .ToListAsync(cancellationToken);
    }
}

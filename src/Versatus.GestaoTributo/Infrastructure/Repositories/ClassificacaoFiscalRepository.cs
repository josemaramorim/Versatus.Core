using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.Repositories;

namespace Versatus.GestaoTributo.Infrastructure.Repositories;

public class ClassificacaoFiscalRepository : GestaoTributoRepositorioBase<ClassificacaoFiscal>, IClassificacaoFiscalRepository
{
    public ClassificacaoFiscalRepository(TributoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ClassificacaoFiscal>> ListarClassificacoesAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await Context.ClassificacoesFiscais
            .OrderBy(c => c.Ncm)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
}

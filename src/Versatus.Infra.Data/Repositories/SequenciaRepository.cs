using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Versatus.Framework.Domain.Entities;
using Versatus.Framework.Repositorio;
using Versatus.Framework.Sequencial;
using Versatus.Infra.Data.Context;

namespace Versatus.Infra.Data.Repositories;

/// <summary>
/// Implementação concreta do repositório de sequenciais.
/// </summary>
public class SequenciaRepository : RepositorioBase<Sequencia>, ISequenciaRepository
{
    public SequenciaRepository(VersatusDbContext context) : base(context)
    {
    }

    public async Task<Sequencia?> GetByTabelaFilialAsync(string tabela, int idFilial, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.Tabela == tabela && s.IdFilial == idFilial, cancellationToken);
    }
}

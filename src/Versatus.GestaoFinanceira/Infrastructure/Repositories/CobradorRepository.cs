using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: servidor/objeto de negócio/gestao.financeira/Cobrador.cs (legado). Tabela: FINCOBRADOR.
public class CobradorRepository(GestaoFinanceiraDbContext context, GestaoFinanceiraReadDbContext readContext)
    : GestaoFinanceiraRepositorioBase<Cobrador>(context, readContext), ICobradorRepository
{
    public Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default)
        => Context.Database.BeginTransactionAsync(cancellationToken);

    public async Task<IReadOnlyList<Cobrador>> ListarAsync(int idFilial, string? texto, bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var query = ReadDbSet.Where(x => x.IdFilial == idFilial);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var termo = texto.Trim();
            query = int.TryParse(termo, out var id)
                ? query.Where(x => x.IdCobrador == id || x.Nome.Contains(termo))
                : query.Where(x => x.Nome.Contains(termo));
        }

        if (ativo.HasValue)
            query = query.Where(x => x.Ativo == ativo.Value);

        return await query.OrderBy(x => x.IdCobrador).ToListAsync(cancellationToken);
    }

    public Task<Cobrador?> ObterAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default)
        => ReadDbSet.FirstOrDefaultAsync(x => x.IdCobrador == idCobrador && x.IdFilial == idFilial, cancellationToken);

    public Task<Cobrador?> ObterParaEdicaoAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(x => x.IdCobrador == idCobrador && x.IdFilial == idFilial, cancellationToken);
}

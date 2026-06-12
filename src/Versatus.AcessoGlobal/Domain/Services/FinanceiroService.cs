using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Finance;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Domain.Services;

public class FinanceiroService : IFinanceiroService
{
    private readonly AcessoGlobalDbContext _context;

    public FinanceiroService(AcessoGlobalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Banco>> ListarBancosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bancos.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FormaPagamento>> ListarFormasPagamentoAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FormasPagamento.ToListAsync(cancellationToken);
    }
}

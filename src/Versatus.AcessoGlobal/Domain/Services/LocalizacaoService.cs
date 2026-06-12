using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Location;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Domain.Services;

public class LocalizacaoService : ILocalizacaoService
{
    private readonly AcessoGlobalDbContext _context;

    public LocalizacaoService(AcessoGlobalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pais>> ListarPaisesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Paises.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Estado>> ListarEstadosAsync(int? idPais = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Estados.AsQueryable();
        if (idPais.HasValue)
        {
            query = query.Where(e => e.IdPais == idPais.Value);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cidade>> ListarCidadesAsync(int? idEstado = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Cidades.AsQueryable();
        if (idEstado.HasValue)
        {
            // Cidade usa SiglaEstado (string) como FK para Estado.Sigla — busca a sigla primeiro
            var sigla = await _context.Estados
                .Where(e => e.IdEstado == idEstado.Value)
                .Select(e => e.Sigla)
                .FirstOrDefaultAsync(cancellationToken);

            if (sigla != null)
            {
                query = query.Where(c => c.SiglaEstado == sigla);
            }
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bairro>> ListarBairrosAsync(int? idCidade = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Bairros.AsQueryable();
        if (idCidade.HasValue)
        {
            query = query.Where(b => b.IdCidade == idCidade.Value);
        }
        return await query.ToListAsync(cancellationToken);
    }
}

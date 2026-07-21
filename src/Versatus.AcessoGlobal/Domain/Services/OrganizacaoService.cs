using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Organization;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Domain.Services;

public class OrganizacaoService : IOrganizacaoService
{
    private readonly AcessoGlobalDbContext _context;

    public OrganizacaoService(AcessoGlobalDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Grupo>> ListarGruposAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Grupos.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Empresa>> ListarEmpresasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Empresas.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Filial>> ListarFiliaisAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Filiais.ToListAsync(cancellationToken);
    }
}

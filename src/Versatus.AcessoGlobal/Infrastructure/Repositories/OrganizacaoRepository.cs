using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Organization;
using Versatus.AcessoGlobal.Domain.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

/// <summary>
/// Repositório para entidades de organização (Grupo → Empresa → Filial).
/// Opera diretamente sobre a réplica de leitura (Read Replica) com NoTracking.
/// </summary>
public class OrganizacaoRepository : IOrganizacaoRepository
{
    private readonly AcessoGlobalReadDbContext _context;

    public OrganizacaoRepository(AcessoGlobalReadDbContext context)
    {
        _context = context;
    }

    // ── Grupo ────────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Grupo>> ListarGruposAsync(CancellationToken cancellationToken = default)
        => await _context.Grupos.OrderBy(g => g.Nome).ToListAsync(cancellationToken);

    public async Task<Grupo?> GetGrupoByIdAsync(int idGrupo, CancellationToken cancellationToken = default)
        => await _context.Grupos
            .Include(g => g.Empresas)
            .FirstOrDefaultAsync(g => g.IdGrupo == idGrupo, cancellationToken);

    // ── Empresa ───────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Empresa>> ListarEmpresasAsync(int? idGrupo = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Empresas.AsQueryable();
        if (idGrupo.HasValue)
            query = query.Where(e => e.IdGrupo == idGrupo.Value);
        return await query.OrderBy(e => e.Nome).ToListAsync(cancellationToken);
    }

    public async Task<Empresa?> GetEmpresaByIdAsync(int idEmpresa, CancellationToken cancellationToken = default)
        => await _context.Empresas
            .Include(e => e.Filiais)
            .FirstOrDefaultAsync(e => e.IdEmpresa == idEmpresa, cancellationToken);

    // ── Filial ────────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Filial>> ListarFiliaisAsync(int? idEmpresa = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Filiais.AsQueryable();
        if (idEmpresa.HasValue)
            query = query.Where(f => f.IdEmpresa == idEmpresa.Value);
        return await query.OrderBy(f => f.Nome).ToListAsync(cancellationToken);
    }

    public async Task<Filial?> GetFilialByIdAsync(int idFilial, CancellationToken cancellationToken = default)
        => await _context.Filiais
            .Include(f => f.Empresa)
            .FirstOrDefaultAsync(f => f.IdFilial == idFilial, cancellationToken);
}

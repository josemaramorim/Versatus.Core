using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Location;
using Versatus.AcessoGlobal.Domain.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

/// <summary>
/// Repositório para entidades geográficas de lookup (Pais, Estado, Cidade, Bairro).
/// Acesso somente-leitura; nenhuma escrita é esperada nessas tabelas via API.
/// </summary>
public class LocalizacaoRepository : ILocalizacaoRepository
{
    private readonly AcessoGlobalDbContext _context;

    public LocalizacaoRepository(AcessoGlobalDbContext context)
    {
        _context = context;
    }

    // ── País ──────────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Pais>> ListarPaisesAsync(CancellationToken cancellationToken = default)
        => await _context.Paises.OrderBy(p => p.Descricao).ToListAsync(cancellationToken);

    public async Task<Pais?> GetPaisByIdAsync(int idPais, CancellationToken cancellationToken = default)
        => await _context.Paises.FirstOrDefaultAsync(p => p.IdPais == idPais, cancellationToken);

    // ── Estado ────────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Estado>> ListarEstadosAsync(int? idPais = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Estados.AsQueryable();
        if (idPais.HasValue)
            query = query.Where(e => e.IdPais == idPais.Value);
        return await query.OrderBy(e => e.Nome).ToListAsync(cancellationToken);
    }

    public async Task<Estado?> GetEstadoByIdAsync(int idEstado, CancellationToken cancellationToken = default)
        => await _context.Estados.FirstOrDefaultAsync(e => e.IdEstado == idEstado, cancellationToken);

    public async Task<Estado?> GetEstadoBySiglaAsync(string sigla, CancellationToken cancellationToken = default)
        => await _context.Estados.FirstOrDefaultAsync(e => e.Sigla == sigla, cancellationToken);

    // ── Cidade ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Cidade usa SiglaEstado (string) como FK — filtra diretamente pela sigla.
    /// </summary>
    public async Task<IEnumerable<Cidade>> ListarCidadesAsync(string? siglaEstado = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Cidades.AsQueryable();
        if (!string.IsNullOrWhiteSpace(siglaEstado))
            query = query.Where(c => c.SiglaEstado == siglaEstado);
        return await query.OrderBy(c => c.Nome).ToListAsync(cancellationToken);
    }

    public async Task<Cidade?> GetCidadeByIdAsync(int idCidade, CancellationToken cancellationToken = default)
        => await _context.Cidades.FirstOrDefaultAsync(c => c.IdCidade == idCidade, cancellationToken);

    // ── Bairro ────────────────────────────────────────────────────────────────
    public async Task<IEnumerable<Bairro>> ListarBairrosAsync(int? idCidade = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Bairros.AsQueryable();
        if (idCidade.HasValue)
            query = query.Where(b => b.IdCidade == idCidade.Value);
        return await query.OrderBy(b => b.Nome).ToListAsync(cancellationToken);
    }

    public async Task<Bairro?> GetBairroByIdAsync(int idBairro, CancellationToken cancellationToken = default)
        => await _context.Bairros.FirstOrDefaultAsync(b => b.IdBairro == idBairro, cancellationToken);
}

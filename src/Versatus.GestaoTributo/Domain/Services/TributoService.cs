using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.ICMS;
using Versatus.GestaoTributo.Domain.Rules;
using Versatus.GestaoTributo.Domain.Repositories;

namespace Versatus.GestaoTributo.Domain.Services;

public class TributoService : ITributoService
{
    private readonly IClassificacaoFiscalRepository _classificacaoFiscalRepository;
    private readonly ICfopRepository _cfopRepository;
    private readonly ISimplesNacionalRepository _simplesNacionalRepository;
    private readonly IRegraTributoConfiguracaoRepository _regraTributoConfiguracaoRepository;
    private readonly Versatus.GestaoTributo.Infrastructure.TributoDbContext _context;

    public TributoService(
        IClassificacaoFiscalRepository classificacaoFiscalRepository,
        ICfopRepository cfopRepository,
        ISimplesNacionalRepository simplesNacionalRepository,
        IRegraTributoConfiguracaoRepository regraTributoConfiguracaoRepository,
        Versatus.GestaoTributo.Infrastructure.TributoDbContext context)
    {
        _classificacaoFiscalRepository = classificacaoFiscalRepository;
        _cfopRepository = cfopRepository;
        _simplesNacionalRepository = simplesNacionalRepository;
        _regraTributoConfiguracaoRepository = regraTributoConfiguracaoRepository;
        _context = context;
    }

    public async Task<IEnumerable<ClassificacaoFiscal>> ListarClassificacoesAsync(int limite = 50, CancellationToken cancellationToken = default)
    {
        return await _classificacaoFiscalRepository.ListarClassificacoesAsync(limite, cancellationToken);
    }

    public async Task<IEnumerable<Cfop>> ListarCfopsAsync(int limite = 50, CancellationToken cancellationToken = default)
    {
        return await _cfopRepository.ListarCfopsAsync(limite, cancellationToken);
    }

    public async Task<IEnumerable<SimplesNacional>> ListarSimplesNacionalComTributosAsync(CancellationToken cancellationToken = default)
    {
        return await _simplesNacionalRepository.ListarSimplesNacionalComTributosAsync(cancellationToken);
    }

    public async Task<RegraTributoConfiguracao?> ObterRegraConfiguracaoComDetalhesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _regraTributoConfiguracaoRepository.ObterComDetalhesAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Cest>> ListarCestsAsync(int limite = 50, CancellationToken cancellationToken = default)
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.Cests.Take(limite), cancellationToken);
    }

    public async Task<IEnumerable<SituacaoTributaria>> ListarSituacoesTributariasAsync(CancellationToken cancellationToken = default)
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.SituacoesTributarias, cancellationToken);
    }

    public async Task<IEnumerable<GrupoTributarioICMS>> ListarGruposTributariosIcmsAsync(CancellationToken cancellationToken = default)
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.GruposTributariosICMS, cancellationToken);
    }

    public async Task<IEnumerable<TributoIcmsSubstituicaoEstoque>> ListarSubstituicoesEstoqueAsync(CancellationToken cancellationToken = default)
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.TributosIcmsSubstituicoesEstoque, cancellationToken);
    }

    public async Task<IEnumerable<RegimeTributarioVigencia>> ListarRegimesVigentesAsync(CancellationToken cancellationToken = default)
    {
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.RegimesTributariosVigencias, cancellationToken);
    }
}

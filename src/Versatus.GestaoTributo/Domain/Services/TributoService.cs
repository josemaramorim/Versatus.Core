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

    public TributoService(
        IClassificacaoFiscalRepository classificacaoFiscalRepository,
        ICfopRepository cfopRepository,
        ISimplesNacionalRepository simplesNacionalRepository,
        IRegraTributoConfiguracaoRepository regraTributoConfiguracaoRepository)
    {
        _classificacaoFiscalRepository = classificacaoFiscalRepository;
        _cfopRepository = cfopRepository;
        _simplesNacionalRepository = simplesNacionalRepository;
        _regraTributoConfiguracaoRepository = regraTributoConfiguracaoRepository;
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
}

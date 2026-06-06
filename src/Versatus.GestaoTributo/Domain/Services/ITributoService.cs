using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.ICMS;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Domain.Services;

public interface ITributoService
{
    Task<IEnumerable<ClassificacaoFiscal>> ListarClassificacoesAsync(int limite = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cfop>> ListarCfopsAsync(int limite = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<SimplesNacional>> ListarSimplesNacionalComTributosAsync(CancellationToken cancellationToken = default);
    Task<RegraTributoConfiguracao?> ObterRegraConfiguracaoComDetalhesAsync(int id, CancellationToken cancellationToken = default);
}

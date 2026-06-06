using Versatus.Framework.Repositories;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Domain.Repositories;

public interface IClassificacaoFiscalRepository : IRepositorio<ClassificacaoFiscal>
{
    Task<IEnumerable<ClassificacaoFiscal>> ListarClassificacoesAsync(int limit = 50, CancellationToken cancellationToken = default);
}

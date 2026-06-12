using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Configuration;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IParametroService
{
    Task<IEnumerable<Parametro>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<string?> ObterValorAsync(string chave, CancellationToken cancellationToken = default);
    Task SalvarValorAsync(string chave, string valor, CancellationToken cancellationToken = default);
}

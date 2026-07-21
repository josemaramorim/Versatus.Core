using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Finance;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IFinanceiroService
{
    Task<IEnumerable<Banco>> ListarBancosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FormaPagamento>> ListarFormasPagamentoAsync(CancellationToken cancellationToken = default);
}

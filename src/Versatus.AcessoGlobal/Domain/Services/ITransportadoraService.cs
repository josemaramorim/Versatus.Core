using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface ITransportadoraService
{
    Task<IEnumerable<TransportadoraResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<Transportadora?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Transportadora> CriarAsync(CriarTransportadoraDto dto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(int id, CriarTransportadoraDto dto, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IClienteService
{
    Task<IEnumerable<ClienteResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente> CriarAsync(CriarClienteDto dto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(int id, CriarClienteDto dto, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IFornecedorService
{
    Task<IEnumerable<FornecedorResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<Fornecedor?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Fornecedor> CriarAsync(CriarFornecedorDto dto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(int id, CriarFornecedorDto dto, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);
}

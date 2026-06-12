using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IFuncionarioService
{
    Task<IEnumerable<FuncionarioResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<Funcionario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Funcionario> CriarAsync(CriarFuncionarioDto dto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(int id, CriarFuncionarioDto dto, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Configuration;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Security;
using Versatus.Framework.Pagination;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IParametroService
{
    Task<IEnumerable<Parametro>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<string?> ObterValorAsync(string chave, CancellationToken cancellationToken = default);
    Task SalvarValorAsync(string chave, string valor, CancellationToken cancellationToken = default);

    Task<PagedResult<ParametroPaginadoDto>> ListarPaginadoAsync(
        int page,
        int limit,
        string sortBy,
        string sortOrder,
        string search,
        CancellationToken cancellationToken = default);

    Task<ParametroPaginadoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ParametroPaginadoDto> CriarAsync(SalvarParametroDto dto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(int id, SalvarParametroDto dto, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);

    Task<List<ParametroPaginadoDto>> ListarPorEscopoAsync(int tipoParametro, int? idPerfil, CancellationToken cancellationToken = default);
    Task SalvarValoresLoteAsync(SalvarValoresParametrosDto dto, CancellationToken cancellationToken = default);
    Task<List<Perfil>> ListarPerfisAsync(CancellationToken cancellationToken = default);
}


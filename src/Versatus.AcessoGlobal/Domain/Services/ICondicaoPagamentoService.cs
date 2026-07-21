using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.Framework.Pagination;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface ICondicaoPagamentoService
{
    Task<PagedResult<CondicaoPagamentoResponseDto>> ListarPaginadoAsync(
        int page,
        int limit,
        string sortBy,
        string sortOrder,
        string search,
        CancellationToken cancellationToken = default);

    Task<CondicaoPagamentoResponseDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CondicaoPagamentoResponseDto> CriarAsync(CriarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default);

    Task AtualizarAsync(int id, EditarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default);

    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<GrupoCondicaoPagamentoResponseDto>> ListarGruposAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<FormaCobrancaResponseDto>> ListarFormasCobrancaAsync(CancellationToken cancellationToken = default);
}

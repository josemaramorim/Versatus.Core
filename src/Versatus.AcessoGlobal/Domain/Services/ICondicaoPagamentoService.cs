using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.Framework.Pagination;
using Versatus.Framework.Validation;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface ICondicaoPagamentoService
{
    Task<PagedResult<CondicaoPagamentoResponseDto>> ListarPaginadoAsync(
        int page,
        int limit,
        string sortBy,
        string sortOrder,
        string search,
        int? disponibilidade = null,
        bool? ativo = null,
        CancellationToken cancellationToken = default);

    Task<CondicaoPagamentoResponseDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<CondicaoPagamentoResponseDto>> CriarAsync(CriarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default);

    Task<Result<CondicaoPagamentoResponseDto>> AtualizarAsync(int id, EditarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default);

    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<GrupoCondicaoPagamentoResponseDto>> ListarGruposAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<FormaCobrancaResponseDto>> ListarFormasCobrancaAsync(CancellationToken cancellationToken = default);
}

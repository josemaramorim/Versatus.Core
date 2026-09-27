using Versatus.Framework.Pagination;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Cadastro de cobrador (FINCOBRADOR) — CRUD simples, sem VAL na Matriz RTV.</summary>
public interface ICobradorService
{
    Task<PagedResult<CobradorDto>> ListarPaginadoAsync(FiltroCobradorDto filtro, CancellationToken cancellationToken = default);

    Task<CobradorDto?> ObterPorIdAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default);

    Task<Result<CobradorDto>> CriarAsync(SalvarCobradorDto dto, CancellationToken cancellationToken = default);

    Task<Result<CobradorDto>> AtualizarAsync(int idCobrador, int idFilial, SalvarCobradorDto dto,
        CancellationToken cancellationToken = default);

    Task<ValidationResult> ExcluirAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default);
}

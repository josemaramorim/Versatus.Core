using Versatus.Framework.Pagination;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Cadastro de cobrador (FINCOBRADOR) — CRUD simples, sem VAL na Matriz RTV.</summary>
public interface ICobradorService
{
    Task<PagedResult<Cobrador>> ListarPaginadoAsync(string? texto, bool? ativo, int page, int limit,
        CancellationToken cancellationToken = default);

    Task<Cobrador?> ObterPorIdAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default);

    Task<Result<Cobrador>> CriarAsync(Cobrador cobrador, CancellationToken cancellationToken = default);

    Task<Result<Cobrador>> AtualizarAsync(Cobrador cobrador, CancellationToken cancellationToken = default);

    Task<ValidationResult> ExcluirAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default);
}

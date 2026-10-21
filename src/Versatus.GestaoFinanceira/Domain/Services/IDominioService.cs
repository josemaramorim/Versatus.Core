using Versatus.Framework.Pagination;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>
/// Cadastro do domínio financeiro (OP-E2-01/02 + VAL-E2-01..15, 28..37, 50) e consultas do
/// domínio (OP-E2-17 central, OP-E2-18 auxiliares).
/// </summary>
public interface IDominioService
{
    Task<PagedResult<DominioListaDto>> ListarPaginadoAsync(FiltroDominioDto filtro, CancellationToken cancellationToken = default);

    Task<DominioDto?> ObterPorIdAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    Task<Result<DominioDto>> CriarAsync(SalvarDominioDto dto, CancellationToken cancellationToken = default);

    Task<Result<DominioDto>> AtualizarAsync(int idDominio, int idFilial, SalvarDominioDto dto, CancellationToken cancellationToken = default);

    Task<ValidationResult> ExcluirAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Domínio do usuário (padrão: o logado) — DominioUsuario.GetDominio.</summary>
    Task<DominioDto?> ObterDominioDoUsuarioAsync(int? idUsuario = null, CancellationToken cancellationToken = default);

    /// <summary>Usuário principal do domínio padrão — Dominio.GetUsuarioPadrao (VAL-E2-28).</summary>
    Task<Result<int?>> ObterUsuarioPadraoAsync(CancellationToken cancellationToken = default);

    /// <summary>Data/hora do último fechamento — Dominio.GetDataUltimoFechamento.</summary>
    Task<UltimoFechamentoDto?> ObterUltimoFechamentoAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Valor do lançamento de abertura do período — DominioPeriodo.RetornarValorAbertura.</summary>
    Task<decimal> ObterValorAberturaAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>Central do domínio do usuário logado — FCentralDominio (OP-E2-17).</summary>
    Task<CentralDominioDto> ObterCentralAsync(CancellationToken cancellationToken = default);
}

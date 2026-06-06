using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

/// <summary>
/// Contrato para o serviço de domínio da Entidade.
/// </summary>
public interface IEntidadeService
{
    /// <summary>
    /// Lista as últimas entidades cadastradas.
    /// </summary>
    Task<IEnumerable<Entidade>> ListarUltimasAsync(int limite = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém uma entidade pelo ID.
    /// </summary>
    Task<Entidade?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria uma nova entidade realizando as validações e geração de sequencial a partir de um DTO.
    /// </summary>
    Task<Entidade> CriarAsync(CriarEntidadeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria uma nova entidade realizando as validações e geração de sequencial.
    /// </summary>
    Task<Entidade> CriarAsync(Entidade entidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    Task AtualizarAsync(Entidade entidade, CancellationToken cancellationToken = default);
}

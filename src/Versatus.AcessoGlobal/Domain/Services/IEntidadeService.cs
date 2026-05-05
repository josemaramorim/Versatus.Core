using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.Services;

/// <summary>
/// Contrato para o serviço de domínio da Entidade.
/// </summary>
public interface IEntidadeService
{
    /// <summary>
    /// Cria uma nova entidade realizando as validações e geração de sequencial.
    /// </summary>
    Task<Entidade> CriarAsync(Entidade entidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    Task AtualizarAsync(Entidade entidade, CancellationToken cancellationToken = default);
}

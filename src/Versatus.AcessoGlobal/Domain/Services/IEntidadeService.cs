using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.Framework.Pagination;

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

    /// <summary>
    /// Lista entidades com paginação e filtros dinâmicos.
    /// </summary>
    Task<PagedResult<Entidade>> ListarPaginadoAsync(
        int pagina, 
        int registrosPorPagina, 
        string ordenarPor, 
        string direcaoOrdenacao, 
        string termoBusca, 
        string papelFiltro, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém os dados completos da entidade.
    /// </summary>
    Task<Entidade?> ObterCompletoPorIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva a entidade completa com transação explícita de "tudo ou nada".
    /// </summary>
    Task<Entidade> SalvarCompletoAsync(SalvarEntidadeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza a entidade completa com transação explícita de "tudo ou nada".
    /// </summary>
    Task<Entidade> AtualizarCompletoAsync(int id, SalvarEntidadeDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui uma entidade e seus papéis vinculados.
    /// </summary>
    Task ExcluirAsync(int id, CancellationToken cancellationToken = default);
}

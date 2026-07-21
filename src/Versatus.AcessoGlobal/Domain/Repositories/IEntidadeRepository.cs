using Versatus.Framework.Repositories;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.Framework.Pagination;

namespace Versatus.AcessoGlobal.Domain.Repositories;

/// <summary>
/// Contrato de repositório para a entidade base Entidade.
/// </summary>
public interface IEntidadeRepository : IRepositorio<Entidade>
{
    /// <summary>
    /// Busca uma entidade pelo CPF.
    /// </summary>
    Task<Entidade?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca uma entidade pelo CNPJ.
    /// </summary>
    Task<Entidade?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca o ID do País associado a uma cidade.
    /// </summary>
    Task<int?> GetPaisIdPorCidadeAsync(int idCidade, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca o ID do País associado ao endereço comercial/residencial da filial.
    /// </summary>
    Task<int?> GetPaisIdPorFilialAsync(int idFilial, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista as últimas entidades cadastradas.
    /// </summary>
    Task<IEnumerable<Entidade>> ListarEntidadesAsync(int limit = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista entidades paginadas com ordenação e filtros.
    /// </summary>
    Task<PagedResult<Entidade>> ListarPaginadoAsync(
        int pagina, 
        int registrosPorPagina, 
        string ordenarPor, 
        string direcaoOrdenacao, 
        string termoBusca, 
        string papelFiltro, 
        int? tipoPessoa = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a entidade completa incluindo todas as sub-tabelas vinculadas.
    /// </summary>
    Task<Entidade?> GetCompletoPorIdAsync(int id, CancellationToken cancellationToken = default);
}

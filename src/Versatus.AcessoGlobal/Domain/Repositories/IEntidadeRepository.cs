using Versatus.Framework.Repositories;
using Versatus.AcessoGlobal.Domain.Entities;

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
}

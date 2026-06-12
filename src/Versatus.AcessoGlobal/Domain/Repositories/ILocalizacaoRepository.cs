using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Domain.Repositories;

/// <summary>
/// Contrato de repositório para entidades geográficas de lookup (Pais, Estado, Cidade, Bairro).
/// </summary>
public interface ILocalizacaoRepository
{
    Task<IEnumerable<Pais>> ListarPaisesAsync(CancellationToken cancellationToken = default);
    Task<Pais?> GetPaisByIdAsync(int idPais, CancellationToken cancellationToken = default);

    Task<IEnumerable<Estado>> ListarEstadosAsync(int? idPais = null, CancellationToken cancellationToken = default);
    Task<Estado?> GetEstadoByIdAsync(int idEstado, CancellationToken cancellationToken = default);
    Task<Estado?> GetEstadoBySiglaAsync(string sigla, CancellationToken cancellationToken = default);

    Task<IEnumerable<Cidade>> ListarCidadesAsync(string? siglaEstado = null, CancellationToken cancellationToken = default);
    Task<Cidade?> GetCidadeByIdAsync(int idCidade, CancellationToken cancellationToken = default);

    Task<IEnumerable<Bairro>> ListarBairrosAsync(int? idCidade = null, CancellationToken cancellationToken = default);
    Task<Bairro?> GetBairroByIdAsync(int idBairro, CancellationToken cancellationToken = default);
}

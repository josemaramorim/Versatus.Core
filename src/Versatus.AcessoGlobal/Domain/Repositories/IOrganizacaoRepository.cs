using Versatus.AcessoGlobal.Domain.Organization;

namespace Versatus.AcessoGlobal.Domain.Repositories;

/// <summary>
/// Contrato de repositório para entidades de organização (Grupo, Empresa, Filial).
/// </summary>
public interface IOrganizacaoRepository
{
    Task<IEnumerable<Grupo>> ListarGruposAsync(CancellationToken cancellationToken = default);
    Task<Grupo?> GetGrupoByIdAsync(int idGrupo, CancellationToken cancellationToken = default);

    Task<IEnumerable<Empresa>> ListarEmpresasAsync(int? idGrupo = null, CancellationToken cancellationToken = default);
    Task<Empresa?> GetEmpresaByIdAsync(int idEmpresa, CancellationToken cancellationToken = default);

    Task<IEnumerable<Filial>> ListarFiliaisAsync(int? idEmpresa = null, CancellationToken cancellationToken = default);
    Task<Filial?> GetFilialByIdAsync(int idFilial, CancellationToken cancellationToken = default);
}

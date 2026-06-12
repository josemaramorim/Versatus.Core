using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.Organization;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IOrganizacaoService
{
    Task<IEnumerable<Grupo>> ListarGruposAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Empresa>> ListarEmpresasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Filial>> ListarFiliaisAsync(CancellationToken cancellationToken = default);
}

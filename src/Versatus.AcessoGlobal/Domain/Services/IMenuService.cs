using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.Framework.Validation;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IMenuService
{
    Task<List<ModuloMenuDto>> ObterArvoreAsync(CancellationToken cancellationToken = default);
    Task<List<FavoritoDto>> ObterFavoritosAsync(int idUsuario = 1, CancellationToken cancellationToken = default);
    Task<Result<FavoritoDto>> AdicionarFavoritoAsync(int idRotina, int idUsuario = 1, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoverFavoritoAsync(int idRotina, int idUsuario = 1, CancellationToken cancellationToken = default);
}

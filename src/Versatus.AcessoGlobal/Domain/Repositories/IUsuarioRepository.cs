using Versatus.Framework.Repositories;
using Versatus.AcessoGlobal.Domain.Security;

namespace Versatus.AcessoGlobal.Domain.Repositories;

public interface IUsuarioRepository : IRepositorio<Usuario>
{
    Task<Usuario?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
}

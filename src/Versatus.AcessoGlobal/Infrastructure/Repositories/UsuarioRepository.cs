using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Security;
using Versatus.AcessoGlobal.Domain.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

public class UsuarioRepository : AcessoGlobalRepositorioBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AcessoGlobalDbContext context) : base(context) { }

    public async Task<Usuario?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        return await Context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }
}

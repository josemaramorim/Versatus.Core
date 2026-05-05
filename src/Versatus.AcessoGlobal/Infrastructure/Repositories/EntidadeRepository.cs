using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

public class EntidadeRepository : AcessoGlobalRepositorioBase<Entidade>, IEntidadeRepository
{
    public EntidadeRepository(AcessoGlobalDbContext context) : base(context)
    {
    }

    public async Task<Entidade?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        return await Context.Entidades
            .Include(e => e.PessoaFisica)
            .FirstOrDefaultAsync(e => e.PessoaFisica != null && e.PessoaFisica.Cpf == cpf, cancellationToken);
    }

    public async Task<Entidade?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await Context.Entidades
            .Include(e => e.PessoaJuridica)
            .FirstOrDefaultAsync(e => e.PessoaJuridica != null && e.PessoaJuridica.Cnpj == cnpj, cancellationToken);
    }

    public override async Task<Entidade?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await Context.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .Include(e => e.Enderecos)
            .FirstOrDefaultAsync(e => e.IdEntidade == (int)id, cancellationToken);
    }
}

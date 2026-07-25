using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

public class ClienteRepository : AcessoGlobalRepositorioBase<Cliente>, IClienteRepository
{
    public ClienteRepository(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext) 
        : base(context, readContext) { }

    public async Task<Cliente?> GetByCodigoAlternativoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Clientes
            .Include(c => c.Entidade)
            .FirstOrDefaultAsync(c => c.CodigoAlternativo == codigo, cancellationToken);
    }
}

public class FornecedorRepository : AcessoGlobalRepositorioBase<Fornecedor>, IFornecedorRepository
{
    public FornecedorRepository(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext) 
        : base(context, readContext) { }

    public async Task<Fornecedor?> GetByCodigoAlternativoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Fornecedores
            .Include(f => f.Entidade)
            .FirstOrDefaultAsync(f => f.CodigoAlternativo == codigo, cancellationToken);
    }
}

public class FuncionarioRepository : AcessoGlobalRepositorioBase<Funcionario>, IFuncionarioRepository
{
    public FuncionarioRepository(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext) 
        : base(context, readContext) { }

    public async Task<Funcionario?> GetByCtpsAsync(string ctps, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Funcionarios
            .Include(f => f.Entidade)
            .FirstOrDefaultAsync(f => f.Ctps == ctps, cancellationToken);
    }
}

public class TransportadoraRepository : AcessoGlobalRepositorioBase<Transportadora>, ITransportadoraRepository
{
    public TransportadoraRepository(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext) 
        : base(context, readContext) { }

    public async Task<Transportadora?> GetByRntrcAsync(string rntrc, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Transportadoras
            .Include(t => t.Entidade)
            .FirstOrDefaultAsync(t => t.Rntrc == rntrc, cancellationToken);
    }
}

public class ParametroRepository : AcessoGlobalRepositorioBase<Versatus.AcessoGlobal.Domain.Configuration.Parametro>, IParametroRepository
{
    public ParametroRepository(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext) 
        : base(context, readContext) { }

    public async Task<Versatus.AcessoGlobal.Domain.Configuration.Parametro?> GetByChaveAsync(string chave, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Parametros
            .FirstOrDefaultAsync(p => p.Chave == chave, cancellationToken);
    }

    public async Task<string?> GetParametroValorAsync(string chave, CancellationToken cancellationToken = default)
    {
        return await (from pv in ReadContext.ParametroValores
                      join p in ReadContext.Parametros on pv.IdParametro equals p.IdParam
                      where p.Chave == chave
                      select pv.Valor).FirstOrDefaultAsync(cancellationToken);
    }
}

using Versatus.Framework.Repositories;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.Repositories;

public interface IClienteRepository : IRepositorio<Cliente>
{
    Task<Cliente?> GetByCodigoAlternativoAsync(string codigo, CancellationToken cancellationToken = default);
}

public interface IFornecedorRepository : IRepositorio<Fornecedor>
{
    Task<Fornecedor?> GetByCodigoAlternativoAsync(string codigo, CancellationToken cancellationToken = default);
}

public interface IFuncionarioRepository : IRepositorio<Funcionario>
{
    Task<Funcionario?> GetByCtpsAsync(string ctps, CancellationToken cancellationToken = default);
}

public interface ITransportadoraRepository : IRepositorio<Transportadora>
{
    Task<Transportadora?> GetByRntrcAsync(string rntrc, CancellationToken cancellationToken = default);
}

public interface IParametroRepository : IRepositorio<Versatus.AcessoGlobal.Domain.Configuration.Parametro>
{
    Task<Versatus.AcessoGlobal.Domain.Configuration.Parametro?> GetByChaveAsync(string chave, CancellationToken cancellationToken = default);
    Task<string?> GetParametroValorAsync(string chave, CancellationToken cancellationToken = default);
}

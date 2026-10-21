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
    /// <summary>
    /// Primeiro valor do parâmetro, sem considerar o escopo. Correto só para parâmetros do tipo
    /// Sistema; para os demais use a sobrecarga com empresa e filial.
    /// </summary>
    Task<string?> GetParametroValorAsync(string chave, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valor do parâmetro no escopo da empresa/filial informadas, resolvido pelo tipo do parâmetro
    /// como no legado (ParametroServidor.ParametroValor): Sistema, Grupo, Empresa ou Filial, sem
    /// recorrer a um nível mais geral. Devolve null quando não há valor no escopo (o legado
    /// devolve o valor padrão do tipo: false, int.MinValue ou ""). O nível Perfil ainda não é
    /// suportado e lança <see cref="NotSupportedException"/>.
    /// </summary>
    Task<string?> GetParametroValorAsync(string chave, int idEmpresa, int idFilial, CancellationToken cancellationToken = default);
}

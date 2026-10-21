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

    // GLOPARAMETRO.IDTIPOPARAMETRO = ParametroTipo do legado (geral/tipoenumerado.cs:918).
    public const int TipoParametroSistema = 159;
    public const int TipoParametroFilial = 160;
    public const int TipoParametroPerfil = 161;
    public const int TipoParametroGrupo = 350;
    public const int TipoParametroEmpresa = 351;

    // acesso.global/ParametroServidor.cs:240-278 (ParametroValor) e RetornarValor{Sistema,Grupo,
    // Empresa,Filial}: cada nível exige a combinação exata das chaves de escopo e não recorre ao
    // nível mais geral. O grupo vem da empresa (GLOEMPRESA.IDGLOGRUPO), pois o contexto não o tem.
    public async Task<string?> GetParametroValorAsync(string chave, int idEmpresa, int idFilial, CancellationToken cancellationToken = default)
    {
        var parametro = await ReadContext.Parametros
            .Where(p => p.Chave == chave)
            .Select(p => new { p.IdParam, p.TipoParametro })
            .FirstOrDefaultAsync(cancellationToken);

        if (parametro is null)
            return null;

        var valores = ReadContext.ParametroValores.Where(v => v.IdParametro == parametro.IdParam);

        switch (parametro.TipoParametro)
        {
            case TipoParametroPerfil:
                throw new NotSupportedException(
                    $"O parâmetro '{chave}' é do nível Perfil, que ainda não é suportado (decisão de 2026-10-21).");

            case TipoParametroGrupo:
            case TipoParametroEmpresa:
            case TipoParametroFilial:
                var idGrupo = await ReadContext.Empresas
                    .Where(e => e.IdEmpresa == idEmpresa)
                    .Select(e => (int?)e.IdGrupo)
                    .FirstOrDefaultAsync(cancellationToken);

                if (idGrupo is null)
                    return null;

                valores = valores.Where(v => v.IdGrupo == idGrupo && v.IdPerfil == null);

                valores = parametro.TipoParametro switch
                {
                    TipoParametroGrupo => valores.Where(v => v.IdEmpresa == null && v.IdFilial == null),
                    TipoParametroEmpresa => valores.Where(v => v.IdEmpresa == idEmpresa && v.IdFilial == null),
                    _ => valores.Where(v => v.IdEmpresa == idEmpresa && v.IdFilial == idFilial),
                };

                return await valores.Select(v => v.Valor).FirstOrDefaultAsync(cancellationToken);

            default:
                // Sistema (e tipo não informado — o legado cai no ramo de Sistema): o primeiro valor
                // da lista (ParametroValores[0]); ordenado pela PK para ser determinístico.
                return await valores
                    .OrderBy(v => v.IdParametroValor)
                    .Select(v => v.Valor)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}

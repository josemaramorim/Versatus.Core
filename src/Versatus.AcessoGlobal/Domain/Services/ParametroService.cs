using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Configuration;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Domain.Services;

public class ParametroService : IParametroService
{
    private readonly IParametroRepository _parametroRepository;
    private readonly AcessoGlobalDbContext _context;

    public ParametroService(IParametroRepository parametroRepository, AcessoGlobalDbContext context)
    {
        _parametroRepository = parametroRepository;
        _context = context;
    }

    public async Task<IEnumerable<Parametro>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _parametroRepository.GetAllAsync(cancellationToken);
    }

    public async Task<string?> ObterValorAsync(string chave, CancellationToken cancellationToken = default)
    {
        return await _parametroRepository.GetParametroValorAsync(chave, cancellationToken);
    }

    public async Task SalvarValorAsync(string chave, string valor, CancellationToken cancellationToken = default)
    {
        var param = await _parametroRepository.GetByChaveAsync(chave, cancellationToken);
        if (param == null)
        {
            throw new InvalidOperationException($"Parâmetro '{chave}' não existe.");
        }

        var paramValor = await _context.ParametroValores
            .FirstOrDefaultAsync(pv => pv.IdParametro == param.IdParam, cancellationToken);

        if (paramValor == null)
        {
            paramValor = new ParametroValor
            {
                IdParametro = param.IdParam,
                Valor = valor
            };
            _context.ParametroValores.Add(paramValor);
        }
        else
        {
            paramValor.Valor = valor;
            _context.ParametroValores.Update(paramValor);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

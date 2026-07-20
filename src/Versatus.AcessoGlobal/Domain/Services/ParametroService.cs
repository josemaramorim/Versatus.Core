using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Configuration;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.Framework.Pagination;
using Versatus.Framework.Sequences;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Domain.Services;

/// <summary>
/// Serviço para manipulação de parâmetros e valores de configuração.
/// Origem: servidor/objeto de negócio/acesso.global/Parametro.cs (legado)
/// </summary>
public class ParametroService : IParametroService
{
    private readonly IParametroRepository _parametroRepository;
    private readonly AcessoGlobalDbContext _context;
    private readonly IGeradorSequencial _geradorSequencial;

    public ParametroService(
        IParametroRepository parametroRepository,
        AcessoGlobalDbContext context,
        IGeradorSequencial geradorSequencial)
    {
        _parametroRepository = parametroRepository;
        _context = context;
        _geradorSequencial = geradorSequencial;
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
            var idParamValor = await _geradorSequencial.ProximoAsync("ParametroValor", SequencialTipo.Geral, cancellationToken);
            paramValor = new ParametroValor
            {
                IdParametroValor = idParamValor,
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

    public async Task<PagedResult<ParametroPaginadoDto>> ListarPaginadoAsync(
        int page,
        int limit,
        string sortBy,
        string sortOrder,
        string search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Parametros.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Chave.Contains(search) ||
                (p.Descricao != null && p.Descricao.Contains(search))
            );
        }

        var total = await query.CountAsync(cancellationToken);

        bool desc = (sortOrder ?? "asc").ToLower() == "desc";
        string sortCol = sortBy ?? "chave";

        switch (sortCol.ToLower())
        {
            case "id":
            case "idparam":
                query = desc ? query.OrderByDescending(p => p.IdParam) : query.OrderBy(p => p.IdParam);
                break;
            case "chave":
            case "nome":
                query = desc ? query.OrderByDescending(p => p.Chave) : query.OrderBy(p => p.Chave);
                break;
            case "descricao":
                query = desc ? query.OrderByDescending(p => p.Descricao) : query.OrderBy(p => p.Descricao);
                break;
            default:
                query = query.OrderBy(p => p.IdParam);
                break;
        }

        var itemsRaw = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var paramIds = itemsRaw.Select(p => p.IdParam).ToList();
        var paramValores = await _context.ParametroValores
            .Where(pv => paramIds.Contains(pv.IdParametro ?? 0))
            .ToListAsync(cancellationToken);

        var items = itemsRaw.Select(p => {
            var val = paramValores.FirstOrDefault(pv => pv.IdParametro == p.IdParam)?.Valor;
            return new ParametroPaginadoDto(
                p.IdParam,
                p.Chave,
                p.Descricao,
                val,
                p.Tipo
            );
        }).ToList();

        return new PagedResult<ParametroPaginadoDto>(items, total);
    }

    public async Task<ParametroPaginadoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var param = await _parametroRepository.GetByIdAsync(id, cancellationToken);
        if (param == null) return null;

        var valor = await _context.ParametroValores
            .Where(pv => pv.IdParametro == param.IdParam)
            .Select(pv => pv.Valor)
            .FirstOrDefaultAsync(cancellationToken);

        return new ParametroPaginadoDto(
            param.IdParam,
            param.Chave,
            param.Descricao,
            valor,
            param.Tipo
        );
    }

    public async Task<ParametroPaginadoDto> CriarAsync(SalvarParametroDto dto, CancellationToken cancellationToken = default)
    {
        var existente = await _parametroRepository.GetByChaveAsync(dto.Chave, cancellationToken);
        if (existente != null)
        {
            throw new InvalidOperationException($"Já existe um parâmetro com a chave '{dto.Chave}'.");
        }

        var idParam = await _geradorSequencial.ProximoAsync("Parametro", SequencialTipo.Geral, cancellationToken);

        var param = new Parametro
        {
            IdParam = idParam,
            Chave = dto.Chave,
            Descricao = dto.Descricao,
            Tipo = dto.Tipo
        };

        await _parametroRepository.AddAsync(param, cancellationToken);
        await _parametroRepository.SaveChangesAsync(cancellationToken);

        string? valorSalvo = null;
        if (dto.Valor != null)
        {
            var idParamValor = await _geradorSequencial.ProximoAsync("ParametroValor", SequencialTipo.Geral, cancellationToken);
            var paramValor = new ParametroValor
            {
                IdParametroValor = idParamValor,
                IdParametro = idParam,
                Valor = dto.Valor
            };
            _context.ParametroValores.Add(paramValor);
            await _context.SaveChangesAsync(cancellationToken);
            valorSalvo = dto.Valor;
        }

        return new ParametroPaginadoDto(
            param.IdParam,
            param.Chave,
            param.Descricao,
            valorSalvo,
            param.Tipo
        );
    }

    public async Task PinkAtualizarAsync(int id, SalvarParametroDto dto, CancellationToken cancellationToken = default)
    {
        // Placeholder para conformidade - mudado abaixo para AtualizarAsync
    }

    public async Task AtualizarAsync(int id, SalvarParametroDto dto, CancellationToken cancellationToken = default)
    {
        var param = await _parametroRepository.GetByIdAsync(id, cancellationToken);
        if (param == null)
        {
            throw new InvalidOperationException($"Parâmetro com ID {id} não encontrado.");
        }

        param.Descricao = dto.Descricao;
        param.Tipo = dto.Tipo;

        await _parametroRepository.UpdateAsync(param, cancellationToken);
        await _parametroRepository.SaveChangesAsync(cancellationToken);

        var paramValor = await _context.ParametroValores
            .FirstOrDefaultAsync(pv => pv.IdParametro == param.IdParam, cancellationToken);

        if (paramValor == null)
        {
            if (dto.Valor != null)
            {
                var idParamValor = await _geradorSequencial.ProximoAsync("ParametroValor", SequencialTipo.Geral, cancellationToken);
                paramValor = new ParametroValor
                {
                    IdParametroValor = idParamValor,
                    IdParametro = param.IdParam,
                    Valor = dto.Valor
                };
                _context.ParametroValores.Add(paramValor);
            }
        }
        else
        {
            paramValor.Valor = dto.Valor;
            _context.ParametroValores.Update(paramValor);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var param = await _parametroRepository.GetByIdAsync(id, cancellationToken);
        if (param == null) return;

        var valores = await _context.ParametroValores
            .Where(pv => pv.IdParametro == param.IdParam)
            .ToListAsync(cancellationToken);

        if (valores.Any())
        {
            _context.ParametroValores.RemoveRange(valores);
        }

        await _parametroRepository.DeleteAsync(param, cancellationToken);
        await _parametroRepository.SaveChangesAsync(cancellationToken);
    }
}

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
using Versatus.Framework.Context;
using Versatus.AcessoGlobal.Domain.Security;
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
    private readonly IContextoExecucao _contexto;

    public ParametroService(
        IParametroRepository parametroRepository,
        AcessoGlobalDbContext context,
        IGeradorSequencial geradorSequencial,
        IContextoExecucao contexto)
    {
        _parametroRepository = parametroRepository;
        _context = context;
        _geradorSequencial = geradorSequencial;
        _contexto = contexto;
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

        // Carrega os dados na memória primeiro devido à limitação do SQL Server 2008 (não suporta OFFSET/FETCH)
        var allItems = await query.ToListAsync(cancellationToken);

        var itemsRaw = allItems
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToList();

        var paramIds = itemsRaw.Select(p => p.IdParam).ToList();
        
        var paramValores = new List<ParametroValor>();
        if (paramIds.Any())
        {
            paramValores = await _context.ParametroValores
                .Where(pv => pv.IdParametro.HasValue && paramIds.Contains(pv.IdParametro.Value))
                .ToListAsync(cancellationToken);
        }

        var items = itemsRaw.Select(p => {
            var pv = paramValores.FirstOrDefault(v => v.IdParametro == p.IdParam);
            return new ParametroPaginadoDto(
                p.IdParam,
                p.Chave,
                p.Descricao,
                p.Valor,
                p.Tipo,
                p.Agrupador,
                p.Visivel,
                p.IdRotina,
                p.TipoParametro,
                pv?.IdParametroValor,
                pv?.Valor,
                pv != null
            );
        }).ToList();

        return new PagedResult<ParametroPaginadoDto>(items, total);
    }

    public async Task<ParametroPaginadoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var param = await _parametroRepository.GetByIdAsync(id, cancellationToken);
        if (param == null) return null;

        var pv = await _context.ParametroValores
            .Where(v => v.IdParametro == param.IdParam)
            .FirstOrDefaultAsync(cancellationToken);

        return new ParametroPaginadoDto(
            param.IdParam,
            param.Chave,
            param.Descricao,
            param.Valor,
            param.Tipo,
            param.Agrupador,
            param.Visivel,
            param.IdRotina,
            param.TipoParametro,
            pv?.IdParametroValor,
            pv?.Valor,
            pv != null
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
            Tipo = dto.Tipo,
            Agrupador = dto.Agrupador,
            Visivel = dto.Visivel,
            IdRotina = dto.IdRotina,
            TipoParametro = dto.TipoParametro
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
            param.Valor,
            param.Tipo,
            param.Agrupador,
            param.Visivel,
            param.IdRotina,
            param.TipoParametro,
            null,
            valorSalvo,
            valorSalvo != null
        );
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
        param.Agrupador = dto.Agrupador;
        param.Visivel = dto.Visivel;
        param.IdRotina = dto.IdRotina;
        param.TipoParametro = dto.TipoParametro;

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

    public async Task<List<ParametroPaginadoDto>> ListarPorEscopoAsync(int tipoParametro, int? idPerfil, CancellationToken cancellationToken = default)
    {
        var query = _context.Parametros.AsQueryable();

        var parametros = await query
            .Where(p => p.Visivel && p.TipoParametro == tipoParametro)
            .OrderBy(p => p.Agrupador)
            .ThenBy(p => p.Chave)
            .ToListAsync(cancellationToken);

        var paramIds = parametros.Select(p => p.IdParam).ToList();

        var paramValoresQuery = _context.ParametroValores.AsQueryable();

        if (tipoParametro == (int)ParametroTipo.Perfil)
        {
            paramValoresQuery = paramValoresQuery.Where(pv => pv.IdPerfil == idPerfil);
        }
        else
        {
            paramValoresQuery = paramValoresQuery.Where(pv => pv.IdPerfil == null);
        }

        if (paramIds.Any())
        {
            paramValoresQuery = paramValoresQuery.Where(pv => pv.IdParametro.HasValue && paramIds.Contains(pv.IdParametro.Value));
        }
        else
        {
            return new List<ParametroPaginadoDto>();
        }

        var paramValores = await paramValoresQuery.ToListAsync(cancellationToken);

        var resultado = parametros.Select(p =>
        {
            var pv = paramValores.FirstOrDefault(v => v.IdParametro == p.IdParam);
            return new ParametroPaginadoDto(
                p.IdParam,
                p.Chave,
                p.Descricao,
                p.Valor,
                p.Tipo,
                p.Agrupador,
                p.Visivel,
                p.IdRotina,
                p.TipoParametro,
                pv?.IdParametroValor,
                pv?.Valor,
                pv != null
            );
        }).ToList();

        return resultado;
    }

    public async Task SalvarValoresLoteAsync(SalvarValoresParametrosDto dto, CancellationToken cancellationToken = default)
    {
        int? idGrupo = 1;
        int? idEmpresa = _contexto.IdEmpresa;
        int? idFilial = _contexto.IdFilial;

        foreach (var item in dto.Valores)
        {
            var pvQuery = _context.ParametroValores
                .Where(pv => pv.IdParametro == item.IdParametro);

            if (dto.TipoParametro == (int)ParametroTipo.Perfil)
            {
                pvQuery = pvQuery.Where(pv => pv.IdPerfil == dto.IdPerfil);
            }
            else
            {
                pvQuery = pvQuery.Where(pv => pv.IdPerfil == null);
            }

            var pv = await pvQuery.FirstOrDefaultAsync(cancellationToken);

            if (!item.Marcado)
            {
                if (pv != null)
                {
                    _context.ParametroValores.Remove(pv);
                }
            }
            else
            {
                if (pv == null)
                {
                    var idParamValor = await _geradorSequencial.ProximoAsync("ParametroValor", SequencialTipo.Geral, cancellationToken);
                    pv = new ParametroValor
                    {
                        IdParametroValor = idParamValor,
                        IdParametro = item.IdParametro,
                        Valor = item.ValorConfigurado ?? string.Empty,
                        IdPerfil = dto.TipoParametro == (int)ParametroTipo.Perfil ? dto.IdPerfil : null,
                        IdFilial = (dto.TipoParametro == (int)ParametroTipo.Filial || dto.TipoParametro == (int)ParametroTipo.Perfil) ? idFilial : null,
                        IdEmpresa = (dto.TipoParametro == (int)ParametroTipo.Empresa || dto.TipoParametro == (int)ParametroTipo.Filial || dto.TipoParametro == (int)ParametroTipo.Perfil) ? idEmpresa : null,
                        IdGrupo = idGrupo
                    };
                    _context.ParametroValores.Add(pv);
                }
                else
                {
                    pv.Valor = item.ValorConfigurado ?? string.Empty;
                    _context.ParametroValores.Update(pv);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Perfil>> ListarPerfisAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Perfis
            .OrderBy(p => p.Descricao)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EnumOpcaoDto>> ObterOpcoesEnumAsync(string enumNome, CancellationToken cancellationToken = default)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Type? enumType = null;
        
        foreach (var assembly in assemblies)
        {
            enumType = assembly.GetType($"Versatus.AcessoGlobal.Domain.Entities.{enumNome}") 
                       ?? assembly.GetType($"Versatus.AcessoGlobal.Domain.Configuration.{enumNome}")
                       ?? assembly.GetType($"Versatus.GestaoTributo.Domain.Rules.{enumNome}");
            
            if (enumType != null && enumType.IsEnum)
                break;
        }

        if (enumType == null || !enumType.IsEnum)
        {
            return new List<EnumOpcaoDto>();
        }

        var values = Enum.GetValues(enumType);
        var result = new List<EnumOpcaoDto>();

        foreach (var val in values)
        {
            var intVal = (int)val;
            var strVal = val.ToString() ?? "";

            var dbDesc = await _context.TiposEnumerados
                .Where(t => t.IdTipoEnumerado == intVal)
                .Select(t => t.Descricao)
                .FirstOrDefaultAsync(cancellationToken);

            result.Add(new EnumOpcaoDto(strVal, dbDesc ?? strVal));
        }

        return result;
    }
}

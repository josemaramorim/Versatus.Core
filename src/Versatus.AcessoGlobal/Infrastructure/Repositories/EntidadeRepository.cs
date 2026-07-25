using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Pagination;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

public class EntidadeRepository : AcessoGlobalRepositorioBase<Entidade>, IEntidadeRepository
{
    public EntidadeRepository(AcessoGlobalDbContext context, AcessoGlobalReadDbContext readContext) 
        : base(context, readContext)
    {
    }

    public async Task<Entidade?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Entidades
            .Include(e => e.PessoaFisica)
            .FirstOrDefaultAsync(e => e.PessoaFisica != null && e.PessoaFisica.Cpf == cpf, cancellationToken);
    }

    public async Task<Entidade?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Entidades
            .Include(e => e.PessoaJuridica)
            .FirstOrDefaultAsync(e => e.PessoaJuridica != null && e.PessoaJuridica.Cnpj == cnpj, cancellationToken);
    }

    public override async Task<Entidade?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .Include(e => e.Enderecos)
            .FirstOrDefaultAsync(e => e.IdEntidade == (int)id, cancellationToken);
    }

    public async Task<int?> GetPaisIdPorCidadeAsync(int idCidade, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Cidades
            .Where(c => c.IdCidade == idCidade)
            .Select(c => (int?)c.IdPais)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int?> GetPaisIdPorFilialAsync(int idFilial, CancellationToken cancellationToken = default)
    {
        return await (from ee in ReadContext.EntidadeEnderecos
                      join c in ReadContext.Cidades on ee.IdCidade equals c.IdCidade
                      where ee.IdEntidade == idFilial && ee.TipoEndereco == EnderecoTipo.ComercialResidencial
                      select (int?)c.IdPais)
                     .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Entidade>> ListarEntidadesAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .OrderByDescending(e => e.IdEntidade)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Entidade>> ListarPaginadoAsync(
        int pagina, 
        int registrosPorPagina, 
        string ordenarPor, 
        string direcaoOrdenacao, 
        string termoBusca, 
        string papelFiltro, 
        int? tipoPessoa = null,
        CancellationToken cancellationToken = default)
    {
        var query = ReadContext.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .AsQueryable();

        if (tipoPessoa.HasValue)
        {
            var tipoEnum = (EntidadeTipoPessoa)tipoPessoa.Value;
            query = query.Where(e => e.TipoPessoa == tipoEnum);
        }

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            query = query.Where(e => 
                (e.Nome != null && e.Nome.Contains(termoBusca)) ||
                (e.PessoaJuridica != null && e.PessoaJuridica.RazaoSocial != null && e.PessoaJuridica.RazaoSocial.Contains(termoBusca)) ||
                (e.PessoaFisica != null && e.PessoaFisica.Cpf != null && e.PessoaFisica.Cpf.Contains(termoBusca)) ||
                (e.PessoaJuridica != null && e.PessoaJuridica.Cnpj != null && e.PessoaJuridica.Cnpj.Contains(termoBusca)) ||
                e.IdEntidade.ToString().Contains(termoBusca)
            );
        }

        if (!string.IsNullOrWhiteSpace(papelFiltro))
        {
            var papeis = papelFiltro.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(p => p.Trim().ToLower())
                                    .ToList();

            if (papeis.Any())
            {
                query = query.Where(e =>
                    (papeis.Contains("iscliente") && e.IsCliente) ||
                    (papeis.Contains("isfornecedor") && e.IsFornecedor) ||
                    (papeis.Contains("isfuncionario") && e.IsFuncionario) ||
                    (papeis.Contains("istransportadora") && e.IsTransportadora) ||
                    (papeis.Contains("iscomissionado") && e.IsComissionado) ||
                    (papeis.Contains("isagencia") && e.IsAgenciaBancaria) ||
                    (papeis.Contains("isfinanceira") && e.IsInstituicaoFinanceira) ||
                    (papeis.Contains("isfilial") && e.IsFilial) ||
                    (papeis.Contains("isobra") && e.IsObra) ||
                    (papeis.Contains("isrepresentante") && e.IsRepresentante) ||
                    (papeis.Contains("isoutro") && e.IsOutro) ||
                    (papeis.Contains("isprospecto") && e.IsProspecto) ||
                    (papeis.Contains("iscontador") && e.IsContador) ||
                    (papeis.Contains("isaluno") && e.IsAluno) ||
                    (papeis.Contains("isprofessor") && e.IsProfessor) ||
                    (papeis.Contains("isintermediador") && e.IsIntermediadorComercial)
                );
            }
        }

        bool desc = (direcaoOrdenacao ?? "asc").ToLower() == "desc";
        string sortCol = ordenarPor ?? "identidade";
        
        switch (sortCol.ToLower())
        {
            case "codigo":
            case "identidade":
                query = desc ? query.OrderByDescending(e => e.IdEntidade) : query.OrderBy(e => e.IdEntidade);
                break;
            case "razaosocial":
                query = desc 
                    ? query.OrderByDescending(e => e.PessoaJuridica != null ? e.PessoaJuridica.RazaoSocial : e.Nome) 
                    : query.OrderBy(e => e.PessoaJuridica != null ? e.PessoaJuridica.RazaoSocial : e.Nome);
                break;
            case "apelido":
            case "nome":
                query = desc ? query.OrderByDescending(e => e.Nome) : query.OrderBy(e => e.Nome);
                break;
            case "tipopessoa":
                query = desc ? query.OrderByDescending(e => e.TipoPessoa) : query.OrderBy(e => e.TipoPessoa);
                break;
            default:
                query = query.OrderByDescending(e => e.IdEntidade);
                break;
        }

        var total = await query.CountAsync(cancellationToken);
        
        // Carrega os dados na memória primeiro devido à limitação do SQL Server 2008 R2 (não suporta OFFSET/FETCH)
        var allItems = await query.ToListAsync(cancellationToken);
        
        var items = allItems
            .Skip((pagina - 1) * registrosPorPagina)
            .Take(registrosPorPagina)
            .ToList();

        return new PagedResult<Entidade>(items, total);
    }

    public async Task<Entidade?> GetCompletoPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ReadContext.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .Include(e => e.Enderecos)
            .FirstOrDefaultAsync(e => e.IdEntidade == id, cancellationToken);
    }
}

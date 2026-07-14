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

    public async Task<int?> GetPaisIdPorCidadeAsync(int idCidade, CancellationToken cancellationToken = default)
    {
        return await Context.Cidades
            .Where(c => c.IdCidade == idCidade)
            .Select(c => (int?)c.IdPais)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int?> GetPaisIdPorFilialAsync(int idFilial, CancellationToken cancellationToken = default)
    {
        return await (from ee in Context.EntidadeEnderecos
                      join c in Context.Cidades on ee.IdCidade equals c.IdCidade
                      where ee.IdEntidade == idFilial && ee.TipoEndereco == EnderecoTipo.ComercialResidencial
                      select (int?)c.IdPais)
                     .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Entidade>> ListarEntidadesAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await Context.Entidades
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
        CancellationToken cancellationToken = default)
    {
        var query = Context.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .AsQueryable();

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
            switch (papelFiltro.ToLower())
            {
                case "iscliente": query = query.Where(e => e.IsCliente); break;
                case "isfornecedor": query = query.Where(e => e.IsFornecedor); break;
                case "isfuncionario": query = query.Where(e => e.IsFuncionario); break;
                case "istransportadora": query = query.Where(e => e.IsTransportadora); break;
                case "iscomissionado": query = query.Where(e => e.IsComissionado); break;
                case "isagencia": query = query.Where(e => e.IsAgenciaBancaria); break;
                case "isfinanceira": query = query.Where(e => e.IsInstituicaoFinanceira); break;
                case "isfilial": query = query.Where(e => e.IsFilial); break;
                case "isobra": query = query.Where(e => e.IsObra); break;
                case "isrepresentante": query = query.Where(e => e.IsRepresentante); break;
                case "isoutro": query = query.Where(e => e.IsOutro); break;
                case "isprospecto": query = query.Where(e => e.IsProspecto); break;
                case "iscontador": query = query.Where(e => e.IsContador); break;
                case "isaluno": query = query.Where(e => e.IsAluno); break;
                case "isprofessor": query = query.Where(e => e.IsProfessor); break;
                case "isintermediador": query = query.Where(e => e.IsIntermediadorComercial); break;
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
        
        var items = await query
            .Skip((pagina - 1) * registrosPorPagina)
            .Take(registrosPorPagina)
            .ToListAsync(cancellationToken);

        return new PagedResult<Entidade>(items, total);
    }

    public async Task<Entidade?> GetCompletoPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Entidades
            .Include(e => e.PessoaFisica)
            .Include(e => e.PessoaJuridica)
            .Include(e => e.Enderecos)
            .FirstOrDefaultAsync(e => e.IdEntidade == id, cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: Dominio.cs (Retornar, GetDataUltimoFechamento, ValidarUsuarioDominioAtivo),
// DominioUsuario.cs (GetDominio, ValidarUsuario), DominioResponsavelLista.cs (VerificarResponsavelCadastrado),
// DominioPeriodo.cs (RetornarValorAbertura) e a view VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA.
// Tabelas: FINDOMINIO, FINDOMINIOUSUARIO, FINDOMINIORESPONSAVEL, FINDOMINIOPERIODO,
// FINDOMINIOPERIODOLANCTO, FINCAIXABANCO (analysis/E2-dominio.md §2).
public class DominioRepository(GestaoFinanceiraDbContext context, GestaoFinanceiraReadDbContext readContext)
    : GestaoFinanceiraRepositorioBase<Dominio>(context, readContext), IDominioRepository
{
    public Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default)
        => Context.Database.BeginTransactionAsync(cancellationToken);

    public async Task<IReadOnlyList<Dominio>> ListarAsync(int idFilial, string? texto, bool? ativo,
        CancellationToken cancellationToken = default)
    {
        var consulta = ReadDbSet.Where(d => d.IdFilial == idFilial);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var termo = texto.Trim();
            consulta = int.TryParse(termo, out var id)
                ? consulta.Where(d => d.IdDominio == id || d.Descricao.Contains(termo))
                : consulta.Where(d => d.Descricao.Contains(termo));
        }

        if (ativo is not null)
            consulta = consulta.Where(d => d.Ativo == ativo);

        return await consulta.OrderBy(d => d.IdDominio).ToListAsync(cancellationToken);
    }

    public Task<Dominio?> ObterAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
        => ReadDbSet
            .Include(d => d.Usuarios)
            .Include(d => d.Responsaveis)
            .FirstOrDefaultAsync(d => d.IdDominio == idDominio && d.IdFilial == idFilial, cancellationToken);

    public Task<Dominio?> ObterParaEdicaoAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
        => DbSet
            .Include(d => d.Usuarios)
            .Include(d => d.Responsaveis)
            .FirstOrDefaultAsync(d => d.IdDominio == idDominio && d.IdFilial == idFilial, cancellationToken);

    public async Task<Dominio?> ObterDominioDoUsuarioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default)
    {
        var idDominio = await ReadContext.DominioUsuarios
            .Where(u => u.IdFilial == idFilial && u.IdUsuario == idUsuario)
            .OrderBy(u => u.IdDominio)
            .Select(u => (int?)u.IdDominio)
            .FirstOrDefaultAsync(cancellationToken);

        return idDominio is null ? null : await ObterAsync(idDominio.Value, idFilial, cancellationToken);
    }

    public async Task<IReadOnlyList<DominioDoUsuario>> ListarDominiosDosUsuariosAsync(IReadOnlyCollection<int> idsUsuarios, int idFilial,
        CancellationToken cancellationToken = default)
    {
        if (idsUsuarios.Count == 0)
            return [];

        var vinculos = await ReadContext.DominioUsuarios
            .Where(u => u.IdFilial == idFilial && idsUsuarios.Contains(u.IdUsuario))
            .Select(u => new { u.IdUsuario, u.IdDominio })
            .ToListAsync(cancellationToken);

        // DominioUsuario.GetDominio: o domínio de menor IdDominio.
        var dominioPorUsuario = vinculos
            .GroupBy(v => v.IdUsuario)
            .ToDictionary(g => g.Key, g => g.Min(v => v.IdDominio));

        var idsDominios = dominioPorUsuario.Values.Distinct().ToList();

        var dominios = await (from d in ReadContext.Dominios
                              where d.IdFilial == idFilial && idsDominios.Contains(d.IdDominio)
                              join c in ReadContext.CaixasBanco
                                  on new { d.IdCaixaBanco, d.IdFilial } equals new { c.IdCaixaBanco, c.IdFilial } into caixas
                              from c in caixas.DefaultIfEmpty()
                              join p in ReadContext.DominioPeriodos
                                  on new { Id = d.IdDominioPeriodo, d.IdFilial } equals new { Id = (int?)p.IdDominioPeriodo, p.IdFilial } into periodos
                              from p in periodos.DefaultIfEmpty()
                              select new
                              {
                                  d.IdDominio,
                                  d.Ativo,
                                  d.IdCaixaBanco,
                                  DescricaoCaixa = c == null ? null : c.Descricao,
                                  d.IdDominioPeriodo,
                                  DataFechamento = p == null ? null : p.DataFechamento,
                              })
            .ToListAsync(cancellationToken);

        var porId = dominios.ToDictionary(d => d.IdDominio);

        return [.. dominioPorUsuario
            .Where(kv => porId.ContainsKey(kv.Value))
            .Select(kv =>
            {
                var d = porId[kv.Value];
                return new DominioDoUsuario(kv.Key, d.IdDominio, d.Ativo, d.IdCaixaBanco, d.DescricaoCaixa, d.IdDominioPeriodo, d.DataFechamento);
            })];
    }

    public async Task<IReadOnlyList<int>> ListarOutrosDominiosAtivosDoUsuarioAsync(int idUsuario, int idFilial, int idDominio,
        CancellationToken cancellationToken = default)
        => await (from u in ReadContext.DominioUsuarios
                  join d in ReadContext.Dominios on new { u.IdDominio, u.IdFilial } equals new { d.IdDominio, d.IdFilial }
                  where u.IdFilial == idFilial && u.IdUsuario == idUsuario && u.IdDominio != idDominio && d.Ativo
                  orderby u.IdDominio
                  select u.IdDominio)
            .ToListAsync(cancellationToken);

    public Task<bool> PossuiResponsavelAsync(int idDominio, int idDominioPai, int idFilial, CancellationToken cancellationToken = default)
        => ReadContext.DominioResponsaveis.AnyAsync(
            r => r.IdFilial == idFilial && r.IdDominio == idDominio && r.IdDominioPai == idDominioPai, cancellationToken);

    public async Task<IReadOnlyList<DominioResponsavel>> ListarFilhosAsync(int idDominioPai, int idFilial,
        CancellationToken cancellationToken = default)
        => await ReadContext.DominioResponsaveis
            .Where(r => r.IdFilial == idFilial && r.IdDominioPai == idDominioPai)
            .OrderBy(r => r.IdDominio)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Dominio>> ListarPorIdsAsync(IReadOnlyCollection<int> idsDominios, int idFilial,
        CancellationToken cancellationToken = default)
        => await ReadDbSet
            .Include(d => d.Usuarios)
            .Where(d => d.IdFilial == idFilial && idsDominios.Contains(d.IdDominio))
            .ToListAsync(cancellationToken);

    public Task<DominioPeriodo?> ObterPeriodoAsync(int idDominioPeriodo, int idFilial, CancellationToken cancellationToken = default)
        => ReadContext.DominioPeriodos
            .FirstOrDefaultAsync(p => p.IdDominioPeriodo == idDominioPeriodo && p.IdFilial == idFilial, cancellationToken);

    public async Task<IReadOnlyList<DominioPeriodo>> ListarPeriodosAsync(IReadOnlyCollection<int> idsPeriodos, int idFilial,
        CancellationToken cancellationToken = default)
        => await ReadContext.DominioPeriodos
            .Where(p => p.IdFilial == idFilial && idsPeriodos.Contains(p.IdDominioPeriodo))
            .ToListAsync(cancellationToken);

    public Task<bool> PossuiPeriodosAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
        => ReadContext.DominioPeriodos.AnyAsync(p => p.IdFilial == idFilial && p.IdDominio == idDominio, cancellationToken);

    public Task<DominioPeriodo?> ObterUltimoPeriodoFechadoAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
        => ReadContext.DominioPeriodos
            .Where(p => p.IdFilial == idFilial && p.IdDominio == idDominio && p.DataFechamento != null)
            .OrderByDescending(p => p.IdDominioPeriodo)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<DominioPeriodoLacto?> ObterLanctoAberturaAsync(int idDominioPeriodo, int idFilial, CancellationToken cancellationToken = default)
        => ReadContext.DominioPeriodoLactos
            .Where(l => l.IdFilial == idFilial && l.IdPeriodoDestino == idDominioPeriodo && l.TipoLancto == LanctoDominioPeriodoTipo.Abertura)
            .OrderBy(l => l.IdDominioPeriodoLacto)
            .FirstOrDefaultAsync(cancellationToken);

    // VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA (analysis/E2-dominio.md §2.8), bloco a bloco. O nome do
    // usuário (join com GLOUSUARIO, MOD-02) não é preenchido aqui.
    public async Task<IReadOnlyList<DominioUsuarioSuprimentoSangria>> ListarHierarquiaAsync(int idFilial,
        CancellationToken cancellationToken = default)
    {
        var dominios = await ReadContext.Dominios.Where(d => d.IdFilial == idFilial).ToListAsync(cancellationToken);
        var usuarios = await ReadContext.DominioUsuarios.Where(u => u.IdFilial == idFilial).ToListAsync(cancellationToken);
        var responsaveis = await ReadContext.DominioResponsaveis.Where(r => r.IdFilial == idFilial).ToListAsync(cancellationToken);

        var porId = dominios.ToDictionary(d => d.IdDominio);
        var usuariosPorDominio = usuarios.ToLookup(u => u.IdDominio);
        var linhas = new List<DominioUsuarioSuprimentoSangria>();

        // Bloco 1 — o próprio domínio e seus usuários.
        foreach (var d in dominios)
        {
            foreach (var u in usuariosPorDominio[d.IdDominio])
                linhas.Add(Linha(idFilial, d.IdDominio, false, null, d, u, d.Ativo, dominioPai: true, caixa: true));
        }

        foreach (var r in responsaveis)
        {
            if (!porId.TryGetValue(r.IdDominioPai, out var pai) || !porId.TryGetValue(r.IdDominio, out var filho))
                continue;

            // Bloco 2 — usuários do FILHO vistos pelo pai (ATIVO do pai).
            foreach (var u in usuariosPorDominio[filho.IdDominio])
                linhas.Add(Linha(idFilial, pai.IdDominio, r.DominioPaiPrincipal, filho.IdDominio, filho, u, pai.Ativo, dominioPai: false, caixa: true));

            // Bloco 3 — usuários do PAI vistos pelo filho (ATIVO do filho).
            foreach (var u in usuariosPorDominio[pai.IdDominio])
                linhas.Add(Linha(idFilial, pai.IdDominio, r.DominioPaiPrincipal, filho.IdDominio, pai, u, filho.Ativo, dominioPai: true, caixa: false));
        }

        return linhas;
    }

    private static DominioUsuarioSuprimentoSangria Linha(int idFilial, int idDominio, bool paiPrincipal, int? idFilho,
        Dominio retorno, DominioUsuario usuario, bool ativo, bool dominioPai, bool caixa) => new()
    {
        IdFilial = idFilial,
        IdDominio = idDominio,
        DominioPaiPrincipal = paiPrincipal,
        IdDominioFilho = idFilho,
        IdDominioRetorno = retorno.IdDominio,
        DescricaoRetorno = retorno.Descricao,
        IdUsuarioRetorno = usuario.IdUsuario,
        UsuarioPrincipalDominio = usuario.UsuarioPrincipal,
        Ativo = ativo,
        DominioPai = dominioPai,
        Caixa = caixa,
    };

    public void RemoverUsuario(DominioUsuario usuario) => Context.DominioUsuarios.Remove(usuario);

    public void RemoverResponsavel(DominioResponsavel responsavel) => Context.DominioResponsaveis.Remove(responsavel);
}

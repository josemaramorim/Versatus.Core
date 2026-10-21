using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: CaixaBanco.cs:ValidarPeriodoCaixa / GetListaDominioPeriodo / ValidarUsuarioCaixaDominio
// e DominioUsuario.cs:GetDominio (legado).
// Tabelas: FINDOMINIO, FINDOMINIOPERIODO, FINDOMINIOUSUARIO.
//
// E2-T04: o adapter SQL temporário do E3-T05 passa a consultar as entidades do E2 (DbSets do
// GestaoFinanceiraReadDbContext), mantendo a porta IDominioFinanceiroConsulta usada pelo E3.
public class DominioFinanceiroConsulta(GestaoFinanceiraReadDbContext readContext) : IDominioFinanceiroConsulta
{
    public async Task<IReadOnlyList<DominioPeriodoInfo>> ListarPeriodosDosDominiosDoCaixaAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
        => await (from d in readContext.Dominios
                  join p in readContext.DominioPeriodos
                      on new { Id = d.IdDominioPeriodo, d.IdFilial } equals new { Id = (int?)p.IdDominioPeriodo, p.IdFilial }
                  where d.IdCaixaBanco == idCaixaBanco && d.IdFilial == idFilial && d.Ativo && d.IdDominioPeriodo != null
                  select new DominioPeriodoInfo(p.IdDominio, p.DataFechamento))
            .ToListAsync(cancellationToken);

    public async Task<bool?> ObterMovimentoBancoDoDominioDoUsuarioAsync(int idUsuario, int idFilial,
        CancellationToken cancellationToken = default)
        => await (from u in readContext.DominioUsuarios
                  join d in readContext.Dominios on new { u.IdDominio, u.IdFilial } equals new { d.IdDominio, d.IdFilial }
                  where u.IdFilial == idFilial && u.IdUsuario == idUsuario
                  orderby u.IdDominio
                  select (bool?)d.MovimentoBanco)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> UsuarioPossuiDominioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default)
        => readContext.DominioUsuarios.AnyAsync(u => u.IdFilial == idFilial && u.IdUsuario == idUsuario, cancellationToken);

    public Task<bool> CaixaPossuiDominioAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => readContext.Dominios.AnyAsync(d => d.IdFilial == idFilial && d.IdCaixaBanco == idCaixaBanco, cancellationToken);
}

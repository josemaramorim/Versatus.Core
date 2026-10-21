using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: DominioPeriodo.cs (ExecutarPersistir, PersistirComplementoAbertura, PersistirLanctoPeriodo),
// DominioPeriodoLacto.cs / DominioPeriodoFormaPagto.cs (ExecutarPersistir/ExecutarExcluir) e os helpers
// de saldo SaldoDominioLactoHelper/SaldoDominioFormaHelper (UPDATE FINDOMINIO ... WHERE filial e domínio).
public class DominioPeriodoRepository(GestaoFinanceiraDbContext context, GestaoFinanceiraReadDbContext readContext)
    : GestaoFinanceiraRepositorioBase<DominioPeriodo>(context, readContext), IDominioPeriodoRepository
{
    public Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default)
        => Context.Database.BeginTransactionAsync(cancellationToken);

    public Task<Dominio?> ObterDominioAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
        => Context.Dominios
            .Include(d => d.Usuarios)
            .Include(d => d.Responsaveis)
            .FirstOrDefaultAsync(d => d.IdDominio == idDominio && d.IdFilial == idFilial, cancellationToken);

    public async Task<Dominio?> ObterDominioDoUsuarioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default)
    {
        var idDominio = await Context.DominioUsuarios
            .Where(u => u.IdFilial == idFilial && u.IdUsuario == idUsuario)
            .OrderBy(u => u.IdDominio)
            .Select(u => (int?)u.IdDominio)
            .FirstOrDefaultAsync(cancellationToken);

        return idDominio is null ? null : await ObterDominioAsync(idDominio.Value, idFilial, cancellationToken);
    }

    public Task<DominioPeriodo?> ObterPeriodoAsync(int idDominioPeriodo, int idFilial, CancellationToken cancellationToken = default)
        => DbSet
            .Include(p => p.Fechamentos)
            .FirstOrDefaultAsync(p => p.IdDominioPeriodo == idDominioPeriodo && p.IdFilial == idFilial, cancellationToken);

    public async Task<IReadOnlyList<Dominio>> ListarDominiosAsync(int idFilial, CancellationToken cancellationToken = default)
        => await Context.Dominios.Where(d => d.IdFilial == idFilial).OrderBy(d => d.IdDominio).ToListAsync(cancellationToken);

    public async Task AddDominioAsync(Dominio dominio, CancellationToken cancellationToken = default)
        => await Context.Dominios.AddAsync(dominio, cancellationToken);

    public async Task AdicionarLanctoAsync(DominioPeriodoLacto lancto, CancellationToken cancellationToken = default)
        => await Context.DominioPeriodoLactos.AddAsync(lancto, cancellationToken);

    public async Task AdicionarFormaPagtoAsync(DominioPeriodoFormaPagto forma, CancellationToken cancellationToken = default)
        => await Context.DominioPeriodoFormasPagto.AddAsync(forma, cancellationToken);

    public void RemoverLancto(DominioPeriodoLacto lancto) => Context.DominioPeriodoLactos.Remove(lancto);

    public void RemoverFormaPagto(DominioPeriodoFormaPagto forma) => Context.DominioPeriodoFormasPagto.Remove(forma);

    public async Task AplicarSaldoAsync(IReadOnlyList<AjusteSaldoDominio> ajustes, int idFilial, CancellationToken cancellationToken = default)
    {
        foreach (var ajuste in ajustes)
        {
            var dominio = await Context.Dominios.FirstOrDefaultAsync(d => d.IdDominio == ajuste.IdDominio && d.IdFilial == idFilial, cancellationToken);
            if (dominio is null)
                continue;

            dominio.Saldo = (dominio.Saldo ?? 0m) + ajuste.Valor;
            switch (ajuste.Coluna)
            {
                case SaldoDominioColuna.Dinheiro:
                    dominio.SaldoDinheiro = (dominio.SaldoDinheiro ?? 0m) + ajuste.Valor;
                    break;
                case SaldoDominioColuna.ChequeRecebido:
                    dominio.SaldoChequeRecebido = (dominio.SaldoChequeRecebido ?? 0m) + ajuste.Valor;
                    break;
                case SaldoDominioColuna.Cartao:
                    dominio.SaldoCartao = (dominio.SaldoCartao ?? 0m) + ajuste.Valor;
                    break;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: servidor/objeto de negócio/gestao.financeira/ContaBancaria.cs (legado — Retornar,
// UpdateDadosContaVinculada). Tabela: FINCONTABANCARIA.
public class ContaBancariaRepository(GestaoFinanceiraDbContext context, GestaoFinanceiraReadDbContext readContext)
    : GestaoFinanceiraRepositorioBase<ContaBancaria>(context, readContext), IContaBancariaRepository
{
    public Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default)
        => Context.Database.BeginTransactionAsync(cancellationToken);

    public Task<ContaBancaria?> ObterAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => ReadDbSet.FirstOrDefaultAsync(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial, cancellationToken);

    public Task<ContaBancaria?> ObterParaEdicaoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial, cancellationToken);

    // ContaBancaria.cs:292-297 — IdFilial = filial do ambiente, IdCaixaBanco <> conta,
    // IdContaBancariaVinculada = conta.
    public async Task<IReadOnlyList<ContaBancaria>> ListarContasQueVinculamParaEdicaoAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
        => await DbSet
            .Where(x => x.IdFilial == idFilial && x.IdCaixaBanco != idCaixaBanco && x.IdContaBancariaVinculada == idCaixaBanco)
            .ToListAsync(cancellationToken);

    public Task<CaixaBanco?> ObterCaixaBancoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => ReadContext.CaixasBanco
            .FirstOrDefaultAsync(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial, cancellationToken);
}

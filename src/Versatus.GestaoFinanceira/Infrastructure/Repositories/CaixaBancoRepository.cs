using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: servidor/objeto de negócio/gestao.financeira/CaixaBanco.cs (legado — Retornar,
// RetornarListaCaixaBancoUsuario, CarregarContaBancaria).
// Tabelas: FINCAIXABANCO, FINCAIXABANCOUSUARIO, FINCONTABANCARIA.
public class CaixaBancoRepository(GestaoFinanceiraDbContext context, GestaoFinanceiraReadDbContext readContext)
    : GestaoFinanceiraRepositorioBase<CaixaBanco>(context, readContext), ICaixaBancoRepository
{
    public Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default)
        => Context.Database.BeginTransactionAsync(cancellationToken);

    public async Task<IReadOnlyList<CaixaBanco>> ListarAsync(int idFilial, string? texto, bool? ativo, ContaTipo? tipoConta,
        bool? entraFluxoCaixa, CancellationToken cancellationToken = default)
    {
        var query = ReadDbSet.Where(x => x.IdFilial == idFilial);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var termo = texto.Trim();
            query = int.TryParse(termo, out var id)
                ? query.Where(x => x.IdCaixaBanco == id || x.Descricao.Contains(termo))
                : query.Where(x => x.Descricao.Contains(termo));
        }

        if (ativo.HasValue)
            query = query.Where(x => x.Ativo == ativo.Value);

        if (tipoConta.HasValue)
            query = query.Where(x => x.TipoConta == tipoConta.Value);

        if (entraFluxoCaixa.HasValue)
            query = query.Where(x => x.EntraFluxoCaixa == entraFluxoCaixa.Value);

        return await query.OrderBy(x => x.IdCaixaBanco).ToListAsync(cancellationToken);
    }

    public Task<CaixaBanco?> ObterAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => ReadDbSet.Include(x => x.Usuarios)
            .FirstOrDefaultAsync(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial, cancellationToken);

    public Task<CaixaBanco?> ObterParaEdicaoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => DbSet.Include(x => x.Usuarios)
            .FirstOrDefaultAsync(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial, cancellationToken);

    public async Task<IReadOnlyList<CaixaBancoUsuario>> ListarUsuariosAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
        => await ReadContext.CaixaBancoUsuarios
            .Where(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial)
            .OrderBy(x => x.IdUsuario)
            .ToListAsync(cancellationToken);

    public Task<ContaBancaria?> ObterContaBancariaParaEdicaoAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
        => Context.ContasBancarias
            .FirstOrDefaultAsync(x => x.IdCaixaBanco == idCaixaBanco && x.IdFilial == idFilial, cancellationToken);

    public async Task AdicionarContaBancariaAsync(ContaBancaria conta, CancellationToken cancellationToken = default)
        => await Context.ContasBancarias.AddAsync(conta, cancellationToken);

    public void RemoverContaBancaria(ContaBancaria conta) => Context.ContasBancarias.Remove(conta);

    public void RemoverUsuario(CaixaBancoUsuario usuario) => Context.CaixaBancoUsuarios.Remove(usuario);
}

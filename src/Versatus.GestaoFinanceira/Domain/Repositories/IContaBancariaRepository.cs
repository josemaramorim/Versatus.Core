using Microsoft.EntityFrameworkCore.Storage;
using Versatus.Framework.Repositories;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>Repositório da conta bancária (FINCONTABANCARIA — núcleo E3).</summary>
public interface IContaBancariaRepository : IRepositorio<ContaBancaria>
{
    Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default);

    Task<ContaBancaria?> ObterAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    Task<ContaBancaria?> ObterParaEdicaoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>
    /// Contas da filial que apontam para <paramref name="idCaixaBanco"/> como conta vinculada
    /// (exceto ela mesma), rastreadas para atualização — OP-E3-05 (UpdateDadosContaVinculada).
    /// </summary>
    Task<IReadOnlyList<ContaBancaria>> ListarContasQueVinculamParaEdicaoAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default);

    Task<CaixaBanco?> ObterCaixaBancoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);
}

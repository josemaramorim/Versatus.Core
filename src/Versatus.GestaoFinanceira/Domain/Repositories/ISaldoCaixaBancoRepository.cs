using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>Leitura de FINSALDOCAIXABANCO (ReadConnection) — OP-E3-08.</summary>
public interface ISaldoCaixaBancoRepository
{
    /// <summary>
    /// Último saldo (maior DataSaldo) do caixa na filial — SaldoCaixaBanco.RetornarCaixaBanco (legado).
    /// Com <paramref name="dataSaldo"/>: <c>DataSaldo &lt; data</c> quando <paramref name="estritamenteAnterior"/>,
    /// senão <c>DataSaldo &lt;= data</c>. Sem data: o último registro.
    /// </summary>
    Task<SaldoCaixaBanco?> ObterUltimoAsync(int idFilial, int idCaixaBanco, DateTime? dataSaldo, bool estritamenteAnterior,
        CancellationToken cancellationToken = default);
}

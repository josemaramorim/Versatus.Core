using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: SaldoCaixaBanco.cs:62-86 (RetornarCaixaBanco, legado) — OP-E3-08. Tabela: FINSALDOCAIXABANCO.
// TOP 1 com ORDER BY DATASALDO DESC (MaximoRegistros = 1 no legado) — sem OFFSET/FETCH (SQL Server 2008).
public class SaldoCaixaBancoRepository(GestaoFinanceiraReadDbContext readContext) : ISaldoCaixaBancoRepository
{
    public Task<SaldoCaixaBanco?> ObterUltimoAsync(int idFilial, int idCaixaBanco, DateTime? dataSaldo, bool estritamenteAnterior,
        CancellationToken cancellationToken = default)
    {
        var query = readContext.SaldosCaixaBanco.Where(x => x.IdFilial == idFilial && x.IdCaixaBanco == idCaixaBanco);

        if (dataSaldo is { } data)
            query = estritamenteAnterior ? query.Where(x => x.DataSaldo < data) : query.Where(x => x.DataSaldo <= data);

        return query.OrderByDescending(x => x.DataSaldo).FirstOrDefaultAsync(cancellationToken);
    }
}

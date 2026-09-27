using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: acesso.global/IndiceEconomico.cs (RetornarListaIndice) e IndiceConversor.cs (legado).
// Tabelas (MOD-02, só leitura): GLOINDICEECONOMICO, GLOINDICEECONOMICOVALOR — colunas conferidas via
// INFORMATION_SCHEMA (2026-09-27). SQL direto na ReadConnection: o MOD-02 ainda não mapeia índices.
public class IndiceEconomicoConsulta(GestaoFinanceiraReadDbContext readContext) : IIndiceEconomicoConsulta
{
    public async Task<IndiceEconomicoInfo?> ObterAsync(int idIndiceEconomico, CancellationToken cancellationToken = default)
    {
        var linhas = await readContext.Database.SqlQuery<IndiceEconomicoInfo>($"""
            SELECT I.IDGLOINDICEECONOMICO AS IdIndiceEconomico, I.SIGLA AS Sigla,
                   I.IDTIPOCORRECAO AS IdTipoCorrecao, I.IDMODOCORRECAO AS IdModoCorrecao
            FROM GLOINDICEECONOMICO I
            WHERE I.IDGLOINDICEECONOMICO = {idIndiceEconomico}
            """).ToListAsync(cancellationToken);

        return linhas.FirstOrDefault();
    }

    // Ordem por DATAINDICE (PK IDGLOINDICEECONOMICO, DATAINDICE — a ordem em que o legado recebia a
    // lista, sem ORDER BY explícito). VALOR nulo = 0, como o double do objeto legado.
    public async Task<IReadOnlyList<decimal>> ListarValoresAsync(int idIndiceEconomico, DateTime inicio, DateTime fim,
        CancellationToken cancellationToken = default)
        => await readContext.Database.SqlQuery<decimal>($"""
            SELECT ISNULL(V.VALOR, 0) AS Value
            FROM GLOINDICEECONOMICOVALOR V
            WHERE V.IDGLOINDICEECONOMICO = {idIndiceEconomico}
              AND V.DATAINDICE >= {inicio} AND V.DATAINDICE <= {fim}
            ORDER BY V.DATAINDICE
            """).ToListAsync(cancellationToken);
}

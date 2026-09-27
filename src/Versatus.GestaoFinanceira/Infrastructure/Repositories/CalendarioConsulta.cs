using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: acesso.global/Feriado.cs:393-421 (EFeriado, legado) — feriado fixo pela chave DIAMES ("dd/MM")
// ou feriado por data (GLOFERIADODATA). Como no legado, a consulta é só pela chave (sem filtrar ATIVO).
// Tabelas (MOD-02, só leitura): GLOFERIADO, GLOFERIADODATA — conferidas via INFORMATION_SCHEMA (2026-09-27).
public class CalendarioConsulta(GestaoFinanceiraReadDbContext readContext) : ICalendarioConsulta
{
    public async Task<bool> EhFeriadoAsync(DateTime data, CancellationToken cancellationToken = default)
    {
        var diaMes = data.ToString("dd'/'MM");
        var dia = data.Date;

        var total = await readContext.Database.SqlQuery<int>($"""
            SELECT (SELECT COUNT(*) FROM GLOFERIADO WHERE DIAMES = {diaMes})
                 + (SELECT COUNT(*) FROM GLOFERIADODATA WHERE DATAFERIADO = {dia}) AS Value
            """).ToListAsync(cancellationToken);

        return total[0] > 0;
    }
}

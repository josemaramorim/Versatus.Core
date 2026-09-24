using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: servidor/objeto de negócio/acesso.global/InstituicaoFinanceira.cs (Entidade = entidade
// de mesmo Id) + CaixaBanco.cs:231-258 (legado).
// Tabelas (MOD-02, só leitura): GLOINSTITUICAOFINANCEIRA, GLOENTIDADE, GLOENTIDADEJURIDICA —
// colunas conferidas via INFORMATION_SCHEMA (2026-09-24). SQL direto na ReadConnection porque
// o MOD-02 ainda não mapeia GLOINSTITUICAOFINANCEIRA; referência cross-módulo só por int
// (Artigo VIII.1).
public class InstituicaoFinanceiraConsulta(GestaoFinanceiraReadDbContext readContext) : IInstituicaoFinanceiraConsulta
{
    public async Task<InstituicaoFinanceiraInfo?> ObterAsync(int idInstituicaoFinanceira, CancellationToken cancellationToken = default)
    {
        var linhas = await readContext.Database.SqlQuery<InstituicaoFinanceiraInfo>($"""
            SELECT E.IDFISICAJURIDICA AS IdFisicaJuridica, J.CNPJ AS Cnpj
            FROM GLOINSTITUICAOFINANCEIRA I
            INNER JOIN GLOENTIDADE E ON E.IDGLOENTIDADE = I.IDGLOINSTITUICAOFINANCEIRA
            LEFT JOIN GLOENTIDADEJURIDICA J ON J.IDGLOENTIDADE = E.IDGLOENTIDADE
            WHERE I.IDGLOINSTITUICAOFINANCEIRA = {idInstituicaoFinanceira}
            """).ToListAsync(cancellationToken);

        return linhas.FirstOrDefault();
    }
}

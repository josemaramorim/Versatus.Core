using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: CaixaBanco.cs:ValidarPeriodoCaixa / GetListaDominioPeriodo / ValidarUsuarioCaixaDominio
// e DominioUsuario.cs:GetDominio (legado).
// Tabelas: FINDOMINIO, FINDOMINIOPERIODO, FINDOMINIOUSUARIO — colunas conferidas via
// INFORMATION_SCHEMA (2026-09-24).
//
// ADAPTER TEMPORÁRIO (decisão do usuário em 2026-09-24, E3-T05): SQL só-leitura na
// ReadConnection até o épico E2 mapear domínio/período; lá ele é substituído pelos
// repositórios do E2, mantendo a porta IDominioFinanceiroConsulta.
public class DominioFinanceiroConsulta(GestaoFinanceiraReadDbContext readContext) : IDominioFinanceiroConsulta
{
    public async Task<IReadOnlyList<DominioPeriodoInfo>> ListarPeriodosDosDominiosDoCaixaAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
        => await readContext.Database.SqlQuery<DominioPeriodoInfo>($"""
            SELECT P.IDFINDOMINIO AS IdDominio, P.DATAFECHAMENTO AS DataFechamento
            FROM FINDOMINIO D
            INNER JOIN FINDOMINIOPERIODO P
                ON P.IDFINDOMINIOPERIODO = D.IDFINDOMINIOPERIODO AND P.IDGLOFILIAL = {idFilial}
            WHERE D.IDFINCAIXABANCO = {idCaixaBanco} AND D.IDGLOFILIAL = {idFilial}
              AND D.ATIVO = 1 AND D.IDFINDOMINIOPERIODO IS NOT NULL
            """).ToListAsync(cancellationToken);

    public async Task<bool?> ObterMovimentoBancoDoDominioDoUsuarioAsync(int idUsuario, int idFilial,
        CancellationToken cancellationToken = default)
    {
        var linhas = await readContext.Database.SqlQuery<short>($"""
            SELECT TOP 1 D.MOVIMENTOBANCO AS Value
            FROM FINDOMINIOUSUARIO U
            INNER JOIN FINDOMINIO D ON D.IDFINDOMINIO = U.IDFINDOMINIO AND D.IDGLOFILIAL = U.IDGLOFILIAL
            WHERE U.IDGLOFILIAL = {idFilial} AND U.IDGLOUSUARIO = {idUsuario}
            ORDER BY U.IDFINDOMINIO
            """).ToListAsync(cancellationToken);

        return linhas.Count == 0 ? null : linhas[0] != 0;
    }

    public async Task<bool> UsuarioPossuiDominioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default)
    {
        var total = await readContext.Database.SqlQuery<int>($"""
            SELECT COUNT(*) AS Value FROM FINDOMINIOUSUARIO
            WHERE IDGLOFILIAL = {idFilial} AND IDGLOUSUARIO = {idUsuario}
            """).ToListAsync(cancellationToken);

        return total[0] > 0;
    }

    public async Task<bool> CaixaPossuiDominioAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
    {
        var total = await readContext.Database.SqlQuery<int>($"""
            SELECT COUNT(*) AS Value FROM FINDOMINIO
            WHERE IDGLOFILIAL = {idFilial} AND IDFINCAIXABANCO = {idCaixaBanco}
            """).ToListAsync(cancellationToken);

        return total[0] > 0;
    }
}

using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: lookups do FCaixaBanco.cs (lookupAgencia, gridFormEditorUsuario,
// lookupEditInstituicaoFinanceiraSPED, lookupEditPlanoContabil, lookupEditContaVinculada).
// Tabelas (só leitura): GLOAGENCIA, GLOBANCO, GLOUSUARIO, GLOINSTITUICAOFINANCEIRA, GLOENTIDADE,
// CONPLANOCONTABIL, FINCAIXABANCO, FINCONTABANCARIA — colunas conferidas via INFORMATION_SCHEMA
// (2026-09-27). SQL direto na ReadConnection: MOD-02/contábil ainda não expõem essas consultas
// (DÚVIDA-CB5). Referência cross-módulo só por int (Artigo VIII.1).
public class LookupFinanceiroConsulta(GestaoFinanceiraReadDbContext readContext) : ILookupFinanceiroConsulta
{
    // GLOENTIDADE.IDFISICAJURIDICA — 3 = Jurídica (DEC-007).
    private const int IdPessoaJuridica = 3;

    // Projeto.Geral.Enumerado.SinteticoAnalitico.Analitico (tipoenumerado.cs:225).
    private const int IdAnalitico = 34;

    // Projeto.Geral.Enumerado.TipoContaBancaria.ContaCorrente.
    private const int IdContaCorrente = 1479;

    public Task<IReadOnlyList<ItemLookupDto>> ListarAgenciasAsync(string? texto, CancellationToken cancellationToken = default)
    {
        var termo = Termo(texto);
        return ListarAsync(readContext.Database.SqlQuery<ItemLookupDto>($"""
            SELECT TOP ({ILookupFinanceiroConsulta.LimiteItens}) A.IDGLOAGENCIA AS Id,
                   A.NUMERO + ' - ' + A.DESCRICAO + ' (' + B.DESCRICAO + ')' AS Descricao
            FROM GLOAGENCIA A
            INNER JOIN GLOBANCO B ON B.IDGLOBANCO = A.IDGLOBANCO
            WHERE {termo} = '' OR A.NUMERO LIKE '%' + {termo} + '%' OR A.DESCRICAO LIKE '%' + {termo} + '%'
               OR B.DESCRICAO LIKE '%' + {termo} + '%' OR CAST(A.IDGLOAGENCIA AS varchar(12)) = {termo}
            ORDER BY B.DESCRICAO, A.NUMERO
            """), cancellationToken);
    }

    public Task<IReadOnlyList<ItemLookupDto>> ListarUsuariosAsync(string? texto, CancellationToken cancellationToken = default)
    {
        var termo = Termo(texto);
        return ListarAsync(readContext.Database.SqlQuery<ItemLookupDto>($"""
            SELECT TOP ({ILookupFinanceiroConsulta.LimiteItens}) U.IDGLOUSUARIO AS Id, U.NOME AS Descricao
            FROM GLOUSUARIO U
            WHERE {termo} = '' OR U.NOME LIKE '%' + {termo} + '%' OR CAST(U.IDGLOUSUARIO AS varchar(12)) = {termo}
            ORDER BY U.NOME
            """), cancellationToken);
    }

    public Task<IReadOnlyList<ItemLookupDto>> ListarInstituicoesFinanceirasAsync(string? texto, CancellationToken cancellationToken = default)
    {
        var termo = Termo(texto);
        return ListarAsync(readContext.Database.SqlQuery<ItemLookupDto>($"""
            SELECT TOP ({ILookupFinanceiroConsulta.LimiteItens}) I.IDGLOINSTITUICAOFINANCEIRA AS Id,
                   I.NOMERESUMIDO + ' - ' + E.NOME AS Descricao
            FROM GLOINSTITUICAOFINANCEIRA I
            INNER JOIN GLOENTIDADE E ON E.IDGLOENTIDADE = I.IDGLOINSTITUICAOFINANCEIRA
            WHERE E.IDFISICAJURIDICA = {IdPessoaJuridica}
              AND ({termo} = '' OR I.NOMERESUMIDO LIKE '%' + {termo} + '%' OR E.NOME LIKE '%' + {termo} + '%'
                   OR CAST(I.IDGLOINSTITUICAOFINANCEIRA AS varchar(12)) = {termo})
            ORDER BY I.NOMERESUMIDO
            """), cancellationToken);
    }

    public Task<IReadOnlyList<ItemLookupDto>> ListarPlanosContabeisAsync(int idFilial, string? texto, CancellationToken cancellationToken = default)
    {
        var termo = Termo(texto);
        return ListarAsync(readContext.Database.SqlQuery<ItemLookupDto>($"""
            SELECT TOP ({ILookupFinanceiroConsulta.LimiteItens}) P.IDCONPLANOCONTABIL AS Id, ISNULL(P.DESCRICAO, '') AS Descricao
            FROM CONPLANOCONTABIL P
            WHERE P.IDGLOFILIAL = {idFilial} AND P.IDTIPO = {IdAnalitico}
              AND ({termo} = '' OR P.DESCRICAO LIKE '%' + {termo} + '%' OR CAST(P.IDCONPLANOCONTABIL AS varchar(12)) = {termo})
            ORDER BY P.DESCRICAO
            """), cancellationToken);
    }

    public Task<IReadOnlyList<ItemLookupDto>> ListarContasCorrentesAsync(int idFilial, string? texto, CancellationToken cancellationToken = default)
    {
        var termo = Termo(texto);
        return ListarAsync(readContext.Database.SqlQuery<ItemLookupDto>($"""
            SELECT TOP ({ILookupFinanceiroConsulta.LimiteItens}) C.IDFINCAIXABANCO AS Id,
                   CAST(C.IDFINCAIXABANCO AS varchar(12)) + ' - ' + C.DESCRICAO AS Descricao
            FROM FINCAIXABANCO C
            INNER JOIN FINCONTABANCARIA B ON B.IDFINCAIXABANCO = C.IDFINCAIXABANCO AND B.IDGLOFILIAL = C.IDGLOFILIAL
            WHERE C.IDGLOFILIAL = {idFilial} AND B.IDTIPOCONTABANCARIA = {IdContaCorrente}
              AND ({termo} = '' OR C.DESCRICAO LIKE '%' + {termo} + '%' OR CAST(C.IDFINCAIXABANCO AS varchar(12)) = {termo})
            ORDER BY C.IDFINCAIXABANCO
            """), cancellationToken);
    }

    private static string Termo(string? texto) => texto?.Trim() ?? string.Empty;

    private static async Task<IReadOnlyList<ItemLookupDto>> ListarAsync(IQueryable<ItemLookupDto> consulta, CancellationToken cancellationToken)
        => await consulta.ToListAsync(cancellationToken);
}

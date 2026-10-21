using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Origem: acesso.global/Perfil.cs:EmpresaLiberada (513-532) e IsPerfilSuporte (188);
// gestao.financeira/DominioUsuario.cs:UsuarioUsaDominioFinanceiro / GetValidacaoPerfilUsuario;
// gestao.financeira/PeriodosAbertos.cs:RetornarPeriodos / AddPeriodos.
// Tabelas MOD-02: GLOPERFIL (IDGLOPERFIL, DESCRICAO, USADOMINIOFINANCEIRO), GLOPERFILUSUARIO
// (IDGLOPERFIL, IDGLOUSUARIO), GLOPERFILEMPRESA (IDGLOPERFIL, IDGLOEMPRESA), GLOEMPRESA (ATIVO),
// GLOUSUARIO (NOME) — colunas conferidas via INFORMATION_SCHEMA (2026-10-21).
//
// EmpresaLiberada(idEmpresa, idUsuario): perfil de suporte ("SUPORTEVERSATUS") ou usuário master
// (Funcoes.UsuarioMaster = 1) → liberado; senão precisa de GLOPERFILEMPRESA para a empresa, com a
// empresa ativa.
public class PerfilDominioConsulta(GestaoFinanceiraReadDbContext readContext) : IPerfilDominioConsulta
{
    public const string PerfilSuporte = "SUPORTEVERSATUS";
    public const int UsuarioMaster = 1;

    public async Task<PerfilUsuarioDominioInfo?> ObterPerfilUsuarioAsync(int idUsuario, int idEmpresa,
        CancellationToken cancellationToken = default)
    {
        var linhas = await readContext.Database.SqlQuery<PerfilUsuarioLinha>($"""
            SELECT U.IDGLOUSUARIO AS IdUsuario, U.NOME AS Nome,
                   (SELECT COUNT(*) FROM GLOPERFILUSUARIO PU WHERE PU.IDGLOUSUARIO = U.IDGLOUSUARIO) AS TotalPerfis,
                   (SELECT COUNT(*) FROM GLOPERFILUSUARIO PU
                    INNER JOIN GLOPERFIL P ON P.IDGLOPERFIL = PU.IDGLOPERFIL
                    WHERE PU.IDGLOUSUARIO = U.IDGLOUSUARIO AND P.USADOMINIOFINANCEIRO = 1
                      AND (P.DESCRICAO = {PerfilSuporte} OR U.IDGLOUSUARIO = {UsuarioMaster}
                           OR EXISTS (SELECT 1 FROM GLOPERFILEMPRESA PE
                                      INNER JOIN GLOEMPRESA E ON E.IDGLOEMPRESA = PE.IDGLOEMPRESA
                                      WHERE PE.IDGLOPERFIL = P.IDGLOPERFIL AND PE.IDGLOEMPRESA = {idEmpresa} AND E.ATIVO = 1))
                   ) AS TotalPerfisDominio
            FROM GLOUSUARIO U
            WHERE U.IDGLOUSUARIO = {idUsuario}
            """).ToListAsync(cancellationToken);

        if (linhas.Count == 0)
            return null;

        var l = linhas[0];
        return new PerfilUsuarioDominioInfo(l.IdUsuario, l.Nome ?? string.Empty, l.TotalPerfis > 0, l.TotalPerfisDominio > 0);
    }

    public async Task<IReadOnlyList<UsuarioPerfilDominioInfo>> ListarUsuariosDosPerfisComDominioAsync(int idEmpresa, int idUsuarioLogado,
        CancellationToken cancellationToken = default)
    {
        // A liberação da empresa é verificada para o usuário LOGADO (pf.EmpresaLiberada(empresa, ambiente.IdUsuario)).
        var linhas = await readContext.Database.SqlQuery<UsuarioPerfilLinha>($"""
            SELECT DISTINCT U.IDGLOUSUARIO AS IdUsuario, U.NOME AS Nome
            FROM GLOPERFIL P
            INNER JOIN GLOPERFILUSUARIO PU ON PU.IDGLOPERFIL = P.IDGLOPERFIL
            INNER JOIN GLOUSUARIO U ON U.IDGLOUSUARIO = PU.IDGLOUSUARIO
            WHERE P.USADOMINIOFINANCEIRO = 1
              AND (P.DESCRICAO = {PerfilSuporte} OR {idUsuarioLogado} = {UsuarioMaster}
                   OR EXISTS (SELECT 1 FROM GLOPERFILEMPRESA PE
                              INNER JOIN GLOEMPRESA E ON E.IDGLOEMPRESA = PE.IDGLOEMPRESA
                              WHERE PE.IDGLOPERFIL = P.IDGLOPERFIL AND PE.IDGLOEMPRESA = {idEmpresa} AND E.ATIVO = 1))
            """).ToListAsync(cancellationToken);

        return [.. linhas.Select(l => new UsuarioPerfilDominioInfo(l.IdUsuario, l.Nome ?? string.Empty))];
    }

    public async Task<IReadOnlyList<int>> ListarPerfisDoUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default)
        => await readContext.Database.SqlQuery<int>($"""
            SELECT IDGLOPERFIL AS Value FROM GLOPERFILUSUARIO WHERE IDGLOUSUARIO = {idUsuario} ORDER BY ORDEM, IDGLOPERFIL
            """).ToListAsync(cancellationToken);

    private sealed record PerfilUsuarioLinha(int IdUsuario, string? Nome, int TotalPerfis, int TotalPerfisDominio);

    private sealed record UsuarioPerfilLinha(int IdUsuario, string? Nome);
}

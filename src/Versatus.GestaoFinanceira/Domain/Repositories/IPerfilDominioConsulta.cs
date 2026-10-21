namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Porta de leitura do MOD-02 (GLOPERFIL / GLOPERFILUSUARIO / GLOPERFILEMPRESA / GLOEMPRESA /
/// GLOUSUARIO) usada pelo E2: perfil do usuário com domínio financeiro (VAL-E2-31/32) e os
/// usuários dos perfis com domínio financeiro (PeriodosAbertos — OP-E2-13).
/// Adapter SQL só-leitura (Artigo VIII — cross-módulo por int), como os adapters do E3.
/// </summary>
public interface IPerfilDominioConsulta
{
    /// <summary>
    /// Perfis do usuário — DominioUsuario.GetValidacaoPerfilUsuario / UsuarioUsaDominioFinanceiro
    /// (DominioUsuario.cs:164-179, 478-503). <c>null</c> quando o usuário não existe.
    /// </summary>
    Task<PerfilUsuarioDominioInfo?> ObterPerfilUsuarioAsync(int idUsuario, int idEmpresa, CancellationToken cancellationToken = default);

    /// <summary>
    /// Usuários dos perfis com UsaDominioFinanceiro liberados para a empresa ao usuário logado —
    /// PeriodosAbertos.RetornarPeriodos/AddPeriodos (PeriodosAbertos.cs:223-246, 260-296).
    /// </summary>
    Task<IReadOnlyList<UsuarioPerfilDominioInfo>> ListarUsuariosDosPerfisComDominioAsync(int idEmpresa, int idUsuarioLogado,
        CancellationToken cancellationToken = default);

    /// <summary>Perfis do usuário (GLOPERFILUSUARIO) — FDominioPadraoAbertura.CarregarPerfilDominio.</summary>
    Task<IReadOnlyList<int>> ListarPerfisDoUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default);
}

/// <param name="PossuiPerfil">O usuário tem ao menos um perfil (GLOPERFILUSUARIO).</param>
/// <param name="UsaDominioFinanceiro">Algum perfil liberado para a empresa tem USADOMINIOFINANCEIRO.</param>
public sealed record PerfilUsuarioDominioInfo(int IdUsuario, string Nome, bool PossuiPerfil, bool UsaDominioFinanceiro);

public sealed record UsuarioPerfilDominioInfo(int IdUsuario, string Nome);

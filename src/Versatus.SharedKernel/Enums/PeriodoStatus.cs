namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Retorno da checagem de período aberto (não é a coluna de situação do período em si).
/// Épico E2 — usado por 14 classes do legado.
/// Origem: Projeto.Geral.EnumeradoObjeto.PeriodoStatus (legado) — não-persistido
/// (comportamento em memória, nunca vira coluna).
/// </summary>
public enum PeriodoStatus
{
    Aberto = 1,
    Fechado = 2,
    NaoAplicavel = 3,
    UsuarioSemPermissao = 4,
    PerfilSemPermissao = 5
}

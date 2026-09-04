namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Situação da distribuição de acerto de adiantamento.
/// Origem: Projeto.Geral.Enumerado.SituacaoDistribuicao (legado, [TipoEnumerado(386)]) —
/// persistido em FINADTOACERTODISTRIBUICAO (coluna a confirmar no analysis de E10). Épico E10.
/// </summary>
public enum SituacaoDistribuicao
{
    Pendente = 387,
    Atendido = 388,
    AtendidoParcial = 389,
    Cancelado = 436,
    Devolvido = 497
}

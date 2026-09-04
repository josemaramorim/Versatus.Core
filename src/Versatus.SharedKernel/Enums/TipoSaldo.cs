namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Momento de um saldo de caixa/banco (inicial, lançamento, atual, final) — golden tests
/// de cálculo em E5.
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoSaldo (legado, sem valor explícito — 0,1,2,3
/// por posição) — não-persistido (comportamento em memória, nunca vira coluna).
/// </summary>
public enum TipoSaldo
{
    Inicial = 0,
    Lancamento = 1,
    Atual = 2,
    Final = 3
}

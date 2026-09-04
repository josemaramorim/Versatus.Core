namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Estado transitório do processamento de domínio/período (abertura, fechamento,
/// suprimento...). Épico E2.
/// Origem: Projeto.Geral.EnumeradoObjeto.StatusDominioFinanceiro (legado) — não-persistido
/// (comportamento em memória, nunca vira coluna).
/// </summary>
public enum StatusDominioFinanceiro
{
    Normal = 1,
    Abertura = 2,
    AberturaPadrao = 3,
    Fechamento = 4,
    FechamentoPadrao = 5,
    SuprimentoDinheiro = 6,
    SuprimentoCheque = 7,
    AtualizarDominio = 8,
    AtualizarDominioPadrao = 9
}

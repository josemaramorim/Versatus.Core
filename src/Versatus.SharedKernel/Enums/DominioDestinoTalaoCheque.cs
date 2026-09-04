namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Destino de um talão de cheque (próprio da conta ou o talão padrão). Épico E9.
/// Origem: Projeto.Geral.EnumeradoObjeto.DominioDestinoTalaoCheque (legado) —
/// não-persistido (comportamento em memória, nunca vira coluna).
/// </summary>
public enum DominioDestinoTalaoCheque
{
    Proprio = 1,
    Padrao = 2
}

namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Dimensão de rateio (classe, centro de custo, projeto). Combinável — [Flags] no legado.
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoRateioItem (legado, [Flags]) — não-persistido
/// (comportamento em memória, nunca vira coluna).
/// </summary>
[Flags]
public enum TipoRateioItem
{
    Nenhum = 0,
    Classe = 1,
    CentroCusto = 2,
    Projeto = 4
}

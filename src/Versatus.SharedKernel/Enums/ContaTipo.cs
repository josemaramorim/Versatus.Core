namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Se a conta de caixa/banco é um caixa ou uma conta bancária.
/// Origem: Projeto.Geral.Enumerado.ContaTipo (legado, [TipoEnumerado(174)]) — persistido em
/// FINCAIXABANCO.IDTIPOCONTA. Nome real do legado — data-model.md cita "TipoConta", mas o
/// tipo é ContaTipo (ver enums.md §2 Tabela A).
/// </summary>
public enum ContaTipo
{
    Caixa = 175,
    Banco = 176
}

namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Disponibilidade de uma condição/forma de pagamento para pagamento, recebimento ou ambos.
/// Origem: Projeto.Geral.Enumerado.Disponibilidade (legado, [TipoEnumerado(55)]) — persistido.
/// Movido de Versatus.AcessoGlobal para cá por DEC-007 (usado por 2+ módulos: AcessoGlobal e
/// GestaoFinanceira). Valores inteiros preservados do legado — não renumerar.
/// </summary>
public enum Disponibilidade
{
    Pagamento = 56,
    Recebimento = 57,
    Ambas = 101
}

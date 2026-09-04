namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Operação aritmética de um item financeiro (juros/multa/desconto) sobre o valor base.
/// Base dos cálculos financeiros do módulo — golden tests de paridade em E4/E6.
/// Origem: Projeto.Geral.Enumerado.CalculoItemFinanceiro (legado, [TipoEnumerado(238)]) —
/// persistido em FINITEMFINANCEIRO.IDCALCULO.
/// </summary>
public enum CalculoItemFinanceiro
{
    Somar = 239,
    Subtrair = 240,
    MultiplicarSomar = 241,
    MultiplicarDiminuir = 242,
    DividirSomar = 243,
    DividirSubtrair = 257,
    PercentualSomar = 244,
    PercentualSubtrair = 245
}

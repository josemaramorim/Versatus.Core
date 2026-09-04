namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Quando um item financeiro (juros/multa/...) se aplica em relação ao vencimento.
/// Origem: Projeto.Geral.Enumerado.ItemFinanceiroAplicar (legado, [TipoEnumerado(246)]) —
/// persistido em FINITEMFINANCEIRO.IDAPLICAR.
/// </summary>
public enum ItemFinanceiroAplicar
{
    NaoAplicar = 247,
    DepoisVencimento = 248,
    AntesVencimento = 249
}

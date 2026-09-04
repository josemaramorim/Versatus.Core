namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Se uma forma de movimento de liquidação/estorno é a forma de pagamento em si ou o troco.
/// Origem: Projeto.Geral.Enumerado.TipoFormaLancamento (legado, [TipoEnumerado(258)]) —
/// persistido em FINLIQUIDACAOFORMAMOVIMENTO, FINLIQESTORNOFORMAPAGTO.
/// </summary>
public enum TipoFormaLancamento
{
    FormaPagamento = 259,
    Troco = 260
}

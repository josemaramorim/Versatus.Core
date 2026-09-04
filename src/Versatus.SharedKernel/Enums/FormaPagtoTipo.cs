namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Tipo/modalidade de forma de pagamento (dinheiro, cheque, cartão, PIX...).
/// Origem: Projeto.Geral.Enumerado.FormaPagtoTipo (legado, [TipoEnumerado(121)]) — persistido.
/// Movido de Versatus.AcessoGlobal para cá por DEC-007 (usado por 2+ módulos: AcessoGlobal e
/// GestaoFinanceira). Valores inteiros preservados do legado — não renumerar.
/// </summary>
public enum FormaPagtoTipo
{
    Dinheiro = 122,
    ChequeEmpresa = 123,
    ChequeCliente = 124,
    CartaoCredito = 125,
    CartaoDebito = 126,
    ParcelamentoProprio = 127,
    ParcelamentoFinanceira = 128,
    Credito = 236,
    CreditoPortador = 237,
    Deposito = 256,
    Outros = 293,
    Abatimento = 483,
    PixEstatico = 1962,
    PixDinamico = 1963
}

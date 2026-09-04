namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Chave de roteamento de operação (Caixa, Banco, Pagar, Receber, LiquidacaoReceber...).
/// Origem: Projeto.Geral.Enumerado.OperacaoTipo (legado) — SEM [TipoEnumerado] no
/// código-fonte, sem entrada em GloTipoEnumerado. Classificação de persistência PENDENTE
/// (ver enums.md §5, item 1): provável não-persistido — nenhuma coluna de FINMOVIMENTO/
/// FINDOCUMENTO grava este valor diretamente; a confirmar em definitivo no analysis de
/// E1/E6, quando então mover este comentário caso se confirme não-persistido.
/// </summary>
public enum OperacaoTipo
{
    Caixa = 1,
    Banco = 2,
    Pagar = 3,
    Receber = 4,
    LiquidacaoReceber = 5,
    LiquidacaoPagar = 6,
    ChequeRecebido = 7,
    MovimentacaoEstoque = 8,
    Venda = 9,
    Compra = 10,
    Folha = 11,
    EntradaVenda = 12,
    MovimentoCartao = 13,
    LiquidacaoMovimentoCartao = 14,
    SaidaCompra = 15,
    MovimentoProducao = 16,
    Transporte = 17,
    Contrato = 18,
    NotaFiscalServico = 19
}

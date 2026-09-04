namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Roteamento de comissão por origem do processo (venda, documento, liquidação, reversão...).
/// Usado em Documento/Liquidacao/Reversao.
/// Origem: Projeto.Geral.EnumeradoObjeto.TipoProcessoComissao (legado) — não-persistido
/// (comportamento em memória, nunca vira coluna).
/// </summary>
public enum TipoProcessoComissao
{
    Venda = 1,
    Documento = 2,
    Liquidacao = 3,
    Reversao = 4,
    CancelamentoVenda = 5,
    DevolucaoVenda = 6,
    NotaServicoFiscal = 7
}

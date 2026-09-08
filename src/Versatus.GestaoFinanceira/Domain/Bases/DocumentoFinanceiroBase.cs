using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/DocumentoFinanceiroBase.cs (legado)
// Base de: FinDocumento (Documento — E4) e demais documentos financeiros.
//
// POCO abstrata SÓ DE DADOS (CLR-04 / spec §3.3 / Artigo III): sem herança de framework,
// sem atributos, sem Lookup<>. Os campos de comportamento do legado
// (ValidarOperacao/ValidarEntidade/AplicarOperacao/AplicarCondicaoPagamento/
// CalcularValorConvertido — ver matriz-rot.md#E1 OP-E1-07..09 / matriz-rtv.md#E1
// VAL-E1-01..07) vivem em Application/Bases/PersistenciaDocumentoBase (E1-T03).
// Referências a entidades de outros módulos são int lógico (Artigo VIII).
public abstract class DocumentoFinanceiroBase
{
    /// <summary>Filial a que o documento pertence.</summary>
    public int IdFilial { get; set; }

    /// <summary>Cliente/Fornecedor/Instituição a que o documento foi emitido (MOD-02).</summary>
    public int IdEntidade { get; set; }

    /// <summary>Operação do documento (MOD-02). 0 = não informada.</summary>
    public int IdOperacao { get; set; }

    /// <summary>Tipo de documento (MOD-02). 0 = não informado.</summary>
    public int IdTipoDocumento { get; set; }

    /// <summary>Condição de pagamento (MOD-02). 0 = não informada.</summary>
    public int IdCondicaoPagamento { get; set; }

    /// <summary>Índice econômico do valor do documento (MOD-02).</summary>
    public int IdIndiceEconomico { get; set; }

    /// <summary>Índice econômico para o qual o valor é convertido (MOD-02).</summary>
    public int IdIndiceConversao { get; set; }

    /// <summary>Portador da(s) parcela(s) (MOD-02).</summary>
    public int IdPortador { get; set; }

    /// <summary>Forma de cobrança (MOD-02). 0 = não informada.</summary>
    public int IdFormaCobranca { get; set; }

    /// <summary>Id da origem que gerou o documento (venda, compra, contrato, OS…).</summary>
    public int IdOrigem { get; set; }

    /// <summary>Processo que originou o documento (RN-05-007).</summary>
    public ProcessoOrigem IdProcessoOrigem { get; set; }

    public string NumeroDocumento { get; set; } = string.Empty;

    public string NumeroCedente { get; set; } = string.Empty;

    public string Historico { get; set; } = string.Empty;

    /// <summary>Valor total do documento.</summary>
    public decimal Valor { get; set; }

    /// <summary>Valor convertido pelo índice econômico (RN-05-008).</summary>
    public decimal ValorConvertido { get; set; }

    public DateTime DataEmissao { get; set; }

    /// <summary>Indica se o título é a Pagar, a Receber ou Movimento de cartão.</summary>
    public PagarReceberTipo PagarReceber { get; set; }

    // Auditoria (Artigo III.4 — anulável)
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
}

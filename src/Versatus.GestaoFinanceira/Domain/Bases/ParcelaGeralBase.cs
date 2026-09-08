using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/ParcelaGeral.cs (legado, ObjectMaster)
// Base de: toda parcela financeira (título) — pai de ParcelaBase.
//
// POCO abstrata SÓ DE DADOS (Artigo III). O legado só tem propriedades aqui (a auditoria
// E1-T01 não achou validação/operação nesta classe).
//
// [E14] As colunas de boleto/remessa/retorno (NossoNumero, CodigoBarras, LinhaDigitavel,
// NumeroAgencia, DigitoAgencia, ContaCorrente, DigitoConta, Favorecido,
// DataProcessamentoBoleto, NomeArquivoRemessa, DataArquivoRemessa, HoraArquivoRemessa,
// AgenciaCodigoCedenteFormatado, …) NÃO entram aqui — são do épico E14 (integração
// bancária, CLR-03); o mapa coluna→propriedade completo da parcela é do E4-T02.
public abstract class ParcelaGeralBase
{
    /// <summary>Situação da parcela — reusa o enum do documento (não existe "SituacaoParcela").</summary>
    public SituacaoDocumento IdSituacao { get; set; }

    /// <summary>Portador da parcela (MOD-02).</summary>
    public int IdPortador { get; set; }

    /// <summary>Forma de pagamento da parcela (MOD-02). 0 = não informada.</summary>
    public int IdFormaPagamento { get; set; }

    /// <summary>Forma de cobrança da parcela (MOD-02). 0 = não informada.</summary>
    public int IdFormaCobrancaParcela { get; set; }

    /// <summary>Cobrador (MOD-02). 0 = não informado.</summary>
    public int IdCobrador { get; set; }

    /// <summary>Banco da parcela (MOD-02). 0 = não informado.</summary>
    public int IdBanco { get; set; }

    /// <summary>Se a parcela é à vista.</summary>
    public bool AVista { get; set; }

    /// <summary>Se a parcela já foi liquidada.</summary>
    public bool ParcelaLiquidada { get; set; }

    /// <summary>Valor cancelado da parcela.</summary>
    public decimal ValorCancelado { get; set; }

    /// <summary>Data de cobrança.</summary>
    public DateTime? DataCobranca { get; set; }
}

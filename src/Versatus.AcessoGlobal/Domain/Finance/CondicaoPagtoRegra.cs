using System;

namespace Versatus.AcessoGlobal.Domain.Finance;

/// <summary>
/// Representa uma regra associada à Condição de Pagamento (seja parcela ou faixa).
/// Tabela: GloCondicaoPagtoRegra
/// </summary>
public class CondicaoPagtoRegra
{
    public int IdCondicaoPagtoParcela { get; set; }
    public int IdCondicaoPagamento { get; set; }
    
    public int? NumeroDias { get; set; }
    public int? NumeroParcela { get; set; }
    public decimal? PercentualDivisao { get; set; }

    // Campos adicionais para Faixas (Aba Faixa)
    public int? DiaInicial { get; set; }
    public int? DiaFinal { get; set; }

    // Campos adicionais para Parcelas (Aba Parcelamento)
    public int? DiasLiberado { get; set; }
    public decimal? PercentualValorMinimo { get; set; }

    // Propriedade de navegação
    public virtual CondicaoPagamento? CondicaoPagamento { get; set; }
}

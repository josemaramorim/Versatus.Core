using System;
using System.Collections.Generic;
using Versatus.AcessoGlobal.Domain.Entities;

namespace Versatus.AcessoGlobal.Domain.Finance;

/// <summary>
/// Representa as informações de uma Condição de Pagamento no sistema.
/// Tabela: GloCondicaoPagamento
/// </summary>
public class CondicaoPagamento
{
    public int IdCondicaoPagamento { get; set; }
    
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public CondicaoPagtoTipo? IdTipoCondicaoPagto { get; set; }
    public Disponibilidade? IdDisponibilidade { get; set; }
    public VencimentoTipo? IdTipoVencimento { get; set; }
    
    // Lookups/FKs
    public int? IdGrupoCondicaoPagamento { get; set; }
    public int? IdFormaCobranca { get; set; }
    public int? IdFormaPagamento { get; set; }
    public int? IdFormaPagamentoVista { get; set; }
    
    public int? OrdemConsulta { get; set; }
    public bool UtilizarPdv { get; set; }
    
    // Acréscimo / Desconto
    public bool RecebeAcrescimo { get; set; }
    public decimal? Acrescimo { get; set; }
    public bool RecebeDesconto { get; set; }
    public decimal? Desconto { get; set; }

    // Configurações de Parcelamento (Aba 1)
    public bool AlteraParcelas { get; set; } // Condição do tipo livre
    public bool? AlteraNroParcela { get; set; }
    public ParcelamentoTipo? IdParcelamentoTipo { get; set; }
    public DivisaoParcelamentoTipo? TipoDivisaoParcelamento { get; set; }
    public int? QuantidadeParcela { get; set; }
    public int? DiasParcelamento { get; set; }
    public bool UsarMesComercial { get; set; }
    public int? DiasMinimoProximoMes { get; set; }
    public bool? PrimeiraParcelaAVista { get; set; }
    public bool? ObrigatorioFormaPagamento { get; set; }
    public ParcelamentoArredondamento? IdParcelaArredondamento { get; set; }

    // Configurações de Faixa (Aba 2)
    public int? QuantidadeFaixa { get; set; }

    // Configurações de Semanal (Aba 3)
    public DiaSemana? IdDiaSemana { get; set; }

    // Coleção de regras associadas
    public virtual ICollection<CondicaoPagtoRegra> Regras { get; set; } = new List<CondicaoPagtoRegra>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

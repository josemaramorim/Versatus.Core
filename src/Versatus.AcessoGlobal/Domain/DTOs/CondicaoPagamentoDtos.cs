using System;
using System.Collections.Generic;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.SharedKernel.Enums;

namespace Versatus.AcessoGlobal.Domain.DTOs;

public record CondicaoPagtoRegraDto
{
    public int IdCondicaoPagtoParcela { get; init; }
    public int NumeroDias { get; init; }
    public int NumeroParcela { get; init; }
    public decimal PercentualDivisao { get; init; }

    // Faixas
    public int? DiaInicial { get; init; }
    public int? DiaFinal { get; init; }

    // Parcelas
    public int? DiasLiberado { get; init; }
    public decimal? PercentualValorMinimo { get; init; }
}

public record CriarCondicaoPagamentoDto
{
    public string Descricao { get; init; } = string.Empty;
    public bool Ativo { get; init; } = true;
    public CondicaoPagtoTipo IdTipoCondicaoPagto { get; init; }
    public Disponibilidade IdDisponibilidade { get; init; }
    public VencimentoTipo IdTipoVencimento { get; init; }
    
    // Lookups
    public int? IdGrupoCondicaoPagamento { get; init; }
    public int? IdFormaCobranca { get; init; }
    public int? IdFormaPagamento { get; init; }
    public int? IdFormaPagamentoVista { get; init; }
    
    public int OrdemConsulta { get; init; }
    public bool UtilizarPdv { get; init; }
    
    // Acréscimo / Desconto
    public bool RecebeAcrescimo { get; init; }
    public decimal Acrescimo { get; init; }
    public bool RecebeDesconto { get; init; }
    public decimal Desconto { get; init; }

    // Configurações de Parcelamento (Aba 1)
    public bool AlteraParcelas { get; init; }
    public bool AlteraNroParcela { get; init; }
    public ParcelamentoTipo IdParcelamentoTipo { get; init; }
    public DivisaoParcelamentoTipo TipoDivisaoParcelamento { get; init; }
    public int QuantidadeParcela { get; init; }
    public int DiasParcelamento { get; init; }
    public bool UsarMesComercial { get; init; }
    public int DiasMinimoProximoMes { get; init; }
    public bool PrimeiraParcelaAVista { get; init; }
    public bool ObrigatorioFormaPagamento { get; init; }
    public ParcelamentoArredondamento IdParcelaArredondamento { get; init; }

    // Configurações de Faixa (Aba 2)
    public int QuantidadeFaixa { get; init; }

    // Configurações de Semanal (Aba 3)
    public DiaSemana IdDiaSemana { get; init; }

    // Regras associadas
    public List<CondicaoPagtoRegraDto> Regras { get; init; } = new();
}

public record EditarCondicaoPagamentoDto : CriarCondicaoPagamentoDto
{
    public int IdCondicaoPagamento { get; init; }
}

public record CondicaoPagamentoResponseDto
{
    public int IdCondicaoPagamento { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public bool Ativo { get; init; }
    public CondicaoPagtoTipo IdTipoCondicaoPagto { get; init; }
    public Disponibilidade IdDisponibilidade { get; init; }
    public VencimentoTipo IdTipoVencimento { get; init; }
    
    public int? IdGrupoCondicaoPagamento { get; init; }
    public int? IdFormaCobranca { get; init; }
    public int? IdFormaPagamento { get; init; }
    public int? IdFormaPagamentoVista { get; init; }
    
    public int OrdemConsulta { get; init; }
    public bool UtilizarPdv { get; init; }
    
    public bool RecebeAcrescimo { get; init; }
    public decimal Acrescimo { get; init; }
    public bool RecebeDesconto { get; init; }
    public decimal Desconto { get; init; }

    public bool AlteraParcelas { get; init; }
    public bool AlteraNroParcela { get; init; }
    public ParcelamentoTipo IdParcelamentoTipo { get; init; }
    public DivisaoParcelamentoTipo TipoDivisaoParcelamento { get; init; }
    public int QuantidadeParcela { get; init; }
    public int DiasParcelamento { get; init; }
    public bool UsarMesComercial { get; init; }
    public int DiasMinimoProximoMes { get; init; }
    public bool PrimeiraParcelaAVista { get; init; }
    public bool ObrigatorioFormaPagamento { get; init; }
    public ParcelamentoArredondamento IdParcelaArredondamento { get; init; }

    public int QuantidadeFaixa { get; init; }
    public DiaSemana IdDiaSemana { get; init; }

    public List<CondicaoPagtoRegraDto> Regras { get; init; } = new();
}

public record GrupoCondicaoPagamentoResponseDto(
    int IdGrupoCondicaoPagamento,
    string Descricao,
    bool Ativo
);

public record FormaCobrancaResponseDto(
    int IdFormaCobranca,
    string Descricao,
    string SiglaForma,
    bool Ativo
);

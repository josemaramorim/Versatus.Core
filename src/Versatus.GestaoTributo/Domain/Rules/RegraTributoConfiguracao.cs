using System;
using System.Collections.Generic;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Domain.Rules;

public class RegraTributoConfiguracao
{
    public int IdRegraTributoConfiguracao { get; set; }
    
    // Vigência (em vez de herdar de Vigencia, definimos direto na POCO)
    public DateTime VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }

    public DefinicaoTributaria IdDefinicaoTributaria { get; set; }
    public TipoPauta? IdTipoPauta { get; set; }
    public BaseSubstituicao? IdBaseSubstituicao { get; set; }
    public ValorNaoTributado? IdValorNaoTributado { get; set; }
    public Operadores? IdOperadorValorRetencao { get; set; }
    
    public int IdRegraTributo { get; set; }
    public decimal Aliquota { get; set; }
    public decimal PercentualBaseReduzida { get; set; }
    public bool? TributacaoEspecial { get; set; }
    public decimal BasePauta { get; set; }
    public decimal PercentualLucroIva { get; set; }
    public string? DispositivoLegal { get; set; }
    public bool ConsideraPessoaFisica { get; set; }
    public bool ConsideraRegimeSimples { get; set; }
    public decimal ValorRetencao { get; set; }
    public ComportamentoTributo IdComportamentoTributo { get; set; }
    public decimal AliquotaCupomFiscal { get; set; }
    public int? IdSituacaoTributaria { get; set; }
    public string? CodigoNaturezaReceita { get; set; }
    public bool IncideBaseSubstituicaoICMS { get; set; }
    public TipoBaseCalculoCargaMedia? IdTipoBaseCalculoCargaMedia { get; set; }
    public decimal PercentualCargaMedia { get; set; }
    public int? IdMotivoDesoneracao { get; set; }
    public bool PartilhaTributo { get; set; }
    public decimal AliquotaInterestadual { get; set; }
    public bool AplicarFCP { get; set; }
    public decimal AliquotaFCP { get; set; }
    public bool AplicarCreditoSn { get; set; }
    public int? IdObservacaoFiscal { get; set; }
    public int? IdBeneficioFiscal { get; set; }
    public bool AplicarRepasseST { get; set; }
    public bool SomarFreteBaseTributo { get; set; }
    public bool SomarSeguroBaseTributo { get; set; }
    public bool SomarOutrasDespesasBaseTributo { get; set; }
    public bool UsaPautaFiscal { get; set; }
    public int? IdRestituicaoICMSST { get; set; }
    public decimal AliquotaDiferido { get; set; }
    public bool AplicarAliquotaNcm { get; set; }
    public bool AplicarAliquotaFCPNcm { get; set; }
    public bool UsaLimiteCreditoST { get; set; }
    public decimal PercentualLimiteCreditoST { get; set; }
    public bool IcmsDescontaBasePisCofins { get; set; }
    public bool IncideBaseIcms { get; set; }
    public bool SomarAcrescimoBaseTributo { get; set; }
    public bool SubtrairDescontoBaseTributo { get; set; }
    public TipoExigibilidadeISS? IdTipoExigibilidadeISS { get; set; }
    public bool AplicarReducaoAposIncidenciaBaseCalculo { get; set; }
    public bool SubtrairDestacadoValorSubstituicao { get; set; }
    public bool IncideBaseIPIDevolucao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public RegraTributo? RegraTributo { get; set; }
    public SituacaoTributaria? SituacaoTributaria { get; set; }
    
    private readonly List<RegraTributacaoEspecial> _itens = [];
    public IReadOnlyList<RegraTributacaoEspecial> Itens => _itens.AsReadOnly();
}

using System;
using System.Collections.Generic;
using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.ICMS;

namespace Versatus.GestaoTributo.Domain.Rules;

public class Tributacao
{
    public int IdTributacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public bool AplicarAliquotaNcmUf { get; set; }
    public TipoRegraTributacao RegraTributacaoTipo { get; set; } = TipoRegraTributacao.Nenhuma;
    public TipoBaseCalculo BaseCalculoTipo { get; set; } = TipoBaseCalculo.ValorNota;
    
    public int IdTributo { get; set; }
    public int? IdRegraEntrada { get; set; }
    public int? IdRegraSaida { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public TributoFiscal? Tributo { get; set; }
    public RegraTributo? RegraEntrada { get; set; }
    public RegraTributo? RegraSaida { get; set; }

    private readonly List<DetalheUfTributacao> _detalhesUf = [];
    public IReadOnlyList<DetalheUfTributacao> DetalhesUf => _detalhesUf.AsReadOnly();

    private readonly List<DetalheCidadeTributacao> _detalhesCidade = [];
    public IReadOnlyList<DetalheCidadeTributacao> DetalhesCidade => _detalhesCidade.AsReadOnly();
}

using System;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class DetalheUfTributacao
{
    public int IdDetalheUfTributacao { get; set; }
    public string UfDestino { get; set; } = string.Empty;
    public string UfOrigem { get; set; } = string.Empty;
    
    public int IdTributacao { get; set; }
    public int? IdRegraEntrada { get; set; }
    public int? IdRegraSaida { get; set; }
    
    public DateTime VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }
    public bool Ativo { get; set; } = true;
    public TipoBaseCalculo? IdTipoBaseCalculo { get; set; }

    // Relacionamentos
    public Tributacao? Tributacao { get; set; }
    public RegraTributo? RegraEntrada { get; set; }
    public RegraTributo? RegraSaida { get; set; }
}

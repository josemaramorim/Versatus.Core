using System;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class DetalheCidadeTributacao
{
    public int IdDetalheCidadeTributacao { get; set; }
    public int? IdRegraEntrada { get; set; }
    public int? IdRegraSaida { get; set; }
    public int IdTributacao { get; set; }
    public int IdCidade { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }
    public TipoBaseCalculo? IdTipoBaseCalculo { get; set; }

    // Relacionamentos
    public Tributacao? Tributacao { get; set; }
    public RegraTributo? RegraEntrada { get; set; }
    public RegraTributo? RegraSaida { get; set; }
}

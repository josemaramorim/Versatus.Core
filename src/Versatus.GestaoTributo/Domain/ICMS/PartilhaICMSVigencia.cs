using System;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class PartilhaICMSVigencia
{
    public int IdPartilhaICMSVigencia { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }
    public decimal PercentualPartilhaICMS { get; set; }
}

using System;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class RegimeTributarioVigencia
{
    public int IdRegimeTributarioVigencia { get; set; }
    public int IdFilial { get; set; }
    public int? IdIncidenciaTributaria { get; set; }
    public int? IdApropriacaoCredito { get; set; }
    public int? IdTipoContribuicaoApurada { get; set; }
    public int? IdRegimeEscrituracaoApuracaoPresumido { get; set; }
    public DateTime VigenciaInicio { get; set; }
    public DateTime? VigenciaFim { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int? IdEscrituracaoNfeEcf { get; set; }
    public bool EnviarPlanoContabil { get; set; }
    public int IdApuracaoContribuicaoPrevidenciaria { get; set; }
}

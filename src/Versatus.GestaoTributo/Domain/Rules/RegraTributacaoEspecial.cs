using System;

namespace Versatus.GestaoTributo.Domain.Rules;

public class RegraTributacaoEspecial
{
    public int IdRegraTributoConfiguracao { get; set; }
    public int IdTributoFormula { get; set; }
    public int IdRegraTributoEspecial { get; set; }
    public int Ordem { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public RegraTributoConfiguracao? Configuracao { get; set; }
    public RegraTributo? RegraEspecial { get; set; }
}

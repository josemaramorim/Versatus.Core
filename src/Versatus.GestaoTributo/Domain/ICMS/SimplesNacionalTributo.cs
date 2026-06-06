using System;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class SimplesNacionalTributo
{
    public int IdSimplesNacionalTributo { get; set; }
    public int IdSimplesNacional { get; set; }
    public int IdTributoFiscal { get; set; }
    public decimal? Aliquota { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public SimplesNacional? SimplesNacional { get; set; }
    public TributoFiscal? TributoFiscal { get; set; }
}

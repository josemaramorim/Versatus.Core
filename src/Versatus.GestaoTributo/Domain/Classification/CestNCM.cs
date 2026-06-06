using System;

namespace Versatus.GestaoTributo.Domain.Classification;

public class CestNCM
{
    public int IdCestNcm { get; set; }
    public int IdCest { get; set; }
    public string Ncm { get; set; } = string.Empty;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public Cest? Cest { get; set; }
}

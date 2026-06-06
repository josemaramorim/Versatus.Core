using System;
using System.Collections.Generic;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class SimplesNacional
{
    public int IdSimplesNacional { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Aliquota { get; set; }
    public bool Ativo { get; set; } = true;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    private readonly List<SimplesNacionalTributo> _tributos = [];
    public IReadOnlyList<SimplesNacionalTributo> Tributos => _tributos.AsReadOnly();
}

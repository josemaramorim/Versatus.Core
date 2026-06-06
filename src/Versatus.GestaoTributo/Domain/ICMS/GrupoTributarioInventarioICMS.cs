using System;
using System.Collections.Generic;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class GrupoTributarioInventarioICMS
{
    public int IdGrupoTributarioInventarioICMS { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public bool Substituicao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    private readonly List<GrupoTributarioICMS> _grupos = [];
    public IReadOnlyList<GrupoTributarioICMS> Grupos => _grupos.AsReadOnly();
}

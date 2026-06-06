using System;
using Versatus.GestaoTributo.Domain.Rules;

namespace Versatus.GestaoTributo.Domain.ICMS;

public class GrupoTributarioICMS
{
    public int IdGrupoTributarioICMS { get; set; }
    public int IdGrupoTributarioInventarioICMS { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DefinicaoTributaria IdDefinicaoTributaria { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public GrupoTributarioInventarioICMS? GrupoTributarioInventario { get; set; }
}

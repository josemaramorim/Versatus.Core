using System;

namespace Versatus.GestaoTributo.Domain.Classification;

public class SituacaoTributaria
{
    public int IdSituacaoTributaria { get; set; }
    public int IdTributoFiscal { get; set; }
    public int? IdSituacaoTributariaFiscal { get; set; }
    public int? IdSituacaoTributariaFiscalSped { get; set; }
    public int? IdSituacaoTributariaFiscalSN { get; set; }
    public int? IdSituacaoTributariaFiscalSNSped { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? SituacaoTributariaCodigo { get; set; } // Legado: SituacaoTributaria -> SituacaoTributariaCodigo
    public string? SituacaoSimplesNacional { get; set; }
    public string? SituacaoSped { get; set; }
    public string? SituacaoSimplesNacionalSped { get; set; }
    public bool Ativo { get; set; } = true;
    public bool Substituicao { get; set; }
    public bool Monofasico { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamento
    public TributoFiscal? TributoFiscal { get; set; }
}

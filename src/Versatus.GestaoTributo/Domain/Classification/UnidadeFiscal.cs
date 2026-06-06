using System;

namespace Versatus.GestaoTributo.Domain.Classification;

public class UnidadeFiscal
{
    public int IdUnidadeFiscal { get; set; }
    public string SiglaFiscal { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public int? DecimaisQuantidade { get; set; }
    public int? DecimaisValor { get; set; }
    public string? SiglaFiscalEcf { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

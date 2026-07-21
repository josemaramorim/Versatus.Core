using System;

namespace Versatus.AcessoGlobal.Domain.Finance;

/// <summary>
/// Representa uma Forma de Cobrança.
/// Tabela: GloFormaCobranca
/// </summary>
public class FormaCobranca
{
    public int IdFormaCobranca { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string SiglaForma { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

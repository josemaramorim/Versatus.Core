using System;

namespace Versatus.AcessoGlobal.Domain.Finance;

/// <summary>
/// Representa um Grupo de Condição de Pagamento.
/// Tabela: GloGrupoCondicaoPagamento
/// </summary>
public class GrupoCondicaoPagamento
{
    public int IdGrupoCondicaoPagamento { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

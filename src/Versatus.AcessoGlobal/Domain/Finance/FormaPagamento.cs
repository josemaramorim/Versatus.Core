using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.SharedKernel.Enums;

namespace Versatus.AcessoGlobal.Domain.Finance;

/// <summary>
/// Representa as informações de uma Forma de Pagamento no sistema.
/// Origem: servidor/objeto de negócio/acesso.global/FormaPagamento.cs (legado)
/// Tabela: GloFormaPagamento
/// </summary>
public class FormaPagamento
{
    public int IdForma { get; set; }
    
    /// <summary>
    /// Código / Sigla da forma de pagamento. Mapeia para SiglaForma no legado.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome / Descrição da forma de pagamento. Mapeia para Descricao no legado.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da forma de pagamento. Mapeia para IdTipoFormaPagamento no legado.
    /// </summary>
    public FormaPagtoTipo Tipo { get; set; }

    public bool Ativo { get; set; } = true;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

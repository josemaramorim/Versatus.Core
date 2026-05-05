using Versatus.AcessoGlobal.Domain.Classification;

namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Representa as informações específicas de um Fornecedor.
/// Origem: servidor/objeto de negócio/acesso.global/Fornecedor.cs (legado)
/// Tabela: GloFornecedor
/// </summary>
public class Fornecedor
{
    public int IdFornecedor { get; set; }
    
    public string? CodigoAlternativo { get; set; }
    public string? ContaContabil { get; set; }
    public bool Ativo { get; set; } = true;
    public bool IsFornecedorCotacao { get; set; }

    // FKs
    public int? IdCategoria { get; set; }
    public int? IdCondicaoPagamento { get; set; }

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public Entidade? Entidade { get; set; }
    public Categoria? Categoria { get; set; }
}

using Versatus.AcessoGlobal.Domain.Classification;

namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Representa as informações específicas de um Cliente.
/// Origem: servidor/objeto de negócio/acesso.global/Cliente.cs (legado)
/// Tabela: GloCliente
/// </summary>
public class Cliente
{
    public int IdCliente { get; set; }
    
    public string? LocalTrabalho { get; set; }
    public string? TelefoneTrabalho { get; set; }
    public string? Profissao { get; set; }
    public string? InscricaoProdutor { get; set; }
    public string? CodigoAlternativo { get; set; }
    
    public bool Ativo { get; set; } = true;
    public bool Bloqueado { get; set; }
    public bool ItemFinanceiroPadrao { get; set; } = true;
    public bool EnviarCNDNFe { get; set; }
    
    public double RendaMensal { get; set; }
    public double LimiteCredito { get; set; }
    public double ValorAluguel { get; set; }
    
    public DateTime? DataAdmissao { get; set; }
    public DateTime? HoraCobranca { get; set; }
    
    public TipoImovel ImovelTipo { get; set; }
    public SituacaoClienteSPC SituacaoSPC { get; set; }
    
    // FKs
    public int? IdCategoria { get; set; }
    public int? IdClienteConceito { get; set; }
    public int? IdDiaSemanaCobranca { get; set; }

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

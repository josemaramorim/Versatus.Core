namespace Versatus.AcessoGlobal.Domain.Entities;

/// <summary>
/// Representa as informações específicas de um Funcionário.
/// Origem: servidor/objeto de negócio/acesso.global/Funcionario.cs (legado)
/// Tabela: GloFuncionario
/// </summary>
public class Funcionario
{
    public int IdFuncionario { get; set; }
    
    public string? Ctps { get; set; }
    public string? SerieCtps { get; set; }
    public string? UfCtps { get; set; }
    public DateTime? DataEmissaoCtps { get; set; }
    
    public string? NumeroCnh { get; set; }
    public string? CategoriaCnh { get; set; }
    public DateTime? DataVencimentoCnh { get; set; }
    
    public string? InscricaoPis { get; set; }
    public int? IdBancoPis { get; set; }
    public string? NumeroAgenciaPis { get; set; }
    public string? NomeAgenciaPis { get; set; }
    public DateTime? DataInscricaoPis { get; set; }
    
    public string? NomePai { get; set; }
    public string? NomeMae { get; set; }
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;

    // FKs
    public int? IdRaca { get; set; }
    public int? IdTipoDeficiencia { get; set; }
    public int? IdPais { get; set; }

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public Entidade? Entidade { get; set; }
}

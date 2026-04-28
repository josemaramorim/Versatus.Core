namespace Versatus.AcessoGlobal.Domain.Classification;

using Versatus.AcessoGlobal.Domain.Organization;

/// <summary>
/// Entidade que representa uma Classe Financeira/Material.
/// Mapeada da tabela legada FinClasse.
/// </summary>
public class Classe
{
    /// <summary>
    /// Identificador único (IdClasse).
    /// </summary>
    public int IdClasse { get; set; }

    /// <summary>
    /// Chave estrangeira para a Filial à qual esta classe pertence.
    /// </summary>
    public int IdFilial { get; set; }

    /// <summary>
    /// Chave estrangeira para a Classe pai (autorrelacionamento).
    /// </summary>
    public int? IdClassePai { get; set; }

    /// <summary>
    /// Nome/Descrição da classe.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Código hierárquico formatado (ex: 1.01.002). Corresponde ao campo Extenso no legado.
    /// </summary>
    public string CodigoFormatado { get; set; } = string.Empty;

    /// <summary>
    /// Nível hierárquico (1, 2, 3...).
    /// </summary>
    public int Nivel { get; set; }

    /// <summary>
    /// Natureza da classe (1 - Receita, 2 - Despesa, etc.).
    /// </summary>
    public int IdTipoNatureza { get; set; }

    /// <summary>
    /// Define se é Sintética (Pai) ou Analítica (Folha).
    /// </summary>
    public int IdSinteticoAnalitico { get; set; }

    /// <summary>
    /// Define se a classe está ativa.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public Filial? Filial { get; set; }
    public Classe? ClassePai { get; set; }
    public ICollection<Classe> SubClasses { get; set; } = new List<Classe>();

    // Metadados de Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

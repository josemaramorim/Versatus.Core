namespace Versatus.AcessoGlobal.Domain.Classification;

using Versatus.AcessoGlobal.Domain.Organization;

/// <summary>
/// Entidade que representa um Centro de Custo.
/// Mapeada da tabela legada GloCentroCusto.
/// </summary>
public class CentroCusto
{
    /// <summary>
    /// Identificador único (IdCentroCusto).
    /// </summary>
    public int IdCentroCusto { get; set; }

    /// <summary>
    /// Chave estrangeira para a Filial à qual este centro de custo pertence.
    /// </summary>
    public int IdFilial { get; set; }

    /// <summary>
    /// Chave estrangeira para o Centro de Custo pai (autorrelacionamento).
    /// </summary>
    public int? IdCentroCustoPai { get; set; }

    /// <summary>
    /// Nome/Descrição do centro de custo.
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
    /// Define se é Sintético (1 - Pai/Agrupador) ou Analítico (2 - Folha/Lançável).
    /// </summary>
    public int IdSinteticoAnalitico { get; set; }

    /// <summary>
    /// Define se o centro de custo está ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    public Filial? Filial { get; set; }
    public CentroCusto? CentroCustoPai { get; set; }
    public ICollection<CentroCusto> SubCentros { get; set; } = new List<CentroCusto>();

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

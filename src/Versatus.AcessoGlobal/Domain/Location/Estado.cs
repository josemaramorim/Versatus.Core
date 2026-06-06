namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa um Estado/Unidade Federativa (UF).
/// Mapeada da tabela legada GloEstado.
/// </summary>
public class Estado
{
    /// <summary>
    /// Identificador único (IdSequencialEstado no legado).
    /// </summary>
    public int IdEstado { get; set; }

    /// <summary>
    /// Chave estrangeira para o País ao qual este estado pertence.
    /// </summary>
    public int IdPais { get; set; }

    /// <summary>
    /// Sigla da Unidade Federativa (ex: SP, RJ, MG).
    /// </summary>
    public string Sigla { get; set; } = string.Empty;

    /// <summary>
    /// Nome descritivo do estado (Descricao no legado).
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Código do IBGE para o estado (específico para estados brasileiros).
    /// </summary>
    public string CodigoIBGE { get; set; } = string.Empty;

    /// <summary>
    /// Define se o estado está ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Se exige identificação de responsável técnico (regra do legado).
    /// </summary>
    public bool ExigeIdentificacaoTecnico { get; set; }

    /// <summary>
    /// Se exige registro no sistema (regra do legado).
    /// </summary>
    public bool ExigeRegistroSistema { get; set; }

    // Propriedade de Navegação
    public Pais? Pais { get; set; }

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

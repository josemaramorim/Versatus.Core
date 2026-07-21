namespace Versatus.AcessoGlobal.Domain.Configuration;

/// <summary>
/// Representa as configurações e parâmetros do sistema.
/// Origem: servidor/objeto de negócio/acesso.global/Parametro.cs (legado)
/// Tabela: GloParametro
/// </summary>
public class Parametro
{
    public int IdParam { get; set; }

    /// <summary>
    /// Chave/Nome do parâmetro. Mapeia para Nome no legado.
    /// </summary>
    public string Chave { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do parâmetro. Mapeia para Descricao no legado.
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Valor atual do parâmetro. Mapeia para Objeto no legado.
    /// </summary>
    public string? Valor { get; set; }

    /// <summary>
    /// Tipo de dados do valor (string, int, bool). Mapeia para IdTipoValor no legado.
    /// </summary>
    public int? Tipo { get; set; }

    /// <summary>
    /// Agrupador do parâmetro para fins de exibição/organização. Mapeia para Agrupador no legado.
    /// </summary>
    public int Agrupador { get; set; }

    /// <summary>
    /// Indica se o parâmetro deve ser exibido na tela de configurações. Mapeia para Visivel no legado.
    /// </summary>
    public bool Visivel { get; set; } = true;

    /// <summary>
    /// ID da rotina associada. Mapeia para IdRotina no legado.
    /// </summary>
    public int? IdRotina { get; set; }

    /// <summary>
    /// Tipo de parâmetro (Sistema, Usuário, etc.). Mapeia para TipoParametro no legado.
    /// </summary>
    public int? TipoParametro { get; set; }
}

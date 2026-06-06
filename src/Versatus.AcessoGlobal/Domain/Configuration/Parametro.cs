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


}

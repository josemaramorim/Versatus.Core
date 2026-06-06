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
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor atual do parâmetro. Mapeia para Objeto no legado.
    /// </summary>
    public string Valor { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de dados do valor (string, int, bool). Mapeia para IdTipoValor no legado.
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

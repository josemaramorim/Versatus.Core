namespace Versatus.AcessoGlobal.Domain.Configuration;

/// <summary>
/// Representa as informações de uma Série de Documento no sistema.
/// Origem: servidor/objeto de negócio/acesso.global/SerieDocumento.cs (legado)
/// Tabela: GloSerieDocumento
/// </summary>
public class SerieDocumento
{
    public int IdSerie { get; set; }

    /// <summary>
    /// Código da série de documento (ex: "01", "99"). Mapeia para IdSerieDocumento no legado.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome / Descrição da série. Mapeia para Descricao no legado.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Modelo Fiscal (ex: "55" para NF-e). Mapeia para ModeloFiscal no legado.
    /// </summary>
    public string Prefixo { get; set; } = string.Empty;

    /// <summary>
    /// Próximo número sequencial a ser gerado para esta série.
    /// </summary>
    public int ProximoNumero { get; set; }

    public bool Ativa { get; set; } = true;

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}

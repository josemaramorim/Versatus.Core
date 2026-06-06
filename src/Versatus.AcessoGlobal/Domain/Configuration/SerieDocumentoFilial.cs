using Versatus.AcessoGlobal.Domain.Organization;

namespace Versatus.AcessoGlobal.Domain.Configuration;

/// <summary>
/// Representa o relacionamento entre Série de Documento e Filial, controlando a numeração.
/// Origem: servidor/objeto de negócio/acesso.global/SerieDocumentoFilial.cs (legado)
/// Tabela: GloSerieDocumentoFilial
/// </summary>
public class SerieDocumentoFilial
{
    public int IdRelacao { get; set; }
    
    /// <summary>
    /// ID da Série de Documento (IdSequencialSerieDocto no legado).
    /// </summary>
    public int IdSerie { get; set; }

    /// <summary>
    /// ID da Filial.
    /// </summary>
    public int IdFilial { get; set; }

    /// <summary>
    /// Próximo número sequencial a ser gerado para esta série nesta filial.
    /// </summary>
    public int ProximoNumero { get; set; }

    // Auditoria
    public int IdUsuarioInclusao { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public SerieDocumento? SerieDocumento { get; set; }
    public Filial? Filial { get; set; }
}

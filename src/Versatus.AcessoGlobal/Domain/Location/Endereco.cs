namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa um Logradouro/Endereço base.
/// Mapeada da tabela legada GloEndereco.
/// </summary>
public class Endereco
{
    /// <summary>
    /// Identificador único (IdGloEndereco no legado).
    /// </summary>
    public int IdEndereco { get; set; }

    /// <summary>
    /// Chave estrangeira para a Cidade.
    /// </summary>
    public int IdCidade { get; set; }

    /// <summary>
    /// Chave estrangeira para o Bairro Inicial.
    /// </summary>
    public int IdBairroInicial { get; set; }

    /// <summary>
    /// Chave estrangeira para o Bairro Final (opcional, para logradouros que cruzam bairros).
    /// </summary>
    public int? IdBairroFinal { get; set; }

    /// <summary>
    /// Chave estrangeira para o Tipo de Logradouro (Rua, Av, etc).
    /// </summary>
    public int IdTipoLogradouro { get; set; }

    /// <summary>
    /// Nome do logradouro (ex: Paulista, Sete de Setembro).
    /// </summary>
    public string Logradouro { get; set; } = string.Empty;

    /// <summary>
    /// CEP do logradouro.
    /// </summary>
    public string CEP { get; set; } = string.Empty;

    /// <summary>
    /// Complemento ou informações adicionais do logradouro.
    /// </summary>
    public string Complemento { get; set; } = string.Empty;

    /// <summary>
    /// Latitude para geolocalização.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude para geolocalização.
    /// </summary>
    public double Longitude { get; set; }

    // Propriedades de Navegação
    public Cidade? Cidade { get; set; }
    public Bairro? BairroInicial { get; set; }
    public Bairro? BairroFinal { get; set; }
    public TipoLogradouro? TipoLogradouro { get; set; }
}

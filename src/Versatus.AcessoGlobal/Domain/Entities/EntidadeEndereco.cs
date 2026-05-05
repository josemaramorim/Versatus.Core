using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Domain.Entities;

public enum EnderecoTipo
{
    ComercialResidencial = 1,
    Comercial = 2,
    Residencial = 3,
    Entrega = 4,
    Cobranca = 5,
    Outro = 6
}

/// <summary>
/// Representa o endereço vinculado a uma Entidade.
/// No legado, esta tabela contém os campos de endereço diretamente (denormalizado).
/// Origem: servidor/objeto de negócio/acesso.global/EntidadeEndereco.cs (legado)
/// Tabela: GloEntidadeEndereco
/// </summary>
public class EntidadeEndereco
{
    public int IdEntidadeEndereco { get; set; }
    public int IdEntidade { get; set; }
    public int IdCidade { get; set; }
    public int IdTipoLogradouro { get; set; }
    public int? IdBairro { get; set; }
    
    public int Numero { get; set; }
    public string? Logradouro { get; set; }
    public string? Complemento { get; set; }
    public string? Cep { get; set; }
    public string? CaixaPostal { get; set; }
    
    public bool Ativo { get; set; } = true;
    public bool Padrao { get; set; }
    public EnderecoTipo TipoEndereco { get; set; }

    // Relacionamentos
    public Entidade? Entidade { get; set; }
    public Cidade? Cidade { get; set; }
    public Bairro? Bairro { get; set; }
    public TipoLogradouro? TipoLogradouro { get; set; }
}

using System;
using System.Collections.Generic;

namespace Versatus.AcessoGlobal.Domain.DTOs;

public record SalvarEntidadeDto
{
    public string Nome { get; init; } = string.Empty;
    public string? Apelido { get; init; }
    public string? Email { get; init; }
    public string? EmailNFE { get; init; }
    public string? EmailFinanceiro { get; init; }
    public string? EmailVenda { get; init; }
    public string? EmailCompra { get; init; }
    public string? HomePage { get; init; }
    public string? Observacao { get; init; }
    public string? InscricaoEstadual { get; init; }
    public string? InscricaoMunicipal { get; init; }
    public string? InscricaoSuframa { get; init; }
    public bool Ativo { get; init; } = true;
    public int TipoPessoa { get; init; } = 1; // 1 = Fisica, 2 = Juridica
    public string? Cpf { get; init; }
    public string? Cnpj { get; init; }
    public string? Rg { get; init; }

    // Roles (Papéis)
    public bool IsCliente { get; init; }
    public bool IsFornecedor { get; init; }
    public bool IsTransportadora { get; init; }
    public bool IsComissionado { get; init; }
    public bool IsAgencia { get; init; }
    public bool IsFinanceira { get; init; }
    public bool IsFilial { get; init; }
    public bool IsFuncionario { get; init; }
    public bool IsObra { get; init; }
    public bool IsRepresentante { get; init; }
    public bool IsOutro { get; init; }
    public bool IsProspecto { get; init; }
    public bool IsContador { get; init; }
    public bool IsAluno { get; init; }
    public bool IsProfessor { get; init; }
    public bool IsIntermediador { get; init; }

    // Listas filhas
    public List<EnderecoDto>? Enderecos { get; init; }
    public List<ContatoDto>? Contatos { get; init; }
    public List<CnaeDto>? Cnaes { get; init; }
    public List<TelefoneDto>? Telefones { get; init; }
}

public record EnderecoDto
{
    public int Id { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string Logradouro { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Bairro { get; init; } = string.Empty;
    public string Cidade { get; init; } = string.Empty;
    public string Uf { get; init; } = string.Empty;
    public string Cep { get; init; } = string.Empty;
}

public record ContatoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cargo { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Celular { get; init; } = string.Empty;
}

public record CnaeDto
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public bool Principal { get; init; }
}

public record TelefoneDto
{
    public int Id { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Contato { get; init; } = string.Empty;
}

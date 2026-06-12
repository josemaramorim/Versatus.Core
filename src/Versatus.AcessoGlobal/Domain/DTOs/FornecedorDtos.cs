using System;

namespace Versatus.AcessoGlobal.Domain.DTOs;

public record CriarFornecedorDto
{
    public int? IdEntidade { get; init; }
    public CriarEntidadeDto? NovaEntidade { get; init; }
    public string? CodigoAlternativo { get; init; }
    public string? ContaContabil { get; init; }
    public bool Ativo { get; init; } = true;
    public bool IsFornecedorCotacao { get; init; }
    public int? IdCategoria { get; init; }
    public int? IdCondicaoPagamento { get; init; }
}

public record FornecedorResponseDto(
    int IdFornecedor,
    string Nome,
    string? CodigoAlternativo,
    string? ContaContabil,
    bool Ativo
);

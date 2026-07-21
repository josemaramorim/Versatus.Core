using System;

namespace Versatus.AcessoGlobal.Domain.DTOs;

public record CriarFuncionarioDto
{
    public int? IdEntidade { get; init; }
    public CriarEntidadeDto? NovaEntidade { get; init; }
    public string? Ctps { get; init; }
    public string? SerieCtps { get; init; }
    public string? UfCtps { get; init; }
    public DateTime? DataEmissaoCtps { get; init; }
    public string? NumeroCnh { get; init; }
    public string? CategoriaCnh { get; init; }
    public DateTime? DataVencimentoCnh { get; init; }
    public string? InscricaoPis { get; init; }
    public int? IdBancoPis { get; init; }
    public string? NumeroAgenciaPis { get; init; }
    public string? NomeAgenciaPis { get; init; }
    public DateTime? DataInscricaoPis { get; init; }
    public string? NomePai { get; init; }
    public string? NomeMae { get; init; }
    public string? Observacao { get; init; }
    public bool Ativo { get; init; } = true;
    public int? IdRaca { get; init; }
    public int? IdTipoDeficiencia { get; init; }
    public int? IdPais { get; init; }
}

public record FuncionarioResponseDto(
    int IdFuncionario,
    string Nome,
    string? Ctps,
    bool Ativo
);

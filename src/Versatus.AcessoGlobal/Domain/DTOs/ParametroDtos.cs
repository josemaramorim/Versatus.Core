namespace Versatus.AcessoGlobal.Domain.DTOs;

/// <summary>
/// DTO para criação e atualização de um parâmetro do sistema.
/// </summary>
public record SalvarParametroDto(
    string Chave,
    string? Descricao,
    string? Valor,
    int? Tipo
);

/// <summary>
/// DTO para retorno de dados do parâmetro, incluindo seu valor configurado.
/// </summary>
public record ParametroPaginadoDto(
    int Id,
    string Chave,
    string? Descricao,
    string? Valor,
    int? Tipo
);

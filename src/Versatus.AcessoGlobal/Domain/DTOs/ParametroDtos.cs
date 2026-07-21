namespace Versatus.AcessoGlobal.Domain.DTOs;

/// <summary>
/// DTO para criação e atualização de um parâmetro do sistema.
/// </summary>
public record SalvarParametroDto(
    string Chave,
    string? Descricao,
    string? Valor,
    int? Tipo,
    int Agrupador,
    bool Visivel,
    int? IdRotina,
    int? TipoParametro
);

/// <summary>
/// DTO para retorno de dados do parâmetro, incluindo seu valor configurado e escopo.
/// </summary>
public record ParametroPaginadoDto(
    int Id,
    string Chave,
    string? Descricao,
    string? Valor, // Valor padrão do parâmetro (Objeto no legado)
    int? Tipo,
    int Agrupador,
    bool Visivel,
    int? IdRotina,
    int? TipoParametro,
    int? IdParametroValor,
    string? ValorConfigurado,
    bool Marcado
);

/// <summary>
/// DTO de item para salvar valor do parâmetro.
/// </summary>
public record SalvarValorParametroDto(
    int IdParametro,
    string? ValorConfigurado,
    bool Marcado
);

/// <summary>
/// DTO principal para salvar valores de parâmetros em lote.
/// </summary>
public record SalvarValoresParametrosDto(
    int TipoParametro,
    int? IdPerfil,
    List<SalvarValorParametroDto> Valores
);

/// <summary>
/// DTO para retorno de opções de enumerado dinâmico.
/// </summary>
public record EnumOpcaoDto(
    string Value,
    string Label
);

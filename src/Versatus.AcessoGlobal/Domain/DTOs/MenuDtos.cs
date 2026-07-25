namespace Versatus.AcessoGlobal.Domain.DTOs;

public record ModuloMenuDto(
    int IdModulo,
    string Nome,
    string? PrefixoRota,
    string? IconeMui,
    string? CorHex,
    int Ordem,
    List<MenuItemDto> Menus
);

public record MenuItemDto(
    int IdMenu,
    string Descricao,
    int Ordem,
    List<MenuItemDto> SubMenus,
    List<RotinaItemDto> Rotinas
);

public record RotinaItemDto(
    int IdRotina,
    string Nome,
    string? Objeto,
    string? RotaWeb,
    int Ordem,
    string RotaCompleta
);

public record FavoritoDto(
    int IdFavorito,
    int IdRotina,
    string NomeRotina,
    string RotaCompleta,
    string NomeModulo,
    string? CorHex,
    string CaminhoCompleto
);

public record AdicionarFavoritoDto(
    int IdRotina
);

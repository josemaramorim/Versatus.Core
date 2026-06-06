namespace Versatus.AcessoGlobal.Domain.DTOs;

public record LoginRequest(string Username, string Password);

public record UsuarioResponseDto(
    int Id,
    string Nome,
    string Login,
    string Perfil
);

public record LoginResponseDto(
    string Token,
    UsuarioResponseDto User
);

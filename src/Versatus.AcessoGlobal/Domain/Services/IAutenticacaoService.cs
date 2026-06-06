using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Domain.Services;

public interface IAutenticacaoService
{
    Task<LoginResponseDto?> AutenticarAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AuthController(IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _autenticacaoService.AutenticarAsync(request);

        if (result == null)
        {
            return BadRequest(new { message = "Usuário não encontrado." });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(new
        {
            username = User.Identity?.Name,
            claims = claims
        });
    }
}

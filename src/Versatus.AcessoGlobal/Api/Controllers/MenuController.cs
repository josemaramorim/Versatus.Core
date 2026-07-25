using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Services;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet("arvore")]
    public async Task<IActionResult> ObterArvore(CancellationToken cancellationToken = default)
    {
        var arvore = await _menuService.ObterArvoreAsync(cancellationToken);
        return Ok(arvore);
    }

    [HttpGet("favoritos")]
    public async Task<IActionResult> ObterFavoritos(CancellationToken cancellationToken = default)
    {
        var favoritos = await _menuService.ObterFavoritosAsync(1, cancellationToken);
        return Ok(favoritos);
    }

    [HttpPost("favoritos")]
    public async Task<IActionResult> AdicionarFavorito([FromBody] AdicionarFavoritoDto dto, CancellationToken cancellationToken = default)
    {
        var result = await _menuService.AdicionarFavoritoAsync(dto.IdRotina, 1, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = "Falha ao adicionar favorito", errors = result.Errors });
        }
        return Ok(result.Value);
    }

    [HttpDelete("favoritos/{idRotina}")]
    public async Task<IActionResult> RemoverFavorito(int idRotina, CancellationToken cancellationToken = default)
    {
        var result = await _menuService.RemoverFavoritoAsync(idRotina, 1, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = "Falha ao remover favorito", errors = result.Errors });
        }
        return Ok(new { success = true });
    }
}

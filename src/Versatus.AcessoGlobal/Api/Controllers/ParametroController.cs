using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParametroController : ControllerBase
{
    private readonly IParametroService _parametroService;

    public ParametroController(IParametroService parametroService)
    {
        _parametroService = parametroService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var parametros = await _parametroService.ListarTodosAsync();
        return Ok(parametros);
    }

    [HttpGet("{chave}")]
    public async Task<IActionResult> ObterValor(string chave)
    {
        var valor = await _parametroService.ObterValorAsync(chave);
        if (valor == null)
        {
            return NotFound(new { message = $"Valor do parâmetro '{chave}' não encontrado." });
        }
        return Ok(new { chave, valor });
    }

    [HttpPut("{chave}")]
    public async Task<IActionResult> SalvarValor(string chave, [FromBody] string valor)
    {
        try
        {
            await _parametroService.SalvarValorAsync(chave, valor);
            return Ok(new { chave, valor });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar parâmetro.", error = ex.Message });
        }
    }
}

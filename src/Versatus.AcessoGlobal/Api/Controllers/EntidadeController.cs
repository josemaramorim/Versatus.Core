using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntidadeController : ControllerBase
{
    private readonly IEntidadeService _entidadeService;

    public EntidadeController(IEntidadeService entidadeService)
    {
        _entidadeService = entidadeService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var entidades = await _entidadeService.ListarUltimasAsync(50);
        return Ok(entidades);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var entidade = await _entidadeService.ObterPorIdAsync(id);

        if (entidade == null)
        {
            return NotFound();
        }

        return Ok(entidade);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarEntidadeDto dto)
    {
        try
        {
            var criada = await _entidadeService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.IdEntidade }, criada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar entidade.", error = ex.Message });
        }
    }
}

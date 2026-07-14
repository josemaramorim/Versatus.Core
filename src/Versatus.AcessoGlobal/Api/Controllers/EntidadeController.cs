using System;
using System.Threading.Tasks;
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

    [HttpGet("paginado")]
    public async Task<IActionResult> ListarPaginado(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string sortBy = "codigo",
        [FromQuery] string sortOrder = "asc",
        [FromQuery] string search = "",
        [FromQuery] string role = "")
    {
        var resultado = await _entidadeService.ListarPaginadoAsync(page, limit, sortBy, sortOrder, search, role);
        return Ok(resultado);
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

    [HttpGet("completo/{id}")]
    public async Task<IActionResult> ObterCompleto(int id)
    {
        var entidade = await _entidadeService.ObterCompletoPorIdAsync(id);

        if (entidade == null)
        {
            return NotFound();
        }

        return Ok(entidade);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] SalvarEntidadeDto dto)
    {
        try
        {
            var criada = await _entidadeService.SalvarCompletoAsync(dto);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarEntidadeDto dto)
    {
        try
        {
            var atualizada = await _entidadeService.AtualizarCompletoAsync(id, dto);
            return Ok(atualizada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar entidade.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _entidadeService.ExcluirAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao excluir entidade.", error = ex.Message });
        }
    }
}

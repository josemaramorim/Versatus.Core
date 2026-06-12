using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransportadoraController : ControllerBase
{
    private readonly ITransportadoraService _transportadoraService;

    public TransportadoraController(ITransportadoraService transportadoraService)
    {
        _transportadoraService = transportadoraService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var transportadoras = await _transportadoraService.ListarTodosAsync();
        return Ok(transportadoras);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var transportadora = await _transportadoraService.ObterPorIdAsync(id);
        if (transportadora == null)
        {
            return NotFound();
        }
        return Ok(transportadora);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTransportadoraDto dto)
    {
        try
        {
            var criado = await _transportadoraService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.IdTransportadora }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar transportadora.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarTransportadoraDto dto)
    {
        try
        {
            await _transportadoraService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar transportadora.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _transportadoraService.ExcluirAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao excluir transportadora.", error = ex.Message });
        }
    }
}

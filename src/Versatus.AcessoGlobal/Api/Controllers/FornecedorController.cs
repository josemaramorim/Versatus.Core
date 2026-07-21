using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FornecedorController : ControllerBase
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedorController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var fornecedores = await _fornecedorService.ListarTodosAsync();
        return Ok(fornecedores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var fornecedor = await _fornecedorService.ObterPorIdAsync(id);
        if (fornecedor == null)
        {
            return NotFound();
        }
        return Ok(fornecedor);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarFornecedorDto dto)
    {
        try
        {
            var criado = await _fornecedorService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.IdFornecedor }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar fornecedor.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarFornecedorDto dto)
    {
        try
        {
            await _fornecedorService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar fornecedor.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _fornecedorService.ExcluirAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao excluir fornecedor.", error = ex.Message });
        }
    }
}

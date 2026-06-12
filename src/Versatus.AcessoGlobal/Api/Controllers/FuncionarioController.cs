using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionarioController : ControllerBase
{
    private readonly IFuncionarioService _funcionarioService;

    public FuncionarioController(IFuncionarioService funcionarioService)
    {
        _funcionarioService = funcionarioService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var funcionarios = await _funcionarioService.ListarTodosAsync();
        return Ok(funcionarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var funcionario = await _funcionarioService.ObterPorIdAsync(id);
        if (funcionario == null)
        {
            return NotFound();
        }
        return Ok(funcionario);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarFuncionarioDto dto)
    {
        try
        {
            var criado = await _funcionarioService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.IdFuncionario }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar funcionário.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarFuncionarioDto dto)
    {
        try
        {
            await _funcionarioService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar funcionário.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _funcionarioService.ExcluirAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao excluir funcionário.", error = ex.Message });
        }
    }
}

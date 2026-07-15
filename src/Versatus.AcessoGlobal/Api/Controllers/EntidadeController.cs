using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.Framework.Validation;

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
        [FromQuery] string role = "",
        [FromQuery] int? tipoPessoa = null)
    {
        var resultado = await _entidadeService.ListarPaginadoAsync(page, limit, sortBy, sortOrder, search, role, tipoPessoa);
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
            var result = await _entidadeService.SalvarCompletoAsync(dto);
            if (!result.IsSuccess)
            {
                var firstMessage = result.Errors.Count > 0 ? result.Errors[0].Mensagem : "Erro de validação.";
                return BadRequest(new { message = firstMessage, errors = result.Errors });
            }
            var criada = result.Value!;
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.IdEntidade }, criada);
        }
        catch (Exception ex)
        {
            return TratarException(ex, "salvar");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarEntidadeDto dto)
    {
        try
        {
            var result = await _entidadeService.AtualizarCompletoAsync(id, dto);
            if (!result.IsSuccess)
            {
                var firstMessage = result.Errors.Count > 0 ? result.Errors[0].Mensagem : "Erro de validação.";
                return BadRequest(new { message = firstMessage, errors = result.Errors });
            }
            var atualizada = result.Value!;
            return Ok(atualizada);
        }
        catch (Exception ex)
        {
            return TratarException(ex, "atualizar");
        }
    }

    private IActionResult TratarException(Exception ex, string acao)
    {
        var msg = ex.Message;
        
        // Verificar se é erro do EF Core de rastreamento
        if (msg.Contains("tracked") || msg.Contains("same key value") || msg.Contains("attaching existing entities"))
        {
            return BadRequest(new
            {
                message = "Conflito de identificador interno no banco de dados. Por favor, tente novamente ou contate o suporte.",
                error = msg
            });
        }

        if (msg.Contains("foreign key") || msg.Contains("constraint") || msg.Contains("FK_"))
        {
            return BadRequest(new
            {
                message = "Erro de integridade de dados. O registro faz referência a outra informação que não existe ou está inválida no sistema.",
                error = msg
            });
        }

        if (msg.Contains("duplicate key") || msg.Contains("index") || msg.Contains("duplicado"))
        {
            return BadRequest(new
            {
                message = "Erro de duplicidade. Um registro com estas informações já está cadastrado.",
                error = msg
            });
        }

        return StatusCode(500, new 
        { 
            message = $"Erro interno ao {acao} entidade. Por favor, contate o suporte.", 
            error = msg 
        });
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

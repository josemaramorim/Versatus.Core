using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Services;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoPagamentoController : ControllerBase
{
    private readonly ICondicaoPagamentoService _condicaoPagamentoService;

    public CondicaoPagamentoController(ICondicaoPagamentoService condicaoPagamentoService)
    {
        _condicaoPagamentoService = condicaoPagamentoService;
    }

    [HttpGet("paginado")]
    public async Task<IActionResult> ListarPaginado(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string sortBy = "descricao",
        [FromQuery] string sortOrder = "asc",
        [FromQuery] string search = "",
        CancellationToken cancellationToken = default)
    {
        var resultado = await _condicaoPagamentoService.ListarPaginadoAsync(page, limit, sortBy, sortOrder, search, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        var cp = await _condicaoPagamentoService.ObterPorIdAsync(id, cancellationToken);
        if (cp == null)
        {
            return NotFound();
        }
        return Ok(cp);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _condicaoPagamentoService.CriarAsync(dto, cancellationToken);
            if (!result.IsSuccess)
            {
                var firstMessage = result.Errors.Count > 0 ? result.Errors[0].Mensagem : "Erro de validação.";
                return BadRequest(new { message = firstMessage, errors = result.Errors });
            }
            var criada = result.Value!;
            return CreatedAtAction(nameof(ObterPorId), new { id = criada.IdCondicaoPagamento }, criada);
        }
        catch (Exception ex)
        {
            return TratarException(ex, "salvar");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] EditarCondicaoPagamentoDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _condicaoPagamentoService.AtualizarAsync(id, dto, cancellationToken);
            if (!result.IsSuccess)
            {
                var firstMessage = result.Errors.Count > 0 ? result.Errors[0].Mensagem : "Erro de validação.";
                return BadRequest(new { message = firstMessage, errors = result.Errors });
            }
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            return TratarException(ex, "atualizar");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _condicaoPagamentoService.ExcluirAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao excluir condição de pagamento.", error = ex.Message });
        }
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> ListarGrupos(CancellationToken cancellationToken = default)
    {
        var grupos = await _condicaoPagamentoService.ListarGruposAsync(cancellationToken);
        return Ok(grupos);
    }

    [HttpGet("formas-cobranca")]
    public async Task<IActionResult> ListarFormasCobranca(CancellationToken cancellationToken = default)
    {
        var formas = await _condicaoPagamentoService.ListarFormasCobrancaAsync(cancellationToken);
        return Ok(formas);
    }

    private IActionResult TratarException(Exception ex, string acao)
    {
        var msg = ex.Message;
        
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
            message = $"Erro interno ao {acao} condição de pagamento. Por favor, contate o suporte.", 
            error = msg 
        });
    }
}

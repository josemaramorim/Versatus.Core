using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Domain.DTOs;

namespace Versatus.AcessoGlobal.Api.Controllers;

/// <summary>
/// Adaptador de entrega HTTP fino para Parâmetros de Configuração.
/// Respeita o desacoplamento estrito de controladores (REGRA 17).
/// </summary>
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

    [HttpGet("paginado")]
    public async Task<IActionResult> ListarPaginado(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string sortBy = "chave",
        [FromQuery] string sortOrder = "asc",
        [FromQuery] string search = "")
    {
        var resultado = await _parametroService.ListarPaginadoAsync(page, limit, sortBy, sortOrder, search);
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var dto = await _parametroService.ObterPorIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpGet("completo/{id:int}")]
    public async Task<IActionResult> ObterCompleto(int id)
    {
        var dto = await _parametroService.ObterPorIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
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

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] SalvarParametroDto dto)
    {
        try
        {
            var criado = await _parametroService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao criar parâmetro.", error = ex.Message });
        }
    }

    [HttpPut("completo/{id:int}")]
    public async Task<IActionResult> AtualizarCompleto(int id, [FromBody] SalvarParametroDto dto)
    {
        try
        {
            await _parametroService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar parâmetro.", error = ex.Message });
        }
    }

    [HttpPut("editar/{id:int}")]
    public async Task<IActionResult> EditarCompleto(int id, [FromBody] SalvarParametroDto dto)
    {
        try
        {
            await _parametroService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar parâmetro.", error = ex.Message });
        }
    }

    [HttpPut("salvar/{id:int}")]
    public async Task<IActionResult> SalvarCompleto(int id, [FromBody] SalvarParametroDto dto)
    {
        try
        {
            await _parametroService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar parâmetro.", error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] SalvarParametroDto dto)
    {
        try
        {
            await _parametroService.AtualizarAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao atualizar parâmetro.", error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _parametroService.ExcluirAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao excluir parâmetro.", error = ex.Message });
        }
    }

    [HttpGet("escopo")]
    public async Task<IActionResult> ListarPorEscopo(
        [FromQuery] int tipoParametro,
        [FromQuery] int? idPerfil = null)
    {
        try
        {
            var resultado = await _parametroService.ListarPorEscopoAsync(tipoParametro, idPerfil);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao buscar parâmetros por escopo.", error = ex.Message });
        }
    }

    [HttpPut("salvar-valores")]
    public async Task<IActionResult> SalvarValoresLote([FromBody] SalvarValoresParametrosDto dto)
    {
        try
        {
            await _parametroService.SalvarValoresLoteAsync(dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao salvar valores dos parâmetros.", error = ex.Message });
        }
    }

    [HttpGet("perfis")]
    public async Task<IActionResult> ListarPerfis()
    {
        try
        {
            var perfis = await _parametroService.ListarPerfisAsync();
            return Ok(perfis);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao buscar perfis.", error = ex.Message });
        }
    }

    [HttpGet("enum-opcoes")]
    public async Task<IActionResult> ObterOpcoesEnum([FromQuery] string enumNome)
    {
        try
        {
            var resultado = await _parametroService.ObterOpcoesEnumAsync(enumNome);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar opções de enumeração.", error = ex.Message });
        }
    }
}

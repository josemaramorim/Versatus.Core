using Microsoft.AspNetCore.Mvc;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Services;

namespace Versatus.GestaoFinanceira.Api.Controllers;

// Contrato: specs/modulos/MOD-05/contracts/caixa-banco.md §Cobrador (E3-T06) — CRUD Padrão.
[ApiController]
[Route("api/financeiro/cobrador")]
public class CobradorController(ICobradorService service) : ControllerBase
{
    [HttpGet("paginado")]
    public async Task<IActionResult> ListarPaginado([FromQuery] FiltroCobradorDto filtro, CancellationToken cancellationToken)
        => Ok(await service.ListarPaginadoAsync(filtro, cancellationToken));

    [HttpGet("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> ObterPorId(int idFilial, int id, CancellationToken cancellationToken)
        => await service.ObterPorIdAsync(id, idFilial, cancellationToken) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] SalvarCobradorDto dto, CancellationToken cancellationToken)
    {
        var resultado = await service.CriarAsync(dto, cancellationToken);
        if (!resultado.IsSuccess)
            return Falha(resultado.Errors);

        var criado = resultado.Value!;
        return CreatedAtAction(nameof(ObterPorId), new { idFilial = criado.IdFilial, id = criado.IdCobrador }, criado);
    }

    [HttpPut("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> Atualizar(int idFilial, int id, [FromBody] SalvarCobradorDto dto, CancellationToken cancellationToken)
    {
        if (await service.ObterPorIdAsync(id, idFilial, cancellationToken) is null)
            return NotFound();

        var resultado = await service.AtualizarAsync(id, idFilial, dto, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : Falha(resultado.Errors);
    }

    [HttpDelete("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> Excluir(int idFilial, int id, CancellationToken cancellationToken)
    {
        if (await service.ObterPorIdAsync(id, idFilial, cancellationToken) is null)
            return NotFound();

        var resultado = await service.ExcluirAsync(id, idFilial, cancellationToken);
        return resultado.IsValid ? NoContent() : Falha(resultado.Errors);
    }

    private BadRequestObjectResult Falha(IReadOnlyList<ValidationError> erros)
        => BadRequest(new { message = erros.Count > 0 ? erros[0].Mensagem : "Erro de validação.", errors = erros });
}

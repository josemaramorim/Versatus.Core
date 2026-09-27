using Microsoft.AspNetCore.Mvc;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Services;

namespace Versatus.GestaoFinanceira.Api.Controllers;

// Contrato: specs/modulos/MOD-05/contracts/caixa-banco.md §ContaBancaria (E3-T06).
// id = IdCaixaBanco (PK compartilhada 1:1). Criação/exclusão acompanham o CaixaBanco.
[ApiController]
[Route("api/financeiro/conta-bancaria")]
public class ContaBancariaController(IContaBancariaService service) : ControllerBase
{
    [HttpGet("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> ObterPorId(int idFilial, int id, CancellationToken cancellationToken)
        => await service.ObterPorIdAsync(id, idFilial, cancellationToken) is { } dto ? Ok(dto) : NotFound();

    [HttpPut("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> Atualizar(int idFilial, int id, [FromBody] AtualizarContaBancariaDto dto,
        CancellationToken cancellationToken)
    {
        if (await service.ObterPorIdAsync(id, idFilial, cancellationToken) is null)
            return NotFound();

        var resultado = await service.AtualizarAsync(id, idFilial, dto, cancellationToken);
        if (resultado.IsSuccess)
            return Ok(resultado.Value);

        var erros = resultado.Errors;
        return BadRequest(new { message = erros.Count > 0 ? erros[0].Mensagem : "Erro de validação.", errors = erros });
    }
}

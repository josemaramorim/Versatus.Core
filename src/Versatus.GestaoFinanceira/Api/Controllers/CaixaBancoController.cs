using Microsoft.AspNetCore.Mvc;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Services;

namespace Versatus.GestaoFinanceira.Api.Controllers;

// Contrato: specs/modulos/MOD-05/contracts/caixa-banco.md §CaixaBanco (E3-T06).
// Adaptador HTTP fino (Artigo IV / Regra 17): só ICaixaBancoService; Result → 400.
[ApiController]
[Route("api/financeiro/caixa-banco")]
public class CaixaBancoController(ICaixaBancoService service, ISaldoCalculadora saldos) : ControllerBase
{
    [HttpGet("paginado")]
    public async Task<IActionResult> ListarPaginado([FromQuery] FiltroCaixaBancoDto filtro, CancellationToken cancellationToken)
        => Ok(await service.ListarPaginadoAsync(filtro, cancellationToken));

    [HttpGet("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> ObterPorId(int idFilial, int id, CancellationToken cancellationToken)
        => await service.ObterPorIdAsync(id, idFilial, cancellationToken) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCaixaBancoDto dto, CancellationToken cancellationToken)
    {
        var resultado = await service.CriarAsync(dto, cancellationToken);
        if (!resultado.IsSuccess)
            return Falha(resultado.Errors);

        var criado = resultado.Value!;
        return CreatedAtAction(nameof(ObterPorId), new { idFilial = criado.IdFilial, id = criado.IdCaixaBanco }, criado);
    }

    [HttpPut("{idFilial:int}/{id:int}")]
    public async Task<IActionResult> Atualizar(int idFilial, int id, [FromBody] AtualizarCaixaBancoDto dto,
        CancellationToken cancellationToken)
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

    [HttpGet("{idFilial:int}/{id:int}/usuarios")]
    public async Task<IActionResult> ListarUsuarios(int idFilial, int id, CancellationToken cancellationToken)
        => Ok(await service.ListarUsuariosAsync(id, idFilial, cancellationToken));

    [HttpPut("{idFilial:int}/{id:int}/usuarios")]
    public async Task<IActionResult> SalvarUsuarios(int idFilial, int id, [FromBody] SalvarCaixaBancoUsuariosDto dto,
        CancellationToken cancellationToken)
    {
        var resultado = await service.SalvarUsuariosAsync(id, idFilial, dto, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : Falha(resultado.Errors);
    }

    /// <summary>Saldo do caixa na data (OP-E3-08 — E3-T09); sem <c>data</c>, o último saldo.</summary>
    [HttpGet("{idFilial:int}/{id:int}/saldo")]
    public async Task<IActionResult> ObterSaldo(int idFilial, int id, [FromQuery] DateTime? data, CancellationToken cancellationToken)
        => await saldos.ObterSaldoAsync(id, idFilial, data, cancellationToken) is { } dto ? Ok(dto) : NotFound();

    private BadRequestObjectResult Falha(IReadOnlyList<ValidationError> erros)
        => BadRequest(new { message = erros.Count > 0 ? erros[0].Mensagem : "Erro de validação.", errors = erros });
}

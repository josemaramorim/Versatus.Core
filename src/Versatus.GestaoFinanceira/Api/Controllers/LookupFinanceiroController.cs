using Microsoft.AspNetCore.Mvc;
using Versatus.GestaoFinanceira.Domain.Services;

namespace Versatus.GestaoFinanceira.Api.Controllers;

// Listas de consulta da tela Caixa/Banco (E3-T07 — DÚVIDA-CB5). Adaptador HTTP fino (Artigo IV).
[ApiController]
[Route("api/financeiro/lookups")]
public class LookupFinanceiroController(ILookupFinanceiroService service) : ControllerBase
{
    [HttpGet("agencias")]
    public async Task<IActionResult> Agencias([FromQuery] string? texto, CancellationToken cancellationToken)
        => Ok(await service.ListarAgenciasAsync(texto, cancellationToken));

    [HttpGet("usuarios")]
    public async Task<IActionResult> Usuarios([FromQuery] string? texto, CancellationToken cancellationToken)
        => Ok(await service.ListarUsuariosAsync(texto, cancellationToken));

    [HttpGet("instituicoes-financeiras")]
    public async Task<IActionResult> InstituicoesFinanceiras([FromQuery] string? texto, CancellationToken cancellationToken)
        => Ok(await service.ListarInstituicoesFinanceirasAsync(texto, cancellationToken));

    [HttpGet("planos-contabeis")]
    public async Task<IActionResult> PlanosContabeis([FromQuery] string? texto, CancellationToken cancellationToken)
        => Ok(await service.ListarPlanosContabeisAsync(texto, cancellationToken));

    [HttpGet("contas-correntes")]
    public async Task<IActionResult> ContasCorrentes([FromQuery] string? texto, CancellationToken cancellationToken)
        => Ok(await service.ListarContasCorrentesAsync(texto, cancellationToken));
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinanceiroController : ControllerBase
{
    private readonly IFinanceiroService _financeiroService;

    public FinanceiroController(IFinanceiroService financeiroService)
    {
        _financeiroService = financeiroService;
    }

    [HttpGet("bancos")]
    public async Task<IActionResult> ListarBancos()
    {
        var bancos = await _financeiroService.ListarBancosAsync();
        return Ok(bancos);
    }

    [HttpGet("formas-pagamento")]
    public async Task<IActionResult> ListarFormasPagamento()
    {
        var formas = await _financeiroService.ListarFormasPagamentoAsync();
        return Ok(formas);
    }
}

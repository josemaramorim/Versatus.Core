using Microsoft.AspNetCore.Mvc;
using Versatus.GestaoTributo.Domain.Services;

namespace Versatus.GestaoTributo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TributacaoController : ControllerBase
{
    private readonly ITributoService _tributoService;

    public TributacaoController(ITributoService tributoService)
    {
        _tributoService = tributoService;
    }

    [HttpGet("classificacoes")]
    public async Task<IActionResult> ListarClassificacoes()
    {
        var classf = await _tributoService.ListarClassificacoesAsync(50);
        return Ok(classf);
    }

    [HttpGet("cfops")]
    public async Task<IActionResult> ListarCfops()
    {
        var cfops = await _tributoService.ListarCfopsAsync(50);
        return Ok(cfops);
    }

    [HttpGet("simples-nacional")]
    public async Task<IActionResult> ListarSimplesNacional()
    {
        var simples = await _tributoService.ListarSimplesNacionalComTributosAsync();
        return Ok(simples);
    }

    [HttpGet("regras/{id}")]
    public async Task<IActionResult> ObterRegraConfiguracao(int id)
    {
        var config = await _tributoService.ObterRegraConfiguracaoComDetalhesAsync(id);

        if (config == null)
        {
            return NotFound();
        }

        return Ok(config);
    }
}

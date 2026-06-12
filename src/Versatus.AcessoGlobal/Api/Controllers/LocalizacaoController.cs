using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocalizacaoController : ControllerBase
{
    private readonly ILocalizacaoService _localizacaoService;

    public LocalizacaoController(ILocalizacaoService localizacaoService)
    {
        _localizacaoService = localizacaoService;
    }

    [HttpGet("paises")]
    public async Task<IActionResult> ListarPaises()
    {
        var paises = await _localizacaoService.ListarPaisesAsync();
        return Ok(paises);
    }

    [HttpGet("estados")]
    public async Task<IActionResult> ListarEstados([FromQuery] int? idPais)
    {
        var estados = await _localizacaoService.ListarEstadosAsync(idPais);
        return Ok(estados);
    }

    [HttpGet("cidades")]
    public async Task<IActionResult> ListarCidades([FromQuery] int? idEstado)
    {
        var cidades = await _localizacaoService.ListarCidadesAsync(idEstado);
        return Ok(cidades);
    }

    [HttpGet("bairros")]
    public async Task<IActionResult> ListarBairros([FromQuery] int? idCidade)
    {
        var bairros = await _localizacaoService.ListarBairrosAsync(idCidade);
        return Ok(bairros);
    }
}

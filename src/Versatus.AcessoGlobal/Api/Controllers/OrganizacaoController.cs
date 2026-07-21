using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Versatus.AcessoGlobal.Domain.Services;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizacaoController : ControllerBase
{
    private readonly IOrganizacaoService _organizacaoService;

    public OrganizacaoController(IOrganizacaoService organizacaoService)
    {
        _organizacaoService = organizacaoService;
    }

    [HttpGet("grupos")]
    public async Task<IActionResult> ListarGrupos()
    {
        var grupos = await _organizacaoService.ListarGruposAsync();
        return Ok(grupos);
    }

    [HttpGet("empresas")]
    public async Task<IActionResult> ListarEmpresas()
    {
        var empresas = await _organizacaoService.ListarEmpresasAsync();
        return Ok(empresas);
    }

    [HttpGet("filiais")]
    public async Task<IActionResult> ListarFiliais()
    {
        var filiais = await _organizacaoService.ListarFiliaisAsync();
        return Ok(filiais);
    }
}

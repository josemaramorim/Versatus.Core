using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Infrastructure;

namespace Versatus.AcessoGlobal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoEnumeradoController : ControllerBase
{
    private readonly AcessoGlobalDbContext _context;

    public TipoEnumeradoController(AcessoGlobalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna a lista de itens enumerados pertencentes a um determinado pai.
    /// Ex: /api/tipoenumerado/1 (Física/Jurídica)
    /// </summary>
    [HttpGet("{idPai}")]
    public async Task<IActionResult> ObterPorPai(int idPai)
    {
        try
        {
            var itens = await _context.TiposEnumerados
                .Where(t => t.IdTipoEnumeradoPai == idPai)
                .OrderBy(t => t.Ordem)
                .Select(t => new
                {
                    value = t.IdTipoEnumerado,
                    label = t.Descricao
                })
                .ToListAsync();

            return Ok(itens);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar tipos enumerados.", details = ex.Message });
        }
    }
}

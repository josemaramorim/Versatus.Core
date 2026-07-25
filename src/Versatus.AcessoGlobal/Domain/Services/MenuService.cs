using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.Framework.Validation;

namespace Versatus.AcessoGlobal.Domain.Services;

public class MenuService : IMenuService
{
    private readonly AcessoGlobalDbContext _context;

    public MenuService(AcessoGlobalDbContext context)
    {
        _context = context;
    }

    public async Task<List<ModuloMenuDto>> ObterArvoreAsync(CancellationToken cancellationToken = default)
    {
        var modulos = await _context.Modulos.OrderBy(m => m.Ordem).ToListAsync(cancellationToken);
        var menus = await _context.Menus.ToListAsync(cancellationToken);
        var menuModulos = await _context.MenuModulos.ToListAsync(cancellationToken);
        var menuMenus = await _context.MenuMenus.ToListAsync(cancellationToken);
        var menuRotinas = await _context.MenuRotinas.ToListAsync(cancellationToken);
        var rotinas = await _context.Rotinas.ToListAsync(cancellationToken);

        var dictMenus = menus.ToDictionary(m => m.IdMenu);
        var dictRotinas = rotinas.ToDictionary(r => r.IdRotina);

        var resultado = new List<ModuloMenuDto>();

        foreach (var modulo in modulos)
        {
            var rootMenuLinks = menuModulos.Where(mm => mm.IdModulo == modulo.IdModulo)
                                           .OrderBy(mm => mm.Ordem)
                                           .ToList();

            var menuDtos = new List<MenuItemDto>();
            foreach (var link in rootMenuLinks)
            {
                var menuItem = MontarItemMenu(link.IdMenu, modulo, dictMenus, dictRotinas, menuMenus, menuRotinas);
                if (menuItem != null)
                {
                    menuDtos.Add(menuItem);
                }
            }

            resultado.Add(new ModuloMenuDto(
                modulo.IdModulo,
                modulo.Nome,
                modulo.PrefixoRota,
                modulo.IconeMui,
                modulo.CorHex,
                modulo.Ordem,
                menuDtos
            ));
        }

        return resultado;
    }

    public async Task<List<FavoritoDto>> ObterFavoritosAsync(int idUsuario = 1, CancellationToken cancellationToken = default)
    {
        var favoritos = await _context.Favoritos
            .Where(f => f.IdUsuario == idUsuario)
            .OrderBy(f => f.Ordem)
            .ToListAsync(cancellationToken);

        if (!favoritos.Any())
            return new List<FavoritoDto>();

        var arvore = await ObterArvoreAsync(cancellationToken);
        var rotinaMap = MapearRotinasDaArvore(arvore);

        var resultado = new List<FavoritoDto>();
        foreach (var fav in favoritos)
        {
            if (rotinaMap.TryGetValue(fav.IdRotina, out var info))
            {
                resultado.Add(new FavoritoDto(
                    fav.IdFavorito,
                    fav.IdRotina,
                    info.NomeRotina,
                    info.RotaCompleta,
                    info.NomeModulo,
                    info.CorHex,
                    info.CaminhoCompleto
                ));
            }
            else
            {
                // Fallback se a rotina não estiver vinculada em nenhum menu da árvore
                var rotina = await _context.Rotinas.FirstOrDefaultAsync(r => r.IdRotina == fav.IdRotina, cancellationToken);
                if (rotina != null)
                {
                    resultado.Add(new FavoritoDto(
                        fav.IdFavorito,
                        fav.IdRotina,
                        rotina.Nome,
                        FormatRota("/acesso-global", rotina.Objeto),
                        "Sistema",
                        "#637381",
                        $"Sistema → {rotina.Nome}"
                    ));
                }
            }
        }

        return resultado;
    }

    public async Task<Result<FavoritoDto>> AdicionarFavoritoAsync(int idRotina, int idUsuario = 1, CancellationToken cancellationToken = default)
    {
        var existe = await _context.Favoritos
            .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario && f.IdRotina == idRotina, cancellationToken);

        if (existe == null)
        {
            var maxOrdem = await _context.Favoritos
                .Where(f => f.IdUsuario == idUsuario)
                .Select(f => (int?)f.Ordem)
                .MaxAsync(cancellationToken) ?? 0;

            existe = new GloFavorito
            {
                IdUsuario = idUsuario,
                IdRotina = idRotina,
                Ordem = maxOrdem + 1
            };

            _context.Favoritos.Add(existe);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var lista = await ObterFavoritosAsync(idUsuario, cancellationToken);
        var dto = lista.FirstOrDefault(f => f.IdRotina == idRotina);

        if (dto != null)
        {
            return Result<FavoritoDto>.Ok(dto);
        }

        return Result<FavoritoDto>.Fail(new ValidationError("IdRotina", "Erro ao processar favorito."));
    }

    public async Task<Result<bool>> RemoverFavoritoAsync(int idRotina, int idUsuario = 1, CancellationToken cancellationToken = default)
    {
        var favorito = await _context.Favoritos
            .FirstOrDefaultAsync(f => f.IdUsuario == idUsuario && f.IdRotina == idRotina, cancellationToken);

        if (favorito == null)
        {
            return Result<bool>.Fail(new ValidationError("IdRotina", "Favorito não encontrado."));
        }

        _context.Favoritos.Remove(favorito);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }

    private MenuItemDto? MontarItemMenu(
        int idMenu,
        GloModulo modulo,
        Dictionary<int, GloMenu> dictMenus,
        Dictionary<int, GloRotina> dictRotinas,
        List<GloMenuMenu> menuMenus,
        List<GloMenuRotina> menuRotinas)
    {
        if (!dictMenus.TryGetValue(idMenu, out var menu))
            return null;

        var subMenuLinks = menuMenus.Where(mm => mm.IdMenuPai == idMenu)
                                    .OrderBy(mm => mm.Ordem)
                                    .ToList();

        var subMenuDtos = new List<MenuItemDto>();
        foreach (var subLink in subMenuLinks)
        {
            var subItem = MontarItemMenu(subLink.IdMenu, modulo, dictMenus, dictRotinas, menuMenus, menuRotinas);
            if (subItem != null)
            {
                subMenuDtos.Add(subItem);
            }
        }

        var rotinaLinks = menuRotinas.Where(mr => mr.IdMenu == idMenu)
                                     .OrderBy(mr => mr.Ordem)
                                     .ToList();

        var rotinaDtos = new List<RotinaItemDto>();
        foreach (var mr in rotinaLinks)
        {
            if (dictRotinas.TryGetValue(mr.IdRotina, out var rotina))
            {
                var rotaCompleta = FormatRota(modulo.PrefixoRota, rotina.Objeto);
                rotinaDtos.Add(new RotinaItemDto(rotina.IdRotina, rotina.Nome, rotina.Objeto, mr.Ordem, rotaCompleta));
            }
        }

        return new MenuItemDto(menu.IdMenu, menu.Descricao, menu.Ordem, subMenuDtos, rotinaDtos);
    }

    private record RotinaMapInfo(string NomeRotina, string RotaCompleta, string NomeModulo, string? CorHex, string CaminhoCompleto);

    private Dictionary<int, RotinaMapInfo> MapearRotinasDaArvore(List<ModuloMenuDto> arvore)
    {
        var map = new Dictionary<int, RotinaMapInfo>();

        foreach (var modulo in arvore)
        {
            foreach (var menu in modulo.Menus)
            {
                ProcessarMenuParaMap(modulo, menu, modulo.Nome + " → " + menu.Descricao, map);
            }
        }

        return map;
    }

    private void ProcessarMenuParaMap(ModuloMenuDto modulo, MenuItemDto menu, string caminhoAtual, Dictionary<int, RotinaMapInfo> map)
    {
        foreach (var rotina in menu.Rotinas)
        {
            if (!map.ContainsKey(rotina.IdRotina))
            {
                map[rotina.IdRotina] = new RotinaMapInfo(
                    rotina.Nome,
                    rotina.RotaCompleta,
                    modulo.Nome,
                    modulo.CorHex,
                    $"{caminhoAtual} → {rotina.Nome}"
                );
            }
        }

        foreach (var sub in menu.SubMenus)
        {
            ProcessarMenuParaMap(modulo, sub, $"{caminhoAtual} → {sub.Descricao}", map);
        }
    }

    private static string FormatRota(string? prefixo, string? objeto)
    {
        if (string.IsNullOrWhiteSpace(objeto))
            return prefixo ?? string.Empty;

        var cleanPrefix = (prefixo ?? string.Empty).TrimEnd('/');
        var cleanObjeto = objeto.TrimStart('/');
        return $"{cleanPrefix}/{cleanObjeto}";
    }
}

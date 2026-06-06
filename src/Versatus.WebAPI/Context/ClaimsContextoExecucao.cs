using System.Security.Claims;
using Versatus.Framework.Context;

namespace Versatus.WebAPI.Context;

public class ClaimsContextoExecucao : IContextoExecucao
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private int _idFilial = 1;
    private int _idUsuario = 1;
    private int _idEmpresa = 1;
    private string[] _perfis = ["Admin"];

    public ClaimsContextoExecucao(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int IdUsuario
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("IdUsuario");
                if (claim != null && int.TryParse(claim.Value, out var id))
                {
                    return id;
                }
            }
            return _idUsuario;
        }
        set => _idUsuario = value;
    }

    public int IdFilial
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var claim = user.FindFirst("IdFilial");
                if (claim != null && int.TryParse(claim.Value, out var id))
                {
                    return id;
                }
            }
            return _idFilial;
        }
        set => _idFilial = value;
    }

    public int IdEmpresa
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var claim = user.FindFirst("IdEmpresa");
                if (claim != null && int.TryParse(claim.Value, out var id))
                {
                    return id;
                }
            }
            return _idEmpresa;
        }
        set => _idEmpresa = value;
    }

    public string[] Perfis
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
                if (roles.Length > 0)
                {
                    return roles;
                }
            }
            return _perfis;
        }
        set => _perfis = value;
    }
}

using System.Linq;
using System.Security.Claims;

namespace Versatus.Framework.Context;

/// <summary>
/// Extensões para facilitar a inicialização do contexto de execução a partir do ClaimsPrincipal.
/// </summary>
public static class ContextoExecucaoExtensions
{
    private const string ClaimIdUsuario = "IdUsuario";
    private const string ClaimIdEmpresa = "IdEmpresa";
    private const string ClaimIdFilial = "IdFilial";

    /// <summary>
    /// Preenche um objeto IContextoExecucao a partir de um ClaimsPrincipal (usuário logado).
    /// </summary>
    public static void Fill(this ClaimsPrincipal principal, IContextoExecucao contexto)
    {
        if (principal == null) return;

        if (int.TryParse(principal.FindFirst(ClaimIdUsuario)?.Value, out var idUsuario))
            contexto.IdUsuario = idUsuario;

        if (int.TryParse(principal.FindFirst(ClaimIdEmpresa)?.Value, out var idEmpresa))
            contexto.IdEmpresa = idEmpresa;

        if (int.TryParse(principal.FindFirst(ClaimIdFilial)?.Value, out var idFilial))
            contexto.IdFilial = idFilial;

        contexto.Perfis = principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
    }
}

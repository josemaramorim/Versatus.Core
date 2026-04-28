using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Versatus.Framework.Context;

namespace Versatus.Framework.Web;

/// <summary>
/// Middleware para interceptar a requisição e popular o Contexto de Execução
/// a partir das Claims do usuário autenticado.
/// </summary>
public class ContextMiddleware
{
    private readonly RequestDelegate _next;

    public ContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IContextoExecucao contextoExecucao)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.User.Fill(contextoExecucao);
        }

        await _next(context);
    }
}

public static class ContextMiddlewareExtensions
{
    public static IApplicationBuilder UseContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContextMiddleware>();
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Net;

namespace Versatus.WebAPI.Middleware
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _configuredApiKey;

        public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuredApiKey = configuration["Security:StranglerApiKey"] ?? string.Empty;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Bypass para chamadas locais ou documentação do Swagger
            var remoteIp = context.Connection.RemoteIpAddress;
            bool bypassAuth = false;

            if (remoteIp != null && IPAddress.IsLoopback(remoteIp))
            {
                bypassAuth = true;
            }

            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                bypassAuth = true;
            }

            // 2. Verifica se o cabeçalho X-Api-Key contém a chave configurada
            if (!bypassAuth)
            {
                if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey) ||
                    !string.Equals(_configuredApiKey, extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("Acesso não autorizado via API Key.");
                    return;
                }
            }

            // 3. Estabelece a identidade de execução para a integração
            var claims = new[] {
                new Claim(ClaimTypes.Name, "UsuarioStrangler"),
                new Claim(ClaimTypes.Role, "SystemIntegration")
            };
            var identity = new ClaimsIdentity(claims, "ApiKey");
            context.User = new ClaimsPrincipal(identity);

            await _next(context);
        }
    }
}

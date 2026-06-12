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
            // 1. Bypass para chamadas originárias do próprio servidor local (localhost/loopback)
            var remoteIp = context.Connection.RemoteIpAddress;
            bool isLocal = false;

            if (remoteIp != null)
            {
                if (IPAddress.IsLoopback(remoteIp))
                {
                    isLocal = true;
                }
            }

            // 2. Verifica se o cabeçalho X-Api-Key contém a chave configurada
            if (!isLocal)
            {
                if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey) ||
                    !string.Equals(_configuredApiKey, extractedApiKey))
                {
                    context.Response.StatusCode = 401;
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

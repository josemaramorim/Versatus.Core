# Prompt de Execução — MOD-08: Integração Segura via API Key (Strangler)


> **Para a IA executora:** Sua tarefa é configurar o canal de comunicação segura entre o legado e a nova API do .NET 8.
> Você implementará a autenticação via cabeçalho `X-Api-Key` com bypass para conexões de `localhost`.
> Todo o código de infraestrutura no legado deve permanecer dentro do projeto `Servidor.Strangler`.

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**

### Antes de começar:
```powershell
git checkout develop
git pull origin develop
git checkout -b feat/seguranca-integracao-strangler
```

### Ao finalizar a alteração:
```powershell
git add projeto_tag_1906/servidor/framework/Servidor.Strangler/Common/FinancialStranglerHelper.cs
git add src/Versatus.WebAPI/
git commit -m "feat(seguranca): implementa autenticacao via API Key com bypass de localhost"
# NÃO faça merge — deixe a branch para o usuário revisar
```

---

## Contexto

Estamos aplicando o **Padrão de Estrangulamento** (Strangler Fig Pattern). Para garantir que a nova API .NET 8 não fique vulnerável, as requisições enviadas pelo legado devem conter uma credencial de segurança. 
Optamos por uma chave secreta compartilhada (**API Key**) passada via cabeçalho HTTP (`X-Api-Key`). Para simplificar testes locais de desenvolvedores, conexões originadas de `localhost` (loopback) serão autorizadas automaticamente (Bypass).

---

## Arquivos a Modificar / Criar

### [MODIFICAR no legado]
1. [FinancialStranglerHelper.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Common/FinancialStranglerHelper.cs)

### [CRIAR / MODIFICAR no .NET 8]
2. [appsettings.json](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.WebAPI/appsettings.json)
3. [ApiKeyAuthMiddleware.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.WebAPI/Middleware/ApiKeyAuthMiddleware.cs) **[NEW]**
4. [Program.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.WebAPI/Program.cs)

---

## Roteiro de Implementação

### Passo 1: Modificar `FinancialStranglerHelper.cs` no Legado
Abra o arquivo [FinancialStranglerHelper.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/projeto_tag_1906/servidor/framework/Servidor.Strangler/Common/FinancialStranglerHelper.cs) e altere a classe para ler a API Key e adicioná-la aos cabeçalhos das requisições HTTP:

1. Adicione a propriedade privada para carregar a API Key a partir do arquivo de configuração:
   ```csharp
   private static string _stranglerApiKey = null;

   public static string StranglerApiKey
   {
       get
       {
           if (_stranglerApiKey == null)
           {
               try
               {
                   _stranglerApiKey = System.Configuration.ConfigurationManager.AppSettings["Net8StranglerApiKey"];
               }
               catch
               {
                   // Ignora erro fora de ambiente de aplicação ativa
               }

               if (string.IsNullOrEmpty(_stranglerApiKey))
               {
                   _stranglerApiKey = "";
               }
           }
           return _stranglerApiKey;
       }
   }
   ```

2. Crie um método auxiliar interno para configurar os cabeçalhos das instâncias do `WebClient`:
   ```csharp
   private static void ConfigurarClient(WebClient client)
   {
       client.Encoding = Encoding.UTF8;
       string key = StranglerApiKey;
       if (!string.IsNullOrEmpty(key))
       {
           client.Headers["X-Api-Key"] = key;
       }
   }
   ```

3. Modifique os métodos de envio para chamar `ConfigurarClient(client)`:
   * **GetJson:**
     ```csharp
     public static T GetJson<T>(string relativeUrl)
     {
         using (var client = new WebClient())
         {
             ConfigurarClient(client);
             string fullUrl = ApiBaseUrl + relativeUrl;
             string json = client.DownloadString(fullUrl);
             return JsonConvert.DeserializeObject<T>(json);
         }
     }
     ```
   * **PutJson:**
     ```csharp
     public static void PutJson<T>(string relativeUrl, T body)
     {
         using (var client = new WebClient())
         {
             ConfigurarClient(client);
             client.Headers[HttpRequestHeader.ContentType] = "application/json";
             string fullUrl = ApiBaseUrl + relativeUrl;
             string jsonBody = JsonConvert.SerializeObject(body);
             client.UploadString(fullUrl, "PUT", jsonBody);
         }
     }
     ```
   * **PostJson:**
     ```csharp
     public static TResponse PostJson<TRequest, TResponse>(string relativeUrl, TRequest body)
     {
         using (var client = new WebClient())
         {
             ConfigurarClient(client);
             client.Headers[HttpRequestHeader.ContentType] = "application/json";
             string fullUrl = ApiBaseUrl + relativeUrl;
             string jsonBody = JsonConvert.SerializeObject(body);
             string responseJson = client.UploadString(fullUrl, "POST", jsonBody);
             return JsonConvert.DeserializeObject<TResponse>(responseJson);
         }
     }
     ```

### Passo 2: Configurar a Chave Secreta no `.NET 8`
No arquivo [appsettings.json](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.WebAPI/appsettings.json) do projeto `Versatus.WebAPI`, adicione a chave de configuração sob o nó `Security`:

```json
  "Security": {
    "StranglerApiKey": "SUA_CHAVE_SUPER_SECRETA_DE_INTEGRACAO_2026"
  }
```

### Passo 3: Criar o Middleware de Autenticação no `.NET 8`
Crie o arquivo [ApiKeyAuthMiddleware.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.WebAPI/Middleware/ApiKeyAuthMiddleware.cs) com o seguinte código para validação do cabeçalho customizado:

```csharp
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
            _configuredApiKey = configuration["Security:StranglerApiKey"];
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
```

### Passo 4: Registrar o Middleware no Pipeline HTTP do `.NET 8`
Abra o arquivo [Program.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.WebAPI/Program.cs) da API.

1. Adicione a diretiva `using` no topo:
   ```csharp
   using Versatus.WebAPI.Middleware;
   ```
2. Registre o middleware antes de `app.UseAuthorization()`:
   ```csharp
   app.UseAuthentication();
   
   // Registrar o Middleware de Api Key customizado
   app.UseMiddleware<ApiKeyAuthMiddleware>();
   
   app.UseAuthorization();
   ```

---

## Verificação e Testes

Ao finalizar a implementação:
1. Verifique se o projeto `Versatus.WebAPI` compila sem erros (`dotnet build`).
2. Execute requisições para a API a partir de um IP externo (não local) e garanta que retorne `401 Unauthorized` se o cabeçalho `X-Api-Key` estiver ausente ou incorreto.
3. Garanta que as chamadas originadas a partir do legado Versatus incluam a chave correta e que sejam aceitas pela API .NET 8.
4. Submeta a branch para revisão do usuário.

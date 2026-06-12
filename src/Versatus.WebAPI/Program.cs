using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Versatus.AcessoGlobal.DependencyInjection;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.GestaoTributo.DependencyInjection;
using Versatus.GestaoTributo.Infrastructure;
using Versatus.WebAPI.Context;
using Versatus.WebAPI.Services;
using Versatus.WebAPI.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Connection String
const string connectionString = "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;";

// DbContexts
builder.Services.AddDbContext<AcessoGlobalDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<TributoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar dependências de negócio
builder.Services.AddAcessoGlobal();
builder.Services.AddGestaoTributo();

// Suporte a HttpContext e Contexto de Execução com Claims
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IContextoExecucao, ClaimsContextoExecucao>();
builder.Services.AddSingleton<IGeradorSequencial, FakeGeradorSequencial>();

// Autenticação JWT
var key = Encoding.ASCII.GetBytes("SuperSecretKeyForVersatusWebAPIDemonstrator2026");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Controladores dos módulos como Application Parts
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    })
    .AddApplicationPart(typeof(Versatus.AcessoGlobal.Api.Controllers.AuthController).Assembly)
    .AddApplicationPart(typeof(Versatus.GestaoTributo.Api.Controllers.TributacaoController).Assembly);

// Swagger e OpenAPI com suporte a Segurança (JWT Bearer)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Versatus Web API Demonstrator", 
        Version = "v1",
        Description = "Demonstrador de Acesso Global, Gestão de Tributos e Autenticação JWT."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT neste formato: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true) // Habilita em todos os ambientes no demonstrator
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Versatus Web API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthorization();


app.MapControllers();

app.Run();

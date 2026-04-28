using Microsoft.Extensions.DependencyInjection;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;

namespace Versatus.Framework.DependencyInjection;

/// <summary>
/// Extensões para registrar serviços do Framework no container DI.
/// </summary>
public static class FrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddVersatusFramework(this IServiceCollection services)
    {
        // Registrar contexto
        services.AddScoped<IContextoExecucao, ContextoExecucao>();

        // Registrar gerador sequencial
        services.AddScoped<IGeradorSequencial, GeradorSequencialService>();

        // Outros serviços base...

        return services;
    }
}

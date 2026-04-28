using Microsoft.Extensions.DependencyInjection;
using Versatus.Framework.Configuration;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using System;

namespace Versatus.Framework;

/// <summary>
/// Extensões para IServiceCollection para registro dos serviços do Framework.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona os serviços base do Versatus Framework ao container de DI.
    /// </summary>
    public static IServiceCollection AddVersatusFramework(this IServiceCollection services, Action<VersatusOptions>? configureOptions = null)
    {
        // Configurações
        var options = new VersatusOptions();
        configureOptions?.Invoke(options);
        services.AddSingleton(options);

        // Contexto de Execução
        services.AddScoped<ContextoExecucao>();
        services.AddScoped<IContextoExecucao>(provider => provider.GetRequiredService<ContextoExecucao>());

        // Sequencial
        services.AddScoped<IGeradorSequencial, GeradorSequencialService>();

        return services;
    }
}

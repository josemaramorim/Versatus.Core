using Microsoft.Extensions.DependencyInjection;

namespace Versatus.SharedKernel.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra os serviços transversais do SharedKernel financeiro
    /// (enums/Lookup/container de rateio/interfaces transversais).
    /// Ponto de extensão estável — os registros concretos entram em E0-T04.
    /// </summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        return services;
    }
}

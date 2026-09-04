using Microsoft.Extensions.DependencyInjection;

namespace Versatus.GestaoFinanceira.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra repositórios e serviços de domínio do módulo Gestão Financeira.
    /// Preenchido incrementalmente pelas tarefas de serviço de cada épico (E?-T05…).
    /// </summary>
    public static IServiceCollection AddGestaoFinanceira(this IServiceCollection services)
    {
        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Versatus.GestaoTributo.Domain.Repositories;
using Versatus.GestaoTributo.Domain.Services;
using Versatus.GestaoTributo.Infrastructure.Repositories;

namespace Versatus.GestaoTributo.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGestaoTributo(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IClassificacaoFiscalRepository, ClassificacaoFiscalRepository>();
        services.AddScoped<ICfopRepository, CfopRepository>();
        services.AddScoped<ISimplesNacionalRepository, SimplesNacionalRepository>();
        services.AddScoped<IRegraTributoConfiguracaoRepository, RegraTributoConfiguracaoRepository>();

        // Domain Services
        services.AddScoped<ITributoService, TributoService>();

        return services;
    }
}

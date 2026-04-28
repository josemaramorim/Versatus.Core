using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versatus.Framework.Sequencial;
using Versatus.Infra.Data.Context;
using Versatus.Infra.Data.Repositories;

namespace Versatus.Infra.Data;

/// <summary>
/// Extensões para registro da camada de infraestrutura.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVersatusInfraData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<VersatusDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ISequenciaRepository, SequenciaRepository>();

        return services;
    }
}

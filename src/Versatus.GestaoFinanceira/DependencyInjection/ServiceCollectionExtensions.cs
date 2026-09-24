using Microsoft.Extensions.DependencyInjection;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.GestaoFinanceira.Infrastructure.Repositories;

namespace Versatus.GestaoFinanceira.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra repositórios e serviços de domínio do módulo Gestão Financeira.
    /// Preenchido incrementalmente pelas tarefas de serviço de cada épico (E?-T05…).
    /// </summary>
    public static IServiceCollection AddGestaoFinanceira(this IServiceCollection services)
    {
        // E3 — Caixa e Banco (E3-T05)
        services.AddScoped<ICaixaBancoRepository, CaixaBancoRepository>();
        services.AddScoped<IContaBancariaRepository, ContaBancariaRepository>();
        services.AddScoped<ICobradorRepository, CobradorRepository>();
        services.AddScoped<IInstituicaoFinanceiraConsulta, InstituicaoFinanceiraConsulta>();
        services.AddScoped<IDominioFinanceiroConsulta, DominioFinanceiroConsulta>(); // temporário até o E2
        services.AddScoped<ICaixaBancoService, CaixaBancoService>();
        services.AddScoped<IContaBancariaService, ContaBancariaService>();
        services.AddScoped<ICobradorService, CobradorService>();

        return services;
    }
}

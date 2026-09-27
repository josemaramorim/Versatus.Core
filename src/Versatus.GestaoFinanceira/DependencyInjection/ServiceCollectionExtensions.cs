using Microsoft.Extensions.DependencyInjection;
using Versatus.GestaoFinanceira.Application.Bases;
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

        // E3 — listas de consulta da tela Caixa/Banco (E3-T07)
        services.AddScoped<ILookupFinanceiroConsulta, LookupFinanceiroConsulta>();
        services.AddScoped<ILookupFinanceiroService, LookupFinanceiroService>();

        // E3 — saldos e conversão por índice (E3-T09)
        services.AddScoped<ISaldoCaixaBancoRepository, SaldoCaixaBancoRepository>();
        services.AddScoped<ISaldoCalculadora, SaldoCalculadora>();
        services.AddScoped<IIndiceEconomicoConsulta, IndiceEconomicoConsulta>();
        services.AddScoped<ICalendarioConsulta, CalendarioConsulta>();
        services.AddScoped<IConversorIndiceService, ConversorIndiceService>();

        return services;
    }
}

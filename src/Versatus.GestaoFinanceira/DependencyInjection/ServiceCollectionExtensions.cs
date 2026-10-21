using Microsoft.Extensions.DependencyInjection;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.GestaoFinanceira.Application.Handlers.Periodo;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Application.Services;
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
        services.AddScoped<IDominioFinanceiroConsulta, DominioFinanceiroConsulta>(); // sobre as entidades do E2 desde o E2-T04
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

        // E2 — Domínio e Período (E2-T04)
        services.AddScoped<IDominioRepository, DominioRepository>();
        services.AddScoped<IPerfilDominioConsulta, PerfilDominioConsulta>();
        services.AddScoped<IPeriodosAbertosService, PeriodosAbertosService>();
        services.AddScoped<IDominioService, DominioService>();

        // E2 — operações de período (E2-T05). E5 (movimento financeiro) e E9 (cheques) entram por porta
        // com implementação "pendente" que recusa a operação até o épico dono (decisão D4).
        services.AddScoped<IDominioPeriodoRepository, DominioPeriodoRepository>();
        services.AddScoped<IOperacaoCaixaConsulta, OperacaoCaixaConsulta>();
        services.AddScoped<IParametroFinanceiroEscrita, ParametroFinanceiroEscrita>();
        services.AddScoped<IResumoFormaPagtoConsulta, ResumoFormaPagtoConsulta>();
        services.AddScoped<IGeradorMovimentoPeriodo, GeradorMovimentoPeriodoPendente>(); // até o E5
        services.AddScoped<IChequePeriodoPorta, ChequePeriodoPendente>();                // até o E9
        services.AddScoped<DominioPeriodoLactoService>();
        services.AddScoped<DominioPeriodoFormaPagtoService>();
        services.AddScoped<FechamentoPeriodoService>();
        services.AddScoped<AbrirPeriodoHandler>();
        services.AddScoped<FecharPeriodoHandler>();
        services.AddScoped<SuprimentoDinheiroHandler>();
        services.AddScoped<AbrirDominioPadraoHandler>();
        services.AddScoped<FecharDominioPadraoHandler>();

        return services;
    }
}

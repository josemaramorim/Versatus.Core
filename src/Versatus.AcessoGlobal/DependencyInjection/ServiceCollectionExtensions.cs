using Microsoft.Extensions.DependencyInjection;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Infrastructure.Repositories;

namespace Versatus.AcessoGlobal.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAcessoGlobal(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IEntidadeRepository, EntidadeRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
        services.AddScoped<ITransportadoraRepository, TransportadoraRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IParametroRepository, ParametroRepository>();
        services.AddScoped<ILocalizacaoRepository, LocalizacaoRepository>();
        services.AddScoped<IOrganizacaoRepository, OrganizacaoRepository>();


        // Domain Services
        services.AddScoped<IEntidadeService, EntidadeService>();
        services.AddScoped<IEnderecoService, EnderecoService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        services.AddScoped<ITransportadoraService, TransportadoraService>();
        services.AddScoped<IParametroService, ParametroService>();
        services.AddScoped<ILocalizacaoService, LocalizacaoService>();
        services.AddScoped<IFinanceiroService, FinanceiroService>();
        services.AddScoped<IOrganizacaoService, OrganizacaoService>();
        services.AddScoped<ICondicaoPagamentoService, CondicaoPagamentoService>();
        services.AddScoped<IMenuService, MenuService>();

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Versatus.SharedKernel.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ponto de registro dos serviços transversais do <c>Versatus.SharedKernel</c>.
    ///
    /// No escopo mínimo atual (CLR-02) o SharedKernel só contém enums, o contrato
    /// <c>IDadosComissao</c> e os containers de rateio (<c>RateioContainer</c> /
    /// <c>ValidationRateioContainer</c>) — todos POCOs/enums/interfaces que os consumidores
    /// instanciam ou implementam diretamente, sem passar pelo contêiner de DI. Portanto,
    /// não há nada a registrar aqui hoje.
    ///
    /// Mantido como extensão estável: quando o kernel ganhar um serviço registrável
    /// (ex.: o <c>Lookup&lt;T&gt;</c> previsto em <c>research.md §4</c>, quando um consumidor
    /// definir sua forma), o registro entra aqui e a chamada em <c>Program.cs</c> já existe.
    /// </summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        return services;
    }
}

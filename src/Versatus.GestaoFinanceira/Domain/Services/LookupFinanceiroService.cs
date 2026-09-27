using Versatus.Framework.Context;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: lookups do FCaixaBanco.cs (legado). Só consulta — filial = a do ambiente (IContextoExecucao).
public class LookupFinanceiroService(ILookupFinanceiroConsulta consulta, IContextoExecucao contexto) : ILookupFinanceiroService
{
    public Task<IReadOnlyList<ItemLookupDto>> ListarAgenciasAsync(string? texto, CancellationToken cancellationToken = default)
        => consulta.ListarAgenciasAsync(texto, cancellationToken);

    public Task<IReadOnlyList<ItemLookupDto>> ListarUsuariosAsync(string? texto, CancellationToken cancellationToken = default)
        => consulta.ListarUsuariosAsync(texto, cancellationToken);

    public Task<IReadOnlyList<ItemLookupDto>> ListarInstituicoesFinanceirasAsync(string? texto, CancellationToken cancellationToken = default)
        => consulta.ListarInstituicoesFinanceirasAsync(texto, cancellationToken);

    public Task<IReadOnlyList<ItemLookupDto>> ListarPlanosContabeisAsync(string? texto, CancellationToken cancellationToken = default)
        => consulta.ListarPlanosContabeisAsync(contexto.IdFilial, texto, cancellationToken);

    public Task<IReadOnlyList<ItemLookupDto>> ListarContasCorrentesAsync(string? texto, CancellationToken cancellationToken = default)
        => consulta.ListarContasCorrentesAsync(contexto.IdFilial, texto, cancellationToken);
}

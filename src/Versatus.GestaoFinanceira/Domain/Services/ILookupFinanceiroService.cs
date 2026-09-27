using Versatus.GestaoFinanceira.Domain.DTOs;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Listas de consulta da tela Caixa/Banco (E3-T07 — DÚVIDA-CB5). Filial = a do contexto.</summary>
public interface ILookupFinanceiroService
{
    Task<IReadOnlyList<ItemLookupDto>> ListarAgenciasAsync(string? texto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ItemLookupDto>> ListarUsuariosAsync(string? texto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ItemLookupDto>> ListarInstituicoesFinanceirasAsync(string? texto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ItemLookupDto>> ListarPlanosContabeisAsync(string? texto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ItemLookupDto>> ListarContasCorrentesAsync(string? texto, CancellationToken cancellationToken = default);
}

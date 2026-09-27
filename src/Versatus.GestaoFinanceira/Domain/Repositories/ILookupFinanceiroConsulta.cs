using Versatus.GestaoFinanceira.Domain.DTOs;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Porta de leitura das listas de consulta da tela Caixa/Banco (DÚVIDA-CB5, decisão do usuário em
/// 2026-09-27): tabelas do MOD-02 e do módulo contábil ainda sem endpoint próprio. Filtro por
/// texto (código ou descrição); resultado limitado a <see cref="LimiteItens"/>.
/// </summary>
public interface ILookupFinanceiroConsulta
{
    const int LimiteItens = 100;

    /// <summary>GLOAGENCIA + GLOBANCO — lookupAgencia.</summary>
    Task<IReadOnlyList<ItemLookupDto>> ListarAgenciasAsync(string? texto, CancellationToken cancellationToken = default);

    /// <summary>GLOUSUARIO — grade de usuários do caixa.</summary>
    Task<IReadOnlyList<ItemLookupDto>> ListarUsuariosAsync(string? texto, CancellationToken cancellationToken = default);

    /// <summary>GLOINSTITUICAOFINANCEIRA de pessoa jurídica — FCaixaBanco.cs:2387-2391 (UI-14).</summary>
    Task<IReadOnlyList<ItemLookupDto>> ListarInstituicoesFinanceirasAsync(string? texto, CancellationToken cancellationToken = default);

    /// <summary>CONPLANOCONTABIL analítico da filial — CaixaBanco.cs:460 (ValidacaoSinteticoAnalitico).</summary>
    Task<IReadOnlyList<ItemLookupDto>> ListarPlanosContabeisAsync(int idFilial, string? texto, CancellationToken cancellationToken = default);

    /// <summary>Contas bancárias Conta corrente da filial — FCaixaBanco.cs:2372-2376 (UI-12).</summary>
    Task<IReadOnlyList<ItemLookupDto>> ListarContasCorrentesAsync(int idFilial, string? texto, CancellationToken cancellationToken = default);
}

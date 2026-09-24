namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Porta cross-módulo (MOD-02) para a instituição financeira do SPED — VAL-E3-02/04/05.
/// No legado, <c>InstituicaoFinanceira.Entidade</c> é a entidade de mesmo Id
/// (GLOINSTITUICAOFINANCEIRA.IDGLOINSTITUICAOFINANCEIRA = GLOENTIDADE.IDGLOENTIDADE).
/// </summary>
public interface IInstituicaoFinanceiraConsulta
{
    /// <summary>Retorna <c>null</c> quando a instituição não existe.</summary>
    Task<InstituicaoFinanceiraInfo?> ObterAsync(int idInstituicaoFinanceira, CancellationToken cancellationToken = default);
}

/// <param name="IdFisicaJuridica">GLOENTIDADE.IDFISICAJURIDICA (2 = Física, 3 = Jurídica — DEC-007).</param>
/// <param name="Cnpj">GLOENTIDADEJURIDICA.CNPJ (nulo se a entidade não tem registro jurídico).</param>
public sealed record InstituicaoFinanceiraInfo(int IdFisicaJuridica, string? Cnpj);

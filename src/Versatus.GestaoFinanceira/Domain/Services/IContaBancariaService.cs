using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Conta bancária — núcleo E3 (FINCONTABANCARIA). Integração bancária → E14.</summary>
public interface IContaBancariaService
{
    Task<ContaBancariaDto?> ObterPorIdAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Persiste a conta (OP-E3-05, sem os sequenciais de arquivo [E14]) em 1 transação.</summary>
    Task<Result<ContaBancariaDto>> AtualizarAsync(int idCaixaBanco, int idFilial, AtualizarContaBancariaDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// VAL-E3-23..26 — efeitos dos setters do legado (terceiro, SPED, tipo da conta, conta
    /// vinculada). Aplicado antes das validações; não grava nada.
    /// </summary>
    Task NormalizarAsync(ContaBancaria conta, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Validações núcleo de <c>ContaBancaria.Validate</c> — VAL-E3-17.</summary>
    ValidationResult Validar(ContaBancaria conta);

    /// <summary>
    /// OP-E3-05 (UpdateDadosContaVinculada) — propaga os dados da conta para as contas que a têm
    /// como vinculada. Só marca as alterações; quem grava é o chamador (Artigo VII.2).
    /// </summary>
    Task AtualizarDadosContasVinculadasAsync(ContaBancaria conta, int idFilial, CancellationToken cancellationToken = default);
}

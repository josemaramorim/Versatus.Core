using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Conta bancária — núcleo E3 (FINCONTABANCARIA). Integração bancária → E14.</summary>
public interface IContaBancariaService
{
    Task<ContaBancaria?> ObterPorIdAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Persiste a conta (OP-E3-05, sem os sequenciais de arquivo [E14]) em 1 transação.</summary>
    Task<Result<ContaBancaria>> AtualizarAsync(ContaBancaria conta, CancellationToken cancellationToken = default);

    /// <summary>Validações núcleo de <c>ContaBancaria.Validate</c> — VAL-E3-17.</summary>
    ValidationResult Validar(ContaBancaria conta);

    /// <summary>
    /// OP-E3-05 (UpdateDadosContaVinculada) — propaga os dados da conta para as contas que a têm
    /// como vinculada. Só marca as alterações; quem grava é o chamador (Artigo VII.2).
    /// </summary>
    Task AtualizarDadosContasVinculadasAsync(ContaBancaria conta, int idFilial, CancellationToken cancellationToken = default);
}

using Versatus.Framework.Pagination;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Caixa/Banco (FINCAIXABANCO) + usuários (FINCAIXABANCOUSUARIO) + conta bancária 1:1.</summary>
public interface ICaixaBancoService
{
    Task<PagedResult<CaixaBanco>> ListarPaginadoAsync(string? texto, bool? ativo, ContaTipo? tipoConta, bool? entraFluxoCaixa,
        int page, int limit, CancellationToken cancellationToken = default);

    Task<CaixaBanco?> ObterPorIdAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>OP-E3-01/03/04/07 — cria o caixa (+ usuários + conta bancária quando Banco) em 1 transação.</summary>
    Task<Result<CaixaBanco>> CriarAsync(CaixaBanco caixa, ContaBancaria? conta, CancellationToken cancellationToken = default);

    /// <summary>OP-E3-01/03/04/07 — atualiza o caixa (+ usuários + conta bancária) em 1 transação.</summary>
    Task<Result<CaixaBanco>> AtualizarAsync(CaixaBanco caixa, ContaBancaria? conta, CancellationToken cancellationToken = default);

    /// <summary>OP-E3-02 — exclui conta bancária, usuários e caixa em 1 transação.</summary>
    Task<ValidationResult> ExcluirAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CaixaBancoUsuario>> ListarUsuariosAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>OP-E3-07 — sincroniza a grade de usuários do caixa (VAL-E3-11/12).</summary>
    Task<Result<IReadOnlyList<CaixaBancoUsuario>>> SalvarUsuariosAsync(int idCaixaBanco, int idFilial,
        IReadOnlyList<CaixaBancoUsuarioItemDto> itens, CancellationToken cancellationToken = default);

    /// <summary>VAL-E3-01 — CaixaBanco.ValidarUsuario (usuário repetido, com o parâmetro VinculaCaixaBancoUsuario).</summary>
    Task<ValidationResult> ValidarUsuarioAsync(CaixaBanco caixa, CancellationToken cancellationToken = default);

    /// <summary>
    /// VAL-E3-09 — CaixaBanco.ValidarControleCaixaBanco. O status do período vem de
    /// PeriodosAbertos.PeriodoAberto (épico E2), calculado pelo chamador.
    /// </summary>
    Task<ValidationResult> ValidarControleCaixaBancoAsync(PeriodoStatus statusPeriodo, CancellationToken cancellationToken = default);

    /// <summary>VAL-E3-10 — CaixaBanco.ValidarCaixaPeriodo (caixa movimentável pelo usuário logado).</summary>
    Task<ValidationResult> ValidarCaixaPeriodoAsync(CaixaBanco caixa, CancellationToken cancellationToken = default);

    /// <summary>
    /// VAL-E1-27 — ParcelaBase.ValidarCaixaBanco: caixa do tipo Banco exige conta Conta corrente.
    /// Zerar o IdCaixaBancoParcela na falha é do serviço de parcela (E4).
    /// </summary>
    Task<ValidationResult> ValidarCaixaBancoParcelaAsync(int? idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);
}

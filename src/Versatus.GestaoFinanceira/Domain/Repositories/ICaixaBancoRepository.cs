using Microsoft.EntityFrameworkCore.Storage;
using Versatus.Framework.Repositories;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Repositório do agregado CaixaBanco (FINCAIXABANCO + FINCAIXABANCOUSUARIO) e da conta
/// bancária 1:1 (FINCONTABANCARIA). CQRS leve (Artigo VII.4): métodos <c>Listar*</c>/<c>Obter*</c>
/// leem no ReadContext; <c>*ParaEdicao*</c> carregam rastreado no Context de escrita.
/// </summary>
public interface ICaixaBancoRepository : IRepositorio<CaixaBanco>
{
    Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default);

    /// <summary>Materializa a consulta filtrada (Artigo VII.5 — o chamador pagina em memória).</summary>
    Task<IReadOnlyList<CaixaBanco>> ListarAsync(int idFilial, string? texto, bool? ativo, ContaTipo? tipoConta,
        bool? entraFluxoCaixa, CancellationToken cancellationToken = default);

    Task<CaixaBanco?> ObterAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    Task<CaixaBanco?> ObterParaEdicaoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CaixaBancoUsuario>> ListarUsuariosAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    Task<ContaBancaria?> ObterContaBancariaParaEdicaoAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);

    Task AdicionarContaBancariaAsync(ContaBancaria conta, CancellationToken cancellationToken = default);

    void RemoverContaBancaria(ContaBancaria conta);

    void RemoverUsuario(CaixaBancoUsuario usuario);
}

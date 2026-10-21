using Microsoft.EntityFrameworkCore.Storage;
using Versatus.Framework.Repositories;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.Services;
using DominioEntidade = Versatus.GestaoFinanceira.Domain.Dominio.Dominio;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Escrita das operações de período (FINDOMINIOPERIODO, FINDOMINIOPERIODOFECHAMENTO,
/// FINDOMINIOPERIODOLANCTO, FINDOMINIOPERIODOFORMAPAGTO, saldos e ponteiro de FINDOMINIO).
/// Tudo no Context de escrita: as leituras feitas DENTRO da transação do handler não podem ir à
/// ReadConnection (outra conexão — veria/bloquearia linhas ainda não confirmadas).
/// </summary>
public interface IDominioPeriodoRepository : IRepositorio<DominioPeriodo>
{
    Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default);

    Task<DominioEntidade?> ObterDominioAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Domínio do usuário na filial (menor IdDominio) — leitura no Context de escrita.</summary>
    Task<DominioEntidade?> ObterDominioDoUsuarioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default);

    Task<DominioPeriodo?> ObterPeriodoAsync(int idDominioPeriodo, int idFilial, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DominioEntidade>> ListarDominiosAsync(int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Domínio novo criado pela abertura do domínio padrão.</summary>
    Task AddDominioAsync(DominioEntidade dominio, CancellationToken cancellationToken = default);

    Task AdicionarLanctoAsync(DominioPeriodoLacto lancto, CancellationToken cancellationToken = default);

    Task AdicionarFormaPagtoAsync(DominioPeriodoFormaPagto forma, CancellationToken cancellationToken = default);

    void RemoverLancto(DominioPeriodoLacto lancto);

    void RemoverFormaPagto(DominioPeriodoFormaPagto forma);

    /// <summary>Soma os ajustes em FINDOMINIO.SALDO e na coluna da forma (nulo conta como 0).</summary>
    Task AplicarSaldoAsync(IReadOnlyList<AjusteSaldoDominio> ajustes, int idFilial, CancellationToken cancellationToken = default);
}

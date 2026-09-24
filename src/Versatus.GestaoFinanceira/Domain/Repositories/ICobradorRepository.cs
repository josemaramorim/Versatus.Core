using Microsoft.EntityFrameworkCore.Storage;
using Versatus.Framework.Repositories;
using Versatus.GestaoFinanceira.Domain.Bancos;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>Repositório do cadastro de cobrador (FINCOBRADOR).</summary>
public interface ICobradorRepository : IRepositorio<Cobrador>
{
    Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default);

    /// <summary>Materializa a consulta filtrada (Artigo VII.5 — o chamador pagina em memória).</summary>
    Task<IReadOnlyList<Cobrador>> ListarAsync(int idFilial, string? texto, bool? ativo, CancellationToken cancellationToken = default);

    Task<Cobrador?> ObterAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default);

    Task<Cobrador?> ObterParaEdicaoAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default);
}

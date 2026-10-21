using Microsoft.EntityFrameworkCore.Storage;
using Versatus.Framework.Repositories;
using DominioEntidade = Versatus.GestaoFinanceira.Domain.Dominio.Dominio;
using Versatus.GestaoFinanceira.Domain.Dominio;

namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Repositório do agregado Dominio (FINDOMINIO + FINDOMINIOUSUARIO + FINDOMINIORESPONSAVEL) e das
/// consultas de período (FINDOMINIOPERIODO / FINDOMINIOPERIODOLANCTO) usadas pelo E2-T04.
/// CQRS leve (Artigo VII.4): <c>Listar*</c>/<c>Obter*</c> no ReadContext; <c>*ParaEdicao*</c>
/// rastreado no Context de escrita.
/// </summary>
public interface IDominioRepository : IRepositorio<DominioEntidade>
{
    Task<IDbContextTransaction> IniciarTransacaoAsync(CancellationToken cancellationToken = default);

    /// <summary>Materializa a consulta filtrada (Artigo VII.5 — o chamador pagina em memória).</summary>
    Task<IReadOnlyList<DominioEntidade>> ListarAsync(int idFilial, string? texto, bool? ativo, CancellationToken cancellationToken = default);

    /// <summary>Domínio com usuários e responsáveis (leitura).</summary>
    Task<DominioEntidade?> ObterAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    Task<DominioEntidade?> ObterParaEdicaoAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>
    /// Domínio do usuário na filial: o de menor IdDominio em FINDOMINIOUSUARIO
    /// (DominioUsuario.GetDominio — DominioUsuario.cs:457-473).
    /// </summary>
    Task<DominioEntidade?> ObterDominioDoUsuarioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Domínio de cada usuário (menor IdDominio) — base do PeriodosAbertos.</summary>
    Task<IReadOnlyList<DominioDoUsuario>> ListarDominiosDosUsuariosAsync(IReadOnlyCollection<int> idsUsuarios, int idFilial,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Outros domínios ATIVOS da filial (≠ <paramref name="idDominio"/>) que contêm o usuário —
    /// DominioUsuario.ValidarUsuario (117-158) e Dominio.ValidarUsuarioDominioAtivo (300-319).
    /// </summary>
    Task<IReadOnlyList<int>> ListarOutrosDominiosAtivosDoUsuarioAsync(int idUsuario, int idFilial, int idDominio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// O domínio <paramref name="idDominio"/> tem o <paramref name="idDominioPai"/> como responsável —
    /// DominioResponsavelLista.VerificarResponsavelCadastrado (45-56).
    /// </summary>
    Task<bool> PossuiResponsavelAsync(int idDominio, int idDominioPai, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Filhos do domínio (FINDOMINIORESPONSAVEL.IDFINDOMINIOPAI = pai).</summary>
    Task<IReadOnlyList<DominioResponsavel>> ListarFilhosAsync(int idDominioPai, int idFilial, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DominioEntidade>> ListarPorIdsAsync(IReadOnlyCollection<int> idsDominios, int idFilial,
        CancellationToken cancellationToken = default);

    Task<DominioPeriodo?> ObterPeriodoAsync(int idDominioPeriodo, int idFilial, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DominioPeriodo>> ListarPeriodosAsync(IReadOnlyCollection<int> idsPeriodos, int idFilial,
        CancellationToken cancellationToken = default);

    Task<bool> PossuiPeriodosAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Último período fechado do domínio (maior Id com DataFechamento) — Dominio.GetDataUltimoFechamento.</summary>
    Task<DominioPeriodo?> ObterUltimoPeriodoFechadoAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>Lançamento de abertura do período (destino = período, tipo Abertura) — DominioPeriodo.RetornarValorAbertura.</summary>
    Task<DominioPeriodoLacto?> ObterLanctoAberturaAsync(int idDominioPeriodo, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hierarquia achatada usuário × domínio × pai/filho, com a mesma regra da view
    /// VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA (3 blocos do UNION ALL), calculada sobre as tabelas.
    /// </summary>
    Task<IReadOnlyList<DominioUsuarioSuprimentoSangria>> ListarHierarquiaAsync(int idFilial, CancellationToken cancellationToken = default);

    void RemoverUsuario(DominioUsuario usuario);

    void RemoverResponsavel(DominioResponsavel responsavel);
}

/// <summary>Domínio de um usuário e o período corrente desse domínio.</summary>
public sealed record DominioDoUsuario(
    int IdUsuario,
    int IdDominio,
    bool Ativo,
    int IdCaixaBanco,
    string? DescricaoCaixaBanco,
    int? IdDominioPeriodo,
    DateTime? DataFechamento);

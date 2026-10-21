using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>
/// Períodos abertos dos usuários de domínio financeiro na empresa/filial do contexto
/// (PeriodosAbertos.cs — OP-E2-13) e permissão de trocar caixa/banco (OP-E2-14).
/// </summary>
public interface IPeriodosAbertosService
{
    /// <summary>Situação do período do usuário (padrão: o usuário logado) — PeriodosAbertos.PeriodoAberto.</summary>
    Task<PeriodoStatus> PeriodoAbertoAsync(int? idUsuario = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// PeriodosAbertos.ValidarPeriodoAberto. Sem o cache do legado, recarregar é o mesmo que consultar:
    /// equivale a <see cref="PeriodoAbertoAsync"/> do usuário logado.
    /// </summary>
    Task<PeriodoStatus> ValidarPeriodoAbertoAsync(CancellationToken cancellationToken = default);

    /// <summary>Período aberto do usuário logado (null se não se aplica ou não está aberto) — VAL-E2-42.</summary>
    Task<Result<int?>> RetornarIdPeriodoAsync(CancellationToken cancellationToken = default);

    Task<InfoPeriodoDto> ObterInfoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>Caixa do domínio do usuário logado (0 se não há).</summary>
    Task<int> RetornarIdCaixaBancoAsync(CancellationToken cancellationToken = default);

    Task<int> RetornarIdCaixaBancoDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    Task<int> RetornarIdUsuarioDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>Período do domínio padrão (pode ser 0 quando fechado; null se o padrão não está na lista).</summary>
    Task<int?> RetornarIdPeriodoPadraoAsync(CancellationToken cancellationToken = default);

    /// <summary>Caixas com período aberto na filial — PeriodosAbertos.RetornarListaCaixaPeriodo.</summary>
    Task<IReadOnlyList<int>> ListarCaixasComPeriodoAbertoAsync(CancellationToken cancellationToken = default);

    /// <summary>Dominio.EditarCaixaBancoDominioPeriodo (OP-E2-14).</summary>
    Task<bool> PodeEditarCaixaBancoAsync(bool validarBanco, CancellationToken cancellationToken = default);
}

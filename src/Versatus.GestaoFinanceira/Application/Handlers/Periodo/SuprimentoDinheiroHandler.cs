using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Services;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Handlers.Periodo;

/// <param name="IdUsuarioSuprimento">Usuário do caixa que recebe o dinheiro (a sangria é sempre do usuário logado).</param>
public sealed record SuprimentoDinheiroCommand(int IdUsuarioSuprimento, decimal Valor);

// Origem: Dominio.cs:EfetuarSuprimentoDinheiro (1338) → PermiteUsuarioSuprimento (1301), PersistirDominioPeriodo
// (414); regras da tela FSuprimentoBase/FSuprimentoDinheiro levadas ao backend (decisão D1).
// Cobre: OP-E2-08 · VAL-E2-01..04, 16, 17, 19, 46, 47.
public class SuprimentoDinheiroHandler(
    IDominioPeriodoRepository periodos,
    IDominioRepository dominios,
    IPeriodosAbertosService periodosAbertos,
    ICaixaBancoService caixaBancoService,
    IPerfilDominioConsulta perfis,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    DominioPeriodoLactoService lanctos,
    IContextoExecucao contexto)
    : PeriodoHandlerBase(periodos, dominios, periodosAbertos, caixaBancoService, perfis, parametroRepository, geradorSequencial, lanctos, contexto)
{
    /// <summary>Devolve o lançamento gravado (0 quando o legado não grava: domínio de origem sem período — Q2).</summary>
    public async Task<Result<int>> ExecutarAsync(SuprimentoDinheiroCommand comando, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(comando);

        var logado = await Dominios.ObterDominioDoUsuarioAsync(Contexto.IdUsuario, Contexto.IdFilial, cancellationToken);
        if (logado is null)
            return Result<int>.Fail([.. Falha(string.Format(MensagensPeriodo.LogadoSemDominio, Contexto.IdUsuario)).Errors]);

        var validacao = await ValidarAsync(comando, logado, cancellationToken);
        if (!validacao.IsValid)
            return Result<int>.Fail([.. validacao.Errors]);

        // PersistirDominioPeriodo(destino, origem = logado): VAL-E2-19 de novo contra o domínio de origem.
        var destino = (await Dominios.ObterDominioDoUsuarioAsync(comando.IdUsuarioSuprimento, Contexto.IdFilial, cancellationToken))!;
        var saldo = ValidarSaldoDominio(comando.Valor, logado, StatusDominioFinanceiro.SuprimentoDinheiro);
        if (!saldo.IsValid)
            return Result<int>.Fail([.. saldo.Errors]);

        // GetDominioPeriodo: o período ATUAL da origem; sem ele o legado sai sem gravar.
        if (logado.IdDominioPeriodo is not int idPeriodoOrigem)
            return Result<int>.Ok(0);

        await using var transacao = await Periodos.IniciarTransacaoAsync(cancellationToken);

        // dp.Persist do período da origem: só a auditoria de alteração (sem datas de abertura/fechamento).
        var periodo = await Periodos.ObterPeriodoAsync(idPeriodoOrigem, Contexto.IdFilial, cancellationToken);
        if (periodo is null)
            return Result<int>.Ok(0);
        RegistrarAlteracao(periodo);
        await Periodos.SaveChangesAsync(cancellationToken);

        var lancto = LanctoDinheiro(LanctoDominioPeriodoTipo.Suprimento, logado.IdDominioPeriodo, destino.IdDominioPeriodo,
            destino.IdCaixaBanco, comando.Valor);
        var gravado = await Lanctos.GravarAsync(lancto, StatusDominioFinanceiro.SuprimentoDinheiro, cancellationToken);
        if (!gravado.IsValid)
            return Result<int>.Fail([.. gravado.Errors]);

        await transacao.CommitAsync(cancellationToken);
        return Result<int>.Ok(lancto.IdDominioPeriodoLacto);
    }

    private async Task<ValidationResult> ValidarAsync(SuprimentoDinheiroCommand comando, Domain.Dominio.Dominio logado, CancellationToken cancellationToken)
    {
        // VAL-E2-47 / VAL-E2-16 (item 31) — usuário do suprimento.
        if (comando.IdUsuarioSuprimento == 0)
            return Falha(MensagensPeriodo.SuprimentoSemUsuario);

        // VAL-E2-46 — FSuprimentoBase.FiltrarUsuarioSuprimento: caixa de OUTRO domínio, na linha do próprio domínio.
        var hierarquia = await Dominios.ListarHierarquiaAsync(Contexto.IdFilial, cancellationToken);
        if (!hierarquia.Any(h => h.IdUsuarioRetorno == comando.IdUsuarioSuprimento && h.IdDominio != logado.IdDominio && h.Caixa && h.IdDominioFilho is null))
            return Falha(string.Format(MensagensPeriodo.SuprimentoNaoPermitido, Contexto.IdUsuario, Contexto.IdUsuario));

        var controle = await ValidarControleCaixaBancoAsync(cancellationToken);                                // VAL-E2-01
        if (!controle.IsValid)
            return controle;

        // PermiteUsuarioSuprimento — VAL-E2-16.
        var destino = await Dominios.ObterDominioDoUsuarioAsync(comando.IdUsuarioSuprimento, Contexto.IdFilial, cancellationToken);
        if (destino is null)
            return Falha(MensagensPeriodo.SuprimentoSemUsuario);

        var config = await ValidarConfigDominioAsync(destino, logado.IdDominio, aberturaPadrao: false, cancellationToken);
        if (!config.IsValid)
            return config;

        var idPeriodoDestino = destino.IdDominioPeriodo ?? 0;
        if ((await PeriodosAbertos.ObterInfoPeriodoAsync(idPeriodoDestino, cancellationToken)).Status != PeriodoStatus.Aberto)
            return Falha(string.Format(MensagensPeriodo.PeriodoNaoAberto, destino.IdDominio));

        // VAL-E2-17.
        if (comando.Valor <= 0)
            return Falha(MensagensPeriodo.SuprimentoValorZero);
        if ((logado.SaldoDinheiro ?? 0m) < comando.Valor)
            return Falha(string.Format(MensagensPeriodo.ValorMaiorQueSaldo, FormatarValor(comando.Valor), FormatarValor(logado.SaldoDinheiro ?? 0m)));

        return ValidationResult.Ok();
    }
}

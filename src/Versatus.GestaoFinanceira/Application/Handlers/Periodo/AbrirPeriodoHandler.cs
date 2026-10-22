using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Services;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Handlers.Periodo;

/// <param name="IdUsuarioCaixa">Usuário do caixa a abrir.</param>
/// <param name="IdUsuarioSangria">Usuário de quem sai o dinheiro da abertura.</param>
/// <param name="Valor">Valor em dinheiro da abertura (0 = abre sem dinheiro).</param>
public sealed record AbrirPeriodoCommand(int IdUsuarioCaixa, int IdUsuarioSangria, decimal Valor);

// Origem: Dominio.cs:PersistirAberturaDominioPeriodo (1529) → PermiteUsuarioAbertura (1467),
// PermiteUsuarioSangriaAbertura (1486), PersistirDominioPeriodo (414), UpdatePeriodoDominio (493);
// DominioPeriodo.cs (Assimilar, OnBeforeExecutarPersistir, PersistirLanctoPeriodo, CarregarIdPeriodo) e as
// regras da tela FDominioFinanceiroAbertura levadas ao backend (decisão D1).
// Cobre: OP-E2-03 · VAL-E2-01..04, 19..23, 43, 44, 46, 47, 49.
public class AbrirPeriodoHandler(
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
    public async Task<Result<int>> ExecutarAsync(AbrirPeriodoCommand comando, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(comando);

        var validacao = await ValidarAsync(comando, cancellationToken);
        if (!validacao.IsValid)
            return Result<int>.Fail([.. validacao.Errors]);

        // Dominio.PersistirDominioPeriodo: origem = domínio da sangria, destino = domínio do caixa.
        var origem = (await Dominios.ObterDominioDoUsuarioAsync(comando.IdUsuarioSangria, Contexto.IdFilial, cancellationToken))!;
        var destino = (await Dominios.ObterDominioDoUsuarioAsync(comando.IdUsuarioCaixa, Contexto.IdFilial, cancellationToken))!;

        var saldo = ValidarSaldoDominio(comando.Valor, origem, StatusDominioFinanceiro.Abertura);
        if (!saldo.IsValid)
            return Result<int>.Fail([.. saldo.Errors]);

        await using var transacao = await Periodos.IniciarTransacaoAsync(cancellationToken);

        // DominioPeriodo.ExecutarPersistir: o período novo, depois os lançamentos com o destino = período novo.
        var periodo = await CriarPeriodoAsync(destino.IdDominio, cancellationToken);

        if (comando.Valor != 0)
        {
            var lancto = LanctoDinheiro(LanctoDominioPeriodoTipo.Abertura, origem.IdDominioPeriodo, periodo.IdDominioPeriodo,
                destino.IdCaixaBanco, comando.Valor);
            var gravado = await Lanctos.GravarAsync(lancto, StatusDominioFinanceiro.Abertura, cancellationToken);
            if (!gravado.IsValid)
                return Result<int>.Fail([.. gravado.Errors]);
        }

        await AtualizarPonteiroAsync(destino.IdDominio, periodo.IdDominioPeriodo, cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<int>.Ok(periodo.IdDominioPeriodo);
    }

    private async Task<ValidationResult> ValidarAsync(AbrirPeriodoCommand comando, CancellationToken cancellationToken)
    {
        var perfil = await ValidarPerfilLogadoAsync(cancellationToken);                                         // VAL-E2-49
        if (!perfil.IsValid)
            return perfil;

        if (comando.IdUsuarioCaixa == 0)                                                                       // VAL-E2-47
            return Falha(MensagensPeriodo.InformarUsuarioCaixa);
        if (comando.IdUsuarioSangria == 0)
            return Falha(MensagensPeriodo.InformarUsuarioSangria);

        // VAL-E2-21 — antes das regras da tela: pelo filtro de sangria da tela ela seria inalcançável.
        if (comando.IdUsuarioCaixa == comando.IdUsuarioSangria)
            return Falha(MensagensPeriodo.CaixaIgualSangria);

        var logado = await Dominios.ObterDominioDoUsuarioAsync(Contexto.IdUsuario, Contexto.IdFilial, cancellationToken);
        if (logado is null)
            return Falha(string.Format(MensagensPeriodo.LogadoSemDominio, Contexto.IdUsuario));

        // VAL-E2-20 — PermiteUsuarioAbertura: sem permissão o legado não grava e não avisa; a tela avisa (D1).
        if (!logado.AbrirPeriodo && !logado.AbrirOutrosPeriodos)
            return Falha(string.Format(MensagensPeriodo.LogadoSemConfiguracaoAbertura, Contexto.IdUsuario));

        // VAL-E2-44 — o caixa precisa estar na hierarquia do domínio logado (filtro do lookup + ValidarUsuarioCaixa).
        var hierarquia = await Dominios.ListarHierarquiaAsync(Contexto.IdFilial, cancellationToken);
        var linha = FiltrarCaixas(hierarquia, logado.IdDominio, logado.AbrirPeriodo, logado.AbrirOutrosPeriodos)
            .FirstOrDefault(h => h.IdUsuarioRetorno == comando.IdUsuarioCaixa);
        if (linha is null)
            return Falha(string.Format(MensagensPeriodo.CaixaNaoPodeSerAberto, comando.IdUsuarioCaixa, Contexto.IdUsuario));

        var selecionado = (await Dominios.ObterAsync(linha.IdDominioRetorno, Contexto.IdFilial, cancellationToken))!;

        // VAL-E2-43 — o caixa do domínio padrão abre em outra rotina.
        if (await ParametroIntAsync(ParametroDominioPadrao, cancellationToken) == selecionado.IdDominio)
            return Falha(string.Format(MensagensPeriodo.CaixaNaoPermitido, comando.IdUsuarioCaixa));

        var proprio = selecionado.IdDominio == logado.IdDominio;

        // SetUsuarioCaixaDefault: abrir outro caixa exige o período do logado aberto.
        if (!proprio && (logado.IdDominioPeriodo is not int idLogado
                         || (await PeriodosAbertos.ObterInfoPeriodoAsync(idLogado, cancellationToken)).Status != PeriodoStatus.Aberto))
            return Falha(string.Format(MensagensPeriodo.LogadoPeriodoDeveEstarAberto, Contexto.IdUsuario));

        // VAL-E2-46 — sangria: abrindo outro caixa, é o próprio logado; abrindo o próprio, um usuário de domínio-pai.
        var sangriaPermitida = proprio
            ? hierarquia.Any(h => h.IdUsuarioRetorno == comando.IdUsuarioSangria && !h.Caixa && h.IdDominioFilho == logado.IdDominio
                                  && h.IdDominio != logado.IdDominio)
            : comando.IdUsuarioSangria == Contexto.IdUsuario;
        if (!sangriaPermitida)
            return Falha(MensagensPeriodo.SangriaNaoPermitida);

        var controle = await ValidarControleCaixaBancoAsync(cancellationToken);                                // VAL-E2-01
        if (!controle.IsValid)
            return controle;

        // PermiteUsuarioAbertura — VAL-E2-02..04 do logado e do selecionado.
        var configLogado = await ValidarConfigDominioAsync(logado, logado.IdDominio, aberturaPadrao: false, cancellationToken);
        if (!configLogado.IsValid)
            return configLogado;
        var configSelecionado = await ValidarConfigDominioAsync(selecionado, logado.IdDominio, aberturaPadrao: false, cancellationToken);
        if (!configSelecionado.IsValid)
            return configSelecionado;

        // PermiteUsuarioSangriaAbertura — VAL-E2-22/23 (a VAL-E2-21 é verificada logo no início).
        if (!await PeriodoFechadoAsync(selecionado, cancellationToken))
            return Falha(string.Format(MensagensPeriodo.PeriodoJaAberto, comando.IdUsuarioCaixa));

        var sangria = await Dominios.ObterDominioDoUsuarioAsync(comando.IdUsuarioSangria, Contexto.IdFilial, cancellationToken);
        if (sangria is null)
            return Falha(MensagensPeriodo.SangriaNaoPermitida);

        var configSangria = await ValidarConfigDominioAsync(sangria, logado.IdDominio, aberturaPadrao: false, cancellationToken);
        if (!configSangria.IsValid)
            return configSangria;

        if (sangria.IdDominioPeriodo is null || await PeriodoFechadoAsync(sangria, cancellationToken))
        {
            return await ParametroIntAsync(ParametroDominioPadrao, cancellationToken) == sangria.IdDominio
                ? Falha($"{MensagensPeriodo.AbrirPadrao}\r\n\r\n{MensagensPeriodo.AberturaPadraoOutraRotina}")
                : Falha(string.Format(MensagensPeriodo.SangriaPeriodoFechado, comando.IdUsuarioSangria));
        }

        return ValidationResult.Ok();
    }
}

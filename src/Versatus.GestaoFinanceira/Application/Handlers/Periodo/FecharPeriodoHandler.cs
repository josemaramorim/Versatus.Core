using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Application.Services;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;
using DominioEntidade = Versatus.GestaoFinanceira.Domain.Dominio.Dominio;

namespace Versatus.GestaoFinanceira.Application.Handlers.Periodo;

/// <param name="IdUsuarioCaixa">Usuário do caixa a fechar.</param>
/// <param name="IdUsuarioSuprimento">Usuário que recebe o fechamento (domínio de destino).</param>
/// <param name="Formas">Quantidade/valor conferidos por forma de pagamento.</param>
public sealed record FecharPeriodoCommand(int IdUsuarioCaixa, int IdUsuarioSuprimento, IReadOnlyList<FormaInformada> Formas);

// Origem: Dominio.cs:PersistirFechamentoDominioPeriodo (1661) → PermiteUsuarioFechamento (1580),
// PermiteUsuarioSuprimentoFechamento (1611), PersistirDominioPeriodo (414); DominioPeriodo.cs (Assimilar,
// CarregarListaLancamentos 184-229, CarregarChequesFechamento 261-300, OnBeforeExecutarPersistir 309-347);
// DominioPeriodoFechamentoLista.AddFechamento; regras da tela FDominioFinanceiroFechamento (decisão D1).
// Cobre: OP-E2-04, OP-E2-05 (tesouraria) · VAL-E2-01..04, 26, 27, 43, 45, 46, 47, 49 · CALC-E2-08.
public class FecharPeriodoHandler(
    IDominioPeriodoRepository periodos,
    IDominioRepository dominios,
    IPeriodosAbertosService periodosAbertos,
    ICaixaBancoService caixaBancoService,
    IPerfilDominioConsulta perfis,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    DominioPeriodoLactoService lanctos,
    FechamentoPeriodoService fechamentoService,
    IChequePeriodoPorta cheques,
    IContextoExecucao contexto)
    : PeriodoHandlerBase(periodos, dominios, periodosAbertos, caixaBancoService, perfis, parametroRepository, geradorSequencial, lanctos, contexto)
{
    // [AutoSequencial("IdDominioPeriodoFechamento", SequencialTipo.Filial)].
    public const string NomeSequencialFechamento = "DominioPeriodoFechamento";

    private readonly IGeradorSequencial _geradorSequencial = geradorSequencial;

    public async Task<Result<int>> ExecutarAsync(FecharPeriodoCommand comando, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(comando);

        var validacao = await ValidarAsync(comando, cancellationToken);
        if (!validacao.IsSuccess)
            return Result<int>.Fail([.. validacao.Errors]);

        var (origem, destino, formas) = validacao.Value!;

        await using var transacao = await Periodos.IniciarTransacaoAsync(cancellationToken);

        var periodo = (await Periodos.ObterPeriodoAsync(origem.IdDominioPeriodo!.Value, Contexto.IdFilial, cancellationToken))!;
        var lanctos = new List<DominioPeriodoLacto>();

        // CarregarListaLancamentos (Fechamento): 1 linha de fechamento por forma + lançamentos.
        foreach (var forma in formas)
        {
            var agora = DateTime.Now;
            periodo.AdicionarFechamento(new DominioPeriodoFechamento
            {
                IdFilial = Contexto.IdFilial,
                IdDominioPeriodoFechamento = await _geradorSequencial.ProximoAsync(NomeSequencialFechamento, SequencialTipo.Filial, cancellationToken),
                IdDominioPeriodo = periodo.IdDominioPeriodo,
                TipoForma = forma.TipoFormaPagto,
                ValorCalculado = forma.ValorCalculado,
                ValorInformado = forma.ValorConferencia,
                IdUsuarioInclusao = Contexto.IdUsuario,
                DataInclusao = DateTime.Today,
                HoraInclusao = agora,
            });

            var doFechamento = await LanctosDaFormaAsync(forma, origem, destino, cancellationToken);
            if (!doFechamento.IsSuccess)
                return Result<int>.Fail([.. doFechamento.Errors]);
            lanctos.AddRange(doFechamento.Value!);
        }

        // OnBeforeExecutarPersistir (318-330): datas de fechamento e, com ControleCaixaTesouraria, da tesouraria (OP-E2-05).
        var momento = DateTime.Now;
        periodo.DataFechamento = DateTime.Today;
        periodo.HoraFechamento = momento;
        periodo.IdUsuarioFechamento = Contexto.IdUsuario;
        if (await ParametroLigadoAsync(ParametroControleCaixaTesouraria, cancellationToken))
        {
            periodo.DataFechamentoTesouraria = DateTime.Today;
            periodo.HoraFechamentoTesouraria = momento;
        }
        RegistrarAlteracao(periodo);
        await Periodos.SaveChangesAsync(cancellationToken);

        foreach (var lancto in lanctos)
        {
            var gravado = await Lanctos.GravarAsync(lancto, StatusDominioFinanceiro.Fechamento, cancellationToken);
            if (!gravado.IsValid)
                return Result<int>.Fail([.. gravado.Errors]);
        }

        await transacao.CommitAsync(cancellationToken);
        return Result<int>.Ok(periodo.IdDominioPeriodo);
    }

    // CALC-E2-08 — lançamentos de uma forma no fechamento.
    private async Task<Result<IReadOnlyList<DominioPeriodoLacto>>> LanctosDaFormaAsync(FormaFechamentoPeriodo forma, DominioEntidade origem,
        DominioEntidade destino, CancellationToken cancellationToken)
    {
        DominioPeriodoLacto Novo(FormaPagtoTipo tipo, decimal? valor) => new()
        {
            TipoLancto = LanctoDominioPeriodoTipo.Fechamento,
            TipoForma = tipo,
            IdPeriodoOrigem = origem.IdDominioPeriodo,
            IdPeriodoDestino = destino.IdDominioPeriodo,
            Valor = valor,
            DataLancto = DateTime.Now,
        };

        // CarregarChequesFechamento: cheques do período, um lançamento por cheque (lista vazia → nenhum).
        if (forma.TipoFormaPagto == FormaPagtoTipo.ChequeCliente)
        {
            var recebidos = await cheques.ListarChequesRecebidosDoPeriodoAsync(origem.IdDominioPeriodo!.Value, cancellationToken);
            if (!recebidos.IsSuccess)
                return Result<IReadOnlyList<DominioPeriodoLacto>>.Fail([.. recebidos.Errors]);

            return Result<IReadOnlyList<DominioPeriodoLacto>>.Ok([.. recebidos.Value!.Select(c =>
            {
                var l = Novo(FormaPagtoTipo.ChequeCliente, c.Valor);
                l.IdChequeRecebido = c.IdChequeRecebido;
                return l;
            })]);
        }

        if (forma.TipoFormaPagto == FormaPagtoTipo.ChequeEmpresa)
        {
            var folhas = await cheques.ListarFolhasTalaoDoPeriodoAsync(origem.IdDominioPeriodo!.Value, cancellationToken);
            if (!folhas.IsSuccess)
                return Result<IReadOnlyList<DominioPeriodoLacto>>.Fail([.. folhas.Errors]);

            return Result<IReadOnlyList<DominioPeriodoLacto>>.Ok([.. folhas.Value!.Select(c =>
            {
                var l = Novo(FormaPagtoTipo.ChequeEmpresa, c.Valor);
                l.IdCaixaBanco = c.IdCaixaBanco;
                l.Cheque = c.Cheque;
                return l;
            })]);
        }

        // Demais formas: Valor = ValorCalculado (ValorInformado = ValorConferencia não é coluna); dinheiro vai
        // para o caixa do domínio de destino.
        var lancto = Novo(forma.TipoFormaPagto, forma.ValorCalculado);
        if (forma.TipoFormaPagto == FormaPagtoTipo.Dinheiro)
            lancto.IdCaixaBanco = destino.IdCaixaBanco;

        return Result<IReadOnlyList<DominioPeriodoLacto>>.Ok([lancto]);
    }

    private async Task<Result<(DominioEntidade Origem, DominioEntidade Destino, IReadOnlyList<FormaFechamentoPeriodo> Formas)>> ValidarAsync(
        FecharPeriodoCommand comando, CancellationToken cancellationToken)
    {
        Result<(DominioEntidade, DominioEntidade, IReadOnlyList<FormaFechamentoPeriodo>)> Erro(ValidationResult r) => new(false, default, r.Errors);
        Result<(DominioEntidade, DominioEntidade, IReadOnlyList<FormaFechamentoPeriodo>)> ErroMsg(string msg) => Erro(Falha(msg));

        // VAL-E2-49 — pré-verificações do formulário (perfil + período do logado não fechado).
        var perfil = await ValidarPerfilLogadoAsync(cancellationToken);
        if (!perfil.IsValid)
            return Erro(perfil);
        if (await PeriodosAbertos.PeriodoAbertoAsync(null, cancellationToken) == PeriodoStatus.Fechado)
            return ErroMsg(MensagensPeriodo.PeriodoLogadoNaoAberto);

        var logado = await Dominios.ObterDominioDoUsuarioAsync(Contexto.IdUsuario, Contexto.IdFilial, cancellationToken);
        if (logado is null)
            return ErroMsg(string.Format(MensagensPeriodo.LogadoSemDominio, Contexto.IdUsuario));

        // VAL-E2-45 — SetUsuarioCaixaDefault.
        if (await PeriodoFechadoAsync(logado, cancellationToken))
            return ErroMsg(string.Format(MensagensPeriodo.LogadoPeriodoFechado, Contexto.IdUsuario));
        if (!logado.FecharPeriodo && !logado.FecharOutrosPeriodos)
            return ErroMsg(string.Format(MensagensPeriodo.LogadoSemConfiguracaoFechamento, Contexto.IdUsuario));

        if (comando.IdUsuarioCaixa == 0)                                                                                // VAL-E2-47
            return ErroMsg(MensagensPeriodo.InformarUsuarioCaixa);

        var hierarquia = await Dominios.ListarHierarquiaAsync(Contexto.IdFilial, cancellationToken);
        var linha = FiltrarCaixas(hierarquia, logado.IdDominio, logado.FecharPeriodo, logado.FecharOutrosPeriodos)
            .FirstOrDefault(h => h.IdUsuarioRetorno == comando.IdUsuarioCaixa);
        if (linha is null)
            return ErroMsg(string.Format(MensagensPeriodo.CaixaNaoPodeSerFechado, comando.IdUsuarioCaixa, Contexto.IdUsuario));

        var selecionado = (await Dominios.ObterAsync(linha.IdDominioRetorno, Contexto.IdFilial, cancellationToken))!;

        if (await ParametroIntAsync(ParametroDominioPadrao, cancellationToken) == selecionado.IdDominio)          // VAL-E2-43
            return ErroMsg(string.Format(MensagensPeriodo.CaixaNaoPermitido, comando.IdUsuarioCaixa));

        if (selecionado.IdDominioPeriodo is not int idPeriodo)                                                         // CarregarFechamento
            return ErroMsg(string.Format(MensagensPeriodo.CaixaSemPeriodoAberto, comando.IdUsuarioCaixa));

        var fechamento = await fechamentoService.CarregarAsync(idPeriodo, comando.Formas, cancellationToken);
        if (!fechamento.IsSuccess)
            return Erro(ValidationResult.Fail([.. fechamento.Errors]));

        // VAL-E2-47 — o usuário do suprimento é sempre exigido: sem ele o legado quebrava (Q6).
        if (comando.IdUsuarioSuprimento == 0)
            return ErroMsg(MensagensPeriodo.InformarUsuarioSuprimento);

        // VAL-E2-46 — suprimento: usuário de um domínio-pai do caixa (lookup do suprimento da tela).
        if (!await SuprimentoPermitidoAsync(hierarquia, selecionado.IdDominio, comando.IdUsuarioSuprimento, cancellationToken))
            return ErroMsg(MensagensPeriodo.SangriaNaoPermitida);

        var controle = await ValidarControleCaixaBancoAsync(cancellationToken);                                       // VAL-E2-01
        if (!controle.IsValid)
            return Erro(controle);

        // PermiteUsuarioFechamento — VAL-E2-26.
        var config = await ValidarConfigDominioAsync(selecionado, logado.IdDominio, aberturaPadrao: false, cancellationToken);
        if (!config.IsValid)
            return Erro(config);
        if (await PeriodoFechadoAsync(selecionado, cancellationToken))
            return ErroMsg(string.Format(MensagensPeriodo.DominioFechado, selecionado.IdDominio));

        // PermiteUsuarioSuprimentoFechamento — VAL-E2-27.
        var suprimento = await Dominios.ObterDominioDoUsuarioAsync(comando.IdUsuarioSuprimento, Contexto.IdFilial, cancellationToken);
        if (suprimento is null)
            return ErroMsg(MensagensPeriodo.SangriaNaoPermitida);

        var configSuprimento = await ValidarConfigDominioAsync(suprimento, logado.IdDominio, aberturaPadrao: false, cancellationToken);
        if (!configSuprimento.IsValid)
            return Erro(configSuprimento);

        if (await PeriodoFechadoAsync(suprimento, cancellationToken))
        {
            if (comando.IdUsuarioCaixa == Contexto.IdUsuario && await ParametroIntAsync(ParametroDominioPadrao, cancellationToken) == suprimento.IdDominio)
                return ErroMsg($"{MensagensPeriodo.AbrirPadrao}\r\n\r\n{MensagensPeriodo.AberturaPadraoOutraRotina}");

            return ErroMsg(string.Format(MensagensPeriodo.SuprimentoFechamentoFechado, comando.IdUsuarioSuprimento));
        }

        if (comando.IdUsuarioCaixa != Contexto.IdUsuario && comando.IdUsuarioSuprimento != Contexto.IdUsuario)
            return ErroMsg(string.Format(MensagensPeriodo.SuprimentoFechamentoLogado, Contexto.IdUsuario));

        return new(true, (selecionado, suprimento, fechamento.Value!.Formas), []);
    }

    // FDominioFinanceiroFechamento.GetIdUsuarioSangriaDefault (1333-1405): sobe a hierarquia até o primeiro nível
    // com algum domínio-pai de período aberto; o suprimento pode ser qualquer usuário dos pais desse nível.
    private async Task<bool> SuprimentoPermitidoAsync(IReadOnlyList<DominioUsuarioSuprimentoSangria> hierarquia, int idDominio, int idUsuario,
        CancellationToken cancellationToken)
    {
        var chaves = new HashSet<int>();
        await ColetarChavesAsync(hierarquia, idDominio, chaves, [], cancellationToken);
        return hierarquia.Any(h => !h.Caixa && h.IdDominioFilho is int filho && chaves.Contains(filho) && h.IdUsuarioRetorno == idUsuario);
    }

    private async Task<bool> ColetarChavesAsync(IReadOnlyList<DominioUsuarioSuprimentoSangria> hierarquia, int idDominio, HashSet<int> chaves,
        HashSet<int> visitados, CancellationToken cancellationToken)
    {
        if (!visitados.Add(idDominio))
            return false;

        var pais = hierarquia.Where(h => !h.Caixa && h.IdDominioFilho == idDominio).ToList();
        var abertos = new List<DominioUsuarioSuprimentoSangria>();
        var fechados = new List<DominioUsuarioSuprimentoSangria>();
        foreach (var p in pais)
        {
            var dominio = await Dominios.ObterAsync(p.IdDominioRetorno, Contexto.IdFilial, cancellationToken);
            if (dominio?.IdDominioPeriodo is not int idPeriodo)
                continue;

            var periodo = await Dominios.ObterPeriodoAsync(idPeriodo, Contexto.IdFilial, cancellationToken);
            if (periodo is null)
                continue;

            (periodo.DataFechamento is null ? abertos : fechados).Add(p);
        }

        if (abertos.Count > 0)
        {
            chaves.Add(idDominio);
            return true;
        }

        foreach (var f in fechados)
        {
            if (await ColetarChavesAsync(hierarquia, f.IdDominioRetorno, chaves, visitados, cancellationToken))
                return true;
        }

        return false;
    }
}

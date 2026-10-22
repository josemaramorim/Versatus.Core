using System.Globalization;
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

/// <summary>Mensagens do legado das operações de período (acentuação restaurada).</summary>
public static class MensagensPeriodo
{
    // Dominio.cs:1682-1712 (pelo número do item).
    public const string SemAcesso = "Este usuário não tem acesso a esta rotina.";                                                   // item 2  — VAL-E2-03
    public const string AbrirPadrao = "Deve ser efetuado a abertura do domínio padrão.";                                             // item 3
    public const string PeriodoNaoAberto = "Não foi efetuada abertura para o período do domínio ({0})";                            // item 14 — VAL-E2-16/26
    public const string SomentePadrao = "Esta rotina somente pode ser executada pelo(s) usuário(s) do domínio padrão. Verifique."; // item 17 — VAL-E2-24
    public const string DominioFechado = "O domínio ({0}) está fechado.";                                                            // item 18 — VAL-E2-24/26
    public const string ValorMaiorQueSaldo = "O valor informado ({0}) não pode ser maior que o saldo disponível ({1})";             // item 19 — VAL-E2-17/19
    public const string SemDominioPadrao = "Deve ser informado o domínio padrão no cadastro de parâmetros.";                         // item 27 — VAL-E2-04
    public const string AberturaPadraoOutraRotina = "A abertura do domínio padrão deve ser executada em outra rotina.";             // item 28
    public const string DominioInativo = "O domínio financeiro {0} não está ativo.";                                                // item 29 — VAL-E2-02
    public const string SuprimentoValorZero = "O valor do suprimento de dinheiro deve ser maior que zero (0).";                     // item 30 — VAL-E2-17
    public const string SuprimentoSemUsuario = "Deve ser informado o usuário do caixa no qual será efetuado o suprimento.";         // item 31 — VAL-E2-16
    public const string SangriaPeriodoFechado = "O usuário selecionado na sangria da abertura '({0})' deve estar com o domínio período aberto."; // item 39 — VAL-E2-23
    public const string CaixaIgualSangria = "O usuário do caixa deve ser diferente do usuário da sangria da abertura do domínio período.";        // item 41 — VAL-E2-21
    public const string SuprimentoFechamentoFechado = "O usuário selecionado no suprimento do fechamento '({0})' deve estar com o domínio período aberto."; // item 43 — VAL-E2-27
    public const string SuprimentoFechamentoLogado = "O usuário selecionado para suprimento do fechamento deve ser o usuário do domínio periodo logado '({0})'."; // item 44 — VAL-E2-27
    public const string PeriodoJaAberto = "O período para o usuário selecionado '({0})' já está aberto.";                          // Dominio.cs:1501 — VAL-E2-22

    // FDominioFinanceiroAbertura.cs (itens 1-6 e textos fixos) — VAL-E2-43/44/46/47.
    public const string CaixaNaoPermitido = "O usuário '({0})' informado para o caixa não é permitido para este processo.";
    public const string InformarUsuarioCaixa = "Deve ser informado o usuário do caixa.";
    public const string LogadoSemDominio = "O usuário logado ({0}) não possui um dominio financeiro cadastrado. Verifique.";
    public const string SangriaNaoPermitida = "O usuário informado para a sangria não é permitido para este processo.";
    public const string InformarUsuarioSangria = "Deve ser informado o usuário da sangria.";
    public const string CaixaNaoPodeSerAberto = "O usuário '({0})' informado para o caixa não pode ser aberto pelo usuário logado '({1})'.";
    public const string LogadoSemConfiguracaoAbertura = "O usuário logado '({0})' não está configurado para efetuar abertura do período.";
    public const string LogadoPeriodoDeveEstarAberto = "O domínio período do usuário logado '({0})' deve estar aberto. Verifique.";

    // FDominioFinanceiroFechamento.cs (itens 2-8) — VAL-E2-45/47.
    public const string LogadoPeriodoFechado = "O domínio período do usuário logado '({0})' já está fechado.";
    public const string LogadoSemConfiguracaoFechamento = "O usuário logado '({0})' não está configurado para efetuar fechamento do período.";
    public const string CaixaSemPeriodoAberto = "O usuário do caixa selecionado '({0})' não possui período aberto.";
    public const string CaixaNaoPodeSerFechado = "O usuário '({0})' informado para o caixa não pode ser fechado pelo usuário logado '({1})'.";
    public const string InformarUsuarioSuprimento = "Deve ser informado o usuário do suprimento.";

    // FSuprimentoBase.cs — VAL-E2-46/47.
    public const string SuprimentoNaoPermitido = "O usuário '({0})' informado da sangria não pode efetuar o suprimento para o usuário selecionado '({1})'.";
    public const string InformarUsuarioDeSuprimento = "Deve ser informado o usuário de suprimento.";

    // FBase.VerificarPerfilDominioFinanceiro — VAL-E2-49. O texto de ClienteComponenteMensagem.PeriodoNaoAberto
    // vem de recurso em runtime e não está no repositório (DÚVIDA): fica o nome da mensagem.
    public const string PerfilSemDominio = "Para acessar esta rotina, o usuário deve ter perfil que trabalha com domínio financeiro.";
    public const string PeriodoLogadoNaoAberto = "PeriodoNaoAberto";

    // FDominioPadraoAbertura / FDominioPadraoFechamento — VAL-E2-24/25/48.
    public const string PadraoInformarCaixa = "Deve ser informado o caixa do domínio.";
    public const string PadraoInformarPrincipal = "Deve ser informado usuário e marcado como principal.";
    public const string PadraoMarcarPerfil = "Deve ser MARCADO perfil que será utilizado para trabalhar com domínio financeiro.";
    public const string PadraoSomenteUmPerfil = "Deve ser marcado SOMENTE um perfil para trabalhar com domínio financeiro.";
    public const string PadraoInformarDominio = "Deve ser informado um domínio.";
    public const string PadraoInformarDescricao = "Deve ser informado a descrição para criar novo domínio.";
    public const string PadraoSomenteAtivo = "Deve ser informado SOMENTE domínio ativo.";
    public const string PadraoTodosFechados = "Todos os períodos dos domínios devem estar fechados.";
    public const string PadraoDemaisFechados = "Para efetuar este procedimento, os demais períodos devem estar fechados.";
    public const string PadraoUsuariosObrigatorios = "Para o domínio selecionado como padrão '({0})', deve ser informado o(s) usuário(s). Verifique."; // item 49 — VAL-E2-05
    public const string PadraoSemResponsavel = "Para o domínio selecionado como padrão '({0})', não deve ter responsável informado. Verifique."; // item 50 — VAL-E2-06
}

/// <summary>
/// Base dos handlers de período (Application/Handlers/Periodo). Reúne as validações de
/// Dominio.cs usadas por mais de uma operação e a gravação do período/lançamentos na ordem
/// do legado (DominioPeriodo.ExecutarPersistir → PersistirComplementoAbertura).
/// </summary>
public abstract class PeriodoHandlerBase(
    IDominioPeriodoRepository periodos,
    IDominioRepository dominios,
    IPeriodosAbertosService periodosAbertos,
    ICaixaBancoService caixaBancoService,
    IPerfilDominioConsulta perfis,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    DominioPeriodoLactoService lanctos,
    IContextoExecucao contexto)
{
    public const string ParametroTrabalhaComDominio = "TRABALHACOMDOMINIO";
    public const string ParametroDominioPadrao = "DOMINIOPADRAO";
    public const string ParametroControleCaixaTesouraria = "CONTROLECAIXATESOURARIA";
    public const string ParametroChequeEmpresaPorSuprimento = "CHEQUEEMPRESAPORSUPRIMENTO";

    // [AutoSequencial("IdDominioPeriodo", SequencialTipo.Filial)].
    public const string NomeSequencialPeriodo = "DominioPeriodo";

    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    protected IDominioPeriodoRepository Periodos => periodos;
    protected IDominioRepository Dominios => dominios;
    protected IPeriodosAbertosService PeriodosAbertos => periodosAbertos;
    protected IPerfilDominioConsulta Perfis => perfis;
    protected DominioPeriodoLactoService Lanctos => lanctos;
    protected IContextoExecucao Contexto => contexto;

    /// <summary>VAL-E2-01 — DominioPeriodoLacto.Validate → ValidarVinculoCaixaBancoUsuario (= VAL-E3-09).</summary>
    protected async Task<ValidationResult> ValidarControleCaixaBancoAsync(CancellationToken cancellationToken)
        => await caixaBancoService.ValidarControleCaixaBancoAsync(await periodosAbertos.PeriodoAbertoAsync(null, cancellationToken), cancellationToken);

    /// <summary>VAL-E2-49 — FBase.VerificarPerfilDominioFinanceiro: usuário com perfil, mas nenhum com domínio.</summary>
    protected async Task<ValidationResult> ValidarPerfilLogadoAsync(CancellationToken cancellationToken)
    {
        var perfil = await perfis.ObterPerfilUsuarioAsync(contexto.IdUsuario, contexto.IdEmpresa, cancellationToken);
        return perfil is { PossuiPerfil: true, UsaDominioFinanceiro: false } ? Falha(MensagensPeriodo.PerfilSemDominio) : ValidationResult.Ok();
    }

    /// <summary>
    /// VAL-E2-02..04 — Dominio.ValidarConfigDominio (253-277). A mensagem de inativo usa o Id do domínio
    /// "this" (o logado) do legado, não o do domínio validado.
    /// </summary>
    protected async Task<ValidationResult> ValidarConfigDominioAsync(DominioEntidade dominio, int idDominioMensagem, bool aberturaPadrao,
        CancellationToken cancellationToken)
    {
        if (!dominio.Ativo)
            return Falha(string.Format(MensagensPeriodo.DominioInativo, idDominioMensagem));

        if (aberturaPadrao)
            return ValidationResult.Ok();

        if (!await ParametroLigadoAsync(ParametroTrabalhaComDominio, cancellationToken))
            return Falha(MensagensPeriodo.SemAcesso);

        if (await ParametroIntAsync(ParametroDominioPadrao, cancellationToken) == 0)
            return Falha(MensagensPeriodo.SemDominioPadrao);

        return ValidationResult.Ok();
    }

    /// <summary>VAL-E2-19 — Dominio.ValidarSaldoDominio (462-488).</summary>
    protected static ValidationResult ValidarSaldoDominio(decimal valor, DominioEntidade origem, StatusDominioFinanceiro status)
    {
        if (valor == 0)
            return ValidationResult.Ok();

        if (status is StatusDominioFinanceiro.AberturaPadrao or StatusDominioFinanceiro.FechamentoPadrao)
            return ValidationResult.Ok();

        if ((status is StatusDominioFinanceiro.SuprimentoDinheiro or StatusDominioFinanceiro.Abertura) && valor > (origem.SaldoDinheiro ?? 0m))
            return Falha(string.Format(MensagensPeriodo.ValorMaiorQueSaldo, FormatarValor(valor), FormatarValor(origem.SaldoDinheiro ?? 0m)));

        if (status == StatusDominioFinanceiro.SuprimentoCheque && valor > (origem.SaldoChequeRecebido ?? 0m))
            return Falha(string.Format(MensagensPeriodo.ValorMaiorQueSaldo, FormatarValor(valor), FormatarValor(origem.SaldoChequeRecebido ?? 0m)));

        return ValidationResult.Ok();
    }

    /// <summary>Dominio.PeriodoFechado: sem período conta como fechado.</summary>
    protected async Task<bool> PeriodoFechadoAsync(DominioEntidade dominio, CancellationToken cancellationToken)
    {
        if (dominio.IdDominioPeriodo is not int id)
            return true;

        var periodo = await dominios.ObterPeriodoAsync(id, contexto.IdFilial, cancellationToken);
        return periodo?.DataFechamento is not null;
    }

    /// <summary>Novo período do domínio (DominioPeriodo.Assimilar + OnBeforeExecutarPersistir da abertura).</summary>
    protected async Task<DominioPeriodo> CriarPeriodoAsync(int idDominio, CancellationToken cancellationToken)
    {
        var agora = DateTime.Now;
        var periodo = new DominioPeriodo
        {
            IdFilial = contexto.IdFilial,
            IdDominioPeriodo = await geradorSequencial.ProximoAsync(NomeSequencialPeriodo, SequencialTipo.Filial, cancellationToken),
            IdDominio = idDominio,
            DataAbertura = DateTime.Today,
            HoraAbertura = agora,
            IdUsuarioInclusao = contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = agora,
        };

        await periodos.AddAsync(periodo, cancellationToken);
        await periodos.SaveChangesAsync(cancellationToken);
        return periodo;
    }

    /// <summary>Auditoria de alteração do período (OnBeforeExecutarPersistir — período já persistido).</summary>
    protected void RegistrarAlteracao(DominioPeriodo periodo)
    {
        periodo.IdUsuarioAlteracao = contexto.IdUsuario;
        periodo.DataAlteracao = DateTime.Today;
        periodo.HoraAlteracao = DateTime.Now;
    }

    /// <summary>Lançamento em dinheiro do período (DominioPeriodoLacto.Assimilar — 782-806).</summary>
    protected static DominioPeriodoLacto LanctoDinheiro(LanctoDominioPeriodoTipo tipo, int? idOrigem, int? idDestino, int? idCaixaBanco, decimal valor)
        => new()
        {
            TipoLancto = tipo,
            TipoForma = FormaPagtoTipo.Dinheiro,
            IdPeriodoOrigem = idOrigem,
            IdPeriodoDestino = idDestino,
            IdCaixaBanco = idCaixaBanco,
            Valor = valor,
            DataLancto = DateTime.Now,
        };

    /// <summary>
    /// DominioPeriodo.PersistirFinanceiro (421-438) + PersistirLanctoPeriodoPadraoChequeRecebido (443-462): talões
    /// da filial para o período e um lançamento de cheque de cliente por cheque recebido em aberto. A atribuição do
    /// período ao cheque é feita pelo próprio lançamento (DominioPeriodoLacto.PersistirFinanceiro) — o legado fazia
    /// as duas e a última prevalece.
    /// </summary>
    protected async Task<ValidationResult> PersistirChequesAsync(int idDominioPeriodo, LanctoDominioPeriodoTipo tipo, StatusDominioFinanceiro status,
        IChequePeriodoPorta portaCheques, CancellationToken cancellationToken)
    {
        var taloes = await portaCheques.AtribuirPeriodoATodosTaloesAsync(idDominioPeriodo, cancellationToken);
        if (!taloes.IsValid)
            return taloes;

        var recebidos = await portaCheques.ListarChequesRecebidosEmAbertoAsync(cancellationToken);
        if (!recebidos.IsSuccess)
            return ValidationResult.Fail([.. recebidos.Errors]);

        var abertura = tipo == LanctoDominioPeriodoTipo.Abertura;
        foreach (var cheque in recebidos.Value!)
        {
            var lancto = new DominioPeriodoLacto
            {
                TipoLancto = tipo,
                TipoForma = FormaPagtoTipo.ChequeCliente,
                IdPeriodoOrigem = abertura ? null : idDominioPeriodo,
                IdPeriodoDestino = abertura ? idDominioPeriodo : null,
                IdChequeRecebido = cheque.IdChequeRecebido,
                Valor = cheque.Valor,
                DataLancto = DateTime.Now,
            };
            var gravado = await Lanctos.GravarAsync(lancto, status, cancellationToken);
            if (!gravado.IsValid)
                return gravado;
        }

        return ValidationResult.Ok();
    }

    /// <summary>UPDATE FINDOMINIO SET IDFINDOMINIOPERIODO (Dominio.UpdatePeriodoDominio — 493-501).</summary>
    protected async Task AtualizarPonteiroAsync(int idDominio, int idDominioPeriodo, CancellationToken cancellationToken)
    {
        var dominio = await periodos.ObterDominioAsync(idDominio, contexto.IdFilial, cancellationToken);
        if (dominio is null)
            return;

        dominio.IdDominioPeriodo = idDominioPeriodo;
        await periodos.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Linhas da hierarquia do domínio logado com Caixa = 1, filtradas pelas flags (próprio/outros).</summary>
    protected static IEnumerable<DominioUsuarioSuprimentoSangria> FiltrarCaixas(IEnumerable<DominioUsuarioSuprimentoSangria> linhas, int idDominio,
        bool proprio, bool outros)
    {
        var caixas = linhas.Where(h => h.IdDominio == idDominio && h.Caixa);
        if (proprio && !outros)
            return caixas.Where(h => h.IdDominioFilho is null);
        if (!proprio && outros)
            return caixas.Where(h => h.IdDominioFilho is not null);
        return caixas;
    }

    protected async Task<bool> ParametroLigadoAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return bool.TryParse(valor, out var ligado) && ligado;
    }

    protected async Task<int> ParametroIntAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return int.TryParse(valor, out var numero) ? numero : 0;
    }

    /// <summary>Funcoes.FormatarValor — máscara "###,###,###,##0.00".</summary>
    protected static string FormatarValor(decimal valor) => valor.ToString("###,###,###,##0.00", PtBr);

    protected static ValidationResult Falha(string mensagem, string campo = "Dominio") => ValidationResult.Fail(new ValidationError(campo, mensagem));
}

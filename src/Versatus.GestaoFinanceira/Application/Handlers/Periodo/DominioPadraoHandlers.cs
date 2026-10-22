using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Application.Services;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;
using DominioEntidade = Versatus.GestaoFinanceira.Domain.Dominio.Dominio;

namespace Versatus.GestaoFinanceira.Application.Handlers.Periodo;

/// <summary>
/// Abertura do domínio padrão. Com <paramref name="IdDominio"/>, abre um domínio existente (caixa e usuários
/// dele); sem, cria um domínio novo com <paramref name="Descricao"/>, caixa e usuários.
/// <paramref name="IdsPerfis"/>: perfis do usuário principal marcados para trabalhar com domínio (nulo = o
/// único perfil do principal, como a tela marca sozinha).
/// </summary>
public sealed record AbrirDominioPadraoCommand(int? IdDominio, string? Descricao, int IdCaixaBanco,
    IReadOnlyList<DominioUsuarioItemDto>? Usuarios, IReadOnlyList<int>? IdsPerfis);

// Origem: FDominioPadraoAbertura (ValidarAberturaPadrao, Filtrar, ValidarDominio, CarregarPerfilDominio,
// ExecutarAberturaPadrao) → Dominio.AssimilarDominioPadrao (1439), Validate (AberturaPadrao — 130-152),
// PersitirDominioPadrao (1454) → DominioPeriodo.PersistirComplementoAbertura (362-373), PersistirFinanceiro
// (421-438), PersistirLanctoPeriodoPadraoChequeRecebido (443-462), PersistirConfiguracoesDominioPadrao (467-501).
// Cobre: OP-E2-06 · VAL-E2-01, 02, 05, 06, 14, 25, 29, 31, 33, 48.
public class AbrirDominioPadraoHandler(
    IDominioPeriodoRepository periodos,
    IDominioRepository dominios,
    IPeriodosAbertosService periodosAbertos,
    ICaixaBancoService caixaBancoService,
    ICaixaBancoRepository caixaBancoRepository,
    ISaldoCalculadora saldoCalculadora,
    IPerfilDominioConsulta perfis,
    IParametroRepository parametroRepository,
    IParametroFinanceiroEscrita parametroEscrita,
    IChequePeriodoPorta cheques,
    IGeradorSequencial geradorSequencial,
    DominioPeriodoLactoService lanctos,
    IContextoExecucao contexto)
    : PeriodoHandlerBase(periodos, dominios, periodosAbertos, caixaBancoService, perfis, parametroRepository, geradorSequencial, lanctos, contexto)
{
    private readonly IGeradorSequencial _geradorSequencial = geradorSequencial;

    public async Task<Result<int>> ExecutarAsync(AbrirDominioPadraoCommand comando, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(comando);

        var validacao = await ValidarAsync(comando, cancellationToken);
        if (!validacao.IsSuccess)
            return Result<int>.Fail([.. validacao.Errors]);

        var (existente, usuarios, idPerfil) = validacao.Value!;
        var idCaixa = existente?.IdCaixaBanco ?? comando.IdCaixaBanco;

        // Dominio.SetSaldoCaixa: saldo ATUAL do caixa na data do sistema (erro → 0).
        decimal valorAbertura;
        try
        {
            valorAbertura = await saldoCalculadora.RetornarSaldoCaixaBancoAsync(idCaixa, false, DateTime.Today, TipoSaldo.Atual, cancellationToken);
        }
        catch (Exception)
        {
            // try/catch do legado: qualquer falha ao obter o saldo vira 0.
            valorAbertura = 0m;
        }

        await using var transacao = await Periodos.IniciarTransacaoAsync(cancellationToken);

        // AssimilarDominioPadrao + Persist do domínio.
        var dominio = existente is null
            ? await CriarDominioAsync(comando, usuarios, cancellationToken)
            : (await Periodos.ObterDominioAsync(existente.IdDominio, Contexto.IdFilial, cancellationToken))!;
        dominio.AbrirPeriodo = dominio.AbrirOutrosPeriodos = dominio.FecharPeriodo = dominio.FecharOutrosPeriodos = true;
        dominio.MovimentoBanco = dominio.ConsultaTodosPeriodos = true;
        if (existente is not null)
        {
            dominio.IdUsuarioAlteracao = Contexto.IdUsuario;
            dominio.DataAlteracao = DateTime.Today;
            dominio.HoraAlteracao = DateTime.Now;
        }
        await Periodos.SaveChangesAsync(cancellationToken);

        // PersitirDominioPadrao(principal, principal): origem = destino = domínio do usuário principal.
        var principal = dominio.Usuarios.First(u => u.UsuarioPrincipal).IdUsuario;
        var destino = (await Periodos.ObterDominioDoUsuarioAsync(principal, Contexto.IdFilial, cancellationToken))!;

        var periodo = await CriarPeriodoAsync(destino.IdDominio, cancellationToken);

        if (valorAbertura != 0)
        {
            var gravado = await Lanctos.GravarAsync(
                LanctoDinheiro(LanctoDominioPeriodoTipo.Abertura, null, periodo.IdDominioPeriodo, destino.IdCaixaBanco, valorAbertura),
                StatusDominioFinanceiro.AberturaPadrao, cancellationToken);
            if (!gravado.IsValid)
                return Result<int>.Fail([.. gravado.Errors]);
        }

        // PersistirFinanceiro: todos os talões e os cheques recebidos em aberto passam para o período do padrão.
        var financeiro = await PersistirChequesAsync(periodo.IdDominioPeriodo, LanctoDominioPeriodoTipo.Abertura, StatusDominioFinanceiro.AberturaPadrao,
            cheques, cancellationToken);
        if (!financeiro.IsValid)
            return Result<int>.Fail([.. financeiro.Errors]);

        // PersistirConfiguracoesDominioPadrao (abertura).
        await parametroEscrita.GravarAsync(ParametroControleCaixaTesouraria, destino.Tesouraria.ToString(), cancellationToken);
        await parametroEscrita.GravarAsync(ParametroTrabalhaComDominio, bool.TrueString, cancellationToken);
        await parametroEscrita.GravarAsync(ParametroChequeEmpresaPorSuprimento, bool.TrueString, cancellationToken);
        await parametroEscrita.GravarAsync(ParametroDominioPadrao, destino.IdDominio.ToString(), cancellationToken);
        if (idPerfil is int perfil)
            await parametroEscrita.MarcarPerfilUsaDominioAsync(perfil, cancellationToken);

        await AtualizarPonteiroAsync(destino.IdDominio, periodo.IdDominioPeriodo, cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<int>.Ok(periodo.IdDominioPeriodo);
    }

    private async Task<DominioEntidade> CriarDominioAsync(AbrirDominioPadraoCommand comando, IReadOnlyList<DominioUsuarioItemDto> usuarios,
        CancellationToken cancellationToken)
    {
        var agora = DateTime.Now;
        var dominio = new DominioEntidade
        {
            IdFilial = Contexto.IdFilial,
            IdDominio = await _geradorSequencial.ProximoAsync(DominioService.NomeSequencial, SequencialTipo.Filial, cancellationToken),
            Descricao = comando.Descricao!.Trim(),
            IdCaixaBanco = comando.IdCaixaBanco,
            IdUsuarioInclusao = Contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = agora,
        };

        foreach (var item in usuarios)
        {
            dominio.AdicionarUsuario(new DominioUsuario
            {
                IdDominio = dominio.IdDominio,
                IdFilial = dominio.IdFilial,
                IdUsuario = item.IdUsuario,
                UsuarioPrincipal = item.UsuarioPrincipal,
                IdUsuarioInclusao = Contexto.IdUsuario,
                DataInclusao = DateTime.Today,
                HoraInclusao = agora,
            });
        }

        await Periodos.AddDominioAsync(dominio, cancellationToken);
        return dominio;
    }

    private async Task<Result<(DominioEntidade? Existente, IReadOnlyList<DominioUsuarioItemDto> Usuarios, int? IdPerfil)>> ValidarAsync(
        AbrirDominioPadraoCommand comando, CancellationToken cancellationToken)
    {
        Result<(DominioEntidade?, IReadOnlyList<DominioUsuarioItemDto>, int?)> Erro(ValidationResult r) => new(false, default, r.Errors);
        Result<(DominioEntidade?, IReadOnlyList<DominioUsuarioItemDto>, int?)> ErroMsg(string msg) => Erro(Falha(msg));

        DominioEntidade? existente = null;
        IReadOnlyList<DominioUsuarioItemDto> usuarios;

        if (comando.IdDominio is int idDominio)
        {
            // FDominioPadraoAbertura.ValidarDominio (295-327) — VAL-E2-25.
            existente = await Dominios.ObterAsync(idDominio, Contexto.IdFilial, cancellationToken);
            if (existente is null)
                return ErroMsg(MensagensPeriodo.PadraoInformarDominio);
            if (!existente.Ativo)
                return ErroMsg(MensagensPeriodo.PadraoSomenteAtivo);
            if (!await TodosPeriodosFechadosAsync(cancellationToken))
                return ErroMsg(MensagensPeriodo.PadraoTodosFechados);

            usuarios = [.. existente.Usuarios.Select(u => new DominioUsuarioItemDto(u.IdUsuario, u.UsuarioPrincipal, u.IdUsuario))];
        }
        else
        {
            // Filtrar (252-270) — descrição obrigatória para o domínio novo.
            if (string.IsNullOrWhiteSpace(comando.Descricao))
                return ErroMsg(MensagensPeriodo.PadraoInformarDescricao);

            usuarios = comando.Usuarios ?? [];
            if (usuarios.Count > 0)                                                                                   // VAL-E2-34
                usuarios = [usuarios[0] with { UsuarioPrincipal = true }, .. usuarios.Skip(1)];
        }

        var idCaixa = existente?.IdCaixaBanco ?? comando.IdCaixaBanco;
        if (idCaixa == 0)                                                                                             // VAL-E2-48
            return ErroMsg(MensagensPeriodo.PadraoInformarCaixa);

        if (existente is null)
        {
            var caixa = await ValidarCaixaAsync(idCaixa, cancellationToken);                                         // VAL-E2-14
            if (!caixa.IsValid)
                return Erro(caixa);

            if (usuarios.GroupBy(u => u.IdUsuario).Any(g => g.Count() > 1))                                         // VAL-E2-29
                return ErroMsg(DominioService.MsgUsuarioJaInformado);

            foreach (var item in usuarios)
            {
                // DominioUsuario.ValidarUsuario: perfil cadastrado (VAL-E2-31; VAL-E2-32 não roda com
                // SetarSaldoCaixaBanco) e outro domínio ativo (VAL-E2-33).
                var perfil = await Perfis.ObterPerfilUsuarioAsync(item.IdUsuario, Contexto.IdEmpresa, cancellationToken);
                if (perfil is null || !perfil.PossuiPerfil)
                    return ErroMsg(string.Format(DominioService.MsgUsuarioSemPerfil, item.IdUsuario));

                if ((await Dominios.ListarOutrosDominiosAtivosDoUsuarioAsync(item.IdUsuario, Contexto.IdFilial, 0, cancellationToken)).Count > 0)
                    return ErroMsg(string.Format(DominioService.MsgUsuarioOutroDominioAtivo, item.IdUsuario, perfil.Nome));
            }
        }

        // VAL-E2-05 — sem usuários (antes da regra do principal da tela, cuja mensagem é menos precisa).
        if (usuarios.Count == 0)
            return ErroMsg(string.Format(MensagensPeriodo.PadraoUsuariosObrigatorios, existente?.IdDominio ?? 0));

        // ValidarAberturaPadrao — principal e perfil marcado (VAL-E2-48).
        var principais = usuarios.Where(u => u.UsuarioPrincipal).ToList();
        if (principais.Count == 0)
            return ErroMsg(MensagensPeriodo.PadraoInformarPrincipal);
        if (principais.Count > 1)
            return ErroMsg(DominioService.MsgMaisDeUmPrincipal);

        var perfisPrincipal = await Perfis.ListarPerfisDoUsuarioAsync(principais[0].IdUsuario, cancellationToken);
        var marcados = (comando.IdsPerfis ?? (perfisPrincipal.Count == 1 ? perfisPrincipal : [])).Where(perfisPrincipal.Contains).Distinct().ToList();
        if (marcados.Count == 0)
            return ErroMsg(MensagensPeriodo.PadraoMarcarPerfil);
        if (marcados.Count > 1)
            return ErroMsg(MensagensPeriodo.PadraoSomenteUmPerfil);

        var controle = await ValidarControleCaixaBancoAsync(cancellationToken);                                     // VAL-E2-01
        if (!controle.IsValid)
            return Erro(controle);

        // Dominio.ValidarDominio (AberturaPadrao — 130-152): ativo (VAL-E2-02), usuários (05), sem responsáveis (06).
        if (existente is not null)
        {
            var config = await ValidarConfigDominioAsync(existente, existente.IdDominio, aberturaPadrao: true, cancellationToken);
            if (!config.IsValid)
                return Erro(config);
        }

        if (existente is not null && existente.Responsaveis.Count > 0)
            return ErroMsg(string.Format(MensagensPeriodo.PadraoSemResponsavel, existente.IdDominio));

        return new(true, (existente, usuarios, marcados[0]), []);
    }

    private async Task<ValidationResult> ValidarCaixaAsync(int idCaixa, CancellationToken cancellationToken)
    {
        // Dominio.ValidarCaixaBanco (506-524).
        var caixa = await caixaBancoRepository.ObterAsync(idCaixa, Contexto.IdFilial, cancellationToken);
        if (caixa is null)
            return ValidationResult.Ok();
        if (caixa.TipoConta != ContaTipo.Caixa)
            return Falha(DominioService.MsgCaixaNaoCaixa);
        return caixa.TipoContaCaixa == TipoContaCaixa.Normal ? ValidationResult.Ok() : Falha(DominioService.MsgCaixaNaoNormal);
    }

    // Dominio.ValidarDominioPadrao(0, Abertura) — todos os domínios da filial com período fechado.
    private async Task<bool> TodosPeriodosFechadosAsync(CancellationToken cancellationToken)
    {
        foreach (var d in await Periodos.ListarDominiosAsync(Contexto.IdFilial, cancellationToken))
        {
            if (d.IdDominioPeriodo is not int id)
                continue;
            var periodo = await Dominios.ObterPeriodoAsync(id, Contexto.IdFilial, cancellationToken);
            if (periodo is not null && periodo.DataFechamento is null)
                return false;
        }

        return true;
    }
}

// Origem: FDominioPadraoFechamento (Iniciar, ValidarDominio, PersistirFechamento) → Dominio.ValidarDominioPadrao
// (Fechamento — 1395-1434), PersitirDominioPadrao → DominioPeriodo (CarregarListaLancamentos com
// valor = SaldoDinheiro, OnBeforeExecutarPersistir sem tesouraria, PersistirFinanceiro, PersistirConfiguracoesDominioPadrao).
// Cobre: OP-E2-07 · VAL-E2-01, 24, 49.
public class FecharDominioPadraoHandler(
    IDominioPeriodoRepository periodos,
    IDominioRepository dominios,
    IPeriodosAbertosService periodosAbertos,
    ICaixaBancoService caixaBancoService,
    IPerfilDominioConsulta perfis,
    IParametroRepository parametroRepository,
    IParametroFinanceiroEscrita parametroEscrita,
    IChequePeriodoPorta cheques,
    IGeradorSequencial geradorSequencial,
    DominioPeriodoLactoService lanctos,
    IContextoExecucao contexto)
    : PeriodoHandlerBase(periodos, dominios, periodosAbertos, caixaBancoService, perfis, parametroRepository, geradorSequencial, lanctos, contexto)
{
    /// <summary>Devolve o período fechado (0 quando o padrão não tem período — o legado sai sem gravar).</summary>
    public async Task<Result<int>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var idPadrao = await ParametroIntAsync(ParametroDominioPadrao, cancellationToken);
        if (idPadrao == 0)
            return Result<int>.Fail([.. Falha(MensagensPeriodo.AbrirPadrao).Errors]);

        var validacao = await ValidarAsync(idPadrao, cancellationToken);
        if (!validacao.IsValid)
            return Result<int>.Fail([.. validacao.Errors]);

        // PersitirDominioPadrao(logado, logado): origem = domínio do usuário logado (= o padrão, validado acima).
        var origem = (await Dominios.ObterDominioDoUsuarioAsync(Contexto.IdUsuario, Contexto.IdFilial, cancellationToken))!;
        if (origem.IdDominioPeriodo is not int idPeriodo)
            return Result<int>.Ok(0);

        await using var transacao = await Periodos.IniciarTransacaoAsync(cancellationToken);

        var periodo = await Periodos.ObterPeriodoAsync(idPeriodo, Contexto.IdFilial, cancellationToken);
        if (periodo is null)
            return Result<int>.Ok(0);

        // OnBeforeExecutarPersistir (FechamentoPadrao): datas de fechamento, sem tesouraria.
        periodo.DataFechamento = DateTime.Today;
        periodo.HoraFechamento = DateTime.Now;
        periodo.IdUsuarioFechamento = Contexto.IdUsuario;
        RegistrarAlteracao(periodo);
        await Periodos.SaveChangesAsync(cancellationToken);

        // CarregarListaLancamentos: no fechamento do padrão o valor é o SaldoDinheiro do domínio (ignora a tela).
        var saldoDinheiro = origem.SaldoDinheiro ?? 0m;
        if (saldoDinheiro != 0)
        {
            var gravado = await Lanctos.GravarAsync(
                LanctoDinheiro(LanctoDominioPeriodoTipo.Fechamento, idPeriodo, null, origem.IdCaixaBanco, saldoDinheiro),
                StatusDominioFinanceiro.FechamentoPadrao, cancellationToken);
            if (!gravado.IsValid)
                return Result<int>.Fail([.. gravado.Errors]);
        }

        var financeiro = await PersistirChequesAsync(idPeriodo, LanctoDominioPeriodoTipo.Fechamento, StatusDominioFinanceiro.FechamentoPadrao,
            cheques, cancellationToken);
        if (!financeiro.IsValid)
            return Result<int>.Fail([.. financeiro.Errors]);

        // PersistirConfiguracoesDominioPadrao (fechamento): tudo desligado; perfil e ponteiro não mudam.
        await parametroEscrita.GravarAsync(ParametroControleCaixaTesouraria, bool.FalseString, cancellationToken);
        await parametroEscrita.GravarAsync(ParametroTrabalhaComDominio, bool.FalseString, cancellationToken);
        await parametroEscrita.GravarAsync(ParametroChequeEmpresaPorSuprimento, bool.FalseString, cancellationToken);
        await parametroEscrita.GravarAsync(ParametroDominioPadrao, "0", cancellationToken);

        await transacao.CommitAsync(cancellationToken);
        return Result<int>.Ok(idPeriodo);
    }

    private async Task<ValidationResult> ValidarAsync(int idPadrao, CancellationToken cancellationToken)
    {
        var perfil = await ValidarPerfilLogadoAsync(cancellationToken);                                          // VAL-E2-49
        if (!perfil.IsValid)
            return perfil;

        var padrao = await Dominios.ObterAsync(idPadrao, Contexto.IdFilial, cancellationToken);
        if (padrao is null)
            return Falha(MensagensPeriodo.AbrirPadrao);

        // VAL-E2-24 — ValidarDominioPadrao(Fechamento).
        var logado = await Dominios.ObterDominioDoUsuarioAsync(Contexto.IdUsuario, Contexto.IdFilial, cancellationToken);
        if (logado?.IdDominio != idPadrao)
            return Falha(MensagensPeriodo.SomentePadrao);

        if (padrao.IdDominioPeriodo is int idPeriodo)
        {
            var periodo = await Dominios.ObterPeriodoAsync(idPeriodo, Contexto.IdFilial, cancellationToken);
            if (periodo?.DataFechamento is not null)
                return Falha(MensagensPeriodo.DominioFechado);   // "{0}" literal — o legado não formata (Q3).
        }

        foreach (var d in await Periodos.ListarDominiosAsync(Contexto.IdFilial, cancellationToken))
        {
            if (d.IdDominio == idPadrao || d.IdDominioPeriodo is not int id)
                continue;
            var periodo = await Dominios.ObterPeriodoAsync(id, Contexto.IdFilial, cancellationToken);
            if (periodo is not null && periodo.DataFechamento is null)
                return Falha(MensagensPeriodo.PadraoDemaisFechados);
        }

        return await ValidarControleCaixaBancoAsync(cancellationToken);                                            // VAL-E2-01
    }
}

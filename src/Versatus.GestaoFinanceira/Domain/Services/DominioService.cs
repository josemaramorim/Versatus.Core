using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Pagination;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;
using DominioEntidade = Versatus.GestaoFinanceira.Domain.Dominio.Dominio;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/Dominio.cs (Validate, ValidarDominio, ValidarUsuario,
// ValidarDominioResponsavel, PermiteAlterarDominio, ValidarConfigDominio, ValidarUsuarioDominioAtivo,
// ValidarCaixaBanco, IdCaixaBanco.set, Ativo.set, OnBeforeExecutarPersistir, GetUsuarioPadrao,
// GetDataUltimoFechamento), DominioUsuario(+Lista).cs, DominioResponsavel(+Lista).cs,
// DominioPeriodo.cs:RetornarValorAbertura e as telas FDominioFinanceiro (VAL-E2-50) e FCentralDominio
// (OP-E2-17). Tabelas: FINDOMINIO, FINDOMINIOUSUARIO, FINDOMINIORESPONSAVEL.
// Cobre: VAL-E2-01, 07..15, 28..37, 50 · OP-E2-01, 02, 17, 18 (matriz-rtv.md#E2 / matriz-rot.md#E2).
//
// Status do legado: a tela FDominioFinanceiro grava o cadastro sempre como AtualizarDominio (domínio
// comum) ou AtualizarDominioPadrao (domínio padrão) — por isso ValidarUsuario roda sempre e
// ValidarDominioResponsavel (VAL-E2-11..13) roda só para o domínio comum. O bloqueio de campos com
// período aberto (VAL-E2-50) era da tela; vem ao backend pela decisão D1, com a mensagem do item 26
// (VAL-E2-07).
public class DominioService(
    IDominioRepository repository,
    ICaixaBancoRepository caixaBancoRepository,
    ICaixaBancoService caixaBancoService,
    IPeriodosAbertosService periodosAbertos,
    IPerfilDominioConsulta perfilDominioConsulta,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    IContextoExecucao contexto) : IDominioService
{
    public const string ParametroTrabalhaComDominio = "TRABALHACOMDOMINIO";
    public const string ParametroDominioPadrao = "DOMINIOPADRAO";

    // Sequencial do legado: [AutoSequencial("IdDominio", SequencialTipo.Filial)] — GLOSEQUENCIAL.NOMEOBJETO.
    public const string NomeSequencial = "Dominio";

    // Mensagens do legado (Dominio.cs:1682-1712 pelo número do item; acentuação restaurada).
    public const string MsgPeriodoEmAberto = "Este domínio está com período em aberto, logo não pode ser alterado.";            // item 26 — VAL-E2-07/50
    public const string MsgSemUsuarios = "Deve ser informado o(s) usuário(s) do domínio.";                                    // item 34 — VAL-E2-08
    public const string MsgSemPrincipal = "Para o(s) usuário(s) informado(s), deve ser MARCADO um usuário como 'principal'.";  // VAL-E2-09
    public const string MsgMaisDeUmPrincipal = "Para o(s) usuário(s) informado(s), deve conter UM usuário marcado como 'principal'."; // VAL-E2-09
    public const string MsgPadraoComResponsavel = "Não é permitido para o cadastro do domínio padrão informar responsável.";     // item 45 — VAL-E2-10
    public const string MsgSemResponsaveis =
        "Deve ser informado o(s) domínio(s) responsável(is) para ser vinculado ao domínio que está sendo cadastrado.";         // VAL-E2-11
    public const string MsgSemResponsavelPrincipal =
        "Na lista do(s) domínio(s) responsável(is), deve estar marcado um responsável principal. Verifique.";                 // item 48 — VAL-E2-12
    public const string MsgErrosResponsaveis = "Na lista do(s) domínio(s) responsável(is) deve ser corrigido o(s) seguinte(s) erro(s):\r\n{0}"; // item 46 — VAL-E2-13
    public const string MsgResponsavelIgualCadastro = "O domínio informado '{0}' deve ser diferente do cadastro ({1}).";     // VAL-E2-13
    public const string MsgResponsavelCiclico =
        "Não é permitido informar o domínio '{0}', pois o mesmo possui como responsável o domínio do cadastro ({1}).";         // VAL-E2-13
    public const string MsgCaixaNaoCaixa = "Deve ser informado caixa do tipo 'Caixa'.";                                      // VAL-E2-14
    public const string MsgCaixaNaoNormal = "Deve ser informado caixa do tipo 'Normal'.";                                    // VAL-E2-14
    public const string MsgAtivarUsuarioOutroDominio =
        "Para ativar o domínio '{0}', verificar o(s) usuário(s) abaixo, pois está(ão) vinculado a outro domínio:\r\n\r\n{1}"; // VAL-E2-15
    public const string MsgLinhaUsuarioOutroDominio = "Usuário: {0} - {1}     Domínio ativo: {2}";                         // VAL-E2-15
    public const string MsgSemDominioPadrao = "Deve estar configurado o domínio padrão no cadastro de 'parâmetros'.";        // item 33 — VAL-E2-28
    public const string MsgUsuarioJaInformado = "Este usuário já foi informado.";                                           // VAL-E2-29
    public const string MsgAlterarUsuarioSalvo = "NÃO é permitido alterar usuário após ter sido salvo.";                    // VAL-E2-30
    public const string MsgUsuarioSemPerfil = "O usuário informado '{0}' NÃO possui perfil cadastrado. Verifique.";          // VAL-E2-31
    public const string MsgUsuarioSemPerfilDominio =
        "O usuário informado '{0}' NÃO possui perfil indicando que USA domínio financeiro. Verifique.";                       // VAL-E2-32
    // VAL-E2-33: o legado lança ObjetoNegocioMensagem.UsuarioInvalido com o texto "({id} - {nome})"; o texto
    // do enum vem do LanguageManager em runtime e não está no repositório — fica só a parte conhecida.
    public const string MsgUsuarioOutroDominioAtivo = "({0} - {1})";                                                        // VAL-E2-33
    public const string MsgDominioJaInformado = "Este domínio já foi informado.";                                           // VAL-E2-35
    public const string MsgAlterarDominioSalvo = "NÃO é permitido alterar domínio após ter sido salvo.";                    // VAL-E2-36
    public const string MsgNaoEncontrado = "Domínio não encontrado.";
    // OP-E2-02: no legado a exclusão era recusada pela FK do banco (sem mensagem própria).
    public const string MsgExclusaoComVinculos =
        "O domínio '{0}' possui período(s) ou é responsável por outro(s) domínio(s) e não pode ser excluído.";

    // ---- Consultas (ReadConnection) ------------------------------------------------------

    public async Task<PagedResult<DominioListaDto>> ListarPaginadoAsync(FiltroDominioDto filtro, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        // Artigo VII.5 — materializa antes de paginar (SQL Server 2008).
        var todos = await repository.ListarAsync(contexto.IdFilial, filtro.Texto, filtro.Ativo, cancellationToken);
        var pagina = Math.Max(filtro.Page, 1);
        var tamanho = Math.Max(filtro.Limit, 1);

        return new PagedResult<DominioListaDto>(
            [.. todos.Skip((pagina - 1) * tamanho).Take(tamanho)
                .Select(d => new DominioListaDto(d.IdDominio, d.IdFilial, d.Descricao, d.IdCaixaBanco, d.Ativo, d.IdDominioPeriodo))],
            todos.Count);
    }

    public async Task<DominioDto?> ObterPorIdAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
    {
        var dominio = await repository.ObterAsync(idDominio, idFilial, cancellationToken);
        return dominio is null ? null : ParaDto(dominio);
    }

    public async Task<DominioDto?> ObterDominioDoUsuarioAsync(int? idUsuario = null, CancellationToken cancellationToken = default)
    {
        var dominio = await repository.ObterDominioDoUsuarioAsync(idUsuario ?? contexto.IdUsuario, contexto.IdFilial, cancellationToken);
        return dominio is null ? null : ParaDto(dominio);
    }

    public async Task<Result<int?>> ObterUsuarioPadraoAsync(CancellationToken cancellationToken = default)
    {
        // Dominio.cs:533-549.
        var idDominioPadrao = await ParametroIntAsync(ParametroDominioPadrao, cancellationToken);
        if (idDominioPadrao == 0)
            return Result<int?>.Fail(new ValidationError("Parametro", MsgSemDominioPadrao));

        var padrao = await repository.ObterAsync(idDominioPadrao, contexto.IdFilial, cancellationToken);
        return Result<int?>.Ok(padrao?.Usuarios.FirstOrDefault(u => u.UsuarioPrincipal)?.IdUsuario);
    }

    public async Task<UltimoFechamentoDto?> ObterUltimoFechamentoAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
    {
        // Dominio.cs:1545-1571 — período corrente fechado, senão o último período fechado do domínio.
        var dominio = await repository.ObterAsync(idDominio, idFilial, cancellationToken);
        if (dominio is null)
            return null;

        if (dominio.IdDominioPeriodo is int idPeriodo)
        {
            var atual = await repository.ObterPeriodoAsync(idPeriodo, idFilial, cancellationToken);
            if (atual?.DataFechamento is DateTime fechamento)
                return new UltimoFechamentoDto(fechamento, atual.HoraFechamento);
        }

        var ultimo = await repository.ObterUltimoPeriodoFechadoAsync(idDominio, idFilial, cancellationToken);
        return ultimo?.DataFechamento is DateTime data ? new UltimoFechamentoDto(data, ultimo.HoraFechamento) : null;
    }

    public async Task<decimal> ObterValorAberturaAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
    {
        // DominioPeriodo.cs:956-967 — sem lançamento de abertura, 0.
        var lancto = await repository.ObterLanctoAberturaAsync(idDominioPeriodo, contexto.IdFilial, cancellationToken);
        return lancto?.Valor ?? 0m;
    }

    // ---- Persistência (WriteConnection) --------------------------------------------------

    public async Task<Result<DominioDto>> CriarAsync(SalvarDominioDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var usuarios = dto.Usuarios ?? [];
        var responsaveis = dto.Responsaveis ?? [];

        var validacao = await ValidarAsync(null, dto, usuarios, responsaveis, cancellationToken);
        if (!validacao.IsValid)
            return Result<DominioDto>.Fail([.. validacao.Errors]);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        var dominio = new DominioEntidade
        {
            IdFilial = contexto.IdFilial,
            IdDominio = await geradorSequencial.ProximoAsync(NomeSequencial, SequencialTipo.Filial, cancellationToken),
        };
        CopiarCampos(dto, dominio);
        RegistrarInclusao(dominio);

        foreach (var item in AplicarPrincipalPadrao(usuarios, []))
            dominio.AdicionarUsuario(NovoUsuario(dominio, item));

        foreach (var item in AplicarPrincipalPadrao(responsaveis, []))
            dominio.AdicionarResponsavel(NovoResponsavel(dominio, item));

        await repository.AddAsync(dominio, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<DominioDto>.Ok((await ObterPorIdAsync(dominio.IdDominio, dominio.IdFilial, cancellationToken))!);
    }

    public async Task<Result<DominioDto>> AtualizarAsync(int idDominio, int idFilial, SalvarDominioDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existente = await repository.ObterParaEdicaoAsync(idDominio, idFilial, cancellationToken);
        if (existente is null)
            return Result<DominioDto>.Fail(new ValidationError(nameof(DominioEntidade.IdDominio), MsgNaoEncontrado));

        var usuarios = dto.Usuarios ?? [];
        var responsaveis = dto.Responsaveis ?? [];

        var validacao = await ValidarAsync(existente, dto, usuarios, responsaveis, cancellationToken);
        if (!validacao.IsValid)
            return Result<DominioDto>.Fail([.. validacao.Errors]);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        CopiarCampos(dto, existente);
        RegistrarAlteracao(existente);
        SincronizarUsuarios(existente, usuarios);
        SincronizarResponsaveis(existente, responsaveis);

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<DominioDto>.Ok((await ObterPorIdAsync(existente.IdDominio, existente.IdFilial, cancellationToken))!);
    }

    public async Task<ValidationResult> ExcluirAsync(int idDominio, int idFilial, CancellationToken cancellationToken = default)
    {
        var existente = await repository.ObterParaEdicaoAsync(idDominio, idFilial, cancellationToken);
        if (existente is null)
            return ValidationResult.Fail(new ValidationError(nameof(DominioEntidade.IdDominio), MsgNaoEncontrado));

        // OP-E2-02 — FK_FINDOMINIOPERIODO_FINDOMINIO e FK_FINDOMINIORESPONSAVEL_FINDOMINIO_PAI recusariam a exclusão.
        if (await repository.PossuiPeriodosAsync(idDominio, idFilial, cancellationToken)
            || (await repository.ListarFilhosAsync(idDominio, idFilial, cancellationToken)).Count > 0)
            return ValidationResult.Fail(new ValidationError(nameof(DominioEntidade.IdDominio), string.Format(MsgExclusaoComVinculos, idDominio)));

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        // ObjectMaster.ExecutarExcluir: listas primeiro, depois o registro.
        foreach (var usuario in existente.Usuarios.ToList())
            repository.RemoverUsuario(usuario);
        foreach (var responsavel in existente.Responsaveis.ToList())
            repository.RemoverResponsavel(responsavel);

        await repository.DeleteAsync(existente, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return ValidationResult.Ok();
    }

    // ---- Validações ------------------------------------------------------------------------

    private async Task<ValidationResult> ValidarAsync(DominioEntidade? existente, SalvarDominioDto dto,
        IReadOnlyList<DominioUsuarioItemDto> usuarios, IReadOnlyList<DominioResponsavelItemDto> responsaveis,
        CancellationToken cancellationToken)
    {
        // VAL-E2-01 — Dominio.Validate → ValidarVinculoCaixaBancoUsuario = VAL-E3-09.
        var controle = await caixaBancoService.ValidarControleCaixaBancoAsync(
            await periodosAbertos.PeriodoAbertoAsync(null, cancellationToken), cancellationToken);
        if (!controle.IsValid)
            return controle;

        var idDominioPadrao = await ParametroIntAsync(ParametroDominioPadrao, cancellationToken);
        var ehPadrao = existente is not null && idDominioPadrao > 0 && idDominioPadrao == existente.IdDominio;

        // VAL-E2-50 / VAL-E2-07 — com período aberto só mudam os campos que a tela libera.
        if (existente is not null && await PeriodoAbertoAsync(existente, cancellationToken)
            && AlterouCampoBloqueado(existente, dto, responsaveis, ehPadrao))
            return Falha(nameof(DominioEntidade.IdDominio), MsgPeriodoEmAberto);

        // VAL-E2-14 — IdCaixaBanco.set (só quando muda).
        if (existente is null || existente.IdCaixaBanco != dto.IdCaixaBanco)
        {
            var caixa = await ValidarCaixaAsync(dto.IdCaixaBanco, cancellationToken);
            if (!caixa.IsValid)
                return caixa;
        }

        var salvos = existente?.Usuarios.Select(u => u.IdUsuario).ToHashSet() ?? [];

        // VAL-E2-15 — Ativo.set: reativar domínio persistido.
        if (existente is not null && !existente.Ativo && dto.Ativo && usuarios.Count > 0)
        {
            var ativacao = await ValidarAtivacaoAsync(existente.IdDominio, usuarios, cancellationToken);
            if (!ativacao.IsValid)
                return ativacao;
        }

        // VAL-E2-30 / VAL-E2-29 / VAL-E2-31..33 — linhas de usuário (DominioUsuario.IdUsuario.set).
        var linhasUsuario = await ValidarLinhasUsuarioAsync(existente?.IdDominio ?? 0, usuarios, salvos, cancellationToken);
        if (!linhasUsuario.IsValid)
            return linhasUsuario;

        // VAL-E2-36 / VAL-E2-35 — linhas de responsável (DominioResponsavel.IdDominioPai.set).
        var paisSalvos = existente?.Responsaveis.Select(r => r.IdDominioPai).ToHashSet() ?? [];
        if (responsaveis.Any(r => r.IdDominioPaiSalvo is { } anterior && anterior != 0 && anterior != r.IdDominioPai && paisSalvos.Contains(anterior)))
            return Falha(nameof(DominioResponsavel.IdDominioPai), MsgAlterarDominioSalvo);
        if (responsaveis.GroupBy(r => r.IdDominioPai).Any(g => g.Count() > 1))
            return Falha(nameof(DominioResponsavel.IdDominioPai), MsgDominioJaInformado);

        // VAL-E2-34 / VAL-E2-37 — efeito do Add na lista vazia antes das regras de principal.
        var usuariosEfetivos = AplicarPrincipalPadrao(usuarios, salvos);
        var responsaveisEfetivos = AplicarPrincipalPadrao(responsaveis, paisSalvos);

        // VAL-E2-08 / VAL-E2-09 — ValidarUsuario (roda nos dois status da tela).
        var usuario = await ValidarUsuariosAsync(usuariosEfetivos, cancellationToken);
        if (!usuario.IsValid)
            return usuario;

        // VAL-E2-10..13 — ValidarDominioResponsavel.
        return ehPadrao
            ? (responsaveisEfetivos.Count > 0 ? Falha(nameof(DominioEntidade.Responsaveis), MsgPadraoComResponsavel) : ValidationResult.Ok())
            : await ValidarResponsaveisAsync(existente, responsaveisEfetivos, cancellationToken);
    }

    private async Task<bool> PeriodoAbertoAsync(DominioEntidade dominio, CancellationToken cancellationToken)
    {
        // FDominioFinanceiro.binderControl_BinderEnableChanging: ServidorFactory.PeriodoAberto(IdDominioPeriodo).
        if (dominio.IdDominioPeriodo is not int idPeriodo)
            return false;

        return (await periodosAbertos.ObterInfoPeriodoAsync(idPeriodo, cancellationToken)).Status == PeriodoStatus.Aberto;
    }

    private static bool AlterouCampoBloqueado(DominioEntidade existente, SalvarDominioDto dto,
        IReadOnlyList<DominioResponsavelItemDto> responsaveis, bool ehPadrao)
    {
        // Sempre bloqueados com período aberto.
        if (existente.Descricao != dto.Descricao || existente.IdCaixaBanco != dto.IdCaixaBanco
            || existente.Ativo != dto.Ativo || existente.Tesouraria != dto.Tesouraria)
            return true;

        if (!ehPadrao)
            return false;

        // Domínio padrão: só os usuários podem mudar.
        var responsaveisAtuais = existente.Responsaveis.Select(r => (r.IdDominioPai, r.DominioPaiPrincipal)).OrderBy(r => r.IdDominioPai);
        var responsaveisNovos = responsaveis.Select(r => (r.IdDominioPai, r.DominioPaiPrincipal)).OrderBy(r => r.IdDominioPai);

        return existente.AbrirPeriodo != dto.AbrirPeriodo || existente.AbrirOutrosPeriodos != dto.AbrirOutrosPeriodos
            || existente.FecharPeriodo != dto.FecharPeriodo || existente.FecharOutrosPeriodos != dto.FecharOutrosPeriodos
            || existente.MovimentoBanco != dto.MovimentoBanco || existente.ConsultaTodosPeriodos != dto.ConsultaTodosPeriodos
            || !responsaveisAtuais.SequenceEqual(responsaveisNovos);
    }

    private async Task<ValidationResult> ValidarCaixaAsync(int idCaixaBanco, CancellationToken cancellationToken)
    {
        // Dominio.cs:506-524 — caixa inexistente passa (CaixaBanco == null → sem mensagem); a FK recusa ao gravar.
        var caixa = await caixaBancoRepository.ObterAsync(idCaixaBanco, contexto.IdFilial, cancellationToken);
        if (caixa is null)
            return ValidationResult.Ok();

        if (caixa.TipoConta != ContaTipo.Caixa)
            return Falha(nameof(DominioEntidade.IdCaixaBanco), MsgCaixaNaoCaixa);

        return caixa.TipoContaCaixa == TipoContaCaixa.Normal
            ? ValidationResult.Ok()
            : Falha(nameof(DominioEntidade.IdCaixaBanco), MsgCaixaNaoNormal);
    }

    private async Task<ValidationResult> ValidarAtivacaoAsync(int idDominio, IReadOnlyList<DominioUsuarioItemDto> usuarios,
        CancellationToken cancellationToken)
    {
        // Dominio.cs:282-326 — linhas concatenadas sem separador, como no legado.
        var msg = "";
        foreach (var item in usuarios)
        {
            var outros = await repository.ListarOutrosDominiosAtivosDoUsuarioAsync(item.IdUsuario, contexto.IdFilial, idDominio, cancellationToken);
            if (outros.Count == 0)
                continue;

            var nome = (await perfilDominioConsulta.ObterPerfilUsuarioAsync(item.IdUsuario, contexto.IdEmpresa, cancellationToken))?.Nome ?? "";
            foreach (var outro in outros)
                msg += string.Format(MsgLinhaUsuarioOutroDominio, item.IdUsuario, nome, outro);
        }

        return msg.Length == 0
            ? ValidationResult.Ok()
            : Falha(nameof(DominioEntidade.Ativo), string.Format(MsgAtivarUsuarioOutroDominio, idDominio, msg));
    }

    private async Task<ValidationResult> ValidarLinhasUsuarioAsync(int idDominio, IReadOnlyList<DominioUsuarioItemDto> usuarios,
        HashSet<int> salvos, CancellationToken cancellationToken)
    {
        if (usuarios.Any(i => i.IdUsuarioSalvo is { } anterior && anterior != 0 && anterior != i.IdUsuario && salvos.Contains(anterior)))
            return Falha(nameof(DominioUsuario.IdUsuario), MsgAlterarUsuarioSalvo);

        if (usuarios.GroupBy(i => i.IdUsuario).Any(g => g.Count() > 1))
            return Falha(nameof(DominioUsuario.IdUsuario), MsgUsuarioJaInformado);

        // DominioUsuario.ValidarUsuario: só quando o usuário da linha é definido (linha nova).
        foreach (var item in usuarios.Where(i => i.IdUsuario != 0 && !salvos.Contains(i.IdUsuario)))
        {
            var perfil = await perfilDominioConsulta.ObterPerfilUsuarioAsync(item.IdUsuario, contexto.IdEmpresa, cancellationToken);
            var erroPerfil = ValidarPerfil(item.IdUsuario, perfil);
            if (!erroPerfil.IsValid)
                return erroPerfil;

            // VAL-E2-33 — outro domínio ATIVO da filial com o mesmo usuário.
            if ((await repository.ListarOutrosDominiosAtivosDoUsuarioAsync(item.IdUsuario, contexto.IdFilial, idDominio, cancellationToken)).Count > 0)
                return Falha(nameof(DominioUsuario.IdUsuario), string.Format(MsgUsuarioOutroDominioAtivo, item.IdUsuario, perfil?.Nome ?? ""));
        }

        return ValidationResult.Ok();
    }

    private async Task<ValidationResult> ValidarUsuariosAsync(IReadOnlyList<DominioUsuarioItemDto> usuarios, CancellationToken cancellationToken)
    {
        // Dominio.cs:162-176 + DominioUsuarioLista.GetValidacaoUsuarioDominio (83-105).
        if (usuarios.Count == 0)
            return Falha(nameof(DominioEntidade.Usuarios), MsgSemUsuarios);

        var principais = usuarios.Where(u => u.UsuarioPrincipal).ToList();
        if (principais.Count == 0)
            return Falha(nameof(DominioEntidade.Usuarios), MsgSemPrincipal);
        if (principais.Count > 1)
            return Falha(nameof(DominioEntidade.Usuarios), MsgMaisDeUmPrincipal);

        var idPrincipal = principais[0].IdUsuario;
        return ValidarPerfil(idPrincipal, await perfilDominioConsulta.ObterPerfilUsuarioAsync(idPrincipal, contexto.IdEmpresa, cancellationToken));
    }

    private static ValidationResult ValidarPerfil(int idUsuario, PerfilUsuarioDominioInfo? perfil)
    {
        // DominioUsuario.GetValidacaoPerfilUsuario (478-503); SetarSaldoCaixaBanco = false no cadastro.
        if (perfil is null || !perfil.PossuiPerfil)
            return Falha(nameof(DominioUsuario.IdUsuario), string.Format(MsgUsuarioSemPerfil, idUsuario));

        return perfil.UsaDominioFinanceiro
            ? ValidationResult.Ok()
            : Falha(nameof(DominioUsuario.IdUsuario), string.Format(MsgUsuarioSemPerfilDominio, idUsuario));
    }

    private async Task<ValidationResult> ValidarResponsaveisAsync(DominioEntidade? existente, IReadOnlyList<DominioResponsavelItemDto> responsaveis,
        CancellationToken cancellationToken)
    {
        // Dominio.cs:181-219 (fora do domínio padrão).
        if (!await ParametroLigadoAsync(ParametroTrabalhaComDominio, cancellationToken))
            return ValidationResult.Ok();

        if (responsaveis.Count == 0)
            return Falha(nameof(DominioEntidade.Responsaveis), MsgSemResponsaveis);

        if (!responsaveis.Any(r => r.DominioPaiPrincipal))
            return Falha(nameof(DominioEntidade.Responsaveis), MsgSemResponsavelPrincipal);

        if (existente is null)
            return ValidationResult.Ok();

        // DominioResponsavelLista.ValidarResponsavel (101-126) — só com o domínio persistido.
        var msg = "";
        for (var i = 0; i < responsaveis.Count; i++)
        {
            var idPai = responsaveis[i].IdDominioPai;
            string? erro = null;
            if (idPai == existente.IdDominio)
                erro = string.Format(MsgResponsavelIgualCadastro, idPai, existente.IdDominio);
            else if (await repository.PossuiResponsavelAsync(idPai, existente.IdDominio, existente.IdFilial, cancellationToken))
                erro = string.Format(MsgResponsavelCiclico, idPai, existente.IdDominio);

            if (erro is not null)
                msg += $"Linha {i + 1}: {erro}\r\n";
        }

        return msg.Length == 0
            ? ValidationResult.Ok()
            : Falha(nameof(DominioEntidade.Responsaveis), string.Format(MsgErrosResponsaveis, msg));
    }

    // VAL-E2-34 — DominioUsuarioLista.Add (28-40): o 1º usuário NOVO numa lista vazia vira principal.
    private static IReadOnlyList<DominioUsuarioItemDto> AplicarPrincipalPadrao(IReadOnlyList<DominioUsuarioItemDto> usuarios, HashSet<int> salvos)
        => salvos.Count == 0 && usuarios.Count > 0
            ? [usuarios[0] with { UsuarioPrincipal = true }, .. usuarios.Skip(1)]
            : usuarios;

    // VAL-E2-37 — DominioResponsavelLista.Add (29-40): o 1º responsável numa lista vazia vira principal.
    private static IReadOnlyList<DominioResponsavelItemDto> AplicarPrincipalPadrao(IReadOnlyList<DominioResponsavelItemDto> responsaveis, HashSet<int> salvos)
        => salvos.Count == 0 && responsaveis.Count > 0
            ? [responsaveis[0] with { DominioPaiPrincipal = true }, .. responsaveis.Skip(1)]
            : responsaveis;

    // ---- Central do domínio (OP-E2-17) -----------------------------------------------------

    public async Task<CentralDominioDto> ObterCentralAsync(CancellationToken cancellationToken = default)
    {
        var logado = await repository.ObterDominioDoUsuarioAsync(contexto.IdUsuario, contexto.IdFilial, cancellationToken);
        if (logado is null)
            return new CentralDominioDto(false, false, [], [], []);

        var podeAbrir = logado.AbrirPeriodo || logado.AbrirOutrosPeriodos;
        var podeFechar = logado.FecharPeriodo || logado.FecharOutrosPeriodos;

        var hierarquia = (await repository.ListarHierarquiaAsync(contexto.IdFilial, cancellationToken))
            .Where(h => h.IdDominio == logado.IdDominio && h.Caixa)
            .ToList();

        var dominios = (await repository.ListarPorIdsAsync([.. hierarquia.Select(h => h.IdDominioRetorno).Append(logado.IdDominio).Distinct()],
            contexto.IdFilial, cancellationToken)).ToDictionary(d => d.IdDominio);
        var periodos = await CarregarPeriodosAsync(dominios.Values, cancellationToken);

        // FCentralDominio.CarregarAberturaDominio (361-398): caixas sem período ou com período fechado.
        var abrir = new List<CentralDominioItemDto>();
        if (podeAbrir)
        {
            foreach (var h in FiltrarHierarquia(hierarquia, logado.AbrirPeriodo, logado.AbrirOutrosPeriodos))
            {
                var d = dominios[h.IdDominioRetorno];
                if (d.IdDominioPeriodo is null || PeriodoFechado(d, periodos))
                    abrir.Add(Item(null, h.IdUsuarioRetorno, d, null, saldos: false));
            }
        }

        // FCentralDominio.CarregarFechamentoDominio (403-443): caixas com período não fechado.
        var fechar = new List<CentralDominioItemDto>();
        if (podeFechar)
        {
            foreach (var h in FiltrarHierarquia(hierarquia, logado.FecharPeriodo, logado.FecharOutrosPeriodos))
            {
                var d = dominios[h.IdDominioRetorno];
                if (!PeriodoFechado(d, periodos))
                    fechar.Add(Item(null, h.IdUsuarioRetorno, d, Periodo(d, periodos), saldos: true));
            }
        }

        // FCentralDominio.CarregarConsultaDominio / AddDominioConsulta (448-493): o próprio domínio e os filhos ativos.
        var consulta = new List<CentralDominioItemDto>
        {
            Item(null, logado.Usuarios.FirstOrDefault(u => u.UsuarioPrincipal)?.IdUsuario ?? 0, logado, Periodo(logado, periodos), saldos: true),
        };
        await AdicionarFilhosAsync(consulta, logado.IdDominio, logado.ConsultaTodosPeriodos, [logado.IdDominio], cancellationToken);

        return new CentralDominioDto(podeAbrir, podeFechar, abrir, fechar, consulta);
    }

    private async Task AdicionarFilhosAsync(List<CentralDominioItemDto> consulta, int idDominioPai, bool consultaFilhos, HashSet<int> visitados,
        CancellationToken cancellationToken)
    {
        foreach (var responsavel in await repository.ListarFilhosAsync(idDominioPai, contexto.IdFilial, cancellationToken))
        {
            // O legado não tinha proteção contra hierarquia circular (laço infinito); aqui cada domínio entra uma vez.
            if (!visitados.Add(responsavel.IdDominio))
                continue;

            var filho = await repository.ObterAsync(responsavel.IdDominio, contexto.IdFilial, cancellationToken);
            if (filho is null || !filho.Ativo)
                continue;

            var periodos = await CarregarPeriodosAsync([filho], cancellationToken);
            consulta.Add(Item(responsavel.IdDominioPai, filho.Usuarios.FirstOrDefault(u => u.UsuarioPrincipal)?.IdUsuario ?? 0, filho,
                Periodo(filho, periodos), saldos: true));

            if (consultaFilhos)
                await AdicionarFilhosAsync(consulta, filho.IdDominio, consultaFilhos, visitados, cancellationToken);
        }
    }

    private static IEnumerable<DominioUsuarioSuprimentoSangria> FiltrarHierarquia(IEnumerable<DominioUsuarioSuprimentoSangria> linhas,
        bool proprio, bool outros)
    {
        // Só o próprio → IdDominioFilho IS NULL; só os outros → IS NOT NULL; os dois → tudo.
        if (proprio && !outros)
            return linhas.Where(h => h.IdDominioFilho is null);
        if (!proprio && outros)
            return linhas.Where(h => h.IdDominioFilho is not null);
        return linhas;
    }

    private async Task<Dictionary<int, DominioPeriodo>> CarregarPeriodosAsync(IEnumerable<DominioEntidade> dominios, CancellationToken cancellationToken)
    {
        var ids = dominios.Where(d => d.IdDominioPeriodo is not null).Select(d => d.IdDominioPeriodo!.Value).Distinct().ToList();
        return ids.Count == 0
            ? []
            : (await repository.ListarPeriodosAsync(ids, contexto.IdFilial, cancellationToken)).ToDictionary(p => p.IdDominioPeriodo);
    }

    private static DominioPeriodo? Periodo(DominioEntidade dominio, Dictionary<int, DominioPeriodo> periodos)
        => dominio.IdDominioPeriodo is int id && periodos.TryGetValue(id, out var p) ? p : null;

    // Dominio.PeriodoFechado (1059-1068): sem período conta como fechado.
    private static bool PeriodoFechado(DominioEntidade dominio, Dictionary<int, DominioPeriodo> periodos)
        => dominio.IdDominioPeriodo is null || Periodo(dominio, periodos)?.DataFechamento is not null;

    private static CentralDominioItemDto Item(int? idDominioPai, int idUsuario, DominioEntidade d, DominioPeriodo? p, bool saldos) => new(
        idDominioPai, idUsuario, d.IdDominio, $"{d.IdDominio} - {d.Descricao}", d.IdCaixaBanco, p?.IdDominioPeriodo ?? 0,
        p?.DataAbertura, p?.HoraAbertura, p?.DataFechamento, p?.HoraFechamento,
        saldos ? d.SaldoDinheiro : null, saldos ? d.SaldoChequeRecebido : null, saldos ? d.SaldoCartao : null, saldos ? d.Saldo : null);

    // ---- Apoio -----------------------------------------------------------------------------

    private static void CopiarCampos(SalvarDominioDto dto, DominioEntidade dominio)
    {
        dominio.Descricao = dto.Descricao;
        dominio.IdCaixaBanco = dto.IdCaixaBanco;
        dominio.Ativo = dto.Ativo;
        dominio.Tesouraria = dto.Tesouraria;
        dominio.AbrirPeriodo = dto.AbrirPeriodo;
        dominio.AbrirOutrosPeriodos = dto.AbrirOutrosPeriodos;
        dominio.FecharPeriodo = dto.FecharPeriodo;
        dominio.FecharOutrosPeriodos = dto.FecharOutrosPeriodos;
        dominio.MovimentoBanco = dto.MovimentoBanco;
        dominio.ConsultaTodosPeriodos = dto.ConsultaTodosPeriodos;
    }

    private void SincronizarUsuarios(DominioEntidade dominio, IReadOnlyList<DominioUsuarioItemDto> itens)
    {
        var efetivos = AplicarPrincipalPadrao(itens, dominio.Usuarios.Select(u => u.IdUsuario).ToHashSet());
        var porUsuario = efetivos.ToDictionary(i => i.IdUsuario);

        foreach (var usuario in dominio.Usuarios.Where(u => !porUsuario.ContainsKey(u.IdUsuario)).ToList())
        {
            dominio.RemoverUsuario(usuario);
            repository.RemoverUsuario(usuario);
        }

        foreach (var usuario in dominio.Usuarios)
        {
            var principal = porUsuario[usuario.IdUsuario].UsuarioPrincipal;
            if (usuario.UsuarioPrincipal == principal)
                continue;

            usuario.UsuarioPrincipal = principal;
            RegistrarAlteracao(usuario);
        }

        foreach (var item in efetivos.Where(i => dominio.Usuarios.All(u => u.IdUsuario != i.IdUsuario)).ToList())
            dominio.AdicionarUsuario(NovoUsuario(dominio, item));
    }

    private void SincronizarResponsaveis(DominioEntidade dominio, IReadOnlyList<DominioResponsavelItemDto> itens)
    {
        var efetivos = AplicarPrincipalPadrao(itens, dominio.Responsaveis.Select(r => r.IdDominioPai).ToHashSet());
        var porPai = efetivos.ToDictionary(i => i.IdDominioPai);

        foreach (var responsavel in dominio.Responsaveis.Where(r => !porPai.ContainsKey(r.IdDominioPai)).ToList())
        {
            dominio.RemoverResponsavel(responsavel);
            repository.RemoverResponsavel(responsavel);
        }

        foreach (var responsavel in dominio.Responsaveis)
        {
            var principal = porPai[responsavel.IdDominioPai].DominioPaiPrincipal;
            if (responsavel.DominioPaiPrincipal == principal)
                continue;

            responsavel.DominioPaiPrincipal = principal;
            RegistrarAlteracao(responsavel);
        }

        foreach (var item in efetivos.Where(i => dominio.Responsaveis.All(r => r.IdDominioPai != i.IdDominioPai)).ToList())
            dominio.AdicionarResponsavel(NovoResponsavel(dominio, item));
    }

    private DominioUsuario NovoUsuario(DominioEntidade dominio, DominioUsuarioItemDto item)
    {
        var usuario = new DominioUsuario
        {
            IdDominio = dominio.IdDominio,
            IdFilial = dominio.IdFilial,
            IdUsuario = item.IdUsuario,
            UsuarioPrincipal = item.UsuarioPrincipal,
        };
        RegistrarInclusao(usuario);
        return usuario;
    }

    private DominioResponsavel NovoResponsavel(DominioEntidade dominio, DominioResponsavelItemDto item)
    {
        var responsavel = new DominioResponsavel
        {
            IdDominio = dominio.IdDominio,
            IdFilial = dominio.IdFilial,
            IdDominioPai = item.IdDominioPai,
            DominioPaiPrincipal = item.DominioPaiPrincipal,
        };
        RegistrarInclusao(responsavel);
        return responsavel;
    }

    // OnBeforeExecutarPersistir de Dominio/DominioUsuario/DominioResponsavel — auditoria.
    private void RegistrarInclusao(DominioEntidade d) { d.IdUsuarioInclusao = contexto.IdUsuario; d.DataInclusao = DateTime.Today; d.HoraInclusao = DateTime.Now; }
    private void RegistrarAlteracao(DominioEntidade d) { d.IdUsuarioAlteracao = contexto.IdUsuario; d.DataAlteracao = DateTime.Today; d.HoraAlteracao = DateTime.Now; }
    private void RegistrarInclusao(DominioUsuario u) { u.IdUsuarioInclusao = contexto.IdUsuario; u.DataInclusao = DateTime.Today; u.HoraInclusao = DateTime.Now; }
    private void RegistrarAlteracao(DominioUsuario u) { u.IdUsuarioAlteracao = contexto.IdUsuario; u.DataAlteracao = DateTime.Today; u.HoraAlteracao = DateTime.Now; }
    private void RegistrarInclusao(DominioResponsavel r) { r.IdUsuarioInclusao = contexto.IdUsuario; r.DataInclusao = DateTime.Today; r.HoraInclusao = DateTime.Now; }
    private void RegistrarAlteracao(DominioResponsavel r) { r.IdUsuarioAlteracao = contexto.IdUsuario; r.DataAlteracao = DateTime.Today; r.HoraAlteracao = DateTime.Now; }

    private static DominioDto ParaDto(DominioEntidade d) => new(
        d.IdDominio, d.IdFilial, d.IdCaixaBanco, d.IdDominioPeriodo, d.Descricao, d.Ativo, d.Tesouraria,
        d.AbrirPeriodo, d.AbrirOutrosPeriodos, d.FecharPeriodo, d.FecharOutrosPeriodos, d.MovimentoBanco, d.ConsultaTodosPeriodos,
        d.Saldo, d.SaldoDinheiro, d.SaldoChequeRecebido, d.SaldoCartao,
        [.. d.Usuarios.OrderBy(u => u.IdUsuario).Select(u => new DominioUsuarioDto(u.IdDominio, u.IdUsuario, u.IdFilial, u.UsuarioPrincipal,
            u.IdUsuarioInclusao, u.DataInclusao, u.HoraInclusao, u.IdUsuarioAlteracao, u.DataAlteracao, u.HoraAlteracao))],
        [.. d.Responsaveis.OrderBy(r => r.IdDominioPai).Select(r => new DominioResponsavelDto(r.IdDominio, r.IdFilial, r.IdDominioPai,
            r.DominioPaiPrincipal, r.IdUsuarioInclusao, r.DataInclusao, r.HoraInclusao, r.IdUsuarioAlteracao, r.DataAlteracao, r.HoraAlteracao))],
        d.IdUsuarioInclusao, d.DataInclusao, d.HoraInclusao, d.IdUsuarioAlteracao, d.DataAlteracao, d.HoraAlteracao);

    private async Task<bool> ParametroLigadoAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return bool.TryParse(valor, out var ligado) && ligado;
    }

    private async Task<int> ParametroIntAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return int.TryParse(valor, out var numero) ? numero : 0;
    }

    private static ValidationResult Falha(string campo, string mensagem) => ValidationResult.Fail(new ValidationError(campo, mensagem));
}

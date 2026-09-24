using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Pagination;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/CaixaBanco.cs (Validate, ValidarUsuario,
// ValidarContaBancaria, OnBeforeExecutarPersistir, ValidarPeriodoCaixa, ExecutarPersistir,
// PersistirContaBancaria, ExecutarExcluir, ValidarControleCaixaBanco, ValidarCaixaPeriodo),
// CaixaBancoUsuario.cs (IdUsuario.set, ValidarUsuarioUnico) e ParcelaBase.cs:ValidarCaixaBanco.
// Tabelas: FINCAIXABANCO, FINCAIXABANCOUSUARIO, FINCONTABANCARIA.
// Cobre: VAL-E3-01..12 · VAL-E1-27 · OP-E3-01..04, 07 (matriz-rtv.md#E3 / matriz-rot.md#E3).
public class CaixaBancoService(
    ICaixaBancoRepository repository,
    IContaBancariaRepository contaBancariaRepository,
    IContaBancariaService contaBancariaService,
    IInstituicaoFinanceiraConsulta instituicaoFinanceiraConsulta,
    IDominioFinanceiroConsulta dominioFinanceiroConsulta,
    IParametroRepository parametroRepository,
    IGeradorSequencial geradorSequencial,
    IContextoExecucao contexto) : ICaixaBancoService
{
    // Nomes dos parâmetros em GLOPARAMETRO.NOME.
    public const string ParametroVinculaCaixaBancoUsuario = "VINCULACAIXABANCOUSUARIO";
    public const string ParametroTrabalhaComDominio = "TRABALHACOMDOMINIO";

    // Sequencial do legado: [AutoSequencial("IdCaixaBanco", SequencialTipo.Filial)] — GLOSEQUENCIAL.NOMEOBJETO.
    public const string NomeSequencial = "CaixaBanco";

    // Mensagens do legado (acentuação restaurada).
    public const string MsgUsuarioRepetido = "Não é permitido repetir usuário(s). Verifique.";                 // VAL-E3-01
    public const string MsgSpedSemInstituicao =
        "Para conta bancária, a instituição financeira para SPED, deve ser informado a instituição financeira fiscal."; // VAL-E3-02
    public const string MsgSpedContaTerceiro =
        "Para conta bancária, ao MARCAR a opção 'Enviar SPED (Bloco 1601)',\r\ndeve ser DESMARCADO a opção 'Conta de terceiro'."; // VAL-E3-03
    public const string MsgSpedNaoJuridica =
        "Para conta bancária, a instituição financeira para SPED, deve ser do tipo 'Jurídica'.";               // VAL-E3-04
    public const string MsgSpedSemCnpj =
        "Para conta bancária, a instituição financeira para SPED, deve ser preenchido o CNPJ.";                // VAL-E3-05
    public const string MsgInvestimentoSemVinculada =
        "Para conta bancária do tipo 'Investimento', deve ser informada a conta vinculada.";                   // VAL-E3-06
    public const string MsgInvestimentoVinculadaIgual =
        "Para conta bancária do tipo 'Investimento', a conta vinculada deve ser DIFERENTE do ID da conta bancária."; // VAL-E3-07
    public const string MsgInativarCaixaPeriodoAberto =
        "Para inativar este caixa deve ser fechado seu domínio período pertencente ao domínio '{0}'.";         // VAL-E3-08
    public const string MsgParametrosExclusivos =
        "Os parâmetros 'Vincular caixa/banco por usuário' e 'Trabalha com domínio financeiro' somente um deles pode estar marcado. Verifique."; // VAL-E3-09
    public const string MsgUsuarioSemDominio = "O usuário logado não possui um domínio financeiro cadastrado."; // VAL-E3-10
    public const string MsgDominioSemMovimentoBanco =
        "O domínio do usuário logado está configurado para não efetuar movimento de banco.";                  // VAL-E3-10
    public const string MsgCaixaVinculadoDominio =
        "O caixa informado '({0})' está vinculado a um domínio.\r\nEste caixa somente pode ser movimentado por usuário que trabalha com domínio."; // VAL-E3-10
    public const string MsgUsuarioJaInformado = "Este usuário já foi informado.";                          // VAL-E3-11
    public const string MsgAlterarUsuarioSalvo = "NÃO é permitido alterar usuário após ter sido salvo.";     // VAL-E3-12
    public const string MsgParcelaContaCorrente =
        "Para a conta deve ser informado conta bancária do tipo 'Conta corrente'.";                            // VAL-E1-27
    public const string MsgNaoEncontrado = "Caixa/Banco não encontrado.";

    // GLOENTIDADE.IDFISICAJURIDICA — convenção do legado (DEC-007): 2 = Física, 3 = Jurídica.
    private const int IdPessoaJuridica = 3;

    // ---- Consultas (ReadConnection) ------------------------------------------------------

    public async Task<PagedResult<CaixaBanco>> ListarPaginadoAsync(string? texto, bool? ativo, ContaTipo? tipoConta,
        bool? entraFluxoCaixa, int page, int limit, CancellationToken cancellationToken = default)
    {
        // Artigo VII.5 — materializa antes de paginar (SQL Server 2008).
        var todos = await repository.ListarAsync(contexto.IdFilial, texto, ativo, tipoConta, entraFluxoCaixa, cancellationToken);
        var pagina = Math.Max(page, 1);
        var tamanho = Math.Max(limit, 1);

        return new PagedResult<CaixaBanco>([.. todos.Skip((pagina - 1) * tamanho).Take(tamanho)], todos.Count);
    }

    public Task<CaixaBanco?> ObterPorIdAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => repository.ObterAsync(idCaixaBanco, idFilial, cancellationToken);

    public Task<IReadOnlyList<CaixaBancoUsuario>> ListarUsuariosAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
        => repository.ListarUsuariosAsync(idCaixaBanco, idFilial, cancellationToken);

    // ---- Persistência (WriteConnection) --------------------------------------------------

    public async Task<Result<CaixaBanco>> CriarAsync(CaixaBanco caixa, ContaBancaria? conta, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(caixa);

        var validacao = await ValidarPersistenciaAsync(caixa, conta, contaPersistida: false, ativoPersistido: null, cancellationToken);
        if (!validacao.IsValid)
            return Result<CaixaBanco>.Fail([.. validacao.Errors]);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        caixa.IdFilial = contexto.IdFilial;
        caixa.IdCaixaBanco = await geradorSequencial.ProximoAsync(NomeSequencial, SequencialTipo.Filial, cancellationToken);
        RegistrarInclusao(caixa);

        foreach (var usuario in caixa.Usuarios)
        {
            usuario.IdCaixaBanco = caixa.IdCaixaBanco;
            usuario.IdFilial = caixa.IdFilial;
            RegistrarInclusao(usuario);
        }

        await repository.AddAsync(caixa, cancellationToken);

        // OP-E3-04 — PersistirContaBancaria: só existe conta bancária quando TipoConta = Banco.
        if (caixa.TipoConta == ContaTipo.Banco && conta is not null)
        {
            conta.IdCaixaBanco = caixa.IdCaixaBanco;
            conta.IdFilial = caixa.IdFilial;
            ContaBancariaService.RegistrarInclusao(conta, contexto.IdUsuario);
            await repository.AdicionarContaBancariaAsync(conta, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<CaixaBanco>.Ok(caixa);
    }

    public async Task<Result<CaixaBanco>> AtualizarAsync(CaixaBanco caixa, ContaBancaria? conta, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(caixa);

        var existente = await repository.ObterParaEdicaoAsync(caixa.IdCaixaBanco, caixa.IdFilial, cancellationToken);
        if (existente is null)
            return Result<CaixaBanco>.Fail(new ValidationError(nameof(CaixaBanco.IdCaixaBanco), MsgNaoEncontrado));

        var contaExistente = await repository.ObterContaBancariaParaEdicaoAsync(caixa.IdCaixaBanco, caixa.IdFilial, cancellationToken);

        var validacao = await ValidarPersistenciaAsync(caixa, conta, contaPersistida: contaExistente is not null,
            ativoPersistido: existente.Ativo, cancellationToken);
        if (!validacao.IsValid)
            return Result<CaixaBanco>.Fail([.. validacao.Errors]);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        existente.Descricao = caixa.Descricao;
        existente.TipoConta = caixa.TipoConta;
        existente.Ativo = caixa.Ativo;
        existente.EntraFluxoCaixa = caixa.EntraFluxoCaixa;
        existente.UltimaDataConferida = caixa.UltimaDataConferida;
        existente.Saldo = caixa.Saldo;
        existente.ContaContabil = caixa.ContaContabil;
        existente.IdPlanoContabil = caixa.IdPlanoContabil;
        existente.TipoContaCaixa = caixa.TipoContaCaixa;
        RegistrarAlteracao(existente);

        SincronizarUsuarios(existente, [.. caixa.Usuarios.Select(u => u.IdUsuario)]);

        // OP-E3-04 — Caixa remove a FINCONTABANCARIA; Banco grava a conta com a PK do caixa.
        if (existente.TipoConta == ContaTipo.Caixa)
        {
            if (contaExistente is not null)
                repository.RemoverContaBancaria(contaExistente);
        }
        else if (conta is not null)
        {
            if (contaExistente is null)
            {
                conta.IdCaixaBanco = existente.IdCaixaBanco;
                conta.IdFilial = existente.IdFilial;
                ContaBancariaService.RegistrarInclusao(conta, contexto.IdUsuario);
                await repository.AdicionarContaBancariaAsync(conta, cancellationToken);
            }
            else
            {
                ContaBancariaService.CopiarNucleo(conta, contaExistente);
                ContaBancariaService.RegistrarAlteracao(contaExistente, contexto.IdUsuario);
                // OP-E3-05 — só quando a conta já estava persistida.
                await contaBancariaService.AtualizarDadosContasVinculadasAsync(contaExistente, contexto.IdFilial, cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<CaixaBanco>.Ok(existente);
    }

    public async Task<ValidationResult> ExcluirAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
    {
        var existente = await repository.ObterParaEdicaoAsync(idCaixaBanco, idFilial, cancellationToken);
        if (existente is null)
            return ValidationResult.Fail(new ValidationError(nameof(CaixaBanco.IdCaixaBanco), MsgNaoEncontrado));

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        // OP-E3-02 — CaixaBanco.cs:135-144: conta bancária primeiro, depois o caixa (com usuários).
        var conta = await repository.ObterContaBancariaParaEdicaoAsync(idCaixaBanco, idFilial, cancellationToken);
        if (conta is not null)
            repository.RemoverContaBancaria(conta);

        foreach (var usuario in existente.Usuarios.ToList())
            repository.RemoverUsuario(usuario);

        await repository.DeleteAsync(existente, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return ValidationResult.Ok();
    }

    public async Task<Result<IReadOnlyList<CaixaBancoUsuario>>> SalvarUsuariosAsync(int idCaixaBanco, int idFilial,
        IReadOnlyList<CaixaBancoUsuarioItemDto> itens, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(itens);

        var existente = await repository.ObterParaEdicaoAsync(idCaixaBanco, idFilial, cancellationToken);
        if (existente is null)
            return Result<IReadOnlyList<CaixaBancoUsuario>>.Fail(new ValidationError(nameof(CaixaBanco.IdCaixaBanco), MsgNaoEncontrado));

        var salvos = existente.Usuarios.Select(u => u.IdUsuario).ToHashSet();

        // VAL-E3-12 — CaixaBancoUsuario.IdUsuario.set: linha salva não troca de usuário.
        if (itens.Any(i => i.IdUsuarioSalvo is { } anterior && anterior != 0 && anterior != i.IdUsuario && salvos.Contains(anterior)))
            return Result<IReadOnlyList<CaixaBancoUsuario>>.Fail(new ValidationError(nameof(CaixaBancoUsuario.IdUsuario), MsgAlterarUsuarioSalvo));

        var ids = itens.Select(i => i.IdUsuario).ToList();

        var unico = ValidarUsuarioUnico(ids);
        if (!unico.IsValid)
            return Result<IReadOnlyList<CaixaBancoUsuario>>.Fail([.. unico.Errors]);

        var vinculo = await ValidarUsuarioAsync(ids, cancellationToken);
        if (!vinculo.IsValid)
            return Result<IReadOnlyList<CaixaBancoUsuario>>.Fail([.. vinculo.Errors]);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        SincronizarUsuarios(existente, ids);

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<IReadOnlyList<CaixaBancoUsuario>>.Ok([.. existente.Usuarios.OrderBy(u => u.IdUsuario)]);
    }

    // ---- Validações ------------------------------------------------------------------------

    public Task<ValidationResult> ValidarUsuarioAsync(CaixaBanco caixa, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(caixa);
        return ValidarUsuarioAsync([.. caixa.Usuarios.Select(u => u.IdUsuario)], cancellationToken);
    }

    public async Task<ValidationResult> ValidarControleCaixaBancoAsync(PeriodoStatus statusPeriodo,
        CancellationToken cancellationToken = default)
    {
        // CaixaBanco.cs:774-794.
        if (statusPeriodo is PeriodoStatus.NaoAplicavel or PeriodoStatus.PerfilSemPermissao)
            return ValidationResult.Ok();

        if (statusPeriodo != PeriodoStatus.UsuarioSemPermissao)
            return ValidationResult.Ok();

        if (await ParametroLigadoAsync(ParametroVinculaCaixaBancoUsuario, cancellationToken))
            return ValidationResult.Fail(new ValidationError("Parametro", MsgParametrosExclusivos));

        return ValidationResult.Ok();
    }

    public async Task<ValidationResult> ValidarCaixaPeriodoAsync(CaixaBanco caixa, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(caixa);

        // CaixaBanco.cs:799-833 — no legado devolve a mensagem; aqui, ValidationResult.
        if (!await ParametroLigadoAsync(ParametroTrabalhaComDominio, cancellationToken))
            return ValidationResult.Ok();

        if (caixa.TipoConta == ContaTipo.Banco)
        {
            var movimentoBanco = await dominioFinanceiroConsulta.ObterMovimentoBancoDoDominioDoUsuarioAsync(
                contexto.IdUsuario, contexto.IdFilial, cancellationToken);

            if (movimentoBanco is null)
                return ValidationResult.Fail(new ValidationError(nameof(CaixaBanco.IdCaixaBanco), MsgUsuarioSemDominio));

            return movimentoBanco.Value
                ? ValidationResult.Ok()
                : ValidationResult.Fail(new ValidationError(nameof(CaixaBanco.IdCaixaBanco), MsgDominioSemMovimentoBanco));
        }

        if (await dominioFinanceiroConsulta.UsuarioPossuiDominioAsync(contexto.IdUsuario, contexto.IdFilial, cancellationToken))
            return ValidationResult.Ok();

        if (!await dominioFinanceiroConsulta.CaixaPossuiDominioAsync(caixa.IdCaixaBanco, contexto.IdFilial, cancellationToken))
            return ValidationResult.Ok();

        return ValidationResult.Fail(new ValidationError(nameof(CaixaBanco.IdCaixaBanco),
            string.Format(MsgCaixaVinculadoDominio, caixa.IdCaixaBanco)));
    }

    public async Task<ValidationResult> ValidarCaixaBancoParcelaAsync(int? idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default)
    {
        // ParcelaBase.cs:497-516.
        if (idCaixaBanco is null)
            return ValidationResult.Ok();

        var caixa = await repository.ObterAsync(idCaixaBanco.Value, idFilial, cancellationToken);
        if (caixa is null || caixa.TipoConta != ContaTipo.Banco)
            return ValidationResult.Ok();

        var conta = await contaBancariaRepository.ObterAsync(idCaixaBanco.Value, idFilial, cancellationToken);
        if (conta is null || conta.ContaBancariaTipo == TipoContaBancaria.ContaCorrente)
            return ValidationResult.Ok();

        return ValidationResult.Fail(new ValidationError("IdCaixaBancoParcela", MsgParcelaContaCorrente));
    }

    // Ordem do legado: Validate (setters/VAL-E3-11 → ValidarUsuario → ValidarContaBancaria) →
    // OnBeforeExecutarPersistir (ValidarPeriodoCaixa) → ContaBancaria.Validate (dentro do persist).
    private async Task<ValidationResult> ValidarPersistenciaAsync(CaixaBanco caixa, ContaBancaria? conta, bool contaPersistida,
        bool? ativoPersistido, CancellationToken cancellationToken)
    {
        var ids = caixa.Usuarios.Select(u => u.IdUsuario).ToList();

        var resultado = ValidarUsuarioUnico(ids);
        if (!resultado.IsValid)
            return resultado;

        resultado = await ValidarUsuarioAsync(ids, cancellationToken);
        if (!resultado.IsValid)
            return resultado;

        resultado = await ValidarContaBancariaAsync(caixa, conta, contaPersistida, cancellationToken);
        if (!resultado.IsValid)
            return resultado;

        if (ativoPersistido.HasValue)
        {
            resultado = await ValidarPeriodoCaixaAsync(caixa, ativoPersistido.Value, cancellationToken);
            if (!resultado.IsValid)
                return resultado;
        }

        if (caixa.TipoConta == ContaTipo.Banco && conta is not null)
            return contaBancariaService.Validar(conta);

        return ValidationResult.Ok();
    }

    // VAL-E3-11 — CaixaBancoUsuario.ValidarUsuarioUnico (disparada no setter, sem parâmetro).
    private static ValidationResult ValidarUsuarioUnico(IReadOnlyList<int> idsUsuarios)
        => idsUsuarios.Count == idsUsuarios.Distinct().Count()
            ? ValidationResult.Ok()
            : ValidationResult.Fail(new ValidationError(nameof(CaixaBancoUsuario.IdUsuario), MsgUsuarioJaInformado));

    // VAL-E3-01 — CaixaBanco.cs:186-216.
    private async Task<ValidationResult> ValidarUsuarioAsync(IReadOnlyList<int> idsUsuarios, CancellationToken cancellationToken)
    {
        if (!await ParametroLigadoAsync(ParametroVinculaCaixaBancoUsuario, cancellationToken))
            return ValidationResult.Ok();

        if (idsUsuarios.Count == 0 || idsUsuarios.Count == idsUsuarios.Distinct().Count())
            return ValidationResult.Ok();

        return ValidationResult.Fail(new ValidationError(nameof(CaixaBanco.Usuarios), MsgUsuarioRepetido));
    }

    // VAL-E3-02..07 — CaixaBanco.cs:221-278.
    private async Task<ValidationResult> ValidarContaBancariaAsync(CaixaBanco caixa, ContaBancaria? conta, bool contaPersistida,
        CancellationToken cancellationToken)
    {
        if (caixa.TipoConta != ContaTipo.Banco || conta is null)
            return ValidationResult.Ok();

        if (conta.EnviarSped)
        {
            var instituicao = conta.IdInstituicaoFinanceira is { } idInstituicao
                ? await instituicaoFinanceiraConsulta.ObterAsync(idInstituicao, cancellationToken)
                : null;

            if (instituicao is null)
                return Falha(nameof(ContaBancaria.IdInstituicaoFinanceira), MsgSpedSemInstituicao);

            if (conta.ContaTerceiro)
                return Falha(nameof(ContaBancaria.ContaTerceiro), MsgSpedContaTerceiro);

            if (instituicao.IdFisicaJuridica != IdPessoaJuridica)
                return Falha(nameof(ContaBancaria.IdInstituicaoFinanceira), MsgSpedNaoJuridica);

            if (string.IsNullOrEmpty(instituicao.Cnpj))
                return Falha(nameof(ContaBancaria.IdInstituicaoFinanceira), MsgSpedSemCnpj);
        }

        if (conta.ContaBancariaTipo != TipoContaBancaria.Investimento)
            return ValidationResult.Ok();

        if (conta.IdContaBancariaVinculada is null)
            return Falha(nameof(ContaBancaria.IdContaBancariaVinculada), MsgInvestimentoSemVinculada);

        if (!contaPersistida)
            return ValidationResult.Ok();

        if (conta.IdContaBancariaVinculada == caixa.IdCaixaBanco)
            return Falha(nameof(ContaBancaria.IdContaBancariaVinculada), MsgInvestimentoVinculadaIgual);

        return ValidationResult.Ok();
    }

    // VAL-E3-08 — CaixaBanco.cs:283-328 (só na alteração: IsPersisted).
    private async Task<ValidationResult> ValidarPeriodoCaixaAsync(CaixaBanco caixa, bool ativoPersistido, CancellationToken cancellationToken)
    {
        if (caixa.TipoConta != ContaTipo.Caixa || caixa.Ativo || !ativoPersistido)
            return ValidationResult.Ok();

        var periodos = await dominioFinanceiroConsulta.ListarPeriodosDosDominiosDoCaixaAsync(
            caixa.IdCaixaBanco, contexto.IdFilial, cancellationToken);

        foreach (var periodo in periodos)
        {
            if (periodo.DataFechamento is { } fechamento && fechamento > DateTime.MinValue)
                continue;

            return Falha(nameof(CaixaBanco.Ativo), string.Format(MsgInativarCaixaPeriodoAberto, periodo.IdDominio));
        }

        return ValidationResult.Ok();
    }

    // OP-E3-07 — insere os novos e remove os ausentes; os mantidos ficam intactos.
    private void SincronizarUsuarios(CaixaBanco caixa, IReadOnlyList<int> idsUsuarios)
    {
        foreach (var usuario in caixa.Usuarios.Where(u => !idsUsuarios.Contains(u.IdUsuario)).ToList())
        {
            caixa.RemoverUsuario(usuario);
            repository.RemoverUsuario(usuario);
        }

        foreach (var idUsuario in idsUsuarios.Where(id => caixa.Usuarios.All(u => u.IdUsuario != id)))
        {
            var usuario = new CaixaBancoUsuario { IdCaixaBanco = caixa.IdCaixaBanco, IdFilial = caixa.IdFilial, IdUsuario = idUsuario };
            RegistrarInclusao(usuario);
            caixa.AdicionarUsuario(usuario);
        }
    }

    private async Task<bool> ParametroLigadoAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, cancellationToken);
        return bool.TryParse(valor, out var ligado) && ligado;
    }

    private static ValidationResult Falha(string campo, string mensagem) => ValidationResult.Fail(new ValidationError(campo, mensagem));

    // CaixaBanco.cs:107-118 / CaixaBancoUsuario.cs:62-73 — auditoria do OnBeforeExecutarPersistir.
    private void RegistrarInclusao(CaixaBanco caixa)
    {
        caixa.IdUsuarioInclusao = contexto.IdUsuario;
        caixa.DataInclusao = DateTime.Today;
        caixa.HoraInclusao = DateTime.Now;
    }

    private void RegistrarAlteracao(CaixaBanco caixa)
    {
        caixa.IdUsuarioAlteracao = contexto.IdUsuario;
        caixa.DataAlteracao = DateTime.Today;
        caixa.HoraAlteracao = DateTime.Now;
    }

    private void RegistrarInclusao(CaixaBancoUsuario usuario)
    {
        usuario.IdUsuarioInclusao = contexto.IdUsuario;
        usuario.DataInclusao = DateTime.Today;
        usuario.HoraInclusao = DateTime.Now;
    }
}

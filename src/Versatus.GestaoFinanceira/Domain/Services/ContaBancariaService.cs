using Versatus.Framework.Context;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/ContaBancaria.cs (legado — Validate,
// OnBeforeExecutarPersistir, ExecutarPersistir, UpdateDadosContaVinculada).
// Tabela: FINCONTABANCARIA (núcleo E3).
// Cobre: VAL-E3-17 · OP-E3-05 (núcleo). VAL-E3-13..16/18/19/21 e os sequenciais de arquivo
// (AdicionarSequencial/RemoverSequencial) → épico E14.
public class ContaBancariaService(IContaBancariaRepository repository, IContextoExecucao contexto) : IContaBancariaService
{
    // LanguageManager ContaBancaria item 4 (ContaBancaria.cs:1699).
    public const string MsgContaTerceiro =
        "Para cadastrar uma conta de terceiro tem que informar o nome do titular e o CPF/CNPJ do mesmo.";

    public Task<ContaBancaria?> ObterPorIdAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default)
        => repository.ObterAsync(idCaixaBanco, idFilial, cancellationToken);

    public async Task<Result<ContaBancaria>> AtualizarAsync(ContaBancaria conta, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(conta);

        var existente = await repository.ObterParaEdicaoAsync(conta.IdCaixaBanco, conta.IdFilial, cancellationToken);
        if (existente is null)
            return Result<ContaBancaria>.Fail(new ValidationError(nameof(ContaBancaria.IdCaixaBanco), "Conta bancária não encontrada."));

        var validacao = Validar(conta);
        if (!validacao.IsValid)
            return Result<ContaBancaria>.Fail([.. validacao.Errors]);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        CopiarNucleo(conta, existente);
        RegistrarAlteracao(existente, contexto.IdUsuario);
        await AtualizarDadosContasVinculadasAsync(existente, contexto.IdFilial, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<ContaBancaria>.Ok(existente);
    }

    // ContaBancaria.cs:202 — só a regra núcleo; as de boleto/remessa/protesto são [E14].
    public ValidationResult Validar(ContaBancaria conta)
    {
        ArgumentNullException.ThrowIfNull(conta);

        if (conta.ContaTerceiro && (string.IsNullOrEmpty(conta.Titular) || string.IsNullOrEmpty(conta.CpfCnpj)))
            return ValidationResult.Fail(new ValidationError(nameof(ContaBancaria.Titular), MsgContaTerceiro));

        return ValidationResult.Ok();
    }

    // ContaBancaria.cs:286-344 — o legado monta um UPDATE por conta; aqui as mesmas colunas são
    // atribuídas às entidades rastreadas (vazio → NULL; LIMITE só é copiado se conta de terceiro).
    public async Task AtualizarDadosContasVinculadasAsync(ContaBancaria conta, int idFilial, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(conta);

        var contas = await repository.ListarContasQueVinculamParaEdicaoAsync(conta.IdCaixaBanco, idFilial, cancellationToken);
        foreach (var cb in contas)
        {
            cb.IdAgencia = conta.IdAgencia;
            cb.NumeroConta = conta.NumeroConta;
            cb.EnviarSped = false;
            cb.IdInstituicaoFinanceira = null;
            cb.DigitoConta = string.IsNullOrEmpty(conta.DigitoConta) ? null : conta.DigitoConta;
            cb.Titular = string.IsNullOrEmpty(conta.Titular) ? null : conta.Titular;
            cb.CpfCnpj = string.IsNullOrEmpty(conta.CpfCnpj) ? null : conta.CpfCnpj;
            cb.ContaTerceiro = conta.ContaTerceiro;
            cb.Limite = conta.ContaTerceiro ? conta.Limite ?? 0m : 0m;
        }
    }

    /// <summary>Copia as colunas núcleo editáveis (PK e auditoria ficam de fora).</summary>
    internal static void CopiarNucleo(ContaBancaria origem, ContaBancaria destino)
    {
        destino.IdAgencia = origem.IdAgencia;
        destino.Titular = origem.Titular;
        destino.NumeroConta = origem.NumeroConta;
        destino.DigitoConta = origem.DigitoConta;
        destino.Limite = origem.Limite;
        destino.CreditoPendente = origem.CreditoPendente;
        destino.DebitoPendente = origem.DebitoPendente;
        destino.ChequePendente = origem.ChequePendente;
        destino.ContaTerceiro = origem.ContaTerceiro;
        destino.PermiteEmitirCheque = origem.PermiteEmitirCheque;
        destino.IdContaBancariaVinculada = origem.IdContaBancariaVinculada;
        destino.ContaBancariaTipo = origem.ContaBancariaTipo;
        destino.IdInstituicaoFinanceira = origem.IdInstituicaoFinanceira;
        destino.EnviarSped = origem.EnviarSped;
        destino.CpfCnpj = origem.CpfCnpj;
    }

    // ContaBancaria.cs:214-230 — OnBeforeExecutarPersistir.
    internal static void RegistrarInclusao(ContaBancaria conta, int idUsuario)
    {
        conta.IdUsuarioInclusao = idUsuario;
        conta.DataInclusao = DateTime.Today;
        conta.HoraInclusao = DateTime.Now;
    }

    internal static void RegistrarAlteracao(ContaBancaria conta, int idUsuario)
    {
        conta.IdUsuarioAlteracao = idUsuario;
        conta.DataAlteracao = DateTime.Today;
        conta.HoraAlteracao = DateTime.Now;
    }
}

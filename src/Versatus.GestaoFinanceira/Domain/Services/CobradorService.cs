using Versatus.Framework.Context;
using Versatus.Framework.Pagination;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Repositories;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/Cobrador.cs (legado — OnBeforeExecutarPersistir,
// [AutoSequencial("IdCobrador", SequencialTipo.Filial)]). Tabela: FINCOBRADOR.
// Sem VAL-xx na matriz-rtv.md#E3 ("CRUD simples"): só sequencial + auditoria.
public class CobradorService(ICobradorRepository repository, IGeradorSequencial geradorSequencial, IContextoExecucao contexto)
    : ICobradorService
{
    public const string NomeSequencial = "Cobrador";
    public const string MsgNaoEncontrado = "Cobrador não encontrado.";

    public async Task<PagedResult<Cobrador>> ListarPaginadoAsync(string? texto, bool? ativo, int page, int limit,
        CancellationToken cancellationToken = default)
    {
        // Artigo VII.5 — materializa antes de paginar (SQL Server 2008).
        var todos = await repository.ListarAsync(contexto.IdFilial, texto, ativo, cancellationToken);
        var pagina = Math.Max(page, 1);
        var tamanho = Math.Max(limit, 1);

        return new PagedResult<Cobrador>([.. todos.Skip((pagina - 1) * tamanho).Take(tamanho)], todos.Count);
    }

    public Task<Cobrador?> ObterPorIdAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default)
        => repository.ObterAsync(idCobrador, idFilial, cancellationToken);

    public async Task<Result<Cobrador>> CriarAsync(Cobrador cobrador, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cobrador);

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        cobrador.IdFilial = contexto.IdFilial;
        cobrador.IdCobrador = await geradorSequencial.ProximoAsync(NomeSequencial, SequencialTipo.Filial, cancellationToken);
        cobrador.IdUsuarioInclusao = contexto.IdUsuario;
        cobrador.DataInclusao = DateTime.Today;
        cobrador.HoraInclusao = DateTime.Now;

        await repository.AddAsync(cobrador, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<Cobrador>.Ok(cobrador);
    }

    public async Task<Result<Cobrador>> AtualizarAsync(Cobrador cobrador, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cobrador);

        var existente = await repository.ObterParaEdicaoAsync(cobrador.IdCobrador, cobrador.IdFilial, cancellationToken);
        if (existente is null)
            return Result<Cobrador>.Fail(new ValidationError(nameof(Cobrador.IdCobrador), MsgNaoEncontrado));

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        existente.IdEntidade = cobrador.IdEntidade;
        existente.Nome = cobrador.Nome;
        existente.Ativo = cobrador.Ativo;
        existente.IdUsuario = cobrador.IdUsuario;
        existente.IdMeioContato = cobrador.IdMeioContato;
        existente.IdUsuarioAlteracao = contexto.IdUsuario;
        existente.DataAlteracao = DateTime.Today;
        existente.HoraAlteracao = DateTime.Now;

        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return Result<Cobrador>.Ok(existente);
    }

    public async Task<ValidationResult> ExcluirAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default)
    {
        var existente = await repository.ObterParaEdicaoAsync(idCobrador, idFilial, cancellationToken);
        if (existente is null)
            return ValidationResult.Fail(new ValidationError(nameof(Cobrador.IdCobrador), MsgNaoEncontrado));

        await using var transacao = await repository.IniciarTransacaoAsync(cancellationToken);

        await repository.DeleteAsync(existente, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return ValidationResult.Ok();
    }
}

using Versatus.Framework.Context;
using Versatus.Framework.Pagination;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;
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

    public async Task<PagedResult<CobradorDto>> ListarPaginadoAsync(FiltroCobradorDto filtro, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        // Artigo VII.5 — materializa antes de paginar (SQL Server 2008).
        var todos = await repository.ListarAsync(contexto.IdFilial, filtro.Texto, filtro.Ativo, cancellationToken);
        var pagina = Math.Max(filtro.Page, 1);
        var tamanho = Math.Max(filtro.Limit, 1);

        return new PagedResult<CobradorDto>([.. todos.Skip((pagina - 1) * tamanho).Take(tamanho).Select(BancosDtoMapper.ParaDto)], todos.Count);
    }

    public async Task<CobradorDto?> ObterPorIdAsync(int idCobrador, int idFilial, CancellationToken cancellationToken = default)
        => await repository.ObterAsync(idCobrador, idFilial, cancellationToken) is { } cobrador ? BancosDtoMapper.ParaDto(cobrador) : null;

    public async Task<Result<CobradorDto>> CriarAsync(SalvarCobradorDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return ParaResultadoDto(await CriarAsync(BancosDtoMapper.ParaEntidade(0, contexto.IdFilial, dto), cancellationToken));
    }

    public async Task<Result<CobradorDto>> AtualizarAsync(int idCobrador, int idFilial, SalvarCobradorDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return ParaResultadoDto(await AtualizarAsync(BancosDtoMapper.ParaEntidade(idCobrador, idFilial, dto), cancellationToken));
    }

    private static Result<CobradorDto> ParaResultadoDto(Result<Cobrador> resultado)
        => resultado.IsSuccess
            ? Result<CobradorDto>.Ok(BancosDtoMapper.ParaDto(resultado.Value!))
            : Result<CobradorDto>.Fail([.. resultado.Errors]);

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

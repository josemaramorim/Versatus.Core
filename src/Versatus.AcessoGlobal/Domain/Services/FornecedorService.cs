using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.Framework.Context;

namespace Versatus.AcessoGlobal.Domain.Services;

public class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IEntidadeRepository _entidadeRepository;
    private readonly IEntidadeService _entidadeService;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<FornecedorService> _logger;

    public FornecedorService(
        IFornecedorRepository fornecedorRepository,
        IEntidadeRepository entidadeRepository,
        IEntidadeService entidadeService,
        IContextoExecucao contexto,
        ILogger<FornecedorService> logger)
    {
        _fornecedorRepository = fornecedorRepository;
        _entidadeRepository = entidadeRepository;
        _entidadeService = entidadeService;
        _contexto = contexto;
        _logger = logger;
    }

    public async Task<IEnumerable<FornecedorResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        var fornecedores = await _fornecedorRepository.GetAllAsync(cancellationToken);
        return fornecedores.Select(f => new FornecedorResponseDto(
            f.IdFornecedor,
            f.Entidade?.Nome ?? "Entidade Desconhecida",
            f.CodigoAlternativo,
            f.ContaContabil,
            f.Ativo
        ));
    }

    public async Task<Fornecedor?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Fornecedor> CriarAsync(CriarFornecedorDto dto, CancellationToken cancellationToken = default)
    {
        Entidade? entidade = null;

        if (dto.IdEntidade.HasValue)
        {
            entidade = await _entidadeRepository.GetByIdAsync(dto.IdEntidade.Value, cancellationToken);
            if (entidade == null)
            {
                throw new InvalidOperationException($"Entidade base com ID {dto.IdEntidade.Value} não encontrada.");
            }
            entidade.IsFornecedor = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else if (dto.NovaEntidade != null)
        {
            var novaEnt = await _entidadeService.CriarAsync(dto.NovaEntidade, cancellationToken);
            entidade = novaEnt;
            entidade.IsFornecedor = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("É necessário informar o IdEntidade ou os dados da NovaEntidade.");
        }

        var fornecedor = new Fornecedor
        {
            IdFornecedor = entidade.IdEntidade,
            CodigoAlternativo = dto.CodigoAlternativo,
            ContaContabil = dto.ContaContabil,
            Ativo = dto.Ativo,
            IsFornecedorCotacao = dto.IsFornecedorCotacao,
            IdCategoria = dto.IdCategoria,
            IdCondicaoPagamento = dto.IdCondicaoPagamento,
            IdUsuarioInclusao = _contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Now
        };

        await _fornecedorRepository.AddAsync(fornecedor, cancellationToken);
        await _fornecedorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fornecedor {IdFornecedor} criado com sucesso.", fornecedor.IdFornecedor);

        return fornecedor;
    }

    public async Task PinkAtualizarAsync(int id, CriarFornecedorDto dto, CancellationToken cancellationToken = default)
    {
        // Métodos de atualização normais
    }

    public async Task AtualizarAsync(int id, CriarFornecedorDto dto, CancellationToken cancellationToken = default)
    {
        var fornecedor = await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
        if (fornecedor == null)
        {
            throw new InvalidOperationException($"Fornecedor com ID {id} não encontrado.");
        }

        fornecedor.CodigoAlternativo = dto.CodigoAlternativo;
        fornecedor.ContaContabil = dto.ContaContabil;
        fornecedor.Ativo = dto.Ativo;
        fornecedor.IsFornecedorCotacao = dto.IsFornecedorCotacao;
        fornecedor.IdCategoria = dto.IdCategoria;
        fornecedor.IdCondicaoPagamento = dto.IdCondicaoPagamento;

        fornecedor.IdUsuarioAlteracao = _contexto.IdUsuario;
        fornecedor.DataAlteracao = DateTime.Today;
        fornecedor.HoraAlteracao = DateTime.Now;

        await _fornecedorRepository.UpdateAsync(fornecedor, cancellationToken);
        await _fornecedorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fornecedor {IdFornecedor} atualizado com sucesso.", id);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var fornecedor = await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
        if (fornecedor != null)
        {
            await _fornecedorRepository.DeleteAsync(fornecedor, cancellationToken);
            await _fornecedorRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Fornecedor {IdFornecedor} excluído com sucesso.", id);
        }
    }
}

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

public class TransportadoraService : ITransportadoraService
{
    private readonly ITransportadoraRepository _transportadoraRepository;
    private readonly IEntidadeRepository _entidadeRepository;
    private readonly IEntidadeService _entidadeService;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<TransportadoraService> _logger;

    public TransportadoraService(
        ITransportadoraRepository transportadoraRepository,
        IEntidadeRepository entidadeRepository,
        IEntidadeService entidadeService,
        IContextoExecucao contexto,
        ILogger<TransportadoraService> logger)
    {
        _transportadoraRepository = transportadoraRepository;
        _entidadeRepository = entidadeRepository;
        _entidadeService = entidadeService;
        _contexto = contexto;
        _logger = logger;
    }

    public async Task<IEnumerable<TransportadoraResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        var transportadoras = await _transportadoraRepository.GetAllAsync(cancellationToken);
        return transportadoras.Select(t => new TransportadoraResponseDto(
            t.IdTransportadora,
            t.Entidade?.Nome ?? "Entidade Desconhecida",
            t.Rntrc,
            t.Ativo
        ));
    }

    public async Task<Transportadora?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _transportadoraRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Transportadora> CriarAsync(CriarTransportadoraDto dto, CancellationToken cancellationToken = default)
    {
        Entidade? entidade = null;

        if (dto.IdEntidade.HasValue)
        {
            entidade = await _entidadeRepository.GetCompletoPorIdAsync(dto.IdEntidade.Value, cancellationToken);
            if (entidade == null)
            {
                throw new InvalidOperationException($"Entidade base com ID {dto.IdEntidade.Value} não encontrada.");
            }
            entidade.IsTransportadora = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else if (dto.NovaEntidade != null)
        {
            var novaEnt = await _entidadeService.CriarAsync(dto.NovaEntidade, cancellationToken);
            entidade = novaEnt;
            entidade.IsTransportadora = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("É necessário informar o IdEntidade ou os dados da NovaEntidade.");
        }

        var transportadora = new Transportadora
        {
            IdTransportadora = entidade.IdEntidade,
            Rntrc = dto.Rntrc,
            Ativo = dto.Ativo,
            ProprietarioTipo = dto.ProprietarioTipo,
            TransportadorTipo = dto.TransportadorTipo,
            IdCategoria = dto.IdCategoria,
            IdUsuarioInclusao = _contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Now
        };

        await _transportadoraRepository.AddAsync(transportadora, cancellationToken);
        await _transportadoraRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transportadora {IdTransportadora} criada com sucesso.", transportadora.IdTransportadora);

        return transportadora;
    }

    public async Task PinkAtualizarAsync(int id, CriarTransportadoraDto dto, CancellationToken cancellationToken = default)
    {
        // Métodos de atualização normais
    }

    public async Task PinkExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        // Métodos de exclusão normais
    }

    public async Task AtualizarAsync(int id, CriarTransportadoraDto dto, CancellationToken cancellationToken = default)
    {
        var transportadora = await _transportadoraRepository.GetByIdAsync(id, cancellationToken);
        if (transportadora == null)
        {
            throw new InvalidOperationException($"Transportadora com ID {id} não encontrada.");
        }

        transportadora.Rntrc = dto.Rntrc;
        transportadora.Ativo = dto.Ativo;
        transportadora.ProprietarioTipo = dto.ProprietarioTipo;
        transportadora.TransportadorTipo = dto.TransportadorTipo;
        transportadora.IdCategoria = dto.IdCategoria;

        transportadora.IdUsuarioAlteracao = _contexto.IdUsuario;
        transportadora.DataAlteracao = DateTime.Today;
        transportadora.HoraAlteracao = DateTime.Now;

        await _transportadoraRepository.UpdateAsync(transportadora, cancellationToken);
        await _transportadoraRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transportadora {IdTransportadora} atualizada com sucesso.", id);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var transportadora = await _transportadoraRepository.GetByIdAsync(id, cancellationToken);
        if (transportadora != null)
        {
            await _transportadoraRepository.DeleteAsync(transportadora, cancellationToken);
            await _transportadoraRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Transportadora {IdTransportadora} excluída com sucesso.", id);
        }
    }
}

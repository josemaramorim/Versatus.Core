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

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IEntidadeRepository _entidadeRepository;
    private readonly IEntidadeService _entidadeService;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(
        IClienteRepository clienteRepository,
        IEntidadeRepository entidadeRepository,
        IEntidadeService entidadeService,
        IContextoExecucao contexto,
        ILogger<ClienteService> logger)
    {
        _clienteRepository = clienteRepository;
        _entidadeRepository = entidadeRepository;
        _entidadeService = entidadeService;
        _contexto = contexto;
        _logger = logger;
    }

    public async Task<IEnumerable<ClienteResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _clienteRepository.GetAllAsync(cancellationToken);
        return clientes.Select(c => new ClienteResponseDto(
            c.IdCliente,
            c.Entidade?.Nome ?? "Entidade Desconhecida",
            c.CodigoAlternativo,
            c.LimiteCredito,
            c.Ativo,
            c.SituacaoSPC.ToString()
        ));
    }

    public async Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _clienteRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Cliente> CriarAsync(CriarClienteDto dto, CancellationToken cancellationToken = default)
    {
        Entidade? entidade = null;

        if (dto.IdEntidade.HasValue)
        {
            entidade = await _entidadeRepository.GetCompletoPorIdAsync(dto.IdEntidade.Value, cancellationToken);
            if (entidade == null)
            {
                throw new InvalidOperationException($"Entidade base com ID {dto.IdEntidade.Value} não encontrada.");
            }
            entidade.IsCliente = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else if (dto.NovaEntidade != null)
        {
            var novaEnt = await _entidadeService.CriarAsync(dto.NovaEntidade, cancellationToken);
            entidade = novaEnt;
            entidade.IsCliente = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("É necessário informar o IdEntidade ou os dados da NovaEntidade.");
        }

        var cliente = new Cliente
        {
            IdCliente = entidade.IdEntidade,
            LocalTrabalho = dto.LocalTrabalho,
            TelefoneTrabalho = dto.TelefoneTrabalho,
            Profissao = dto.Profissao,
            InscricaoProdutor = dto.InscricaoProdutor,
            CodigoAlternativo = dto.CodigoAlternativo,
            Ativo = dto.Ativo,
            Bloqueado = dto.Bloqueado,
            ItemFinanceiroPadrao = dto.ItemFinanceiroPadrao,
            EnviarCNDNFe = dto.EnviarCNDNFe,
            RendaMensal = dto.RendaMensal,
            LimiteCredito = dto.LimiteCredito,
            ValorAluguel = dto.ValorAluguel,
            DataAdmissao = dto.DataAdmissao,
            HoraCobranca = dto.HoraCobranca,
            ImovelTipo = dto.ImovelTipo,
            SituacaoSPC = dto.SituacaoSPC,
            IdCategoria = dto.IdCategoria,
            IdClienteConceito = dto.IdClienteConceito,
            IdDiaSemanaCobranca = dto.IdDiaSemanaCobranca,
            IdUsuarioInclusao = _contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Now
        };

        await _clienteRepository.AddAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {IdCliente} criado com sucesso.", cliente.IdCliente);

        return cliente;
    }

    public async Task AtualizarAsync(int id, CriarClienteDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            throw new InvalidOperationException($"Cliente com ID {id} não encontrado.");
        }

        cliente.LocalTrabalho = dto.LocalTrabalho;
        cliente.TelefoneTrabalho = dto.TelefoneTrabalho;
        cliente.Profissao = dto.Profissao;
        cliente.InscricaoProdutor = dto.InscricaoProdutor;
        cliente.CodigoAlternativo = dto.CodigoAlternativo;
        cliente.Ativo = dto.Ativo;
        cliente.Bloqueado = dto.Bloqueado;
        cliente.ItemFinanceiroPadrao = dto.ItemFinanceiroPadrao;
        cliente.EnviarCNDNFe = dto.EnviarCNDNFe;
        cliente.RendaMensal = dto.RendaMensal;
        cliente.LimiteCredito = dto.LimiteCredito;
        cliente.ValorAluguel = dto.ValorAluguel;
        cliente.DataAdmissao = dto.DataAdmissao;
        cliente.HoraCobranca = dto.HoraCobranca;
        cliente.ImovelTipo = dto.ImovelTipo;
        cliente.SituacaoSPC = dto.SituacaoSPC;
        cliente.IdCategoria = dto.IdCategoria;
        cliente.IdClienteConceito = dto.IdClienteConceito;
        cliente.IdDiaSemanaCobranca = dto.IdDiaSemanaCobranca;

        cliente.IdUsuarioAlteracao = _contexto.IdUsuario;
        cliente.DataAlteracao = DateTime.Today;
        cliente.HoraAlteracao = DateTime.Now;

        await _clienteRepository.UpdateAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente {IdCliente} atualizado com sucesso.", id);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente != null)
        {
            await _clienteRepository.DeleteAsync(cliente, cancellationToken);
            await _clienteRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Cliente {IdCliente} excluído com sucesso.", id);
        }
    }
}

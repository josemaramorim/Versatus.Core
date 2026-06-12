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

public class FuncionarioService : IFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IEntidadeRepository _entidadeRepository;
    private readonly IEntidadeService _entidadeService;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<FuncionarioService> _logger;

    public FuncionarioService(
        IFuncionarioRepository funcionarioRepository,
        IEntidadeRepository entidadeRepository,
        IEntidadeService entidadeService,
        IContextoExecucao contexto,
        ILogger<FuncionarioService> logger)
    {
        _funcionarioRepository = funcionarioRepository;
        _entidadeRepository = entidadeRepository;
        _entidadeService = entidadeService;
        _contexto = contexto;
        _logger = logger;
    }

    public async Task<IEnumerable<FuncionarioResponseDto>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        var funcionarios = await _funcionarioRepository.GetAllAsync(cancellationToken);
        return funcionarios.Select(f => new FuncionarioResponseDto(
            f.IdFuncionario,
            f.Entidade?.Nome ?? "Entidade Desconhecida",
            f.Ctps,
            f.Ativo
        ));
    }

    public async Task<Funcionario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _funcionarioRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Funcionario> CriarAsync(CriarFuncionarioDto dto, CancellationToken cancellationToken = default)
    {
        Entidade? entidade = null;

        if (dto.IdEntidade.HasValue)
        {
            entidade = await _entidadeRepository.GetByIdAsync(dto.IdEntidade.Value, cancellationToken);
            if (entidade == null)
            {
                throw new InvalidOperationException($"Entidade base com ID {dto.IdEntidade.Value} não encontrada.");
            }
            entidade.IsFuncionario = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else if (dto.NovaEntidade != null)
        {
            var novaEnt = await _entidadeService.CriarAsync(dto.NovaEntidade, cancellationToken);
            entidade = novaEnt;
            entidade.IsFuncionario = true;
            await _entidadeService.AtualizarAsync(entidade, cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("É necessário informar o IdEntidade ou os dados da NovaEntidade.");
        }

        var funcionario = new Funcionario
        {
            IdFuncionario = entidade.IdEntidade,
            Ctps = dto.Ctps,
            SerieCtps = dto.SerieCtps,
            UfCtps = dto.UfCtps,
            DataEmissaoCtps = dto.DataEmissaoCtps,
            NumeroCnh = dto.NumeroCnh,
            CategoriaCnh = dto.CategoriaCnh,
            DataVencimentoCnh = dto.DataVencimentoCnh,
            InscricaoPis = dto.InscricaoPis,
            IdBancoPis = dto.IdBancoPis,
            NumeroAgenciaPis = dto.NumeroAgenciaPis,
            NomeAgenciaPis = dto.NomeAgenciaPis,
            DataInscricaoPis = dto.DataInscricaoPis,
            NomePai = dto.NomePai,
            NomeMae = dto.NomeMae,
            Observacao = dto.Observacao,
            Ativo = dto.Ativo,
            IdRaca = dto.IdRaca,
            IdTipoDeficiencia = dto.IdTipoDeficiencia,
            IdPais = dto.IdPais,
            IdUsuarioInclusao = _contexto.IdUsuario,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Now
        };

        await _funcionarioRepository.AddAsync(funcionario, cancellationToken);
        await _funcionarioRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Funcionário {IdFuncionario} criado com sucesso.", funcionario.IdFuncionario);

        return funcionario;
    }

    public async Task AtualizarAsync(int id, CriarFuncionarioDto dto, CancellationToken cancellationToken = default)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id, cancellationToken);
        if (funcionario == null)
        {
            throw new InvalidOperationException($"Funcionário com ID {id} não encontrado.");
        }

        funcionario.Ctps = dto.Ctps;
        funcionario.SerieCtps = dto.SerieCtps;
        funcionario.UfCtps = dto.UfCtps;
        funcionario.DataEmissaoCtps = dto.DataEmissaoCtps;
        funcionario.NumeroCnh = dto.NumeroCnh;
        funcionario.CategoriaCnh = dto.CategoriaCnh;
        funcionario.DataVencimentoCnh = dto.DataVencimentoCnh;
        funcionario.InscricaoPis = dto.InscricaoPis;
        funcionario.IdBancoPis = dto.IdBancoPis;
        funcionario.NumeroAgenciaPis = dto.NumeroAgenciaPis;
        funcionario.NomeAgenciaPis = dto.NomeAgenciaPis;
        funcionario.DataInscricaoPis = dto.DataInscricaoPis;
        funcionario.NomePai = dto.NomePai;
        funcionario.NomeMae = dto.NomeMae;
        funcionario.Observacao = dto.Observacao;
        funcionario.Ativo = dto.Ativo;
        funcionario.IdRaca = dto.IdRaca;
        funcionario.IdTipoDeficiencia = dto.IdTipoDeficiencia;
        funcionario.IdPais = dto.IdPais;

        funcionario.IdUsuarioAlteracao = _contexto.IdUsuario;
        funcionario.DataAlteracao = DateTime.Today;
        funcionario.HoraAlteracao = DateTime.Now;

        await _funcionarioRepository.UpdateAsync(funcionario, cancellationToken);
        await _funcionarioRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Funcionário {IdFuncionario} atualizado com sucesso.", id);
    }

    public async Task ExcluirAsync(int id, CancellationToken cancellationToken = default)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id, cancellationToken);
        if (funcionario != null)
        {
            await _funcionarioRepository.DeleteAsync(funcionario, cancellationToken);
            await _funcionarioRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Funcionário {IdFuncionario} excluído com sucesso.", id);
        }
    }
}

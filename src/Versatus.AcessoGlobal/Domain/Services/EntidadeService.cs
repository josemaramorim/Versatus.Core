using Microsoft.Extensions.Logging;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;

namespace Versatus.AcessoGlobal.Domain.Services;

public class EntidadeService : IEntidadeService
{
    private readonly IEntidadeRepository _repository;
    private readonly IGeradorSequencial _geradorSequencial;
    private readonly IContextoExecucao _contexto;
    private readonly ILogger<EntidadeService> _logger;

    public EntidadeService(
        IEntidadeRepository repository,
        IGeradorSequencial geradorSequencial,
        IContextoExecucao contexto,
        ILogger<EntidadeService> logger)
    {
        _repository = repository;
        _geradorSequencial = geradorSequencial;
        _contexto = contexto;
        _logger = logger;
    }

    public async Task<Entidade> CriarAsync(Entidade entidade, CancellationToken cancellationToken = default)
    {
        // 1. Validações de unicidade
        await ValidarUnicidadeAsync(entidade, cancellationToken);

        // 2. Geração de Sequencial (Padrão legado: Tabela "Entidade")
        entidade.IdEntidade = await _geradorSequencial.ProximoAsync("Entidade", SequencialTipo.Geral, cancellationToken);

        // 3. Auditoria Inclusão
        entidade.IdUsuarioInclusao = _contexto.IdUsuario;
        entidade.DataInclusao = DateTime.Today;
        entidade.HoraInclusao = DateTime.Now;

        // 4. Salvar
        await _repository.AddAsync(entidade, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entidade {IdEntidade} ({Nome}) criada com sucesso.", entidade.IdEntidade, entidade.Nome);

        return entidade;
    }

    public async Task AtualizarAsync(Entidade entidade, CancellationToken cancellationToken = default)
    {
        // 1. Auditoria Alteração
        entidade.IdUsuarioAlteracao = _contexto.IdUsuario;
        entidade.DataAlteracao = DateTime.Today;
        entidade.HoraAlteracao = DateTime.Now;

        // 2. Atualizar
        await _repository.UpdateAsync(entidade, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entidade {IdEntidade} atualizada com sucesso.", entidade.IdEntidade);
    }

    private async Task ValidarUnicidadeAsync(Entidade entidade, CancellationToken cancellationToken)
    {
        if (entidade.PessoaFisica != null && !string.IsNullOrWhiteSpace(entidade.PessoaFisica.Cpf))
        {
            var existente = await _repository.GetByCpfAsync(entidade.PessoaFisica.Cpf, cancellationToken);
            if (existente != null)
            {
                throw new InvalidOperationException($"Já existe uma entidade cadastrada com o CPF {entidade.PessoaFisica.Cpf}.");
            }
        }

        if (entidade.PessoaJuridica != null && !string.IsNullOrWhiteSpace(entidade.PessoaJuridica.Cnpj))
        {
            var existente = await _repository.GetByCnpjAsync(entidade.PessoaJuridica.Cnpj, cancellationToken);
            if (existente != null)
            {
                throw new InvalidOperationException($"Já existe uma entidade cadastrada com o CNPJ {entidade.PessoaJuridica.Cnpj}.");
            }
        }
    }
}

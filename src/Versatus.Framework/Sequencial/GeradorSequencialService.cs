using System.Threading;
using System.Threading.Tasks;
using Versatus.Framework.Contexto;
using Versatus.Framework.Domain.Entities;

namespace Versatus.Framework.Sequencial;

/// <summary>
/// Serviço para geração de números sequenciais customizados seguindo a lógica do legado.
/// </summary>
public class GeradorSequencialService : IGeradorSequencial
{
    private readonly ISequenciaRepository _repository;
    private readonly IContextoExecucao _contexto;

    public GeradorSequencialService(ISequenciaRepository repository, IContextoExecucao contexto)
    {
        _repository = repository;
        _contexto = contexto;
    }

    /// <inheritdoc />
    public async Task<int> ProximoAsync(string nomeObjeto, SequencialTipo tipo, CancellationToken cancellationToken = default)
    {
        int idContexto = tipo switch
        {
            SequencialTipo.Filial => _contexto.IdFilial,
            SequencialTipo.Empresa => _contexto.IdEmpresa,
            _ => 0
        };

        var sequencia = await _repository.GetByTabelaFilialAsync(nomeObjeto, idContexto, cancellationToken);

        if (sequencia == null)
        {
            int valorInicial = nomeObjeto.Equals("Sequencial", System.StringComparison.OrdinalIgnoreCase) ? 2 : 1;
            
            sequencia = new Sequencia
            {
                Tabela = nomeObjeto,
                IdFilial = idContexto,
                ValorAtual = valorInicial
            };

            await _repository.AddAsync(sequencia, cancellationToken);
            return valorInicial;
        }

        sequencia.ValorAtual++;
        await _repository.UpdateAsync(sequencia, cancellationToken);

        return sequencia.ValorAtual;
    }

    /// <inheritdoc />
    public async Task SetValorAsync(string nomeObjeto, SequencialTipo tipo, int novoValor, CancellationToken cancellationToken = default)
    {
        int idContexto = tipo switch
        {
            SequencialTipo.Filial => _contexto.IdFilial,
            SequencialTipo.Empresa => _contexto.IdEmpresa,
            _ => 0
        };

        var sequencia = await _repository.GetByTabelaFilialAsync(nomeObjeto, idContexto, cancellationToken);

        if (sequencia == null)
        {
            sequencia = new Sequencia
            {
                Tabela = nomeObjeto,
                IdFilial = idContexto,
                ValorAtual = novoValor
            };
            await _repository.AddAsync(sequencia, cancellationToken);
        }
        else
        {
            sequencia.ValorAtual = novoValor;
            await _repository.UpdateAsync(sequencia, cancellationToken);
        }
    }
}

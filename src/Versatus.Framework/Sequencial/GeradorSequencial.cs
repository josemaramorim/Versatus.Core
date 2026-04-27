using Versatus.Framework.Domain.Entities;
using Versatus.Framework.Common;

namespace Versatus.Framework.Sequencial;

/// <summary>
/// Implementação do gerador de sequenciais usando tabela de banco.
/// CRÍTICO: Preserva integridade, usa transações.
/// </summary>
public class GeradorSequencial : IGeradorSequencial
{
    private readonly ISequenciaRepository _repository;

    public GeradorSequencial(ISequenciaRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ProximoAsync(string tabela, int idFilial)
    {
        // Busca ou cria sequência
        var sequencia = await _repository.GetByTabelaFilialAsync(tabela, idFilial);
        if (sequencia == null)
        {
            sequencia = new Sequencia { Tabela = tabela, IdFilial = idFilial, ValorAtual = 0 };
            await _repository.AddAsync(sequencia);
        }

        // Incrementa e salva
        sequencia.ValorAtual++;
        await _repository.UpdateAsync(sequencia);

        return sequencia.ValorAtual;
    }
}
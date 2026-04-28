using System.Threading;
using System.Threading.Tasks;

namespace Versatus.Framework.Sequences;

/// <summary>
/// Contrato para geração de números sequenciais customizados, substituindo o GeradorSequencial legado.
/// </summary>
public interface IGeradorSequencial
{
    /// <summary>
    /// Gera o próximo número sequencial para um determinado objeto/tabela.
    /// </summary>
    /// <param name="nomeObjeto">Nome identificador da regra de sequencial (geralmente nome da tabela).</param>
    /// <param name="tipo">Escopo do sequencial (Geral, Empresa ou Filial).</param>
    /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
    /// <returns>Próximo número disponível.</returns>
    Task<int> ProximoAsync(string nomeObjeto, SequencialTipo tipo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Define manualmente um valor para o sequencial.
    /// </summary>
    Task SetValorAsync(string nomeObjeto, SequencialTipo tipo, int novoValor, CancellationToken cancellationToken = default);
}

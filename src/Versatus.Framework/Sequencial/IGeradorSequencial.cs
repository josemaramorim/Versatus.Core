namespace Versatus.Framework.Sequencial;

/// <summary>
/// Gerador de sequenciais por filial, replicando o comportamento legado.
/// CRÍTICO: Preserva integridade do banco, não usar IDENTITY.
/// </summary>
public interface IGeradorSequencial
{
    /// <summary>
    /// Gera o próximo ID sequencial para a tabela e filial especificadas.
    /// </summary>
    /// <param name="tabela">Nome da tabela (ex: "Cliente", "Produto")</param>
    /// <param name="idFilial">ID da filial</param>
    /// <returns>Próximo ID sequencial</returns>
    Task<int> ProximoAsync(string tabela, int idFilial);
}
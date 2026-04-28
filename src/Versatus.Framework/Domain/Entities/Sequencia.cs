namespace Versatus.Framework.Domain.Entities;

/// <summary>
/// Entidade para tabela de sequenciais, replicando o legado.
/// Chave composta: Tabela + IdFilial
/// </summary>
public class Sequencia
{
    public string Tabela { get; set; } = string.Empty;

    public int IdFilial { get; set; }

    public int ValorAtual { get; set; }
}
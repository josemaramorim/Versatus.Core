using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versatus.Framework.Domain.Entities;

/// <summary>
/// Entidade para tabela de sequenciais, replicando o legado.
/// Chave composta: Tabela + IdFilial
/// </summary>
[Table("Sequencial")]
public class Sequencia
{
    [Key]
    [Column("Tabela")]
    public string Tabela { get; set; } = string.Empty;

    [Key]
    [Column("IdFilial")]
    public int IdFilial { get; set; }

    [Column("ValorAtual")]
    public int ValorAtual { get; set; }
}
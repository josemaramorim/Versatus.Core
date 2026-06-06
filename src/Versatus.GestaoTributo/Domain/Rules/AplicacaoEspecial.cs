using System;

namespace Versatus.GestaoTributo.Domain.Rules;

public class AplicacaoEspecial
{
    public int IdAplicacaoProduto { get; set; }
    public int IdTributoFormula { get; set; }
    public string? SufixoEspecial { get; set; }
    public int IdAplicacaoProdutoEspecial { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public AplicacaoProduto? AplicacaoProduto { get; set; }
    public AplicacaoProduto? AplicacaoProdutoEspecial { get; set; }
}

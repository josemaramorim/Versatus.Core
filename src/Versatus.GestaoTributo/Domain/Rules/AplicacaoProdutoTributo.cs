using System;
using Versatus.GestaoTributo.Domain.Classification;

namespace Versatus.GestaoTributo.Domain.Rules;

public class AplicacaoProdutoTributo
{
    public int IdAplicacaoProduto { get; set; }
    public int IdTributo { get; set; }
    public int IdTributacao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public AplicacaoProduto? AplicacaoProduto { get; set; }
    public TributoFiscal? Tributo { get; set; }
    public Tributacao? Tributacao { get; set; }
}

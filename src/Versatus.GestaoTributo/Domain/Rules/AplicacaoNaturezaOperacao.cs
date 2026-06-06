using System;

namespace Versatus.GestaoTributo.Domain.Rules;

public class AplicacaoNaturezaOperacao
{
    public int IdAplicacaoNaturezaOperacao { get; set; }
    public int IdAplicacaoProduto { get; set; }
    public int IdNaturezaOperacao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }

    // Relacionamentos
    public AplicacaoProduto? AplicacaoProduto { get; set; }
}

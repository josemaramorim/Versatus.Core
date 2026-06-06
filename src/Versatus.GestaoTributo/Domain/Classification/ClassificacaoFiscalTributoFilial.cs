namespace Versatus.GestaoTributo.Domain.Classification;

public class ClassificacaoFiscalTributoFilial
{
    public int IdClassificacaoFiscalTributo { get; set; }
    public int IdFilial { get; set; } // Referencia GloFilial (MOD-02)
    public decimal? Aliquota { get; set; }

    // Relacionamentos
    public ClassificacaoFiscalTributo? ClassificacaoFiscalTributo { get; set; }
}

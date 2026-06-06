using System.Collections.Generic;

namespace Versatus.GestaoTributo.Domain.Classification;

public class ClassificacaoFiscalTributo
{
    public int IdClassificacaoFiscalTributo { get; set; }
    public int IdClassificacaoFiscal { get; set; }
    public int IdTributoFiscal { get; set; }
    public bool UsaAliquota { get; set; }
    public decimal? Aliquota { get; set; }
    public bool PossuiAliquotaFilial { get; set; }

    // Relacionamentos
    public ClassificacaoFiscal? ClassificacaoFiscal { get; set; }
    public TributoFiscal? TributoFiscal { get; set; }

    private readonly List<ClassificacaoFiscalTributoFilial> _aliquotasFilial = [];
    public IReadOnlyList<ClassificacaoFiscalTributoFilial> AliquotasFilial => _aliquotasFilial.AsReadOnly();
}

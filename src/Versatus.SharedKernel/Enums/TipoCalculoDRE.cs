namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Origem do valor de um título de DRE (movimento com rateio, fórmula, avulso, custo de
/// venda...). Épico E11.
/// Origem: Projeto.Geral.Enumerado.TipoCalculoDRE (legado, [TipoEnumerado(1150)]) —
/// persistido em FINDRETITULO.IDTIPOCALCULODRE.
/// </summary>
public enum TipoCalculoDRE
{
    MovimentoRateio = 1151,
    Formula = 1152,
    Avulso = 1153,
    CustoVenda = 1168,
    CustoVendaTipoProduto = 1166,
    CustoVendaGrupoEstoque = 1167
}

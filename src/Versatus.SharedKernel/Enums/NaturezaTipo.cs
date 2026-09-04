namespace Versatus.SharedKernel.Enums;

/// <summary>
/// Natureza de um item financeiro (credora/devedora) — usado por 38 classes do legado.
/// Origem: Projeto.Geral.Enumerado.NaturezaTipo (legado, [TipoEnumerado(29)]) — persistido
/// em FINITEMFINANCEIRO.IDNATUREZA, rateio, DRE. Não confundir com a FK IDGLONATUREZA.
/// </summary>
public enum NaturezaTipo
{
    Credora = 30,
    Devedora = 31
}

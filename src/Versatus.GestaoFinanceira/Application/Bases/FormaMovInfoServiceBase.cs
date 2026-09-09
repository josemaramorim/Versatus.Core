namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/FormaMovInfo.cs:DefinirValor (legado).
// Cobre: VAL-E1-31 (matriz-rtv.md#E1).
//
// Base pequena — o comportamento concreto está nas 11 FormaMovInfo* do épico E5. Aqui só a
// regra de teto do valor disponível, que é pura.
public abstract class FormaMovInfoServiceBase
{
    /// <summary>
    /// VAL-E1-31 — se <paramref name="limitarValorDisponivel"/>: devolve 0 quando o
    /// disponível é ≤ 0; senão limita o valor ao disponível. Ajuste, não erro.
    /// </summary>
    protected static decimal AplicarTetoValorDisponivel(decimal valor, decimal valorDisponivel, bool limitarValorDisponivel)
    {
        if (!limitarValorDisponivel)
            return valor;

        if (valorDisponivel <= 0m)
            return 0m;

        return valor > valorDisponivel ? valorDisponivel : valor;
    }
}

using FluentAssertions;
using Versatus.GestaoFinanceira.Application.Bases;

namespace Versatus.GestaoFinanceira.Tests.E1;

/// <summary>
/// VAL-E1-31 (matriz-rtv.md#E1) — teto do valor disponível em
/// <c>FormaMovInfo.DefinirValor</c> (legado). Ajuste, não erro.
/// </summary>
public class FormaMovInfoTetoTests
{
    private sealed class Servico : FormaMovInfoServiceBase
    {
        public static decimal Teto(decimal valor, decimal disponivel, bool limitar) => AplicarTetoValorDisponivel(valor, disponivel, limitar);
    }

    [Theory]
    [InlineData(100, 500, false, 100)]  // sem limite -> passa
    [InlineData(100, 500, true, 100)]   // dentro do disponível
    [InlineData(800, 500, true, 500)]   // acima -> limita
    [InlineData(100, 0, true, 0)]       // disponível 0 -> zera
    [InlineData(100, -1, true, 0)]      // disponível negativo -> zera
    public void VAL_E1_31_AplicarTetoValorDisponivel(decimal valor, decimal disponivel, bool limitar, decimal esperado)
        => Servico.Teto(valor, disponivel, limitar).Should().Be(esperado);
}

using FluentAssertions;
using Versatus.GestaoFinanceira.Application.Bases;

namespace Versatus.GestaoFinanceira.Tests.E1;

/// <summary>
/// Paridade de <see cref="ArredondamentoFinanceiro"/> com <c>Projeto.Geral.Funcoes.Arredondar</c>
/// do legado (<c>projeto_tag_1906/geral/Funcoes.cs:712</c>): arredonda 0,5 **para longe do zero**.
/// </summary>
public class ArredondamentoFinanceiroTests
{
    [Theory]
    [InlineData(0, 2, 0)]
    [InlineData(2.344, 2, 2.34)]
    [InlineData(2.345, 2, 2.35)]   // meio → sobe (≠ banker's, que daria 2.34)
    [InlineData(2.355, 2, 2.36)]
    [InlineData(-2.345, 2, -2.35)] // meio no negativo → afasta do zero
    [InlineData(-2.344, 2, -2.34)]
    [InlineData(10.201, 2, 10.20)]
    [InlineData(4.99995, 2, 5.00)]
    [InlineData(123.456, 0, 123)]
    [InlineData(123.5, 0, 124)]
    public void Arredondar_bate_com_o_legado(decimal valor, int casas, decimal esperado)
        => ArredondamentoFinanceiro.Arredondar(valor, casas).Should().Be(esperado);
}

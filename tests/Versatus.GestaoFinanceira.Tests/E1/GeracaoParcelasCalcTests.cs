using System.Globalization;
using FluentAssertions;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.GestaoFinanceira.Domain.Bases;

namespace Versatus.GestaoFinanceira.Tests.E1;

/// <summary>
/// Golden de paridade das fórmulas puras de parcela (matriz-rot.md#E1 CALC-E1-07/08),
/// transcritas de <c>ParcelaBase.cs</c> (legado). Igualdade exata de <c>decimal</c>.
/// </summary>
public class GeracaoParcelasCalcTests
{
    private sealed class Parcelas : GeracaoParcelasService
    {
        public static decimal ValorMinimo(decimal total, decimal percDiv, decimal percMin) => CalcularValorMinimo(total, percDiv, percMin);
        public static decimal PercentualNovo(decimal valorParcela, decimal total) => CalcularPercentualNovo(valorParcela, total);

        protected override ValidationResult ValidarVencimento(ParcelaBase p, DateTime v) => throw new NotSupportedException();
        protected override ValidationResult ValidarVencimentoParcelaAlterada(ParcelaBase p, int i, DateTime v) => throw new NotSupportedException();
        protected override ValidationResult ValidarNumeroParcela(ParcelaBase p, int n) => throw new NotSupportedException();
        protected override ValidationResult ValidarValorMinimo(ParcelaBase p, decimal v) => throw new NotSupportedException();
        protected override ValidationResult ValidarValor(ParcelaBase p, decimal v) => throw new NotSupportedException();
        protected override ValidationResult ValidarCaixaBanco(ParcelaBase p) => throw new NotSupportedException();
        protected override IReadOnlyList<ParcelaBase> Rebalancear(IReadOnlyList<ParcelaBase> parcelas, int i, decimal total) => throw new NotSupportedException();
    }

    public static IEnumerable<object[]> Golden() =>
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "E1", "golden", "CALC-E1-07-08-parcela.csv"))
            .Skip(1)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Split(';'))
            .Select(c => new object[]
            {
                c[0], c[1],
                decimal.Parse(c[2], CultureInfo.InvariantCulture),
                decimal.Parse(c[3], CultureInfo.InvariantCulture),
                string.IsNullOrEmpty(c[4]) ? 0m : decimal.Parse(c[4], CultureInfo.InvariantCulture),
                decimal.Parse(c[5], CultureInfo.InvariantCulture)
            });

    [Theory]
    [MemberData(nameof(Golden))]
    public void CALC_E1_07_08(string calc, string caso, decimal p1, decimal p2, decimal p3, decimal esperado)
    {
        decimal resultado = calc switch
        {
            "CALC-E1-07" => Parcelas.ValorMinimo(p1, p2, p3),
            "CALC-E1-08" => Parcelas.PercentualNovo(p1, p2),
            _ => throw new InvalidOperationException(calc)
        };
        resultado.Should().Be(esperado, $"{calc} · {caso}");
    }
}

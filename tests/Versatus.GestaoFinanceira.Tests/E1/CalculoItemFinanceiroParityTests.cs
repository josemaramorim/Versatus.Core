using System.Globalization;
using FluentAssertions;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E1;

/// <summary>
/// Golden tests de paridade dos cálculos de item financeiro (matriz-rot.md#E1
/// CALC-E1-01..06). Fórmulas transcritas de <c>ItemFinanceiroBase.cs</c> (legado) sem
/// refatorar. `ExecutarCalculo` auto-arredonda (%): igualdade exata. `CalcularComposto`
/// devolve soma não arredondada: comparação na escala monetária (2 casas), que é como o
/// valor é consumido.
/// </summary>
public class CalculoItemFinanceiroParityTests
{
    // Expõe os métodos protegidos da base para o teste.
    private sealed class Calc : CalculadoraItemFinanceiroBase
    {
        public static decimal Executar(decimal vb, decimal v, CalculoItemFinanceiro c) => ExecutarCalculo(vb, v, c);
        public static decimal Composto(decimal vb, decimal v, int d, CalculoItemFinanceiro c) => CalcularComposto(vb, v, d, c);
        public static int Dias(DateTime i, DateTime f, ItemFinanceiroAplicar a) => CalcularDias(i, f, a);
        public static decimal ConsideraDias(decimal vc, bool comp, bool cons) => ConsiderarDiasParaCalculo(vc, comp, cons);
        public static decimal ConsideraDias(decimal vc, bool comp, bool cons, int d) => ConsiderarDiasParaCalculo(vc, comp, cons, d);
    }

    public static IEnumerable<object[]> GoldenExecutarCalculo() =>
        LerGolden("CALC-E1-01-executar-calculo.csv")
            .Select(c => new object[]
            {
                c["caso"],
                decimal.Parse(c["valorBase"], CultureInfo.InvariantCulture),
                decimal.Parse(c["valor"], CultureInfo.InvariantCulture),
                Enum.Parse<CalculoItemFinanceiro>(c["calculo"]),
                decimal.Parse(c["esperado"], CultureInfo.InvariantCulture)
            });

    [Theory]
    [MemberData(nameof(GoldenExecutarCalculo))]
    public void CALC_E1_01_ExecutarCalculo(string caso, decimal valorBase, decimal valor, CalculoItemFinanceiro calculo, decimal esperado)
        => Calc.Executar(valorBase, valor, calculo).Should().Be(esperado, caso);

    public static IEnumerable<object[]> GoldenComposto() =>
        LerGolden("CALC-E1-02-calcular-composto.csv")
            .Select(c => new object[]
            {
                c["caso"],
                decimal.Parse(c["valorBase"], CultureInfo.InvariantCulture),
                decimal.Parse(c["valor"], CultureInfo.InvariantCulture),
                int.Parse(c["dias"], CultureInfo.InvariantCulture),
                Enum.Parse<CalculoItemFinanceiro>(c["calculo"]),
                decimal.Parse(c["esperado"], CultureInfo.InvariantCulture)
            });

    [Theory]
    [MemberData(nameof(GoldenComposto))]
    public void CALC_E1_02_CalcularComposto(string caso, decimal valorBase, decimal valor, int dias, CalculoItemFinanceiro calculo, decimal esperado)
    {
        var resultado = Calc.Composto(valorBase, valor, dias, calculo);
        Math.Round(resultado, 2, MidpointRounding.AwayFromZero).Should().Be(esperado, caso);
    }

    [Theory]
    [InlineData("2026-02-01", "2026-01-20", ItemFinanceiroAplicar.AntesVencimento, 12)]
    [InlineData("2026-02-01", "2026-01-20", ItemFinanceiroAplicar.DepoisVencimento, -12)]
    [InlineData("2026-02-01", "2026-01-20", ItemFinanceiroAplicar.NaoAplicar, 0)]
    [InlineData("2026-03-01", "2026-03-15", ItemFinanceiroAplicar.DepoisVencimento, 14)]
    public void CALC_E1_03_CalcularDias(string dataInicial, string dataFinal, ItemFinanceiroAplicar aplicar, int esperado)
        => Calc.Dias(DateTime.Parse(dataInicial, CultureInfo.InvariantCulture),
                     DateTime.Parse(dataFinal, CultureInfo.InvariantCulture), aplicar)
               .Should().Be(esperado);

    [Theory]
    [InlineData(30, false, true, 1)]      // não composto + considera dias -> /30
    [InlineData(30, false, false, 30)]    // não considera -> inalterado
    [InlineData(30, true, true, 30)]      // composto -> inalterado
    public void CALC_E1_04_ConsiderarDiasParaCalculo_sem_dias(decimal valorCalculo, bool composto, bool consideraDias, decimal esperado)
        => Calc.ConsideraDias(valorCalculo, composto, consideraDias).Should().Be(esperado);

    [Theory]
    [InlineData(30, false, true, 15, 15)]   // 30/30*15
    [InlineData(30, false, true, 45, 45)]   // 30/30*45
    [InlineData(30, true, true, 15, 30)]    // composto -> inalterado
    public void CALC_E1_04_ConsiderarDiasParaCalculo_com_dias(decimal valorCalculo, bool composto, bool consideraDias, int dias, decimal esperado)
        => Calc.ConsideraDias(valorCalculo, composto, consideraDias, dias).Should().Be(esperado);

    // --- infra de leitura dos golden CSV (delimitador ';', cultura invariante) ---
    private static IEnumerable<Dictionary<string, string>> LerGolden(string arquivo)
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "E1", "golden", arquivo);
        var linhas = File.ReadAllLines(caminho);
        var cab = linhas[0].Split(';');
        foreach (var l in linhas.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            var campos = l.Split(';');
            yield return cab.Select((h, i) => (h, v: i < campos.Length ? campos[i] : ""))
                            .ToDictionary(p => p.h, p => p.v);
        }
    }
}

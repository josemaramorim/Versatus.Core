using FluentAssertions;
using Versatus.GestaoFinanceira.Domain.Services;
using static Versatus.GestaoFinanceira.Tests.E3.GoldenE3;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T09 — paridade dos saldos com o legado (igualdade exata de decimal, sem tolerância):
/// CALC-E3-01/02 (SaldoCaixaBanco.Saldo / SaldoConciliado), CALC-E3-03 (RetornarSaldoCaixaBanco),
/// CALC-E3-04 (SaldoRateio, 8 casas). Divergência = bug do código novo, nunca do golden.
/// </summary>
public class SaldoParityTests
{
    public static IEnumerable<object[]> GoldenSaldo() =>
        Ler("CALC-E3-01-02-saldo.csv").Select(c => new object[]
            { c["caso"], Dec(c["saldoAnterior"]), Dec(c["totalCredito"]), Dec(c["totalDebito"]), Dec(c["esperado"]) });

    [Theory]
    [MemberData(nameof(GoldenSaldo))]
    public void CALC_E3_01_02_Saldo(string caso, decimal saldoAnterior, decimal totalCredito, decimal totalDebito, decimal esperado)
        => SaldoCalculadora.CalcularSaldo(saldoAnterior, totalCredito, totalDebito).Should().Be(esperado, caso);

    public static IEnumerable<object[]> GoldenRetorno() =>
        Ler("CALC-E3-03-retornar-saldo.csv").Select(c => new object?[]
            { c["caso"], c["inicialSemData"] == "true", DecOuNull(c["saldo"]), Dec(c["esperado"]) }!);

    [Theory]
    [MemberData(nameof(GoldenRetorno))]
    public void CALC_E3_03_RetornarSaldo(string caso, bool inicialSemData, decimal? saldo, decimal esperado)
        => SaldoCalculadora.CalcularRetornoSaldo(inicialSemData, saldo).Should().Be(esperado, caso);

    public static IEnumerable<object[]> GoldenRateio() =>
        Ler("CALC-E3-04-saldo-rateio.csv").Select(c => new object[]
            { c["caso"], Dec(c["saldoAnterior"]), Dec(c["totalCredito"]), Dec(c["totalDebito"]), Dec(c["esperado"]) });

    [Theory]
    [MemberData(nameof(GoldenRateio))]
    public void CALC_E3_04_SaldoRateio(string caso, decimal saldoAnterior, decimal totalCredito, decimal totalDebito, decimal esperado)
        => SaldoCalculadora.CalcularSaldoRateio(saldoAnterior, totalCredito, totalDebito).Should().Be(esperado, caso);
}

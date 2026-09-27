using FluentAssertions;
using Versatus.GestaoFinanceira.Application.Bases;
using static Versatus.GestaoFinanceira.Tests.E3.GoldenE3;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T09 — paridade do conversor por índice com o legado (IndiceConversor.cs):
/// CALC-E3-05 (CalcularConversao), CALC-E3-06 (RetornarDataIndiceValida + Feriado.DiaUtil),
/// CALC-E3-07 (RetornarIndice — posição na lista de valores). Índice padrão = 1.
/// </summary>
public class ConversorIndiceParityTests
{
    private const int IdPadrao = 1;

    public static IEnumerable<object[]> GoldenConversao() =>
        Ler("CALC-E3-05-conversao.csv").Select(c => new object[]
        {
            c["caso"], int.Parse(c["idOrigem"]), int.Parse(c["idDestino"]), Dec(c["valor"]),
            Dec(c["valorIndiceOrigem"]), Dec(c["valorIndiceDestino"]), int.Parse(c["decimais"]), Dec(c["esperado"]),
        });

    [Theory]
    [MemberData(nameof(GoldenConversao))]
    public void CALC_E3_05_CalcularConversao(string caso, int idOrigem, int idDestino, decimal valor,
        decimal valorIndiceOrigem, decimal valorIndiceDestino, int decimais, decimal esperado)
        => ConversorIndiceService.CalcularConversao(idOrigem, idDestino, IdPadrao, valor, valorIndiceOrigem, valorIndiceDestino, decimais)
            .Should().Be(esperado, caso);

    // Mesmos feriados usados no gerador: GLOFERIADO do banco de dev + 1 feriado por data (Carnaval 2026).
    private static readonly HashSet<string> FeriadosDiaMes = ["01/01", "21/04", "01/05", "07/09", "12/10", "02/11", "15/11", "25/12"];
    private static readonly HashSet<DateTime> FeriadosData = [new DateTime(2026, 2, 17)];

    private static Task<bool> DiaUtil(DateTime d)
        => Task.FromResult(ConversorIndiceService.DiaUtil(d, FeriadosDiaMes.Contains(d.ToString("dd'/'MM")) || FeriadosData.Contains(d.Date)));

    public static IEnumerable<object[]> GoldenData() =>
        Ler("CALC-E3-06-data-indice.csv").Select(c => new object[] { c["caso"], Data(c["data"]), int.Parse(c["modo"]), Data(c["esperado"]) });

    [Theory]
    [MemberData(nameof(GoldenData))]
    public async Task CALC_E3_06_RetornarDataIndiceValida(string caso, DateTime data, int modo, DateTime esperado)
        => (await ConversorIndiceService.RetornarDataIndiceValidaAsync(data, modo == 0 ? null : modo, DiaUtil))
            .Should().Be(esperado, caso);

    private static readonly IReadOnlyDictionary<string, decimal[]> Listas = new Dictionary<string, decimal[]>
    {
        ["mensal"] = [1.01m, 1.02m, 1.03m, 0m, 1.05m, 1.06m, 1.07m, 1.08m, 1.09m, 1.10m, 1.11m, 1.12m],
        ["diario"] = [.. Enumerable.Range(1, 30).Select(k => 5m + k / 1000m)],
        ["vazia"] = [],
    };

    public static IEnumerable<object[]> GoldenValorIndice() =>
        Ler("CALC-E3-07-valor-indice.csv").Select(c => new object[]
            { c["caso"], c["ehPadrao"] == "true", int.Parse(c["tipoCorrecao"]), Data(c["data"]), c["lista"], c["esperado"] });

    [Theory]
    [MemberData(nameof(GoldenValorIndice))]
    public void CALC_E3_07_RetornarIndice(string caso, bool ehPadrao, int tipoCorrecao, DateTime data, string lista, string esperado)
    {
        // Índice padrão vale 1 sem consultar a lista (RetornarIndice:80-81) — tratado no serviço.
        decimal? valor = ehPadrao ? 1.00m : ConversorIndiceService.SelecionarValorIndice(Listas[lista], tipoCorrecao, data);

        if (esperado.StartsWith("ERRO:"))
            valor.Should().BeNull(caso);
        else
            valor.Should().Be(Dec(esperado), caso);
    }
}

using System.Globalization;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// Leitura dos golden values do E3 (E3/golden/*.csv), gerados executando o código legado em .NET
/// Framework 4 — specs/modulos/MOD-05/golden/legado/GeradorGoldenE3.cs (coluna origem=legado).
/// </summary>
internal static class GoldenE3
{
    public static IEnumerable<Dictionary<string, string>> Ler(string arquivo)
    {
        var linhas = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "E3", "golden", arquivo));
        var cabecalho = linhas[0].Split(';');
        foreach (var linha in linhas.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            var campos = linha.Split(';');
            yield return cabecalho.Select((h, i) => (h, v: i < campos.Length ? campos[i] : ""))
                .ToDictionary(p => p.h, p => p.v);
        }
    }

    public static decimal Dec(string texto) => decimal.Parse(texto, NumberStyles.Float, CultureInfo.InvariantCulture);

    public static decimal? DecOuNull(string texto) => string.IsNullOrEmpty(texto) ? null : Dec(texto);

    public static DateTime Data(string texto) => DateTime.ParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture);
}

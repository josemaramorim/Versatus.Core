namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Porta cross-módulo (MOD-02) para índices econômicos — usada pelo ConversorIndiceService
/// (E3-T09, CALC-E3-05..07). Tabelas GLOINDICEECONOMICO / GLOINDICEECONOMICOVALOR.
/// </summary>
public interface IIndiceEconomicoConsulta
{
    /// <summary><c>null</c> quando o índice não existe.</summary>
    Task<IndiceEconomicoInfo?> ObterAsync(int idIndiceEconomico, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valores do índice entre <paramref name="inicio"/> e <paramref name="fim"/> (inclusive), na ordem
    /// de DATAINDICE — IndiceEconomico.RetornarListaIndice (legado). VALOR nulo vira 0.
    /// </summary>
    Task<IReadOnlyList<decimal>> ListarValoresAsync(int idIndiceEconomico, DateTime inicio, DateTime fim,
        CancellationToken cancellationToken = default);
}

/// <param name="IdTipoCorrecao">GLOINDICEECONOMICO.IDTIPOCORRECAO (109 Diário · 110 Mensal · 0/nulo = não definido).</param>
/// <param name="IdModoCorrecao">GLOINDICEECONOMICO.IDMODOCORRECAO (210 dia · 106 data anterior · 107 posterior).</param>
public sealed record IndiceEconomicoInfo(int IdIndiceEconomico, string Sigla, int? IdTipoCorrecao, int? IdModoCorrecao);

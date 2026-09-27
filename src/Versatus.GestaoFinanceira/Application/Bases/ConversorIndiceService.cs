using System.Globalization;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Bases;

/// <summary>Resultado da conversão — IndiceConversor.IndiceOrigem / IndiceDestino / ValorConvertido (legado).</summary>
public sealed record ConversaoIndice(decimal ValorOriginal, decimal ValorConvertido, decimal ValorIndiceOrigem, decimal ValorIndiceDestino);

/// <summary>Conversão de valores entre índices econômicos — RN-05-008.</summary>
public interface IConversorIndiceService
{
    /// <summary>IndiceConversor.ConverterIndice (VAL-E3-20 + CALC-E3-05..07). <paramref name="decimais"/> = 2 por padrão.</summary>
    Task<Result<ConversaoIndice>> ConverterAsync(int? idIndiceOrigem, int? idIndiceDestino, decimal valor, DateTime data,
        int decimais = 2, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conversão do índice do item para o índice padrão do sistema — o que o gancho
    /// <c>CalculadoraItemFinanceiroBase.ConverterIndice</c> (E1-T03) deve delegar
    /// (ItemFinanceiroBase.cs:175 no legado).
    /// </summary>
    Task<Result<ConversaoIndice>> ConverterParaPadraoAsync(int idIndiceEconomico, decimal valor, DateTime data,
        CancellationToken cancellationToken = default);
}

// Origem: gestao.financeira/IndiceConversor.cs (ConverterIndice:195-218, CalcularConversao:41-73,
// RetornarIndice:78-97, RetornarDataIndiceValida:102-116) + acesso.global/Feriado.cs:426-432 (DiaUtil) +
// AmbienteServidorGlobal.IndiceEconomicoDefault (parâmetro INDICEECONOMICOPADRAO) — legado.
// Sem tabela própria (FININDICECONVERSOR não existe). Fórmulas transcritas sem refatorar (Regra 5);
// paridade: E3/ConversorIndiceParityTests (golden origem=legado).
public class ConversorIndiceService(
    IIndiceEconomicoConsulta indices,
    ICalendarioConsulta calendario,
    IParametroRepository parametros) : IConversorIndiceService
{
    public const string ParametroIndicePadrao = "INDICEECONOMICOPADRAO";

    // ControleErro.cs:283 (ValorNulo) e :433 (DataSemIndiceEconomico) — mensagens legadas.
    public const string MsgValorNulo = "{0} não pode conter um valor nulo ou nenhum item.";
    public const string MsgDataSemIndice = "Não há valor para o índice '{0}' definido para o dia ({1}).";
    public const string MsgParametroPadrao = "Parâmetro 'IndiceEconomicoPadrao' inválido.";
    public const string MsgIndiceInexistente = "Índice econômico {0} não encontrado.";

    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public async Task<Result<ConversaoIndice>> ConverterAsync(int? idIndiceOrigem, int? idIndiceDestino, decimal valor, DateTime data,
        int decimais = 2, CancellationToken cancellationToken = default)
    {
        // VAL-E3-20 — IndiceConversor.cs:197-201.
        if (idIndiceOrigem is null)
            return Falha("IndiceOrigem", string.Format(MsgValorNulo, "IndiceOrigem"));
        if (idIndiceDestino is null)
            return Falha("IndiceDestino", string.Format(MsgValorNulo, "IndiceDestino"));

        // CalcularConversao:43-49 — mesmo índice: valor inalterado (sem arredondar), índices = 1.
        if (idIndiceOrigem == idIndiceDestino)
            return Result<ConversaoIndice>.Ok(new ConversaoIndice(valor, valor, 1m, 1m));

        var idPadrao = await ObterIdIndicePadraoAsync(cancellationToken);
        if (idPadrao is null)
            return Falha("IndiceEconomicoPadrao", MsgParametroPadrao);

        var origem = await indices.ObterAsync(idIndiceOrigem.Value, cancellationToken);
        if (origem is null)
            return Falha("IndiceOrigem", string.Format(MsgIndiceInexistente, idIndiceOrigem));
        var destino = await indices.ObterAsync(idIndiceDestino.Value, cancellationToken);
        if (destino is null)
            return Falha("IndiceDestino", string.Format(MsgIndiceInexistente, idIndiceDestino));

        // CalcularConversao:56-61 — data válida e valor de cada índice.
        var dataOrigem = await RetornarDataIndiceValidaAsync(data, origem.IdModoCorrecao, cancellationToken);
        var dataDestino = await RetornarDataIndiceValidaAsync(data, destino.IdModoCorrecao, cancellationToken);

        var valorOrigem = await RetornarIndiceAsync(origem, idPadrao.Value, dataOrigem, cancellationToken);
        if (valorOrigem is null)
            return Falha("IndiceOrigem", MensagemDataSemIndice(origem, data));
        var valorDestino = await RetornarIndiceAsync(destino, idPadrao.Value, dataDestino, cancellationToken);
        if (valorDestino is null)
            return Falha("IndiceDestino", MensagemDataSemIndice(destino, data));

        var convertido = CalcularConversao(origem.IdIndiceEconomico, destino.IdIndiceEconomico, idPadrao.Value,
            valor, valorOrigem.Value, valorDestino.Value, decimais);

        return Result<ConversaoIndice>.Ok(new ConversaoIndice(valor, convertido, valorOrigem.Value, valorDestino.Value));
    }

    public async Task<Result<ConversaoIndice>> ConverterParaPadraoAsync(int idIndiceEconomico, decimal valor, DateTime data,
        CancellationToken cancellationToken = default)
    {
        var idPadrao = await ObterIdIndicePadraoAsync(cancellationToken);
        if (idPadrao is null)
            return Falha("IndiceEconomicoPadrao", MsgParametroPadrao);

        return await ConverterAsync(idIndiceEconomico, idPadrao, valor, data, 2, cancellationToken);
    }

    // ---- Fórmulas puras (paridade) ------------------------------------------------------------

    /// <summary>
    /// CALC-E3-05 — IndiceConversor.CalcularConversao:43-72. <paramref name="idPadrao"/> = índice padrão
    /// do sistema (valor 1). Mesmo índice: valor sem arredondar.
    /// </summary>
    public static decimal CalcularConversao(int idOrigem, int idDestino, int idPadrao, decimal valor,
        decimal valorIndiceOrigem, decimal valorIndiceDestino, int decimais)
    {
        if (idOrigem == idDestino)
            return valor;

        if (idOrigem != idPadrao && idDestino != idPadrao)
        {
            var convertido = ArredondamentoFinanceiro.Arredondar(valor / valorIndiceOrigem, decimais);
            return ArredondamentoFinanceiro.Arredondar(convertido * valorIndiceDestino, decimais);
        }

        if (idDestino != idPadrao)
            return ArredondamentoFinanceiro.Arredondar(valor / valorIndiceDestino, decimais);

        return ArredondamentoFinanceiro.Arredondar(valor * valorIndiceOrigem, decimais);
    }

    /// <summary>Feriado.DiaUtil:426-432 — sábado/domingo ou feriado não são dia útil.</summary>
    public static bool DiaUtil(DateTime data, bool ehFeriado)
        => data.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday) && !ehFeriado;

    /// <summary>
    /// CALC-E3-06 — IndiceConversor.RetornarDataIndiceValida:102-116: a própria data se for dia útil;
    /// senão avança (+1) ou, em <see cref="IndiceModoCorrecao.UsarDataAnterior"/>, retrocede (−1) até achar.
    /// </summary>
    public static async Task<DateTime> RetornarDataIndiceValidaAsync(DateTime data, int? idModoCorrecao,
        Func<DateTime, Task<bool>> ehDiaUtil)
    {
        if (await ehDiaUtil(data))
            return data;

        var passo = idModoCorrecao == (int)IndiceModoCorrecao.UsarDataAnterior ? -1 : 1;

        data = data.AddDays(passo);
        while (!await ehDiaUtil(data))
            data = data.AddDays(passo);

        return data;
    }

    /// <summary>
    /// CALC-E3-07 — IndiceConversor.RetornarIndice:83-96: posição = mês − 1 (Mensal) ou dia − 1 (Diário)
    /// na lista de valores; <c>null</c> = DataSemIndiceEconomico (lista vazia ou valor 0).
    /// Posição além do fim da lista (lista incompleta) também devolve <c>null</c> — no legado era erro
    /// de índice fora da faixa (DÚVIDA-E3-2).
    /// </summary>
    public static decimal? SelecionarValorIndice(IReadOnlyList<decimal> valores, int? idTipoCorrecao, DateTime data)
    {
        var i = idTipoCorrecao == (int)IndiceTipoCorrecao.Diario ? data.Day - 1 : data.Month - 1;

        if (valores.Count == 0 || i >= valores.Count || valores[i] == 0m)
            return null;

        return valores[i];
    }

    // ---- Acesso a dados --------------------------------------------------------------------------

    private Task<DateTime> RetornarDataIndiceValidaAsync(DateTime data, int? idModoCorrecao, CancellationToken cancellationToken)
        => RetornarDataIndiceValidaAsync(data, idModoCorrecao,
            async d => DiaUtil(d, await calendario.EhFeriadoAsync(d, cancellationToken)));

    // RetornarIndice:80-81 — índice padrão vale 1; senão carrega a lista do ano (Mensal: 01/01..01/12)
    // ou do mês (Diário: 1..último dia) — IndiceEconomico.RetornarListaIndice.
    private async Task<decimal?> RetornarIndiceAsync(IndiceEconomicoInfo indice, int idPadrao, DateTime data,
        CancellationToken cancellationToken)
    {
        if (indice.IdIndiceEconomico == idPadrao)
            return 1.00m;

        var (inicio, fim) = indice.IdTipoCorrecao == (int)IndiceTipoCorrecao.Diario
            ? (new DateTime(data.Year, data.Month, 1), new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month)))
            : (new DateTime(data.Year, 1, 1), new DateTime(data.Year, 12, 1));

        var valores = await indices.ListarValoresAsync(indice.IdIndiceEconomico, inicio, fim, cancellationToken);
        return SelecionarValorIndice(valores, indice.IdTipoCorrecao, data);
    }

    private async Task<int?> ObterIdIndicePadraoAsync(CancellationToken cancellationToken)
    {
        var valor = await parametros.GetParametroValorAsync(ParametroIndicePadrao, cancellationToken);
        return int.TryParse(valor, out var id) && id != 0 ? id : null;
    }

    // A mensagem usa a data ORIGINAL da conversão, não a data útil ajustada (IndiceConversor.cs:94).
    private static string MensagemDataSemIndice(IndiceEconomicoInfo indice, DateTime data)
        => string.Format(MsgDataSemIndice, indice.Sigla, data.ToString("d", PtBr));

    private static Result<ConversaoIndice> Falha(string campo, string mensagem) => Result<ConversaoIndice>.Fail(new ValidationError(campo, mensagem));
}

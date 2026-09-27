using FluentAssertions;
using Moq;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T09 — ConversorIndiceService: VAL-E3-20 (guardas do IndiceConversor) e o fluxo completo
/// (data útil → valor do índice → conversão), com as portas do MOD-02 mockadas.
/// </summary>
public class ConversorIndiceServiceTests
{
    private const int Padrao = 1;
    private const int Igpm = 2;
    private const int Dolar = 3;

    private readonly Mock<IIndiceEconomicoConsulta> _indices = new();
    private readonly Mock<ICalendarioConsulta> _calendario = new();
    private readonly Mock<IParametroRepository> _parametros = new();

    public ConversorIndiceServiceTests()
    {
        _parametros.Setup(x => x.GetParametroValorAsync(ConversorIndiceService.ParametroIndicePadrao, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Padrao.ToString());
        _indices.Setup(x => x.ObterAsync(Padrao, It.IsAny<CancellationToken>())).ReturnsAsync(new IndiceEconomicoInfo(Padrao, "R$", 0, 0));
        _indices.Setup(x => x.ObterAsync(Igpm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IndiceEconomicoInfo(Igpm, "IGPM", (int)IndiceTipoCorrecao.Mensal, (int)IndiceModoCorrecao.UsaValorDia));
        _indices.Setup(x => x.ObterAsync(Dolar, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IndiceEconomicoInfo(Dolar, "US$", (int)IndiceTipoCorrecao.Diario, (int)IndiceModoCorrecao.UsarDataAnterior));
    }

    private ConversorIndiceService Servico() => new(_indices.Object, _calendario.Object, _parametros.Object);

    private void ValoresMensais(int idIndice, int ano, params decimal[] valores)
        => _indices.Setup(x => x.ListarValoresAsync(idIndice, new DateTime(ano, 1, 1), new DateTime(ano, 12, 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valores);

    // ---- VAL-E3-20 --------------------------------------------------------------------------------

    [Fact]
    public async Task VAL_E3_20_IndiceOrigemNulo_Falha()
    {
        var r = await Servico().ConverterAsync(null, Padrao, 100m, new DateTime(2026, 9, 23));

        r.Errors.Single().Mensagem.Should().Be("IndiceOrigem não pode conter um valor nulo ou nenhum item.");
    }

    [Fact]
    public async Task VAL_E3_20_IndiceDestinoNulo_Falha()
    {
        var r = await Servico().ConverterAsync(Igpm, null, 100m, new DateTime(2026, 9, 23));

        r.Errors.Single().Mensagem.Should().Be("IndiceDestino não pode conter um valor nulo ou nenhum item.");
    }

    [Fact]
    public async Task VAL_E3_20_DataSemIndiceEconomico_UsaADataOriginalNaMensagem()
    {
        // 2026-09-26 é sábado: a data útil vira segunda (28/09), mas a mensagem usa a data original.
        ValoresMensais(Igpm, 2026);

        var r = await Servico().ConverterAsync(Igpm, Padrao, 100m, new DateTime(2026, 9, 26));

        r.Errors.Single().Mensagem.Should().Be("Não há valor para o índice 'IGPM' definido para o dia (26/09/2026).");
    }

    [Fact]
    public async Task VAL_E3_20_ValorDoIndiceZero_Falha()
    {
        ValoresMensais(Igpm, 2026, 1.01m, 1.02m, 1.03m, 1.04m, 1.05m, 1.06m, 1.07m, 1.08m, 0m, 1.10m, 1.11m, 1.12m);

        var r = await Servico().ConverterAsync(Igpm, Padrao, 100m, new DateTime(2026, 9, 23));

        r.IsSuccess.Should().BeFalse();
    }

    // ---- Fluxo --------------------------------------------------------------------------------------

    [Fact]
    public async Task MesmoIndice_DevolveValorSemArredondarEIndicesUm()
    {
        var r = await Servico().ConverterAsync(Igpm, Igpm, 123.456m, new DateTime(2026, 9, 23));

        r.Value.Should().Be(new ConversaoIndice(123.456m, 123.456m, 1m, 1m));
    }

    [Fact]
    public async Task IndiceMensalParaPadrao_MultiplicaPeloValorDoMes()
    {
        ValoresMensais(Igpm, 2026, 1.01m, 1.02m, 1.03m, 1.04m, 1.05m, 1.06m, 1.07m, 1.08m, 1.23456m, 1.10m, 1.11m, 1.12m);

        var r = await Servico().ConverterAsync(Igpm, Padrao, 100m, new DateTime(2026, 9, 23));

        r.Value!.ValorConvertido.Should().Be(123.46m);
        r.Value.ValorIndiceOrigem.Should().Be(1.23456m);
        r.Value.ValorIndiceDestino.Should().Be(1m);
    }

    [Fact]
    public async Task IndiceDiarioComDataAnterior_UsaODiaUtilAnteriorNaLista()
    {
        // Sábado 26/09 com UsarDataAnterior → sexta 25/09 → posição 24 da lista diária de setembro.
        var diario = Enumerable.Range(1, 30).Select(d => 5m + d / 100m).ToArray();
        _indices.Setup(x => x.ListarValoresAsync(Dolar, new DateTime(2026, 9, 1), new DateTime(2026, 9, 30), It.IsAny<CancellationToken>()))
            .ReturnsAsync(diario);

        var r = await Servico().ConverterAsync(Padrao, Dolar, 1000m, new DateTime(2026, 9, 26));

        r.Value!.ValorIndiceDestino.Should().Be(5.25m);
        r.Value.ValorConvertido.Should().Be(ConversorIndiceService.CalcularConversao(Padrao, Dolar, Padrao, 1000m, 1m, 5.25m, 2));
    }

    [Fact]
    public async Task FeriadoConsultaOCalendario()
    {
        // Quarta 07/10/2026 marcada como feriado → IGPM de outubro continua (mensal), mas o calendário é consultado.
        _calendario.Setup(x => x.EhFeriadoAsync(new DateTime(2026, 10, 7), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        ValoresMensais(Igpm, 2026, 1.01m, 1.02m, 1.03m, 1.04m, 1.05m, 1.06m, 1.07m, 1.08m, 1.09m, 1.10m, 1.11m, 1.12m);

        var r = await Servico().ConverterAsync(Igpm, Padrao, 100m, new DateTime(2026, 10, 7));

        r.Value!.ValorIndiceOrigem.Should().Be(1.10m);
        _calendario.Verify(x => x.EhFeriadoAsync(new DateTime(2026, 10, 8), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task SemParametroDeIndicePadrao_Falha()
    {
        _parametros.Setup(x => x.GetParametroValorAsync(ConversorIndiceService.ParametroIndicePadrao, It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        var r = await Servico().ConverterAsync(Igpm, Dolar, 100m, new DateTime(2026, 9, 23));

        r.Errors.Single().Mensagem.Should().Be(ConversorIndiceService.MsgParametroPadrao);
    }

    [Fact]
    public async Task ConverterParaPadrao_ConverteDoIndiceDoItemParaOPadrao()
    {
        ValoresMensais(Igpm, 2026, 1.01m, 1.02m, 1.03m, 1.04m, 1.05m, 1.06m, 1.07m, 1.08m, 1.5m, 1.10m, 1.11m, 1.12m);

        var r = await Servico().ConverterParaPadraoAsync(Igpm, 10m, new DateTime(2026, 9, 23));

        r.Value!.ValorConvertido.Should().Be(15m);
    }
}

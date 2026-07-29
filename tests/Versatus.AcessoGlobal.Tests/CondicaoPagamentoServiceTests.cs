using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Versatus.AcessoGlobal.Domain.DTOs;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Xunit;

namespace Versatus.AcessoGlobal.Tests;

public class CondicaoPagamentoServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AcessoGlobalDbContext _context;
    private readonly Mock<IGeradorSequencial> _geradorSequencialMock;
    private readonly Mock<IContextoExecucao> _contextoMock;
    private readonly CondicaoPagamentoService _service;

    public CondicaoPagamentoServiceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AcessoGlobalDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AcessoGlobalDbContext(options);
        _context.Database.EnsureCreated();

        _geradorSequencialMock = new Mock<IGeradorSequencial>();
        _contextoMock = new Mock<IContextoExecucao>();

        _contextoMock.Setup(c => c.IdUsuario).Returns(1);

        _service = new CondicaoPagamentoService(
            _context,
            _geradorSequencialMock.Object,
            _contextoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoAcrescimoEDescontoMarcadosSimultaneamente()
    {
        var dto = ObterDtoBaseComValoresValidos();
        var dtoInvalido = dto with { RecebeAcrescimo = true, Acrescimo = 5, RecebeDesconto = true, Desconto = 5 };

        var result = await _service.CriarAsync(dtoInvalido);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("acréscimo e desconto ao mesmo tempo"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoSemanalESemDiaSemana()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Semanal Sem Dia",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Semanal,
            IdDiaSemana = 0
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("dia da semana é obrigatório"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoParceladaEQuantidadeParcelaZero()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Parcelada Zero",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = false,
            QuantidadeParcela = 0,
            Regras = new List<CondicaoPagtoRegraDto>()
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("quantidade de parcelas"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoParceladaEGradeSemParcelas()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Sem Grade",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = false,
            QuantidadeParcela = 2,
            Regras = new List<CondicaoPagtoRegraDto>()
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("ao menos uma parcela na grade"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoParceladaEQuantidadeParcelaDiferenteDaGrade()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Divergente",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = false,
            QuantidadeParcela = 3,
            Regras = new List<CondicaoPagtoRegraDto>
            {
                new() { NumeroParcela = 1, NumeroDias = 30 },
                new() { NumeroParcela = 2, NumeroDias = 60 }
            }
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("exatamente 3 parcelas"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoParceladaEDivisaoPercentualDiferenteDeCem()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Soma 90%",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = false,
            QuantidadeParcela = 2,
            TipoDivisaoParcelamento = DivisaoParcelamentoTipo.Percentual,
            Regras = new List<CondicaoPagtoRegraDto>
            {
                new() { NumeroParcela = 1, NumeroDias = 30, PercentualDivisao = 45 },
                new() { NumeroParcela = 2, NumeroDias = 60, PercentualDivisao = 45 }
            }
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("exatamente 100%"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoCondicaoLivreEArredondamentoNaoForUltima()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Livre Erro Arredondamento",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = true,
            IdParcelaArredondamento = ParcelamentoArredondamento.Primeira,
            TipoDivisaoParcelamento = DivisaoParcelamentoTipo.Quantidade,
            IdParcelamentoTipo = ParcelamentoTipo.DiasEntreParcela
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("Última parcela"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoCondicaoLivreEDivisaoForPercentual()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Livre Erro Percentual",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = true,
            IdParcelaArredondamento = ParcelamentoArredondamento.Ultima,
            TipoDivisaoParcelamento = DivisaoParcelamentoTipo.Percentual,
            IdParcelamentoTipo = ParcelamentoTipo.DiasEntreParcela
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("Quantidade"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoCondicaoLivreETipoParcelamentoForDiasUteis()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Livre Erro Dias Uteis",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            AlteraParcelas = true,
            IdParcelaArredondamento = ParcelamentoArredondamento.Ultima,
            TipoDivisaoParcelamento = DivisaoParcelamentoTipo.Quantidade,
            IdParcelamentoTipo = ParcelamentoTipo.DiasUteis
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("Dias úteis"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoMesComercialETipoNaoForDiasEntreParcela()
    {
        var dto = ObterDtoBaseComValoresValidos() with
        {
            UsarMesComercial = true,
            IdParcelamentoTipo = ParcelamentoTipo.DiaFixo,
            DiasParcelamento = 30
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("Dias entre parcelas"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoMesComercialEDiasEntreParcelaNaoForTrinta()
    {
        var dto = ObterDtoBaseComValoresValidos() with
        {
            UsarMesComercial = true,
            IdParcelamentoTipo = ParcelamentoTipo.DiasEntreParcela,
            DiasParcelamento = 15
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("igual a 30"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoPrimeiraParcelaAVistaETipoNaoForDiasEntreParcela()
    {
        var dto = ObterDtoBaseComValoresValidos() with
        {
            PrimeiraParcelaAVista = true,
            IdParcelamentoTipo = ParcelamentoTipo.DiaFixo
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("Dias entre parcelas"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoFaixaDiasSemRegras()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Faixa Sem Regras",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.FaixaDias,
            Regras = new List<CondicaoPagtoRegraDto>()
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("faixas para o cálculo"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoFaixaDiasComDiaFinalMenorQueInicial()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Faixa Invalida Final Menor",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.FaixaDias,
            Regras = new List<CondicaoPagtoRegraDto>
            {
                new() { DiaInicial = 10, DiaFinal = 5, NumeroDias = 15 }
            }
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("inferior ao dia inicial"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarFalha_QuandoFaixaDiasComVencimentoDentroDoIntervalo()
    {
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Faixa Invalida Vencimento Dentro",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.FaixaDias,
            Regras = new List<CondicaoPagtoRegraDto>
            {
                new() { DiaInicial = 1, DiaFinal = 10, NumeroDias = 5 }
            }
        };

        var result = await _service.CriarAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("dentro do intervalo"));
    }

    [Fact]
    public async Task CriarAsync_DeveRetornarSucesso_QuandoDadosValidos()
    {
        var dto = ObterDtoBaseComValoresValidos();
        _geradorSequencialMock.Setup(g => g.ProximoAsync(It.IsAny<string>(), It.IsAny<SequencialTipo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CriarAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Condicao Valida", result.Value.Descricao);
    }

    private static CriarCondicaoPagamentoDto ObterDtoBaseComValoresValidos()
    {
        return new CriarCondicaoPagamentoDto
        {
            Descricao = "Condicao Valida",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            IdDisponibilidade = Disponibilidade.Ambas,
            IdTipoVencimento = VencimentoTipo.Normal,
            RecebeAcrescimo = false,
            RecebeDesconto = false,
            AlteraParcelas = false,
            QuantidadeParcela = 2,
            IdParcelamentoTipo = ParcelamentoTipo.DiasEntreParcela,
            TipoDivisaoParcelamento = DivisaoParcelamentoTipo.Percentual,
            Regras = new List<CondicaoPagtoRegraDto>
            {
                new() { NumeroParcela = 1, NumeroDias = 30, PercentualDivisao = 50 },
                new() { NumeroParcela = 2, NumeroDias = 60, PercentualDivisao = 50 }
            }
        };
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}

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
        // Arrange (Matriz RTV: Regra de exclusividade de Acréscimo e Desconto)
        var dto = new CriarCondicaoPagamentoDto
        {
            Descricao = "Condicao Teste Exclusividade",
            IdTipoCondicaoPagto = CondicaoPagtoTipo.Parcelada,
            IdDisponibilidade = Disponibilidade.Ambas,
            IdTipoVencimento = VencimentoTipo.Normal,
            RecebeAcrescimo = true,
            Acrescimo = 5.0m,
            RecebeDesconto = true,
            Desconto = 10.0m,
            QuantidadeParcela = 1,
            IdParcelamentoTipo = ParcelamentoTipo.DiasEntreParcela,
            TipoDivisaoParcelamento = DivisaoParcelamentoTipo.Quantidade,
            Regras = new List<CondicaoPagtoRegraDto>
            {
                new() { NumeroParcela = 1, NumeroDias = 30, PercentualDivisao = 100 }
            }
        };

        // Act
        var result = await _service.CriarAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Mensagem.Contains("acréscimo e desconto ao mesmo tempo"));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}

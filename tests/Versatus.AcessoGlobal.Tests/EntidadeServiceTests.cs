using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.Services;
using Xunit;

namespace Versatus.AcessoGlobal.Tests;

public class EntidadeServiceTests
{
    private readonly Mock<IEntidadeRepository> _repositoryMock;
    private readonly Mock<IGeradorSequencial> _geradorMock;
    private readonly Mock<IContextoExecucao> _contextoMock;
    private readonly Mock<ILogger<EntidadeService>> _loggerMock;
    private readonly EntidadeService _service;

    public EntidadeServiceTests()
    {
        _repositoryMock = new Mock<IEntidadeRepository>();
        _geradorMock = new Mock<IGeradorSequencial>();
        _contextoMock = new Mock<IContextoExecucao>();
        _loggerMock = new Mock<ILogger<EntidadeService>>();
        
        _service = new EntidadeService(
            _repositoryMock.Object,
            _geradorMock.Object,
            _contextoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarIdEntidade_QuandoSucesso()
    {
        // Arrange
        var entidade = new Entidade { Nome = "Teste" };
        _geradorMock.Setup(g => g.ProximoAsync("Entidade", SequencialTipo.Geral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);

        // Act
        var resultado = await _service.CriarAsync(entidade);

        // Assert
        resultado.IdEntidade.Should().Be(123);
        resultado.IdUsuarioInclusao.Should().Be(1);
        resultado.DataInclusao.Should().Be(DateTime.Today);
        _repositoryMock.Verify(r => r.AddAsync(entidade, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoCpfJaExiste()
    {
        // Arrange
        var cpf = "12345678901";
        var entidade = new Entidade 
        { 
            Nome = "Teste", 
            PessoaFisica = new DadosPessoaFisica { Cpf = cpf } 
        };
        
        _repositoryMock.Setup(r => r.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Entidade { IdEntidade = 999 });

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Já existe uma entidade cadastrada com o CPF {cpf}.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoCnpjJaExiste()
    {
        // Arrange
        var cnpj = "12345678000199";
        var entidade = new Entidade 
        { 
            Nome = "Teste", 
            PessoaJuridica = new DadosPessoaJuridica { Cnpj = cnpj } 
        };
        
        _repositoryMock.Setup(r => r.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Entidade { IdEntidade = 999 });

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Já existe uma entidade cadastrada com o CNPJ {cnpj}.");
    }
}

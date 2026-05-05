using FluentAssertions;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Services;
using Xunit;

namespace Versatus.AcessoGlobal.Tests;

public class EnderecoServiceTests
{
    private readonly EnderecoService _service;

    public EnderecoServiceTests()
    {
        _service = new EnderecoService();
    }

    [Fact]
    public async Task ValidarEnderecoAsync_DevePassar_QuandoEnderecoValido()
    {
        // Arrange
        var endereco = new EntidadeEndereco
        {
            Logradouro = "Rua Teste",
            Numero = 100,
            Cep = "12345678",
            IdCidade = 1,
            IdTipoLogradouro = 1
        };

        // Act
        var act = () => _service.ValidarEnderecoAsync(endereco);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidarEnderecoAsync_DeveLancarExcecao_QuandoLogradouroVazio()
    {
        // Arrange
        var endereco = new EntidadeEndereco { Logradouro = "", Numero = 100 };

        // Act
        var act = () => _service.ValidarEnderecoAsync(endereco);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O logradouro é obrigatório.");
    }

    [Fact]
    public async Task ValidarEnderecoAsync_DeveLancarExcecao_QuandoNumeroInvalido()
    {
        // Arrange
        var endereco = new EntidadeEndereco { Logradouro = "Rua", Numero = 0 };

        // Act
        var act = () => _service.ValidarEnderecoAsync(endereco);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O número deve ser maior que zero.");
    }

    [Fact]
    public async Task ValidarEnderecoAsync_DeveLancarExcecao_QuandoCepInvalido()
    {
        // Arrange
        var endereco = new EntidadeEndereco { Logradouro = "Rua", Numero = 10, Cep = "123" };

        // Act
        var act = () => _service.ValidarEnderecoAsync(endereco);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("O CEP deve conter exatamente 8 dígitos.");
    }
}

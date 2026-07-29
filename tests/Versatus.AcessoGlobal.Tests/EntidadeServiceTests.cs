using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Location;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.AcessoGlobal.Domain.Services;
using Xunit;

namespace Versatus.AcessoGlobal.Tests;

public class EntidadeServiceTests
{
    private readonly Mock<IEntidadeRepository> _repositoryMock;
    private readonly Mock<IParametroRepository> _parametroRepositoryMock;
    private readonly Mock<IGeradorSequencial> _geradorMock;
    private readonly Mock<IContextoExecucao> _contextoMock;
    private readonly Mock<ILogger<EntidadeService>> _loggerMock;
    private readonly EntidadeService _service;

    public EntidadeServiceTests()
    {
        _repositoryMock = new Mock<IEntidadeRepository>();
        _parametroRepositoryMock = new Mock<IParametroRepository>();
        _geradorMock = new Mock<IGeradorSequencial>();
        _contextoMock = new Mock<IContextoExecucao>();
        _loggerMock = new Mock<ILogger<EntidadeService>>();
        
        // Permitir CPFs/CNPJs matematicamente inválidos por padrão nos testes legados
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("ACEITACNPJCPFINVALIDO", It.IsAny<CancellationToken>()))
            .ReturnsAsync("true");

        _service = new EntidadeService(
            _repositoryMock.Object,
            _parametroRepositoryMock.Object,
            _geradorMock.Object,
            _contextoMock.Object,
            _loggerMock.Object,
            null!,
            new Mock<IServiceProvider>().Object);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarIdEntidade_QuandoSucesso()
    {
        // Arrange
        var entidade = new Entidade { Nome = "Teste", IsCliente = true };
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
            IsCliente = true,
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
            IsCliente = true,
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

    [Fact]
    public async Task CriarAsync_NaoDeveLancarExcecao_QuandoCpfJaExiste_E_TipoValidacaoForNaoValidar()
    {
        // Arrange
        var cpf = "12345678901";
        var entidade = new Entidade 
        { 
            Nome = "Teste", 
            IsCliente = true,
            PessoaFisica = new DadosPessoaFisica { Cpf = cpf } 
        };
        
        _repositoryMock.Setup(r => r.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Entidade { IdEntidade = 999 });
        
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("TipoBloqueioCpfCnpjDuplicado", It.IsAny<CancellationToken>()))
            .ReturnsAsync("NaoValidar");

        // Act
        var resultado = await _service.CriarAsync(entidade);

        // Assert
        resultado.Should().NotBeNull();
        _repositoryMock.Verify(r => r.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_NaoDeveLancarExcecao_QuandoCpfJaExiste_E_TipoValidacaoForAvisar()
    {
        // Arrange
        var cpf = "12345678901";
        var entidade = new Entidade 
        { 
            Nome = "Teste", 
            IsCliente = true,
            PessoaFisica = new DadosPessoaFisica { Cpf = cpf } 
        };
        
        _repositoryMock.Setup(r => r.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Entidade { IdEntidade = 999 });
        
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("TipoBloqueioCpfCnpjDuplicado", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Avisar");

        // Act
        var resultado = await _service.CriarAsync(entidade);

        // Assert
        resultado.Should().NotBeNull();
        _repositoryMock.Verify(r => r.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoCpfJaExiste_E_TipoValidacaoForBloquearSalvar()
    {
        // Arrange
        var cpf = "12345678901";
        var entidade = new Entidade 
        { 
            Nome = "Teste", 
            IsCliente = true,
            PessoaFisica = new DadosPessoaFisica { Cpf = cpf } 
        };
        
        _repositoryMock.Setup(r => r.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Entidade { IdEntidade = 999 });
        
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("TipoBloqueioCpfCnpjDuplicado", It.IsAny<CancellationToken>()))
            .ReturnsAsync("BloquearSalvar");

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Já existe uma entidade cadastrada com o CPF {cpf}.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoRazaoSocialForCurta()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Teste Juridico", 
            IsCliente = true,
            TipoPessoa = EntidadeTipoPessoa.Juridica,
            PessoaJuridica = new DadosPessoaJuridica { RazaoSocial = "AB" } // Curta (< 3)
        };
        _geradorMock.Setup(g => g.ProximoAsync("Entidade", SequencialTipo.Geral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A razão social deve ter no mínimo 3 caracteres válidos.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoCpfForInvalido_E_ParametroAceitaInvalidoForFalse()
    {
        // Arrange
        var cpfInvalido = "11111111111"; // Matemáticos inválidos (dígitos iguais)
        var entidade = new Entidade 
        { 
            Nome = "Teste Fisica", 
            IsCliente = true,
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica { Cpf = cpfInvalido } 
        };
        _geradorMock.Setup(g => g.ProximoAsync("Entidade", SequencialTipo.Geral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("ACEITACNPJCPFINVALIDO", It.IsAny<CancellationToken>()))
            .ReturnsAsync("false");

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("CPF inválido.");
    }

    [Fact]
    public async Task CriarAsync_NaoDeveLancarExcecao_QuandoCpfForInvalido_E_ParametroAceitaInvalidoForTrue()
    {
        // Arrange
        var cpfInvalido = "11111111111"; 
        var entidade = new Entidade 
        { 
            Nome = "Teste Fisica", 
            IsCliente = true,
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica { Cpf = cpfInvalido } 
        };
        _geradorMock.Setup(g => g.ProximoAsync("Entidade", SequencialTipo.Geral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("ACEITACNPJCPFINVALIDO", It.IsAny<CancellationToken>()))
            .ReturnsAsync("true");

        // Act
        var resultado = await _service.CriarAsync(entidade);

        // Assert
        resultado.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(entidade, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoCpfNaoForInformado_E_ParametroObrigatorioForBloquearSalvar()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Teste Fisica", 
            IsCliente = true,
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica { Cpf = "" } 
        };
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("CPFCNPJOBRIGATORIO", It.IsAny<CancellationToken>()))
            .ReturnsAsync("BloquearSalvar");

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Deve ser informado o CPF.");
    }

    [Fact]
    public async Task CriarAsync_NaoDeveLancarExcecao_QuandoCpfNaoForInformado_E_ParametroObrigatorioForNaoValidar()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Teste Fisica", 
            IsCliente = true,
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica { Cpf = "" } 
        };
        _geradorMock.Setup(g => g.ProximoAsync("Entidade", SequencialTipo.Geral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);
        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("CPFCNPJOBRIGATORIO", It.IsAny<CancellationToken>()))
            .ReturnsAsync("NaoValidar");

        // Act
        var resultado = await _service.CriarAsync(entidade);

        // Assert
        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task CriarAsync_NaoDeveValidarCpfCnpj_QuandoEntidadeForEstrangeira()
    {
        // Arrange
        var cpfInvalido = "11111111111"; // Matemáticos inválidos
        var entidade = new Entidade 
        { 
            Nome = "Teste Estrangeiro", 
            IsCliente = true,
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica { Cpf = cpfInvalido } 
        };
        // Adiciona um endereço de outro país (ex: Cidade do país ID 999)
        var cidadeEstrangeira = new Cidade { IdCidade = 50, IdPais = 999 };
        var endereco = new EntidadeEndereco 
        { 
            TipoEndereco = EnderecoTipo.ComercialResidencial,
            IdCidade = 50,
            Cidade = cidadeEstrangeira
        };
        entidade.AdicionarEndereco(endereco);

        _geradorMock.Setup(g => g.ProximoAsync("Entidade", SequencialTipo.Geral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(123);
        _contextoMock.Setup(c => c.IdUsuario).Returns(1);
        _contextoMock.Setup(c => c.IdFilial).Returns(1);
        
        // Filial é do Brasil (país 1058), Entidade é de outro país (999)
        _repositoryMock.Setup(r => r.GetPaisIdPorFilialAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1058);

        _parametroRepositoryMock.Setup(p => p.GetParametroValorAsync("ACEITACNPJCPFINVALIDO", It.IsAny<CancellationToken>()))
            .ReturnsAsync("false"); // Se fosse nacional, deveria lançar exceção de CPF inválido

        // Act
        var resultado = await _service.CriarAsync(entidade);

        // Assert
        resultado.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(entidade, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoNenhumPapelForSelecionado()
    {
        // Arrange
        var entidade = new Entidade { Nome = "Sem Papel" }; // Nenhum IsCliente, IsFornecedor etc marcado

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Pelo menos um tipo de entidade (papel) deve ser selecionado.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoFilialPessoaFisicaSemCaracteristicaJuridica()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Filial PF", 
            IsFilial = true, 
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica { FisicaTipoJuridica = false }
        };

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Para entidade do tipo 'Filial' definida como pessoa 'Física', deve estar marcado 'Pessoa física com característica de jurídica'.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoFuncionarioForPessoaJuridica()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Funcionario PJ", 
            IsFuncionario = true, 
            TipoPessoa = EntidadeTipoPessoa.Juridica 
        };

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Para a entidade do tipo 'Funcionário', deve ser pessoa física e não possuir característica de pessoa jurídica.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoIntermediadorForPessoaFisica()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Intermediador PF", 
            IsIntermediadorComercial = true, 
            TipoPessoa = EntidadeTipoPessoa.Fisica 
        };

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Para a entidade do tipo 'Intermediador', deve ser SOMENTE pessoa definida como jurídica.");
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoInscricaoSuframaComecarComZeroZero()
    {
        // Arrange
        var entidade = new Entidade 
        { 
            Nome = "Suframa Invalida", 
            IsCliente = true,
            InscricaoSuframa = "001234567"
        };

        // Act
        var act = () => _service.CriarAsync(entidade);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Os dois primeiros caracteres da inscrição SUFRAMA, NÃO pode ser '00'.");
    }
}

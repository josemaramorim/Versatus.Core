using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Api.Controllers;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T06 — controllers E3 (contracts/caixa-banco.md): mapeamento HTTP (200/201/204/400/404)
/// com o serviço mockado, e fluxo DTO ponta a ponta com o serviço real (InMemory).
/// </summary>
public class ControllersE3Tests
{
    private static readonly ValidationError Erro = new("Campo", "Mensagem de negócio.");

    private static CaixaBancoDto CaixaDto(int id = 1) => new(id, CenarioE3.Filial, "Conta", (int)ContaTipo.Caixa, true, false,
        null, null, null, null, null, [], null, null, null, null, null, null, null);

    private static CriarCaixaBancoDto CriarDto(ContaTipo tipo = ContaTipo.Caixa, AtualizarContaBancariaDto? conta = null,
        IReadOnlyList<int>? usuarios = null)
        => new("Conta nova", (int)tipo, true, true, null, null, null, null, null, usuarios, conta);

    private static AtualizarContaBancariaDto ContaDto(bool contaTerceiro = false) => new(1, null, "12345", "9", null, null, null, null,
        contaTerceiro, true, null, (int)TipoContaBancaria.ContaCorrente, null, false, null);

    // ---- CaixaBancoController (serviço mockado) ------------------------------------------

    [Fact]
    public async Task CaixaBanco_Get_Inexistente_Retorna404()
    {
        var service = new Mock<ICaixaBancoService>();

        var r = await new CaixaBancoController(service.Object, Mock.Of<ISaldoCalculadora>()).ObterPorId(CenarioE3.Filial, 9, default);

        r.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CaixaBanco_Post_Valido_Retorna201ComRotaDoRecurso()
    {
        var service = new Mock<ICaixaBancoService>();
        service.Setup(x => x.CriarAsync(It.IsAny<CriarCaixaBancoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CaixaBancoDto>.Ok(CaixaDto(100)));

        var r = await new CaixaBancoController(service.Object, Mock.Of<ISaldoCalculadora>()).Criar(CriarDto(), default);

        var criado = r.Should().BeOfType<CreatedAtActionResult>().Subject;
        criado.ActionName.Should().Be(nameof(CaixaBancoController.ObterPorId));
        criado.RouteValues.Should().Contain("id", 100).And.Contain("idFilial", CenarioE3.Filial);
    }

    [Fact]
    public async Task CaixaBanco_Post_FalhaDeNegocio_Retorna400()
    {
        var service = new Mock<ICaixaBancoService>();
        service.Setup(x => x.CriarAsync(It.IsAny<CriarCaixaBancoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CaixaBancoDto>.Fail(Erro));

        var r = await new CaixaBancoController(service.Object, Mock.Of<ISaldoCalculadora>()).Criar(CriarDto(), default);

        r.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CaixaBanco_Put_Inexistente_Retorna404SemChamarAtualizacao()
    {
        var service = new Mock<ICaixaBancoService>();

        var r = await new CaixaBancoController(service.Object, Mock.Of<ISaldoCalculadora>()).Atualizar(CenarioE3.Filial, 9,
            new AtualizarCaixaBancoDto("X", (int)ContaTipo.Caixa, true, false, null, null, null, null, null, null, null), default);

        r.Should().BeOfType<NotFoundResult>();
        service.Verify(x => x.AtualizarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<AtualizarCaixaBancoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CaixaBanco_Delete_Existente_Retorna204()
    {
        var service = new Mock<ICaixaBancoService>();
        service.Setup(x => x.ObterPorIdAsync(1, CenarioE3.Filial, It.IsAny<CancellationToken>())).ReturnsAsync(CaixaDto());
        service.Setup(x => x.ExcluirAsync(1, CenarioE3.Filial, It.IsAny<CancellationToken>())).ReturnsAsync(ValidationResult.Ok());

        var r = await new CaixaBancoController(service.Object, Mock.Of<ISaldoCalculadora>()).Excluir(CenarioE3.Filial, 1, default);

        r.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task CaixaBanco_PutUsuarios_FalhaDeNegocio_Retorna400()
    {
        var service = new Mock<ICaixaBancoService>();
        service.Setup(x => x.SalvarUsuariosAsync(1, CenarioE3.Filial, It.IsAny<SalvarCaixaBancoUsuariosDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<CaixaBancoUsuarioDto>>.Fail(Erro));

        var r = await new CaixaBancoController(service.Object, Mock.Of<ISaldoCalculadora>()).SalvarUsuarios(CenarioE3.Filial, 1,
            new SalvarCaixaBancoUsuariosDto([new CaixaBancoUsuarioItemDto(7)]), default);

        r.Should().BeOfType<BadRequestObjectResult>();
    }

    // ---- Fluxo DTO ponta a ponta (serviço real, InMemory) -------------------------------------

    [Fact]
    public async Task CaixaBanco_PostBancoComConta_RetornaDtoComContaEUsuarios()
    {
        using var c = new CenarioE3();
        var controller = new CaixaBancoController(c.CaixaBancoService(), Mock.Of<ISaldoCalculadora>());

        var r = await controller.Criar(CriarDto(ContaTipo.Banco, ContaDto(), [8, 7]), default);

        var dto = r.Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().BeOfType<CaixaBancoDto>().Subject;
        dto.IdCaixaBanco.Should().Be(100);
        dto.IdTipoConta.Should().Be((int)ContaTipo.Banco);
        dto.Usuarios.Select(u => u.IdUsuario).Should().Equal(7, 8);
        dto.ContaBancaria.Should().NotBeNull();
        dto.ContaBancaria!.NumeroConta.Should().Be("12345");
        dto.ContaBancaria.IdTipoContaBancaria.Should().Be((int)TipoContaBancaria.ContaCorrente);
    }

    [Fact]
    public async Task CaixaBanco_PostContaDeTerceiroIncompleta_Retorna400ComMensagemDoLegado()
    {
        using var c = new CenarioE3();
        var controller = new CaixaBancoController(c.CaixaBancoService(), Mock.Of<ISaldoCalculadora>());

        var r = await controller.Criar(CriarDto(ContaTipo.Banco, ContaDto(contaTerceiro: true)), default);

        var erro = r.Should().BeOfType<BadRequestObjectResult>().Subject.Value!;
        erro.GetType().GetProperty("message")!.GetValue(erro).Should().Be(ContaBancariaService.MsgContaTerceiro);
    }

    [Fact]
    public async Task ContaBancaria_PutExistente_Retorna200()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));

        var r = await new ContaBancariaController(c.ContaBancariaService()).Atualizar(CenarioE3.Filial, 1, ContaDto(), default);

        r.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<ContaBancariaDto>()
            .Which.DigitoConta.Should().Be("9");
    }

    [Fact]
    public async Task ContaBancaria_PutInexistente_Retorna404()
    {
        using var c = new CenarioE3();

        var r = await new ContaBancariaController(c.ContaBancariaService()).Atualizar(CenarioE3.Filial, 1, ContaDto(), default);

        r.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Cobrador_CrudPeloController()
    {
        using var c = new CenarioE3();
        var controller = new CobradorController(c.CobradorService());

        var criado = (await controller.Criar(new SalvarCobradorDto(50, "Cobrador", null, null), default))
            .Should().BeOfType<CreatedAtActionResult>().Subject.Value.Should().BeOfType<CobradorDto>().Subject;
        criado.Ativo.Should().BeTrue("Ativo default true (contracts/caixa-banco.md)");

        (await controller.Atualizar(CenarioE3.Filial, criado.IdCobrador, new SalvarCobradorDto(50, "Outro", null, null), default))
            .Should().BeOfType<OkObjectResult>();
        (await controller.Excluir(CenarioE3.Filial, criado.IdCobrador, default)).Should().BeOfType<NoContentResult>();
        (await controller.ObterPorId(CenarioE3.Filial, criado.IdCobrador, default)).Should().BeOfType<NotFoundResult>();
    }
}

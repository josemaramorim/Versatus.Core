using FluentAssertions;
using Moq;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.GestaoFinanceira.Infrastructure.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E2;

/// <summary>
/// E2-T04 — PeriodosAbertosService (OP-E2-13, OP-E2-14, VAL-E2-42) e o DominioFinanceiroConsulta
/// sobre as entidades do E2 (substitui o adapter SQL temporário do E3-T05).
/// </summary>
public class PeriodosAbertosServiceTests
{
    private static async Task<CenarioE2> CenarioAsync(bool trabalhaComDominio = true, params int[] usuariosComPerfil)
    {
        var c = new CenarioE2();
        c.Parametro(PeriodosAbertosService.ParametroTrabalhaComDominio, trabalhaComDominio ? "True" : "False");
        c.Perfis.Setup(x => x.ListarUsuariosDosPerfisComDominioAsync(CenarioE2.Empresa, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([.. usuariosComPerfil.Select(u => new UsuarioPerfilDominioInfo(u, $"Usuário {u}"))]);
        await c.SemearAsync(CenarioE2.Caixa(1));
        return c;
    }

    // ---- OP-E2-13 — PeriodoAberto ------------------------------------------------------------

    [Fact]
    public async Task OP_E2_13_SemTrabalhaComDominio_NaoAplicavel()
    {
        using var c = await CenarioAsync(trabalhaComDominio: false, CenarioE2.Usuario);

        (await c.PeriodosAbertosService().PeriodoAbertoAsync()).Should().Be(PeriodoStatus.NaoAplicavel);
    }

    [Fact]
    public async Task OP_E2_13_NenhumUsuarioComPerfilDeDominio_PerfilSemPermissao()
    {
        using var c = await CenarioAsync(true);

        (await c.PeriodosAbertosService().PeriodoAbertoAsync()).Should().Be(PeriodoStatus.PerfilSemPermissao);
    }

    [Fact]
    public async Task OP_E2_13_UsuarioForaDosPerfis_UsuarioSemPermissao()
    {
        using var c = await CenarioAsync(true, 7);

        (await c.PeriodosAbertosService().PeriodoAbertoAsync()).Should().Be(PeriodoStatus.UsuarioSemPermissao);
    }

    [Fact]
    public async Task OP_E2_13_PeriodoAbertoOuFechado()
    {
        using var c = await CenarioAsync(true, CenarioE2.Usuario, 7, 8);
        await c.SemearAsync(
            CenarioE2.NovoDominio(1, idPeriodo: 10, usuarios: CenarioE2.Usuario), CenarioE2.Periodo(10, 1),
            CenarioE2.NovoDominio(2, idPeriodo: 20, usuarios: 7), CenarioE2.Periodo(20, 2, new DateTime(2026, 10, 2)),
            CenarioE2.NovoDominio(3, ativo: false, idPeriodo: 30, usuarios: 8), CenarioE2.Periodo(30, 3));
        var service = c.PeriodosAbertosService();

        (await service.PeriodoAbertoAsync()).Should().Be(PeriodoStatus.Aberto);
        (await service.PeriodoAbertoAsync(7)).Should().Be(PeriodoStatus.Fechado);
        (await service.PeriodoAbertoAsync(8)).Should().Be(PeriodoStatus.Fechado, "domínio inativo não conta período");
    }

    [Fact]
    public async Task OP_E2_13_InfoPeriodo_CaixasEPadrao()
    {
        using var c = await CenarioAsync(true, CenarioE2.Usuario, 7);
        await c.SemearAsync(
            CenarioE2.NovoDominio(1, idPeriodo: 10, usuarios: CenarioE2.Usuario), CenarioE2.Periodo(10, 1),
            CenarioE2.NovoDominio(2, idCaixa: 1, usuarios: 7));
        c.Parametro(PeriodosAbertosService.ParametroDominioPadrao, "2");
        var service = c.PeriodosAbertosService();

        var info = await service.ObterInfoPeriodoAsync(10);
        info.Status.Should().Be(PeriodoStatus.Aberto);
        info.IdUsuario.Should().Be(CenarioE2.Usuario);
        info.IdCaixaBanco.Should().Be(1);
        info.NomeUsuario.Should().Be($"Usuário {CenarioE2.Usuario}");
        info.DescricaoCaixaBanco.Should().Be("Caixa 1");
        (await service.ObterInfoPeriodoAsync(0)).Status.Should().Be(PeriodoStatus.NaoAplicavel);
        (await service.ObterInfoPeriodoAsync(99)).Status.Should().Be(PeriodoStatus.Fechado);

        (await service.RetornarIdCaixaBancoAsync()).Should().Be(1);
        (await service.RetornarIdUsuarioDoPeriodoAsync(10)).Should().Be(CenarioE2.Usuario);
        (await service.ListarCaixasComPeriodoAbertoAsync()).Should().Equal(1);
        // O domínio padrão (2) não tem período: o legado só acha a linha se o domínio está ativo e com período.
        (await service.RetornarIdPeriodoPadraoAsync()).Should().BeNull();
    }

    // ---- VAL-E2-42 ---------------------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_42_RetornarIdPeriodo_DevolvePeriodoAbertoOuNulo()
    {
        // Sem o cache do legado, status Aberto sempre tem o período na lista: o ramo de erro
        // (MsgPeriodoNaoEncontrado) só ocorria com o cache desatualizado.
        using var c = await CenarioAsync(true, CenarioE2.Usuario, 7);
        await c.SemearAsync(
            CenarioE2.NovoDominio(1, idPeriodo: 10, usuarios: CenarioE2.Usuario), CenarioE2.Periodo(10, 1),
            CenarioE2.NovoDominio(2, usuarios: 7));

        var aberto = await c.PeriodosAbertosService().RetornarIdPeriodoAsync();
        c.Contexto.SetupGet(x => x.IdUsuario).Returns(7);
        var fechado = await c.PeriodosAbertosService().RetornarIdPeriodoAsync();

        aberto.Value.Should().Be(10);
        fechado.IsSuccess.Should().BeTrue();
        fechado.Value.Should().BeNull();
    }

    // ---- OP-E2-14 — EditarCaixaBancoDominioPeriodo -------------------------------------------

    [Fact]
    public async Task OP_E2_14_PeriodoNaoAberto_PodeEditar()
    {
        using var c = await CenarioAsync(trabalhaComDominio: false);

        (await c.PeriodosAbertosService().PodeEditarCaixaBancoAsync(validarBanco: false)).Should().BeTrue();
    }

    [Fact]
    public async Task OP_E2_14_PeriodoAberto_RegrasDoDominio()
    {
        using var c = await CenarioAsync(true, CenarioE2.Usuario);
        var dominio = CenarioE2.NovoDominio(1, idPeriodo: 10, usuarios: CenarioE2.Usuario);
        dominio.MovimentoBanco = true;
        await c.SemearAsync(dominio, CenarioE2.Periodo(10, 1));
        var service = c.PeriodosAbertosService();

        (await service.PodeEditarCaixaBancoAsync(validarBanco: true)).Should().BeTrue("MovimentoBanco com validarBanco");
        (await service.PodeEditarCaixaBancoAsync(validarBanco: false)).Should().BeFalse();

        c.Parametro(PeriodosAbertosService.ParametroDominioPadrao, "1");
        (await service.PodeEditarCaixaBancoAsync(validarBanco: false)).Should().BeTrue("domínio padrão");
    }

    [Fact]
    public async Task OP_E2_14_PeriodoAberto_TesoureiroComControleDeTesouraria()
    {
        using var c = await CenarioAsync(true, CenarioE2.Usuario);
        var dominio = CenarioE2.NovoDominio(1, idPeriodo: 10, usuarios: CenarioE2.Usuario);
        dominio.Tesouraria = true;
        await c.SemearAsync(dominio, CenarioE2.Periodo(10, 1));
        var service = c.PeriodosAbertosService();

        (await service.PodeEditarCaixaBancoAsync(false)).Should().BeFalse();
        c.Parametro(PeriodosAbertosService.ParametroControleCaixaTesouraria, "True");
        (await service.PodeEditarCaixaBancoAsync(false)).Should().BeTrue();
    }

    // ---- DominioFinanceiroConsulta sobre as entidades do E2 ----------------------------------

    [Fact]
    public async Task DominioFinanceiroConsulta_UsaAsEntidadesDoE2()
    {
        using var c = new CenarioE2();
        var dominio = CenarioE2.NovoDominio(2, idCaixa: 1, idPeriodo: 20, usuarios: 7);
        dominio.MovimentoBanco = true;
        await c.SemearAsync(dominio, CenarioE2.Periodo(20, 2), CenarioE2.NovoDominio(3, idCaixa: 1, ativo: false, idPeriodo: 21, usuarios: 8),
            CenarioE2.Periodo(21, 3));
        var consulta = new DominioFinanceiroConsulta(c.ReadContext);

        var periodos = await consulta.ListarPeriodosDosDominiosDoCaixaAsync(1, CenarioE2.Filial);

        periodos.Should().BeEquivalentTo([new DominioPeriodoInfo(2, null)], "só domínios ativos com período");
        (await consulta.ObterMovimentoBancoDoDominioDoUsuarioAsync(7, CenarioE2.Filial)).Should().BeTrue();
        (await consulta.ObterMovimentoBancoDoDominioDoUsuarioAsync(99, CenarioE2.Filial)).Should().BeNull();
        (await consulta.UsuarioPossuiDominioAsync(8, CenarioE2.Filial)).Should().BeTrue();
        (await consulta.CaixaPossuiDominioAsync(1, CenarioE2.Filial)).Should().BeTrue();
        (await consulta.CaixaPossuiDominioAsync(5, CenarioE2.Filial)).Should().BeFalse();
    }
}

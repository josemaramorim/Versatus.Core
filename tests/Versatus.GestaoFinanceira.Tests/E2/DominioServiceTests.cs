using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E2;

/// <summary>
/// E2-T04 — DominioService. 1 [Fact] por VAL-E2-01, 07..15, 28..37, 50 (matriz-rtv.md#E2) e testes
/// de integração (InMemory) das operações OP-E2-01, 02, 17, 18 (matriz-rot.md#E2).
/// </summary>
public class DominioServiceTests
{
    private static DominioUsuarioItemDto U(int id, bool principal = false, int? salvo = null) => new(id, principal, salvo);
    private static DominioResponsavelItemDto R(int idPai, bool principal = false, int? salvo = null) => new(idPai, principal, salvo);

    private static async Task<CenarioE2> CenarioComCaixaAsync()
    {
        var c = new CenarioE2();
        await c.SemearAsync(CenarioE2.Caixa(1));
        return c;
    }

    // ---- VAL-E2-01 ---------------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_01_ControleCaixaBancoFalha_NaoGrava()
    {
        using var c = await CenarioComCaixaAsync();
        c.ControleCaixaBanco.Setup(x => x.ValidarControleCaixaBancoAsync(It.IsAny<PeriodoStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidationResult.Fail(new ValidationError("Parametro", CaixaBancoService.MsgParametrosExclusivos)));

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgParametrosExclusivos);
        (await c.ReadContext.Dominios.CountAsync()).Should().Be(0);
    }

    // ---- VAL-E2-07 / VAL-E2-50 ------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_07_PeriodoAberto_AlterarDescricao_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, idPeriodo: 9, usuarios: 7));
        c.PeriodoAberto(9);

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Outra", usuarios: [U(7, true, 7)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgPeriodoEmAberto);
    }

    [Fact]
    public async Task VAL_E2_50_PeriodoAberto_DominioComum_PodeAlterarFlagsEUsuarios()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, idPeriodo: 9, usuarios: 7));
        c.PeriodoAberto(9);
        var dto = CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7), U(8)]) with { AbrirPeriodo = true, MovimentoBanco = true };

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial, dto);

        r.IsSuccess.Should().BeTrue();
        r.Value!.AbrirPeriodo.Should().BeTrue();
        r.Value.Usuarios.Should().HaveCount(2);
    }

    [Fact]
    public async Task VAL_E2_50_PeriodoAberto_DominioPadrao_SoUsuarios()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, idPeriodo: 9, usuarios: 7));
        c.PeriodoAberto(9);
        c.Parametro(DominioService.ParametroDominioPadrao, "2");
        var service = c.DominioService();

        var flags = await service.AtualizarAsync(2, CenarioE2.Filial,
            CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)]) with { FecharPeriodo = true });
        var usuarios = await service.AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7), U(8)]));

        flags.Errors.Single().Mensagem.Should().Be(DominioService.MsgPeriodoEmAberto);
        usuarios.IsSuccess.Should().BeTrue();
    }

    // ---- VAL-E2-08 / 09 -----------------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_08_SemUsuarios_Falha()
    {
        using var c = await CenarioComCaixaAsync();

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(usuarios: []));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgSemUsuarios);
    }

    [Fact]
    public async Task VAL_E2_09_SemPrincipalOuMaisDeUm_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, usuarios: [7, 8]));
        var service = c.DominioService();

        var nenhum = await service.AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, false, 7), U(8, false, 8)]));
        var dois = await service.AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7), U(8, true, 8)]));

        nenhum.Errors.Single().Mensagem.Should().Be(DominioService.MsgSemPrincipal);
        dois.Errors.Single().Mensagem.Should().Be(DominioService.MsgMaisDeUmPrincipal);
    }

    [Fact]
    public async Task VAL_E2_09_PrincipalSemPerfilDeDominio_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, usuarios: 7));
        c.Perfil(7, possuiPerfil: true, usaDominio: false);

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)]));

        r.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgUsuarioSemPerfilDominio, 7));
    }

    // ---- VAL-E2-10..13 ------------------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_10_DominioPadraoComResponsavel_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, usuarios: 7), CenarioE2.NovoDominio(3, usuarios: 9));
        c.Parametro(DominioService.ParametroDominioPadrao, "2");

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial,
            CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)], responsaveis: [R(3, true)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgPadraoComResponsavel);
    }

    [Fact]
    public async Task VAL_E2_11_TrabalhaComDominio_SemResponsaveis_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        c.Parametro(DominioService.ParametroTrabalhaComDominio, "True");

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgSemResponsaveis);
    }

    [Fact]
    public async Task VAL_E2_11_SemTrabalhaComDominio_ResponsaveisNaoSaoExigidos()
    {
        using var c = await CenarioComCaixaAsync();
        c.Parametro(DominioService.ParametroTrabalhaComDominio, "False");

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task VAL_E2_12_ResponsavelSemPrincipal_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        var dominio = CenarioE2.NovoDominio(2, usuarios: 7);
        dominio.AdicionarResponsavel(CenarioE2.Responsavel(2, 3, principal: true));
        await c.SemearAsync(dominio, CenarioE2.NovoDominio(3, usuarios: 9));
        c.Parametro(DominioService.ParametroTrabalhaComDominio, "True");

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial,
            CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)], responsaveis: [R(3, false, 3)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgSemResponsavelPrincipal);
    }

    [Fact]
    public async Task VAL_E2_13_ResponsavelIgualAoCadastroOuCiclico_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        var dominio = CenarioE2.NovoDominio(2, usuarios: 7);
        var tres = CenarioE2.NovoDominio(3, usuarios: 9);
        tres.AdicionarResponsavel(CenarioE2.Responsavel(3, 2)); // o 3 já tem o 2 como responsável
        await c.SemearAsync(dominio, tres);
        c.Parametro(DominioService.ParametroTrabalhaComDominio, "True");

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial,
            CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)], responsaveis: [R(2, true), R(3)]));

        var linhas = $"Linha 1: {string.Format(DominioService.MsgResponsavelIgualCadastro, 2, 2)}\r\n"
                   + $"Linha 2: {string.Format(DominioService.MsgResponsavelCiclico, 3, 2)}\r\n";
        r.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgErrosResponsaveis, linhas));
    }

    // ---- VAL-E2-14 / 15 -----------------------------------------------------------------------

    [Theory]
    [InlineData(ContaTipo.Banco, null, DominioService.MsgCaixaNaoCaixa)]
    [InlineData(ContaTipo.Caixa, TipoContaCaixa.Cofre, DominioService.MsgCaixaNaoNormal)]
    public async Task VAL_E2_14_CaixaQueNaoECaixaNormal_Falha(ContaTipo tipo, TipoContaCaixa? subtipo, string mensagem)
    {
        using var c = new CenarioE2();
        await c.SemearAsync(CenarioE2.Caixa(4, tipo, subtipo));

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(idCaixa: 4));

        r.Errors.Single().Mensagem.Should().Be(mensagem);
    }

    [Fact]
    public async Task VAL_E2_15_ReativarComUsuarioEmOutroDominioAtivo_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, ativo: false, usuarios: 7), CenarioE2.NovoDominio(3, usuarios: 7));

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)]));

        var linha = string.Format(DominioService.MsgLinhaUsuarioOutroDominio, 7, "Usuário 7", 3);
        r.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgAtivarUsuarioOutroDominio, 2, linha));
    }

    // ---- VAL-E2-28 --------------------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_28_SemDominioPadraoConfigurado_Falha()
    {
        using var c = new CenarioE2();

        var r = await c.DominioService().ObterUsuarioPadraoAsync();

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgSemDominioPadrao);
    }

    [Fact]
    public async Task VAL_E2_28_ComDominioPadrao_DevolveUsuarioPrincipal()
    {
        using var c = new CenarioE2();
        await c.SemearAsync(CenarioE2.NovoDominio(2, usuarios: [8, 7]));
        c.Parametro(DominioService.ParametroDominioPadrao, "2");

        var r = await c.DominioService().ObterUsuarioPadraoAsync();

        r.Value.Should().Be(8);
    }

    // ---- VAL-E2-29..34 — usuários -------------------------------------------------------------

    [Fact]
    public async Task VAL_E2_29_UsuarioRepetido_Falha()
    {
        using var c = await CenarioComCaixaAsync();

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(usuarios: [U(7, true), U(7)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgUsuarioJaInformado);
    }

    [Fact]
    public async Task VAL_E2_30_AlterarUsuarioSalvo_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(2, usuarios: 7));

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial, CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(8, true, 7)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgAlterarUsuarioSalvo);
    }

    [Fact]
    public async Task VAL_E2_31_UsuarioNovoSemPerfil_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        c.Perfil(7, possuiPerfil: false, usaDominio: false);

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgUsuarioSemPerfil, 7));
    }

    [Fact]
    public async Task VAL_E2_32_UsuarioNovoSemPerfilDeDominio_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        c.Perfil(7, possuiPerfil: true, usaDominio: false);

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgUsuarioSemPerfilDominio, 7));
    }

    [Fact]
    public async Task VAL_E2_33_UsuarioNovoEmOutroDominioAtivo_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(3, usuarios: 7));

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgUsuarioOutroDominioAtivo, 7, "Usuário 7"));
    }

    [Fact]
    public async Task VAL_E2_33_UsuarioEmOutroDominioInativo_Passa()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(3, ativo: false, usuarios: 7));

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto());

        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task VAL_E2_34_PrimeiroUsuarioNovo_ViraPrincipal()
    {
        using var c = await CenarioComCaixaAsync();

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(usuarios: [U(7, false), U(8, false)]));

        r.Value!.Usuarios.Single(u => u.UsuarioPrincipal).IdUsuario.Should().Be(7);
    }

    // ---- VAL-E2-35..37 — responsáveis ---------------------------------------------------------

    [Fact]
    public async Task VAL_E2_35_ResponsavelRepetido_Falha()
    {
        using var c = await CenarioComCaixaAsync();

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(responsaveis: [R(3, true), R(3)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgDominioJaInformado);
    }

    [Fact]
    public async Task VAL_E2_36_AlterarResponsavelSalvo_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        var dominio = CenarioE2.NovoDominio(2, usuarios: 7);
        dominio.AdicionarResponsavel(CenarioE2.Responsavel(2, 3));
        await c.SemearAsync(dominio);

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial,
            CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(7, true, 7)], responsaveis: [R(4, true, 3)]));

        r.Errors.Single().Mensagem.Should().Be(DominioService.MsgAlterarDominioSalvo);
    }

    [Fact]
    public async Task VAL_E2_37_PrimeiroResponsavel_ViraPrincipal()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(3, usuarios: 9));
        c.Parametro(DominioService.ParametroTrabalhaComDominio, "True");

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(responsaveis: [R(3, false)]));

        r.Value!.Responsaveis.Single().DominioPaiPrincipal.Should().BeTrue();
    }

    // ---- OP-E2-01 / OP-E2-02 -----------------------------------------------------------------

    [Fact]
    public async Task OP_E2_01_Criar_GravaComSequencialAuditoriaEListas()
    {
        using var c = await CenarioComCaixaAsync();
        await c.SemearAsync(CenarioE2.NovoDominio(3, usuarios: 9));

        var r = await c.DominioService().CriarAsync(CenarioE2.Dto(usuarios: [U(7, true), U(8)], responsaveis: [R(3, true)]));

        r.IsSuccess.Should().BeTrue();
        var gravado = await c.ReadContext.Dominios.Include(d => d.Usuarios).Include(d => d.Responsaveis).SingleAsync(d => d.IdDominio == 100);
        gravado.IdFilial.Should().Be(CenarioE2.Filial);
        gravado.IdUsuarioInclusao.Should().Be(CenarioE2.Usuario);
        gravado.IdDominioPeriodo.Should().BeNull();
        gravado.Usuarios.Should().HaveCount(2).And.OnlyContain(u => u.IdUsuarioInclusao == CenarioE2.Usuario);
        gravado.Responsaveis.Single().IdDominioPai.Should().Be(3);
    }

    [Fact]
    public async Task OP_E2_01_Atualizar_SincronizaUsuariosEResponsaveis()
    {
        using var c = await CenarioComCaixaAsync();
        var dominio = CenarioE2.NovoDominio(2, usuarios: [7, 8]);
        dominio.AdicionarResponsavel(CenarioE2.Responsavel(2, 3));
        await c.SemearAsync(dominio, CenarioE2.NovoDominio(3, usuarios: 9), CenarioE2.NovoDominio(4, usuarios: 10));

        var r = await c.DominioService().AtualizarAsync(2, CenarioE2.Filial,
            CenarioE2.Dto(descricao: "Domínio 2", usuarios: [U(8, true, 8), U(11)], responsaveis: [R(4, true)]));

        r.IsSuccess.Should().BeTrue();
        r.Value!.Usuarios.Select(u => (u.IdUsuario, u.UsuarioPrincipal)).Should().BeEquivalentTo([(8, true), (11, false)]);
        r.Value.Responsaveis.Select(x => x.IdDominioPai).Should().Equal(4);
        r.Value.IdUsuarioAlteracao.Should().Be(CenarioE2.Usuario);
    }

    [Fact]
    public async Task OP_E2_02_Excluir_RemoveDominioEListas()
    {
        using var c = await CenarioComCaixaAsync();
        var dominio = CenarioE2.NovoDominio(2, usuarios: 7);
        dominio.AdicionarResponsavel(CenarioE2.Responsavel(2, 3));
        await c.SemearAsync(dominio, CenarioE2.NovoDominio(3, usuarios: 9));

        var r = await c.DominioService().ExcluirAsync(2, CenarioE2.Filial);

        r.IsValid.Should().BeTrue();
        (await c.ReadContext.Dominios.CountAsync(d => d.IdDominio == 2)).Should().Be(0);
        (await c.ReadContext.DominioUsuarios.CountAsync(u => u.IdDominio == 2)).Should().Be(0);
        (await c.ReadContext.DominioResponsaveis.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task OP_E2_02_ExcluirComPeriodoOuComoResponsavel_Falha()
    {
        using var c = await CenarioComCaixaAsync();
        var filho = CenarioE2.NovoDominio(3, usuarios: 9);
        filho.AdicionarResponsavel(CenarioE2.Responsavel(3, 4));
        await c.SemearAsync(CenarioE2.NovoDominio(2, usuarios: 7), CenarioE2.Periodo(1, 2), CenarioE2.NovoDominio(4, usuarios: 10), filho);
        var service = c.DominioService();

        var comPeriodo = await service.ExcluirAsync(2, CenarioE2.Filial);
        var responsavel = await service.ExcluirAsync(4, CenarioE2.Filial);

        comPeriodo.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgExclusaoComVinculos, 2));
        responsavel.Errors.Single().Mensagem.Should().Be(string.Format(DominioService.MsgExclusaoComVinculos, 4));
    }

    // ---- OP-E2-17 — central ----------------------------------------------------------------

    [Fact]
    public async Task OP_E2_17_Central_ListaCaixasParaAbrirFecharEConsulta()
    {
        using var c = new CenarioE2();
        var logado = CenarioE2.NovoDominio(1, usuarios: CenarioE2.Usuario);
        logado.AbrirOutrosPeriodos = true;
        logado.FecharOutrosPeriodos = true;
        logado.ConsultaTodosPeriodos = true;
        var fechado = CenarioE2.NovoDominio(2, idCaixa: 2, usuarios: 7);        // filho sem período → abrir
        fechado.AdicionarResponsavel(CenarioE2.Responsavel(2, 1));
        var aberto = CenarioE2.NovoDominio(3, idCaixa: 3, idPeriodo: 30, usuarios: 8); // filho com período aberto → fechar
        aberto.AdicionarResponsavel(CenarioE2.Responsavel(3, 1));
        var neto = CenarioE2.NovoDominio(4, idCaixa: 4, usuarios: 9);           // filho do filho → consulta recursiva
        neto.AdicionarResponsavel(CenarioE2.Responsavel(4, 2));
        await c.SemearAsync(logado, fechado, aberto, neto, CenarioE2.Periodo(30, 3));

        var central = await c.DominioService().ObterCentralAsync();

        central.PodeAbrir.Should().BeTrue();
        central.CaixasParaAbrir.Select(i => i.IdUsuario).Should().BeEquivalentTo([7]);
        central.CaixasParaFechar.Select(i => (i.IdUsuario, i.IdDominioPeriodo)).Should().BeEquivalentTo([(8, 30)]);
        central.Consulta.Select(i => i.IdDominio).Should().BeEquivalentTo([1, 2, 3, 4]);
    }

    // ---- OP-E2-18 — consultas auxiliares -------------------------------------------------------

    [Fact]
    public async Task OP_E2_18_DominioDoUsuario_MenorIdDominio()
    {
        using var c = new CenarioE2();
        await c.SemearAsync(CenarioE2.NovoDominio(5, usuarios: 7), CenarioE2.NovoDominio(3, usuarios: 7));

        var dominio = await c.DominioService().ObterDominioDoUsuarioAsync(7);

        dominio!.IdDominio.Should().Be(3);
    }

    [Fact]
    public async Task OP_E2_18_UltimoFechamentoEValorAbertura()
    {
        using var c = new CenarioE2();
        await c.SemearAsync(
            CenarioE2.NovoDominio(2, idPeriodo: 12, usuarios: 7),
            CenarioE2.Periodo(11, 2, new DateTime(2026, 9, 30)),
            CenarioE2.Periodo(12, 2),
            new Domain.Dominio.DominioPeriodoLacto
            {
                IdDominioPeriodoLacto = 1, IdFilial = CenarioE2.Filial, IdPeriodoDestino = 12, TipoLancto = LanctoDominioPeriodoTipo.Abertura,
                TipoForma = FormaPagtoTipo.Dinheiro, DataLancto = new DateTime(2026, 10, 1), Valor = 150.5m,
            });
        var service = c.DominioService();

        var ultimo = await service.ObterUltimoFechamentoAsync(2, CenarioE2.Filial);
        var abertura = await service.ObterValorAberturaAsync(12);
        var semAbertura = await service.ObterValorAberturaAsync(11);

        ultimo!.DataFechamento.Should().Be(new DateTime(2026, 9, 30));
        abertura.Should().Be(150.5m);
        semAbertura.Should().Be(0m);
    }
}

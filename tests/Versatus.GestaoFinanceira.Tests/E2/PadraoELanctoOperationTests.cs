using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Handlers.Periodo;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Application.Services;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E2;

/// <summary>
/// E2-T05 — abertura/fechamento do domínio padrão (OP-E2-06/07), lançamento do período (OP-E2-10/11,
/// VAL-E2-38..41), forma de pagamento do período (OP-E2-12) e carga do fechamento (OP-E2-15).
/// </summary>
public class PadraoELanctoOperationTests
{
    private static string Msg<T>(Result<T> r) => r.Errors.Single().Mensagem;

    private static async Task<CenarioE2> PadraoAsync(bool periodoAberto, int? outroPeriodoAberto = null)
    {
        var c = new CenarioE2();
        c.ConfigurarDominioEOperacoes();
        c.Contexto.SetupGet(x => x.IdUsuario).Returns(9);
        c.Perfis.Setup(x => x.ListarPerfisDoUsuarioAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync([2]);

        var padrao = CenarioE2.NovoDominio(1, idCaixa: 1, idPeriodo: periodoAberto ? 10 : null, usuarios: 9);
        padrao.SaldoDinheiro = 400m;
        padrao.Tesouraria = true;
        var entidades = new List<object> { CenarioE2.Caixa(1), padrao };
        if (periodoAberto)
            entidades.Add(CenarioE2.Periodo(10, 1));
        if (outroPeriodoAberto is int p)
        {
            entidades.Add(CenarioE2.NovoDominio(2, idCaixa: 1, idPeriodo: p, usuarios: CenarioE2.Usuario));
            entidades.Add(CenarioE2.Periodo(p, 2));
        }
        await c.SemearAsync([.. entidades]);
        return c;
    }

    // ======================= Abertura do domínio padrão (OP-E2-06) =================================

    [Fact]
    public async Task OP_E2_06_AbrirPadraoExistente_GravaPeriodoLanctoParametrosPerfilEPonteiro()
    {
        using var c = await PadraoAsync(periodoAberto: false);
        c.Saldos.Setup(x => x.RetornarSaldoCaixaBancoAsync(1, false, DateTime.Today, TipoSaldo.Atual, It.IsAny<CancellationToken>())).ReturnsAsync(1000m);

        var r = await c.AbrirPadrao().ExecutarAsync(new AbrirDominioPadraoCommand(1, null, 0, null, null));

        r.IsSuccess.Should().BeTrue(r.IsSuccess ? "" : Msg(r));
        var dominio = await c.ReadContext.Dominios.SingleAsync(d => d.IdDominio == 1);
        (dominio.AbrirPeriodo, dominio.AbrirOutrosPeriodos, dominio.FecharPeriodo, dominio.FecharOutrosPeriodos, dominio.MovimentoBanco, dominio.ConsultaTodosPeriodos)
            .Should().Be((true, true, true, true, true, true));
        dominio.IdDominioPeriodo.Should().Be(r.Value);
        dominio.SaldoDinheiro.Should().Be(1400m, "abertura do padrão soma no destino (sem origem)");

        var lancto = await c.ReadContext.DominioPeriodoLactos.SingleAsync();
        (lancto.TipoLancto, lancto.IdPeriodoOrigem, lancto.IdPeriodoDestino, lancto.IdCaixaBanco, lancto.Valor)
            .Should().Be((LanctoDominioPeriodoTipo.Abertura, (int?)null, r.Value, 1, 1000m));

        c.Cheques.Verify(x => x.AtribuirPeriodoATodosTaloesAsync(r.Value, It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.GravarAsync("CONTROLECAIXATESOURARIA", "True", It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.GravarAsync("TRABALHACOMDOMINIO", "True", It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.GravarAsync("CHEQUEEMPRESAPORSUPRIMENTO", "True", It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.GravarAsync("DOMINIOPADRAO", "1", It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.MarcarPerfilUsaDominioAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OP_E2_06_AbrirPadraoNovo_CriaDominioComChequesRecebidos()
    {
        using var c = new CenarioE2();
        c.ConfigurarDominioEOperacoes();
        c.Perfis.Setup(x => x.ListarPerfisDoUsuarioAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync([2]);
        c.Cheques.Setup(x => x.ListarChequesRecebidosEmAbertoAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ChequePeriodo>>.Ok([new ChequePeriodo(55, null, null, 80m)]));
        await c.SemearAsync(CenarioE2.Caixa(1));

        var r = await c.AbrirPadrao().ExecutarAsync(new AbrirDominioPadraoCommand(null, "  Padrão  ", 1, [new DominioUsuarioItemDto(9, false)], null));

        r.IsSuccess.Should().BeTrue(r.IsSuccess ? "" : Msg(r));
        var dominio = await c.ReadContext.Dominios.Include(d => d.Usuarios).SingleAsync();
        dominio.Descricao.Should().Be("Padrão");
        dominio.Usuarios.Single().UsuarioPrincipal.Should().BeTrue();
        var cheque = await c.ReadContext.DominioPeriodoLactos.SingleAsync();
        (cheque.TipoForma, cheque.IdChequeRecebido, cheque.IdPeriodoDestino, cheque.Valor).Should().Be((FormaPagtoTipo.ChequeCliente, 55, r.Value, 80m));
        dominio.SaldoChequeRecebido.Should().Be(80m);
    }

    [Fact]
    public async Task VAL_E2_25_AberturaPadraoComPeriodoAbertoOuDominioInativo_Falha()
    {
        using var c = await PadraoAsync(periodoAberto: false, outroPeriodoAberto: 20);
        Msg(await c.AbrirPadrao().ExecutarAsync(new AbrirDominioPadraoCommand(1, null, 0, null, null))).Should().Be(MensagensPeriodo.PadraoTodosFechados);

        using var inativo = await PadraoAsync(periodoAberto: false);
        var d = await inativo.Context.Dominios.SingleAsync();
        d.Ativo = false;
        await inativo.Context.SaveChangesAsync();
        Msg(await inativo.AbrirPadrao().ExecutarAsync(new AbrirDominioPadraoCommand(1, null, 0, null, null))).Should().Be(MensagensPeriodo.PadraoSomenteAtivo);
    }

    [Fact]
    public async Task VAL_E2_48_AberturaPadrao_CaixaDescricaoPrincipalEPerfil()
    {
        using var c = new CenarioE2();
        c.ConfigurarDominioEOperacoes();
        c.Perfis.Setup(x => x.ListarPerfisDoUsuarioAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync([2, 3]);
        await c.SemearAsync(CenarioE2.Caixa(1));
        var abrir = c.AbrirPadrao();
        IReadOnlyList<DominioUsuarioItemDto> usuarios = [new(9, true)];

        Msg(await abrir.ExecutarAsync(new AbrirDominioPadraoCommand(null, " ", 1, usuarios, null))).Should().Be(MensagensPeriodo.PadraoInformarDescricao);
        Msg(await abrir.ExecutarAsync(new AbrirDominioPadraoCommand(null, "Padrão", 0, usuarios, null))).Should().Be(MensagensPeriodo.PadraoInformarCaixa);
        Msg(await abrir.ExecutarAsync(new AbrirDominioPadraoCommand(null, "Padrão", 1, usuarios, null))).Should().Be(MensagensPeriodo.PadraoMarcarPerfil);
        Msg(await abrir.ExecutarAsync(new AbrirDominioPadraoCommand(null, "Padrão", 1, usuarios, [2, 3]))).Should().Be(MensagensPeriodo.PadraoSomenteUmPerfil);
    }

    [Fact]
    public async Task VAL_E2_05_06_AberturaPadrao_SemUsuariosOuComResponsavel()
    {
        using var c = new CenarioE2();
        c.ConfigurarDominioEOperacoes();
        await c.SemearAsync(CenarioE2.Caixa(1));
        Msg(await c.AbrirPadrao().ExecutarAsync(new AbrirDominioPadraoCommand(null, "Padrão", 1, [], null)))
            .Should().Be(string.Format(MensagensPeriodo.PadraoUsuariosObrigatorios, 0));

        using var comResponsavel = await PadraoAsync(periodoAberto: false);
        var d = await comResponsavel.Context.Dominios.SingleAsync();
        d.AdicionarResponsavel(CenarioE2.Responsavel(1, 3));
        await comResponsavel.Context.SaveChangesAsync();
        Msg(await comResponsavel.AbrirPadrao().ExecutarAsync(new AbrirDominioPadraoCommand(1, null, 0, null, null)))
            .Should().Be(string.Format(MensagensPeriodo.PadraoSemResponsavel, 1));
    }

    // ======================= Fechamento do domínio padrão (OP-E2-07) ===============================

    [Fact]
    public async Task OP_E2_07_FecharPadrao_FechaComSaldoEmDinheiroEDesligaParametros()
    {
        using var c = await PadraoAsync(periodoAberto: true);
        c.Parametro("CONTROLECAIXATESOURARIA", "True");

        var r = await c.FecharPadrao().ExecutarAsync();

        r.IsSuccess.Should().BeTrue(r.IsSuccess ? "" : Msg(r));
        var periodo = await c.ReadContext.DominioPeriodos.SingleAsync();
        periodo.DataFechamento.Should().Be(DateTime.Today);
        periodo.DataFechamentoTesouraria.Should().BeNull("o fechamento do padrão não fecha a tesouraria");

        var lancto = await c.ReadContext.DominioPeriodoLactos.SingleAsync();
        (lancto.TipoLancto, lancto.IdPeriodoOrigem, lancto.IdPeriodoDestino, lancto.IdCaixaBanco, lancto.Valor)
            .Should().Be((LanctoDominioPeriodoTipo.Fechamento, 10, (int?)null, 1, 400m));
        (await c.ReadContext.Dominios.SingleAsync()).SaldoDinheiro.Should().Be(0m);

        c.ParametroEscrita.Verify(x => x.GravarAsync("TRABALHACOMDOMINIO", "False", It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.GravarAsync("DOMINIOPADRAO", "0", It.IsAny<CancellationToken>()), Times.Once);
        c.ParametroEscrita.Verify(x => x.MarcarPerfilUsaDominioAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task VAL_E2_24_FecharPadrao_SoUsuarioDoPadraoNaoFechadoEDemaisFechados()
    {
        using var outroUsuario = await PadraoAsync(periodoAberto: true);
        outroUsuario.Contexto.SetupGet(x => x.IdUsuario).Returns(CenarioE2.Usuario);
        Msg(await outroUsuario.FecharPadrao().ExecutarAsync()).Should().Be(MensagensPeriodo.SomentePadrao);

        using var jaFechado = await PadraoAsync(periodoAberto: true);
        var p = await jaFechado.Context.DominioPeriodos.SingleAsync();
        p.DataFechamento = new DateTime(2026, 10, 2);
        await jaFechado.Context.SaveChangesAsync();
        Msg(await jaFechado.FecharPadrao().ExecutarAsync()).Should().Be("O domínio ({0}) está fechado.", "o legado não formata a mensagem (Q3)");

        using var demais = await PadraoAsync(periodoAberto: true, outroPeriodoAberto: 20);
        Msg(await demais.FecharPadrao().ExecutarAsync()).Should().Be(MensagensPeriodo.PadraoDemaisFechados);
    }

    // ======================= Lançamento do período (OP-E2-10/11, VAL-E2-38..41) ====================

    private static DominioPeriodoLacto Lancto(FormaPagtoTipo forma, decimal valor) => new()
    {
        TipoLancto = LanctoDominioPeriodoTipo.Suprimento,
        TipoForma = forma,
        IdPeriodoOrigem = 10,
        IdPeriodoDestino = 20,
        Valor = valor,
        DataLancto = DateTime.Now,
    };

    private static async Task<CenarioE2> DoisPeriodosAsync()
    {
        var c = new CenarioE2();
        c.ConfigurarDominioEOperacoes(idDominioPadrao: 9);
        var a = CenarioE2.NovoDominio(1, idPeriodo: 10, usuarios: 9);
        a.SaldoChequeRecebido = 100m;
        await c.SemearAsync(a, CenarioE2.Periodo(10, 1), CenarioE2.NovoDominio(2, idPeriodo: 20, usuarios: CenarioE2.Usuario), CenarioE2.Periodo(20, 2));
        return c;
    }

    [Fact]
    public async Task OP_E2_10_ChequeDeClienteEDeEmpresa_AtribuemPeriodoDeDestino()
    {
        using var c = await DoisPeriodosAsync();
        var service = c.LanctoService();

        var cliente = Lancto(FormaPagtoTipo.ChequeCliente, 30m);
        cliente.IdChequeRecebido = 55;
        (await service.GravarAsync(cliente, StatusDominioFinanceiro.SuprimentoCheque)).IsValid.Should().BeTrue();

        var empresa = Lancto(FormaPagtoTipo.ChequeEmpresa, 40m);
        empresa.IdCaixaBanco = 3;
        empresa.Cheque = 1001;
        (await service.GravarAsync(empresa, StatusDominioFinanceiro.SuprimentoCheque)).IsValid.Should().BeTrue();

        c.Cheques.Verify(x => x.AtribuirPeriodoAChequeRecebidoAsync(55, 20, It.IsAny<CancellationToken>()), Times.Once);
        c.Cheques.Verify(x => x.AtribuirPeriodoAFolhaTalaoAsync(3, 1001, 20, It.IsAny<CancellationToken>()), Times.Once);
        c.Movimentos.VerifyNoOtherCalls();
        var dominios = await c.ReadContext.Dominios.ToDictionaryAsync(d => d.IdDominio);
        dominios[1].SaldoChequeRecebido.Should().Be(70m, "cheque de cliente sai da origem; cheque de empresa não conta saldo");
        dominios[2].SaldoChequeRecebido.Should().Be(30m);
    }

    [Fact]
    public async Task OP_E2_11_ExcluirLancto_SubtraiSaldoSemGuardas()
    {
        using var c = await DoisPeriodosAsync();
        var service = c.LanctoService();
        var lancto = Lancto(FormaPagtoTipo.ChequeCliente, 30m);
        lancto.IdChequeRecebido = 55;
        await service.GravarAsync(lancto, StatusDominioFinanceiro.SuprimentoCheque);

        await service.ExcluirAsync(lancto, StatusDominioFinanceiro.SuprimentoCheque);

        (await c.ReadContext.DominioPeriodoLactos.CountAsync()).Should().Be(0);
        var dominios = await c.ReadContext.Dominios.ToDictionaryAsync(d => d.IdDominio);
        dominios[1].SaldoChequeRecebido.Should().Be(100m);
        dominios[2].SaldoChequeRecebido.Should().Be(0m);
    }

    [Fact]
    public async Task VAL_E2_38_OperacoesNaoConfiguradas_Falha()
    {
        using var c = await DoisPeriodosAsync();
        c.Parametro("OPERACAOPADRAOSANGRIA", null);

        (await c.LanctoService().GravarAsync(Lancto(FormaPagtoTipo.Dinheiro, 10m), StatusDominioFinanceiro.SuprimentoDinheiro))
            .Errors.Single().Mensagem.Should().Be(DominioPeriodoLactoService.MsgOperacoesNaoConfiguradas);
    }

    [Theory]
    [InlineData(25, 2, NaturezaTipo.Credora, false, DominioPeriodoLactoService.MsgSuprimentoNaoCaixa)]      // VAL-E2-39
    [InlineData(26, 2, NaturezaTipo.Devedora, false, DominioPeriodoLactoService.MsgSangriaNaoCaixa)]        // VAL-E2-39
    [InlineData(25, 1, NaturezaTipo.Credora, true, DominioPeriodoLactoService.MsgSuprimentoMovtoEntidade)]  // VAL-E2-40 (Q4)
    [InlineData(26, 1, NaturezaTipo.Devedora, true, DominioPeriodoLactoService.MsgSangriaMovtoEntidade)]    // VAL-E2-40 (Q4)
    [InlineData(25, 1, NaturezaTipo.Devedora, false, DominioPeriodoLactoService.MsgSuprimentoNaoCredora)]   // VAL-E2-41
    [InlineData(26, 1, NaturezaTipo.Credora, false, DominioPeriodoLactoService.MsgSangriaNaoDevedora)]      // VAL-E2-41
    public async Task VAL_E2_39_40_41_OperacaoPadraoInvalida_Falha(int idOperacao, int tipo, NaturezaTipo natureza, bool lancaMovtoEntidade, string mensagem)
    {
        using var c = await DoisPeriodosAsync();
        c.Operacoes.Setup(x => x.ObterAsync(idOperacao, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperacaoCaixaInfo(idOperacao, tipo, natureza, lancaMovtoEntidade));

        (await c.LanctoService().GravarAsync(Lancto(FormaPagtoTipo.Dinheiro, 10m), StatusDominioFinanceiro.SuprimentoDinheiro))
            .Errors.Single().Mensagem.Should().Be(mensagem);
    }

    // ======================= Forma de pagamento do período (OP-E2-12) ==============================

    [Fact]
    public async Task OP_E2_12_FormaPagto_MontaNoPeriodoDoLogadoEGravaSaldo()
    {
        using var c = await DoisPeriodosAsync();
        c.Periodos.Setup(x => x.RetornarIdPeriodoAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Result<int?>.Ok(20));
        var service = c.FormaPagtoService();

        var montada = await service.MontarAsync(4, 75m, NaturezaTipo.Credora, 77, ProcessoOrigem.Documento);
        await service.GravarAsync(montada.Value!, FormaPagtoTipo.Dinheiro);

        var forma = await c.ReadContext.DominioPeriodoFormasPagto.SingleAsync();
        (forma.IdDominioPeriodo, forma.Credito, forma.Debito, forma.IdOrigem, forma.Hora.Length).Should().Be((20, 75m, (decimal?)null, 77, 8));
        (await c.ReadContext.Dominios.SingleAsync(d => d.IdDominio == 2)).SaldoDinheiro.Should().Be(75m);

        await service.ExcluirAsync(montada.Value!, FormaPagtoTipo.Dinheiro);
        (await c.ReadContext.Dominios.SingleAsync(d => d.IdDominio == 2)).SaldoDinheiro.Should().Be(0m);
    }

    // ======================= Carga do fechamento (OP-E2-15) =========================================

    [Fact]
    public async Task OP_E2_15_CargaDoFechamento_FiltraFormasEAplicaInformados()
    {
        using var c = new CenarioE2();
        c.Resumos.Setup(x => x.ListarAsync(20, It.IsAny<CancellationToken>())).ReturnsAsync([
            new ResumoFormaPagto(20, "Dinheiro", FormaPagtoTipo.Dinheiro, 300m, 50m),
            new ResumoFormaPagto(20, "Sem saldo", FormaPagtoTipo.CartaoCredito, 10m, 10m),
            new ResumoFormaPagto(20, "Depósito", FormaPagtoTipo.Deposito, 40m, 0m),
            new ResumoFormaPagto(20, "Cheque emitido", FormaPagtoTipo.ChequeEmpresa, 0m, 0m),
        ]);
        c.Cheques.Setup(x => x.ListarFolhasTalaoDoPeriodoAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ChequePeriodo>>.Ok([new ChequePeriodo(null, 3, 1, 10m), new ChequePeriodo(null, 3, 2, 20m)]));

        var r = await c.FechamentoService().CarregarAsync(20, [new FormaInformada(FormaPagtoTipo.Dinheiro, 0, 240m), new FormaInformada(FormaPagtoTipo.ChequeEmpresa, 2, 0m)]);

        var formas = r.Value!.Formas.ToDictionary(f => f.TipoFormaPagto);
        formas.Keys.Should().BeEquivalentTo([FormaPagtoTipo.Dinheiro, FormaPagtoTipo.Deposito, FormaPagtoTipo.ChequeEmpresa]);
        (formas[FormaPagtoTipo.Dinheiro].ValorCalculado, formas[FormaPagtoTipo.Dinheiro].ValorConferencia, formas[FormaPagtoTipo.Dinheiro].Diferenca)
            .Should().Be((250m, 240m, -10m));
        formas[FormaPagtoTipo.Deposito].ValorConferencia.Should().Be(40m, "forma não listável confere pelo calculado");
        (formas[FormaPagtoTipo.ChequeEmpresa].ValorCalculado, formas[FormaPagtoTipo.ChequeEmpresa].ValorConferencia).Should().Be((2m, 2m));
        r.Value.Divergentes.Select(f => f.TipoFormaPagto).Should().Equal(FormaPagtoTipo.Dinheiro);
        (r.Value.TotalCalculado, r.Value.TotalConferencia).Should().Be((290m, 280m), "o cheque emitido fica fora do total");
    }
}

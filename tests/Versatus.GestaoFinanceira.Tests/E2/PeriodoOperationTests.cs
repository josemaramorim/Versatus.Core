using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Handlers.Periodo;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.GestaoFinanceira.Application.Services;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E2;

/// <summary>
/// E2-T05 — handlers de abertura, fechamento e suprimento (OP-E2-03, 04, 05, 08) e as VAL do escopo.
/// Hierarquia dos testes: domínio 1 = padrão (usuário 9, caixa 1, período 10 aberto, R$ 500 em dinheiro);
/// domínio 2 = do usuário logado (5, caixa 2, pai = 1); domínio 3 = filho do 2 (usuário 7, caixa 3).
/// InMemory não tem transação: o rollback é do SQL Server (validado no E2-T06 contra o banco).
/// </summary>
public class PeriodoOperationTests
{
    private static async Task<CenarioE2> CenarioAsync(Action<Dominio>? logado = null, int? periodoLogado = null, int? periodoFilho = null,
        DateTime? fechamentoFilho = null, DateTime? fechamentoPadrao = null)
    {
        var c = new CenarioE2();
        c.ConfigurarDominioEOperacoes();

        var padrao = CenarioE2.NovoDominio(1, idCaixa: 1, idPeriodo: 10, usuarios: 9);
        padrao.SaldoDinheiro = 500m;
        var dois = CenarioE2.NovoDominio(2, idCaixa: 2, idPeriodo: periodoLogado, usuarios: CenarioE2.Usuario);
        dois.AdicionarResponsavel(CenarioE2.Responsavel(2, 1));
        logado?.Invoke(dois);
        var tres = CenarioE2.NovoDominio(3, idCaixa: 3, idPeriodo: periodoFilho, usuarios: 7);
        tres.AdicionarResponsavel(CenarioE2.Responsavel(3, 2));

        var entidades = new List<object>
        {
            CenarioE2.Caixa(1), CenarioE2.Caixa(2), CenarioE2.Caixa(3), padrao, dois, tres,
            CenarioE2.Periodo(10, 1, fechamentoPadrao),
        };
        if (periodoLogado is int p2)
            entidades.Add(CenarioE2.Periodo(p2, 2));
        if (periodoFilho is int p3)
            entidades.Add(CenarioE2.Periodo(p3, 3, fechamentoFilho));
        await c.SemearAsync([.. entidades]);
        return c;
    }

    private static string Msg<T>(Result<T> r) => r.Errors.Single().Mensagem;

    // ======================= Abertura (OP-E2-03) ==================================================

    [Fact]
    public async Task OP_E2_03_AbrirProprioCaixa_GravaPeriodoLanctoMovimentosSaldoEPonteiro()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);

        var r = await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 100m));

        r.IsSuccess.Should().BeTrue(r.IsSuccess ? "" : Msg(r));
        var periodo = await c.ReadContext.DominioPeriodos.SingleAsync(p => p.IdDominioPeriodo == r.Value);
        periodo.IdDominio.Should().Be(2);
        periodo.DataAbertura.Should().Be(DateTime.Today);
        periodo.DataFechamento.Should().BeNull();

        var lancto = await c.ReadContext.DominioPeriodoLactos.SingleAsync();
        lancto.TipoLancto.Should().Be(LanctoDominioPeriodoTipo.Abertura);
        (lancto.IdPeriodoOrigem, lancto.IdPeriodoDestino, lancto.IdCaixaBanco, lancto.Valor).Should().Be((10, r.Value, 2, 100m));

        c.Movimentos.Verify(x => x.GerarAsync(It.Is<MovimentoPeriodo>(m => m.IdCaixaBanco == 1 && m.IdOperacao == 26 && m.IdDominioPeriodo == 10 && m.Valor == 100m),
            It.IsAny<CancellationToken>()), Times.Once);
        c.Movimentos.Verify(x => x.GerarAsync(It.Is<MovimentoPeriodo>(m => m.IdCaixaBanco == 2 && m.IdOperacao == 25 && m.IdDominioPeriodo == r.Value),
            It.IsAny<CancellationToken>()), Times.Once);

        var dominios = await c.ReadContext.Dominios.ToDictionaryAsync(d => d.IdDominio);
        dominios[1].SaldoDinheiro.Should().Be(400m);
        dominios[2].SaldoDinheiro.Should().Be(100m);
        dominios[2].IdDominioPeriodo.Should().Be(r.Value);
    }

    [Fact]
    public async Task OP_E2_03_SemValor_NaoGravaLancto()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);

        var r = await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m));

        r.IsSuccess.Should().BeTrue();
        (await c.ReadContext.DominioPeriodoLactos.CountAsync()).Should().Be(0);
        c.Movimentos.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task OP_E2_03_MovimentoFinanceiroPendenteDoE5_RecusaAbertura()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);
        c.Movimentos.Setup(x => x.GerarAsync(It.IsAny<MovimentoPeriodo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidationResult.Fail(new ValidationError("MovimentoFinanceiro", "pendente")));

        var r = await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 100m));

        Msg(r).Should().Be("pendente");
    }

    [Fact]
    public async Task VAL_E2_01_ControleCaixaBancoFalha()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);
        c.ControleCaixaBanco.Setup(x => x.ValidarControleCaixaBancoAsync(It.IsAny<PeriodoStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidationResult.Fail(new ValidationError("Parametro", "exclusivos")));

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m))).Should().Be("exclusivos");
    }

    [Fact]
    public async Task VAL_E2_02_DominioInativo_MensagemComIdDoLogado()
    {
        using var c = await CenarioAsync(d => { d.AbrirPeriodo = true; d.Ativo = false; });

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.DominioInativo, 2));
    }

    [Theory]
    [InlineData("TRABALHACOMDOMINIO", "False", MensagensPeriodo.SemAcesso)]      // VAL-E2-03
    [InlineData("DOMINIOPADRAO", "0", MensagensPeriodo.SemDominioPadrao)]         // VAL-E2-04
    public async Task VAL_E2_03_04_ConfiguracaoDoDominio(string parametro, string valor, string mensagem)
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);
        c.Parametro(parametro, valor);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m))).Should().Be(mensagem);
    }

    [Fact]
    public async Task VAL_E2_19_ValorMaiorQueSaldoDaSangria_Falha()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 1000m)))
            .Should().Be(string.Format(MensagensPeriodo.ValorMaiorQueSaldo, "1.000,00", "500,00"));
    }

    [Fact]
    public async Task VAL_E2_20_LogadoSemPermissaoDeAbrir_Falha()
    {
        using var c = await CenarioAsync();

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.LogadoSemConfiguracaoAbertura, CenarioE2.Usuario));
    }

    [Fact]
    public async Task VAL_E2_21_CaixaIgualSangria_Falha()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, CenarioE2.Usuario, 0m)))
            .Should().Be(MensagensPeriodo.CaixaIgualSangria);
    }

    [Fact]
    public async Task VAL_E2_22_PeriodoDoCaixaJaAberto_Falha()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true, periodoLogado: 20);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.PeriodoJaAberto, CenarioE2.Usuario));
    }

    [Fact]
    public async Task VAL_E2_23_SangriaComPeriodoFechado_PadraoOuComum()
    {
        using var padrao = await CenarioAsync(d => d.AbrirPeriodo = true, fechamentoPadrao: new DateTime(2026, 10, 2));
        Msg(await padrao.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m)))
            .Should().Be($"{MensagensPeriodo.AbrirPadrao}\r\n\r\n{MensagensPeriodo.AberturaPadraoOutraRotina}");

        using var comum = await CenarioAsync(d => d.AbrirPeriodo = true, fechamentoPadrao: new DateTime(2026, 10, 2));
        comum.Parametro("DOMINIOPADRAO", "99");
        Msg(await comum.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.SangriaPeriodoFechado, 9));
    }

    [Fact]
    public async Task VAL_E2_43_CaixaDoDominioPadrao_Falha()
    {
        using var c = await CenarioAsync();
        c.Contexto.SetupGet(x => x.IdUsuario).Returns(9);
        var padrao = await c.Context.Dominios.SingleAsync(d => d.IdDominio == 1);
        padrao.AbrirPeriodo = true;
        await c.Context.SaveChangesAsync();

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(9, 5, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.CaixaNaoPermitido, 9));
    }

    [Fact]
    public async Task VAL_E2_44_CaixaForaDaHierarquiaOuOutroComPeriodoDoLogadoFechado_Falha()
    {
        using var c = await CenarioAsync(d => { d.AbrirPeriodo = true; d.AbrirOutrosPeriodos = true; });
        var abrir = c.AbrirPeriodo();

        Msg(await abrir.ExecutarAsync(new AbrirPeriodoCommand(9, CenarioE2.Usuario, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.CaixaNaoPodeSerAberto, 9, CenarioE2.Usuario));
        Msg(await abrir.ExecutarAsync(new AbrirPeriodoCommand(7, CenarioE2.Usuario, 0m)))
            .Should().Be(string.Format(MensagensPeriodo.LogadoPeriodoDeveEstarAberto, CenarioE2.Usuario));
    }

    [Fact]
    public async Task VAL_E2_46_SangriaForaDaHierarquia_Falha()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 7, 0m))).Should().Be(MensagensPeriodo.SangriaNaoPermitida);
    }

    [Theory]
    [InlineData(0, 9, MensagensPeriodo.InformarUsuarioCaixa)]
    [InlineData(CenarioE2.Usuario, 0, MensagensPeriodo.InformarUsuarioSangria)]
    public async Task VAL_E2_47_UsuariosObrigatorios(int caixa, int sangria, string mensagem)
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(caixa, sangria, 0m))).Should().Be(mensagem);
    }

    [Fact]
    public async Task VAL_E2_49_LogadoComPerfilSemDominio_Falha()
    {
        using var c = await CenarioAsync(d => d.AbrirPeriodo = true);
        c.Perfil(CenarioE2.Usuario, possuiPerfil: true, usaDominio: false);

        Msg(await c.AbrirPeriodo().ExecutarAsync(new AbrirPeriodoCommand(CenarioE2.Usuario, 9, 0m))).Should().Be(MensagensPeriodo.PerfilSemDominio);
    }

    // ======================= Fechamento (OP-E2-04 / 05) ===========================================

    private static void ResumoDinheiro(CenarioE2 c, int idPeriodo, decimal credito)
        => c.Resumos.Setup(x => x.ListarAsync(idPeriodo, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ResumoFormaPagto(idPeriodo, "Dinheiro", FormaPagtoTipo.Dinheiro, credito, 0m)]);

    [Fact]
    public async Task OP_E2_04_FecharProprioCaixa_FechaPeriodoGravaFechamentoLanctoESaldo()
    {
        using var c = await CenarioAsync(d => { d.FecharPeriodo = true; d.SaldoDinheiro = 300m; }, periodoLogado: 20);
        ResumoDinheiro(c, 20, 300m);

        var r = await c.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 9, [new FormaInformada(FormaPagtoTipo.Dinheiro, 0, 290m)]));

        r.IsSuccess.Should().BeTrue(r.IsSuccess ? "" : Msg(r));
        var periodo = await c.ReadContext.DominioPeriodos.Include(p => p.Fechamentos).SingleAsync(p => p.IdDominioPeriodo == 20);
        periodo.DataFechamento.Should().Be(DateTime.Today);
        periodo.IdUsuarioFechamento.Should().Be(CenarioE2.Usuario);
        periodo.DataFechamentoTesouraria.Should().BeNull();
        periodo.Fechamentos.Single().Should().Match<DominioPeriodoFechamento>(f =>
            f.TipoForma == FormaPagtoTipo.Dinheiro && f.ValorCalculado == 300m && f.ValorInformado == 290m);

        var lancto = await c.ReadContext.DominioPeriodoLactos.SingleAsync();
        (lancto.TipoLancto, lancto.IdPeriodoOrigem, lancto.IdPeriodoDestino, lancto.IdCaixaBanco, lancto.Valor)
            .Should().Be((LanctoDominioPeriodoTipo.Fechamento, 20, 10, 1, 300m));

        var dominios = await c.ReadContext.Dominios.ToDictionaryAsync(d => d.IdDominio);
        dominios[2].SaldoDinheiro.Should().Be(0m);
        dominios[1].SaldoDinheiro.Should().Be(800m);
        dominios[2].IdDominioPeriodo.Should().Be(20, "o fechamento não muda o ponteiro");
    }

    [Fact]
    public async Task OP_E2_05_ControleCaixaTesouraria_PreencheDatasDaTesouraria()
    {
        using var c = await CenarioAsync(d => d.FecharPeriodo = true, periodoLogado: 20);
        c.Parametro("CONTROLECAIXATESOURARIA", "True");

        var r = await c.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 9, []));

        r.IsSuccess.Should().BeTrue();
        (await c.ReadContext.DominioPeriodos.SingleAsync(p => p.IdDominioPeriodo == 20)).DataFechamentoTesouraria.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task VAL_E2_26_CaixaComPeriodoFechadoOuSemPeriodo_Falha()
    {
        using var fechado = await CenarioAsync(d => { d.FecharOutrosPeriodos = true; }, periodoLogado: 20, periodoFilho: 30,
            fechamentoFilho: new DateTime(2026, 10, 2));
        Msg(await fechado.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(7, CenarioE2.Usuario, [])))
            .Should().Be(string.Format(MensagensPeriodo.DominioFechado, 3));

        using var semPeriodo = await CenarioAsync(d => { d.FecharOutrosPeriodos = true; }, periodoLogado: 20);
        Msg(await semPeriodo.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(7, CenarioE2.Usuario, [])))
            .Should().Be(string.Format(MensagensPeriodo.CaixaSemPeriodoAberto, 7));
    }

    [Fact]
    public async Task VAL_E2_27_SuprimentoComPeriodoFechadoOuNaoLogado_Falha()
    {
        // Pai 1 aberto e pai 4 fechado do domínio 2: o suprimento pode ser de qualquer pai, mas precisa de período aberto.
        using var c = await CenarioAsync(d => { d.FecharPeriodo = true; d.AdicionarResponsavel(CenarioE2.Responsavel(2, 4, principal: false)); },
            periodoLogado: 20);
        await c.SemearAsync(CenarioE2.NovoDominio(4, idCaixa: 1, idPeriodo: 40, usuarios: 8), CenarioE2.Periodo(40, 4, new DateTime(2026, 10, 2)));
        Msg(await c.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 8, [])))
            .Should().Be(string.Format(MensagensPeriodo.SuprimentoFechamentoFechado, 8));

        // Fechando o caixa de outro (7), o suprimento precisa ser o próprio logado.
        using var outro = await CenarioAsync(d => { d.FecharOutrosPeriodos = true; d.AdicionarUsuario(new DominioUsuario { IdDominio = 2, IdFilial = CenarioE2.Filial, IdUsuario = 6 }); },
            periodoLogado: 20, periodoFilho: 30);
        Msg(await outro.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(7, 6, [])))
            .Should().Be(string.Format(MensagensPeriodo.SuprimentoFechamentoLogado, CenarioE2.Usuario));
    }

    [Fact]
    public async Task VAL_E2_45_RegrasDoLogadoNoFechamento()
    {
        using var semPeriodo = await CenarioAsync(d => d.FecharPeriodo = true);
        Msg(await semPeriodo.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 9, [])))
            .Should().Be(string.Format(MensagensPeriodo.LogadoPeriodoFechado, CenarioE2.Usuario));

        using var semFlag = await CenarioAsync(periodoLogado: 20);
        Msg(await semFlag.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 9, [])))
            .Should().Be(string.Format(MensagensPeriodo.LogadoSemConfiguracaoFechamento, CenarioE2.Usuario));

        using var fora = await CenarioAsync(d => d.FecharPeriodo = true, periodoLogado: 20);
        Msg(await fora.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(7, 9, [])))
            .Should().Be(string.Format(MensagensPeriodo.CaixaNaoPodeSerFechado, 7, CenarioE2.Usuario));
    }

    [Fact]
    public async Task VAL_E2_46_47_SuprimentoDoFechamento()
    {
        using var c = await CenarioAsync(d => d.FecharPeriodo = true, periodoLogado: 20);
        var fechar = c.FecharPeriodo();

        Msg(await fechar.ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 0, []))).Should().Be(MensagensPeriodo.InformarUsuarioSuprimento);
        Msg(await fechar.ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 7, []))).Should().Be(MensagensPeriodo.SangriaNaoPermitida);
    }

    [Fact]
    public async Task VAL_E2_49_PeriodoDoLogadoFechado_Fechamento()
    {
        using var c = await CenarioAsync(d => d.FecharPeriodo = true, periodoLogado: 20);
        c.Periodos.Setup(x => x.PeriodoAbertoAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(PeriodoStatus.Fechado);

        Msg(await c.FecharPeriodo().ExecutarAsync(new FecharPeriodoCommand(CenarioE2.Usuario, 9, []))).Should().Be(MensagensPeriodo.PeriodoLogadoNaoAberto);
    }

    // ======================= Suprimento em dinheiro (OP-E2-08) ===================================

    [Fact]
    public async Task OP_E2_08_Suprimento_GravaLanctoESaldo()
    {
        using var c = await CenarioAsync(d => d.SaldoDinheiro = 300m, periodoLogado: 20, periodoFilho: 30);
        c.Periodos.Setup(x => x.ObterInfoPeriodoAsync(30, It.IsAny<CancellationToken>())).ReturnsAsync(new InfoPeriodoDto(PeriodoStatus.Aberto, 7, 3, "", ""));

        var r = await c.SuprimentoDinheiro().ExecutarAsync(new SuprimentoDinheiroCommand(7, 50m));

        r.IsSuccess.Should().BeTrue(r.IsSuccess ? "" : Msg(r));
        var lancto = await c.ReadContext.DominioPeriodoLactos.SingleAsync();
        (lancto.TipoLancto, lancto.IdPeriodoOrigem, lancto.IdPeriodoDestino, lancto.IdCaixaBanco, lancto.Valor)
            .Should().Be((LanctoDominioPeriodoTipo.Suprimento, 20, 30, 3, 50m));
        var dominios = await c.ReadContext.Dominios.ToDictionaryAsync(d => d.IdDominio);
        dominios[2].SaldoDinheiro.Should().Be(250m);
        dominios[3].SaldoDinheiro.Should().Be(50m);
        (await c.ReadContext.DominioPeriodos.SingleAsync(p => p.IdDominioPeriodo == 20)).IdUsuarioAlteracao.Should().Be(CenarioE2.Usuario);
    }

    [Fact]
    public async Task VAL_E2_16_SuprimentoSemUsuarioOuDestinoSemPeriodo_Falha()
    {
        using var c = await CenarioAsync(d => d.SaldoDinheiro = 300m, periodoLogado: 20, periodoFilho: 30);
        var suprimento = c.SuprimentoDinheiro();

        Msg(await suprimento.ExecutarAsync(new SuprimentoDinheiroCommand(0, 50m))).Should().Be(MensagensPeriodo.SuprimentoSemUsuario);
        Msg(await suprimento.ExecutarAsync(new SuprimentoDinheiroCommand(7, 50m))).Should().Be(string.Format(MensagensPeriodo.PeriodoNaoAberto, 3));
    }

    [Fact]
    public async Task VAL_E2_17_ValorZeroOuMaiorQueSaldo_Falha()
    {
        using var c = await CenarioAsync(d => d.SaldoDinheiro = 300m, periodoLogado: 20, periodoFilho: 30);
        c.Periodos.Setup(x => x.ObterInfoPeriodoAsync(30, It.IsAny<CancellationToken>())).ReturnsAsync(new InfoPeriodoDto(PeriodoStatus.Aberto, 7, 3, "", ""));
        var suprimento = c.SuprimentoDinheiro();

        Msg(await suprimento.ExecutarAsync(new SuprimentoDinheiroCommand(7, 0m))).Should().Be(MensagensPeriodo.SuprimentoValorZero);
        Msg(await suprimento.ExecutarAsync(new SuprimentoDinheiroCommand(7, 301m)))
            .Should().Be(string.Format(MensagensPeriodo.ValorMaiorQueSaldo, "301,00", "300,00"));
    }

    [Fact]
    public async Task VAL_E2_46_SuprimentoParaOProprioDominio_Falha()
    {
        using var c = await CenarioAsync(d => d.SaldoDinheiro = 300m, periodoLogado: 20);

        Msg(await c.SuprimentoDinheiro().ExecutarAsync(new SuprimentoDinheiroCommand(CenarioE2.Usuario, 50m)))
            .Should().Be(string.Format(MensagensPeriodo.SuprimentoNaoPermitido, CenarioE2.Usuario, CenarioE2.Usuario));
    }
}

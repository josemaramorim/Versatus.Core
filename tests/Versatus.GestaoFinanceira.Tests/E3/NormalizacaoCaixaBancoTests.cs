using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T07 — efeitos dos setters do legado aplicados no backend ao salvar (VAL-E3-22..26,
/// achado da auditoria do FCaixaBanco; decisão do usuário em 2026-09-27: tela + backend).
/// </summary>
public class NormalizacaoCaixaBancoTests
{
    // ---- VAL-E3-22 — CaixaBanco.cs:655 (setter TipoConta) ---------------------------------

    [Fact]
    public async Task VAL_E3_22_CaixaSemTipoDeCaixa_GravaNormal()
    {
        using var c = new CenarioE3();

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Caixa), null);

        r.Value!.TipoContaCaixa.Should().Be(TipoContaCaixa.Normal);
    }

    [Fact]
    public async Task VAL_E3_22_CaixaCofre_MantemCofre()
    {
        using var c = new CenarioE3();
        var caixa = CenarioE3.Caixa(0, ContaTipo.Caixa);
        caixa.TipoContaCaixa = TipoContaCaixa.Cofre;

        var r = await c.CaixaBancoService().CriarAsync(caixa, null);

        r.Value!.TipoContaCaixa.Should().Be(TipoContaCaixa.Cofre);
    }

    [Fact]
    public async Task VAL_E3_22_Banco_NaoTemTipoDeCaixa()
    {
        using var c = new CenarioE3();
        var caixa = CenarioE3.Caixa(0, ContaTipo.Banco);
        caixa.TipoContaCaixa = TipoContaCaixa.Normal;

        var r = await c.CaixaBancoService().CriarAsync(caixa, CenarioE3.Conta());

        r.Value!.TipoContaCaixa.Should().BeNull();
    }

    // ---- VAL-E3-23 — ContaBancaria.cs:432-439 (LimpaDadosTerceiro) -------------------------

    [Fact]
    public async Task VAL_E3_23_SemContaDeTerceiro_LimpaTitularECpfCnpj()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta();
        conta.Titular = "Fulano";
        conta.CpfCnpj = "12345678909";

        await c.ContaBancariaService().NormalizarAsync(conta, CenarioE3.Filial);

        conta.Titular.Should().BeNull();
        conta.CpfCnpj.Should().BeNull();
    }

    // ---- VAL-E3-24 — ContaBancaria.cs:1190-1195 (setter EnviarSped) -------------------------

    [Fact]
    public async Task VAL_E3_24_SemEnviarSped_LimpaInstituicaoFinanceira()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta();
        conta.IdInstituicaoFinanceira = 40;

        await c.ContaBancariaService().NormalizarAsync(conta, CenarioE3.Filial);

        conta.IdInstituicaoFinanceira.Should().BeNull();
    }

    // ---- VAL-E3-25 — ContaBancaria.cs:1347-1375 (setter ContaBancariaTipo) ------------------

    [Fact]
    public async Task VAL_E3_25_ContaCorrente_LimpaContaVinculada()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta();
        conta.IdContaBancariaVinculada = 2;

        await c.ContaBancariaService().NormalizarAsync(conta, CenarioE3.Filial);

        conta.IdContaBancariaVinculada.Should().BeNull();
    }

    [Fact]
    public async Task VAL_E3_25_InvestimentoSemVinculada_ZeraDadosBancarios()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta(tipo: TipoContaBancaria.Investimento);
        conta.DigitoConta = "9";
        conta.Limite = 500m;
        conta.PermiteEmitirCheque = true;
        conta.ContaTerceiro = true;

        await c.ContaBancariaService().NormalizarAsync(conta, CenarioE3.Filial);

        conta.IdAgencia.Should().Be(0);
        conta.NumeroConta.Should().BeEmpty();
        conta.DigitoConta.Should().BeNull();
        conta.Limite.Should().Be(0m);
        conta.PermiteEmitirCheque.Should().BeFalse();
        conta.ContaTerceiro.Should().BeFalse();
    }

    // ---- VAL-E3-26 — ContaBancaria.cs:444-470 (SetDadosBancoContaVinculada) -------------------

    [Fact]
    public async Task VAL_E3_26_InvestimentoComVinculada_CopiaDadosDaContaCorrente()
    {
        using var c = new CenarioE3();
        var corrente = CenarioE3.Conta(1);
        corrente.IdAgencia = 77;
        corrente.NumeroConta = "999";
        corrente.DigitoConta = "8";
        corrente.Limite = 1000m;
        corrente.ContaTerceiro = true;
        corrente.Titular = "Fulano";
        corrente.CpfCnpj = "12345678909";
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), corrente);

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco),
            new() { IdAgencia = 5, NumeroConta = "111", ContaBancariaTipo = TipoContaBancaria.Investimento, IdContaBancariaVinculada = 1 });

        r.IsSuccess.Should().BeTrue();
        var gravada = await c.ReadContext.ContasBancarias.SingleAsync(x => x.IdCaixaBanco == 100);
        gravada.IdAgencia.Should().Be(77);
        gravada.NumeroConta.Should().Be("999");
        gravada.DigitoConta.Should().Be("8");
        gravada.Limite.Should().Be(1000m);
        gravada.ContaTerceiro.Should().BeTrue();
        gravada.Titular.Should().Be("Fulano");
        gravada.CpfCnpj.Should().Be("12345678909");
        gravada.IdContaBancariaVinculada.Should().Be(1);
    }

    [Fact]
    public async Task VAL_E3_23_AplicadaTambemNoPutDaContaBancaria()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));
        var conta = CenarioE3.Conta(1);
        conta.Titular = "Fulano";

        var r = await c.ContaBancariaService().AtualizarAsync(conta);

        r.Value!.Titular.Should().BeNull();
    }
}

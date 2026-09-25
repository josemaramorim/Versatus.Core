using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T05 — ContaBancariaService (núcleo). VAL-E3-17 (matriz-rtv.md#E3) + OP-E3-05
/// (UpdateDadosContaVinculada — matriz-rot.md#E3).
/// </summary>
public class ContaBancariaServiceTests
{
    [Fact]
    public void VAL_E3_17_ContaDeTerceiroSemTitular_Falha()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta(1);
        conta.ContaTerceiro = true;
        conta.CpfCnpj = "12345678909";

        var r = c.ContaBancariaService().Validar(conta);

        r.Errors.Single().Mensagem.Should().Be(ContaBancariaService.MsgContaTerceiro);
    }

    [Fact]
    public async Task VAL_E3_17_AtualizarContaDeTerceiroSemCpfCnpj_Falha()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));
        var conta = CenarioE3.Conta(1);
        conta.ContaTerceiro = true;
        conta.Titular = "Fulano";

        var r = await c.ContaBancariaService().AtualizarAsync(conta);

        r.Errors.Single().Mensagem.Should().Be(ContaBancariaService.MsgContaTerceiro);
    }

    [Fact]
    public void VAL_E3_17_ContaDeTerceiroCompleta_Valida()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta(1);
        conta.ContaTerceiro = true;
        conta.Titular = "Fulano";
        conta.CpfCnpj = "12345678909";

        c.ContaBancariaService().Validar(conta).IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task OP_E3_05_Atualizar_PropagaDadosParaContasQueAVinculam()
    {
        using var c = new CenarioE3();
        var vinculada = CenarioE3.Conta(2, TipoContaBancaria.Investimento);
        vinculada.IdContaBancariaVinculada = 1;
        vinculada.EnviarSped = true;
        vinculada.IdInstituicaoFinanceira = 40;
        vinculada.Limite = 999m;
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Caixa(2, ContaTipo.Banco), CenarioE3.Conta(1), vinculada);

        var conta = CenarioE3.Conta(1);
        conta.IdAgencia = 77;
        conta.NumeroConta = "999";
        conta.DigitoConta = "";
        conta.Titular = "Fulano";
        conta.CpfCnpj = "12345678909";
        conta.ContaTerceiro = true;
        conta.Limite = 500m;

        var r = await c.ContaBancariaService().AtualizarAsync(conta);

        r.IsSuccess.Should().BeTrue();
        var lida = await c.ReadContext.ContasBancarias.SingleAsync(x => x.IdCaixaBanco == 2);
        lida.IdAgencia.Should().Be(77);
        lida.NumeroConta.Should().Be("999");
        lida.DigitoConta.Should().BeNull();
        lida.Titular.Should().Be("Fulano");
        lida.CpfCnpj.Should().Be("12345678909");
        lida.ContaTerceiro.Should().BeTrue();
        lida.Limite.Should().Be(500m);
        lida.EnviarSped.Should().BeFalse();
        lida.IdInstituicaoFinanceira.Should().BeNull();
        lida.ContaBancariaTipo.Should().Be(TipoContaBancaria.Investimento, "o legado não altera o tipo da conta vinculada");
    }

    [Fact]
    public async Task OP_E3_05_ContaVinculadaSemTerceiro_ZeraLimite()
    {
        using var c = new CenarioE3();
        var vinculada = CenarioE3.Conta(2, TipoContaBancaria.Investimento);
        vinculada.IdContaBancariaVinculada = 1;
        vinculada.Limite = 999m;
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Caixa(2, ContaTipo.Banco), CenarioE3.Conta(1), vinculada);

        var conta = CenarioE3.Conta(1);
        conta.Limite = 500m;

        (await c.ContaBancariaService().AtualizarAsync(conta)).IsSuccess.Should().BeTrue();

        (await c.ReadContext.ContasBancarias.SingleAsync(x => x.IdCaixaBanco == 2)).Limite.Should().Be(0m);
    }

    [Fact]
    public async Task OP_E3_05_Atualizar_RegistraAuditoriaDeAlteracao()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));

        var r = await c.ContaBancariaService().AtualizarAsync(CenarioE3.Conta(1));

        r.Value!.IdUsuarioAlteracao.Should().Be(CenarioE3.Usuario);
        r.Value.DataAlteracao.Should().Be(DateTime.Today);
    }
}

using FluentAssertions;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.GestaoFinanceira.Infrastructure.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T09 — OP-E3-08 (consulta de saldo por data/tipo, SaldoCaixaBanco.RetornarCaixaBanco) + CALC-E3-03,
/// integração InMemory com a filial do contexto.
/// </summary>
public class SaldoCalculadoraTests
{
    private static SaldoCaixaBanco Saldo(int dia, decimal anterior, decimal credito, decimal debito, int filial = CenarioE3.Filial) => new()
    {
        IdCaixaBanco = 1,
        IdFilial = filial,
        DataSaldo = new DateTime(2026, 9, dia),
        SaldoAnterior = anterior,
        TotalCredito = credito,
        TotalDebito = debito,
        SaldoAnteriorConciliado = anterior,
        TotalCreditoConciliado = 0m,
        TotalDebitoConciliado = 0m,
    };

    private static async Task<SaldoCalculadora> CalculadoraAsync(CenarioE3 c)
    {
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa), Saldo(10, 100m, 50m, 20m), Saldo(20, 130m, 10.005m, 0m),
            Saldo(25, 999m, 0m, 0m, filial: 99));
        return new SaldoCalculadora(new SaldoCaixaBancoRepository(c.ReadContext), c.Contexto.Object);
    }

    [Fact]
    public async Task OP_E3_08_Atual_PegaOUltimoSaldoAteAData()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        (await calc.RetornarSaldoCaixaBancoAsync(1, false, new DateTime(2026, 9, 20), TipoSaldo.Atual)).Should().Be(140.01m);
    }

    [Fact]
    public async Task OP_E3_08_Inicial_PegaOSaldoEstritamenteAnterior()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        (await calc.RetornarSaldoCaixaBancoAsync(1, false, new DateTime(2026, 9, 20), TipoSaldo.Inicial)).Should().Be(130m);
    }

    [Fact]
    public async Task OP_E3_08_InicialSemData_Zero()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        (await calc.RetornarSaldoCaixaBancoAsync(1, false, null, TipoSaldo.Inicial)).Should().Be(0m);
    }

    [Fact]
    public async Task OP_E3_08_FinalSemData_UltimoRegistroDaFilialDoContexto()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        // O registro do dia 25 é de outra filial e não entra.
        (await calc.RetornarSaldoCaixaBancoAsync(1, false, null, TipoSaldo.Final)).Should().Be(140.01m);
    }

    [Fact]
    public async Task OP_E3_08_SemRegistroAntesDaData_Zero()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        (await calc.RetornarSaldoCaixaBancoAsync(1, false, new DateTime(2026, 9, 1), TipoSaldo.Atual)).Should().Be(0m);
    }

    [Fact]
    public async Task OP_E3_08_SaldoConciliado()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        (await calc.RetornarSaldoCaixaBancoAsync(1, true, new DateTime(2026, 9, 15), TipoSaldo.Atual)).Should().Be(100m);
    }

    [Fact]
    public async Task ObterSaldo_DevolveDtoComSaldosCalculados()
    {
        using var c = new CenarioE3();
        var calc = await CalculadoraAsync(c);

        var dto = await calc.ObterSaldoAsync(1, CenarioE3.Filial, new DateTime(2026, 9, 15));

        dto!.DataSaldo.Should().Be(new DateTime(2026, 9, 10));
        dto.Saldo.Should().Be(130m);
        dto.SaldoConciliado.Should().Be(100m);
    }
}

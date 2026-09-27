using Versatus.Framework.Context;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

/// <summary>Saldos de caixa/banco e de rateio — CALC-E3-01..04 e consulta OP-E3-08.</summary>
public interface ISaldoCalculadora
{
    /// <summary>
    /// CALC-E3-03 / OP-E3-08 — SaldoCaixaBanco.RetornarSaldoCaixaBanco. <paramref name="dataSaldo"/>
    /// <c>null</c> equivale ao <c>DateTime.MinValue</c> do legado (sem data).
    /// </summary>
    Task<decimal> RetornarSaldoCaixaBancoAsync(int idCaixaBanco, bool saldoConciliado, DateTime? dataSaldo, TipoSaldo tipo,
        CancellationToken cancellationToken = default);

    /// <summary>SaldoCaixaBanco.Retornar — registro de saldo atual (<c>DataSaldo &lt;= data</c>) do caixa na filial.</summary>
    Task<SaldoCaixaBanco?> RetornarAsync(int idCaixaBanco, int idFilial, DateTime? dataSaldo, CancellationToken cancellationToken = default);

    /// <summary>Endpoint GET /saldo — <see cref="RetornarAsync"/> com os saldos calculados.</summary>
    Task<SaldoCaixaBancoDto?> ObterSaldoAsync(int idCaixaBanco, int idFilial, DateTime? dataSaldo, CancellationToken cancellationToken = default);
}

// Origem: SaldoCaixaBanco.cs (Saldo:269-275, SaldoConciliado:280-286, RetornarSaldoCaixaBanco:304-317)
// e SaldoRateio.cs (SaldoEconomico:280-286, SaldoFinanceiro:291-297) — legado.
// Fórmulas transcritas sem refatorar (Regra 5); paridade: E3/SaldoParityTests (golden origem=legado).
// Colunas nulas valem 0, como os campos double do objeto legado.
public class SaldoCalculadora(ISaldoCaixaBancoRepository repository, IContextoExecucao contexto) : ISaldoCalculadora
{
    /// <summary>CALC-E3-01/02 — <c>Arredondar(saldoAnterior + (totalCredito − totalDebito), 2)</c>.</summary>
    public static decimal CalcularSaldo(decimal? saldoAnterior, decimal? totalCredito, decimal? totalDebito)
        => ArredondamentoFinanceiro.Arredondar((saldoAnterior ?? 0m) + ((totalCredito ?? 0m) - (totalDebito ?? 0m)), 2);

    /// <summary>CALC-E3-01 — SaldoCaixaBanco.Saldo.</summary>
    public static decimal Saldo(SaldoCaixaBanco s) => CalcularSaldo(s.SaldoAnterior, s.TotalCredito, s.TotalDebito);

    /// <summary>CALC-E3-02 — SaldoCaixaBanco.SaldoConciliado.</summary>
    public static decimal SaldoConciliado(SaldoCaixaBanco s)
        => CalcularSaldo(s.SaldoAnteriorConciliado, s.TotalCreditoConciliado, s.TotalDebitoConciliado);

    /// <summary>CALC-E3-04 — <c>Arredondar(saldoAnterior + (totalCredito − totalDebito), 8)</c>.</summary>
    public static decimal CalcularSaldoRateio(decimal? saldoAnterior, decimal? totalCredito, decimal? totalDebito)
        => ArredondamentoFinanceiro.Arredondar((saldoAnterior ?? 0m) + ((totalCredito ?? 0m) - (totalDebito ?? 0m)), 8);

    /// <summary>CALC-E3-04 — SaldoRateio.SaldoEconomico.</summary>
    public static decimal SaldoEconomico(SaldoRateio s)
        => CalcularSaldoRateio(s.SaldoAnteriorEconomico, s.TotalCreditoEconomico, s.TotalDebitoEconomico);

    /// <summary>CALC-E3-04 — SaldoRateio.SaldoFinanceiro.</summary>
    public static decimal SaldoFinanceiro(SaldoRateio s)
        => CalcularSaldoRateio(s.SaldoAnteriorFinanceiro, s.TotalCreditoFinanceiro, s.TotalDebitoFinanceiro);

    /// <summary>
    /// CALC-E3-03 — parte pura de RetornarSaldoCaixaBanco: 0 se Inicial sem data ou sem registro;
    /// senão <c>Arredondar(saldo, 2)</c>.
    /// </summary>
    public static decimal CalcularRetornoSaldo(bool inicialSemData, decimal? saldo)
    {
        if (inicialSemData)
            return 0m;
        if (saldo is null)
            return 0m;
        return ArredondamentoFinanceiro.Arredondar(saldo.Value, 2);
    }

    public async Task<decimal> RetornarSaldoCaixaBancoAsync(int idCaixaBanco, bool saldoConciliado, DateTime? dataSaldo, TipoSaldo tipo,
        CancellationToken cancellationToken = default)
    {
        var inicialSemData = tipo == TipoSaldo.Inicial && dataSaldo is null;
        if (inicialSemData)
            return 0m;

        // OP-E3-08 — Inicial: DataSaldo < data; Atual/Final: DataSaldo <= data; filial do ambiente.
        var registro = await repository.ObterUltimoAsync(contexto.IdFilial, idCaixaBanco, dataSaldo,
            estritamenteAnterior: tipo == TipoSaldo.Inicial, cancellationToken);

        if (registro is null)
            return CalcularRetornoSaldo(false, null);

        return CalcularRetornoSaldo(false, saldoConciliado ? SaldoConciliado(registro) : Saldo(registro));
    }

    public Task<SaldoCaixaBanco?> RetornarAsync(int idCaixaBanco, int idFilial, DateTime? dataSaldo, CancellationToken cancellationToken = default)
        => repository.ObterUltimoAsync(idFilial, idCaixaBanco, dataSaldo, estritamenteAnterior: false, cancellationToken);

    public async Task<SaldoCaixaBancoDto?> ObterSaldoAsync(int idCaixaBanco, int idFilial, DateTime? dataSaldo,
        CancellationToken cancellationToken = default)
        => await RetornarAsync(idCaixaBanco, idFilial, dataSaldo, cancellationToken) is { } saldo ? BancosDtoMapper.ParaDto(saldo) : null;
}

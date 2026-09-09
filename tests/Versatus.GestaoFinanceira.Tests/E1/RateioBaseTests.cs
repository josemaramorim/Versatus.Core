using FluentAssertions;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Bases;
using Versatus.SharedKernel.Enums;
using Versatus.SharedKernel.Rateio;

namespace Versatus.GestaoFinanceira.Tests.E1;

/// <summary>
/// Parte pura de <see cref="RateioServiceBase"/> (matriz-rot.md#E1 OP-E1-05): sinal do
/// valor conforme a natureza. O motor de rateio completo é do MOD-02 (CLR-01), coberto no E5.
/// </summary>
public class RateioBaseTests
{
    private sealed class Servico : RateioServiceBase
    {
        public static decimal Natureza(decimal valor, NaturezaTipo n) => AplicarNatureza(valor, n);

        protected override Task<bool> RateioAplicavelAsync(CancellationToken ct) => throw new NotSupportedException();
        public override Task<ValidationRateioContainer> ValidarAsync(RateioContainer c, decimal v, CancellationToken ct) => throw new NotSupportedException();
        protected override Task<ValidationResult> ValidarParametrosTransferenciaFiliaisAsync(CancellationToken ct) => throw new NotSupportedException();
        protected override Task<ValidationResult> ValidarClasseItensFinanceirosAsync(int idOperacao, CancellationToken ct) => throw new NotSupportedException();
        public override Task AtualizarRateioAsync(RateioContainer c, CancellationToken ct) => throw new NotSupportedException();
        protected override Task PersistirRateioAsync(RateioContainer c, CancellationToken ct) => throw new NotSupportedException();
    }

    [Theory]
    [InlineData(100, NaturezaTipo.Credora, 100)]
    [InlineData(100, NaturezaTipo.Devedora, -100)]
    [InlineData(-50, NaturezaTipo.Devedora, 50)]
    public void OP_E1_05_AplicarNatureza(decimal valor, NaturezaTipo natureza, decimal esperado)
        => Servico.Natureza(valor, natureza).Should().Be(esperado);
}

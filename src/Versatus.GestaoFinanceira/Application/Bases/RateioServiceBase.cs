using Versatus.Framework.Validation;
using Versatus.SharedKernel.Enums;
using Versatus.SharedKernel.Rateio;

namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/framework/servidor.framework/ObjetoNegocio.cs (ValidarRateio/
// PersistirRateio — herdado) + OperacaoDocumentoBase.cs (AtualizarRateio/
// GeraRateioSelecionado/CarregarItemFinanceiroRateio).
// Cobre: OP-E1-03, OP-E1-05, OP-E1-06 · VAL-E1-08, VAL-E1-09 (matriz-*.md#E1).
//
// E1-T03 = esqueleto + contratos. O ACUMULADOR (`RateioContainer`) e o agregador de
// validação (`ValidationRateioContainer`) vêm do `Versatus.SharedKernel` (E0-T03). O
// MOTOR de rateio (regras de origem, contra-partida, persistência de `RateioMovtoItem`,
// arredondamento) é do MOD-02 (CLR-01) e é consumido no épico E5 via
// <see cref="PersistirRateioAsync"/> / <see cref="AtualizarRateioAsync"/>.
public abstract class RateioServiceBase
{
    /// <summary>Se o rateio se aplica (algum dos parâmetros UsaClasse/UsaCentroCusto/UsaProjeto ligado).</summary>
    protected abstract Task<bool> RateioAplicavelAsync(CancellationToken cancellationToken);

    /// <summary>
    /// OP-E1-03 — valida o rateio acumulado por dimensão (classe / centro de custo /
    /// projeto): a soma de cada dimensão deve igualar o valor rateável.
    /// </summary>
    public abstract Task<ValidationRateioContainer> ValidarAsync(RateioContainer container, decimal valorRateavel, CancellationToken cancellationToken);

    /// <summary>
    /// VAL-E1-08 — transferência entre filiais exige os parâmetros de classe/centro de
    /// custo/projeto de transferência configurados (conforme UsaClasse/UsaCentroCusto/UsaProjeto).
    /// </summary>
    protected abstract Task<ValidationResult> ValidarParametrosTransferenciaFiliaisAsync(CancellationToken cancellationToken);

    /// <summary>
    /// VAL-E1-09 — quando UsaClasse, todo item financeiro da seleção deve ter classe
    /// definida para a operação.
    /// </summary>
    protected abstract Task<ValidationResult> ValidarClasseItensFinanceirosAsync(int idOperacao, CancellationToken cancellationToken);

    /// <summary>
    /// OP-E1-05 — recálculo completo do rateio a partir da seleção: preenche o
    /// <see cref="RateioContainer"/> e aplica (delegado ao motor do MOD-02 no E5).
    /// </summary>
    public abstract Task AtualizarRateioAsync(RateioContainer container, CancellationToken cancellationToken);

    /// <summary>OP-E1-06 — gancho: a subclasse (Liquidação/Reversão) decide se gera rateio a partir dos itens financeiros.</summary>
    protected virtual bool GerarRateioItemFinanceiro() => false;

    /// <summary>Persistência do rateio (motor MOD-02, CLR-01) — no fluxo de <c>ExecutarPersistir</c> (OP-E1-01).</summary>
    protected abstract Task PersistirRateioAsync(RateioContainer container, CancellationToken cancellationToken);

    /// <summary>Sinal do valor conforme a natureza (devedora inverte).</summary>
    protected static decimal AplicarNatureza(decimal valor, NaturezaTipo natureza)
        => natureza == NaturezaTipo.Devedora ? -valor : valor;
}

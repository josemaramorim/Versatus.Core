using Versatus.Framework.Validation;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/objeto de negócio/gestao.financeira/OperacaoDocumentoBase.cs (legado,
// pai de Liquidacao (E6) e Reversao (E8)) + ObjectGenerated.ValidarOrigemGerador.
// Cobre: OP-E1-04, OP-E1-13 · VAL-E1-10 (matriz-*.md#E1).
//
// E1-T03 = esqueleto + contratos (CLR-04 / Artigo VII). 1 transação por Handler; só o
// Handler chama SaveChanges. A lógica concreta (o que a Liquidação/Reversão persistem, em
// que ordem) é dos épicos E6/E8.
public abstract class OperacaoDocumentoBaseHandler
{
    /// <summary>VAL-E1-10 — valida o tipo de operação vs. <c>PagarReceber</c> (serviço de Operação do AcessoGlobal, por <c>int</c>).</summary>
    protected abstract Task<ValidationResult> ValidarOperacaoAsync(int idOperacao, PagarReceberTipo pagarReceber, CancellationToken cancellationToken);

    /// <summary>OP-E1-04 — objetos gerados por processo externo validam a origem (<c>IdOrigem</c> + <c>ProcessoOrigem</c>) antes de persistir/excluir (RN-05-007).</summary>
    protected virtual Task<ValidationResult> ValidarOrigemGeradorAsync(int idOrigem, ProcessoOrigem processoOrigem, CancellationToken cancellationToken)
        => Task.FromResult(ValidationResult.Ok());

    /// <summary>OP-E1-13 — carrega operação/portador/históricos padrão e limpa os dados dependentes ao definir a ação/operação.</summary>
    protected abstract Task CarregarDefaultsAsync(PagarReceberTipo pagarReceber, CancellationToken cancellationToken);

    /// <summary>OP-E1-06 (gancho) — a subclasse decide se o rateio é gerado a partir dos itens financeiros da seleção.</summary>
    protected abstract bool GerarRateioItemFinanceiro();

    /// <summary>Limpa os dados da operação (equivalente a <c>LimparDados</c> do legado).</summary>
    protected abstract void LimparDados();
}

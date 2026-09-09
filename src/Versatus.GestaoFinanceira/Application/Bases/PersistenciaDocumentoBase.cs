using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bases;

namespace Versatus.GestaoFinanceira.Application.Bases;

// Origem: servidor/framework/servidor.framework/ObjetoNegocio.cs (ExecutarPersistir/
// ExecutarExcluir — herdado) + DocumentoFinanceiroBase.cs (validações/aplicações).
// Cobre: OP-E1-01, OP-E1-02, OP-E1-07..09, OP-E1-12 · VAL-E1-01..07 (matriz-*.md#E1).
//
// E1-T03 = esqueleto + contratos (CLR-04 / Artigo VII). A ordem de persistência do legado
// (Validate → OnBefore → sequencial → gravar → OnAfter → PersistirRateio →
// PersistirCampoEspecifico → PersistirPeriodoFormaPagto) é replicada por
// <see cref="PersistirAsync"/> abrindo UMA transação no handler; os passos concretos
// (gravar quais tabelas, quais filhos) são das subclasses de cada épico (E4 etc.).
// Sincronização e cache do legado ficam fora de escopo.
public abstract class PersistenciaDocumentoBase<TDocumento> where TDocumento : DocumentoFinanceiroBase
{
    // ---- Validações (VAL-E1-01..07) ----------------------------------------------------

    /// <summary>VAL-E1-01 — a operação deve ser compatível com <c>PagarReceber</c> (serviço de Operação do AcessoGlobal, por <c>int</c>).</summary>
    protected abstract Task<ValidationResult> ValidarOperacaoAsync(TDocumento documento, CancellationToken cancellationToken);

    /// <summary>VAL-E1-02..05 — entidade existe e tem o papel certo (Pagar→Fornecedor, Receber→Cliente, MovimentoCartao→Instituição financeira).</summary>
    protected abstract Task<ValidationResult> ValidarEntidadeAsync(TDocumento documento, CancellationToken cancellationToken);

    /// <summary>VAL-E1-06 — unicidade do número do documento (quando <c>ProcessoOrigem.Documento</c>, parâmetro ligado, tipo não sequencial).</summary>
    protected abstract Task<ValidationResult> ValidarNumeroDocumentoAsync(TDocumento documento, CancellationToken cancellationToken);

    /// <summary>VAL-E1-07 — regra do cadastro de Tipo de Documento (delegada ao AcessoGlobal por <c>int</c>).</summary>
    protected abstract Task<ValidationResult> ValidarTipoDocumentoAsync(TDocumento documento, CancellationToken cancellationToken);

    // ---- Aplicações (OP-E1-07..09) ---------------------------------------------------

    /// <summary>OP-E1-07 — aplica a operação: histórico/forma de cobrança/tipo de documento padrão; limpa o rateio.</summary>
    protected abstract Task<Result<TDocumento>> AplicarOperacaoAsync(TDocumento documento, int idOperacao, CancellationToken cancellationToken);

    /// <summary>OP-E1-08 — recalcula o valor convertido e (re)gera as parcelas pela condição de pagamento.</summary>
    protected abstract Task<Result<TDocumento>> AplicarCondicaoPagamentoAsync(TDocumento documento, bool refazer, CancellationToken cancellationToken);

    /// <summary>OP-E1-09 / CALC-E1-09 — <c>valor × índice → valorConvertido</c> (RN-05-008).</summary>
    protected abstract Task<decimal> CalcularValorConvertidoAsync(TDocumento documento, CancellationToken cancellationToken);

    // ---- Ganchos virtuais do legado (OP-E1-12) — implementados pelo épico dono --------

    protected virtual Task RecalcularTributoRateioAsync(TDocumento documento, CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task SetParcelasCondicaoPagtoAsync(TDocumento documento, bool refazer, CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task CarregarComissionadoPadraoAsync(TDocumento documento, CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task CarregaItemFinanceiroOperacaoAsync(TDocumento documento, CancellationToken cancellationToken) => Task.CompletedTask;

    // ---- Orquestração (OP-E1-01 / OP-E1-02) ------------------------------------------

    /// <summary>
    /// OP-E1-01 — persiste o documento em UMA transação (aberta no handler), seguindo a
    /// ordem do legado: validar → gerar sequencial se novo (RN-05-006) → gravar →
    /// persistir rateio → persistir dados dependentes. <see cref="GravarAsync"/> é o passo
    /// concreto do épico.
    /// </summary>
    public abstract Task<Result<TDocumento>> PersistirAsync(TDocumento documento, CancellationToken cancellationToken);

    /// <summary>OP-E1-02 — exclui o documento em UMA transação: excluir rateio → excluir dependentes → remover.</summary>
    public abstract Task<Result<TDocumento>> ExcluirAsync(TDocumento documento, CancellationToken cancellationToken);

    /// <summary>Passo concreto de gravação (INSERT/UPDATE da(s) tabela(s) do agregado) — definido pelo épico.</summary>
    protected abstract Task GravarAsync(TDocumento documento, CancellationToken cancellationToken);
}

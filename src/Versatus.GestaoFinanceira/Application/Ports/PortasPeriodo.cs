using Versatus.Framework.Validation;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Application.Ports;

// Portas das operações de período do E2 (decisão D4 de 2026-10-05 — analysis/E2-dominio.md §4.2).
// O que é de outro épico (E5 movimento financeiro, E9 cheques) entra por porta e tem, até lá, uma
// implementação "pendente" que recusa a operação com mensagem explícita; o que é do MOD-02 tem
// adapter SQL (Artigo VIII).

/// <summary>
/// Movimento financeiro gerado pelo lançamento em dinheiro do período (DominioPeriodoLacto.cs:158-190):
/// sangria no caixa de origem e suprimento no caixa de destino. Natureza e histórico vêm da operação.
/// Implementação real: épico E5.
/// </summary>
public interface IGeradorMovimentoPeriodo
{
    Task<ValidationResult> GerarAsync(MovimentoPeriodo movimento, CancellationToken cancellationToken = default);
}

/// <param name="IdCaixaBanco">Caixa do domínio (origem para a sangria, destino para o suprimento).</param>
/// <param name="IdOperacao">OperacaoPadraoSangria ou OperacaoPadraoSuprimento.</param>
/// <param name="Valor">|Valor| do lançamento.</param>
/// <param name="IdDominioPeriodo">Período de origem (sangria) ou de destino (suprimento).</param>
/// <param name="IdOrigem">IdDominioPeriodoLacto — origem geradora (ProcessoOrigem.DominioPeriodoLancto).</param>
public sealed record MovimentoPeriodo(int IdCaixaBanco, int IdOperacao, decimal Valor, int? IdDominioPeriodo, DateTime DataMovimento,
    int IdOrigem, ProcessoOrigem IdProcessoOrigem = ProcessoOrigem.DominioPeriodoLancto);

/// <summary>
/// Talões e cheques recebidos que acompanham o período (FINTALAOCHEQUE / FINCHEQUERECEBIDO.IDFINDOMINIOPERIODO).
/// Implementação real: épico E9.
/// </summary>
public interface IChequePeriodoPorta
{
    /// <summary>Folhas de talão do período — DominioPeriodo.RetornarChequesFechamentoPeriodo(ITalaoCheque).</summary>
    Task<Result<IReadOnlyList<ChequePeriodo>>> ListarFolhasTalaoDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>Cheques recebidos do período — DominioPeriodo.RetornarChequesFechamentoPeriodo(IChequeRecebido).</summary>
    Task<Result<IReadOnlyList<ChequePeriodo>>> ListarChequesRecebidosDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>Cheques recebidos Aberto/Devolvido/DevolucaoRepasse da filial — DominioPeriodo.RetornarListaFinanceiro.</summary>
    Task<Result<IReadOnlyList<ChequePeriodo>>> ListarChequesRecebidosEmAbertoAsync(CancellationToken cancellationToken = default);

    /// <summary>UPDATE FINTALAOCHEQUE SET IDFINDOMINIOPERIODO para todos os talões da filial (domínio padrão).</summary>
    Task<ValidationResult> AtribuirPeriodoATodosTaloesAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>TalaoCheque.IdDominioPeriodo = período (DominioPeriodoLacto.AtualizarTalaoCheque).</summary>
    Task<ValidationResult> AtribuirPeriodoAFolhaTalaoAsync(int idCaixaBanco, int cheque, int? idDominioPeriodo, CancellationToken cancellationToken = default);

    /// <summary>DominioUtil.UpdateDominioPeriodoChequeRececebido.</summary>
    Task<ValidationResult> AtribuirPeriodoAChequeRecebidoAsync(int idChequeRecebido, int? idDominioPeriodo, CancellationToken cancellationToken = default);
}

/// <param name="IdChequeRecebido">Cheque recebido (cheque de cliente); null em folha de talão.</param>
/// <param name="IdCaixaBanco">Caixa do talão (cheque de empresa); null em cheque recebido.</param>
/// <param name="Cheque">Número da folha (cheque de empresa).</param>
public sealed record ChequePeriodo(int? IdChequeRecebido, int? IdCaixaBanco, int? Cheque, decimal Valor);

/// <summary>Operação padrão de caixa (GLOOPERACAO + GLOOPERACAOCAIXA) — MOD-02, só leitura.</summary>
public interface IOperacaoCaixaConsulta
{
    Task<OperacaoCaixaInfo?> ObterAsync(int idOperacao, CancellationToken cancellationToken = default);
}

/// <param name="IdTipoOperacao">GLOOPERACAO.IDGLOTIPOOPERACAO (OperacaoTipo.Caixa = 1).</param>
/// <param name="IdNatureza">GLOOPERACAOCAIXA.IDNATUREZA; null quando a operação não tem registro de caixa.</param>
public sealed record OperacaoCaixaInfo(int IdOperacao, int IdTipoOperacao, NaturezaTipo? IdNatureza, bool LancaMovtoEntidade);

/// <summary>
/// Escrita no MOD-02 feita pela abertura/fechamento do domínio padrão
/// (DominioPeriodo.PersistirConfiguracoesDominioPadrao — DominioPeriodo.cs:467-501).
/// </summary>
public interface IParametroFinanceiroEscrita
{
    /// <summary>Grava o valor do parâmetro no escopo da filial do contexto (cria a linha se não existir).</summary>
    Task GravarAsync(string nomeParametro, string valor, CancellationToken cancellationToken = default);

    /// <summary>UPDATE GLOPERFIL SET USADOMINIOFINANCEIRO = 1.</summary>
    Task MarcarPerfilUsaDominioAsync(int idPerfil, CancellationToken cancellationToken = default);
}

/// <summary>Resumo por forma de pagamento do período (view VWRESUMODOMINIOPERIODOFORMAPAGTO).</summary>
public interface IResumoFormaPagtoConsulta
{
    Task<IReadOnlyList<ResumoFormaPagto>> ListarAsync(int idDominioPeriodo, CancellationToken cancellationToken = default);
}

public sealed record ResumoFormaPagto(int IdDominioPeriodo, string Descricao, FormaPagtoTipo TipoFormaPagto, decimal? Credito, decimal? Debito);

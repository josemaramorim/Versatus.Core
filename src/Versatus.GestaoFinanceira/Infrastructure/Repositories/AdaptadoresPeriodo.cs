using Microsoft.EntityFrameworkCore;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Application.Ports;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Infrastructure.Repositories;

// Adapters das portas de período do E2 (Application/Ports/PortasPeriodo.cs).
// Colunas MOD-02 conferidas via INFORMATION_SCHEMA em 2026-10-21: GLOOPERACAO (IDGLOOPERACAO,
// IDGLOTIPOOPERACAO), GLOOPERACAOCAIXA (IDGLOOPERACAO, IDNATUREZA, LANCAMOVTOENTIDADE),
// GLOPARAMETRO (IDGLOPARAMETRO, NOME), GLOPARAMETROVALOR (IDGLOPARAMETROVALOR, IDGLOPARAMETRO,
// IDGLOGRUPO, IDGLOEMPRESA, IDGLOFILIAL, IDGLOPERFIL, VALOR), GLOEMPRESA (IDGLOGRUPO),
// GLOPERFIL (USADOMINIOFINANCEIRO).

/// <summary>Operação de caixa (MOD-02) — só leitura.</summary>
public class OperacaoCaixaConsulta(GestaoFinanceiraReadDbContext readContext) : IOperacaoCaixaConsulta
{
    public async Task<OperacaoCaixaInfo?> ObterAsync(int idOperacao, CancellationToken cancellationToken = default)
    {
        var linhas = await readContext.Database.SqlQuery<OperacaoLinha>($"""
            SELECT O.IDGLOOPERACAO AS IdOperacao, O.IDGLOTIPOOPERACAO AS IdTipoOperacao,
                   C.IDNATUREZA AS IdNatureza, ISNULL(C.LANCAMOVTOENTIDADE, 0) AS LancaMovtoEntidade
            FROM GLOOPERACAO O
            LEFT JOIN GLOOPERACAOCAIXA C ON C.IDGLOOPERACAO = O.IDGLOOPERACAO
            WHERE O.IDGLOOPERACAO = {idOperacao}
            """).ToListAsync(cancellationToken);

        if (linhas.Count == 0)
            return null;

        var l = linhas[0];
        return new OperacaoCaixaInfo(l.IdOperacao, l.IdTipoOperacao ?? 0, (NaturezaTipo?)l.IdNatureza, l.LancaMovtoEntidade != 0);
    }

    private sealed record OperacaoLinha(int IdOperacao, int? IdTipoOperacao, int? IdNatureza, short LancaMovtoEntidade);
}

/// <summary>
/// Escrita de parâmetro e perfil (MOD-02) na mesma transação do handler (contexto de escrita).
/// O valor é gravado no escopo Filial do contexto — os 4 parâmetros do domínio padrão são de Filial
/// (IDTIPOPARAMETRO = 160) e o legado grava o ParametroValor resolvido para o ambiente (que é criado
/// quando não existe — ParametroServidor.RetornarParametroValorDefault).
/// </summary>
public class ParametroFinanceiroEscrita(GestaoFinanceiraDbContext context, IGeradorSequencial geradorSequencial,
    IContextoExecucao contexto) : IParametroFinanceiroEscrita
{
    // [AutoSequencial("IdParametroValor")] — sequencial geral (GLOSEQUENCIAL.IDTIPO = 230).
    public const string NomeSequencialParametroValor = "ParametroValor";

    public async Task GravarAsync(string nomeParametro, string valor, CancellationToken cancellationToken = default)
    {
        var empresa = contexto.IdEmpresa;
        var filial = contexto.IdFilial;

        var alteradas = await context.Database.ExecuteSqlAsync($"""
            UPDATE V SET V.VALOR = {valor}
            FROM GLOPARAMETROVALOR V
            INNER JOIN GLOPARAMETRO P ON P.IDGLOPARAMETRO = V.IDGLOPARAMETRO
            INNER JOIN GLOEMPRESA E ON E.IDGLOEMPRESA = {empresa}
            WHERE P.NOME = {nomeParametro} AND V.IDGLOGRUPO = E.IDGLOGRUPO AND V.IDGLOEMPRESA = {empresa}
              AND V.IDGLOFILIAL = {filial} AND V.IDGLOPERFIL IS NULL
            """, cancellationToken);

        if (alteradas > 0)
            return;

        var id = await geradorSequencial.ProximoAsync(NomeSequencialParametroValor, SequencialTipo.Geral, cancellationToken);
        await context.Database.ExecuteSqlAsync($"""
            INSERT INTO GLOPARAMETROVALOR (IDGLOPARAMETROVALOR, IDGLOPARAMETRO, IDGLOGRUPO, IDGLOEMPRESA, IDGLOFILIAL, IDGLOPERFIL, VALOR)
            SELECT {id}, P.IDGLOPARAMETRO, E.IDGLOGRUPO, {empresa}, {filial}, NULL, {valor}
            FROM GLOPARAMETRO P, GLOEMPRESA E
            WHERE P.NOME = {nomeParametro} AND E.IDGLOEMPRESA = {empresa}
            """, cancellationToken);
    }

    public Task MarcarPerfilUsaDominioAsync(int idPerfil, CancellationToken cancellationToken = default)
        => context.Database.ExecuteSqlAsync($"UPDATE GLOPERFIL SET USADOMINIOFINANCEIRO = 1 WHERE IDGLOPERFIL = {idPerfil}", cancellationToken);
}

/// <summary>Resumo por forma de pagamento — view VWRESUMODOMINIOPERIODOFORMAPAGTO (só leitura).</summary>
public class ResumoFormaPagtoConsulta(GestaoFinanceiraReadDbContext readContext, IContextoExecucao contexto) : IResumoFormaPagtoConsulta
{
    public async Task<IReadOnlyList<ResumoFormaPagto>> ListarAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
        => await readContext.ResumosDominioPeriodoFormaPagto
            .Where(r => r.IdFilial == contexto.IdFilial && r.IdDominioPeriodo == idDominioPeriodo)
            .Select(r => new ResumoFormaPagto(r.IdDominioPeriodo, r.Descricao, r.TipoFormaPagto, r.Credito, r.Debito))
            .ToListAsync(cancellationToken);
}

/// <summary>Até o épico E5: o movimento financeiro do período ainda não existe — a operação é recusada.</summary>
public class GeradorMovimentoPeriodoPendente : IGeradorMovimentoPeriodo
{
    public const string MsgPendente = "O movimento financeiro do lançamento do domínio período depende do épico E5 (ainda não migrado).";

    public Task<ValidationResult> GerarAsync(MovimentoPeriodo movimento, CancellationToken cancellationToken = default)
        => Task.FromResult(ValidationResult.Fail(new ValidationError("MovimentoFinanceiro", MsgPendente)));
}

/// <summary>Até o épico E9: talões e cheques recebidos ainda não existem — a operação é recusada.</summary>
public class ChequePeriodoPendente : IChequePeriodoPorta
{
    public const string MsgPendente = "Os cheques do domínio período dependem do épico E9 (ainda não migrado).";

    private static Result<IReadOnlyList<ChequePeriodo>> Lista() => Result<IReadOnlyList<ChequePeriodo>>.Fail(new ValidationError("Cheque", MsgPendente));
    private static ValidationResult Falha() => ValidationResult.Fail(new ValidationError("Cheque", MsgPendente));

    public Task<Result<IReadOnlyList<ChequePeriodo>>> ListarFolhasTalaoDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
        => Task.FromResult(Lista());

    public Task<Result<IReadOnlyList<ChequePeriodo>>> ListarChequesRecebidosDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
        => Task.FromResult(Lista());

    public Task<Result<IReadOnlyList<ChequePeriodo>>> ListarChequesRecebidosEmAbertoAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Lista());

    public Task<ValidationResult> AtribuirPeriodoATodosTaloesAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
        => Task.FromResult(Falha());

    public Task<ValidationResult> AtribuirPeriodoAFolhaTalaoAsync(int idCaixaBanco, int cheque, int? idDominioPeriodo, CancellationToken cancellationToken = default)
        => Task.FromResult(Falha());

    public Task<ValidationResult> AtribuirPeriodoAChequeRecebidoAsync(int idChequeRecebido, int? idDominioPeriodo, CancellationToken cancellationToken = default)
        => Task.FromResult(Falha());
}

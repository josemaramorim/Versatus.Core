namespace Versatus.GestaoFinanceira.Domain.Repositories;

/// <summary>
/// Porta de leitura do domínio financeiro (FINDOMINIO / FINDOMINIOPERIODO / FINDOMINIOUSUARIO),
/// usada pelas regras do E3 que dependem do período (VAL-E3-08 / VAL-E3-10). A implementação
/// atual é um adapter SQL só-leitura temporário, substituído pelos repositórios do épico E2.
/// </summary>
public interface IDominioFinanceiroConsulta
{
    /// <summary>
    /// Períodos dos domínios ativos do caixa (<c>IdCaixaBanco</c>, <c>IdFilial</c>) que têm
    /// período corrente (<c>IdDominioPeriodo</c> não nulo) — CaixaBanco.cs:302-319.
    /// </summary>
    Task<IReadOnlyList<DominioPeriodoInfo>> ListarPeriodosDosDominiosDoCaixaAsync(int idCaixaBanco, int idFilial,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// <c>MovimentoBanco</c> do domínio do usuário (1º por IdDominio — DominioUsuario.GetDominio);
    /// <c>null</c> quando o usuário não tem domínio.
    /// </summary>
    Task<bool?> ObterMovimentoBancoDoDominioDoUsuarioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>O usuário consta em FINDOMINIOUSUARIO na filial — CaixaBanco.ValidarUsuarioCaixaDominio(0).</summary>
    Task<bool> UsuarioPossuiDominioAsync(int idUsuario, int idFilial, CancellationToken cancellationToken = default);

    /// <summary>O caixa consta em FINDOMINIO na filial — CaixaBanco.ValidarUsuarioCaixaDominio(idCaixa).</summary>
    Task<bool> CaixaPossuiDominioAsync(int idCaixaBanco, int idFilial, CancellationToken cancellationToken = default);
}

/// <param name="DataFechamento">FINDOMINIOPERIODO.DATAFECHAMENTO (nulo = período aberto).</param>
public sealed record DominioPeriodoInfo(int IdDominio, DateTime? DataFechamento);

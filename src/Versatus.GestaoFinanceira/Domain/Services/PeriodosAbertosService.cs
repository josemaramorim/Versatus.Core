using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Domain.Services;

// Origem: servidor/objeto de negócio/gestao.financeira/PeriodosAbertos.cs (todo) e
// Dominio.cs:EditarCaixaBancoDominioPeriodo (1223-1246).
// Cobre: OP-E2-13, OP-E2-14, VAL-E2-42 (matriz-rot.md#E2 / matriz-rtv.md#E2).
//
// O legado mantinha a lista de períodos em cache estático (otimização do Remoting) e a
// recarregava em vários pontos; aqui a lista é montada a cada consulta (decisão de OP-E2-13).
// O efeito colateral de PeriodosAbertos.cs:275-276 (alterava a filial do ambiente do chamador
// dentro do laço — Q9) não é reproduzido.
public class PeriodosAbertosService(
    IDominioRepository repository,
    IPerfilDominioConsulta perfilDominioConsulta,
    IParametroRepository parametroRepository,
    IContextoExecucao contexto) : IPeriodosAbertosService
{
    public const string ParametroTrabalhaComDominio = "TRABALHACOMDOMINIO";
    public const string ParametroDominioPadrao = "DOMINIOPADRAO";
    public const string ParametroControleCaixaTesouraria = "CONTROLECAIXATESOURARIA";

    public const string MsgPeriodoNaoEncontrado = "NÃO foi encontrado período para o usuário logado."; // VAL-E2-42

    /// <summary>Linha da lista de períodos (PeriodosAbertos.PeriodoDesc).</summary>
    private sealed record PeriodoDesc(int IdUsuario, int IdPeriodo, int IdCaixaBanco, int IdDominio, string NomeUsuario,
        string DescricaoCaixaBanco);

    public async Task<PeriodoStatus> PeriodoAbertoAsync(int? idUsuario = null, CancellationToken cancellationToken = default)
    {
        // PeriodosAbertos.cs:387-406.
        if (!await ParametroLigadoAsync(ParametroTrabalhaComDominio, cancellationToken))
            return PeriodoStatus.NaoAplicavel;

        var periodos = await CarregarPeriodosAsync(cancellationToken);
        if (periodos.Count == 0)
            return PeriodoStatus.PerfilSemPermissao;

        var usuario = idUsuario ?? contexto.IdUsuario;
        var linha = periodos.FirstOrDefault(p => p.IdUsuario == usuario);
        if (linha is null)
            return PeriodoStatus.UsuarioSemPermissao;

        return linha.IdPeriodo == 0 ? PeriodoStatus.Fechado : PeriodoStatus.Aberto;
    }

    public Task<PeriodoStatus> ValidarPeriodoAbertoAsync(CancellationToken cancellationToken = default)
        => PeriodoAbertoAsync(null, cancellationToken);

    public async Task<Result<int?>> RetornarIdPeriodoAsync(CancellationToken cancellationToken = default)
    {
        // PeriodosAbertos.cs:328-359.
        var status = await PeriodoAbertoAsync(null, cancellationToken);
        if (status != PeriodoStatus.Aberto)
            return Result<int?>.Ok(null);

        var periodos = await CarregarPeriodosAsync(cancellationToken);
        var idPeriodo = periodos.FirstOrDefault(p => p.IdUsuario == contexto.IdUsuario)?.IdPeriodo ?? 0;
        if (idPeriodo != 0)
            return Result<int?>.Ok(idPeriodo);

        return Result<int?>.Fail(new ValidationError("IdDominioPeriodo", MsgPeriodoNaoEncontrado));
    }

    public async Task<InfoPeriodoDto> ObterInfoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
    {
        // PeriodosAbertos.cs:420-436 — não consulta o parâmetro TrabalhaComDominio.
        if (idDominioPeriodo == 0)
            return new InfoPeriodoDto(PeriodoStatus.NaoAplicavel, 0, 0, "", "");

        var periodos = await CarregarPeriodosAsync(cancellationToken);
        var linha = periodos.FirstOrDefault(p => p.IdPeriodo == idDominioPeriodo);

        return linha is null
            ? new InfoPeriodoDto(PeriodoStatus.Fechado, 0, 0, "", "")
            : new InfoPeriodoDto(PeriodoStatus.Aberto, linha.IdUsuario, linha.IdCaixaBanco, linha.NomeUsuario, linha.DescricaoCaixaBanco);
    }

    public async Task<int> RetornarIdCaixaBancoAsync(CancellationToken cancellationToken = default)
        => (await CarregarPeriodosAsync(cancellationToken)).FirstOrDefault(p => p.IdUsuario == contexto.IdUsuario)?.IdCaixaBanco ?? 0;

    public async Task<int> RetornarIdCaixaBancoDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
        => (await CarregarPeriodosAsync(cancellationToken)).FirstOrDefault(p => p.IdPeriodo == idDominioPeriodo)?.IdCaixaBanco ?? 0;

    public async Task<int> RetornarIdUsuarioDoPeriodoAsync(int idDominioPeriodo, CancellationToken cancellationToken = default)
        => (await CarregarPeriodosAsync(cancellationToken)).FirstOrDefault(p => p.IdPeriodo == idDominioPeriodo)?.IdUsuario ?? 0;

    public async Task<int?> RetornarIdPeriodoPadraoAsync(CancellationToken cancellationToken = default)
    {
        // PeriodosAbertos.cs:481-498 — devolve o IdPeriodo da linha do padrão mesmo quando é 0.
        var idDominioPadrao = await ParametroIntAsync(ParametroDominioPadrao, cancellationToken);
        var linha = (await CarregarPeriodosAsync(cancellationToken)).FirstOrDefault(p => p.IdDominio == idDominioPadrao);
        return linha?.IdPeriodo;
    }

    public async Task<IReadOnlyList<int>> ListarCaixasComPeriodoAbertoAsync(CancellationToken cancellationToken = default)
    {
        // PeriodosAbertos.cs:503-527.
        var periodos = (await CarregarPeriodosAsync(cancellationToken))
            .Where(p => p.IdCaixaBanco != 0 && p.IdPeriodo != 0)
            .ToList();
        if (periodos.Count == 0)
            return [];

        var dados = (await repository.ListarPeriodosAsync([.. periodos.Select(p => p.IdPeriodo).Distinct()], contexto.IdFilial, cancellationToken))
            .ToDictionary(p => p.IdDominioPeriodo);

        return [.. periodos
            .Where(p => dados.TryGetValue(p.IdPeriodo, out var dp) && dp.DataAbertura != DateTime.MinValue && dp.DataFechamento is null)
            .Select(p => p.IdCaixaBanco)];
    }

    public async Task<bool> PodeEditarCaixaBancoAsync(bool validarBanco, CancellationToken cancellationToken = default)
    {
        // Dominio.cs:1223-1246.
        if (await ValidarPeriodoAbertoAsync(cancellationToken) != PeriodoStatus.Aberto)
            return true;

        var dominio = await repository.ObterDominioDoUsuarioAsync(contexto.IdUsuario, contexto.IdFilial, cancellationToken);
        if (dominio is null)
            return false;

        if (validarBanco && dominio.MovimentoBanco)
            return true;

        if (await ParametroIntAsync(ParametroDominioPadrao, cancellationToken) == dominio.IdDominio)
            return true;

        return dominio.Tesouraria && await ParametroLigadoAsync(ParametroControleCaixaTesouraria, cancellationToken);
    }

    // PeriodosAbertos.cs:201-319 (RetornarPeriodos / AddPeriodos / CarregarInfoDominio).
    private async Task<IReadOnlyList<PeriodoDesc>> CarregarPeriodosAsync(CancellationToken cancellationToken)
    {
        // Só a filial do contexto, e só se ela trabalha com domínio.
        if (!await ParametroLigadoAsync(ParametroTrabalhaComDominio, cancellationToken))
            return [];

        var usuarios = await perfilDominioConsulta.ListarUsuariosDosPerfisComDominioAsync(contexto.IdEmpresa, contexto.IdUsuario,
            cancellationToken);
        if (usuarios.Count == 0)
            return [];

        var dominios = (await repository.ListarDominiosDosUsuariosAsync([.. usuarios.Select(u => u.IdUsuario)], contexto.IdFilial,
            cancellationToken)).ToDictionary(d => d.IdUsuario);

        var periodos = new List<PeriodoDesc>();
        foreach (var usuario in usuarios)
        {
            if (!dominios.TryGetValue(usuario.IdUsuario, out var d))
            {
                periodos.Add(new PeriodoDesc(usuario.IdUsuario, 0, 0, 0, usuario.Nome, ""));
                continue;
            }

            // O caixa entra mesmo com o domínio inativo ou sem período; o domínio e o período, não.
            var comPeriodo = d.Ativo && d.IdDominioPeriodo is not null;
            var idDominio = comPeriodo ? d.IdDominio : 0;
            var idPeriodo = comPeriodo && d.DataFechamento is null ? d.IdDominioPeriodo!.Value : 0;

            periodos.Add(new PeriodoDesc(usuario.IdUsuario, idPeriodo, d.IdCaixaBanco, idDominio, usuario.Nome, d.DescricaoCaixaBanco ?? ""));
        }

        return periodos;
    }

    private async Task<bool> ParametroLigadoAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return bool.TryParse(valor, out var ligado) && ligado;
    }

    private async Task<int> ParametroIntAsync(string nome, CancellationToken cancellationToken)
    {
        var valor = await parametroRepository.GetParametroValorAsync(nome, contexto.IdEmpresa, contexto.IdFilial, cancellationToken);
        return int.TryParse(valor, out var numero) ? numero : 0;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.Framework.Validation;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.GestaoFinanceira.Infrastructure;
using Versatus.GestaoFinanceira.Infrastructure.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E2;

/// <summary>
/// Banco InMemory compartilhado entre o contexto de escrita e o de leitura (CQRS leve), com as
/// portas cross-módulo (perfil, parâmetros) e o controle de caixa/banco do E3 mockados.
/// </summary>
internal sealed class CenarioE2 : IDisposable
{
    public const int Empresa = 1;
    public const int Filial = 10;
    public const int Usuario = 5;

    private readonly string _banco = Guid.NewGuid().ToString();
    private readonly InMemoryDatabaseRoot _raiz = new();
    private readonly DbContextOptions<GestaoFinanceiraDbContext> _opcoesEscrita;

    public GestaoFinanceiraDbContext Context { get; }
    public GestaoFinanceiraReadDbContext ReadContext { get; }
    public Mock<IParametroRepository> Parametros { get; } = new();
    public Mock<IPerfilDominioConsulta> Perfis { get; } = new();
    public Mock<IGeradorSequencial> Sequencial { get; } = new();
    public Mock<IContextoExecucao> Contexto { get; } = new();
    public Mock<ICaixaBancoService> ControleCaixaBanco { get; } = new();
    public Mock<IPeriodosAbertosService> Periodos { get; } = new();

    public CenarioE2()
    {
        _opcoesEscrita = new DbContextOptionsBuilder<GestaoFinanceiraDbContext>()
            .UseInMemoryDatabase(_banco, _raiz)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        Context = new GestaoFinanceiraDbContext(_opcoesEscrita);
        ReadContext = new GestaoFinanceiraReadDbContext(new DbContextOptionsBuilder<GestaoFinanceiraReadDbContext>()
            .UseInMemoryDatabase(_banco, _raiz)
            .Options);

        Contexto.SetupGet(x => x.IdEmpresa).Returns(Empresa);
        Contexto.SetupGet(x => x.IdFilial).Returns(Filial);
        Contexto.SetupGet(x => x.IdUsuario).Returns(Usuario);
        Sequencial.Setup(x => x.ProximoAsync(It.IsAny<string>(), SequencialTipo.Filial, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        // Por padrão: controle de caixa/banco OK, períodos fechados e todo usuário com perfil de domínio.
        ControleCaixaBanco.Setup(x => x.ValidarControleCaixaBancoAsync(It.IsAny<PeriodoStatus>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ValidationResult.Ok());
        Periodos.Setup(x => x.PeriodoAbertoAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>())).ReturnsAsync(PeriodoStatus.NaoAplicavel);
        Periodos.Setup(x => x.ObterInfoPeriodoAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InfoPeriodoDto(PeriodoStatus.Fechado, 0, 0, "", ""));
        Perfis.Setup(x => x.ObterPerfilUsuarioAsync(It.IsAny<int>(), Empresa, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, int _, CancellationToken _) => new PerfilUsuarioDominioInfo(id, $"Usuário {id}", true, true));
    }

    public void Parametro(string nome, string? valor)
        => Parametros.Setup(x => x.GetParametroValorAsync(nome, Empresa, Filial, It.IsAny<CancellationToken>())).ReturnsAsync(valor);

    public void Perfil(int idUsuario, bool possuiPerfil, bool usaDominio)
        => Perfis.Setup(x => x.ObterPerfilUsuarioAsync(idUsuario, Empresa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PerfilUsuarioDominioInfo(idUsuario, $"Usuário {idUsuario}", possuiPerfil, usaDominio));

    public void PeriodoAberto(int idPeriodo)
        => Periodos.Setup(x => x.ObterInfoPeriodoAsync(idPeriodo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InfoPeriodoDto(PeriodoStatus.Aberto, Usuario, 1, "", ""));

    public DominioRepository Repositorio() => new(Context, ReadContext);

    public DominioService DominioService()
        => new(Repositorio(), new CaixaBancoRepository(Context, ReadContext), ControleCaixaBanco.Object, Periodos.Object, Perfis.Object,
            Parametros.Object, Sequencial.Object, Contexto.Object);

    public PeriodosAbertosService PeriodosAbertosService()
        => new(Repositorio(), Perfis.Object, Parametros.Object, Contexto.Object);

    public async Task SemearAsync(params object[] entidades)
    {
        // Mesmas opções do contexto de escrita — o store InMemory guarda o modelo de quem o criou.
        await using var ctx = new GestaoFinanceiraDbContext(_opcoesEscrita);
        ctx.AddRange(entidades);
        await ctx.SaveChangesAsync();
    }

    public static CaixaBanco Caixa(int id, ContaTipo tipo = ContaTipo.Caixa, TipoContaCaixa? subtipo = TipoContaCaixa.Normal) => new()
    {
        IdCaixaBanco = id,
        IdFilial = Filial,
        Descricao = $"Caixa {id}",
        TipoConta = tipo,
        TipoContaCaixa = subtipo,
        Ativo = true,
    };

    /// <param name="usuarios">Usuários; o primeiro é o principal.</param>
    public static Dominio NovoDominio(int id, int idCaixa = 1, bool ativo = true, int? idPeriodo = null, params int[] usuarios)
    {
        var d = new Dominio { IdDominio = id, IdFilial = Filial, IdCaixaBanco = idCaixa, Descricao = $"Domínio {id}", Ativo = ativo, IdDominioPeriodo = idPeriodo };
        for (var i = 0; i < usuarios.Length; i++)
            d.AdicionarUsuario(new DominioUsuario { IdDominio = id, IdFilial = Filial, IdUsuario = usuarios[i], UsuarioPrincipal = i == 0 });
        return d;
    }

    public static DominioResponsavel Responsavel(int idFilho, int idPai, bool principal = true)
        => new() { IdDominio = idFilho, IdFilial = Filial, IdDominioPai = idPai, DominioPaiPrincipal = principal };

    public static DominioPeriodo Periodo(int id, int idDominio, DateTime? fechamento = null) => new()
    {
        IdDominioPeriodo = id,
        IdFilial = Filial,
        IdDominio = idDominio,
        DataAbertura = new DateTime(2026, 10, 1),
        HoraAbertura = new DateTime(2026, 10, 1, 8, 0, 0),
        DataFechamento = fechamento,
        HoraFechamento = fechamento,
    };

    public static SalvarDominioDto Dto(int idCaixa = 1, bool ativo = true, DominioUsuarioItemDto[]? usuarios = null,
        DominioResponsavelItemDto[]? responsaveis = null, string descricao = "Domínio novo") => new(
        descricao, idCaixa, ativo, Tesouraria: false, AbrirPeriodo: false, AbrirOutrosPeriodos: false, FecharPeriodo: false,
        FecharOutrosPeriodos: false, MovimentoBanco: false, ConsultaTodosPeriodos: false,
        usuarios ?? [new DominioUsuarioItemDto(7, true)], responsaveis ?? []);

    public void Dispose()
    {
        Context.Dispose();
        ReadContext.Dispose();
    }
}

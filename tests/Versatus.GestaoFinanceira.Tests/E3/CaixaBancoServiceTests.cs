using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Versatus.AcessoGlobal.Domain.Repositories;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.DTOs;
using Versatus.GestaoFinanceira.Domain.Repositories;
using Versatus.GestaoFinanceira.Domain.Services;
using Versatus.GestaoFinanceira.Infrastructure;
using Versatus.GestaoFinanceira.Infrastructure.Repositories;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// Banco InMemory compartilhado entre o contexto de escrita e o de leitura (CQRS leve), com
/// as portas cross-módulo/cross-épico e os parâmetros mockados.
/// </summary>
internal sealed class CenarioE3 : IDisposable
{
    public const int Filial = 10;
    public const int Usuario = 5;

    private readonly string _banco = Guid.NewGuid().ToString();
    private readonly InMemoryDatabaseRoot _raiz = new(); // mesmo armazenamento para todos os contextos

    public GestaoFinanceiraDbContext Context { get; }
    public GestaoFinanceiraReadDbContext ReadContext { get; }
    public Mock<IParametroRepository> Parametros { get; } = new();
    public Mock<IGeradorSequencial> Sequencial { get; } = new();
    public Mock<IInstituicaoFinanceiraConsulta> Instituicoes { get; } = new();
    public Mock<IDominioFinanceiroConsulta> Dominios { get; } = new();
    public Mock<IContextoExecucao> Contexto { get; } = new();

    private readonly DbContextOptions<GestaoFinanceiraDbContext> _opcoesEscrita;

    public CenarioE3()
    {
        _opcoesEscrita = new DbContextOptionsBuilder<GestaoFinanceiraDbContext>()
            .UseInMemoryDatabase(_banco, _raiz)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        Context = new GestaoFinanceiraDbContext(_opcoesEscrita);
        ReadContext = new GestaoFinanceiraReadDbContext(new DbContextOptionsBuilder<GestaoFinanceiraReadDbContext>()
            .UseInMemoryDatabase(_banco, _raiz)
            .Options);

        Contexto.SetupGet(x => x.IdFilial).Returns(Filial);
        Contexto.SetupGet(x => x.IdUsuario).Returns(Usuario);
        Sequencial.Setup(x => x.ProximoAsync(It.IsAny<string>(), SequencialTipo.Filial, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);
    }

    public void Parametro(string nome, bool valor)
        => Parametros.Setup(x => x.GetParametroValorAsync(nome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(valor ? "True" : "False");

    public ContaBancariaService ContaBancariaService()
        => new(new ContaBancariaRepository(Context, ReadContext), Contexto.Object);

    public CaixaBancoService CaixaBancoService()
        => new(new CaixaBancoRepository(Context, ReadContext), new ContaBancariaRepository(Context, ReadContext),
            ContaBancariaService(), Instituicoes.Object, Dominios.Object, Parametros.Object, Sequencial.Object, Contexto.Object);

    public CobradorService CobradorService()
        => new(new CobradorRepository(Context, ReadContext), Sequencial.Object, Contexto.Object);

    public async Task SemearAsync(params object[] entidades)
    {
        // Mesmas opções do contexto de escrita — o store InMemory guarda o modelo de quem o criou.
        await using var ctx = new GestaoFinanceiraDbContext(_opcoesEscrita);
        ctx.AddRange(entidades);
        await ctx.SaveChangesAsync();
    }

    public static CaixaBanco Caixa(int id, ContaTipo tipo, bool ativo = true, params int[] usuarios)
    {
        var caixa = new CaixaBanco { IdCaixaBanco = id, IdFilial = Filial, Descricao = $"Conta {id}", TipoConta = tipo, Ativo = ativo };
        foreach (var u in usuarios)
            caixa.AdicionarUsuario(new CaixaBancoUsuario { IdCaixaBanco = id, IdFilial = Filial, IdUsuario = u });
        return caixa;
    }

    public static ContaBancaria Conta(int id = 0, TipoContaBancaria tipo = TipoContaBancaria.ContaCorrente) => new()
    {
        IdCaixaBanco = id,
        IdFilial = Filial,
        IdAgencia = 1,
        NumeroConta = "12345",
        ContaBancariaTipo = tipo,
    };

    public void Dispose()
    {
        Context.Dispose();
        ReadContext.Dispose();
    }
}

/// <summary>
/// E3-T05 — CaixaBancoService. 1 [Fact] por VAL-E3-01..12 (matriz-rtv.md#E3) + VAL-E1-27, e
/// testes de integração (InMemory) das operações OP-E3-01/02/04/07 (matriz-rot.md#E3).
/// </summary>
public class CaixaBancoServiceTests
{
    // ---- VAL-E3-01 --------------------------------------------------------------------

    [Fact]
    public async Task VAL_E3_01_ComVinculoPorUsuario_UsuarioRepetidoNoCaixa_Falha()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroVinculaCaixaBancoUsuario, true);

        var r = await c.CaixaBancoService().ValidarUsuarioAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, true, 7, 7));

        r.IsValid.Should().BeFalse();
        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgUsuarioRepetido);
    }

    [Fact]
    public async Task VAL_E3_01_SemVinculoPorUsuario_NaoValida()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroVinculaCaixaBancoUsuario, false);

        var r = await c.CaixaBancoService().ValidarUsuarioAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, true, 7, 7));

        r.IsValid.Should().BeTrue();
    }

    // ---- VAL-E3-02..05 (SPED) -----------------------------------------------------------

    [Fact]
    public async Task VAL_E3_02_EnviarSpedSemInstituicaoFinanceira_Falha()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta();
        conta.EnviarSped = true;

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), conta);

        r.IsSuccess.Should().BeFalse();
        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgSpedSemInstituicao);
    }

    [Fact]
    public async Task VAL_E3_03_EnviarSpedComContaDeTerceiro_Falha()
    {
        using var c = new CenarioE3();
        c.Instituicoes.Setup(x => x.ObterAsync(40, It.IsAny<CancellationToken>())).ReturnsAsync(new InstituicaoFinanceiraInfo(3, "00000000000191"));
        var conta = CenarioE3.Conta();
        conta.EnviarSped = true;
        conta.IdInstituicaoFinanceira = 40;
        conta.ContaTerceiro = true;

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), conta);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgSpedContaTerceiro);
    }

    [Fact]
    public async Task VAL_E3_04_InstituicaoFinanceiraSpedPessoaFisica_Falha()
    {
        using var c = new CenarioE3();
        c.Instituicoes.Setup(x => x.ObterAsync(40, It.IsAny<CancellationToken>())).ReturnsAsync(new InstituicaoFinanceiraInfo(2, null));
        var conta = CenarioE3.Conta();
        conta.EnviarSped = true;
        conta.IdInstituicaoFinanceira = 40;

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), conta);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgSpedNaoJuridica);
    }

    [Fact]
    public async Task VAL_E3_05_InstituicaoFinanceiraSpedSemCnpj_Falha()
    {
        using var c = new CenarioE3();
        c.Instituicoes.Setup(x => x.ObterAsync(40, It.IsAny<CancellationToken>())).ReturnsAsync(new InstituicaoFinanceiraInfo(3, ""));
        var conta = CenarioE3.Conta();
        conta.EnviarSped = true;
        conta.IdInstituicaoFinanceira = 40;

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), conta);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgSpedSemCnpj);
    }

    [Fact]
    public async Task VAL_E3_02a05_InstituicaoJuridicaComCnpj_Grava()
    {
        using var c = new CenarioE3();
        c.Instituicoes.Setup(x => x.ObterAsync(40, It.IsAny<CancellationToken>())).ReturnsAsync(new InstituicaoFinanceiraInfo(3, "00000000000191"));
        var conta = CenarioE3.Conta();
        conta.EnviarSped = true;
        conta.IdInstituicaoFinanceira = 40;

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), conta);

        r.IsSuccess.Should().BeTrue();
    }

    // ---- VAL-E3-06/07 (investimento) ------------------------------------------------------

    [Fact]
    public async Task VAL_E3_06_InvestimentoSemContaVinculada_Falha()
    {
        using var c = new CenarioE3();

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), CenarioE3.Conta(tipo: TipoContaBancaria.Investimento));

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgInvestimentoSemVinculada);
    }

    [Fact]
    public async Task VAL_E3_07_InvestimentoPersistidoVinculadoASiMesmo_Falha()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));
        var conta = CenarioE3.Conta(1, TipoContaBancaria.Investimento);
        conta.IdContaBancariaVinculada = 1;

        var r = await c.CaixaBancoService().AtualizarAsync(CenarioE3.Caixa(1, ContaTipo.Banco), conta);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgInvestimentoVinculadaIgual);
    }

    // ---- VAL-E3-08 (inativar caixa com período aberto) -----------------------------------

    [Fact]
    public async Task VAL_E3_08_InativarCaixaComDominioPeriodoAberto_Falha()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, ativo: true));
        c.Dominios.Setup(x => x.ListarPeriodosDosDominiosDoCaixaAsync(1, CenarioE3.Filial, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DominioPeriodoInfo(3, null)]);

        var r = await c.CaixaBancoService().AtualizarAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, ativo: false), null);

        r.Errors.Single().Mensagem.Should().Be(string.Format(CaixaBancoService.MsgInativarCaixaPeriodoAberto, 3));
    }

    [Fact]
    public async Task VAL_E3_08_InativarCaixaComPeriodosFechados_Grava()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, ativo: true));
        c.Dominios.Setup(x => x.ListarPeriodosDosDominiosDoCaixaAsync(1, CenarioE3.Filial, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DominioPeriodoInfo(3, new DateTime(2026, 9, 1))]);

        var r = await c.CaixaBancoService().AtualizarAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, ativo: false), null);

        r.IsSuccess.Should().BeTrue();
        r.Value!.Ativo.Should().BeFalse();
    }

    // ---- VAL-E3-09 (parâmetros mutuamente exclusivos) -------------------------------------

    [Fact]
    public async Task VAL_E3_09_UsuarioSemPermissaoNoDominioComVinculoPorUsuario_Falha()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroVinculaCaixaBancoUsuario, true);

        var r = await c.CaixaBancoService().ValidarControleCaixaBancoAsync(PeriodoStatus.UsuarioSemPermissao);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgParametrosExclusivos);
    }

    [Theory]
    [InlineData(PeriodoStatus.NaoAplicavel)]
    [InlineData(PeriodoStatus.PerfilSemPermissao)]
    [InlineData(PeriodoStatus.Aberto)]
    [InlineData(PeriodoStatus.Fechado)]
    public async Task VAL_E3_09_DemaisStatus_NaoValidam(PeriodoStatus status)
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroVinculaCaixaBancoUsuario, true);

        var r = await c.CaixaBancoService().ValidarControleCaixaBancoAsync(status);

        r.IsValid.Should().BeTrue();
    }

    // ---- VAL-E3-10 (caixa movimentável pelo usuário) --------------------------------------

    [Fact]
    public async Task VAL_E3_10_BancoUsuarioSemDominio_Falha()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroTrabalhaComDominio, true);

        var r = await c.CaixaBancoService().ValidarCaixaPeriodoAsync(CenarioE3.Caixa(1, ContaTipo.Banco));

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgUsuarioSemDominio);
    }

    [Fact]
    public async Task VAL_E3_10_BancoDominioSemMovimentoBanco_Falha()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroTrabalhaComDominio, true);
        c.Dominios.Setup(x => x.ObterMovimentoBancoDoDominioDoUsuarioAsync(CenarioE3.Usuario, CenarioE3.Filial, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var r = await c.CaixaBancoService().ValidarCaixaPeriodoAsync(CenarioE3.Caixa(1, ContaTipo.Banco));

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgDominioSemMovimentoBanco);
    }

    [Fact]
    public async Task VAL_E3_10_CaixaDeDominioMovimentadoPorUsuarioSemDominio_Falha()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroTrabalhaComDominio, true);
        c.Dominios.Setup(x => x.CaixaPossuiDominioAsync(1, CenarioE3.Filial, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var r = await c.CaixaBancoService().ValidarCaixaPeriodoAsync(CenarioE3.Caixa(1, ContaTipo.Caixa));

        r.Errors.Single().Mensagem.Should().Be(string.Format(CaixaBancoService.MsgCaixaVinculadoDominio, 1));
    }

    [Fact]
    public async Task VAL_E3_10_SemTrabalharComDominio_NaoValida()
    {
        using var c = new CenarioE3();
        c.Parametro(CaixaBancoService.ParametroTrabalhaComDominio, false);

        var r = await c.CaixaBancoService().ValidarCaixaPeriodoAsync(CenarioE3.Caixa(1, ContaTipo.Banco));

        r.IsValid.Should().BeTrue();
    }

    // ---- VAL-E3-11/12 (grade de usuários) ---------------------------------------------------

    [Fact]
    public async Task VAL_E3_11_UsuarioInformadoDuasVezesNaGrade_Falha()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa));

        var r = await c.CaixaBancoService().SalvarUsuariosAsync(1, CenarioE3.Filial,
            [new CaixaBancoUsuarioItemDto(7), new CaixaBancoUsuarioItemDto(7)]);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgUsuarioJaInformado);
    }

    [Fact]
    public async Task VAL_E3_12_AlterarUsuarioDeLinhaJaSalva_Falha()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, true, 7));

        var r = await c.CaixaBancoService().SalvarUsuariosAsync(1, CenarioE3.Filial, [new CaixaBancoUsuarioItemDto(9, IdUsuarioSalvo: 7)]);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgAlterarUsuarioSalvo);
    }

    // ---- VAL-E3-17 no fluxo do caixa ----------------------------------------------------------

    [Fact]
    public async Task VAL_E3_17_NoCaixa_ContaDeTerceiroSemCpfCnpj_Falha()
    {
        using var c = new CenarioE3();
        var conta = CenarioE3.Conta();
        conta.ContaTerceiro = true;
        conta.Titular = "Fulano";

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco), conta);

        r.Errors.Single().Mensagem.Should().Be(ContaBancariaService.MsgContaTerceiro);
    }

    // ---- VAL-E1-27 (parcela exige conta corrente) --------------------------------------------

    [Fact]
    public async Task VAL_E1_27_CaixaBancoDaParcelaComContaInvestimento_Falha()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1, TipoContaBancaria.Investimento));

        var r = await c.CaixaBancoService().ValidarCaixaBancoParcelaAsync(1, CenarioE3.Filial);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgParcelaContaCorrente);
    }

    [Fact]
    public async Task VAL_E1_27_CaixaBancoDaParcelaComContaCorrente_Valida()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));

        var r = await c.CaixaBancoService().ValidarCaixaBancoParcelaAsync(1, CenarioE3.Filial);

        r.IsValid.Should().BeTrue();
    }

    // ---- Operações (integração InMemory) ---------------------------------------------------------

    [Fact]
    public async Task OP_E3_01_Criar_GeraSequencialPorFilialEGravaCaixaUsuariosEConta()
    {
        using var c = new CenarioE3();

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Banco, true, 7, 8), CenarioE3.Conta());

        r.IsSuccess.Should().BeTrue();
        c.Sequencial.Verify(x => x.ProximoAsync(CaixaBancoService.NomeSequencial, SequencialTipo.Filial, It.IsAny<CancellationToken>()));

        var caixa = await c.ReadContext.CaixasBanco.Include(x => x.Usuarios).SingleAsync();
        caixa.IdCaixaBanco.Should().Be(100);
        caixa.IdFilial.Should().Be(CenarioE3.Filial);
        caixa.IdUsuarioInclusao.Should().Be(CenarioE3.Usuario);
        caixa.Usuarios.Select(u => u.IdUsuario).Should().BeEquivalentTo([7, 8]);
        caixa.Usuarios.Should().OnlyContain(u => u.IdCaixaBanco == 100 && u.IdUsuarioInclusao == CenarioE3.Usuario);

        var conta = await c.ReadContext.ContasBancarias.SingleAsync();
        conta.IdCaixaBanco.Should().Be(100);
        conta.IdUsuarioInclusao.Should().Be(CenarioE3.Usuario);
    }

    [Fact]
    public async Task OP_E3_01_CriarCaixa_NaoGravaContaBancaria()
    {
        using var c = new CenarioE3();

        var r = await c.CaixaBancoService().CriarAsync(CenarioE3.Caixa(0, ContaTipo.Caixa), CenarioE3.Conta());

        r.IsSuccess.Should().BeTrue();
        (await c.ReadContext.ContasBancarias.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task OP_E3_02_Excluir_RemoveContaUsuariosECaixa()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco, true, 7), CenarioE3.Conta(1));

        var r = await c.CaixaBancoService().ExcluirAsync(1, CenarioE3.Filial);

        r.IsValid.Should().BeTrue();
        (await c.ReadContext.CaixasBanco.CountAsync()).Should().Be(0);
        (await c.ReadContext.CaixaBancoUsuarios.CountAsync()).Should().Be(0);
        (await c.ReadContext.ContasBancarias.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task OP_E3_02_ExcluirInexistente_Falha()
    {
        using var c = new CenarioE3();

        var r = await c.CaixaBancoService().ExcluirAsync(99, CenarioE3.Filial);

        r.Errors.Single().Mensagem.Should().Be(CaixaBancoService.MsgNaoEncontrado);
    }

    [Fact]
    public async Task OP_E3_03_Atualizar_RegistraAuditoriaDeAlteracao()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa));
        var dados = CenarioE3.Caixa(1, ContaTipo.Caixa);
        dados.Descricao = "Caixa renomeado";

        var r = await c.CaixaBancoService().AtualizarAsync(dados, null);

        r.IsSuccess.Should().BeTrue();
        var caixa = await c.ReadContext.CaixasBanco.SingleAsync();
        caixa.Descricao.Should().Be("Caixa renomeado");
        caixa.IdUsuarioAlteracao.Should().Be(CenarioE3.Usuario);
        caixa.DataAlteracao.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task OP_E3_04_AlterarDeBancoParaCaixa_RemoveContaBancaria()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta(1));

        var r = await c.CaixaBancoService().AtualizarAsync(CenarioE3.Caixa(1, ContaTipo.Caixa), null);

        r.IsSuccess.Should().BeTrue();
        (await c.ReadContext.ContasBancarias.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task OP_E3_04_AlterarDeCaixaParaBanco_CriaContaBancaria()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa));

        var r = await c.CaixaBancoService().AtualizarAsync(CenarioE3.Caixa(1, ContaTipo.Banco), CenarioE3.Conta());

        r.IsSuccess.Should().BeTrue();
        var conta = await c.ReadContext.ContasBancarias.SingleAsync();
        conta.IdCaixaBanco.Should().Be(1);
        conta.IdFilial.Should().Be(CenarioE3.Filial);
    }

    [Fact]
    public async Task OP_E3_07_SalvarUsuarios_InsereNovosERemoveAusentes()
    {
        using var c = new CenarioE3();
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa, true, 7, 8));

        var r = await c.CaixaBancoService().SalvarUsuariosAsync(1, CenarioE3.Filial,
            [new CaixaBancoUsuarioItemDto(8, IdUsuarioSalvo: 8), new CaixaBancoUsuarioItemDto(9)]);

        r.IsSuccess.Should().BeTrue();
        var usuarios = await c.ReadContext.CaixaBancoUsuarios.OrderBy(u => u.IdUsuario).ToListAsync();
        usuarios.Select(u => u.IdUsuario).Should().Equal(8, 9);
        usuarios[1].IdUsuarioInclusao.Should().Be(CenarioE3.Usuario);
    }

    [Fact]
    public async Task ListarPaginado_FiltraPelaFilialDoContextoEPaginaEmMemoria()
    {
        using var c = new CenarioE3();
        var outraFilial = CenarioE3.Caixa(9, ContaTipo.Caixa);
        outraFilial.IdFilial = 99;
        await c.SemearAsync(CenarioE3.Caixa(1, ContaTipo.Caixa), CenarioE3.Caixa(2, ContaTipo.Banco),
            CenarioE3.Caixa(3, ContaTipo.Banco), outraFilial);

        var r = await c.CaixaBancoService().ListarPaginadoAsync(null, null, ContaTipo.Banco, null, page: 2, limit: 1);

        r.Total.Should().Be(2);
        r.Items.Single().IdCaixaBanco.Should().Be(3);
    }
}

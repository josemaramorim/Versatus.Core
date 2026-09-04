using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Security;
using Versatus.AcessoGlobal.Domain.Finance;
using Versatus.AcessoGlobal.Domain.Configuration;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.SharedKernel.Enums;
using Xunit;
using FluentAssertions;

namespace Versatus.AcessoGlobal.Tests;

public class MappingIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AcessoGlobalDbContext _context;

    public MappingIntegrationTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AcessoGlobalDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AcessoGlobalDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task DeveSalvarERecuperarEntidadeComClienteEFisica()
    {
        // Arrange
        var entidade = new Entidade
        {
            IdEntidade = 1,
            Nome = "Entidade de Teste",
            TipoPessoa = EntidadeTipoPessoa.Fisica,
            PessoaFisica = new DadosPessoaFisica
            {
                IdEntidade = 1,
                Cpf = "12345678901",
                DataNascimento = new DateTime(1990, 1, 1)
            }
        };

        var cliente = new Cliente
        {
            IdCliente = 1,
            RendaMensal = 5000,
            Ativo = true,
            SituacaoSPC = SituacaoClienteSPC.Normal
        };

        // Act
        _context.Entidades.Add(entidade);
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Limpar context para forçar reload do banco
        _context.ChangeTracker.Clear();

        // Assert
        var recuperada = await _context.Entidades
            .Include(e => e.PessoaFisica)
            .FirstOrDefaultAsync(e => e.IdEntidade == 1);

        var clienteRecuperado = await _context.Clientes
            .FirstOrDefaultAsync(c => c.IdCliente == 1);

        recuperada.Should().NotBeNull();
        recuperada!.Nome.Should().Be("Entidade de Teste");
        recuperada.PessoaFisica.Should().NotBeNull();
        recuperada.PessoaFisica!.Cpf.Should().Be("12345678901");
        
        clienteRecuperado.Should().NotBeNull();
        clienteRecuperado!.RendaMensal.Should().Be(5000);
    }

    [Fact]
    public async Task DeveSalvarERecuperarUsuarioEPerfil()
    {
        // Arrange
        var perfil = new Perfil
        {
            IdPerfil = 10,
            Descricao = "Administrador do Sistema",
            Administrador = true,
            UsaDominioFinanceiro = true,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Today
        };

        var usuario = new Usuario
        {
            IdUsuario = 5,
            Login = "admin",
            Nome = "Admin Teste",
            PasswordHash = System.Text.Encoding.UTF8.GetBytes("hashedpassword"),
            Ativo = true,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Today
        };
        usuario.Perfis.Add(perfil);

        // Act
        _context.Perfis.Add(perfil);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var usuarioRecuperado = await _context.Usuarios
            .Include(u => u.Perfis)
            .FirstOrDefaultAsync(u => u.IdUsuario == 5);

        usuarioRecuperado.Should().NotBeNull();
        usuarioRecuperado!.Login.Should().Be("admin");
        usuarioRecuperado.Perfis.Should().NotBeEmpty();
        usuarioRecuperado.Perfis.First().Descricao.Should().Be("Administrador do Sistema");
    }

    [Fact]
    public async Task DeveSalvarERecuperarBancoEFormaPagamento()
    {
        // Arrange
        var banco = new Banco
        {
            IdBanco = 341,
            Codigo = 341,
            Nome = "Banco Itaú S.A.",
            Ativo = true,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Today
        };

        var formaPagamento = new FormaPagamento
        {
            IdForma = 1,
            Codigo = "BOLETO",
            Nome = "Boleto Bancário",
            Tipo = FormaPagtoTipo.Outros,
            Ativo = true,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Today
        };

        // Act
        _context.Bancos.Add(banco);
        _context.FormasPagamento.Add(formaPagamento);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var bancoRecuperado = await _context.Bancos.FirstOrDefaultAsync(b => b.IdBanco == 341);
        var formaRecuperada = await _context.FormasPagamento.FirstOrDefaultAsync(fp => fp.IdForma == 1);

        bancoRecuperado.Should().NotBeNull();
        bancoRecuperado!.Nome.Should().Be("Banco Itaú S.A.");
        bancoRecuperado.Codigo.Should().Be(341);

        formaRecuperada.Should().NotBeNull();
        formaRecuperada!.Nome.Should().Be("Boleto Bancário");
        formaRecuperada.Tipo.Should().Be(FormaPagtoTipo.Outros);
    }

    [Fact]
    public async Task DeveSalvarERecuperarConfiguracoesESeries()
    {
        // Arrange
        var serie = new SerieDocumento
        {
            IdSerie = 1,
            Codigo = "01",
            Nome = "Série Principal",
            Prefixo = "55",
            ProximoNumero = 1000,
            Ativa = true,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Today
        };

        var parametro = new Parametro
        {
            IdParam = 1,
            Chave = "CAMINHO_XML",
            Descricao = "Caminho para salvar os XMLs",
            Valor = @"C:\XML",
            Tipo = 156
        };

        // Act
        _context.SeriesDocumento.Add(serie);
        _context.Parametros.Add(parametro);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var serieRecuperada = await _context.SeriesDocumento.FirstOrDefaultAsync(s => s.IdSerie == 1);
        var paramRecuperado = await _context.Parametros.FirstOrDefaultAsync(p => p.IdParam == 1);

        serieRecuperada.Should().NotBeNull();
        serieRecuperada!.Nome.Should().Be("Série Principal");
        serieRecuperada.Codigo.Should().Be("01");

        paramRecuperado.Should().NotBeNull();
        paramRecuperado!.Chave.Should().Be("CAMINHO_XML");
        paramRecuperado.Valor.Should().Be(@"C:\XML");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}

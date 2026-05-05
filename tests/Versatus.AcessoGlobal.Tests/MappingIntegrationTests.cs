using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Infrastructure;
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

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}

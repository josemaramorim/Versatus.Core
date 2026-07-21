using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Versatus.AcessoGlobal.DependencyInjection;
using Versatus.AcessoGlobal.Domain.Entities;
using Versatus.AcessoGlobal.Domain.Services;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.Framework.Context;
using Versatus.Framework.Sequences;

Console.WriteLine("=== Versatus AcessoGlobal Demo ===");

// 1. Setup DI
var services = new ServiceCollection();

// Configurar o Banco de Dados (Use uma connection string válida aqui para teste real)
const string connectionString = "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;";

services.AddDbContext<AcessoGlobalDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar Módulos
services.AddAcessoGlobal();

// Mock do Contexto de Execução (Simulando usuário logado)
services.AddScoped<IContextoExecucao>(sp => new FakeContextoExecucao());

// Mock do Gerador de Sequencial (Para o demo simplificado, vamos usar um fake ou injetar um real se o banco de sequencial existir)
services.AddScoped<IGeradorSequencial, FakeGeradorSequencial>();

services.AddLogging(configure => configure.AddConsole());

var serviceProvider = services.BuildServiceProvider();

// 2. Executar Demo
using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AcessoGlobalDbContext>();
    var entidadeService = scope.ServiceProvider.GetRequiredService<IEntidadeService>();

    Console.WriteLine("\n--- Criando Banco de Dados de Teste ---");
    context.Database.EnsureCreated();

    Console.WriteLine("\n--- Criando Nova Entidade (Pessoa Física) ---");
    var novaEntidade = new Entidade
    {
        Nome = "João da Silva",
        TipoPessoa = EntidadeTipoPessoa.Fisica,
        PessoaFisica = new DadosPessoaFisica
        {
            Cpf = "12345678901",
            DataNascimento = new DateTime(1985, 5, 20)
        }
    };

    try
    {
        var entidadeCriada = await entidadeService.CriarAsync(novaEntidade);
        Console.WriteLine($"✔ Entidade criada com sucesso! ID: {entidadeCriada.IdEntidade}");
        Console.WriteLine($"  Data Inclusão: {entidadeCriada.DataInclusao:dd/MM/yyyy} por Usuário ID: {entidadeCriada.IdUsuarioInclusao}");
        
        // Buscar novamente para confirmar
        var busca = await context.Entidades
            .Include(e => e.PessoaFisica)
            .FirstOrDefaultAsync(e => e.IdEntidade == entidadeCriada.IdEntidade);

        if (busca != null)
        {
            Console.WriteLine($"✔ Busca realizada com sucesso! Nome: {busca.Nome}, CPF: {busca.PessoaFisica?.Cpf}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro: {ex.Message}");
    }
}

Console.WriteLine("\nDemo finalizada. Pressione qualquer tecla para sair...");

// Classes Fake para o Demo
public class FakeContextoExecucao : IContextoExecucao
{
    public int IdUsuario { get; set; } = 1;
    public int IdEmpresa { get; set; } = 1;
    public int IdFilial { get; set; } = 1;
    public string[] Perfis { get; set; } = ["Admin"];
}

public class FakeGeradorSequencial : IGeradorSequencial
{
    private int _current = 100000;
    public Task<int> ProximoAsync(string nomeObjeto, SequencialTipo tipo, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(++_current);
    }

    public Task SetValorAsync(string nomeObjeto, SequencialTipo tipo, int novoValor, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

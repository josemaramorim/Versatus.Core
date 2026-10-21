using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Versatus.AcessoGlobal.Domain.Configuration;
using Versatus.AcessoGlobal.Domain.Organization;
using Versatus.AcessoGlobal.Infrastructure;
using Versatus.AcessoGlobal.Infrastructure.Repositories;
using Xunit;

namespace Versatus.AcessoGlobal.Tests;

/// <summary>
/// Resolução do valor de parâmetro por escopo, como no legado
/// (acesso.global/ParametroServidor.cs:240-278): o tipo do parâmetro decide o nível e cada nível
/// exige a combinação exata das chaves, sem recorrer ao nível mais geral.
/// </summary>
public class ParametroRepositoryEscopoTests : IDisposable
{
    private const int Grupo = 1;
    private const int Empresa = 10;
    private const int FilialA = 100;
    private const int FilialB = 200;

    private readonly SqliteConnection _connection;
    private readonly AcessoGlobalDbContext _context;
    private readonly AcessoGlobalReadDbContext _readContext;
    private readonly ParametroRepository _repositorio;
    private int _proximoIdValor = 1;

    public ParametroRepositoryEscopoTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _context = new AcessoGlobalDbContext(new DbContextOptionsBuilder<AcessoGlobalDbContext>().UseSqlite(_connection).Options);
        _context.Database.EnsureCreated();
        _readContext = new AcessoGlobalReadDbContext(new DbContextOptionsBuilder<AcessoGlobalReadDbContext>().UseSqlite(_connection).Options);

        _context.Set<Grupo>().Add(new Grupo { IdGrupo = Grupo, Nome = "Grupo" });
        _context.Set<Empresa>().Add(new Empresa { IdEmpresa = Empresa, IdGrupo = Grupo, Nome = "Empresa" });
        _context.SaveChanges();

        _repositorio = new ParametroRepository(_context, _readContext);
    }

    public void Dispose()
    {
        _readContext.Dispose();
        _context.Dispose();
        _connection.Dispose();
    }

    private void NovoParametro(int id, string chave, int? tipo, params (int? grupo, int? empresa, int? filial, int? perfil, string valor)[] valores)
    {
        _context.Set<Parametro>().Add(new Parametro { IdParam = id, Chave = chave, TipoParametro = tipo });
        foreach (var v in valores)
        {
            _context.Set<ParametroValor>().Add(new ParametroValor
            {
                IdParametroValor = _proximoIdValor++,
                IdParametro = id,
                IdGrupo = v.grupo,
                IdEmpresa = v.empresa,
                IdFilial = v.filial,
                IdPerfil = v.perfil,
                Valor = v.valor,
            });
        }

        _context.SaveChanges();
    }

    [Fact]
    public async Task Filial_DeveDevolverOValorDaFilialDoEscopo()
    {
        NovoParametro(1, "TRABALHACOMDOMINIO", ParametroRepository.TipoParametroFilial,
            (Grupo, Empresa, FilialA, null, "True"),
            (Grupo, Empresa, FilialB, null, "False"));

        Assert.Equal("True", await _repositorio.GetParametroValorAsync("TRABALHACOMDOMINIO", Empresa, FilialA));
        Assert.Equal("False", await _repositorio.GetParametroValorAsync("TRABALHACOMDOMINIO", Empresa, FilialB));
    }

    [Fact]
    public async Task Filial_SemValorDaFilial_NaoDeveUsarValorDaEmpresa()
    {
        // O legado não recorre ao nível mais geral: sem linha da filial, devolve o padrão do tipo.
        NovoParametro(1, "TRABALHACOMDOMINIO", ParametroRepository.TipoParametroFilial,
            (Grupo, Empresa, null, null, "True"),
            (Grupo, Empresa, FilialB, null, "True"));

        Assert.Null(await _repositorio.GetParametroValorAsync("TRABALHACOMDOMINIO", Empresa, FilialA));
    }

    [Fact]
    public async Task Filial_LinhaDePerfil_DeveSerIgnorada()
    {
        NovoParametro(1, "DOMINIOPADRAO", ParametroRepository.TipoParametroFilial,
            (Grupo, Empresa, FilialA, 5, "9"));

        Assert.Null(await _repositorio.GetParametroValorAsync("DOMINIOPADRAO", Empresa, FilialA));
    }

    [Fact]
    public async Task Empresa_DeveUsarSoALinhaSemFilial()
    {
        NovoParametro(1, "PARAMEMPRESA", ParametroRepository.TipoParametroEmpresa,
            (Grupo, Empresa, FilialA, null, "da filial"),
            (Grupo, Empresa, null, null, "da empresa"));

        Assert.Equal("da empresa", await _repositorio.GetParametroValorAsync("PARAMEMPRESA", Empresa, FilialA));
    }

    [Fact]
    public async Task Grupo_DeveUsarSoALinhaSemEmpresaESemFilial()
    {
        NovoParametro(1, "PARAMGRUPO", ParametroRepository.TipoParametroGrupo,
            (Grupo, Empresa, null, null, "da empresa"),
            (Grupo, null, null, null, "do grupo"));

        Assert.Equal("do grupo", await _repositorio.GetParametroValorAsync("PARAMGRUPO", Empresa, FilialA));
    }

    [Fact]
    public async Task Grupo_DeOutroGrupo_DeveSerIgnorado()
    {
        NovoParametro(1, "PARAMGRUPO", ParametroRepository.TipoParametroGrupo,
            (99, null, null, null, "outro grupo"));

        Assert.Null(await _repositorio.GetParametroValorAsync("PARAMGRUPO", Empresa, FilialA));
    }

    [Fact]
    public async Task Sistema_DeveDevolverOPrimeiroValorIndependenteDoEscopo()
    {
        // INDICEECONOMICOPADRAO no banco real: tipo Sistema com linha de grupo/empresa preenchidos.
        NovoParametro(1, "INDICEECONOMICOPADRAO", ParametroRepository.TipoParametroSistema,
            (Grupo, 77, null, null, "1"),
            (null, null, null, null, "2"));

        Assert.Equal("1", await _repositorio.GetParametroValorAsync("INDICEECONOMICOPADRAO", Empresa, FilialA));
    }

    [Fact]
    public async Task TipoNaoInformado_DeveSeguirORamoDeSistema()
    {
        NovoParametro(1, "SEMTIPO", null, (Grupo, Empresa, FilialB, null, "x"));

        Assert.Equal("x", await _repositorio.GetParametroValorAsync("SEMTIPO", Empresa, FilialA));
    }

    [Fact]
    public async Task Perfil_DeveLancarNaoSuportado()
    {
        NovoParametro(1, "PARAMPERFIL", ParametroRepository.TipoParametroPerfil,
            (Grupo, Empresa, FilialA, 5, "x"));

        await Assert.ThrowsAsync<NotSupportedException>(() => _repositorio.GetParametroValorAsync("PARAMPERFIL", Empresa, FilialA));
    }

    [Fact]
    public async Task EmpresaInexistente_DeveDevolverNulo()
    {
        NovoParametro(1, "TRABALHACOMDOMINIO", ParametroRepository.TipoParametroFilial,
            (Grupo, Empresa, FilialA, null, "True"));

        Assert.Null(await _repositorio.GetParametroValorAsync("TRABALHACOMDOMINIO", 999, FilialA));
    }

    [Fact]
    public async Task ParametroInexistente_DeveDevolverNulo()
    {
        Assert.Null(await _repositorio.GetParametroValorAsync("NAOEXISTE", Empresa, FilialA));
    }
}

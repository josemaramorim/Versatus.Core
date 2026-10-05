using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Domain.Dominio;
using Versatus.GestaoFinanceira.Infrastructure;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E2;

/// <summary>
/// E2-T03 — DbSets do épico E2 + teste de mapeamento (InMemory). Cobre E2-T02: tabela e ordem
/// física das PKs compostas (analysis/E2-dominio.md §2), materialização, agregados
/// Dominio→Usuarios/Responsaveis e DominioPeriodo→Fechamentos, índice UNIQUE do fechamento,
/// ciclo de FKs FINDOMINIO↔FINDOMINIOPERIODO e as duas views sem chave (§2.8).
/// </summary>
public class MapeamentoE2Tests
{
    private static GestaoFinanceiraDbContext NovoContexto(string banco) =>
        new(new DbContextOptionsBuilder<GestaoFinanceiraDbContext>().UseInMemoryDatabase(banco).Options);

    private static CaixaBanco NovoCaixa(int id, int filial) => new()
    {
        IdCaixaBanco = id,
        IdFilial = filial,
        Descricao = $"Caixa {id}/{filial}",
        TipoConta = ContaTipo.Caixa,
        TipoContaCaixa = TipoContaCaixa.Normal,
        Ativo = true,
    };

    private static Dominio NovoDominio(int id, int filial, int idCaixa = 1) => new()
    {
        IdDominio = id,
        IdFilial = filial,
        IdCaixaBanco = idCaixa,
        Descricao = $"Domínio {id}/{filial}",
    };

    private static DominioPeriodo NovoPeriodo(int id, int filial, int idDominio) => new()
    {
        IdDominioPeriodo = id,
        IdFilial = filial,
        IdDominio = idDominio,
        DataAbertura = new DateTime(2026, 10, 5),
        HoraAbertura = new DateTime(2026, 10, 5, 8, 30, 0),
    };

    [Theory]
    [InlineData("Dominios", typeof(Dominio))]
    [InlineData("DominioUsuarios", typeof(DominioUsuario))]
    [InlineData("DominioResponsaveis", typeof(DominioResponsavel))]
    [InlineData("DominioPeriodos", typeof(DominioPeriodo))]
    [InlineData("DominioPeriodoFechamentos", typeof(DominioPeriodoFechamento))]
    [InlineData("DominioPeriodoLactos", typeof(DominioPeriodoLacto))]
    [InlineData("DominioPeriodoFormasPagto", typeof(DominioPeriodoFormaPagto))]
    [InlineData("DominioUsuariosSuprimentoSangria", typeof(DominioUsuarioSuprimentoSangria))]
    [InlineData("ResumosDominioPeriodoFormaPagto", typeof(ResumoDominioPeriodoFormaPagto))]
    public void DbContext_DeveExporDbSetDaEntidadeE2(string propriedade, Type entidade)
    {
        var prop = typeof(GestaoFinanceiraDbContext).GetProperty(propriedade);

        prop.Should().NotBeNull($"o DbSet {propriedade} deve estar registrado");
        prop!.PropertyType.Should().Be(typeof(DbSet<>).MakeGenericType(entidade));
    }

    [Theory]
    [InlineData(typeof(Dominio), "FINDOMINIO", new[] { "IDFINDOMINIO", "IDGLOFILIAL" })]
    [InlineData(typeof(DominioUsuario), "FINDOMINIOUSUARIO", new[] { "IDFINDOMINIO", "IDGLOUSUARIO", "IDGLOFILIAL" })]
    [InlineData(typeof(DominioResponsavel), "FINDOMINIORESPONSAVEL", new[] { "IDFINDOMINIO", "IDGLOFILIAL", "IDFINDOMINIOPAI" })]
    [InlineData(typeof(DominioPeriodo), "FINDOMINIOPERIODO", new[] { "IDFINDOMINIOPERIODO", "IDGLOFILIAL" })]
    [InlineData(typeof(DominioPeriodoFechamento), "FINDOMINIOPERIODOFECHAMENTO", new[] { "IDFINDOMINIOPERIODOFECHAMENTO", "IDGLOFILIAL" })]
    [InlineData(typeof(DominioPeriodoLacto), "FINDOMINIOPERIODOLANCTO", new[] { "IDFINDOMINIOPERIODOLANCTO", "IDGLOFILIAL" })]
    [InlineData(typeof(DominioPeriodoFormaPagto), "FINDOMINIOPERIODOFORMAPAGTO", new[] { "IDFINDOMINIOPERIODOFORMAPAGTO", "IDGLOFILIAL" })]
    public void Mapeamento_DeveUsarTabelaEPkNaOrdemFisica(Type entidade, string tabela, string[] colunasPk)
    {
        using var ctx = NovoContexto(nameof(Mapeamento_DeveUsarTabelaEPkNaOrdemFisica));
        var tipo = ctx.Model.FindEntityType(entidade)!;

        tipo.GetTableName().Should().Be(tabela);
        tipo.FindPrimaryKey()!.Properties.Select(p => p.GetColumnName())
            .Should().Equal(colunasPk);
    }

    [Fact]
    public async Task Dominio_DevePersistirEMaterializarComUsuariosEResponsaveis()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            ctx.CaixasBanco.Add(NovoCaixa(1, 10));
            ctx.Dominios.Add(NovoDominio(1, 10));

            var dominio = NovoDominio(2, 10);
            dominio.AbrirOutrosPeriodos = true;
            dominio.SaldoDinheiro = 150.12345678m;
            dominio.AdicionarUsuario(new DominioUsuario { IdDominio = 2, IdUsuario = 7, IdFilial = 10, UsuarioPrincipal = true });
            dominio.AdicionarUsuario(new DominioUsuario { IdDominio = 2, IdUsuario = 8, IdFilial = 10 });
            dominio.AdicionarResponsavel(new DominioResponsavel { IdDominio = 2, IdFilial = 10, IdDominioPai = 1, DominioPaiPrincipal = true });
            ctx.Dominios.Add(dominio);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var lido = await ctx.Dominios
                .Include(x => x.Usuarios)
                .Include(x => x.Responsaveis)
                .SingleAsync(x => x.IdDominio == 2);

            lido.Ativo.Should().BeTrue("o construtor legado inicia Ativo = true");
            lido.AbrirOutrosPeriodos.Should().BeTrue();
            lido.AbrirPeriodo.Should().BeFalse();
            lido.IdDominioPeriodo.Should().BeNull();
            lido.SaldoDinheiro.Should().Be(150.12345678m);
            lido.Usuarios.Should().ContainSingle(u => u.UsuarioPrincipal).Which.IdUsuario.Should().Be(7);
            lido.Usuarios.Should().HaveCount(2);
            lido.Responsaveis.Should().ContainSingle().Which.IdDominioPai.Should().Be(1);
        }
    }

    [Fact]
    public async Task DominioUsuario_MesmoUsuarioEmFiliaisDiferentes_DeveCoexistir()
    {
        await using var ctx = NovoContexto(Guid.NewGuid().ToString());
        ctx.DominioUsuarios.AddRange(
            new DominioUsuario { IdDominio = 1, IdUsuario = 7, IdFilial = 10 },
            new DominioUsuario { IdDominio = 1, IdUsuario = 7, IdFilial = 20 });
        await ctx.SaveChangesAsync();

        (await ctx.DominioUsuarios.CountAsync()).Should().Be(2);
    }

    [Fact]
    public void Dominio_PkCompostaDuplicada_DeveSerRejeitada()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());
        ctx.Dominios.Add(NovoDominio(1, 10));

        var acao = () => ctx.Dominios.Add(NovoDominio(1, 10));

        acao.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task DominioPeriodo_DevePersistirComFechamentosEDatasRedundantes()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            var periodo = NovoPeriodo(5, 10, 1);
            periodo.DataFechamento = new DateTime(2026, 10, 5);
            periodo.HoraFechamento = new DateTime(2026, 10, 5, 18, 0, 0);
            periodo.IdUsuarioFechamento = 7;
            periodo.AdicionarFechamento(new DominioPeriodoFechamento
            {
                IdDominioPeriodoFechamento = 1, IdFilial = 10, IdDominioPeriodo = 5,
                TipoForma = FormaPagtoTipo.Dinheiro, ValorCalculado = 100.5m, ValorInformado = 99.5m,
            });
            periodo.AdicionarFechamento(new DominioPeriodoFechamento
            {
                IdDominioPeriodoFechamento = 2, IdFilial = 10, IdDominioPeriodo = 5,
                TipoForma = FormaPagtoTipo.ChequeCliente, ValorCalculado = 30m, ValorInformado = 30m,
            });
            ctx.DominioPeriodos.Add(periodo);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var lido = await ctx.DominioPeriodos.Include(x => x.Fechamentos).SingleAsync();

            lido.HoraAbertura.Should().Be(new DateTime(2026, 10, 5, 8, 30, 0));
            lido.HoraFechamento.Should().Be(new DateTime(2026, 10, 5, 18, 0, 0));
            lido.DataFechamentoTesouraria.Should().BeNull();
            lido.Fechamentos.Should().HaveCount(2);
            lido.Fechamentos.Single(f => f.TipoForma == FormaPagtoTipo.Dinheiro).ValorInformado.Should().Be(99.5m);
        }
    }

    [Fact]
    public void DominioPeriodoFechamento_DeveTerIndiceUnicoPorFilialPeriodoForma()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());
        var tipo = ctx.Model.FindEntityType(typeof(DominioPeriodoFechamento))!;

        var indice = tipo.GetIndexes().Single(i => i.GetDatabaseName() == "IDX_FINDOMINIOPERIODOFECHAMENTO01");

        indice.IsUnique.Should().BeTrue();
        indice.Properties.Select(p => p.GetColumnName())
            .Should().Equal("IDGLOFILIAL", "IDFINDOMINIOPERIODO", "IDTIPOFORMA");
    }

    [Fact]
    public async Task Dominio_PonteiroParaPeriodo_DeveAceitarOrdemDoLegado()
    {
        // OP-E2-03: grava o período e depois atualiza o ponteiro FINDOMINIO.IDFINDOMINIOPERIODO.
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            ctx.CaixasBanco.Add(NovoCaixa(1, 10));
            ctx.Dominios.Add(NovoDominio(1, 10));
            await ctx.SaveChangesAsync();

            ctx.DominioPeriodos.Add(NovoPeriodo(9, 10, 1));
            await ctx.SaveChangesAsync();

            var dominio = await ctx.Dominios.SingleAsync();
            dominio.IdDominioPeriodo = 9;
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            (await ctx.Dominios.SingleAsync()).IdDominioPeriodo.Should().Be(9);
        }
    }

    [Fact]
    public async Task DominioPeriodoLacto_DeveAceitarOrigemEDestinoNulosEEnums()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            // Abertura do domínio padrão: sem período de origem (OP-E2-06).
            ctx.DominioPeriodoLactos.Add(new DominioPeriodoLacto
            {
                IdDominioPeriodoLacto = 1, IdFilial = 10, IdPeriodoOrigem = null, IdPeriodoDestino = 9,
                TipoLancto = LanctoDominioPeriodoTipo.Abertura, TipoForma = FormaPagtoTipo.Dinheiro,
                IdCaixaBanco = 1, DataLancto = new DateTime(2026, 10, 5, 8, 30, 15), Valor = 250.12345678m,
            });
            // Suprimento de cheque de empresa: caixa do talão + número da folha (OP-E2-09).
            ctx.DominioPeriodoLactos.Add(new DominioPeriodoLacto
            {
                IdDominioPeriodoLacto = 2, IdFilial = 10, IdPeriodoOrigem = 8, IdPeriodoDestino = 9,
                TipoLancto = LanctoDominioPeriodoTipo.Suprimento, TipoForma = FormaPagtoTipo.ChequeEmpresa,
                IdCaixaBanco = 3, Cheque = 1001, DataLancto = new DateTime(2026, 10, 5), Valor = 80m,
            });
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var lanctos = await ctx.DominioPeriodoLactos.OrderBy(x => x.IdDominioPeriodoLacto).ToListAsync();

            lanctos[0].IdPeriodoOrigem.Should().BeNull();
            lanctos[0].TipoLancto.Should().Be(LanctoDominioPeriodoTipo.Abertura);
            lanctos[0].DataLancto.Should().Be(new DateTime(2026, 10, 5, 8, 30, 15));
            lanctos[0].Valor.Should().Be(250.12345678m);
            lanctos[0].Cheque.Should().BeNull();
            lanctos[1].TipoForma.Should().Be(FormaPagtoTipo.ChequeEmpresa);
            lanctos[1].Cheque.Should().Be(1001);
        }
    }

    [Fact]
    public async Task DominioPeriodoFormaPagto_DeveManterHoraTextoEOrigemGeradora()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            ctx.DominioPeriodoFormasPagto.Add(new DominioPeriodoFormaPagto
            {
                IdDominioPeriodoFormaPagto = 1, IdFilial = 10, IdDominioPeriodo = 9, IdFormaPagamento = 4,
                Credito = 10m, Debito = null, Data = new DateTime(2026, 10, 5), Hora = "08:30:15",
                IdOrigem = 77, IdProcessoOrigem = ProcessoOrigem.MovimentoChequeRecebido,
            });
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var forma = await ctx.DominioPeriodoFormasPagto.SingleAsync();

            forma.Hora.Should().Be("08:30:15");
            forma.Debito.Should().BeNull();
            forma.IdProcessoOrigem.Should().Be(ProcessoOrigem.MovimentoChequeRecebido);
            forma.IdOrigem.Should().Be(77);
        }
    }

    [Theory]
    [InlineData(typeof(DominioUsuarioSuprimentoSangria), "VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA")]
    [InlineData(typeof(ResumoDominioPeriodoFormaPagto), "VWRESUMODOMINIOPERIODOFORMAPAGTO")]
    public void View_DeveSerMapeadaSemChaveESemTabela(Type entidade, string view)
    {
        // Modelo do provedor SQL Server (sem conexão): o InMemory não aplica as convenções
        // relacionais e devolveria o nome de tabela por convenção mesmo com ToView.
        using var ctx = new GestaoFinanceiraDbContext(new DbContextOptionsBuilder<GestaoFinanceiraDbContext>()
            .UseSqlServer("Server=nao-conecta;Database=nao-conecta").Options);
        var tipo = ctx.Model.FindEntityType(entidade)!;

        tipo.GetViewName().Should().Be(view);
        tipo.GetTableName().Should().BeNull("view não pode virar tabela na migration");
        tipo.FindPrimaryKey().Should().BeNull();
    }

    [Fact]
    public void ViewHierarquia_FlagsIntDaView_DevemConverterBoolParaInt()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());
        var tipo = ctx.Model.FindEntityType(typeof(DominioUsuarioSuprimentoSangria))!;

        // DOMINIOPAIPRINCIPAL, DOMINIOPAI e CAIXA são int na view; USUARIOPRINCIPALDOMINIO e ATIVO, smallint.
        foreach (var flag in new[] { nameof(DominioUsuarioSuprimentoSangria.DominioPaiPrincipal),
                     nameof(DominioUsuarioSuprimentoSangria.DominioPai), nameof(DominioUsuarioSuprimentoSangria.Caixa) })
        {
            tipo.FindProperty(flag)!.GetTypeMapping().Converter!.ProviderClrType.Should().Be(typeof(int), flag);
        }

        tipo.FindProperty(nameof(DominioUsuarioSuprimentoSangria.Ativo))!.GetTypeMapping().Converter!
            .ProviderClrType.Should().Be(typeof(short));
    }

    [Fact]
    public void ViewResumo_CreditoEDebito_DevemTerPrecisao38Escala2()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());
        var tipo = ctx.Model.FindEntityType(typeof(ResumoDominioPeriodoFormaPagto))!;

        foreach (var valor in new[] { nameof(ResumoDominioPeriodoFormaPagto.Credito), nameof(ResumoDominioPeriodoFormaPagto.Debito) })
        {
            var prop = tipo.FindProperty(valor)!;
            prop.GetPrecision().Should().Be(38, valor);
            prop.GetScale().Should().Be(2, valor);
        }
    }

    [Fact]
    public void View_SemChave_NaoPodeSerRastreadaParaEscrita()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());

        var acao = () => ctx.ResumosDominioPeriodoFormaPagto.Add(new ResumoDominioPeriodoFormaPagto { IdFilial = 10, IdDominioPeriodo = 1 });

        acao.Should().Throw<InvalidOperationException>();
    }
}

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoFinanceira.Domain.Bancos;
using Versatus.GestaoFinanceira.Infrastructure;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.E3;

/// <summary>
/// E3-T04 — DbSets do épico E3 + teste de mapeamento (InMemory). Cobre E3-T03: tabela e
/// ordem física das PKs compostas (analysis/E3-caixa-banco.md §2), materialização,
/// agregado CaixaBanco→Usuarios, shadow properties [E14] de ContaBancaria e SaldoRateio
/// sem chave (DÚVIDA-E3-1).
/// </summary>
public class MapeamentoE3Tests
{
    private static GestaoFinanceiraDbContext NovoContexto(string banco) =>
        new(new DbContextOptionsBuilder<GestaoFinanceiraDbContext>().UseInMemoryDatabase(banco).Options);

    private static CaixaBanco NovoCaixaBanco(int id, int filial) => new()
    {
        IdCaixaBanco = id,
        IdFilial = filial,
        Descricao = $"Conta {id}/{filial}",
        TipoConta = ContaTipo.Banco,
        Ativo = true,
    };

    [Theory]
    [InlineData("CaixasBanco", typeof(CaixaBanco))]
    [InlineData("CaixaBancoUsuarios", typeof(CaixaBancoUsuario))]
    [InlineData("ContasBancarias", typeof(ContaBancaria))]
    [InlineData("SaldosCaixaBanco", typeof(SaldoCaixaBanco))]
    [InlineData("SaldosRateio", typeof(SaldoRateio))]
    [InlineData("Cobradores", typeof(Cobrador))]
    public void DbContext_DeveExporDbSetDaEntidadeE3(string propriedade, Type entidade)
    {
        var prop = typeof(GestaoFinanceiraDbContext).GetProperty(propriedade);

        prop.Should().NotBeNull($"o DbSet {propriedade} deve estar registrado");
        prop!.PropertyType.Should().Be(typeof(DbSet<>).MakeGenericType(entidade));
    }

    [Theory]
    [InlineData(typeof(CaixaBanco), "FINCAIXABANCO", new[] { "IDFINCAIXABANCO", "IDGLOFILIAL" })]
    [InlineData(typeof(CaixaBancoUsuario), "FINCAIXABANCOUSUARIO", new[] { "IDFINCAIXABANCO", "IDGLOFILIAL", "IDGLOUSUARIO" })]
    [InlineData(typeof(ContaBancaria), "FINCONTABANCARIA", new[] { "IDFINCAIXABANCO", "IDGLOFILIAL" })]
    [InlineData(typeof(SaldoCaixaBanco), "FINSALDOCAIXABANCO", new[] { "IDFINCAIXABANCO", "IDGLOFILIAL", "DATASALDO" })]
    [InlineData(typeof(Cobrador), "FINCOBRADOR", new[] { "IDFINCOBRADOR", "IDGLOFILIAL" })]
    public void Mapeamento_DeveUsarTabelaEPkNaOrdemFisica(Type entidade, string tabela, string[] colunasPk)
    {
        using var ctx = NovoContexto(nameof(Mapeamento_DeveUsarTabelaEPkNaOrdemFisica));
        var tipo = ctx.Model.FindEntityType(entidade)!;

        tipo.GetTableName().Should().Be(tabela);
        tipo.FindPrimaryKey()!.Properties.Select(p => p.GetColumnName())
            .Should().Equal(colunasPk);
    }

    [Fact]
    public async Task CaixaBanco_DevePersistirEMaterializarComUsuarios()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            var caixa = NovoCaixaBanco(1, 10);
            caixa.Saldo = 1234.56789012m;
            caixa.AdicionarUsuario(new CaixaBancoUsuario { IdCaixaBanco = 1, IdFilial = 10, IdUsuario = 7 });
            caixa.AdicionarUsuario(new CaixaBancoUsuario { IdCaixaBanco = 1, IdFilial = 10, IdUsuario = 8 });
            ctx.CaixasBanco.Add(caixa);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var lido = await ctx.CaixasBanco.Include(x => x.Usuarios).SingleAsync();

            lido.Descricao.Should().Be("Conta 1/10");
            lido.TipoConta.Should().Be(ContaTipo.Banco);
            lido.Saldo.Should().Be(1234.56789012m);
            lido.Usuarios.Select(u => u.IdUsuario).Should().BeEquivalentTo([7, 8]);
            (await ctx.CaixaBancoUsuarios.CountAsync()).Should().Be(2);
        }
    }

    [Fact]
    public async Task CaixaBanco_MesmoIdEmFiliaisDiferentes_DeveCoexistir()
    {
        await using var ctx = NovoContexto(Guid.NewGuid().ToString());
        ctx.CaixasBanco.AddRange(NovoCaixaBanco(1, 10), NovoCaixaBanco(1, 20));
        await ctx.SaveChangesAsync();

        (await ctx.CaixasBanco.CountAsync()).Should().Be(2);
    }

    [Fact]
    public void CaixaBanco_PkCompostaDuplicada_DeveSerRejeitada()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());
        ctx.CaixasBanco.Add(NovoCaixaBanco(1, 10));

        var acao = () => ctx.CaixasBanco.Add(NovoCaixaBanco(1, 10));

        acao.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task ContaBancaria_DevePersistirNucleoEGravarColunasE14ComoFalse()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            ctx.CaixasBanco.Add(NovoCaixaBanco(1, 10));
            ctx.ContasBancarias.Add(new ContaBancaria
            {
                IdCaixaBanco = 1,
                IdFilial = 10,
                IdAgencia = 55,
                NumeroConta = "12345",
                DigitoConta = "9",
                Limite = 5000.12345678m,
                PermiteEmitirCheque = true,
                ContaBancariaTipo = TipoContaBancaria.Investimento,
            });
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var conta = await ctx.ContasBancarias.SingleAsync();

            conta.NumeroConta.Should().Be("12345");
            conta.Limite.Should().Be(5000.12345678m);
            conta.PermiteEmitirCheque.Should().BeTrue();
            conta.ContaBancariaTipo.Should().Be(TipoContaBancaria.Investimento);

            // Colunas NOT NULL de boleto/remessa ([E14], shadow) + ENVIARSPED (propriedade real desde
            // o E3-T05) — default do construtor legado (false).
            foreach (var coluna in new[] { "GeraBoleto", "GeraRemessa", "ProcessaRetorno",
                         "BoletoBeneficiarioDiferente", "BoletoSacadoAvalista", "EnviarSped" })
            {
                ctx.Entry(conta).Property<bool>(coluna).CurrentValue.Should().BeFalse(coluna);
            }
        }
    }

    [Fact]
    public async Task SaldoCaixaBanco_DataSaldoFazParteDaPk()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            ctx.CaixasBanco.Add(NovoCaixaBanco(1, 10));
            ctx.SaldosCaixaBanco.AddRange(
                new SaldoCaixaBanco { IdCaixaBanco = 1, IdFilial = 10, DataSaldo = new DateTime(2026, 9, 1), SaldoAnterior = 100m, TotalCredito = 50m, TotalDebito = 20m },
                new SaldoCaixaBanco { IdCaixaBanco = 1, IdFilial = 10, DataSaldo = new DateTime(2026, 9, 2), SaldoAnterior = 130m, Conferido = true });
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var saldos = await ctx.SaldosCaixaBanco.OrderBy(s => s.DataSaldo).ToListAsync();

            saldos.Should().HaveCount(2);
            saldos[0].TotalCredito.Should().Be(50m);
            saldos[1].Conferido.Should().BeTrue();
        }
    }

    [Fact]
    public async Task Cobrador_DevePersistirEMaterializar()
    {
        var banco = Guid.NewGuid().ToString();
        await using (var ctx = NovoContexto(banco))
        {
            ctx.Cobradores.Add(new Cobrador { IdCobrador = 3, IdFilial = 10, IdEntidade = 99, Nome = "Cobrador A", Ativo = true, IdMeioContato = 4 });
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = NovoContexto(banco))
        {
            var cobrador = await ctx.Cobradores.SingleAsync();

            cobrador.Nome.Should().Be("Cobrador A");
            cobrador.IdEntidade.Should().Be(99);
            cobrador.IdUsuario.Should().BeNull();
            cobrador.IdMeioContato.Should().Be(4);
        }
    }

    [Fact]
    public void SaldoRateio_DeveSerMapeadoSemChaveNaTabelaReal()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());
        var tipo = ctx.Model.FindEntityType(typeof(SaldoRateio))!;

        tipo.GetTableName().Should().Be("FINSALDORATEIO");
        tipo.FindPrimaryKey().Should().BeNull("FINSALDORATEIO não tem PRIMARY KEY no banco (DÚVIDA-E3-1)");
        tipo.FindProperty(nameof(SaldoRateio.IdClasse))!.IsNullable.Should().BeTrue();
        tipo.FindProperty(nameof(SaldoRateio.IdCentroCusto))!.IsNullable.Should().BeTrue();
        tipo.FindProperty(nameof(SaldoRateio.IdProjeto))!.IsNullable.Should().BeTrue();
    }

    [Fact]
    public void SaldoRateio_SemChave_NaoPodeSerRastreadoParaEscrita()
    {
        using var ctx = NovoContexto(Guid.NewGuid().ToString());

        var acao = () => ctx.SaldosRateio.Add(new SaldoRateio { IdFilial = 10, DataSaldo = new DateTime(2026, 9, 1) });

        acao.Should().Throw<InvalidOperationException>();
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoTributo.Domain.Classification;
using Versatus.GestaoTributo.Domain.Rules;
using Versatus.GestaoTributo.Domain.ICMS;
using Versatus.GestaoTributo.Infrastructure;
using Xunit;
using FluentAssertions;

namespace Versatus.GestaoTributo.Tests;

public class MappingIntegrationTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TributoDbContext _context;

    public MappingIntegrationTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TributoDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TributoDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task DeveSalvarERecuperarClassificacaoFiscalComFilhosETributos()
    {
        // Arrange
        var ncmPai = new ClassificacaoFiscal
        {
            IdClassificacaoFiscal = 100,
            IdSinteticoAnalitico = 1,
            Descricao = "Capítulo de Teste",
            Ncm = "84000000",
            Ativo = true
        };

        var ncmFilho = new ClassificacaoFiscal
        {
            IdClassificacaoFiscal = 101,
            IdClassificacaoFiscalPai = 100,
            IdSinteticoAnalitico = 2,
            Descricao = "Sub-item de Teste",
            Ncm = "84112233",
            Ativo = true
        };

        var tributo = new TributoFiscal
        {
            IdTributoFiscal = 10,
            Descricao = "ICMS",
            Sigla = "ICMS",
            IdTipoTributo = 1
        };

        var ncmTributo = new ClassificacaoFiscalTributo
        {
            IdClassificacaoFiscalTributo = 500,
            IdClassificacaoFiscal = 101,
            IdTributoFiscal = 10,
            UsaAliquota = true,
            Aliquota = 18.00m,
            PossuiAliquotaFilial = false
        };

        // Act
        _context.TributosFiscais.Add(tributo);
        _context.ClassificacoesFiscais.Add(ncmPai);
        _context.ClassificacoesFiscais.Add(ncmFilho);
        _context.ClassificacoesFiscaisTributos.Add(ncmTributo);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperado = await _context.ClassificacoesFiscais
            .Include(c => c.Pai)
            .Include(c => c.Tributos)
                .ThenInclude(t => t.TributoFiscal)
            .FirstOrDefaultAsync(c => c.IdClassificacaoFiscal == 101);

        recuperado.Should().NotBeNull();
        recuperado!.Ncm.Should().Be("84112233");
        recuperado.Pai.Should().NotBeNull();
        recuperado.Pai!.Descricao.Should().Be("Capítulo de Teste");

        recuperado.Tributos.Should().HaveCount(1);
        var tribRecuperado = recuperado.Tributos[0];
        tribRecuperado.Aliquota.Should().Be(18.00m);
        tribRecuperado.TributoFiscal.Should().NotBeNull();
        tribRecuperado.TributoFiscal!.Sigla.Should().Be("ICMS");
    }

    [Fact]
    public async Task DeveSalvarERecuperarCfopComHierarquia()
    {
        // Arrange
        var cfopPai = new Cfop
        {
            IdCfop = 5000,
            Descricao = "SAIDAS OU PRESTACOES DE SERVICOS PARA O ESTADO",
            IdSinteticoAnalitico = 1,
            Ativo = true,
            IdSequencial = 1,
            SubstituicaoIcms = false
        };

        var cfopFilho = new Cfop
        {
            IdCfop = 5102,
            IdCfopPai = 5000,
            Descricao = "Venda de mercadoria adquirida ou recebida de terceiros",
            IdSinteticoAnalitico = 2,
            Ativo = true,
            IdSequencial = 2,
            SubstituicaoIcms = false
        };

        // Act
        _context.Cfops.Add(cfopPai);
        _context.Cfops.Add(cfopFilho);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperado = await _context.Cfops
            .Include(c => c.Pai)
            .FirstOrDefaultAsync(c => c.IdCfop == 5102);

        recuperado.Should().NotBeNull();
        recuperado!.Descricao.Should().Be("Venda de mercadoria adquirida ou recebida de terceiros");
        recuperado.Pai.Should().NotBeNull();
        recuperado.Pai!.IdCfop.Should().Be(5000);
    }

    [Fact]
    public async Task DeveSalvarERecuperarCestComSegmentoENcm()
    {
        // Arrange
        var segmento = new CestSegmento
        {
            IdCestSegmento = 5,
            Codigo = "01",
            Descricao = "Autopeças",
            Ativo = true
        };

        var cest = new Cest
        {
            IdCest = 500,
            IdCestSegmento = 5,
            Codigo = "0100100",
            Descricao = "Amortecedores",
            Ativo = true,
            PercentualMva = 40.00m
        };

        var cestNcm = new CestNCM
        {
            IdCestNcm = 1,
            IdCest = 500,
            Ncm = "87088000"
        };

        // Act
        _context.CestSegmentos.Add(segmento);
        _context.Cests.Add(cest);
        _context.CestNcms.Add(cestNcm);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperadoCest = await _context.Cests
            .Include(c => c.Segmento)
            .FirstOrDefaultAsync(c => c.IdCest == 500);

        var recuperadoNcm = await _context.CestNcms
            .Include(c => c.Cest)
            .FirstOrDefaultAsync(n => n.IdCestNcm == 1);

        recuperadoCest.Should().NotBeNull();
        recuperadoCest!.Codigo.Should().Be("0100100");
        recuperadoCest.Segmento.Should().NotBeNull();
        recuperadoCest.Segmento!.Descricao.Should().Be("Autopeças");

        recuperadoNcm.Should().NotBeNull();
        recuperadoNcm!.Ncm.Should().Be("87088000");
        recuperadoNcm.Cest.Should().NotBeNull();
        recuperadoNcm.Cest!.Descricao.Should().Be("Amortecedores");
    }

    [Fact]
    public async Task DeveSalvarERecuperarTributacaoERegrasTributosComConfiguracoesEItensEspeciais()
    {
        // Arrange
        var tributo = new TributoFiscal
        {
            IdTributoFiscal = 20,
            Descricao = "IPI",
            Sigla = "IPI",
            IdTipoTributo = 2
        };

        var regraPai = new RegraTributo
        {
            IdRegraTributo = 10,
            IdTributo = 20,
            Descricao = "Regra Pai IPI",
            Ativo = true,
            IdSinteticoAnalitico = 1
        };

        var regraFilho = new RegraTributo
        {
            IdRegraTributo = 11,
            IdRegraTributoPai = 10,
            IdTributo = 20,
            Descricao = "Regra Filho IPI",
            Ativo = true,
            IdSinteticoAnalitico = 2
        };

        var situacao = new SituacaoTributaria
        {
            IdSituacaoTributaria = 1,
            IdTributoFiscal = 20,
            Descricao = "IPI Tributado",
            SituacaoTributariaCodigo = "50",
            Ativo = true
        };

        var config = new RegraTributoConfiguracao
        {
            IdRegraTributoConfiguracao = 100,
            IdRegraTributo = 11,
            VigenciaInicio = DateTime.Today.AddDays(-30),
            VigenciaFim = DateTime.Today.AddDays(30),
            IdDefinicaoTributaria = DefinicaoTributaria.IncidenciaNormal,
            Aliquota = 10.00m,
            PercentualBaseReduzida = 0.00m,
            IdComportamentoTributo = ComportamentoTributo.Destacado,
            IdSituacaoTributaria = 1,
            ConsideraPessoaFisica = true,
            ConsideraRegimeSimples = false
        };

        var regraEspecial = new RegraTributo
        {
            IdRegraTributo = 12,
            IdTributo = 20,
            Descricao = "Regra Especial IPI",
            Ativo = true,
            IdSinteticoAnalitico = 2
        };

        var rte = new RegraTributacaoEspecial
        {
            IdRegraTributoConfiguracao = 100,
            IdTributoFormula = 5,
            IdRegraTributoEspecial = 12,
            Ordem = 1
        };

        var tributacao = new Tributacao
        {
            IdTributacao = 30,
            Descricao = "Tributacao Geral IPI",
            Ativo = true,
            IdTributo = 20,
            IdRegraEntrada = 10,
            IdRegraSaida = 11,
            RegraTributacaoTipo = TipoRegraTributacao.Estado,
            BaseCalculoTipo = TipoBaseCalculo.ValorNota
        };

        // Act
        _context.TributosFiscais.Add(tributo);
        _context.RegrasTributos.AddRange(regraPai, regraFilho, regraEspecial);
        _context.SituacoesTributarias.Add(situacao);
        _context.RegrasTributosConfiguracoes.Add(config);
        _context.RegrasTributacoesEspeciais.Add(rte);
        _context.Tributacoes.Add(tributacao);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperadaConfig = await _context.RegrasTributosConfiguracoes
            .Include(c => c.RegraTributo)
            .Include(c => c.SituacaoTributaria)
            .Include(c => c.Itens)
                .ThenInclude(i => i.RegraEspecial)
            .FirstOrDefaultAsync(c => c.IdRegraTributoConfiguracao == 100);

        recuperadaConfig.Should().NotBeNull();
        recuperadaConfig!.Aliquota.Should().Be(10.00m);
        recuperadaConfig.RegraTributo.Should().NotBeNull();
        recuperadaConfig.RegraTributo!.Descricao.Should().Be("Regra Filho IPI");
        recuperadaConfig.SituacaoTributaria.Should().NotBeNull();
        recuperadaConfig.SituacaoTributaria!.SituacaoTributariaCodigo.Should().Be("50");
        recuperadaConfig.Itens.Should().HaveCount(1);
        recuperadaConfig.Itens[0].RegraEspecial.Should().NotBeNull();
        recuperadaConfig.Itens[0].RegraEspecial!.Descricao.Should().Be("Regra Especial IPI");

        var recuperadaTributacao = await _context.Tributacoes
            .Include(t => t.RegraEntrada)
            .Include(t => t.RegraSaida)
            .FirstOrDefaultAsync(t => t.IdTributacao == 30);

        recuperadaTributacao.Should().NotBeNull();
        recuperadaTributacao!.RegraEntrada.Should().NotBeNull();
        recuperadaTributacao.RegraEntrada!.Descricao.Should().Be("Regra Pai IPI");
        recuperadaTributacao.RegraSaida.Should().NotBeNull();
        recuperadaTributacao.RegraSaida!.Descricao.Should().Be("Regra Filho IPI");
    }

    [Fact]
    public async Task DeveSalvarERecuperarAplicacaoProdutoEAssociacoes()
    {
        // Arrange
        var tributo = new TributoFiscal
        {
            IdTributoFiscal = 30,
            Descricao = "PIS",
            Sigla = "PIS",
            IdTipoTributo = 3
        };

        var tributacao = new Tributacao
        {
            IdTributacao = 40,
            Descricao = "PIS Geral",
            Ativo = true,
            IdTributo = 30,
            RegraTributacaoTipo = TipoRegraTributacao.Nenhuma,
            BaseCalculoTipo = TipoBaseCalculo.ValorNota
        };

        var aplicacaoPai = new AplicacaoProduto
        {
            IdAplicacaoProduto = 200,
            Descricao = "Produtos Gerais",
            IdSinteticoAnalitico = 1,
            Ativo = true
        };

        var aplicacaoFilho = new AplicacaoProduto
        {
            IdAplicacaoProduto = 201,
            IdAplicacaoProdutoPai = 200,
            Descricao = "Bebidas Frias",
            IdSinteticoAnalitico = 2,
            Ativo = true,
            SufixoCFOP = "101",
            SubstituicaoICMS = true
        };

        var apt = new AplicacaoProdutoTributo
        {
            IdAplicacaoProduto = 201,
            IdTributo = 30,
            IdTributacao = 40
        };

        var ano = new AplicacaoNaturezaOperacao
        {
            IdAplicacaoNaturezaOperacao = 1,
            IdAplicacaoProduto = 201,
            IdNaturezaOperacao = 500
        };

        var aplicacaoEspecial = new AplicacaoProduto
        {
            IdAplicacaoProduto = 202,
            Descricao = "Bebidas Frias Especiais",
            IdSinteticoAnalitico = 2,
            Ativo = true
        };

        var ae = new AplicacaoEspecial
        {
            IdAplicacaoProduto = 201,
            IdTributoFormula = 8,
            SufixoEspecial = "202",
            IdAplicacaoProdutoEspecial = 202
        };

        // Act
        _context.TributosFiscais.Add(tributo);
        _context.Tributacoes.Add(tributacao);
        _context.AplicacoesProdutos.AddRange(aplicacaoPai, aplicacaoFilho, aplicacaoEspecial);
        _context.AplicacoesProdutosTributos.Add(apt);
        _context.AplicacoesNaturezasOperacoes.Add(ano);
        _context.AplicacoesEspeciais.Add(ae);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperada = await _context.AplicacoesProdutos
            .Include(ap => ap.Pai)
            .Include(ap => ap.Tributos)
                .ThenInclude(t => t.Tributo)
            .Include(ap => ap.Naturezas)
            .Include(ap => ap.Especiais)
                .ThenInclude(e => e.AplicacaoProdutoEspecial)
            .FirstOrDefaultAsync(ap => ap.IdAplicacaoProduto == 201);

        recuperada.Should().NotBeNull();
        recuperada!.SufixoCFOP.Should().Be("101");
        recuperada.Pai.Should().NotBeNull();
        recuperada.Pai!.Descricao.Should().Be("Produtos Gerais");

        recuperada.Tributos.Should().HaveCount(1);
        recuperada.Tributos[0].Tributo.Should().NotBeNull();
        recuperada.Tributos[0].Tributo!.Sigla.Should().Be("PIS");

        recuperada.Naturezas.Should().HaveCount(1);
        recuperada.Naturezas[0].IdNaturezaOperacao.Should().Be(500);

        recuperada.Especiais.Should().HaveCount(1);
        recuperada.Especiais[0].AplicacaoProdutoEspecial.Should().NotBeNull();
        recuperada.Especiais[0].AplicacaoProdutoEspecial!.Descricao.Should().Be("Bebidas Frias Especiais");
    }

    [Fact]
    public async Task DeveSalvarERecuperarGruposTributariosICMSEInventario()
    {
        // Arrange
        var inventario = new GrupoTributarioInventarioICMS
        {
            IdGrupoTributarioInventarioICMS = 10,
            Descricao = "Inventario Teste",
            Ativo = true,
            Substituicao = true
        };

        var icms = new GrupoTributarioICMS
        {
            IdGrupoTributarioICMS = 100,
            IdGrupoTributarioInventarioICMS = 10,
            Descricao = "Grupo ICMS Teste",
            Ativo = true,
            IdDefinicaoTributaria = DefinicaoTributaria.IncidenciaNormal
        };

        // Act
        _context.GruposTributariosInventarioICMS.Add(inventario);
        _context.GruposTributariosICMS.Add(icms);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperadoInventario = await _context.GruposTributariosInventarioICMS
            .Include(i => i.Grupos)
            .FirstOrDefaultAsync(i => i.IdGrupoTributarioInventarioICMS == 10);

        recuperadoInventario.Should().NotBeNull();
        recuperadoInventario!.Descricao.Should().Be("Inventario Teste");
        recuperadoInventario.Grupos.Should().HaveCount(1);
        recuperadoInventario.Grupos[0].Descricao.Should().Be("Grupo ICMS Teste");
    }

    [Fact]
    public async Task DeveSalvarERecuperarTributoIcmsSubstituicaoEstoque()
    {
        // Arrange
        var estoque = new TributoIcmsSubstituicaoEstoque
        {
            IdTributoIcmsSubstituicaoEstoque = 1,
            IdFilial = 10,
            IdEstEstoque = 20,
            IdEmpresa = 30,
            DataDocumento = DateTime.Today,
            Quantidade = 15.12345678m,
            ValorTotal = 150.98765432m,
            BaseCalculo = 120.5m,
            Aliquota = 18.00m,
            ValorTributo = 21.69m,
            AliquotaFcp = 2.0m,
            ValorFcp = 2.41m,
            BaseCalculoSt = 180.0m,
            AliquotaSt = 18.0m,
            ValorSt = 32.4m,
            AliquotaFcpSt = 2.0m,
            ValorFcpSt = 3.6m,
            IdOrigem = 1,
            IdProcessoOrigem = 2,
            Monofasico = false,
            IcmsSubstituidoAnterior = true,
            IdUsuarioInclusao = 1,
            DataInclusao = DateTime.Today,
            HoraInclusao = DateTime.Today
        };

        // Act
        _context.TributosIcmsSubstituicoesEstoque.Add(estoque);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperado = await _context.TributosIcmsSubstituicoesEstoque
            .FirstOrDefaultAsync(e => e.IdTributoIcmsSubstituicaoEstoque == 1);

        recuperado.Should().NotBeNull();
        recuperado!.Quantidade.Should().Be(15.12345678m);
        recuperado.ValorTotal.Should().Be(150.98765432m);
        recuperado.IcmsSubstituidoAnterior.Should().BeTrue();
    }

    [Fact]
    public async Task DeveSalvarERecuperarDetalhesUfECidadeTributacao()
    {
        // Arrange
        var tributo = new TributoFiscal
        {
            IdTributoFiscal = 40,
            Descricao = "ICMS Test",
            Sigla = "ICMS",
            IdTipoTributo = 1
        };

        var tributacao = new Tributacao
        {
            IdTributacao = 50,
            Descricao = "Tributacao Test",
            Ativo = true,
            IdTributo = 40,
            RegraTributacaoTipo = TipoRegraTributacao.Estado,
            BaseCalculoTipo = TipoBaseCalculo.ValorNota
        };

        var regraEntrada = new RegraTributo
        {
            IdRegraTributo = 20,
            IdTributo = 40,
            Descricao = "Regra Entrada Test",
            Ativo = true
        };

        var regraSaida = new RegraTributo
        {
            IdRegraTributo = 21,
            IdTributo = 40,
            Descricao = "Regra Saida Test",
            Ativo = true
        };

        var ufDet = new DetalheUfTributacao
        {
            IdDetalheUfTributacao = 1,
            UfOrigem = "SP",
            UfDestino = "RJ",
            IdTributacao = 50,
            IdRegraEntrada = 20,
            IdRegraSaida = 21,
            VigenciaInicio = DateTime.Today,
            Ativo = true,
            IdTipoBaseCalculo = TipoBaseCalculo.DifValorVendaCompra
        };

        var cidadeDet = new DetalheCidadeTributacao
        {
            IdDetalheCidadeTributacao = 1,
            IdTributacao = 50,
            IdRegraEntrada = 20,
            IdRegraSaida = 21,
            IdCidade = 3550308,
            VigenciaInicio = DateTime.Today,
            Ativo = true,
            IdTipoBaseCalculo = TipoBaseCalculo.DifCustoVendaCompra
        };

        // Act
        _context.TributosFiscais.Add(tributo);
        _context.Tributacoes.Add(tributacao);
        _context.RegrasTributos.AddRange(regraEntrada, regraSaida);
        _context.DetalhesUfTributacao.Add(ufDet);
        _context.DetalhesCidadeTributacao.Add(cidadeDet);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recuperadoTributacao = await _context.Tributacoes
            .Include(t => t.DetalhesUf)
                .ThenInclude(u => u.RegraEntrada)
            .Include(t => t.DetalhesCidade)
                .ThenInclude(c => c.RegraSaida)
            .FirstOrDefaultAsync(t => t.IdTributacao == 50);

        recuperadoTributacao.Should().NotBeNull();
        recuperadoTributacao!.DetalhesUf.Should().HaveCount(1);
        recuperadoTributacao.DetalhesUf[0].UfDestino.Should().Be("RJ");
        recuperadoTributacao.DetalhesUf[0].RegraEntrada.Should().NotBeNull();
        recuperadoTributacao.DetalhesUf[0].RegraEntrada!.Descricao.Should().Be("Regra Entrada Test");
        recuperadoTributacao.DetalhesUf[0].IdTipoBaseCalculo.Should().Be(TipoBaseCalculo.DifValorVendaCompra);

        recuperadoTributacao.DetalhesCidade.Should().HaveCount(1);
        recuperadoTributacao.DetalhesCidade[0].IdCidade.Should().Be(3550308);
        recuperadoTributacao.DetalhesCidade[0].RegraSaida.Should().NotBeNull();
        recuperadoTributacao.DetalhesCidade[0].RegraSaida!.Descricao.Should().Be("Regra Saida Test");
        recuperadoTributacao.DetalhesCidade[0].IdTipoBaseCalculo.Should().Be(TipoBaseCalculo.DifCustoVendaCompra);
    }

    [Fact]
    public async Task DeveSalvarERecuperarRegimesSimplesEPartilha()
    {
        // Arrange
        var regime = new RegimeTributarioVigencia
        {
            IdRegimeTributarioVigencia = 1,
            IdFilial = 2,
            IdIncidenciaTributaria = 1,
            IdApropriacaoCredito = 2,
            IdTipoContribuicaoApurada = 3,
            IdRegimeEscrituracaoApuracaoPresumido = 4,
            VigenciaInicio = DateTime.Today,
            Descricao = "Regime Presumido",
            IdEscrituracaoNfeEcf = 5,
            EnviarPlanoContabil = true,
            IdApuracaoContribuicaoPrevidenciaria = 6
        };

        var simples = new SimplesNacional
        {
            IdSimplesNacional = 1,
            Descricao = "Simples Faixa 1",
            Aliquota = 4.0000m,
            Ativo = true
        };

        var tributo = new TributoFiscal
        {
            IdTributoFiscal = 50,
            Descricao = "ISS Test",
            Sigla = "ISS",
            IdTipoTributo = 4
        };

        var simplesTrib = new SimplesNacionalTributo
        {
            IdSimplesNacionalTributo = 1,
            IdSimplesNacional = 1,
            IdTributoFiscal = 50,
            Aliquota = 2.0100m
        };

        var partilha = new PartilhaICMSVigencia
        {
            IdPartilhaICMSVigencia = 1,
            Descricao = "Partilha 2026",
            VigenciaInicio = DateTime.Today,
            PercentualPartilhaICMS = 80.0000m
        };

        // Act
        _context.RegimesTributariosVigencias.Add(regime);
        _context.TributosFiscais.Add(tributo);
        _context.SimplesNacional.Add(simples);
        _context.SimplesNacionalTributos.Add(simplesTrib);
        _context.PartilhasICMSVigencias.Add(partilha);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var recRegime = await _context.RegimesTributariosVigencias
            .FirstOrDefaultAsync(r => r.IdRegimeTributarioVigencia == 1);
        recRegime.Should().NotBeNull();
        recRegime!.Descricao.Should().Be("Regime Presumido");
        recRegime.EnviarPlanoContabil.Should().BeTrue();

        var recSimples = await _context.SimplesNacional
            .Include(s => s.Tributos)
                .ThenInclude(t => t.TributoFiscal)
            .FirstOrDefaultAsync(s => s.IdSimplesNacional == 1);
        recSimples.Should().NotBeNull();
        recSimples!.Aliquota.Should().Be(4.0000m);
        recSimples.Tributos.Should().HaveCount(1);
        recSimples.Tributos[0].Aliquota.Should().Be(2.0100m);
        recSimples.Tributos[0].TributoFiscal.Should().NotBeNull();
        recSimples.Tributos[0].TributoFiscal!.Sigla.Should().Be("ISS");

        var recPartilha = await _context.PartilhasICMSVigencias
            .FirstOrDefaultAsync(p => p.IdPartilhaICMSVigencia == 1);
        recPartilha.Should().NotBeNull();
        recPartilha!.PercentualPartilhaICMS.Should().Be(80.0000m);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}

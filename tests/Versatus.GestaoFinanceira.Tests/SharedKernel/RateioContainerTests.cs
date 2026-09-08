using FluentAssertions;
using Versatus.SharedKernel.Enums;
using Versatus.SharedKernel.Rateio;

namespace Versatus.GestaoFinanceira.Tests.SharedKernel;

/// <summary>
/// Testes do acumulador de rateio (<see cref="RateioContainer"/>, criado em E0-T03).
/// Cobrem o que é escopo do SharedKernel: acumulação e somas. O motor de rateio é do
/// MOD-02 (CLR-01) e não é testado aqui.
/// </summary>
public class RateioContainerTests
{
    [Fact]
    public void Novo_container_esta_vazio()
    {
        var c = new RateioContainer();
        c.Vazio.Should().BeTrue();
        c.Itens.Should().BeEmpty();
    }

    [Fact]
    public void Add_curto_usa_IdPaiMaster_menos_um()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.CentroCusto, id: 10, idPai: 0, valor: 100m, idFilialOrigem: 1);

        c.Vazio.Should().BeFalse();
        var item = c.Itens.Single();
        item.Tipo.Should().Be(TipoRateioItem.CentroCusto);
        item.Id.Should().Be(10);
        item.IdPai.Should().Be(0);
        item.IdPaiMaster.Should().Be(-1);
        item.IdFilialOrigem.Should().Be(1);
        item.Valor.Should().Be(100m);
    }

    [Fact]
    public void Add_completo_preserva_IdPaiMaster()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Projeto, id: 7, idPaiMaster: 3, idPai: 5, valor: 50m, idFilialOrigem: 2);

        var item = c.Itens.Single();
        item.IdPaiMaster.Should().Be(3);
        item.IdPai.Should().Be(5);
    }

    [Fact]
    public void ValorBase_soma_apenas_os_itens_da_mesma_dimensao_e_no()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Classe, id: 1, idPai: 0, valor: 60m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 2, idPai: 0, valor: 40m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 3, idPai: 99, valor: 1000m, idFilialOrigem: 1);   // outro nó
        c.Add(TipoRateioItem.CentroCusto, id: 1, idPai: 0, valor: 500m, idFilialOrigem: 1); // outra dimensão

        c.ValorBase(TipoRateioItem.Classe, idPai: 0, idPaiMaster: 0).Should().Be(100m);
    }

    [Fact]
    public void ValorBase_ignora_IdPaiMaster_quando_o_item_foi_adicionado_curto()
    {
        var c = new RateioContainer();
        // itens "curtos" -> IdPaiMaster = -1 -> a regra do legado ignora o idPaiMaster passado
        c.Add(TipoRateioItem.CentroCusto, id: 1, idPai: 0, valor: 30m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.CentroCusto, id: 2, idPai: 0, valor: 70m, idFilialOrigem: 1);

        c.ValorBase(TipoRateioItem.CentroCusto, idPai: 0, idPaiMaster: 12345).Should().Be(100m);
    }

    [Fact]
    public void ValorPorId_soma_apenas_o_item_pedido()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Classe, id: 1, idPai: 0, valor: 30m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 1, idPai: 0, valor: 20m, idFilialOrigem: 2); // mesmo Id, outra filial
        c.Add(TipoRateioItem.Classe, id: 2, idPai: 0, valor: 50m, idFilialOrigem: 1);

        c.ValorPorId(TipoRateioItem.Classe, id: 1, idPai: 0, idPaiMaster: 0).Should().Be(50m);
    }

    [Fact]
    public void IdsDistintos_retorna_na_ordem_de_primeira_ocorrencia()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Classe, id: 5, idPai: 0, valor: 10m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 2, idPai: 0, valor: 10m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 5, idPai: 0, valor: 10m, idFilialOrigem: 1);

        c.IdsDistintos(TipoRateioItem.Classe, idPai: 0, idPaiMaster: 0)
            .Should().Equal(5, 2);
    }

    [Fact]
    public void Percentual_replica_o_calculo_do_legado()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Classe, id: 1, idPai: 0, valor: 25m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 2, idPai: 0, valor: 75m, idFilialOrigem: 1);

        c.Percentual(TipoRateioItem.Classe, id: 1, idPai: 0, idPaiMaster: 0).Should().Be(25m);
        c.Percentual(TipoRateioItem.Classe, id: 2, idPai: 0, idPaiMaster: 0).Should().Be(75m);
    }

    [Fact]
    public void Percentual_e_zero_quando_a_base_e_zero()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Classe, id: 1, idPai: 0, valor: 50m, idFilialOrigem: 1);
        c.Add(TipoRateioItem.Classe, id: 2, idPai: 0, valor: -50m, idFilialOrigem: 1);

        c.ValorBase(TipoRateioItem.Classe, idPai: 0, idPaiMaster: 0).Should().Be(0m);
        c.Percentual(TipoRateioItem.Classe, id: 1, idPai: 0, idPaiMaster: 0).Should().Be(0m);
    }

    [Fact]
    public void Limpar_esvazia_o_acumulador()
    {
        var c = new RateioContainer();
        c.Add(TipoRateioItem.Classe, id: 1, idPai: 0, valor: 10m, idFilialOrigem: 1);
        c.Limpar();
        c.Vazio.Should().BeTrue();
    }
}

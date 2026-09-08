using FluentAssertions;
using Versatus.SharedKernel.Enums;

namespace Versatus.GestaoFinanceira.Tests.SharedKernel;

/// <summary>
/// Paridade 1:1 dos valores inteiros dos enums de <c>Versatus.SharedKernel.Enums</c> com o
/// legado (<c>projeto_tag_1906/geral/{tipoenumerado.cs, TipoEnumeradoObjeto.cs}</c>),
/// conforme <c>specs/modulos/MOD-05/enums.md</c>. Para os enums PERSISTIDOS, uma divergência
/// aqui corrompe dado histórico já gravado — este é o gate de E0-T02/E0-T04.
/// 1 <c>[Fact]</c> por enum.
/// </summary>
public class EnumValoresTests
{
    // ---------------------------------------------------------------------
    // Tabela A — enums persistidos ([TipoEnumerado(idPai)] no legado)
    // ---------------------------------------------------------------------

    [Fact]
    public void SituacaoDocumento_bate_com_o_legado()
    {
        ((int)SituacaoDocumento.Aberto).Should().Be(83);
        ((int)SituacaoDocumento.Liquidado).Should().Be(85);
        ((int)SituacaoDocumento.LiquidadoParcial).Should().Be(221);
        ((int)SituacaoDocumento.Cancelado).Should().Be(281);
    }

    [Fact]
    public void SituacaoMovimento_bate_com_o_legado()
    {
        ((int)SituacaoMovimento.Normal).Should().Be(288);
        ((int)SituacaoMovimento.Cancelado).Should().Be(289);
    }

    [Fact]
    public void ChequeRecebidoSituacao_bate_com_o_legado()
    {
        ((int)ChequeRecebidoSituacao.Aberto).Should().Be(216);
        ((int)ChequeRecebidoSituacao.Devolvido).Should().Be(217);
        ((int)ChequeRecebidoSituacao.Baixado).Should().Be(218);
        ((int)ChequeRecebidoSituacao.Negociado).Should().Be(235);
        ((int)ChequeRecebidoSituacao.Cancelado).Should().Be(286);
        ((int)ChequeRecebidoSituacao.Repassado).Should().Be(501);
        ((int)ChequeRecebidoSituacao.DevolucaoRepasse).Should().Be(605);
        ((int)ChequeRecebidoSituacao.Sacado).Should().Be(739);
    }

    [Fact]
    public void SituacaoTalaoCheque_bate_com_o_legado()
    {
        ((int)SituacaoTalaoCheque.Disponivel).Should().Be(188);
        ((int)SituacaoTalaoCheque.Cancelado).Should().Be(189);
        ((int)SituacaoTalaoCheque.Emitido).Should().Be(190);
        ((int)SituacaoTalaoCheque.Devolvido).Should().Be(498);
        ((int)SituacaoTalaoCheque.Negociado).Should().Be(499);
        ((int)SituacaoTalaoCheque.Sacado).Should().Be(740);
    }

    [Fact]
    public void SituacaoComissaoLancto_bate_com_o_legado()
    {
        ((int)SituacaoComissaoLancto.Previsto).Should().Be(432);
        ((int)SituacaoComissaoLancto.Efetivado).Should().Be(433);
        ((int)SituacaoComissaoLancto.Revertido).Should().Be(434);
        ((int)SituacaoComissaoLancto.Cancelado).Should().Be(435);
        ((int)SituacaoComissaoLancto.EfetivadoParcial).Should().Be(532);
    }

    [Fact]
    public void SituacaoDistribuicao_bate_com_o_legado()
    {
        ((int)SituacaoDistribuicao.Pendente).Should().Be(387);
        ((int)SituacaoDistribuicao.Atendido).Should().Be(388);
        ((int)SituacaoDistribuicao.AtendidoParcial).Should().Be(389);
        ((int)SituacaoDistribuicao.Cancelado).Should().Be(436);
        ((int)SituacaoDistribuicao.Devolvido).Should().Be(497);
    }

    [Fact]
    public void ProcessoOrigem_bate_com_o_legado()
    {
        ((int)ProcessoOrigem.Venda).Should().Be(130);
        ((int)ProcessoOrigem.Compra).Should().Be(131);
        ((int)ProcessoOrigem.Liquidacao).Should().Be(132);
        ((int)ProcessoOrigem.Documento).Should().Be(133);
        ((int)ProcessoOrigem.ChequeRecebido).Should().Be(134);
        ((int)ProcessoOrigem.MovimentoChequeRecebido).Should().Be(135);
        ((int)ProcessoOrigem.CaixaBanco).Should().Be(196);
        ((int)ProcessoOrigem.DominioPeriodoLancto).Should().Be(292);
        ((int)ProcessoOrigem.Estorno).Should().Be(296);
        ((int)ProcessoOrigem.EntidadeMovimento).Should().Be(295);
        ((int)ProcessoOrigem.Reversao).Should().Be(294);
        ((int)ProcessoOrigem.MovimentoEstoque).Should().Be(335);
        ((int)ProcessoOrigem.LancamentoComissao).Should().Be(420);
        ((int)ProcessoOrigem.FechamentoComissao).Should().Be(421);
        ((int)ProcessoOrigem.MovimentoRateio).Should().Be(440);
        ((int)ProcessoOrigem.CancelamentoVendaCompra).Should().Be(469);
        ((int)ProcessoOrigem.Faturamento).Should().Be(476);
        ((int)ProcessoOrigem.ImplantacaoEstoque).Should().Be(478);
        ((int)ProcessoOrigem.DevolucaoVendaCompra).Should().Be(479);
        ((int)ProcessoOrigem.EstornoDevolucao).Should().Be(489);
        ((int)ProcessoOrigem.MovimentoChequeEmitido).Should().Be(500);
        ((int)ProcessoOrigem.AtendimentoRequisicao).Should().Be(581);
        ((int)ProcessoOrigem.DevolucaoRequisicao).Should().Be(582);
        ((int)ProcessoOrigem.FechamentoFolha).Should().Be(583);
        ((int)ProcessoOrigem.MovimentoConsignacao).Should().Be(627);
        ((int)ProcessoOrigem.CancelamentoDocumentoFinanceiro).Should().Be(644);
        ((int)ProcessoOrigem.OrdemServico).Should().Be(662);
        ((int)ProcessoOrigem.Adiantamento).Should().Be(691);
        ((int)ProcessoOrigem.AcertoAdiantamento).Should().Be(692);
        ((int)ProcessoOrigem.OrdemExpedicao).Should().Be(694);
        ((int)ProcessoOrigem.MovimentoVeiculo).Should().Be(695);
        ((int)ProcessoOrigem.Abastecimento).Should().Be(696);
        ((int)ProcessoOrigem.DistribuicaoAcerto).Should().Be(700);
        ((int)ProcessoOrigem.FechamentoOsFrota).Should().Be(716);
        ((int)ProcessoOrigem.CancelamentoOsFrota).Should().Be(717);
        ((int)ProcessoOrigem.RomaneioArmazem).Should().Be(718);
        ((int)ProcessoOrigem.AtendimentoRequisicaoObra).Should().Be(720);
        ((int)ProcessoOrigem.AtendimentoDevolucaoObra).Should().Be(721);
        ((int)ProcessoOrigem.DespesaVeiculoFrota).Should().Be(878);
        ((int)ProcessoOrigem.MovimentoEstoqueFiscal).Should().Be(882);
        ((int)ProcessoOrigem.DespesaVeiculoGaragem).Should().Be(1438);
        ((int)ProcessoOrigem.CancelamentoMovtoFinanceiroFrota).Should().Be(905);
        ((int)ProcessoOrigem.CancelamentoMovtoFinanceiroGaragem).Should().Be(906);
        ((int)ProcessoOrigem.ClienteFilial).Should().Be(1106);
        ((int)ProcessoOrigem.LiquidacaoContraPartida).Should().Be(1195);
        ((int)ProcessoOrigem.TransacaoFilial).Should().Be(1237);
        ((int)ProcessoOrigem.TicketPesagem).Should().Be(1297);
        ((int)ProcessoOrigem.Contrato).Should().Be(1382);
        ((int)ProcessoOrigem.Matricula).Should().Be(1406);
        ((int)ProcessoOrigem.CancelamentoContrato).Should().Be(1422);
        ((int)ProcessoOrigem.Transporte).Should().Be(1491);
        ((int)ProcessoOrigem.MovimentoProducao).Should().Be(1519);
        ((int)ProcessoOrigem.MovimentoItemProducao).Should().Be(1520);
        ((int)ProcessoOrigem.CancelamentoMovimentoProducao).Should().Be(1521);
        ((int)ProcessoOrigem.CancelamentoMovimentoItemProducao).Should().Be(1522);
        ((int)ProcessoOrigem.MDFeDocumento).Should().Be(1576);
        ((int)ProcessoOrigem.CancelamentoTransporte).Should().Be(1776);
        ((int)ProcessoOrigem.MovimentoContrato).Should().Be(1812);
        ((int)ProcessoOrigem.NotaFiscalServico).Should().Be(1850);
        ((int)ProcessoOrigem.AjusteMovimentoContratoQuantidade).Should().Be(1854);
        ((int)ProcessoOrigem.AjusteMovimentoContratoFinanceiro).Should().Be(1855);
        ((int)ProcessoOrigem.ICMSSubstituicaoEstoque).Should().Be(1906);
    }

    [Fact]
    public void PagarReceberTipo_bate_com_o_legado()
    {
        ((int)PagarReceberTipo.Pagar).Should().Be(172);
        ((int)PagarReceberTipo.Receber).Should().Be(173);
        ((int)PagarReceberTipo.MovimentoCartao).Should().Be(1192);
    }

    [Fact]
    public void NaturezaTipo_bate_com_o_legado()
    {
        ((int)NaturezaTipo.Credora).Should().Be(30);
        ((int)NaturezaTipo.Devedora).Should().Be(31);
    }

    [Fact]
    public void RegimeRateioTipo_bate_com_o_legado()
    {
        ((int)RegimeRateioTipo.Economico).Should().Be(185);
        ((int)RegimeRateioTipo.Financeiro).Should().Be(186);
    }

    [Fact]
    public void ContaTipo_bate_com_o_legado()
    {
        ((int)ContaTipo.Caixa).Should().Be(175);
        ((int)ContaTipo.Banco).Should().Be(176);
    }

    [Fact]
    public void TipoContaCaixa_bate_com_o_legado()
    {
        ((int)TipoContaCaixa.Normal).Should().Be(1482);
        ((int)TipoContaCaixa.Cofre).Should().Be(1483);
    }

    [Fact]
    public void TipoContaBancaria_bate_com_o_legado()
    {
        ((int)TipoContaBancaria.ContaCorrente).Should().Be(1479);
        ((int)TipoContaBancaria.Investimento).Should().Be(1480);
    }

    [Fact]
    public void ContaFinanceiroTipo_bate_com_o_legado()
    {
        ((int)ContaFinanceiroTipo.CentroCusto).Should().Be(178);
        ((int)ContaFinanceiroTipo.Classe).Should().Be(179);
        ((int)ContaFinanceiroTipo.Projeto).Should().Be(180);
        ((int)ContaFinanceiroTipo.PlanoConta).Should().Be(181);
        ((int)ContaFinanceiroTipo.Caixa).Should().Be(182);
        ((int)ContaFinanceiroTipo.Banco).Should().Be(183);
    }

    [Fact]
    public void TipoDocumentoMovimento_bate_com_o_legado()
    {
        ((int)TipoDocumentoMovimento.Liquidacao).Should().Be(254);
        ((int)TipoDocumentoMovimento.Estorno).Should().Be(255);
    }

    [Fact]
    public void TipoFormaLancamento_bate_com_o_legado()
    {
        ((int)TipoFormaLancamento.FormaPagamento).Should().Be(259);
        ((int)TipoFormaLancamento.Troco).Should().Be(260);
    }

    [Fact]
    public void RegistroDocumentoTipo_bate_com_o_legado()
    {
        ((int)RegistroDocumentoTipo.Normal).Should().Be(223);
        ((int)RegistroDocumentoTipo.JuroCapitalizado).Should().Be(224);
        ((int)RegistroDocumentoTipo.BoletoAgrupado).Should().Be(225);
        ((int)RegistroDocumentoTipo.Reversao).Should().Be(226);
        ((int)RegistroDocumentoTipo.RevertidoAgrupado).Should().Be(227);
        ((int)RegistroDocumentoTipo.Descontado).Should().Be(228);
    }

    [Fact]
    public void CalculoItemFinanceiro_bate_com_o_legado()
    {
        ((int)CalculoItemFinanceiro.Somar).Should().Be(239);
        ((int)CalculoItemFinanceiro.Subtrair).Should().Be(240);
        ((int)CalculoItemFinanceiro.MultiplicarSomar).Should().Be(241);
        ((int)CalculoItemFinanceiro.MultiplicarDiminuir).Should().Be(242);
        ((int)CalculoItemFinanceiro.DividirSomar).Should().Be(243);
        ((int)CalculoItemFinanceiro.DividirSubtrair).Should().Be(257);
        ((int)CalculoItemFinanceiro.PercentualSomar).Should().Be(244);
        ((int)CalculoItemFinanceiro.PercentualSubtrair).Should().Be(245);
    }

    [Fact]
    public void CalculoItemFinanceiroTipo_bate_com_o_legado()
    {
        ((int)CalculoItemFinanceiroTipo.Simples).Should().Be(147);
        ((int)CalculoItemFinanceiroTipo.Composto).Should().Be(148);
    }

    [Fact]
    public void ItemFinanceiroAplicar_bate_com_o_legado()
    {
        ((int)ItemFinanceiroAplicar.NaoAplicar).Should().Be(247);
        ((int)ItemFinanceiroAplicar.DepoisVencimento).Should().Be(248);
        ((int)ItemFinanceiroAplicar.AntesVencimento).Should().Be(249);
    }

    [Fact]
    public void ItemFinanceiroAplicacao_bate_com_o_legado()
    {
        ((int)ItemFinanceiroAplicacao.ValorCalculado).Should().Be(251);
        ((int)ItemFinanceiroAplicacao.ValorParcelado).Should().Be(252);
    }

    [Fact]
    public void OperacaoConversao_bate_com_o_legado()
    {
        ((int)OperacaoConversao.Nenhuma).Should().Be(266);
        ((int)OperacaoConversao.Multiplicacao).Should().Be(267);
        ((int)OperacaoConversao.Divisao).Should().Be(268);
    }

    [Fact]
    public void IndiceModoCorrecao_bate_com_o_legado()
    {
        ((int)IndiceModoCorrecao.UsaValorDia).Should().Be(210);
        ((int)IndiceModoCorrecao.UsarDataAnterior).Should().Be(106);
        ((int)IndiceModoCorrecao.UsarDataPosterior).Should().Be(107);
    }

    [Fact]
    public void IndiceTipoCorrecao_bate_com_o_legado()
    {
        ((int)IndiceTipoCorrecao.Diario).Should().Be(109);
        ((int)IndiceTipoCorrecao.Mensal).Should().Be(110);
    }

    [Fact]
    public void LanctoDominioPeriodoTipo_bate_com_o_legado()
    {
        ((int)LanctoDominioPeriodoTipo.Abertura).Should().Be(263);
        ((int)LanctoDominioPeriodoTipo.Fechamento).Should().Be(264);
        ((int)LanctoDominioPeriodoTipo.Suprimento).Should().Be(290);
        ((int)LanctoDominioPeriodoTipo.Sangria).Should().Be(291);
    }

    [Fact]
    public void TipoManutencaoRateio_bate_com_o_legado()
    {
        ((int)TipoManutencaoRateio.Financeiro).Should().Be(442);
        ((int)TipoManutencaoRateio.Estoque).Should().Be(443);
    }

    [Fact]
    public void AcaoBloqueio_bate_com_o_legado()
    {
        ((int)AcaoBloqueio.Bloquear).Should().Be(409);
        ((int)AcaoBloqueio.Desbloquear).Should().Be(410);
    }

    [Fact]
    public void TipoCartao_bate_com_o_legado()
    {
        ((int)TipoCartao.Debito).Should().Be(563);
        ((int)TipoCartao.Credito).Should().Be(564);
    }

    [Fact]
    public void TipoCalculoDRE_bate_com_o_legado()
    {
        ((int)TipoCalculoDRE.MovimentoRateio).Should().Be(1151);
        ((int)TipoCalculoDRE.Formula).Should().Be(1152);
        ((int)TipoCalculoDRE.Avulso).Should().Be(1153);
        ((int)TipoCalculoDRE.CustoVenda).Should().Be(1168);
        ((int)TipoCalculoDRE.CustoVendaTipoProduto).Should().Be(1166);
        ((int)TipoCalculoDRE.CustoVendaGrupoEstoque).Should().Be(1167);
    }

    [Fact]
    public void OperacaoTipo_bate_com_o_legado()
    {
        ((int)OperacaoTipo.Caixa).Should().Be(1);
        ((int)OperacaoTipo.Banco).Should().Be(2);
        ((int)OperacaoTipo.Pagar).Should().Be(3);
        ((int)OperacaoTipo.Receber).Should().Be(4);
        ((int)OperacaoTipo.LiquidacaoReceber).Should().Be(5);
        ((int)OperacaoTipo.LiquidacaoPagar).Should().Be(6);
        ((int)OperacaoTipo.ChequeRecebido).Should().Be(7);
        ((int)OperacaoTipo.MovimentacaoEstoque).Should().Be(8);
        ((int)OperacaoTipo.Venda).Should().Be(9);
        ((int)OperacaoTipo.Compra).Should().Be(10);
        ((int)OperacaoTipo.Folha).Should().Be(11);
        ((int)OperacaoTipo.EntradaVenda).Should().Be(12);
        ((int)OperacaoTipo.MovimentoCartao).Should().Be(13);
        ((int)OperacaoTipo.LiquidacaoMovimentoCartao).Should().Be(14);
        ((int)OperacaoTipo.SaidaCompra).Should().Be(15);
        ((int)OperacaoTipo.MovimentoProducao).Should().Be(16);
        ((int)OperacaoTipo.Transporte).Should().Be(17);
        ((int)OperacaoTipo.Contrato).Should().Be(18);
        ((int)OperacaoTipo.NotaFiscalServico).Should().Be(19);
    }

    // ---------------------------------------------------------------------
    // Enums persistidos já movidos de Versatus.AcessoGlobal (DEC-007 / PR #11)
    // ---------------------------------------------------------------------

    [Fact]
    public void FormaPagtoTipo_bate_com_o_legado()
    {
        ((int)FormaPagtoTipo.Dinheiro).Should().Be(122);
        ((int)FormaPagtoTipo.ChequeEmpresa).Should().Be(123);
        ((int)FormaPagtoTipo.ChequeCliente).Should().Be(124);
        ((int)FormaPagtoTipo.CartaoCredito).Should().Be(125);
        ((int)FormaPagtoTipo.CartaoDebito).Should().Be(126);
        ((int)FormaPagtoTipo.ParcelamentoProprio).Should().Be(127);
        ((int)FormaPagtoTipo.ParcelamentoFinanceira).Should().Be(128);
        ((int)FormaPagtoTipo.Credito).Should().Be(236);
        ((int)FormaPagtoTipo.CreditoPortador).Should().Be(237);
        ((int)FormaPagtoTipo.Deposito).Should().Be(256);
        ((int)FormaPagtoTipo.Outros).Should().Be(293);
        ((int)FormaPagtoTipo.Abatimento).Should().Be(483);
        ((int)FormaPagtoTipo.PixEstatico).Should().Be(1962);
        ((int)FormaPagtoTipo.PixDinamico).Should().Be(1963);
    }

    [Fact]
    public void CondicaoPagtoTipo_bate_com_o_legado()
    {
        ((int)CondicaoPagtoTipo.Parcelada).Should().Be(36);
        ((int)CondicaoPagtoTipo.FaixaDias).Should().Be(37);
        ((int)CondicaoPagtoTipo.Semanal).Should().Be(38);
    }

    [Fact]
    public void ParcelamentoArredondamento_bate_com_o_legado()
    {
        ((int)ParcelamentoArredondamento.Primeira).Should().Be(46);
        ((int)ParcelamentoArredondamento.Ultima).Should().Be(47);
    }

    [Fact]
    public void Disponibilidade_bate_com_o_legado()
    {
        ((int)Disponibilidade.Pagamento).Should().Be(56);
        ((int)Disponibilidade.Recebimento).Should().Be(57);
        ((int)Disponibilidade.Ambas).Should().Be(101);
    }

    [Fact]
    public void VencimentoTipo_bate_com_o_legado()
    {
        ((int)VencimentoTipo.Normal).Should().Be(59);
        ((int)VencimentoTipo.AntecipaDiaUtil).Should().Be(60);
        ((int)VencimentoTipo.ProrrogaDiaUtil).Should().Be(61);
    }

    [Fact]
    public void ParcelamentoTipo_bate_com_o_legado()
    {
        ((int)ParcelamentoTipo.DiaFixo).Should().Be(119);
        ((int)ParcelamentoTipo.DiasEntreParcela).Should().Be(120);
        ((int)ParcelamentoTipo.DiasUteis).Should().Be(693);
    }

    // ---------------------------------------------------------------------
    // Tabela B — enums não-persistidos (EnumeradoObjeto no legado)
    // ---------------------------------------------------------------------

    [Fact]
    public void PeriodoStatus_bate_com_o_legado()
    {
        ((int)PeriodoStatus.Aberto).Should().Be(1);
        ((int)PeriodoStatus.Fechado).Should().Be(2);
        ((int)PeriodoStatus.NaoAplicavel).Should().Be(3);
        ((int)PeriodoStatus.UsuarioSemPermissao).Should().Be(4);
        ((int)PeriodoStatus.PerfilSemPermissao).Should().Be(5);
    }

    [Fact]
    public void StatusDominioFinanceiro_bate_com_o_legado()
    {
        ((int)StatusDominioFinanceiro.Normal).Should().Be(1);
        ((int)StatusDominioFinanceiro.Abertura).Should().Be(2);
        ((int)StatusDominioFinanceiro.AberturaPadrao).Should().Be(3);
        ((int)StatusDominioFinanceiro.Fechamento).Should().Be(4);
        ((int)StatusDominioFinanceiro.FechamentoPadrao).Should().Be(5);
        ((int)StatusDominioFinanceiro.SuprimentoDinheiro).Should().Be(6);
        ((int)StatusDominioFinanceiro.SuprimentoCheque).Should().Be(7);
        ((int)StatusDominioFinanceiro.AtualizarDominio).Should().Be(8);
        ((int)StatusDominioFinanceiro.AtualizarDominioPadrao).Should().Be(9);
    }

    [Fact]
    public void TipoRateioItem_bate_com_o_legado_e_e_Flags()
    {
        ((int)TipoRateioItem.Nenhum).Should().Be(0);
        ((int)TipoRateioItem.Classe).Should().Be(1);
        ((int)TipoRateioItem.CentroCusto).Should().Be(2);
        ((int)TipoRateioItem.Projeto).Should().Be(4);
        typeof(TipoRateioItem).GetCustomAttributes(typeof(FlagsAttribute), false)
            .Should().NotBeEmpty("no legado o enum é [Flags]");
    }

    [Fact]
    public void TipoRateioItemValidacao_bate_com_o_legado()
    {
        ((int)TipoRateioItemValidacao.NaoValidar).Should().Be(0);
        ((int)TipoRateioItemValidacao.ValidarPercentual).Should().Be(1);
        ((int)TipoRateioItemValidacao.ValidarValor).Should().Be(2);
    }

    [Fact]
    public void TipoCalculoValorRateio_bate_com_o_legado()
    {
        ((int)TipoCalculoValorRateio.Valor).Should().Be(1);
        ((int)TipoCalculoValorRateio.Percentual).Should().Be(2);
        ((int)TipoCalculoValorRateio.NaoCalculo).Should().Be(3);
        ((int)TipoCalculoValorRateio.NaoCalculoPercentual).Should().Be(4);
        ((int)TipoCalculoValorRateio.ValorNaturezaInvertida).Should().Be(5);
        ((int)TipoCalculoValorRateio.PercentualDevedor).Should().Be(6);
        ((int)TipoCalculoValorRateio.PercentualNatureza).Should().Be(7);
        ((int)TipoCalculoValorRateio.PercentualNaturezaInvertida).Should().Be(8);
    }

    [Fact]
    public void TipoSaldo_bate_com_o_legado()
    {
        ((int)TipoSaldo.Inicial).Should().Be(0);
        ((int)TipoSaldo.Lancamento).Should().Be(1);
        ((int)TipoSaldo.Atual).Should().Be(2);
        ((int)TipoSaldo.Final).Should().Be(3);
    }

    [Fact]
    public void TipoProcessoComissao_bate_com_o_legado()
    {
        ((int)TipoProcessoComissao.Venda).Should().Be(1);
        ((int)TipoProcessoComissao.Documento).Should().Be(2);
        ((int)TipoProcessoComissao.Liquidacao).Should().Be(3);
        ((int)TipoProcessoComissao.Reversao).Should().Be(4);
        ((int)TipoProcessoComissao.CancelamentoVenda).Should().Be(5);
        ((int)TipoProcessoComissao.DevolucaoVenda).Should().Be(6);
        ((int)TipoProcessoComissao.NotaServicoFiscal).Should().Be(7);
    }

    [Fact]
    public void TipoHistoricoLiquidacaoEstorno_bate_com_o_legado()
    {
        ((int)TipoHistoricoLiquidacaoEstorno.Parcela).Should().Be(1);
        ((int)TipoHistoricoLiquidacaoEstorno.ParcelaQtde).Should().Be(2);
        ((int)TipoHistoricoLiquidacaoEstorno.Entidade).Should().Be(3);
        ((int)TipoHistoricoLiquidacaoEstorno.EntidadeRazao).Should().Be(4);
        ((int)TipoHistoricoLiquidacaoEstorno.EntidadeNomeRazao).Should().Be(5);
        ((int)TipoHistoricoLiquidacaoEstorno.Vencimento).Should().Be(6);
    }

    [Fact]
    public void DominioDestinoTalaoCheque_bate_com_o_legado()
    {
        ((int)DominioDestinoTalaoCheque.Proprio).Should().Be(1);
        ((int)DominioDestinoTalaoCheque.Padrao).Should().Be(2);
    }

    [Fact]
    public void AcaoLiberarDoctoPagar_bate_com_o_legado()
    {
        ((int)AcaoLiberarDoctoPagar.Liberar).Should().Be(1);
        ((int)AcaoLiberarDoctoPagar.Cancelar).Should().Be(2);
    }

    [Fact]
    public void AcaoAprovarDoctoPagar_bate_com_o_legado()
    {
        ((int)AcaoAprovarDoctoPagar.Aprovar).Should().Be(1);
        ((int)AcaoAprovarDoctoPagar.Cancelar).Should().Be(2);
    }

    [Fact]
    public void EntradaSaida_bate_com_o_legado()
    {
        ((int)EntradaSaida.Entrada).Should().Be(1);
        ((int)EntradaSaida.Saida).Should().Be(2);
    }

    [Fact]
    public void TipoLactoEditor_bate_com_o_legado()
    {
        ((int)TipoLactoEditor.Avulso).Should().Be(1);
        ((int)TipoLactoEditor.Cadastro).Should().Be(2);
    }
}

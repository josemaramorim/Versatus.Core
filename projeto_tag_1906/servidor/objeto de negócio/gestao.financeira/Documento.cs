using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using Gentle.Framework;
using Projeto.Language;
using Projeto.Geral;
using Projeto.Geral.Enumerado;
using Projeto.Geral.EnumeradoObjeto;
using Projeto.Servidor.Framework;
using Projeto.Servidor.Interface.Framework;
using Projeto.Servidor.Interface.Ambiente;
using Projeto.Servidor.Interface.NFSe;
using Projeto.Servidor.Interface.Faturamento; 
using Projeto.Servidor.Interface.GestaoFrota;
using Projeto.Servidor.Interface.AcessoGlobal;
using Projeto.Servidor.Interface.GestaoContrato;
using Projeto.Servidor.Interface.GestaoFinanceira;
using Projeto.Servidor.Interface.BaseDistribuicao;
using Projeto.Servidor.Interface.GestaoTransporte;
using Projeto.Servidor.ObjetosNegocio.AcessoGlobal;
using Projeto.Servidor.Strangler.Common;
using Projeto.Servidor.Strangler.GestaoFinanceira.DTOs;

namespace Projeto.Servidor.ObjetosNegocio.GestaoFinanceira
{
    /// <summary>
    /// Classe para o objeto de Negocio de Contas a Receber
    /// </summary>
    [TableName("FinDocumento"), AutoSequencial("IdDocumento", SequencialTipo.Filial)]
    public class Documento : DocumentoFinanceiroBase, IDocumento, IDadosComissao, ILanguageStrings
    {
        private int idDocumento;
        private bool documentoLiberado;
        private double valorLiquidado;
        private double valorRevertido;
        private double valorCancelado;
        private DateTime dataLiberacao;
        private DateTime horaLiberacao;
        private DateTime dataAlteracao;
        private DateTime horaAlteracao;
        private SituacaoDocumento idSituacaoDocumento;
        private DocumentoParcelaLista parcelaLista;
        private DocumentoTributoLista tributoLista;
        private DoctoItemFinanceiroLista itemFinanceiroLista;
        private IDocumentoComissionadoLista comissionadoLista;
        private Lookup usuarioLiberacao = new Lookup(typeof(IUsuario), "IdUsuario");
        private Lookup usuarioInclusao = new Lookup(typeof(IUsuario), "IdUsuario");
        private Lookup usuarioAlteracao = new Lookup(typeof(IUsuario), "IdUsuario");
        


        //Variáveis controle
        private double valorRateioCancelado = 0;
        private int indexCancelamento = -1;
        private bool carregandoParcelas;
        private bool refazerRateio;
        private bool cancelaDocumento = false;
        private bool msgErroExclusaoOutroProcesso = false;
        private Hashtable htParcelaPersistida = null;
        private IListBase parcelaCanceladaLista = null;
        private IContaBancaria contaBancaria;
        private bool validarProcessoOrigem = true;


        //Variaveis controle mov. cartao
        private IDocumentoCartao documentoCartao = null;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Documento()
        {
            this.idSituacaoDocumento = SituacaoDocumento.Aberto;
        }

        /// <summary>
        /// Construtor para carregar o objeto do banco
        /// </summary>        
        public Documento(int IdDocumento, int IdFilial, int IdEntidade, int IdOperacao, int IdTipoDocumento, int IdCondicaoPagamento, int IdIndiceEconomico, int IdIndiceConversao,
                         object IdUsuarioLiberacao, string NumeroDocumento, string Historico, bool DocumentoLiberado, double Valor, double ValorLiquidado, double ValorRevertido,
                         double ValorCancelado, double ValorParcelado, double ValorConvertido, DateTime DataEmissao, DateTime DataInclusao, DateTime HoraInclusao, DateTime DataLiberacao,
                         DateTime HoraLiberacao, PagarReceberTipo PagarReceber, SituacaoDocumento IdSituacao, string NumeroCedente, object IdUsuarioInclusao, object IdUsuarioAlteracao,
                         DateTime DataAlteracao, DateTime HoraAlteracao, int IdOrigem, ProcessoOrigem IdProcessoOrigem)

                        : base(IdFilial, IdEntidade, IdTipoDocumento, IdOperacao, IdCondicaoPagamento, IdIndiceEconomico, IdIndiceConversao, NumeroDocumento, NumeroCedente, Historico, Valor,
                               ValorConvertido, DataEmissao, DataInclusao, HoraInclusao, PagarReceber, IdOrigem, IdProcessoOrigem)
        {
            this.idDocumento = IdDocumento;
            this.usuarioLiberacao.IdObject = IdUsuarioLiberacao;
            this.documentoLiberado = DocumentoLiberado;
            this.valorLiquidado = ValorLiquidado;
            this.valorRevertido = ValorRevertido;
            this.valorCancelado = ValorCancelado;
            this.dataLiberacao = DataLiberacao;
            this.horaLiberacao = HoraLiberacao;
            this.idSituacaoDocumento = IdSituacao;
            this.usuarioInclusao.IdObject = IdUsuarioInclusao;
            this.usuarioAlteracao.IdObject = IdUsuarioAlteracao;
            this.dataAlteracao = DataAlteracao;
            this.horaAlteracao = HoraAlteracao;
        }

        #region Métodos protegidos

        /// <summary>
        /// Sobrescrevendo o método que persiste o rateio (a persistencia acontece
        /// depois dos demais processos)
        /// </summary>
        protected override void PersistirRateio(TransacaoBase transacao)
        {
        }

        /// <summary>
        /// Sobrescrevendo o método que persiste o rateio (a persistência acontece antes 
        /// dos demais processos)
        /// </summary>
        protected override void ExcluirRateio(TransacaoBase transacao)
        {
        }

        /// <summary>
        /// Sobrescrevendo o método de validações
        /// </summary>
        public override void Validate()
        {
            ValidarEntidade();
            base.Validate();

            string s = AmbienteServidorGlobal.AcessoGlobalFactory.ValidarDataBloqueio("emissão", this.DataEmissao, this.Ambiente);
            if (!String.IsNullOrEmpty(s))
                throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, s);

            if (parcelaLista != null)
                parcelaLista.ValidateItems();

            if (!TipoDocumento.Sequencial && String.IsNullOrEmpty(NumeroDocumento))
                throw new ObjetoNegocioException(ObjetoNegocioMensagem.PropriedadeObrigatoria,
                                                 LanguageManager.Manager.GetItemText(this, 1));

            if (this.IdProcessoOrigem == ProcessoOrigem.Documento &&
                Comissionados.Count > 0 && Comissionados.TotalPercentual > 100)
                throw new ValorInvalidoException(ValorInvalidoMensagem.ValorInvalido,
                                                LanguageManager.Manager.GetItemText(this, 2));

            //Verificar caso o indíce for diferente do padrão se tem cotação
            CalcularValorConvertido();
            ValidarNumeroDocumento(this.NumeroDocumento);
            ValidaDocumentoCartao();
        }

        /// <summary>
        /// Sobrescrevendo o método antes de persistir pra garantir a geração do histórico
        /// </summary>
        protected override void OnBeforeExecutarPersistir(TransacaoBase transacao)
        {
            if (!IsPersisted)
            {
                this.usuarioInclusao.IdObject = this.Ambiente.IdUsuario;

                //Liberação do docto pagar
                bool requerLiberacao = AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.RequerLiberacaoDoctoPagar, this.Ambiente).ValorBool;
                if (requerLiberacao)
                {
                    IUsuario u = (IUsuario)Factory.Retornar(typeof(IUsuario), this.Ambiente, transacao, this.Ambiente.IdUsuario);
                    if (!u.RequerLiberacao)
                    {
                        this.documentoLiberado = true;
                        this.dataLiberacao = AmbienteServidorGlobal.DataSistema;
                        this.horaLiberacao = AmbienteServidorGlobal.HoraSistema;
                        this.usuarioLiberacao.IdObject = this.Ambiente.IdUsuario;
                    }
                }
            }
            
            base.OnBeforeExecutarPersistir(transacao);

            if (TipoDocumento.Sequencial && String.IsNullOrEmpty(NumeroDocumento))
                NumeroDocumento = TipoDocumento.GerarProximoSequencial(transacao).ToString();            

            this.IdSituacao = RetornarSituacaoDocumento();
            this.Historico = HistoricoPadrao.GerarHistorico(historico, this);
            this.usuarioAlteracao.IdObject = this.Ambiente.IdUsuario;
            this.dataAlteracao = AmbienteServidorGlobal.DataSistema;
            this.horaAlteracao = AmbienteServidorGlobal.HoraSistema;   

            ExcluirComissao(transacao);
        }

        /// <summary>
        /// Metodo que valida o documento cartão
        /// </summary>
        private void ValidaDocumentoCartao()
        {
            if (this.PagarReceber != PagarReceberTipo.MovimentoCartao)
                return;

            string msg = null;
            if (!Entidade.InstituicaoFinanceira)
                msg += "A entidade deve ser do tipo 'Instituição financeira'.\r\n";           

            if (this.DocumentoCartao.BandeiraCartao == null)
                msg += "Deve ser informado a bandeira do cartão.\r\n";

            string s = this.NumeroAutorizacao;
            this.DocumentoCartao.NumeroAutorizacao = String.IsNullOrEmpty(s) ? null : s.Trim();
            if (String.IsNullOrEmpty(DocumentoCartao.NumeroAutorizacao))
                msg += "Deve ser informado o número de autorização do cartão.\r\n";

            if (DocumentoCartao.TipoCartao == 0)
                msg += "Deve ser informado o tipo do cartão.\r\n";

            double taxa = this.DocumentoCartao.Taxa;
            int count = this.ItensFinanceiros.Count;
            if (taxa > 0 && count == 0)
            {
                IOperacaoMovimentoCartao omc = (IOperacaoMovimentoCartao)this.Operacao.OperacaoPadrao;
                IItemFinanceiro item = omc.ItemFinanceiro;
                if (item == null)
                    msg += String.Format("Deve ser informado item financeiro para a taxa do cartão na operação '{0} - {1}'.\r\n", omc.IdOperacao, Operacao.Descricao);
                else
                {
                    if (item.IdCalculo != CalculoItemFinanceiro.PercentualSubtrair)
                        msg += String.Format("O item financeiro para a taxa do cartão na operação '{0} - {1}', deve estar com cálculo (%-).\r\n", omc.IdOperacao, Operacao.Descricao);
                    else
                    {
                        s = String.Format("NÃO foi possível carregar o item financeiro '{0} - {1}' informado para a taxa do cartão na operação\r\n", item.IdItemFinanceiro, item.Descricao);
                        s = String.Format("{0} ({1} - {2}). Verificar o cadastro do item financeiro.\r\n", s, omc.IdOperacao, this.Operacao.Descricao);
                        msg += s;
                    }
                }
            }

            if (count > 0 && taxa == 0)
                msg += "O item financeiro e a taxa estão INCOMPATÍVEIS. Deve ser preenchido os 2 campos OU limpar ambos.";

            if (String.IsNullOrEmpty(msg))
                return;

            msg = String.Format("Para o movimento de cartão verificar o(s) seguinte(s) erro(s):\r\n{0}", msg);
            throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
        }

        /// <summary>
        /// Sobrescrevendo o método de persistência para garantir que a situação esteja atualizada
        /// </summary>
        protected override void ExecutarPersistir(TransacaoBase transacao)
        {
            bool persistido = IsPersisted;
            base.ExecutarPersistir(transacao);

            PersistirComissao(transacao);
            PersistirRateioDocumento(persistido, transacao);
            PersistirDocumentoCartao(transacao);

            //Deixar nesta posição, ou seja, após executar todos os processos de persistencia
            //atualizar o hashtable de parcelas
            AtualizarListaParcelaPersistida();
        }

        /// <summary>
        /// Metodo que persiste o documento cartão
        /// </summary>
        private void PersistirDocumentoCartao(TransacaoBase transacao)
        {
            if (this.PagarReceber != PagarReceberTipo.MovimentoCartao)
                return;
          
            this.DocumentoCartao.Documento = this;
            this.DocumentoCartao.Persist(transacao);
        }

        /// <summary>
        /// Método para persistir a comissão
        /// </summary>
        private void PersistirComissao(TransacaoBase transacao)
        {
            if (!ValidarComissao())
                return;

            ICalculoComissao cc = (ICalculoComissao)Factory.Instanciar(typeof(ICalculoComissao), this.Ambiente, transacao);
            cc.SetComissao(transacao, this, AcaoDescricao.Persistir);
        }

        /// <summary>
        /// Método para verificar se pode ser gerado comissão
        /// </summary>        
        private bool ValidarComissao()
        {
            if (this.IdSituacao != SituacaoDocumento.Aberto)
                return false;

            if (PagarReceber != PagarReceberTipo.Receber)
                return false;

            if (IdProcessoOrigem != ProcessoOrigem.Documento)
                return false;

            return true;
        }

        /// <summary>
        /// Método para persistir o rateio do documento
        /// </summary>        
        private void PersistirRateioDocumento(bool persistido, TransacaoBase transacao)
        {
            if (!rateioMovto.PermitePersistirRateio() && !refazerRateio)
                return;

            ExcluirRateioDocumento(persistido, transacao);
            base.PersistirRateio(transacao);
            refazerRateio = false;
        }

        /// <summary>
        /// Método para atualizar a lista de parcelas persistidas
        /// </summary>
        private void AtualizarListaParcelaPersistida()
        {
            //Atualizar a quantidade parcelas persistidas, para que caso não seja recarregada
            //a prop. "Parcelas", isto garante que tenha-se uma lista de parcelas atualizadas
            //que será usada para comparar na exclusão do rateio.
            htParcelaPersistida = null;
            AddParcelaPersistida();
        }

        /// <summary>
        /// Método para excluir o rateio do documento quando alterado a 
        /// condição de pagamento, pois as parcelas da condição antes de ser
        /// alterado não estavam sendo excluído, ficando "lixo" no banco
        /// </summary>
        private void ExcluirRateioDocumento(bool persistido, TransacaoBase transacao)
        {
            if (!persistido || this.RateioMovto == null)
                return;

            AddParcelaPersistida();

            Hashtable htParcelaExcluir = GetParcelasExcluir();
            if (htParcelaExcluir == null || htParcelaExcluir.Count == 0)
                return;

            int[] id = new int[htParcelaExcluir.Count];
            int index = 0;
            foreach (DictionaryEntry de in htParcelaExcluir)
            {
                IDocumentoParcela dp = (IDocumentoParcela)de.Value;
                id[index] = dp.IdDocumentoParcela;
                index++;
            }

            if (id.Length == 0)
                return;

            this.RateioMovto.ExcluirRateio(id, transacao);
        }

        /// <summary>
        /// Método para retornar a lista de parcelas a ser excluida
        /// </summary>        
        private Hashtable GetParcelasExcluir()
        {
            if (this.parcelaLista.Count != this.htParcelaPersistida.Count)
                return this.htParcelaPersistida;

            foreach (IDocumentoParcela dp in this.parcelaLista)
            {
                if (!htParcelaPersistida.ContainsKey(dp.IdDocumentoParcela))
                    continue;

                htParcelaPersistida.Remove(dp.IdDocumentoParcela);
            }

            return htParcelaPersistida;
        }

        /// <summary>
        /// Método para excluir a comissão, caso a condição de pagamento foi alterada        
        /// </summary>        
        private void ExcluirComissao(TransacaoBase transacao)
        {
            if (!IsPersisted || this.htParcelaPersistida == null || this.htParcelaPersistida.Count == 0)
                return;

            if (!ValidarComissao())
                return;

            Hashtable htExcluir = GetParcelasExcluir();
            if (htExcluir.Count == 0)
                return;

            //*************************************************************************************************************************
            //A exclusão da comissão se deve ao fato de as parcelas estarem ligadas no lançamento da comissão
            //por parcela, consequentemente ao alterar a condição de pagamento, deve ser excluídas do lançamento
            //para não dar erro de chave estrangeira, e por consequencia, como a comissão será persistida novamente
            //então exclui-se toda a comissão e no processo de comissão garante a geração da nova comissão com as novas parcelas
            //***************************************************************************************************************************

            IObjectQuery c = AmbienteServidorGlobal.ObjetoNegocioFactory.InstanciarQuery(typeof(IVendaComissao), this.Ambiente);
            c.AddConstraint(Operator.Equals, "IdFilial", this.Ambiente.IdFilial);
            c.AddConstraint(Operator.Equals, "IdEmpresa", this.Ambiente.IdEmpresa);
            c.AddConstraint(Operator.Equals, "IdOrigem", this.IdDocumento);
            c.AddConstraint(Operator.Equals, "IdProcessoOrigem", ProcessoOrigem.Documento);

            IListBase l = new ListBase(this.Ambiente);
            AmbienteServidorGlobal.ObjetoNegocioFactory.RetornarConsulta(c, l, transacao);

            if (l.Count == 0)
                return;

            for (int i = 0; i < l.Count; i++)
            {
                IVendaComissao vc = (IVendaComissao)l[i];
                vc.Remove(transacao);

                l.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        /// Método para add as parcelas a serem persistidas
        /// </summary>
        private void AddParcelaPersistida()
        {
            if (this.htParcelaPersistida != null && this.htParcelaPersistida.Count > 0)
                return;

            //Será sempre instanciado uma nova lista, tendo em vista que caso haja
            //algum erro na persistencia e ao retornar a persistir o objeto, garanta
            //que esta lista sempre esteja com as parcelas atualizadas            
            this.htParcelaPersistida = new Hashtable();
            foreach (IDocumentoParcela dp in this.parcelaLista)
                this.htParcelaPersistida.Add(dp.IdDocumentoParcela, dp);
        }


        /// <summary>
        /// Sobrescrevendo o método de exclusão
        /// </summary>
        protected override void ExecutarExcluir(TransacaoBase transacao)
        {
            ValidarExclusao();

            //Excluir comissão
            if (this.PagarReceber == PagarReceberTipo.Receber)
            {
                ICalculoComissao calculo = (ICalculoComissao)Factory.Instanciar(typeof(ICalculoComissao), this.Ambiente, transacao);
                calculo.SetComissao(transacao, this, AcaoDescricao.Excluir);
            }

            base.ExcluirRateio(transacao);
            base.ExecutarExcluir(transacao);
        }

        /// <summary>
        /// Método para validar a exclusão do documento financeiro
        /// </summary>        
        private void ValidarExclusao()
        {
            string msg = null;
            if (validarProcessoOrigem)
            {
                if (IdProcessoOrigem != ProcessoOrigem.Documento)
                    msg += "A origem do documento financeiro está diferente de 'Documento'.\r\n";
            }

            if (IdSituacao != SituacaoDocumento.Aberto)
            {
                string s = AmbienteServidorGlobal.AcessoGlobalFactory.GetLabelEnum(typeof(SituacaoDocumento), this.IdSituacao, this.Ambiente);
                msg += String.Format("Somente é permitido documento 'Aberto' e a situação deste documento está '{0}'.\r\n", s);
            }

            if (IdSituacao == SituacaoDocumento.Aberto && Parcelas.ParcelaMovto)
                msg += "Existem movimentos financeiros para este documento.";

            if (String.IsNullOrEmpty(msg))
                return;

            msg = String.Format("Este documento não pode ser excluído pelo(s) seguinte(s) motivo(s):\r\n{0}", msg);
            ObjetoNegocioMensagem objMsg = msgErroExclusaoOutroProcesso ? ObjetoNegocioMensagem.ValidacaoValidar : ObjetoNegocioMensagem.DocumentoFinanceiroValidar;
            throw new ObjetoNegocioException(objMsg, msg);
        }

        #endregion

        #region Métodos privados do tributo

        /// <summary>
        /// Método para carregar a lista de parcelas de documento
        /// </summary>
        private void RetornarTributoLista()
        {
            tributoLista = new DocumentoTributoLista(this);

            if (IsPersisted)
                this.RetornarLista(typeof(DocumentoTributo), tributoLista, "IdDocumento", "IdFilial");

            tributoLista.ListChanged += new System.ComponentModel.ListChangedEventHandler(tributoLista_ListaAlterada);
        }

        /// <summary>
        /// Método executado na inserção/remoção de itens de tributo
        /// </summary>
        private void tributoLista_ListaAlterada(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (e.ListChangedType == System.ComponentModel.ListChangedType.ItemDeleted)
                TributosAlterados(null);
        }

        /// <summary>
        /// Método para reaplicar o valor para as bases do cálculo do tributo
        /// </summary>
        private void RecalcularTributos()
        {
            foreach (DocumentoTributo dt in Tributos)
                dt.SetBaseCalculo(Valor);
        }

        /// <summary>
        /// Método para aplicar o rateio definido pelos tributos
        /// </summary>
        public void AplicarRateioTributos()
        {
            if (!RateioMovto.UsaClasse)
            {
                RecalcularTributoRateio();
                return;
            }

            // Limpando os itens fixos atuais
            int c = RateioMovto.Classes.Count - 1;
            while (c >= 0)
            {
                RateioItem ri = (RateioItem)RateioMovto.Classes[c];
                if (ri.ItemFixo)
                {
                    ri.ItemFixo = false;
                    RateioMovto.Classes.Remove(ri);
                }

                c--;
            }

            //Armazena temporiamente os rateios já existentes, que não são dos tributos
            IRateioMovto rmOld = new RateioMovto(this);
            Hashtable htClasseOld = new Hashtable();
            foreach (IRateioItem riOld in this.rateioMovto.Classes)
            {
                rmOld.Classes.Add(riOld);
                htClasseOld.Add(riOld.IdRateioItem, riOld);
            }

            TipoRateioItem tipo = TipoRateioItem.Nenhum;
            if (this.rateioMovto.UsaCentroCusto && !this.rateioMovto.ProjetoPrecedeCentroCusto)
                tipo = TipoRateioItem.CentroCusto;
            else if (this.rateioMovto.UsaProjeto || (this.rateioMovto.UsaProjeto && this.rateioMovto.ProjetoPrecedeCentroCusto))
                tipo = TipoRateioItem.Projeto;


            // Adicionando os itens de classe derivados dos tributos
            Hashtable hc = new Hashtable();
            foreach (DocumentoTributo dt in Tributos)
            {
                if (dt.Tributo == null || dt.IdClasse == null)
                    continue;

                //Verificar se o rateio já foi informado e se a classe dos tributos = rateio, não é permitido
                if (htClasseOld != null && htClasseOld.ContainsKey(dt.IdClasse))
                {
                    string msg = String.Format("Para o tributo '{0}', a classe selecionada ({1}) NÃO é permitida, pois o rateio já possui esta classe.", dt.IdTributo, dt.IdClasse);
                    throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);                    
                }

                if (hc.ContainsKey(dt.IdClasse))
                {
                    ((RateioItemClasse)hc[dt.IdClasse]).Valor += dt.Valor;
                    continue;
                }

                //Adiciona classe do tributo
                RateioItemClasse ric = (RateioItemClasse)RateioMovto.Classes.AddItem();
                ric.IdRateioItem = (int)dt.IdClasse;
                ric.Valor = dt.Valor;
                ric.NaturezaDevedora = (PagarReceber == PagarReceberTipo.Receber ? true : false);

                hc.Add(dt.IdClasse, ric);

                //Caso esteja config. para não usar nem centro custo ou projeto
                if (tipo == TipoRateioItem.Nenhum || rmOld.Classes.Count == 0)
                    continue;

                //Adiciona centrocusto/projeto para classe do tributo, com base nos rateios existentes                
                this.rateioMovto.LimparAcumulador();
                foreach (IRateioItem riOld in rmOld.Classes)
                {
                    int idClasse = (int)dt.IdClasse;
                    AddRateioTributo(idClasse, idClasse, dt.IdFilial, dt.Valor, riOld.Rateios);
                }

                int index = (this.rateioMovto.Classes.Count - 1);
                IRateioItemLista ril = this.rateioMovto.Classes[index].Rateios;
                this.rateioMovto.AcumuladorAplicar(tipo, (int)dt.IdClasse, (int)dt.IdClasse, ril);
            }

            //Seta para classe do tributo, o status informando que o rateio é fixo, por se tratar de tributo
            //consequentemente não pode ser alterado
            foreach (DictionaryEntry de in hc)
                ((RateioItemClasse)de.Value).ItemFixo = true;

        }

        /// <summary>
        /// Método que indica alteração nos tributos do documento
        /// </summary>
        internal void TributosAlterados(DocumentoTributo documentoTributo)
        {
            if (IdCondicaoPagamento != 0)
                AplicarCondicaoPagamento(false);

            AplicarRateioTributos();
        }

        /// <summary>
        /// Método para adicionar o rateio do tributo
        /// </summary>
        private void AddRateioTributo(int idRateioPai, int idRateioPaiMaster, int idFilialOrigem, double valor, IRateioItemLista listaRateioOrigem)
        {
            foreach (RateioItem ri in listaRateioOrigem)
            {
                this.rateioMovto.AcumuladorAdd(ri.TipoRateioItem, ri.IdRateioItem, idRateioPaiMaster, idRateioPai, ri.Percentual, idFilialOrigem);
                if (ri.Rateios.TipoRateioItem != TipoRateioItem.Nenhum)
                    AddRateioTributo(ri.IdRateioItem, idRateioPaiMaster, IdFilialOrigem, ri.Percentual, ri.Rateios);
            }
        }

        #endregion

        #region Métodos privados do item financeiro

        /// <summary>
        /// Método para criar e carregar a lista de itens financeiros
        /// </summary>
        private void RetornarItemFinanceiroLista()
        {
            itemFinanceiroLista = new DoctoItemFinanceiroLista(this);

            if (IsPersisted)
                this.RetornarLista(typeof(DoctoItemFinanceiro), itemFinanceiroLista, "IdDocumento", "IdFilial");
        }

        /// <summary>
        /// Método para validar os itens financeiros para a nova operação
        /// </summary>
        private void ValidarItensFinanceiros(int idOperacao)
        {
            if (ItensFinanceiros.Count == 0)
                return;

            foreach (IDoctoItemFinanceiro item in ItensFinanceiros)
            {
                bool encontrado = false;

                foreach (IItemFinanceiroOperacao ifo in item.ItemFinanceiro.ItensOperacao)
                {
                    if (ifo.IdOperacao == idOperacao)
                    {
                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado && item.ItemFinanceiro.ItensOperacao.Count > 0)
                    throw new ItemFinanceiroOperacaoException(item.IdItemFinanceiro, idOperacao);
            }
        }

        /// <summary>
        /// Método para verificar se pode ser inserido item financeiro padrão
        /// </summary>        
                private bool PermiteItemFinanceiroPadrao()
        {
            if (this.PagarReceber != PagarReceberTipo.Receber || this.IdEntidade == 0)
                return false;

            try
            {
                var cliente = FinancialStranglerHelper.GetJson<ClienteDto>("cliente/" + this.IdEntidade);
                return cliente != null && cliente.ItemFinanceiroPadrao.HasValue && cliente.ItemFinanceiroPadrao.Value;
            }
            catch
            {
                return false;
            }
        }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Metodo que carrega na lista de itens financeiros o item financeiro do id informado
        /// </summary>
        private void CarregarItemFinanceiro(int idItemFinanceiro)
        {
            IDoctoItemFinanceiro newItem = null;
            try
            {
                newItem = ItensFinanceiros.AddItemFinanceiro();
                newItem.IdItemFinanceiro = idItemFinanceiro;
            }
            catch (Exception e)
            {
                if (!(e is ObjetoNegocioException))
                    ControleErro.TratarErro(e, true);
                else if (newItem != null)
                    ItensFinanceiros.Remove(newItem);
            }
        }

        #endregion

        #region Métodos privados da condição de pagamento

        /// <summary>
        /// Método para carregar a lista de parcelas do documento
        /// </summary>
        private void RetornarParcelaLista()
        {
            carregandoParcelas = true;

            parcelaLista = (DocumentoParcelaLista)new DocumentoParcelaLista(this);
            parcelaLista.ListChanged += new ListChangedEventHandler(ParcelaLista_ListaAlterada);

            if (IsPersisted)
            {
                this.RetornarLista(typeof(DocumentoParcela), parcelaLista, "IdDocumento", "IdFilial");
                AddParcelaPersistida();
            }

            carregandoParcelas = false;
        }

        /// <summary>
        /// Método para verificação de alteração de parcelamento
        /// </summary>
        private void ParcelaLista_ListaAlterada(object sender, ListChangedEventArgs e)
        {
            if (!carregandoParcelas)
                refazerRateio = true;
        }       

        #endregion

        #region Métodos protegidos documento

        /// <summary>
        /// Sobrescrevendo o método para carregar complemento da entidade
        /// </summary>
        protected override bool CarregarComplementoEntidade(int idEntidade)
        {
            if (!base.CarregarComplementoEntidade(idEntidade))
                return false;

            //Os itens financeiros default só deve ser carregado qdo origem = documento
            //outras origens considera-se o que foi aplicado no objeto dos processos
            if (this.IdProcessoOrigem == ProcessoOrigem.Documento)
                CarregarItensFinanceiroPadrao();

            CarregarComissionadoPadrao(idEntidade);
            return true;
        }

        /// <summary>
        /// Sobrescrevendo o método para validar operação
        /// </summary>
        protected override bool ValidarOperacao(int idOperacao)
        {
            if (!base.ValidarOperacao(idOperacao))
                return false;

            if (itemFinanceiroLista != null)
                itemFinanceiroLista.Clear();

            if (this.PagarReceber == PagarReceberTipo.Receber)
                ValidarItensFinanceiros(idOperacao);

            return true;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o Id da condição de pagamento válida
        /// </summary>
        protected override int GetIdCondicaoPagtoValida(int idCondicaoPagto, PagarReceberTipo tipo)
        {
            return this.Parcelas.GetCondicaoPagamentoValida(idCondicaoPagto, tipo);
        }

        /// <summary>
        /// Sobrescrevendo para verificar se a condição de pagamento informado pode ser aplicada
        /// </summary>
        protected override bool PermiteAplicarCondicaoPagto(int idCondicaoPagto)
        {
            if (idCondicaoPagto == 0 || (this.DataEmissao == DateTime.MinValue && this.Parcelas.Count > 0))
            {
                this.Parcelas.Clear();

                if (idCondicaoPagto > 0 && this.DataEmissao == DateTime.MinValue)
                    this.IdCondicaoPagamento = 0;

                return false;
            }

            return base.PermiteAplicarCondicaoPagto(idCondicaoPagto);
        }

        /// <summary>
        /// Sobrescrevendo o método para carregar as parcelas conforme condição de pagamento
        /// </summary>
        protected override void SetParcelasCondicaoPagto(bool refazerCondicaoPagto)
        {
            if (!refazerCondicaoPagto)
            {
                this.Parcelas.RecalcularValorParcela(this.CondicaoPagamento);
                return;
            }

            this.Parcelas.SetParcelasCondicaoPagto(this.CondicaoPagamento);
            
        }

        /// <summary>
        /// Sobrescrevendo o método para recalcular tributo e rateio com alteração do valor
        /// </summary>        
        protected override void RecalcularTributoRateio()
        {
            if (this.RateioMovto == null)
                return;

            RecalcularTributos();
            this.RateioMovto.ReaplicarPercentuaisRateio(this.RateioMovto);
        }

        /// <summary>
        /// Sobrescrevendo o método para carregar comissionado default
        /// </summary>        
        protected override void CarregarComissionadoPadrao(int idEntidade)
        {
            Comissionados.Clear();
            if (IsPersisted || idEntidade == 0 || this.PagarReceber != PagarReceberTipo.Receber)
                return;

            try
            {
                IClienteConsulta cc = (IClienteConsulta)Factory.Retornar(typeof(IClienteConsulta), this.Ambiente, idEntidade);
                if (cc.IdComissionado == null)
                    return;

                IComissionado c = cc.Comissionado;
                Comissionados.AddComissionado(c.IdComissionado, c.Percentual);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Sobrescrevendo o metodo para carregar o item financeiro da operação.
        /// </summary>
        protected override void CarregaItemFinanceiroOperacao()
        {
            if (PagarReceber != PagarReceberTipo.MovimentoCartao)
                return;

            if (this.Operacao == null)
                return;

            IOperacaoMovimentoCartao omc = (IOperacaoMovimentoCartao)Operacao.OperacaoPadrao;
            if (omc.ItemFinanceiro == null)
                return;

            CarregarItemFinanceiro(omc.ItemFinanceiro.IdItemFinanceiro);
        }

        /// <summary>
        /// Sobrescrevendo o método para setar portador da parcela
        /// </summary>       
        protected override void SetPortadorParcela(int idPortador)
        {
            this.Parcelas.SetPortadorParcela(idPortador, this.parcelaLista);
        }

        /// <summary>
        /// Sobrescrevendo o método para setar a forma de cobrança da parcela
        /// </summary>        
        protected override void SetFormaCobrancaParcela(int idFormaCobranca)
        {
            this.Parcelas.SetCobrancaParcela((idFormaCobranca > 0) ? this.FormaCobranca : null);
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o Id da origem geradora
        /// </summary>        
        protected override int GetIdOrigemGerador()
        {
            return this.IdDocumento;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o processo origem geradora
        /// </summary>        
        protected override ProcessoOrigem GetProcessoOrigemGerador()
        {
            return ProcessoOrigem.Documento;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o Id do documento período
        /// </summary>        
        protected override int GetIdDocumentoFinanceiro()
        {
            return this.IdDocumento;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o valor do rateio
        /// </summary>        
        protected override double RetornarValorRateio(TipoRateioItem tipo)
        {
            if (cancelaDocumento && valorRateioCancelado > 0)
                return valorRateioCancelado;

            if (tipo == TipoRateioItem.Classe)
                return this.Valor;

            return this.ValorParcelado;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o processo origem do rateio
        /// </summary>        
        protected override ProcessoOrigem GetProcessoOrigemRateio()
        {
            if (cancelaDocumento)
                return ProcessoOrigem.CancelamentoDocumentoFinanceiro;

            return ProcessoOrigem.Documento;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar matriz da regra de origem do rateio
        /// </summary>        
        protected override IRateioRegraOrigem[] GetMatrizRegraOrigemRateio()
        {
            IRateioRegraOrigem[] m;
            if (!cancelaDocumento)
            {
                m = new IRateioRegraOrigem[this.Parcelas.Count];

                for (int i = 0; i < this.Parcelas.Count; i++)
                    m[i] = RetornarRateioRegraOrigem(i);
            }
            else
            {
                m = new IRateioRegraOrigem[1];
                m[0] = RetornarRateioRegraOrigem(indexCancelamento);
            }

            return m;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o objeto RateioRegraOrigem
        /// </summary>
        protected override IRateioRegraOrigem RetornarRateioRegraOrigem(int index)
        {
            IRateioRegraOrigem rro = (IRateioRegraOrigem)Factory.Instanciar(typeof(IRateioRegraOrigem), this.Ambiente);

            if (cancelaDocumento)
            {
                IDoctoCanceladoParcela dcp = (IDoctoCanceladoParcela)this.ParcelasCancelada[index];
                IDocumentoParcela dp = dcp.DocumentoParcela;
                rro.SetDados(dcp.IdDoctoCanceladoParcela, dp.DataVencimento);

                return rro;
            }

            double p = (Parcelas.Count == 1 ? 100 : (Parcelas[index].Valor / Valor) * 100);
            rro.SetDados(Parcelas[index].IdDocumentoParcela, Parcelas[index].DataVencimento, p);

            return rro;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o valor de validação do rateio
        /// </summary>        
        protected override double GetValorValidacaoRateio()
        {
            return this.ValorParcelado;
        }

        /// <summary>
        /// Sobrescrevendo o método para retornar o valor digitado do rateio
        /// </summary>        
        protected override double GetValorDigitadoRateio()
        {
            if (this.RateioMovto == null || this.RateioMovto.Classes.Count == 0)
                return base.GetValorDigitadoRateio();

            double total = 0;
            foreach (IRateioItem ri in this.RateioMovto.Classes)
            {
                double valor = ri.Percentual;
                if (RateioMovto.DadosRateio.TipoValidacaoItem == TipoRateioItemValidacao.ValidarValor)
                    valor = ri.Valor;

                if ((ri.NaturezaDevedora && PagarReceber != PagarReceberTipo.Pagar) ||
                   (!ri.NaturezaDevedora && PagarReceber == PagarReceberTipo.Pagar))
                    valor = valor * -1;

                total += valor;
            }

            return total;
        }

        #endregion

        #region Métodos privados

        /// <summary>
        /// Método para criar e carregar a lista de comissionados
        /// </summary>
        private void RetornarComissionadoLista()
        {
            comissionadoLista = new DocumentoComissionadoLista(this);

            if (IsPersisted)
                this.RetornarLista(typeof(DocumentoComissionado), comissionadoLista, "IdDocumento", "IdFilial");
        }

        /// <summary>
        /// Método para carregar informações default da operação
        /// </summary>
        private void CarregarDefaultOperacao()
        {
            if (this.Operacao == null)
                return;

            IOperacao operacao = this.Operacao;

            if (!IsPersisted && operacao.IdRegraRateio != null && RateioMovto != null)
                operacao.RegraRateio.GerarRateio(RateioMovto);

            if (this.pagarReceber == PagarReceberTipo.MovimentoCartao)
                return;

            IOperacaoPagarReceber opr = (IOperacaoPagarReceber)operacao.OperacaoPadrao;
            if (opr.Tributos.Count == 0)
                return;

            Tributos.Clear();
            foreach (IOperacaoTributo ot in opr.Tributos)
            {
                IDocumentoTributo tributo = Tributos.AddTributo();
                if (ot.Tributo == null)
                    continue;

                tributo.IdTributo = ot.IdTributo;

                if (ot.Tributo.TributoClasse != null)
                    tributo.IdClasse = ot.Tributo.TributoClasse.IdClasse;
            }
        }

        /// <summary>
        /// Metodo utilizado para carregar um documento cartao, caso nao haja um documento cartao ele cria um.
        /// </summary>
        private void CarregarDocumentoCartao()
        {
            try
            {
                documentoCartao = (IDocumentoCartao)Factory.Retornar(typeof(IDocumentoCartao), this.Ambiente, this.idDocumento);
            }
            catch (Exception e)
            {
                if (ControleErro.ENumeroInesperadoLinhas(e))
                {
                    documentoCartao = (IDocumentoCartao)Factory.Instanciar(typeof(IDocumentoCartao), this.Ambiente);
                    return;
                }

                ControleErro.TratarErro(e, true);
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Propriedae com o Id do documento
        /// </summary>
        [TableColumn("IdFinDocumento")]
        public int IdDocumento
        {
            get
            {
                return idDocumento;
            }
            set
            {
                idDocumento = value;
            }
        }

        /// <summary>
        /// Propriedade ocm o Id da filial a qual o documento pertence
        /// </summary>
        [TableColumn("IdGloFilial")]
        public override int IdFilial
        {
            get
            {
                return base.IdFilial;
            }
            set
            {
                base.IdFilial = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do Cliente/Fornecedor a qual o documento foi emitido
        /// </summary>
        [TableColumn("IdGloEntidade"), AtualizacaoDinamica("IdCondicaoPagamento")]
        public override int IdEntidade
        {
            get
            {
                return base.IdEntidade;
            }
            set
            {
                base.IdEntidade = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id da operação
        /// </summary>
        [TableColumn("IdGloOperacao"),
        AtualizacaoDinamica("Tributos", "IdTipoDocumento", "Historico")]
        public override int IdOperacao
        {
            get
            {
                return base.IdOperacao;
            }
            set
            {
                base.IdOperacao = value;
                CarregarDefaultOperacao();
            }
        }

        /// <summary>
        /// Propriedade ocm o Id do tipo de documento
        /// </summary>
        [TableColumn("IdGloTipoDocumento"), AtualizacaoDinamica("NumeroDocumento")]
        public override int IdTipoDocumento
        {
            get
            {
                return base.IdTipoDocumento;
            }
            set
            {
                base.IdTipoDocumento = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id da condição de pagamento
        /// </summary>
        [TableColumn("IdGloCondicaoPagamento"), AtualizacaoDinamica("IdPortador", "IdFormaCobranca")]
        public override int IdCondicaoPagamento
        {
            get
            {
                return base.IdCondicaoPagamento;
            }
            set
            {
                base.IdCondicaoPagamento = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do indice economico
        /// </summary>
        [TableColumn("IdGloIndiceEconomico")]
        public override int IdIndiceEconomico
        {
            get
            {
                return base.IdIndiceEconomico;
            }
            set
            {
                base.IdIndiceEconomico = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do Indice para qual será convertido o valor
        /// </summary>
        [TableColumn("IdGloIndiceConversao")]
        public override int IdIndiceConversao
        {
            get
            {
                return base.IdIndiceConversao;
            }
            set
            {
                base.IdIndiceConversao = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do UsuarioLiberacao do documento a pagar
        /// </summary>
        [TableColumn("IdGloUsuarioLiberacao")]
        public object IdUsuarioLiberacao
        {
            get
            {
                return usuarioLiberacao.IdObject;
            }
            set
            {
                usuarioLiberacao.IdObject = value;
            }
        }

        /// <summary>
        /// Propriedade com o numero do documento
        /// </summary>
        [TableColumn("NumeroDocumento")]
        public override string NumeroDocumento
        {
            get
            {
                return base.NumeroDocumento;
            }
            set
            {
                base.NumeroDocumento = value;
            }
        }

        /// <summary>
        /// Propriedade com o histórico
        /// </summary>
        [TableColumn("Historico")]
        public override string Historico
        {
            get
            {
                return base.Historico;
            }
            set
            {
                base.Historico = value;
            }
        }

        /// <summary>
        /// Propriedade indicando se o documento a pagar está liberado
        /// </summary>
        [TableColumn("DocumentoLiberado")]
        public bool DocumentoLiberado
        {
            get
            {
                return documentoLiberado;
            }
            set
            {
                documentoLiberado = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor total do documento
        /// </summary>
        [TableColumn("Valor"), AtualizacaoDinamica("ValorParcelado", "Parcelas")]
        public override double Valor
        {
            get
            {
                return base.Valor;
            }
            set
            {
                base.Valor = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor já liquidado do documento
        /// </summary>
        [TableColumn("ValorLiquidado")]
        public double ValorLiquidado
        {
            get
            {
                return valorLiquidado;
            }
            set
            {
                valorLiquidado = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor revertido do documento
        /// </summary>
        [TableColumn("ValorRevertido")]
        public double ValorRevertido
        {
            get
            {
                return valorRevertido;
            }
            set
            {
                valorRevertido = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor cancelado do documento
        /// </summary>
        [TableColumn("ValorCancelado")]
        public double ValorCancelado
        {
            get
            {
                return valorCancelado;
            }
            set
            {
                valorCancelado = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor parcela
        /// </summary>
        [TableColumn("ValorParcelado")]
        public double ValorParcelado
        {
            get
            {
                return Funcoes.Arredondar((Valor - Tributos.Total), 2);
            }
        }

        /// <summary>
        /// Propriedade com o valor convertido do título
        /// </summary>
        [TableColumn("ValorConvertido")]
        public override double ValorConvertido
        {
            get
            {
                return base.ValorConvertido;
            }
        }

        /// <summary>
        /// Propriedade com a data de emissão do documento
        /// </summary>
        [TableColumn("DataEmissao")]
        public override DateTime DataEmissao
        {
            get
            {
                return base.DataEmissao;
            }
            set
            {
                base.DataEmissao = value;
            }
        }

        /// <summary>
        /// Propriedade com a data de inclusão do documento no sistema 
        /// </summary>
        [TableColumn("DataInclusao")]
        public override DateTime DataInclusao
        {
            get
            {
                return base.DataInclusao;
            }
            set
            {
                base.DataInclusao = value;
            }
        }

        /// <summary>
        /// Propriedade com a hora da inclusao
        /// </summary>
        [TableColumn("HoraInclusao")]
        public override DateTime HoraInclusao
        {
            get
            {
                return base.HoraInclusao;
            }
            set
            {
                base.HoraInclusao = value;
            }
        }

        /// <summary>
        /// Propriedade com a data da liberação do documento a pagar
        /// </summary>
        [TableColumn("DataLiberacao")]
        public DateTime DataLiberacao
        {
            get
            {
                return dataLiberacao;
            }
            set
            {
                dataLiberacao = value;
            }
        }

        /// <summary>
        /// Propriedade com a hora da liberação do documento a pagar
        /// </summary>
        [TableColumn("HoraLiberacao")]
        public DateTime HoraLiberacao
        {
            get
            {
                return horaLiberacao;
            }
            set
            {
                horaLiberacao = value;
            }
        }

        /// <summary>
        /// Propriedade indicando se o título é de Receber ou Pagar
        /// </summary>
        [TableColumn("IdReceberPagar"), AtualizacaoDinamica("IdEntidade", "IdOperacao", "NumeroDocumento")]
        public override PagarReceberTipo PagarReceber
        {
            get
            {
                return base.PagarReceber;
            }
            set
            {
                base.PagarReceber = value;
            }
        }

        /// <summary>
        /// Propriedade com a situação do documento
        /// </summary>
        [TableColumn("IdSituacao")]
        public SituacaoDocumento IdSituacao
        {
            get
            {
                return idSituacaoDocumento;
            }
            set
            {
                idSituacaoDocumento = value;
            }
        }

        /// <summary>
        /// Propriedade com o número do cedente
        /// </summary>
        [TableColumn("NumeroCedente")]
        public override string NumeroCedente
        {
            get
            {
                return base.NumeroCedente;
            }
            set
            {
                base.NumeroCedente = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do Usuario na inclusão do documento
        /// </summary>
        [TableColumn("IdGloUsuarioInclusao")]
        public object IdUsuarioInclusao
        {
            get
            {
                return usuarioInclusao.IdObject;
            }
            set
            {
                usuarioInclusao.IdObject = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do Usuario na alteração do documento
        /// </summary>
        [TableColumn("IdGloUsuarioAlteracao")]
        public object IdUsuarioAlteracao
        {
            get
            {
                return usuarioAlteracao.IdObject;
            }
            set
            {
                usuarioAlteracao.IdObject = value;
            }
        }

        /// <summary>
        /// Propriedade com a data da alteração do documento
        /// </summary>
        [TableColumn("DataAlteracao")]
        public DateTime DataAlteracao
        {
            get
            {
                return dataAlteracao;
            }
            set
            {
                dataAlteracao = value;
            }
        }

        /// <summary>
        /// Propriedade com a hora da alteração
        /// </summary>
        [TableColumn("HoraAlteracao")]
        public DateTime HoraAlteracao
        {
            get
            {
                return horaAlteracao;
            }
            set
            {
                horaAlteracao = value;
            }
        }

        /// <summary>
        /// Propriedade com a lista de Parcelas do documento
        /// </summary>
        public IDocumentoParcelaLista Parcelas
        {
            get
            {

                if (parcelaLista == null)
                    RetornarParcelaLista();

                return parcelaLista;
            }
        }

        /// <summary>
        /// Propriedade com os tributos do documento
        /// </summary>
        public IDocumentoTributoLista Tributos
        {
            get
            {
                if (tributoLista == null)
                    RetornarTributoLista();

                return tributoLista;
            }
        }

        /// <summary>
        /// Propriedade com os itens financeiros do documento
        /// </summary>
        public IDoctoItemFinanceiroLista ItensFinanceiros
        {
            get
            {
                if (itemFinanceiroLista == null)
                    RetornarItemFinanceiroLista();

                return itemFinanceiroLista;
            }
        }

        /// <summary>
        /// Propriedade com a lista dos Comissionados
        /// </summary>
        public IDocumentoComissionadoLista Comissionados
        {
            get
            {
                if (comissionadoLista == null)
                    RetornarComissionadoLista();

                return comissionadoLista;
            }
        }

        /// <summary>
        /// Propriedade com o objeto do UsuarioLiberacao
        /// </summary>
        public IUsuario UsuarioLiberacao
        {
            get
            {
                return (IUsuario)usuarioLiberacao.GetObjeto(this.Ambiente);
            }
            set
            {
                usuarioLiberacao.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade com o objeto do UsuarioInclusao
        /// </summary>
        public IUsuario UsuarioInclusao
        {
            get
            {
                return (IUsuario)usuarioInclusao.GetObjeto(this.Ambiente);
            }
            set
            {
                usuarioInclusao.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade com o objeto do UsuarioAlteracao
        /// </summary>
        public IUsuario UsuarioAlteracao
        {
            get
            {
                return (IUsuario)usuarioAlteracao.GetObjeto(this.Ambiente);
            }
            set
            {
                usuarioAlteracao.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade que ativa a transação para o objeto
        /// </summary>
        public override bool IsTransactioned
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Propriedade com a conta bancaria para o calculo dos valores dos itens financeiros para o boleto
        /// </summary>
        public IContaBancaria ContaBancaria
        {
            get
            {
                return contaBancaria;
            }
            set
            {
                if (contaBancaria == value)
                    return;

                contaBancaria = value;
            }
        }

        #endregion

        #region Propriedades controle cartão

        /// <summary>
        /// Propriedade com o Id da Bandeira do cartão
        /// </summary>
        public int IdBandeiraCartao
        {
            get
            {
                return DocumentoCartao.IdBandeiraCartao;
            }
            set
            {
                DocumentoCartao.IdBandeiraCartao = value;
            }
        }

        /// <summary>
        /// Propriedade com o número de autorização do cartão
        /// </summary>
        public string NumeroAutorizacao
        {
            get
            {
                return DocumentoCartao.NumeroAutorizacao;
            }
            set
            {
                DocumentoCartao.NumeroAutorizacao = value;
            }
        }

        /// <summary>
        /// Propriedade com a taxa do cartão
        /// </summary>
        public double Taxa
        {
            get
            {
                return DocumentoCartao.Taxa;
            }
            set
            {
                DocumentoCartao.Taxa = value;
            }
        }

      
        /// <summary>
        /// Propriedade com o tipo do cartão
        /// </summary>
        public TipoCartao TipoCartao
        {
            get
            {
                return DocumentoCartao.TipoCartao;
            }
            set
            {
                DocumentoCartao.TipoCartao = value;
            }
        }
        
        /// <summary>
        /// Propriedade com o objeto documento cartão
        /// </summary>
        public IDocumentoCartao DocumentoCartao
        {
            get
            {
                if (documentoCartao == null)
                    CarregarDocumentoCartao();

                return documentoCartao;
            }
        }

        #endregion

        #region Propriedades controle

        /// <summary>
        /// Propriedade indicando se a validação de exclusão do documento
        /// está sendo disparado diretamente pelo processo ou está vindo de outro método
        /// </summary>
        public bool ValidacaoExclusaoOutroProcesso
        {
            get
            {
                return msgErroExclusaoOutroProcesso;
            }
            set
            {
                msgErroExclusaoOutroProcesso = value;
            }
        }

        /// <summary>
        /// Propriedade indicando se é para validar o processo origem
        /// </summary>
        public bool ValidarProcessoOrigem
        {
            get
            {
                return validarProcessoOrigem;
            }
            set
            {
                validarProcessoOrigem = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor pendente do documento para o cancelamento
        /// </summary>
        public double ValorPendente
        {
            get
            {
                return Funcoes.Arredondar((this.ValorParcelado - this.ValorLiquidado - this.ValorCancelado - this.ValorRevertido), 2);
            }
        }

        /// <summary>
        /// Propriedade com a lista de parcelas persistidas
        /// (exclusão qdo altera-se condição de pagamento)
        /// </summary>
        public Hashtable ParcelasPersistida
        {
            get
            {
                if (htParcelaPersistida == null)
                    htParcelaPersistida = new Hashtable();

                return htParcelaPersistida;
            }
        }

        /// <summary>
        /// Propriedade com a lista de parcelas canceladas
        /// </summary>
        public IListBase ParcelasCancelada
        {
            get
            {
                if (parcelaCanceladaLista == null)
                    parcelaCanceladaLista = new ListBase(this.Ambiente);

                return parcelaCanceladaLista;
            }
        }

        #endregion

        #region Propriedades Dados da Comissão

        /// <summary>
        /// Propriedade com a lista de comissionados utilizados no documento
        /// </summary>
        public IList ComissionadosComissao
        {
            get
            {
                return Comissionados;
            }
        }

        /// <summary>
        /// Propriedade com o Id da filial origem
        /// </summary>
        public int IdFilialOrigemComissao
        {
            get
            {
                return IdFilial;
            }
        }

        /// <summary>
        /// Propriedade com a data de emissão do documento
        /// </summary>
        public DateTime DataComissao
        {
            get
            {
                return DataEmissao;
            }
        }

        /// <summary>
        /// Propriedade com o valor líquido do documento
        /// </summary>
        public double ValorBase
        {
            get
            {
                return ValorConvertido;
            }
        }

        /// <summary>
        /// Propriedade com o valor bruto do documento
        /// </summary>
        public double ValorTotalParcelado
        {
            get
            {
                return ValorConvertido;
            }
        }

        /// <summary>
        /// Propriedade com o tipo da natureza do documento
        /// </summary>
        public NaturezaTipo TipoNatureza
        {
            get
            {
                return NaturezaTipo.Credora;
            }

        }

        /// <summary>
        /// Propriedade com as parcelas geradas pela documento
        /// </summary>
        public IList ParcelasComissao
        {
            get
            {
                return Parcelas;
            }
        }

        /// <summary>
        /// Propriedade com o tipo do processo
        /// </summary>
        public TipoProcessoComissao TipoProcesso
        {
            get
            {
                return TipoProcessoComissao.Documento;
            }
        }

        /// <summary>
        /// Propriedade indicando o IdOrigem deste gerador
        /// </summary>
        public override int IdOrigemGerador
        {
            get
            {
                return this.IdDocumento;
            }
        }

        /// <summary>
        /// Propriedade indicando o ProcessoOrigem deste gerador
        /// </summary>
        public override ProcessoOrigem IdProcessoOrigemGerador
        {
            get
            {
                return ProcessoOrigem.Documento;
            }
        }

        /// <summary>
        /// Propriedade com o Id da filial deste gerador
        /// </summary>
        public override int IdFilialGerador
        {
            get
            {
                return this.IdFilial;
            }
        }

        /// <summary>
        /// Propriedade com o histórico para o movimento de comissão
        /// </summary>
        public string HistoricoMovtoComissao
        {
            get
            {
                return RetonarHistoricoComissao();
            }
        }


        #region Métodos privados

        /// <summary>
        /// Método para retornar o historico da comissão
        /// </summary>        
        private string RetonarHistoricoComissao()
        {
            if (!cancelaDocumento)
                return Historico;

            this.Operacao.Ambiente.Transacao = this.Ambiente.Transacao;
            IOperacaoReceber or = (IOperacaoReceber)Operacao.OperacaoPadrao;
            if (or.IdHistoricoComissaoCancela != null)
                return or.HistoricoComissaoCancela.TextoPadrao;

            return null;
        }

        #endregion

        #endregion

        #region Métodos

        /// <summary>
        /// Método estático para carregar e retornar um Documento
        /// </summary>
        static public IDocumento Retornar(int idDocumento, IAmbiente ambiente)
        {
            Key key = new Key(typeof(Documento), true);
            key["IdDocumento"] = idDocumento;
            key["IdFilial"] = ambiente.IdFilial;
            return (IDocumento)ObjectPersist.Retornar(typeof(Documento), key, ambiente);
        }

        /// <summary>
        /// Método estático para carregar e retornar um Documento
        /// </summary>
        static public IDocumento Retornar(string numeroDocumento, int idTipoDocumento, IAmbiente ambiente)
        {
            Key key = new Key(typeof(Documento), true);
            key["NumeroDocumento"] = numeroDocumento;
            key["IdTipoDocumento"] = idTipoDocumento;
            key["IdFilial"] = ambiente.IdFilial;
            return (IDocumento)ObjectPersist.Retornar(typeof(Documento), key, ambiente);
        }

        /// <summary>
        /// Método para remover o documento financeiro do movimento de cartão
        /// </summary>        
        static public string RemoverDocumentoMovimentoCartaoEstorno(int idOrigem, ProcessoOrigem po, ILiquidacaoEstorno le, IAmbiente ambiente, TransacaoBase transacao)
        {
            string msgExclusao = null;
            var l = RetornarDocumentosProcessoOrigem(po, idOrigem, ambiente, transacao);
            if (l.Count == 0)
                return msgExclusao;

            IDocumento dSel = null;

            try
            {
                foreach (Documento d in l)
                {
                    dSel = d;
                    msgExclusao = Documento.RemoverDocumento(d, true, true, le.DataMovimento, "Estorno da liquidação", ambiente, transacao);               
                }
            }
            catch (Exception e)
            {
                string msg = String.Format("Ao remover o documento '{0}-{1}({2})' ocorreu o seguinte erro:\r\n\r\n{3}", dSel.TipoDocumento.SiglaDocumento, dSel.NumeroDocumento, dSel.IdDocumento, e.Message);
                throw new ObjetoNegocioException(ObjetoNegocioMensagem.ValidacaoValidar, msg);                  
            }

            return msgExclusao;
        }

        /// <summary>
        /// Metodo statico privado que remove o documento financeiro
        /// </summary>     
        private static string RemoverDocumento(Documento doc, bool permiteExclusaoOutroProcesso, bool cancelarParcelasMovimento, DateTime dataMovimento, 
                                               string motivoCancelamento, IAmbiente ambiente, TransacaoBase transacao)
        {
            string tipoDoc = doc.PagarReceber == PagarReceberTipo.Pagar ? "Dococumento a pagar" : doc.PagarReceber == PagarReceberTipo.Receber ? "Dococumento a receber" : "Dococumento cartão";
            
            if (doc.IdSituacao != SituacaoDocumento.Aberto)
            {
                string s = AmbienteServidorGlobal.AcessoGlobalFactory.GetLabelEnum(typeof(SituacaoDocumento), doc.IdSituacao, doc.Ambiente);
                return String.Format("{4}: '{0}-{1}({2})'  Situação: {3}", doc.TipoDocumento.SiglaDocumento, doc.NumeroDocumento, doc.IdDocumento, s, tipoDoc);
            }

            //se o documento estiver aberto e tiver movimento então cancelar as parcelas do movimento
            if (doc.Parcelas.ParcelaMovto)
            {
                if (!cancelarParcelasMovimento)
                {
                    string s = AmbienteServidorGlobal.AcessoGlobalFactory.GetLabelEnum(typeof(SituacaoDocumento), doc.IdSituacao, doc.Ambiente);
                    return String.Format("O {4}: '{0}-{1}({2})'  Situação: {3}, possui movimento de parcelas mas não é permitido cancelar as parcelas restantes deste documento.", doc.TipoDocumento.SiglaDocumento, doc.NumeroDocumento, doc.IdDocumento, s, tipoDoc);
                }

                List<IDocumentoParcela> lParcelasRemove = new List<IDocumentoParcela>();
                for (int i = 0; i < doc.Parcelas.Count; i++)
                {
                    IDocumentoParcela dp = doc.Parcelas[i];
                    if (dp.IdSituacao != SituacaoDocumento.Aberto &&
                        dp.IdSituacao != SituacaoDocumento.LiquidadoParcial)
                        continue;

                    dp.IndexParcela = i;
                    dp.Marcado = true;
                    lParcelasRemove.Add(dp);
                }

                doc.PersistirCancelamentoDocumento(motivoCancelamento, dataMovimento, lParcelasRemove, null, transacao);
                return String.Empty;
            }

            if (permiteExclusaoOutroProcesso)
            {
                doc.ValidacaoExclusaoOutroProcesso = true;
                doc.ValidarProcessoOrigem = false;
            }

            doc.Remove(transacao);

            if (permiteExclusaoOutroProcesso)
                doc.ValidarProcessoOrigem = true;

            return String.Empty;
        }

        /// <summary>
        /// Método para remover o documento financeiro do movimento de cartão
        /// </summary>        
        static public string RemoverDocumentoContrato(int idContrato, IAmbiente ambiente, TransacaoBase transacao)
        {
            var msgExclusao = new StringBuilder();

            IList l = RetornarDocumentosProcessoOrigem(ProcessoOrigem.Contrato, idContrato, ambiente, transacao);

            if (l.Count == 0)
                return msgExclusao.ToString();

            foreach (Documento d in l)
            {
                var msg = Documento.RemoverDocumento(d, true, true, AmbienteServidorGlobal.DataSistema, "Cancelamento documento financeiro do contrato", ambiente, transacao);

                if (!String.IsNullOrEmpty(msg))
                    msgExclusao.AppendLine(msg);
            }

            return msgExclusao.ToString();
        }

        /// <summary>
        /// Metodo estatico que retorna uma lista de documentos para o processo origem e id origem informados
        /// </summary>
        static public IList RetornarDocumentosProcessoOrigem(ProcessoOrigem processoOrigem, int idOrigem, IAmbiente ambiente, TransacaoBase transacao = null)
        {
            IObjectQuery c = AmbienteServidorGlobal.ObjetoNegocioFactory.InstanciarQuery(typeof(IDocumento), ambiente);
            c.AddConstraint(Operator.Equals, "IdFilial", ambiente.IdFilial);
            c.AddConstraint(Operator.Equals, "IdProcessoOrigem", processoOrigem);
            c.AddConstraint(Operator.Equals, "IdOrigem", idOrigem);

            return AmbienteServidorGlobal.ObjetoNegocioFactory.RetornarConsulta(c, transacao);
        }     

        /// <summary>
        /// Método para retornar o objeto DocumentoParcela
        /// </summary>
        public IDocumentoParcela RetornaDocumentoParcela(int numeroParcela)
        {
            return this.Parcelas.RetornaDocumentoParcela(numeroParcela);
        }

        /// <summary>
        /// Método para assimilar dados para o Documento
        /// </summary>        
        public void Assimilar(IReversao r)
        {
            this.dataEmissao = r.Data;
            this.pagarReceber = r.PagarReceber;
            this.IdCondicaoPagamento = r.IdCondicaoPagamento;
            this.IdEntidade = r.IdEntidade;
            this.IdIndiceEconomico = r.IdIndiceEconomico;
            this.IdSituacao = SituacaoDocumento.Aberto;
            this.valor = r.Total;
            this.AplicarOperacao(r.IdOperacao);
            this.IdTipoDocumento = r.IdTipoDocumento;
            this.numeroDocumento = r.NumeroDocumento;
            this.SetDadosOrigemGerador(r);

            // Parcelas
            this.Parcelas.Clear();
            foreach (DocumentoParcela parc in r.Parcelas)
            {
                parc.Ambiente.Transacao = this.Ambiente.Transacao;

                parc.TipoRegistro = RegistroDocumentoTipo.Reversao;
                this.Parcelas.Add(parc);
            }

            //Itens financeiros            
            this.ItensFinanceiros.Clear();
            foreach (IDoctoItemFinanceiro dif in r.ItensFinanceirosNovo)
            {
                dif.Ambiente.Transacao = this.Ambiente.Transacao;
                this.ItensFinanceiros.Add(dif);
            }

            this.RateioMovto.Classes.Clear();
            foreach (IRateioItem ri in r.RateioMovto.Classes)
            {
                ri.Ambiente.Transacao = this.Ambiente.Transacao;
                this.rateioMovto.Classes.Add(ri);
            }

            this.RateioMovto.CentroCustosProjetos.Clear();
            foreach (IRateioItem ri in r.RateioMovto.CentroCustosProjetos)
            {
                ri.Ambiente.Transacao = this.Ambiente.Transacao;
                this.rateioMovto.CentroCustosProjetos.Add(ri);
            }

            this.Tributos.Clear();
        }

        /// <summary>
        /// Método para assimilar dados para o Documento
        /// </summary>        
        public void Assimilar(IVeiculoMovtoFinanceiro vmf)
        {
            this.SetDadosOrigemGerador(vmf);

            this.dataEmissao = vmf.DataEmissao;
            this.pagarReceber = vmf.PagarReceber;
            this.IdEntidade = vmf.IdEntidade;
            this.IdCondicaoPagamento = vmf.IdCondicaoPagamento;
            this.IdIndiceEconomico = vmf.IdIndiceEconomico;
            this.valor = vmf.Valor;
            this.AplicarOperacao(vmf.IdOperacao);
            this.IdTipoDocumento = vmf.IdTipoDocumento;
            this.numeroDocumento = vmf.NumeroDocumento;
            this.historico = vmf.Historico;
            this.NumeroCedente = vmf.NumeroCedente;

            this.ItensFinanceiros.Clear();
            this.Tributos.Clear();

            // Parcelas
            this.Parcelas.Clear();
            foreach (IVeiculoMovtoFinanceiroParcela vmfp in vmf.Parcelas)
            {
                vmfp.Ambiente.Transacao = this.Ambiente.Transacao;

                IParcelaBase pb = this.Parcelas.AddParcela(vmfp.IdFormaCobrancaParcela, vmfp.IdPortadorParcela, true, vmfp.DataVencimento, vmfp.Valor, vmfp.NumeroParcela, this.Ambiente.Transacao);
                pb.IdBanco = vmfp.IdBanco;
                pb.NumeroAgencia = vmfp.NumeroAgencia;
                pb.DigitoAgencia = vmfp.DigitoAgencia;
                pb.ContaCorrente = vmfp.ContaCorrente;
                pb.DigitoConta = vmfp.DigitoConta;
                pb.Favorecido = vmfp.Favorecido;
                pb.TipoConta = vmfp.TipoConta;
                pb.CpfCnpjFavorecido = vmfp.CpfCnpjFavorecido;
                pb.Complemento = vmfp.Complemento;
                pb.NossoNumero = vmfp.NossoNumero;
                pb.CodigoBarras = vmfp.CodigoBarras;                
            }

            this.RateioMovto.Classes.Clear();
            foreach (IRateioItem ri in vmf.RateioMovto.Classes)
            {
                ri.Ambiente.Transacao = this.Ambiente.Transacao;
                this.RateioMovto.Classes.Add(ri);
            }

            this.RateioMovto.CentroCustosProjetos.Clear();
            foreach (IRateioItem ri in vmf.RateioMovto.CentroCustosProjetos)
            {
                ri.Ambiente.Transacao = this.Ambiente.Transacao;
                this.rateioMovto.CentroCustosProjetos.Add(ri);
            }
        }

        /// <summary>
        /// Método para assimilar dados para o Documento
        /// </summary>
        public void Assimilar(ILiquidacaoEstornoFormaPagto lefo)
        {
            IFormaMovInfoCartao info = lefo.FormaMovInfoCartao;
            info.Ambiente.Transacao = this.Ambiente.Transacao;

            ILiquidacaoEstorno le = lefo.LiquidacaoEstorno;
            le.Ambiente.Transacao = this.Ambiente.Transacao;

            this.DataEmissao = le.DataMovimento;
            this.PagarReceber = PagarReceberTipo.MovimentoCartao;
            this.IdEntidade = Convert.ToInt32(lefo.IdInstituicaoFinanceira);
            this.Valor = lefo.Valor;
            this.IdCondicaoPagamento = AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.CondicaoPagamentoGeracaoDoctoMovCartao, this.Ambiente).ValorInt;
            SetDadosOrigemGerador(le);

            if (this.TipoDocumento != null && !TipoDocumento.Sequencial)
            {
                this.NumeroDocumento = lefo.IdLiquidacaoEstorno.ToString();
                if (!String.IsNullOrEmpty(info.NumeroAutorizacaoCartao))
                    this.NumeroDocumento = info.NumeroAutorizacaoCartao;
            }

            string docsHistorico = null;
            foreach (DocumentoMovto dm in lefo.LiquidacaoEstorno.Documentos)
            {
                dm.Ambiente.Transacao = this.Ambiente.Transacao;
                if (!String.IsNullOrEmpty(docsHistorico))
                    docsHistorico += ", ";

                docsHistorico += String.Format("{0}/{1}({2})", dm.DocumentoParcela.Documento.NumeroDocumento, dm.DocumentoParcela.NumeroParcela, dm.DocumentoParcela.IdDocumento);
            }

            this.historico = String.Format("Documento gerado pela liquidação com cartão, nº aut.: {0}, ref. Doc(s) {1}.", info.NumeroAutorizacaoCartao, docsHistorico);


            // Diminuindo o valor da forma recebida
            double valorParcela = Funcoes.Arredondar((Valor / info.NumeroParcelas), 2);
            double valorSomado = 0;
            DateTime data = le.DataMovimento;
            int idFormaCobrancaDefault = AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.FormaCobrancaGeracaoDoctoMovCartao, this.Ambiente).ValorInt;

            IFormaPagamentoFilial fpf = lefo.FormaPagamento.FormaPagamentoFilial;
            IRegraCartao rc = (fpf != null) ? fpf.RegraCartao : null;

            Parcelas.Clear();
            for (int nParc = 1; nParc <= info.NumeroParcelas; nParc++)
            {
                data = data.AddDays(rc.DiaLancamento);

                // Buscando uma data util, se requerida
                if (rc.DiasUteis)
                {
                    while (!Feriado.DiaUtil(data, this.Ambiente))
                        data = data.AddDays(1);
                }

                if (nParc == info.NumeroParcelas)
                    valorParcela = Funcoes.Arredondar((Valor - valorSomado), 2);
                else
                    valorSomado += valorParcela;

                Parcelas.AddParcela(idFormaCobrancaDefault, data, valorParcela, 0, this.Ambiente.Transacao);
            }          

            //Setar dados p doc. cartão     
            if (info.IdBandeiraCartao != null)
                this.IdBandeiraCartao = Convert.ToInt32(info.IdBandeiraCartao);

            this.NumeroAutorizacao = info.NumeroAutorizacaoCartao;
            this.TipoCartao = info.Tipo;

            double percTaxa = rc.UsaTaxaCartao ? rc.GetPercentualTaxa(info.NumeroParcelas, info.IdBandeiraCartao, this.DataEmissao) : 0;
            double taxa = (percTaxa * 100);
            this.Taxa = taxa;

            int count = this.ItensFinanceiros.Count;
            if (count == 0)
                return;                        
            
            if (taxa == 0)
            {
                this.ItensFinanceiros.Clear();
                return;
            }

            (ItensFinanceiros[0]).Valor = taxa;       
        }

        /// <summary>
        /// Método para assimilar dados
        /// </summary>        
        public void Assimilar(int idTipo, IDistribuicao d)
        {
            this.tipoDocumento.Id = idTipo;
            this.dataEmissao = d.DataEmissao;

            if (this.IdEntidade != d.IdEntidade)
                this.entidade.Id = d.IdEntidade;

            this.valor = d.ValorTotal;
            this.indiceConversao.Id = AmbienteServidorGlobal.IndiceEconomicoDefault.IdIndiceEconomico;
            this.indiceEconomico.Id = d.IdIndiceEconomico;
            this.condicaoPagamento.Id = (int)d.IdCondicaoPagamento;                   
            this.historico = d.Historico;
            SetNumeroDocumento(d.NumeroDocumento.ToString());

            //Setar variavel p/recalcular valor convertido
            this.recalcularValorConvertido = true;            
        }

        /// <summary>
        /// Método para assimilar dados
        /// </summary>        
        public void Assimilar(ICTeDocumento cd)
        {
            IOperacaoTransporte ot = (IOperacaoTransporte)cd.OperacaoTransporte.OperacaoPadrao;            
            IOperacaoPagarReceber of = (IOperacaoPagarReceber)ot.FinanceiroReceber.OperacaoPadrao;

            this.tipoDocumento.Id = (int)of.IdTipoDocumento;
            this.dataEmissao = cd.DataEmissao;
            this.entidade.Id = cd.IdEntidadeTomador;
            this.valor = cd.ValorTotal;
            this.indiceConversao.Id = AmbienteServidorGlobal.IndiceEconomicoDefault.IdIndiceEconomico;
            this.indiceEconomico.Id = this.indiceConversao.Id;
            this.condicaoPagamento.Id = (int)cd.IdCondicaoPagamento;
            this.historico = cd.Historico;
            SetNumeroDocumento(cd.NumeroDocumento.ToString());

            //Setar variavel p/recalcular valor convertido
            this.recalcularValorConvertido = true;            
        }

        /// <summary>
        /// Método para assimilar dados
        /// </summary>        
        public void Assimilar(INFSeDocumento nd)
        {
            IOperacaoNotaFiscalServico onfs = (IOperacaoNotaFiscalServico)nd.Operacao.OperacaoPadrao;
            IOperacaoPagarReceber of = (IOperacaoPagarReceber)onfs.FinanceiroReceber.OperacaoPadrao;

            this.tipoDocumento.Id = (int)of.IdTipoDocumento;
            this.dataEmissao = nd.DataEmissao;
            this.entidade.Id = nd.IdEntidadeTomador;
            this.valor = nd.ValorTotal;
            this.indiceConversao.Id = AmbienteServidorGlobal.IndiceEconomicoDefault.IdIndiceEconomico;
            this.indiceEconomico.Id = this.indiceConversao.Id;
            this.condicaoPagamento.Id = (int)nd.IdCondicaoPagamento;
            this.historico = nd.Historico;
            SetNumeroDocumento(nd.NumeroDocumento.ToString());


            //Setar variavel p/recalcular valor convertido
            this.recalcularValorConvertido = true;            
        }
        
        /// <summary>
        /// Método para assimilar dados do contrato
        /// </summary>
        public void Assimilar(Hashtable htValores, ArrayList lParcelas, IContrato c, IContratoFinanceiro cf)
        {
            int idOperacao = Convert.ToInt32(htValores["IdOperacao"]);
            double totalObjeto = Convert.ToDouble(htValores["TotalObjeto"]);
            double totalFinanceiro = Convert.ToDouble(htValores["TotalFinanceiro"]);
            DateTime dataEmissao = Convert.ToDateTime(htValores["DataEmissao"]);


            TipoGrupoContrato tipoGrupo = cf.GrupoContratoTipo;
            object idCondicao = (tipoGrupo == TipoGrupoContrato.Geral) ? cf.IdCondicaoPagamentoGeral : cf.IdCondicaoPagamentoParcelamento;
            double v = (lParcelas != null) ? Funcoes.Arredondar(totalFinanceiro, 2) : cf.Valor;

            this.dataEmissao = dataEmissao;
            this.pagarReceber = c.PagarReceber;
            this.IdCondicaoPagamento = Convert.ToInt32(idCondicao);
            this.IdEntidade = c.IdEntidadeCobranca;
            this.IdIndiceEconomico = cf.IdIndiceEconomico;
            this.IdSituacao = SituacaoDocumento.Aberto;
            this.valor = v;
            this.AplicarOperacao(idOperacao);
            this.numeroDocumento = c.NumeroContrato;
            this.SetDadosOrigemGerador(cf);

            this.Tributos.Clear();
            this.Comissionados.Clear();


            // Parcelas
            this.Parcelas.Clear();
            if (lParcelas == null)
            {
                foreach (IContratoFinanceiroParcela cfp in cf.Parcelas)
                {
                    cfp.Ambiente.Transacao = this.Ambiente.Transacao;
                    this.Parcelas.AddParcela(cfp.IdFormaCobrancaParcela, cfp.IdPortadorParcela, false, cfp.DataVencimento, cfp.Valor, cfp.NumeroParcela, this.Ambiente.Transacao);
                }
            }
            else
            {
                foreach (IContratoFinanceiroParcela cfp in lParcelas)
                {
                    IDocumentoParcela dp = (IDocumentoParcela)this.Parcelas.AddParcela(cfp.IdFormaCobrancaParcela, cfp.IdPortadorParcela, false, cfp.DataVencimento, cfp.Valor, cfp.NumeroParcela,  this.Ambiente.Transacao);
                    dp.SetNumeroParcela(cfp.NumeroParcela);
                }
            }

            if (this.RateioMovto != null)
                this.RateioMovto.Limpar();

            double valorBase = 0;
            IRateioMovto rmObjeto = new RateioMovto(this);
            bool devedor = (c.PagarReceber == PagarReceberTipo.Pagar);

            for (int i = 0; i < c.ObjetosContrato.Count; i++)
            {
                IObjetoContrato oc = (IObjetoContrato)c.ObjetosContrato[i];
                oc.Ambiente.Transacao = this.Ambiente.Transacao;

                if (oc.GrupoContrato.TipoGrupoContrato != tipoGrupo)
                    continue;

                valorBase = ((oc.ValorTotal / totalObjeto) * totalFinanceiro);
                valorBase = Funcoes.Arredondar(valorBase, 8);

                int idClasse = (oc.IdClasse != null) ? Convert.ToInt32(oc.IdClasse) : 0;
                int idCentroCusto = (oc.IdCentroCusto != null) ? Convert.ToInt32(oc.IdCentroCusto) : 0;
                int idProjeto = (oc.IdProjetos != null) ? Convert.ToInt32(oc.IdProjetos) : 0;

                rmObjeto.GerarRateio(oc.IdFilial, idClasse, idCentroCusto, idProjeto, true, oc.ValorTotal);
                rmObjeto.AcumuladorAplicar();

                this.RateioMovto.AplicarRateio(oc.IdFilial, valorBase, 0, rmObjeto, devedor, false, TipoCalculoValorRateio.Percentual);
            }

            this.RateioMovto.AcumuladorAplicar();
        }

     
        /// <summary>
        /// Método para setar ação (Pagar ou Receber) default
        /// </summary>
        public void SetDefaultAcao()
        {
            PagarReceberTipo tipoPadrao = (PagarReceberTipo)AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.AcaoPadraoDoctoFinanceiro, this.Ambiente).ValorEnumerado;
            this.PagarReceber = (tipoPadrao != 0) ? tipoPadrao : PagarReceberTipo.Receber;
        }

        /// <summary>
        /// Método para setar o numero do documento
        /// </summary>
        public void SetNumeroDocumento(string numero)
        {
            this.numeroDocumento = numero;
        }

        /// <summary>
        /// Método para retornar a situação do documento financeiro
        /// </summary>
        public SituacaoDocumento RetornarSituacaoDocumento()
        {
            if (this.IdSituacao == SituacaoDocumento.Cancelado)
                return this.IdSituacao;

            if (ValorLiquidado.Equals((double)0) && ValorRevertido.Equals((double)0))
                return SituacaoDocumento.Aberto;

            double total = (ValorLiquidado + ValorCancelado + ValorRevertido);

            //Arrendondar no max. de casas decimais e no minimo, para depois comparar os valores
            //para "tentar" evitar as diferenças da somatoria
            double valorComparacao1 = Funcoes.Arredondar(total, 8);
            double valorComparacao2 = Funcoes.Arredondar(total, 2);

            //Considerar o valor parcela = valor - tributos retidos
            double v = this.ValorParcelado;
            if (valorComparacao1.Equals(v) || valorComparacao2.Equals(v))
                return SituacaoDocumento.Liquidado;

            return SituacaoDocumento.LiquidadoParcial;
        }

        /// <summary>
        /// Método para carregar itens financeiros padrões
        /// </summary>
        public void CarregarItensFinanceiroPadrao()
        {
            //Verifica se aplica o item financeiro padrão
            if (!PermiteItemFinanceiroPadrao() && this.PagarReceber != PagarReceberTipo.MovimentoCartao)
            {
                //Caso não se aplica, limpar a lista
                this.ItensFinanceiros.Clear();
                return;
            }

            //Caso aplica o item, verifica se já foi informado algum
            if (this.ItensFinanceiros.Count > 0)
                return;

            IObjectQuery c = AmbienteServidorGlobal.ObjetoNegocioFactory.InstanciarQuery(typeof(IItemFinanceiro), this.Ambiente);
            c.AddConstraint(Operator.Equals, "Padrao", 1);

            IListBase l = new ListBase(this.Ambiente);
            AmbienteServidorGlobal.ObjetoNegocioFactory.RetornarConsulta(c, l);

            if (l.Count == 0)
                return;

            foreach (IItemFinanceiro item in l)
                CarregarItemFinanceiro(item.IdItemFinanceiro);
        }

        #endregion

        #region Métodos para cancelamento do doc. financeiro
       
        /// <summary>
        /// Método para persitir o cancelamento do documento financeiro
        /// </summary>
        public List<IDocumentoParcela> PersistirCancelamentoDocumento(string motivo, DateTime dataMovto, List<IDocumentoParcela> lParcelas, IGeneratorDataSource dog, TransacaoBase transacao)
        {
            this.cancelaDocumento = false;
            double totalCancelado = 0;
            List<IDocumentoParcela> lParcelaSel = new List<IDocumentoParcela>();

            IList lSel = lParcelas.Where(p => p.Marcado == true).ToList();
            for (int i = 0; i < lSel.Count;i++)
            {
                IDocumentoParcela dpc = (IDocumentoParcela)lSel[i];
                dpc.Ambiente.Transacao = transacao;

                lParcelaSel.Add(dpc);


                //Este valor para atualizar o valor da parcela cancela na FINDOCTOCANCELADOPARCELA
                dpc.ValorRateioCancelado = dpc.ValorPendente;
                

                //Atualiza a situação da parcela da lista de parcelas do doc. selecionado                
                IDocumentoParcela dp = this.Parcelas[dpc.IndexParcela];
                IDocumentoParcelaManutencao dpm = (IDocumentoParcelaManutencao)Factory.Retornar(typeof(IDocumentoParcelaManutencao), this.Ambiente, transacao, dp.IdDocumentoParcela);
                dpm.Ambiente.Transacao = transacao;

                dpm.IdSituacao = SituacaoDocumento.Cancelado;
                if (dp.ValorLiquidado > 0)
                    dpm.IdSituacao = SituacaoDocumento.Liquidado;

                totalCancelado += dp.ValorPendente;
                dpm.ValorCancelado = Funcoes.Arredondar((dpm.ValorCancelado + dp.ValorPendente), 2);
                dpm.Persist(transacao);
            }          
          
            if (totalCancelado > 0)
                totalCancelado = Funcoes.Arredondar(totalCancelado, 2);

            //Atualizar o documento
            IDocumentoManutencao dm = (IDocumentoManutencao)Factory.Retornar(typeof(IDocumentoManutencao), this.Ambiente, transacao, this.IdDocumento);
            dm.Ambiente.Transacao = transacao;
            dm.ValorCancelado = Funcoes.Arredondar((this.ValorCancelado + totalCancelado), 2);

            if (this.IdSituacao == SituacaoDocumento.Aberto)
                dm.IdSituacao = (dm.ValorPendente == 0) ? SituacaoDocumento.Cancelado : SituacaoDocumento.Aberto;
            else if (this.IdSituacao == SituacaoDocumento.LiquidadoParcial)
                dm.IdSituacao = (dm.ValorPendente == 0) ? SituacaoDocumento.Liquidado : SituacaoDocumento.LiquidadoParcial;

            dm.Persist(transacao);

            //Salvar o documento cancelado
            IDoctoCancelado dc = PersistirDoctoCancelado(motivo, dataMovto, dog, lSel, transacao);

            //Cancelar o rateio
            CancelarComissaoRateio(dc, transacao);

            return lParcelaSel;
        }

        /// <summary>
        /// Método para persistir o documento cancelado
        /// </summary>
        private IDoctoCancelado PersistirDoctoCancelado(string motivo, DateTime dataMovto, IGeneratorDataSource dog, IList lParcelasSel, TransacaoBase transacao)
        {
            IDoctoCancelado dc = (IDoctoCancelado)Factory.Instanciar(typeof(IDoctoCancelado), this.Ambiente, transacao);
            dc.IdDocumento = this.IdDocumento;
            dc.Motivo = motivo;
            dc.DataMovto = dataMovto;

            if (dog != null)
                dc.SetDadosOrigemGerador(dog);

            foreach (IDocumentoParcela dp in lParcelasSel)
            {
                dp.Ambiente.Transacao = transacao;
                dc.Parcelas.AddDoctoCanceladoParcela(dp, transacao);
            }

            dc.Persist(transacao);

            //Recarregar o objeto para atualizar as parcelas cancelas persistidas
            int id = dc.IdDoctoCancelado;
            dc = (IDoctoCancelado)Factory.Retornar(typeof(IDoctoCancelado), this.Ambiente, transacao, id);

            return dc;            
        }

        /// <summary>
        /// Método para cancelar comissão e rateio do documento financeiro
        /// </summary>
        private void CancelarComissaoRateio(IDoctoCancelado dc, TransacaoBase transacao)
        {
            //Recarregar o objeto Documento para considerar as atualizações do cancelamento
            IDocumento d = (IDocumento)Factory.Retornar(typeof(IDocumento), this.Ambiente, transacao, this.IdDocumento);
            d.Ambiente.Transacao = transacao;

            //Carregar o objeto de rateio do doc. origem p/ contrapartida
            IRateioMovto rmOrigem = d.RateioMovto;
            rmOrigem.Ambiente.Transacao = transacao;

            //Carregar as parcelas que foram canceladas, pois esta lista servirá de base para busca do rateio
            foreach (IDoctoCanceladoParcela dcp in dc.Parcelas)
            {
                dcp.Ambiente.Transacao = transacao;
                d.ParcelasCancelada.Add(dcp);
            }


            for (int i = 0; i < d.ParcelasCancelada.Count; i++)
            {
                IDoctoCanceladoParcela dcp = (IDoctoCanceladoParcela)d.ParcelasCancelada[i];
                dcp.Ambiente.Transacao = transacao;

                d.SetCancelamentoRateio(i, true, dcp.ValorPendente);            
                CancelarRateio(d, dcp, rmOrigem, transacao);

                IDocumentoParcela dp = dcp.DocumentoParcela;
                AtualizarComissaoCancelada(d, dp, transacao);
            }
        }

        /// <summary>
        /// Método para efetuar contrapartida no rateio no cancelamento do documento
        /// </summary>
        private void CancelarRateio(IDocumento d, IDoctoCanceladoParcela dcp, IRateioMovto rm, TransacaoBase transacao)
        {
            IRateioMovto rmNew = new RateioMovto(d);
            rmNew.Ambiente.Transacao = transacao;

            double valorParcelado = 0;
            if (rm.Classes.Count > 0)
            {
                valorParcelado = ((IDocumento)rm.Classes.RateioMovto.DadosRateio).ValorParcelado;
                FinanceiroUtil.GerarMovimentoRateio(ref rmNew, rm, dcp.IdFilial, dcp.ValorPendente, true, d.PagarReceber, valorParcelado);             
            }

            if (rm.CentroCustosProjetos.Count > 0)
            {
                valorParcelado = ((IDocumento)rm.CentroCustosProjetos.RateioMovto.DadosRateio).ValorParcelado;
                FinanceiroUtil.GerarMovimentoRateio(ref rmNew, rm, dcp.IdFilial, dcp.ValorPendente, false, d.PagarReceber, valorParcelado);
            }

            if (rmNew != null)
                rmNew.AcumuladorAplicar();

            //Inverter a natureza do rateio para contrapartida
            AlterarNaturezaRateio(rmNew.Classes);
            AlterarNaturezaRateio(rmNew.CentroCustosProjetos);

            rmNew.PersistirRateio(transacao);
        }

        /// <summary>
        /// Método para alterar a natureza do rateio
        /// </summary>        
        private void AlterarNaturezaRateio(IRateioItemLista l)
        {
            foreach (IRateioItem ri in l)
                ri.NaturezaDevedora = !ri.NaturezaDevedora;
        }

        /// <summary>
        /// Método para atualizar lançamento/movimento de comissão para o cancelamento do documento financeiro
        /// </summary>
        private void AtualizarComissaoCancelada(IDocumento d, IDocumentoParcela dp, TransacaoBase transacao)
        {
            if (this.pagarReceber == PagarReceberTipo.Pagar)
                return;

            IDadosComissao dc = (IDadosComissao)d;

            ICalculoComissao cc = (ICalculoComissao)Factory.Instanciar(typeof(ICalculoComissao), this.Ambiente, transacao);
            cc.SetComissao(transacao, dc, 0);
            cc.AtualizarComissaoCancelada(dp, transacao);
        }

        /// <summary>
        /// Método para setar o cancelamento do rateio
        /// </summary>
        public void SetCancelamentoRateio(int index, bool cancelar, double valorCancelado)
        {
            this.indexCancelamento = index;
            this.cancelaDocumento = cancelar;
            this.valorRateioCancelado = valorCancelado;
        }

        #endregion

        #region ILanguageStrings

        public void GetLangBaseItems(List<LangItemId> l)
        {
            l.Add(new LangStringItemId(this, 1, "Número do documento"));
            l.Add(new LangStringItemId(this, 2, "Verifique os percentuais informado para os comissionados."));
            l.Add(new LangStringItemId(this, 5, "A ação não pode ser alterada por haver comissionado(s) com movimento fechado."));
            l.Add(new LangStringItemId(this, 6, "O tipo de documento para o movimento de cartão não pode ter sequencial automático."));
        }

        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gentle.Framework;
using Projeto.Geral;
using Projeto.Geral.Enumerado;
using Projeto.Geral.EnumeradoObjeto;
using Projeto.Servidor.Framework;
using Projeto.Servidor.Interface.Framework;
using Projeto.Servidor.Interface.AcessoGlobal;
using Projeto.Servidor.Interface.GestaoFinanceira;
using Projeto.Servidor.ObjetosNegocio.AcessoGlobal;
using Projeto.Servidor.Strangler.Common;
using Projeto.Servidor.Strangler.GestaoFinanceira.DTOs; 

namespace Projeto.Servidor.ObjetosNegocio.GestaoFinanceira
{
    /// <summary>
    /// Classe para objeto de negócio DocumentoFinanceiro base
    /// </summary>
    public class DocumentoFinanceiroBase : ObjectGenerator, IDocumentoFinanceiroBase
    {
        protected string numeroDocumento;
        protected string historico;
        private string numeroCedente;
        protected double valor;
        private double valorConvertido;
        protected DateTime dataEmissao;
        private DateTime dataInclusao;
        private DateTime horaInclusao;
        protected PagarReceberTipo pagarReceber;
        protected RateioMovto rateioMovto;
        protected Lookup entidade = new Lookup(typeof(IEntidade), "IdEntidade");
		protected Lookup operacao = new Lookup(typeof(IOperacao), "IdOperacao");
        protected Lookup tipoDocumento = new Lookup(typeof(ITipoDocumento), "IdTipoDocumento");
        protected Lookup condicaoPagamento = new Lookup(typeof(ICondicaoPagamento), "IdCondicaoPagamento");
        protected Lookup indiceEconomico = new Lookup(typeof(IIndiceEconomico), "IdIndiceEconomico");
        protected Lookup indiceConversao = new Lookup(typeof(IIndiceEconomico), "IdIndiceEconomico");
                

        //Variaveis controle
        protected bool recalcularValorConvertido;                
        private int idPortador;        
        private Lookup formaCobranca = new Lookup(typeof(IFormaCobranca), "IdFormaCobranca");

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public DocumentoFinanceiroBase()
        {
            this.dataEmissao = AmbienteServidorGlobal.DataSistema;                        
        }

        /// <summary>
        /// Construtor padrão carregado do banco
        /// </summary>
        public DocumentoFinanceiroBase(int IdFilial, int IdEntidade, int IdTipoDocumento, int IdOperacao, int IdCondicaoPagamento, int IdIndiceEconomico, int IdIndiceConversao,
                                       string NumeroDocumento, string NumeroCedente, string Historico, double Valor, double ValorConvertido, DateTime DataEmissao, DateTime DataInclusao, DateTime HoraInclusao,
                                       PagarReceberTipo PagarReceber, int IdOrigem, ProcessoOrigem IdProcessoOrigem)
                            
                                       : base(IdOrigem, IdProcessoOrigem, IdFilial)
        {
            this.Ambiente.IdFilial = IdFilial;      
            this.entidade.Id = IdEntidade;
            this.operacao.IdObject = IdOperacao;
            this.tipoDocumento.IdObject = IdTipoDocumento;
            this.condicaoPagamento.IdObject = IdCondicaoPagamento;
            this.indiceEconomico.IdObject = IdIndiceEconomico;
            this.indiceConversao.Id = IdIndiceConversao;
            this.numeroDocumento = NumeroDocumento;
            this.numeroCedente = NumeroCedente;
            this.historico = Historico;
            this.valor = Valor;
            this.valorConvertido = ValorConvertido; 
            this.dataEmissao = DataEmissao;
            this.dataInclusao = DataInclusao;
            this.horaInclusao = HoraInclusao;
            this.pagarReceber = PagarReceber;
        }

        #region Métodos protegidos a ser sobrescrito

        /// <summary>
        /// Sobrescrevendo o método disparado antes da persistencia
        /// </summary>        
        protected override void OnBeforeExecutarPersistir(TransacaoBase transacao)
        {
            if (!IsPersisted)
            {
                this.dataInclusao = AmbienteServidorGlobal.DataSistema;
                this.horaInclusao = AmbienteServidorGlobal.HoraSistema;            
            }

            base.OnBeforeExecutarPersistir(transacao);
        }

        /// <summary>
        /// Método para validar a operação do documento
        /// </summary>        
        protected virtual bool ValidarOperacao(int idOperacao)
        {
            if (idOperacao == 0 || this.PagarReceber == 0)
                return true;

            OperacaoTipo ot = OperacaoTipo.Receber;
            if (this.PagarReceber > 0 && this.PagarReceber != PagarReceberTipo.Receber)
                ot = (this.PagarReceber == PagarReceberTipo.Pagar) ? OperacaoTipo.Pagar : OperacaoTipo.MovimentoCartao;

            bool valido = AmbienteServidorGlobal.AcessoGlobalFactory.ValidarTipoOperacao(idOperacao, ot, this.Ambiente);

            return valido;
        }

        /// <summary>
        /// Método para carregar complemento da entidade
        /// </summary>
        protected virtual bool CarregarComplementoEntidade(int idEntidade)
        {
            if (IsPersisted || idEntidade == 0)
                return false;

            CarregarCondicaoPagamentoDefault();
            ValidarNumeroDocumento(this.NumeroDocumento);

            return true;
        }

        /// <summary>
        /// Método para retornar o Id da condição pagamento válida
        /// </summary>        
        protected virtual int GetIdCondicaoPagtoValida(int idCondicaoPagto, PagarReceberTipo tipo)
        {
            return 0;
        }

        /// <summary>
        /// Método para verificar se a condição de pagamento informado pode ser aplicada
        /// </summary>
        protected virtual bool PermiteAplicarCondicaoPagto(int idCondicaoPagto)
        {
            return true;
        }

        /// <summary>
        /// Método para carregar as parcelas conforme condição de pagamento
        /// </summary>
        protected virtual void SetParcelasCondicaoPagto(bool refazerCondicaoPagto)
        {
        }

        /// <summary>
        /// Método para setar portador da parcela
        /// </summary>        
        protected virtual void SetPortadorParcela(int idPortador)
        {
        }

        /// <summary>
        /// Método para setar forma da cobrança da parcela
        /// </summary>        
        protected virtual void SetFormaCobrancaParcela(int idFormaCobranca)
        {
        }

        /// <summary>
        /// Método para recalcular tributo e rateio com alteração do valor
        /// </summary>        
        protected virtual void RecalcularTributoRateio()
        {
        }

        /// <summary>
        /// Método para carregar comissionado default
        /// </summary>        
        protected virtual void CarregarComissionadoPadrao(int idEntidade)
        {
        }

        /// <summary>
        /// Método para carregar o item financeiro da operacao
        /// </summary>        
        protected virtual void CarregaItemFinanceiroOperacao()
        {

        }

        /// <summary>
        /// Método para retornar o Id da origem geradora
        /// </summary>        
        protected virtual int GetIdOrigemGerador()
        {
            return 0;
        }

        /// <summary>
        /// Método para retornar o processo origem gerador
        /// </summary>        
        protected virtual ProcessoOrigem GetProcessoOrigemGerador()
        {
            return 0; 
        }

        /// <summary>
        /// Método para retornar o Id do documento financeiro
        /// </summary>        
        protected virtual int GetIdDocumentoFinanceiro()
        {
            return 0;
        }       

        #endregion

        #region Métodos protegidos a ser sobrescrito rateio

        /// <summary>
        /// Método para retornar o valor do rateio
        /// </summary>        
        protected virtual double RetornarValorRateio(TipoRateioItem tipo)
        {
            return this.Valor;
        }

        /// <summary>
        /// Método para retornar o processo origem do rateio
        /// </summary>        
        protected virtual ProcessoOrigem GetProcessoOrigemRateio()
        {
            return ProcessoOrigem.Documento;
        }

        /// <summary>
        /// Método para retornar a matriz de regra do rateio
        /// </summary>        
        protected virtual IRateioRegraOrigem[] GetMatrizRegraOrigemRateio()
        {
            return null;   
        }

        /// <summary>
        /// Método para retornar o objeto RateioRegraOrigem
        /// </summary>
        protected virtual IRateioRegraOrigem RetornarRateioRegraOrigem(int index)
        {
            return null;
        }

        /// <summary>
        /// Método para retornar o valor de validação do rateio
        /// </summary>        
        protected virtual double GetValorValidacaoRateio()
        {
            return 0;
        }

        /// <summary>
        /// Método para retornar o valor digitado no rateio
        /// </summary>        
        protected virtual double GetValorDigitadoRateio()
        {
            return 0;
        }

        #endregion

        #region Métodos protegidos e privados

        /// <summary>
        /// Sobrescrevendo método que indicando quando ha ambiente assimiliado
        /// </summary>
        protected override void AmbienteAssimilado()
        {
            base.AmbienteAssimilado();

            //Verificar se o ambiente foi carregado corretamente
            //pois p. ex. ao executar o ajuste de sequencial não é passado
            //ambiente então consequentemente se houver algo implementado
            //neste método que dependa do ambiente pode gerar erro
            if (!this.Ambiente.Carregado)
                return;

            if (!IsPersisted)
            {
                this.indiceEconomico.SetObjeto(AmbienteServidorGlobal.IndiceEconomicoDefault);
                this.indiceConversao.SetObjeto(AmbienteServidorGlobal.IndiceEconomicoDefault);
                this.IdPortador = AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.PortadorPadrao, this.Ambiente).ValorInt;
            }
        }

        /// <summary>
        /// Método para validar a entidade
        /// </summary>
                protected void ValidarEntidade()
        {
            if (this.IdEntidade == 0)
                return;

            string sTipo = AmbienteServidorGlobal.AcessoGlobalFactory.GetLabelEnum(typeof(PagarReceberTipo), this.PagarReceber, this.Ambiente);
            string msg = null;

            try
            {
                // Consome a API do .NET 8 via Helper compartilhado
                var entidade = FinancialStranglerHelper.GetJson<EntidadeDto>("entidade/" + this.IdEntidade);

                if (entidade == null)
                {
                    throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, "Entidade informada não existe na API principal.");
                }

                if (this.PagarReceber == PagarReceberTipo.Pagar && !entidade.IsFornecedor)
                {
                    msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Fornecedor'.", sTipo);
                    throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
                }

                if (this.PagarReceber == PagarReceberTipo.Receber && !entidade.IsCliente)
                {
                    msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Cliente'.", sTipo);
                    throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
                }

                if (this.PagarReceber == PagarReceberTipo.MovimentoCartao && !entidade.IsInstituicaoFinanceira)
                {
                    msg = String.Format("Para documento do tipo '{0}', a entidade deve ser do tipo 'Instituição financeira'.", sTipo);
                    throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
                }
            }
            catch (Exception e)
            {
                if (e is ObjetoNegocioException) throw;
                throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, "Erro ao validar entidade via API: " + e.Message);
            }
        }
        /// <summary>
        /// Método para validar o número do documento informado
        /// </summary>        
        protected bool ValidarNumeroDocumento(string numeroDoc)
        {
            //Validação numero do documento somente será verifica para origem = doc. financeiro
            if (this.IdProcessoOrigem != ProcessoOrigem.Documento)
                return true;

            if (this.IsPersisted)
                return true;

            if (!AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.VerificarDocumentoExiste, this.Ambiente).ValorBool)
                return true;

            if (this.TipoDocumento == null || this.IdEntidade == 0 || String.IsNullOrEmpty(numeroDoc))
                return true;

            if (this.TipoDocumento.Sequencial)
                return true;

            IObjectQuery c = AmbienteServidorGlobal.ObjetoNegocioFactory.InstanciarQuery(typeof(IDocumento), this.Ambiente);
            c.AddConstraint(Operator.NotEquals, "IdSituacao", SituacaoDocumento.Cancelado);
            c.AddConstraint(Operator.Equals, "IdFilial", this.Ambiente.IdFilial);
            c.AddConstraint(Operator.Equals, "IdEntidade", this.IdEntidade);
            c.AddConstraint(Operator.Equals, "IdTipoDocumento", this.IdTipoDocumento);
            c.AddConstraint(Operator.Equals, "NumeroDocumento", numeroDoc);
            c.AddConstraint(Operator.Equals, "PagarReceber", this.PagarReceber);
            c.AddConstraint(Operator.NotEquals, "IdDocumento", GetIdDocumentoFinanceiro());

            IList l = AmbienteServidorGlobal.ObjetoNegocioFactory.RetornarConsulta(c);
            if (l.Count == 0)
                return true;

            string s = String.Format("O número do documento informado '({0})' já está cadastrado. Verifique.", numeroDoc);
            throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, s);
        }

        /// <summary>
        /// Método para aplicar a Operação
        /// </summary>
        protected void AplicarOperacao(int idOperacao)
        {
            if (idOperacao == 0)
                return;

            IOperacao op = idOperacao.Equals(IdOperacao) ? this.Operacao : AcessoGlobal.Operacao.Retornar(idOperacao, this.Ambiente);
            AplicarOperacao(op);
        }

        /// <summary>
        /// Método para aplicar a definição de Operação ao Receber
        /// </summary>		
        protected void AplicarOperacao(IOperacao operacao)
        {
            try
            {
                if (this.Operacao == null || !this.Operacao.Equals(operacao))
                    this.Operacao = operacao;

                IOperacaoDocumentoFinanceiroBase or = (IOperacaoDocumentoFinanceiroBase)operacao.OperacaoPadrao;

                if (or.HistoricoPadrao != null)
                    this.Historico = or.HistoricoPadrao.TextoPadrao;

                this.IdFormaCobranca = 0;

                if (pagarReceber == PagarReceberTipo.MovimentoCartao)
                    this.IdFormaCobranca = AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.FormaCobrancaGeracaoDoctoMovCartao, this.Ambiente).ValorInt;

                if (or.IdTipoDocumento != null)
                    this.IdTipoDocumento = (int)or.IdTipoDocumento;
                
                RateioMovto.Limpar();               
            }
            catch
            {
                this.IdOperacao = 0;
                this.Historico = null;
                this.IdTipoDocumento = 0;

                throw;                
            }
        }

        /// <summary>
        /// Método que aplica uma Condição de Pagamento, gerando as parcelas
        /// a partir desta condição
        /// </summary>
        protected void AplicarCondicaoPagamento(bool refazerCondicaoPagto)
        {
            recalcularValorConvertido = true;
            CalcularValorConvertido();

            if (this.DataEmissao == DateTime.MinValue)
                return;

            SetParcelasCondicaoPagto(refazerCondicaoPagto);            
        }

        /// <summary>
        /// Método para recalcular o valor convertido entre os índices informados
        /// </summary>
        protected bool CalcularValorConvertido()
        {
            if (!recalcularValorConvertido || this.IdIndiceEconomico == 0)
                return true;

            try
            {
                IndiceConversor ic = (IndiceConversor)Factory.Instanciar(typeof(IIndiceConversor), this.Ambiente);
                ic.ConverterIndice(IndiceEconomico, IndiceConversao, Valor, DataEmissao);

                valorConvertido = ic.ValorConvertido;
                recalcularValorConvertido = false;
            }
            catch
            {
                throw;
            }

            return true;
        }       

        /// <summary>
        /// Método para carregar a condição de pagamento default da entidade
        /// </summary>
                private void CarregarCondicaoPagamentoDefault()
        {
            bool setPortador = true;

            try
            {
                if (this.IdEntidade == 0 || this.PagarReceber == PagarReceberTipo.MovimentoCartao)
                    return;

                int? idCondicao = null;

                if (this.PagarReceber == PagarReceberTipo.Pagar)
                {
                    var f = FinancialStranglerHelper.GetJson<FornecedorDto>("fornecedor/" + this.IdEntidade);
                    if (f != null && f.IdCondicaoPagamento.HasValue)
                    {
                        idCondicao = f.IdCondicaoPagamento;
                    }
                }
                else
                {
                    var ce = FinancialStranglerHelper.GetJson<ClienteDto>("cliente/" + this.IdEntidade);
                    if (ce != null && ce.IdCondicaoPagamento.HasValue)
                    {
                        idCondicao = ce.IdCondicaoPagamento;
                    }
                }

                if (idCondicao.HasValue && idCondicao.Value > 0)
                {
                    this.IdCondicaoPagamento = idCondicao.Value;
                    setPortador = false;
                }
            }
            catch (Exception e)
            {
                setPortador = false;
                ControleErro.TratarErro(e, false, true);
            }
            finally
            {
                if (setPortador)
                    this.IdPortador = GetPortadorDefault();
            }
        }
        /// <summary>
        /// Método para retornar o portador default
        /// </summary>
                protected int GetPortadorDefault()
        {
            int idPortadorDefault = 0;
            try
            {
                var param = FinancialStranglerHelper.GetJson<ParametroDto>("parametro/PortadorPadrao");
                if (param != null && !string.IsNullOrEmpty(param.Valor))
                {
                    idPortadorDefault = Convert.ToInt32(param.Valor);
                }
            }
            catch
            {
                idPortadorDefault = 1; // ID padrão fallback
            }

            return idPortadorDefault;        
        }

        /// <summary>
        /// Método para validar o tipo do documento
        /// </summary>
        private void ValidarTipoDocumento()
        {
            if (this.TipoDocumento == null)
                return;

            string s = this.TipoDocumento.ValidarTipoDocumento(this.IdTipoDocumento, this.PagarReceber);
            if (!String.IsNullOrEmpty(s))
            {
                this.IdTipoDocumento = 0;
                throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, s);
            }

            ValidarNumeroDocumento(this.NumeroDocumento); 
        }

        /// <summary>
        /// Método para setar informações complementares da condição de pagamento
        /// </summary>        
        private void SetComplementoCondicaoPagto(int idCondicaoPagto)
        {
            if (!PermiteAplicarCondicaoPagto(idCondicaoPagto))
                return;
            
            AplicarCondicaoPagamento(true);
            this.IdFormaCobranca = (this.CondicaoPagamento != null) ? Convert.ToInt32(this.CondicaoPagamento.IdFormaCobranca) : 0;
            this.IdPortador = GetPortadorDefault();
        }

        /// <summary>
        /// Método que carrega os dados default de um novo documento
        /// </summary>
        private void CarregarOperacaoDefault()
        {
            int idOperacaoPadraoPagar = (int)AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.OperacaoPagar, this.Ambiente).ValorInt;
            int idOperacaoPadraoReceber = (int)AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.OperacaoReceber, this.Ambiente).ValorInt;
            int idOperacaoPadraoMovimentoCartao = (int)AmbienteServidorGlobal.ParametroServidor.ParametroValor(Parametros.OperacaoMovimentoCartao, this.Ambiente).ValorInt;

            int id = idOperacaoPadraoReceber;
            if(this.PagarReceber > 0 && this.PagarReceber != PagarReceberTipo.Receber)
                id = (this.PagarReceber == PagarReceberTipo.Pagar) ? idOperacaoPadraoPagar : idOperacaoPadraoMovimentoCartao;

            this.IdOperacao = id;            
            CarregarComissionadoPadrao(this.IdEntidade);
            CarregaItemFinanceiroOperacao();
        }

        #endregion

        #region Métodos privados rateio

        /// <summary>
        /// Método para retornar o objeto de rateio de movimento
        /// </summary>
        private void RetornarRateioMovto(TransacaoBase trans)
        {
            rateioMovto = new RateioMovto(this);            
            rateioMovto.GetValorValidacao += new GetCustomValueHandler(rateioMovto_GetValorValidacao);
            rateioMovto.GetValorDigitado += new GetCustomValueHandler(rateioMovto_GetValorDigitado);

            if (IsPersisted)
                rateioMovto.CarregarRateio(trans);
        }

        private void rateioMovto_GetValorDigitado(object sender, GetCustomValueArgs e)
        {
            e.Value = GetValorDigitadoRateio();
        }

        /// <summary>
        /// Método para retornar o valor para validação 
        /// </summary>
        private void rateioMovto_GetValorValidacao(object sender, GetCustomValueArgs e)
        {            
            // O valor usado para validar o rateio final é igual para classe/CC/projeto
            e.Value = GetValorValidacaoRateio(); 
        }	       

        #endregion

        #region Propriedades

        /// <summary>
        /// Propriedade ocm o Id da filial a qual o documento pertence
        /// </summary>
        public virtual int IdFilial
        {
            get
            {
                return this.Ambiente.IdFilial;
            }
            set
            {
                this.Ambiente.IdFilial = value;
            }
        }

        /// <summary>
        /// Propriedade com o Id do Cliente/Fornecedor a qual o documento foi emitido
        /// </summary>
        public virtual int IdEntidade
        {
            get
            {
                return entidade.Id;
            }
            set
            {
                if (entidade.Id.Equals(value))
                    return;

                entidade.Id = value;
                CarregarComplementoEntidade(value);
            }
        }

        /// <summary>
        /// Propriedade com o Id da operação
        /// </summary>
        public virtual int IdOperacao
        {
            get
            {
                return operacao.Id;
            }
            set
            {
                if (operacao.Id.Equals(value))
                    return;

                //Verificar se a operação informada é valida
                bool valido = ValidarOperacao(value);                   

                //Caso operação nao for valida, setar zero, para limpar no lado do cliente
                operacao.Id = valido ? value : 0;
                if (!valido)
                {
                    string msg = String.Format("A operação '{0}' informada não é válida para este documento.", value);
                    throw new ObjetoNegocioException(ObjetoNegocioMensagem.DocumentoFinanceiroValidar, msg);
                }

                //Setar complemento da operação somente se a operação
                //informada for valida
                AplicarOperacao(value);
            }
        }

        /// <summary>
        /// Propriedade ocm o Id do tipo de documento
        /// </summary>
        public virtual int IdTipoDocumento
        {
            get
            {
                return tipoDocumento.Id;
            }
            set
            {
                if (tipoDocumento.Id.Equals(value))
                    return;

                tipoDocumento.Id = value;
                ValidarTipoDocumento();
            }
        }

        /// <summary>
        /// Propriedade com o Id da condição de pagamento
        /// </summary>
        public virtual int IdCondicaoPagamento
        {
            get
            {
                return condicaoPagamento.Id;
            }
            set
            {
                if (condicaoPagamento.Id.Equals(value))
                    return;

                this.condicaoPagamento.Id = GetIdCondicaoPagtoValida(value, this.PagarReceber);
                SetComplementoCondicaoPagto(this.condicaoPagamento.Id);
            }
        }

        /// <summary>
        /// Propriedade com o Id do indice economico
        /// </summary>
        public virtual int IdIndiceEconomico
        {
            get
            {
                return indiceEconomico.Id;
            }
            set
            {
                if (indiceEconomico.Id.Equals(value))
                    return;

                indiceEconomico.Id = value;

                if (value > 0)
                    AplicarCondicaoPagamento(false);
            }
        }

        /// <summary>
        /// Propriedade com o Id do Indice para qual será convertido o valor
        /// </summary>
        public virtual int IdIndiceConversao
        {
            get
            {
                if (indiceConversao.Id == 0)
                    return AmbienteServidorGlobal.IndiceEconomicoDefault.IdIndiceEconomico;

                return indiceConversao.Id;
            }
            set
            {
            }
        }

        /// <summary>
        /// Propriedade com o numero do documento
        /// </summary>
        public virtual string NumeroDocumento
        {
            get
            {
                return numeroDocumento;
            }
            set
            {
                if (!ValidarNumeroDocumento(value))
                    return;

                numeroDocumento = value;
            }
        }

        /// <summary>
        /// Propriedade com o número do cedente
        /// </summary>
        public virtual string NumeroCedente
        {
            get
            {
                return numeroCedente;
            }
            set
            {
                numeroCedente = value;
            }
        }

        /// <summary>
        /// Propriedade com o histórico
        /// </summary>
        public virtual string Historico
        {
            get
            {
                return historico;
            }
            set
            {
                historico = value;
            }
        }

        /// <summary>
        /// Propriedade com o valor total do documento
        /// </summary>
        public virtual double Valor
        {
            get
            {
                return valor;
            }
            set
            {
                if (valor.Equals(value))
                    return;

                valor = value;

                RecalcularTributoRateio();
                AplicarCondicaoPagamento(false);
            }
        }

        /// <summary>
        /// Propriedade com o valor convertido do título
        /// </summary>
        public virtual double ValorConvertido
        {
            get
            {
                CalcularValorConvertido();
                return valorConvertido;
            }
        }

        /// <summary>
        /// Propriedade com a data de emissão do documento
        /// </summary>
        public virtual DateTime DataEmissao
        {
            get
            {
                return dataEmissao;
            }
            set
            {
                if (dataEmissao.Equals(value))
                    return;

                dataEmissao = value;

                if (value > DateTime.MinValue)
                    AplicarCondicaoPagamento(false);
            }
        }

        /// <summary>
        /// Propriedade com a data de inclusão do documento no sistema 
        /// </summary>
        public virtual DateTime DataInclusao
        {
            get
            {
                return dataInclusao;
            }
            set
            {
                dataInclusao = value;
            }
        }

        /// <summary>
        /// Propriedade com a hora da inclusao
        /// </summary>        
        public virtual DateTime HoraInclusao
        {
            get
            {
                return horaInclusao;
            }
            set
            {
                horaInclusao = value;
            }
        }

        /// <summary>
        /// Propriedade indicando se o título é de Receber ou Pagar
        /// </summary>
        public virtual PagarReceberTipo PagarReceber
        {
            get
            {
                return pagarReceber;
            }
            set
            {
                if (PagarReceber.Equals(value))
                    return;

                pagarReceber = value;

                if (this.IdEntidade > 0)
                    this.IdEntidade = 0;

                CarregarOperacaoDefault();
                ValidarNumeroDocumento(this.NumeroDocumento);
            }
        }

        /// <summary>
        /// Propriedade com o objeto Entidade a qual o documento foi emitido
        /// </summary>
        public IEntidade Entidade
        {
            get
            {
                return (IEntidade)entidade.GetObjeto(this.Ambiente);
            }
            set
            {
                entidade.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade com o objeto Operacao
        /// </summary>
        public IOperacao Operacao
        {
            get
            {
                return (IOperacao)operacao.GetObjeto(this.Ambiente);
            }
            set
            {
                operacao.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade com o objeto de Condição de Pagamento
        /// </summary>
        public ICondicaoPagamento CondicaoPagamento
        {
            get
            {
                return (ICondicaoPagamento)condicaoPagamento.GetObjeto(this.Ambiente);
            }
            set
            {
                condicaoPagamento.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade com o objeto de Indice Economico
        /// </summary>
        public IIndiceEconomico IndiceEconomico
        {
            get
            {
                return (IIndiceEconomico)indiceEconomico.GetObjeto(this.Ambiente);
            }
            set
            {
                indiceEconomico.SetObjeto(value);
            }
        }

        /// <summary>
        /// Propriedade com o objeto de indice para qual será convertido o valor do documento
        /// </summary>
        public IIndiceEconomico IndiceConversao
        {
            get
            {
                IIndiceEconomico ie = (IIndiceEconomico)indiceConversao.GetObjeto(this.Ambiente);
                if (ie == null)
                    ie = AmbienteServidorGlobal.IndiceEconomicoDefault;

                if (ie != null)
                    ie.Ambiente.Transacao = this.Ambiente.Transacao;

                return ie;
            }
        }

        /// <summary>
        /// Propriedade com o objeto Tipo de Documento
        /// </summary>
        public ITipoDocumento TipoDocumento
        {
            get
            {
                return (ITipoDocumento)tipoDocumento.GetObjeto(this.Ambiente);
            }
            set
            {
                tipoDocumento.SetObjeto(value);
            }
        }
        
        #endregion

        #region Propriedades de controle

        /// <summary>
        /// Propriedade não persistida com o Id do Portador
        /// </summary>		
        public int IdPortador
        {
            get
            {
                return idPortador;
            }
            set
            {
                idPortador = value;
                SetPortadorParcela(value);                
            }
        }

        /// <summary>
        /// Propriedade não persistida com o Id da FormaCobranca
        /// </summary>
        public int IdFormaCobranca
        {
            get
            {
                return formaCobranca.Id;
            }
            set
            {
                formaCobranca.Id = value;
                SetFormaCobrancaParcela(value);                
            }
        }

        /// <summary>
        /// Propriedade com o objeto FormaCobranca
        /// </summary>
        public IFormaCobranca FormaCobranca
        {
            get
            {
                return (IFormaCobranca)formaCobranca.GetObjeto(this.Ambiente);
            }
            set
            {
                formaCobranca.SetObjeto(value);
            }
        }

        #endregion

        #region Propriedades origem

        /// <summary>
        /// Propriedade indicando o IdOrigem deste gerador
        /// </summary>
        public override int IdOrigemGerador
        {
            get
            {
                return GetIdOrigemGerador();                
            }
        }

        /// <summary>
        /// Propriedade indicando o ProcessoOrigem deste gerador
        /// </summary>
        public override ProcessoOrigem IdProcessoOrigemGerador
        {
            get
            {
                return GetProcessoOrigemGerador();                
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

        #endregion

        #region Propriedades Dados de Rateios

        /// <summary>
        /// Método que retorna o valor para rateio do tipo informado
        /// </summary>
        public double GetValorRateio(TipoRateioItem tipo)
        {
            return RetornarValorRateio(tipo);             
        }

        /// <summary>
        /// Propriedade com o Id da filial do rateio
        /// </summary>
        public int IdFilialRateio
        {
            get
            {
                return this.Ambiente.IdFilial;
            }
        }

        /// <summary>
        /// Propriedade com o objeto do rateio
        /// </summary>
        public IRateioMovto RateioMovto
        {
            get
            {
                if (rateioMovto == null)
                    RetornarRateioMovto(this.Ambiente.Transacao);

                if (rateioMovto != null)
                    this.Ambiente.Transacao = this.Ambiente.Transacao;

                return rateioMovto;
            }
        }

        /// <summary>
        /// Propriedade para Rateio com o Tipo de Natureza
        /// </summary>
        public NaturezaTipo IdNatureza
        {
            get
            {
                if (this.PagarReceber != PagarReceberTipo.Pagar)
                    return NaturezaTipo.Credora;

                return NaturezaTipo.Devedora;
            }
            set
            {
            }
        }

        /// <summary>
        /// Propriedade para o Rateio com o Tipo de Regime
        /// </summary>
        public RegimeRateioTipo IdTipoRegime
        {
            get
            {
                return RegimeRateioTipo.Economico;
            }
        }

        /// <summary>
        /// Propriedade com a origem do processo
        /// </summary>
        public ProcessoOrigem IdProcessoOrigemRateio
        {
            get
            {
                return GetProcessoOrigemRateio();                 
            }
        }

        /// <summary>
        /// Propriedade indicando o type da interface para persistencia
        /// </summary>
        public Type InterfacePersistencia
        {
            get
            {
                return typeof(IMovtoFinanceiroRateio);
            }
        }

        /// <summary>
        /// Propriedade indicando o tipo de validação para os itens do rateio
        /// </summary>
        public TipoRateioItemValidacao TipoValidacaoItem
        {
            get
            {
                return TipoRateioItemValidacao.ValidarValor;
            }
        }

        #region Métodos

        /// <summary>
        /// Método que refaz o carregamento do movimento financeiro
        /// </summary>
        public void CarregarRateioMovto(TransacaoBase trans)
        {
            RetornarRateioMovto(trans);
        }

        /// <summary>
        /// Método que retorna a matriz com as regras origem
        /// </summary>
        public IRateioRegraOrigem[] RegraOrigem()
        {
            return GetMatrizRegraOrigemRateio();
        }

        #endregion

        #endregion

        #region Propriedades DadosFinanceiro
        
        /// <summary>
        /// Propriedade com a interface da forma de pagamento
        /// </summary>
        public Type InterfaceFormaPagto 
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Propriedade com a lista de forma de pagamento
        /// </summary>
        public IListBase FormasPagtoValidar
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Propriedade com a lista de troco
        /// </summary>
        public IListBase TrocoValidar
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Métodos para tratamento histórico

        /// <summary>
        /// Método que retorna a matriz de propriedades do histórico
        /// </summary>
        public string[] HistoricoPropriedades()
        {
            return new string[] { "Entidade", "EntidadeRazao", "EntidadeNomeRazao", "TipoDocumento", "Numero", "DataEmissao", "ValorTotal" };
        }

        /// <summary>
        /// Método que retorna o valor para historico da propriedade informada
        /// </summary>
        public object HistoricoValor(string propriedade)
        {
            string[] m = HistoricoPropriedades();
            int idx = Funcoes.MatrizIndexOf(m, propriedade, true);

            if (idx == 0 || idx == 1 || idx == 2)
            {
                TipoInfoEntidade tipo = TipoInfoEntidade.SomenteNome;
                if (idx != 0)
                    tipo = (idx == 1) ? TipoInfoEntidade.Nome : TipoInfoEntidade.NomeRazao;

                return HistoricoPadrao.GetHistoricoEntidade(tipo, this.Entidade);
            }
            
            if (idx == 3)
                return (TipoDocumento == null ? "" : TipoDocumento.Descricao);
            
            if (idx == 4)
                return NumeroDocumento;
            
            if (idx == 5)
                return DataEmissao;
            
            if (idx == 6)
                return Valor;
            
            return "(n/d)";
        }

        /// <summary>
        /// Método que retorno o processo origem identificador deste processo
        /// </summary>
        public ProcessoOrigem HistoricoProcesso()
        {
            return GetProcessoOrigemGerador();
        }

        #endregion
    }
}

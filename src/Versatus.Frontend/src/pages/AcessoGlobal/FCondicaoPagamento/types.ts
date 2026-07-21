export interface ICondicaoPagtoRegraParceladaForm {
  idCondicaoPagtoParcela: number;
  numeroDias: number;
  numeroParcela: number;
  percentualDivisao: number;
  diasLiberado?: number;
  percentualValorMinimo?: number;
}

export interface ICondicaoPagtoRegraFaixaForm {
  idCondicaoPagtoParcela: number;
  numeroDias: number; // Dia de vencimento
  numeroParcela: number; // Faixa nº
  diaInicial: number;
  diaFinal: number;
}

export interface ICondicaoPagamentoForm {
  idCondicaoPagamento: number;
  descricao: string;
  ativo: boolean;
  idTipoCondicaoPagto: number; // CondicaoPagtoTipo
  idDisponibilidade: number; // Disponibilidade
  idTipoVencimento: number; // VencimentoTipo
  
  idGrupoCondicaoPagamento?: number;
  idFormaCobranca?: number;
  idFormaPagamento?: number;
  idFormaPagamentoVista?: number;

  ordemConsulta: number;
  utilizarPdv: boolean;

  recebeAcrescimo: boolean;
  acrescimo: number;
  recebeDesconto: boolean;
  desconto: number;

  // Parcelamento
  alteraParcelas: boolean;
  alteraNroParcela: boolean;
  idParcelamentoTipo: number; // ParcelamentoTipo
  tipoDivisaoParcelamento: number; // DivisaoParcelamentoTipo
  quantidadeParcela: number;
  diasParcelamento: number;
  usarMesComercial: boolean;
  diasMinimoProximoMes: number;
  primeiraParcelaAVista: boolean;
  obrigatorioFormaPagamento: boolean;
  idParcelaArredondamento: number; // ParcelamentoArredondamento

  // Faixa
  quantidadeFaixa: number;

  // Semanal
  idDiaSemana: number; // DiaSemana

  // Coleções de regras separadas para o form
  parceladas: ICondicaoPagtoRegraParceladaForm[];
  faixas: ICondicaoPagtoRegraFaixaForm[];
}

export const defaultValues: ICondicaoPagamentoForm = {
  idCondicaoPagamento: 0,
  descricao: '',
  ativo: true,
  idTipoCondicaoPagto: 36, // Parcelada
  idDisponibilidade: 57, // Recebimento
  idTipoVencimento: 59, // Normal
  idGrupoCondicaoPagamento: undefined,
  idFormaCobranca: undefined,
  idFormaPagamento: undefined,
  idFormaPagamentoVista: undefined,
  ordemConsulta: 0,
  utilizarPdv: false,
  recebeAcrescimo: false,
  acrescimo: 0,
  recebeDesconto: false,
  desconto: 0,
  
  // Parcelamento
  alteraParcelas: false,
  alteraNroParcela: false,
  idParcelamentoTipo: 120, // DiasEntreParcela
  tipoDivisaoParcelamento: 604, // Quantidade
  quantidadeParcela: 1,
  diasParcelamento: 0,
  usarMesComercial: false,
  diasMinimoProximoMes: 0,
  primeiraParcelaAVista: false,
  obrigatorioFormaPagamento: false,
  idParcelaArredondamento: 46, // Primeira

  // Faixas
  quantidadeFaixa: 0,

  // Semanal
  idDiaSemana: 165, // Segunda

  parceladas: [],
  faixas: [],
};

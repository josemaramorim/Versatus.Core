// Tela Caixa/Banco (FCaixaBanco) — spec: docs/spec_fcaixabanco.md v1.0 (E3-T07).
// Enums pelo valor inteiro do banco (GLOTIPOENUMERADO) — spec §3.1.

/** ContaTipo (idPai 174). */
export const CONTA_TIPO = { Caixa: 175, Banco: 176 } as const;

/** TipoContaCaixa (idPai 1481). */
export const TIPO_CONTA_CAIXA = { Normal: 1482, Cofre: 1483 } as const;

/** TipoContaBancaria (idPai 1478). */
export const TIPO_CONTA_BANCARIA = { ContaCorrente: 1479, Investimento: 1480 } as const;

/** Linha da grade de usuários (FINCAIXABANCOUSUARIO). */
export interface ICaixaBancoUsuarioForm {
  idUsuario: number;
  /** Usuário com que a linha foi carregada (linha já salva) — VAL-E3-12. */
  idUsuarioSalvo: number | null;
  descricao: string;
}

/** Aba Conta bancária (FINCONTABANCARIA — núcleo E3). */
export interface IContaBancariaForm {
  idTipoContaBancaria: number | null;
  idAgencia: number | null;
  numeroConta: string;
  digitoConta: string;
  limite: number | null;
  creditoPendente: number | null;
  debitoPendente: number | null;
  chequePendente: number | null;
  contaTerceiro: boolean;
  titular: string;
  cpfCnpj: string;
  permiteEmitirCheque: boolean;
  idContaBancariaVinculada: number | null;
  enviarSped: boolean;
  idInstituicaoFinanceira: number | null;
}

export interface ICaixaBancoForm {
  idCaixaBanco: number;
  idFilial: number;
  descricao: string;
  ativo: boolean;
  idTipoConta: number | null;
  idTipoContaCaixa: number | null;
  entraFluxoCaixa: boolean;
  /** Sem controle na tela (spec §5.4) — preservados no PUT. */
  ultimaDataConferida: string | null;
  saldo: number | null;
  contaContabil: string | null;
  idPlanoContabil: number | null;
  usuarios: ICaixaBancoUsuarioForm[];
  contaBancaria: IContaBancariaForm;
}

export const contaBancariaDefault: IContaBancariaForm = {
  idTipoContaBancaria: null,
  idAgencia: null,
  numeroConta: '',
  digitoConta: '',
  limite: null,
  creditoPendente: null,
  debitoPendente: null,
  chequePendente: null,
  contaTerceiro: false,
  titular: '',
  cpfCnpj: '',
  permiteEmitirCheque: false,
  idContaBancariaVinculada: null,
  enviarSped: false,
  idInstituicaoFinanceira: null,
};

export const defaultValues: ICaixaBancoForm = {
  idCaixaBanco: 0,
  idFilial: 0,
  descricao: '',
  ativo: true,
  idTipoConta: null,
  idTipoContaCaixa: null,
  entraFluxoCaixa: false,
  ultimaDataConferida: null,
  saldo: null,
  contaContabil: null,
  idPlanoContabil: null,
  usuarios: [],
  contaBancaria: contaBancariaDefault,
};

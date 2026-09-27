import { describe, it, expect } from 'vitest';
import { caixaBancoSchema, MSG } from './schema';
import { CONTA_TIPO, TIPO_CONTA_BANCARIA, defaultValues, contaBancariaDefault } from './types';
import type { ICaixaBancoForm, IContaBancariaForm } from './types';

// 1 teste por regra Zod da Matriz RTV da tela (docs/spec_fcaixabanco.md §6 / C7).

const caixa = (extra: Partial<ICaixaBancoForm> = {}): ICaixaBancoForm => ({
  ...defaultValues,
  descricao: 'Caixa geral',
  idTipoConta: CONTA_TIPO.Caixa,
  ...extra,
});

const banco = (conta: Partial<IContaBancariaForm> = {}, extra: Partial<ICaixaBancoForm> = {}): ICaixaBancoForm =>
  caixa({
    idTipoConta: CONTA_TIPO.Banco,
    contaBancaria: {
      ...contaBancariaDefault,
      idTipoContaBancaria: TIPO_CONTA_BANCARIA.ContaCorrente,
      idAgencia: 3,
      numeroConta: '12345',
      ...conta,
    },
    ...extra,
  });

const mensagens = (form: ICaixaBancoForm): string[] => {
  const r = caixaBancoSchema.safeParse(form);
  return r.success ? [] : r.error.issues.map((i) => i.message);
};

describe('FCaixaBanco — schema Zod (Matriz RTV)', () => {
  it('aceita um caixa válido', () => {
    expect(mensagens(caixa())).toEqual([]);
  });

  it('aceita um banco conta corrente válido', () => {
    expect(mensagens(banco())).toEqual([]);
  });

  it('obrigatório: descrição (NOT NULL)', () => {
    expect(mensagens(caixa({ descricao: '   ' }))).toContain(MSG.descricao);
  });

  it('obrigatório: tipo da conta (NOT NULL)', () => {
    expect(mensagens(caixa({ idTipoConta: null }))).toContain(MSG.tipoConta);
  });

  it('UI-07: banco sem tipo da conta bancária', () => {
    expect(mensagens(banco({ idTipoContaBancaria: null }))).toContain(MSG.tipoContaBancaria);
  });

  it('obrigatório: agência e número em conta corrente (NOT NULL)', () => {
    const m = mensagens(banco({ idAgencia: null, numeroConta: '' }));
    expect(m).toContain(MSG.agencia);
    expect(m).toContain(MSG.numeroConta);
  });

  it('VAL-E3-02: enviar SPED sem instituição financeira', () => {
    expect(mensagens(banco({ enviarSped: true }))).toContain(MSG.spedSemInstituicao);
  });

  it('VAL-E3-03: enviar SPED com conta de terceiro', () => {
    const m = mensagens(banco({ enviarSped: true, idInstituicaoFinanceira: 4, contaTerceiro: true, titular: 'Fulano', cpfCnpj: '12345678909' }));
    expect(m).toEqual([MSG.spedContaTerceiro]);
  });

  it('VAL-E3-06: investimento sem conta vinculada', () => {
    const m = mensagens(banco({ idTipoContaBancaria: TIPO_CONTA_BANCARIA.Investimento, idAgencia: null, numeroConta: '' }));
    expect(m).toEqual([MSG.investimentoSemVinculada]);
  });

  it('VAL-E3-07: conta vinculada igual à própria conta (em edição)', () => {
    const form = banco(
      { idTipoContaBancaria: TIPO_CONTA_BANCARIA.Investimento, idContaBancariaVinculada: 7 },
      { idCaixaBanco: 7 },
    );
    expect(mensagens(form)).toEqual([MSG.investimentoVinculadaIgual]);
  });

  it('VAL-E3-17: conta de terceiro sem titular ou CPF/CNPJ', () => {
    expect(mensagens(banco({ contaTerceiro: true, titular: 'Fulano' }))).toEqual([MSG.contaTerceiro]);
    expect(mensagens(banco({ contaTerceiro: true, cpfCnpj: '12345678909' }))).toEqual([MSG.contaTerceiro]);
  });

  it('VAL-E3-11: usuário repetido na grade', () => {
    const usuarios = [
      { idUsuario: 2, idUsuarioSalvo: null, descricao: 'A' },
      { idUsuario: 2, idUsuarioSalvo: null, descricao: 'A' },
    ];
    expect(mensagens(caixa({ usuarios }))).toEqual([MSG.usuarioJaInformado]);
  });

  it('UI-05: caixa não valida a aba Conta bancária', () => {
    expect(mensagens(caixa({ contaBancaria: { ...contaBancariaDefault, enviarSped: true } }))).toEqual([]);
  });
});

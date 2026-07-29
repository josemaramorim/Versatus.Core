import { describe, it, expect } from 'vitest';
import { entidadeSchema, isValidCPF, isValidCNPJ, isValidSuframa } from './schema';

describe('FEntidade Schema Zod Validation Tests', () => {
  const baseValida = {
    codigo: '1',
    razaoSocial: 'Empresa Teste LTDA',
    apelido: 'Empresa Teste',
    tipoEspecificoEntidade: 'Geral',
    tipoPessoa: 3, // Juridica
    isCliente: true,
    isFornecedor: false,
    isFuncionario: false,
    isTransportadora: false,
    isComissionado: false,
    isFilial: false,
    isAgencia: false,
    isFinanceira: false,
    isObra: false,
    isRepresentante: false,
    isOutro: false,
    isProspecto: false,
    isContador: false,
    isAluno: false,
    isProfessor: false,
    isIntermediador: false,
    ativo: true,
    cpf: '',
    cnpj: '00000000000191',
    fisicaTipoJuridica: false,
    contribuinteIcms: 'Sim',
    inscricaoEstadual: '12345678',
    regimeTributario: 1,
    naturezaJuridica: 1,
    enquadramento: 'ME',
    fornCotacaoProd: false,
  };

  it('deve validar utilitário isValidCPF', () => {
    expect(isValidCPF('11111111111')).toBe(false);
    expect(isValidCPF('52998224725')).toBe(true);
  });

  it('deve validar utilitário isValidCNPJ', () => {
    expect(isValidCNPJ('00000000000000')).toBe(false);
    expect(isValidCNPJ('00000000000191')).toBe(true);
  });

  it('deve validar utilitário isValidSuframa', () => {
    expect(isValidSuframa('001234567')).toBe(false);
    expect(isValidSuframa('123456780')).toBe(false);
  });

  it('deve recusar quando nenhum papel for selecionado', () => {
    const data = { ...baseValida, isCliente: false };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('Pelo menos um papel'))).toBe(true);
    }
  });

  it('deve recusar Pessoa Física com CPF inválido', () => {
    const data = { ...baseValida, tipoPessoa: 2, cpf: '12345678900', cnpj: '' };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('CPF inválido'))).toBe(true);
    }
  });

  it('deve recusar Pessoa Jurídica com CNPJ inválido', () => {
    const data = { ...baseValida, tipoPessoa: 3, cnpj: '11111111111111' };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('CNPJ inválido'))).toBe(true);
    }
  });

  it('deve recusar Filial Pessoa Física sem característica jurídica', () => {
    const data = { ...baseValida, isFilial: true, tipoPessoa: 2, fisicaTipoJuridica: false, cpf: '52998224725' };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('característica de jurídica'))).toBe(true);
    }
  });

  it('deve recusar Funcionário sendo Pessoa Jurídica', () => {
    const data = { ...baseValida, isFuncionario: true, tipoPessoa: 3 };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('Funcionário'))).toBe(true);
    }
  });

  it('deve recusar Intermediador sendo Pessoa Física', () => {
    const data = { ...baseValida, isIntermediador: true, tipoPessoa: 2, cpf: '52998224725' };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('Intermediador'))).toBe(true);
    }
  });

  it('deve recusar Inscrição SUFRAMA iniciando com 00', () => {
    const data = { ...baseValida, inscricaoSuframa: '001234567' };
    const res = entidadeSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('SUFRAMA'))).toBe(true);
    }
  });

  it('deve aprovar entidade válida com todos os requisitos', () => {
    const res = entidadeSchema.safeParse(baseValida);
    expect(res.success).toBe(true);
  });
});

import { describe, it, expect } from 'vitest';
import { condicaoPagamentoSchema } from './schema';

describe('FCondicaoPagamento Schema Zod Validation Tests', () => {
  const baseValido = {
    idCondicaoPagamento: 1,
    descricao: 'Condição Válida',
    ativo: true,
    idTipoCondicaoPagto: 36, // Parcelada
    idDisponibilidade: 101, // Ambas
    idTipoVencimento: 59, // Normal
    ordemConsulta: 0,
    utilizarPdv: false,
    recebeAcrescimo: false,
    acrescimo: 0,
    recebeDesconto: false,
    desconto: 0,
    alteraParcelas: false,
    alteraNroParcela: false,
    idParcelamentoTipo: 120, // DiasEntreParcela
    tipoDivisaoParcelamento: 603, // Percentual
    quantidadeParcela: 2,
    diasParcelamento: 30,
    usarMesComercial: false,
    diasMinimoProximoMes: 0,
    primeiraParcelaAVista: false,
    obrigatorioFormaPagamento: false,
    idParcelaArredondamento: 46,
    quantidadeFaixa: 0,
    idDiaSemana: 165,
    parceladas: [
      { idCondicaoPagtoParcela: 1, numeroDias: 30, numeroParcela: 1, percentualDivisao: 50 },
      { idCondicaoPagtoParcela: 2, numeroDias: 60, numeroParcela: 2, percentualDivisao: 50 },
    ],
    faixas: [],
  };

  it('deve recusar quando recebeAcrescimo e recebeDesconto forem true simultaneamente', () => {
    const data = { ...baseValido, recebeAcrescimo: true, recebeDesconto: true };
    const res = condicaoPagamentoSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('acréscimo e desconto ao mesmo tempo'))).toBe(true);
    }
  });

  it('deve recusar condição semanal se idDiaSemana for zero', () => {
    const data = { ...baseValido, idTipoCondicaoPagto: 38, idDiaSemana: 0 };
    const res = condicaoPagamentoSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('Dia da semana é obrigatório'))).toBe(true);
    }
  });

  it('deve recusar parcela com quantidadeParcela <= 0', () => {
    const data = { ...baseValido, quantidadeParcela: 0, parceladas: [] };
    const res = condicaoPagamentoSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('maior que 0'))).toBe(true);
    }
  });

  it('deve recusar se a soma dos percentuais for diferente de 100%', () => {
    const data = {
      ...baseValido,
      parceladas: [
        { idCondicaoPagtoParcela: 1, numeroDias: 30, numeroParcela: 1, percentualDivisao: 40 },
        { idCondicaoPagtoParcela: 2, numeroDias: 60, numeroParcela: 2, percentualDivisao: 40 },
      ],
    };
    const res = condicaoPagamentoSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('exatamente 100%'))).toBe(true);
    }
  });

  it('deve recusar condição livre com arredondamento diferente de Ultima parcela', () => {
    const data = {
      ...baseValido,
      alteraParcelas: true,
      idParcelaArredondamento: 46, // Primeira
      tipoDivisaoParcelamento: 604, // Quantidade
    };
    const res = condicaoPagamentoSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('Última parcela'))).toBe(true);
    }
  });

  it('deve aprovar dados totalmente válidos', () => {
    const res = condicaoPagamentoSchema.safeParse(baseValido);
    expect(res.success).toBe(true);
  });
});

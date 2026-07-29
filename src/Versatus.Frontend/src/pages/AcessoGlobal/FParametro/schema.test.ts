import { describe, it, expect } from 'vitest';
import { parametroSchema } from './schema';

describe('FParametro Schema Zod Validation Tests', () => {
  it('deve recusar chave com menos de 3 caracteres', () => {
    const data = { chave: 'AB', descricao: 'Teste' };
    const res = parametroSchema.safeParse(data);
    expect(res.success).toBe(false);
    if (!res.success) {
      expect(res.error.issues.some(i => i.message.includes('pelo menos 3 caracteres'))).toBe(true);
    }
  });

  it('deve recusar chave vazia', () => {
    const data = { chave: '', descricao: 'Teste' };
    const res = parametroSchema.safeParse(data);
    expect(res.success).toBe(false);
  });

  it('deve aceitar parâmetro com dados válidos', () => {
    const data = {
      chave: 'CPFCNPJOBRIGATORIO',
      descricao: 'Bloquear salvar se CPF ou CNPJ vazio',
      valor: 'BloquearSalvar',
      tipo: 155,
      agrupador: 31,
      visivel: true,
      marcado: true,
    };
    const res = parametroSchema.safeParse(data);
    expect(res.success).toBe(true);
  });
});

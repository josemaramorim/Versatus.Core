import { z } from 'zod';

export const parametroSchema = z.object({
  id: z.number().optional().default(0),
  chave: z.string()
    .min(3, 'A Chave deve ter pelo menos 3 caracteres')
    .max(100, 'A Chave não pode exceder 100 caracteres')
    .nonempty('A Chave é obrigatória'),
  descricao: z.string()
    .max(250, 'A descrição não pode exceder 250 caracteres')
    .optional()
    .default(''),
  valor: z.string()
    .max(2000, 'O valor não pode exceder 2000 caracteres')
    .optional()
    .default(''),
  tipo: z.number().default(155),
});

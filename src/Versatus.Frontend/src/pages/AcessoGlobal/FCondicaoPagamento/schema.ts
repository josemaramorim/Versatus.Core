import { z } from 'zod';

export const condicaoPagamentoSchema = z.object({
  idCondicaoPagamento: z.number().default(0),
  descricao: z.string().min(3, 'A descrição deve ter pelo menos 3 caracteres'),
  ativo: z.boolean().default(true),
  idTipoCondicaoPagto: z.number(),
  idDisponibilidade: z.number(),
  idTipoVencimento: z.number(),
  
  idGrupoCondicaoPagamento: z.number().optional().nullable(),
  idFormaCobranca: z.number().optional().nullable(),
  idFormaPagamento: z.number().optional().nullable(),
  idFormaPagamentoVista: z.number().optional().nullable(),

  ordemConsulta: z.number().min(0, 'Ordem deve ser >= 0').max(100, 'Ordem deve ser <= 100'),
  utilizarPdv: z.boolean().default(false),

  recebeAcrescimo: z.boolean().default(false),
  acrescimo: z.number().min(0).max(99.99).default(0),
  recebeDesconto: z.boolean().default(false),
  desconto: z.number().min(0).max(99.99).default(0),

  // Parcelamento
  alteraParcelas: z.boolean().default(false),
  alteraNroParcela: z.boolean().default(false),
  idParcelamentoTipo: z.number(),
  tipoDivisaoParcelamento: z.number(),
  quantidadeParcela: z.number().min(0).default(0),
  diasParcelamento: z.number().min(0).default(0),
  usarMesComercial: z.boolean().default(false),
  diasMinimoProximoMes: z.number().min(0).max(31, 'Dias limite deve ser <= 31').default(0),
  primeiraParcelaAVista: z.boolean().default(false),
  obrigatorioFormaPagamento: z.boolean().default(false),
  idParcelaArredondamento: z.number(),

  // Faixas
  quantidadeFaixa: z.number().min(0).default(0),

  // Semanal
  idDiaSemana: z.number().default(165),

  // Regras
  parceladas: z.array(
    z.object({
      idCondicaoPagtoParcela: z.number().default(0),
      numeroDias: z.number().min(0, 'Dias deve ser >= 0'),
      numeroParcela: z.number(),
      percentualDivisao: z.number().min(0).max(100),
      diasLiberado: z.number().optional().nullable(),
      percentualValorMinimo: z.number().min(0).max(100).optional().nullable(),
    })
  ).default([]),

  faixas: z.array(
    z.object({
      idCondicaoPagtoParcela: z.number().default(0),
      numeroDias: z.number().min(1, 'Dia de vencimento deve ser >= 1').max(31, 'Dia de vencimento deve ser <= 31'),
      numeroParcela: z.number(),
      diaInicial: z.number().min(1, 'Dia inicial deve ser >= 1').max(31, 'Dia inicial deve ser <= 31'),
      diaFinal: z.number().min(1, 'Dia final deve ser >= 1').max(31, 'Dia final deve ser <= 31'),
    })
  ).default([]),
}).superRefine((data, ctx) => {
  // 1. Validação Semanal
  if (data.idTipoCondicaoPagto === 38) { // Semanal
    if (!data.idDiaSemana || data.idDiaSemana === 0) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Dia da semana é obrigatório para condição semanal.',
        path: ['idDiaSemana'],
      });
    }
  }

  // 2. Validação Parcelamento
  if (data.idTipoCondicaoPagto === 36) { // Parcelada
    if (!data.alteraParcelas) {
      if (data.quantidadeParcela <= 0) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'Quantidade de parcelas deve ser maior que 0.',
          path: ['quantidadeParcela'],
        });
      }
      
      // Soma dos percentuais deve ser 100% se for divisão percentual
      if (data.tipoDivisaoParcelamento === 603) { // Percentual
        const totalPct = data.parceladas.reduce((sum, p) => sum + p.percentualDivisao, 0);
        if (totalPct !== 100 && data.parceladas.length > 0) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: `A soma dos percentuais deve ser exatamente 100% (atual: ${totalPct}%).`,
            path: ['parceladas'],
          });
        }
      }
    } else {
      // Condição Livre
      if (data.idParcelaArredondamento !== 47) { // Ultima
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Para condição livre o arredondamento deve ser 'Última parcela'.",
          path: ['idParcelaArredondamento'],
        });
      }
      if (data.tipoDivisaoParcelamento === 603) { // Percentual
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Para condição livre o tipo da divisão deve ser 'Quantidade'.",
          path: ['tipoDivisaoParcelamento'],
        });
      }
      if (data.idParcelamentoTipo === 693) { // Dias Uteis
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Para condição livre não é permitido o tipo de parcelamento 'Dias úteis'.",
          path: ['idParcelamentoTipo'],
        });
      }
    }

    if (data.usarMesComercial) {
      if (data.idParcelamentoTipo !== 120) { // DiasEntreParcela
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Para utilizar mês comercial o parcelamento deve ser 'Dias entre parcelas'.",
          path: ['usarMesComercial'],
        });
      }
      if (data.diasParcelamento !== 30) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: "Para utilizar mês comercial o número de dias entre parcelas deve ser igual a 30.",
          path: ['diasParcelamento'],
        });
      }
    }

    if (data.primeiraParcelaAVista && data.idParcelamentoTipo !== 120) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Para marcar 1ª parcela à vista, o tipo de parcelamento deve ser 'Dias entre parcelas'.",
        path: ['primeiraParcelaAVista'],
      });
    }
  }

  // 3. Validação Faixas
  if (data.idTipoCondicaoPagto === 37) { // Faixa de dias
    if (data.faixas.length === 0) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Devem ser informadas as faixas de dia.',
        path: ['faixas'],
      });
    }

    data.faixas.forEach((faixa, idx) => {
      if (faixa.diaFinal < faixa.diaInicial) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'Dia final não pode ser menor que o dia inicial.',
          path: ['faixas', idx, 'diaFinal'],
        });
      }

      if (faixa.numeroDias >= faixa.diaInicial && faixa.numeroDias <= faixa.diaFinal) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'O dia de vencimento não pode estar dentro do intervalo da faixa.',
          path: ['faixas', idx, 'numeroDias'],
        });
      }
    });
  }

  // 4. Exclusividade Desconto / Acréscimo
  if (data.recebeAcrescimo && data.recebeDesconto) {
    ctx.addIssue({
      code: z.ZodIssueCode.custom,
      message: 'A condição de pagamento não pode receber acréscimo e desconto ao mesmo tempo.',
      path: ['recebeAcrescimo'],
    });
  }
});

import { z } from 'zod';
import { CONTA_TIPO, TIPO_CONTA_BANCARIA } from './types';

// Regras de validação da tela Caixa/Banco — docs/spec_fcaixabanco.md §6 (Matriz RTV).
// As mensagens com ID VAL-E3-xx são as do legado; o backend repete as mesmas regras.

export const MSG = {
  descricao: 'Informe a descrição.',
  tipoConta: 'Informe o tipo da conta.',
  tipoContaBancaria: 'Deve ser informado o tipo da conta bancária.', // UI-07 (FCaixaBanco.cs:2450)
  agencia: 'Informe a agência.',
  numeroConta: 'Informe o número da conta.',
  spedSemInstituicao:
    'Para conta bancária, a instituição financeira para SPED, deve ser informado a instituição financeira fiscal.', // VAL-E3-02
  spedContaTerceiro:
    "Para conta bancária, ao MARCAR a opção 'Enviar SPED (Bloco 1601)', deve ser DESMARCADO a opção 'Conta de terceiro'.", // VAL-E3-03
  investimentoSemVinculada: "Para conta bancária do tipo 'Investimento', deve ser informada a conta vinculada.", // VAL-E3-06
  investimentoVinculadaIgual:
    "Para conta bancária do tipo 'Investimento', a conta vinculada deve ser DIFERENTE do ID da conta bancária.", // VAL-E3-07
  contaTerceiro: 'Para cadastrar uma conta de terceiro tem que informar o nome do titular e o CPF/CNPJ do mesmo.', // VAL-E3-17
  usuarioJaInformado: 'Este usuário já foi informado.', // VAL-E3-11
} as const;

const usuarioSchema = z.object({
  idUsuario: z.number().int().positive(),
  idUsuarioSalvo: z.number().nullable(),
  descricao: z.string(),
});

const contaBancariaSchema = z.object({
  idTipoContaBancaria: z.number().nullable(),
  idAgencia: z.number().nullable(),
  numeroConta: z.string().max(15),
  digitoConta: z.string().max(2),
  limite: z.number().nullable(),
  creditoPendente: z.number().nullable(),
  debitoPendente: z.number().nullable(),
  chequePendente: z.number().nullable(),
  contaTerceiro: z.boolean(),
  titular: z.string().max(50),
  cpfCnpj: z.string().max(14),
  permiteEmitirCheque: z.boolean(),
  idContaBancariaVinculada: z.number().nullable(),
  enviarSped: z.boolean(),
  idInstituicaoFinanceira: z.number().nullable(),
});

export const caixaBancoSchema = z
  .object({
    idCaixaBanco: z.number(),
    idFilial: z.number(),
    descricao: z.string().trim().min(1, MSG.descricao).max(100),
    ativo: z.boolean(),
    idTipoConta: z.number({ error: MSG.tipoConta }).nullable(),
    idTipoContaCaixa: z.number().nullable(),
    entraFluxoCaixa: z.boolean(),
    ultimaDataConferida: z.string().nullable(),
    saldo: z.number().nullable(),
    contaContabil: z.string().nullable(),
    idPlanoContabil: z.number().nullable(),
    usuarios: z.array(usuarioSchema),
    contaBancaria: contaBancariaSchema,
  })
  .superRefine((form, ctx) => {
    const erro = (path: (string | number)[], message: string) => ctx.addIssue({ code: 'custom', path, message });

    if (form.idTipoConta == null) erro(['idTipoConta'], MSG.tipoConta);

    // VAL-E3-11 — usuário repetido na grade.
    const vistos = new Set<number>();
    form.usuarios.forEach((u, i) => {
      if (vistos.has(u.idUsuario)) erro(['usuarios', i, 'idUsuario'], MSG.usuarioJaInformado);
      vistos.add(u.idUsuario);
    });

    // Aba Conta bancária — só valida quando Tipo = Banco (UI-05).
    if (form.idTipoConta !== CONTA_TIPO.Banco) return;
    const cb = form.contaBancaria;

    if (cb.idTipoContaBancaria == null) {
      erro(['contaBancaria', 'idTipoContaBancaria'], MSG.tipoContaBancaria);
      return;
    }

    if (cb.idTipoContaBancaria === TIPO_CONTA_BANCARIA.ContaCorrente) {
      // Colunas NOT NULL — em Investimento vêm da conta vinculada (UI-08 / VAL-E3-26).
      if (cb.idAgencia == null || cb.idAgencia <= 0) erro(['contaBancaria', 'idAgencia'], MSG.agencia);
      if (!cb.numeroConta.trim()) erro(['contaBancaria', 'numeroConta'], MSG.numeroConta);
    }

    if (cb.idTipoContaBancaria === TIPO_CONTA_BANCARIA.Investimento) {
      if (cb.idContaBancariaVinculada == null) {
        erro(['contaBancaria', 'idContaBancariaVinculada'], MSG.investimentoSemVinculada); // VAL-E3-06
      } else if (form.idCaixaBanco > 0 && cb.idContaBancariaVinculada === form.idCaixaBanco) {
        erro(['contaBancaria', 'idContaBancariaVinculada'], MSG.investimentoVinculadaIgual); // VAL-E3-07
      }
    }

    // VAL-E3-17 — conta de terceiro exige titular e CPF/CNPJ.
    if (cb.contaTerceiro) {
      if (!cb.titular.trim()) erro(['contaBancaria', 'titular'], MSG.contaTerceiro);
      if (!cb.cpfCnpj.trim()) erro(['contaBancaria', 'cpfCnpj'], MSG.contaTerceiro);
    }

    if (cb.enviarSped) {
      if (cb.idInstituicaoFinanceira == null) erro(['contaBancaria', 'idInstituicaoFinanceira'], MSG.spedSemInstituicao); // VAL-E3-02
      if (cb.contaTerceiro) erro(['contaBancaria', 'contaTerceiro'], MSG.spedContaTerceiro); // VAL-E3-03
    }
  });

export type CaixaBancoSchema = z.infer<typeof caixaBancoSchema>;

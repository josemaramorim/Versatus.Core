import { Box, Chip } from '@mui/material';
import type { ZodTypeAny } from 'zod';
import type { IColunaConfig, IFiltroConfig } from '../../../types/cadastro';
import { BaseCadastroConfig } from '../../../types/cadastro';
import { buildApiEndpoint } from '../../../config/api';
import { caixaBancoSchema } from './schema';
import { CONTA_TIPO, TIPO_CONTA_CAIXA, contaBancariaDefault, defaultValues } from './types';
import type { ICaixaBancoForm, IContaBancariaForm } from './types';

// Contrato: specs/modulos/MOD-05/contracts/caixa-banco.md — spec: docs/spec_fcaixabanco.md §4/§5.1.

const vazioParaNull = (valor: string | null | undefined): string | null => (valor && valor.trim() ? valor.trim() : null);

export class CaixaBancoCadastroConfig extends BaseCadastroConfig<ICaixaBancoForm> {
  getTitulo(): string {
    return 'Caixa / Banco';
  }

  getApiEndpoint(): string {
    return buildApiEndpoint('/api/financeiro/caixa-banco');
  }

  getDefaultValues(): ICaixaBancoForm {
    return defaultValues;
  }

  getValidationSchema(): ZodTypeAny {
    return caixaBancoSchema;
  }

  /** Chave composta (IDFINCAIXABANCO, IDGLOFILIAL) → rota `{idFilial}/{id}`. */
  override getRecordId(record: any): any {
    if (!record?.idCaixaBanco) return undefined;
    return `${record.idFilial}/${record.idCaixaBanco}`;
  }

  override getExtraListParams(filters: Record<string, any>): Record<string, string> {
    const params: Record<string, string> = {};
    if (filters.texto) params.texto = String(filters.texto);
    if (filters.idTipoConta !== undefined && filters.idTipoConta !== '') params.idTipoConta = String(filters.idTipoConta);
    if (filters.ativo !== undefined && filters.ativo !== '') params.ativo = String(filters.ativo);
    if (filters.entraFluxoCaixa !== undefined && filters.entraFluxoCaixa !== '') params.entraFluxoCaixa = String(filters.entraFluxoCaixa);
    return params;
  }

  override mapBackendToForm(backend: any): ICaixaBancoForm {
    if (!backend) return defaultValues;

    const cb = backend.contaBancaria;
    const contaBancaria: IContaBancariaForm = cb
      ? {
          idTipoContaBancaria: cb.idTipoContaBancaria ?? null,
          idAgencia: cb.idAgencia || null,
          numeroConta: cb.numeroConta ?? '',
          digitoConta: cb.digitoConta ?? '',
          limite: cb.limite ?? null,
          creditoPendente: cb.creditoPendente ?? null,
          debitoPendente: cb.debitoPendente ?? null,
          chequePendente: cb.chequePendente ?? null,
          contaTerceiro: cb.contaTerceiro ?? false,
          titular: cb.titular ?? '',
          cpfCnpj: cb.cpfCnpj ?? '',
          permiteEmitirCheque: cb.permiteEmitirCheque ?? false,
          idContaBancariaVinculada: cb.idContaBancariaVinculada ?? null,
          enviarSped: cb.enviarSped ?? false,
          idInstituicaoFinanceira: cb.idInstituicaoFinanceira ?? null,
        }
      : contaBancariaDefault;

    return {
      idCaixaBanco: backend.idCaixaBanco ?? 0,
      idFilial: backend.idFilial ?? 0,
      descricao: backend.descricao ?? '',
      ativo: backend.ativo ?? true,
      idTipoConta: backend.idTipoConta ?? null,
      idTipoContaCaixa: backend.idTipoContaCaixa ?? null,
      entraFluxoCaixa: backend.entraFluxoCaixa ?? false,
      ultimaDataConferida: backend.ultimaDataConferida ?? null,
      saldo: backend.saldo ?? null,
      contaContabil: backend.contaContabil ?? null,
      idPlanoContabil: backend.idPlanoContabil ?? null,
      usuarios: (backend.usuarios ?? []).map((u: any) => ({
        idUsuario: u.idUsuario,
        idUsuarioSalvo: u.idUsuario,
        descricao: '',
      })),
      contaBancaria,
    };
  }

  /** `CriarCaixaBancoDto` / `AtualizarCaixaBancoDto` (mesmo formato). */
  override mapFormToBackend(form: ICaixaBancoForm): any {
    const banco = form.idTipoConta === CONTA_TIPO.Banco;
    const cb = form.contaBancaria;

    return {
      descricao: form.descricao.trim(),
      idTipoConta: form.idTipoConta,
      ativo: form.ativo,
      entraFluxoCaixa: form.entraFluxoCaixa,
      ultimaDataConferida: form.ultimaDataConferida,
      saldo: form.saldo,
      contaContabil: form.contaContabil,
      idPlanoContabil: form.idPlanoContabil,
      idTipoContaCaixa: banco ? null : form.idTipoContaCaixa,
      usuarios: form.usuarios.map((u) => u.idUsuario),
      contaBancaria: banco
        ? {
            idAgencia: cb.idAgencia ?? 0,
            titular: vazioParaNull(cb.titular),
            numeroConta: cb.numeroConta.trim(),
            digitoConta: vazioParaNull(cb.digitoConta),
            limite: cb.limite,
            creditoPendente: cb.creditoPendente,
            debitoPendente: cb.debitoPendente,
            chequePendente: cb.chequePendente,
            contaTerceiro: cb.contaTerceiro,
            permiteEmitirCheque: cb.permiteEmitirCheque,
            idContaBancariaVinculada: cb.idContaBancariaVinculada,
            idTipoContaBancaria: cb.idTipoContaBancaria,
            idInstituicaoFinanceira: cb.idInstituicaoFinanceira,
            enviarSped: cb.enviarSped,
            cpfCnpj: vazioParaNull(cb.cpfCnpj),
          }
        : null,
    };
  }

  getColunas(): IColunaConfig<ICaixaBancoForm>[] {
    return [
      { header: 'Código', field: 'idCaixaBanco', width: 100 },
      { header: 'Descrição', field: 'descricao' },
      {
        header: 'Tipo',
        field: 'idTipoConta',
        width: 110,
        renderCell: (r) => (
          <Chip
            size="small"
            variant="outlined"
            color={r.idTipoConta === CONTA_TIPO.Banco ? 'primary' : 'default'}
            label={r.idTipoConta === CONTA_TIPO.Banco ? 'Banco' : 'Caixa'}
          />
        ),
      },
      {
        header: 'Tipo conta caixa',
        field: 'idTipoContaCaixa',
        width: 150,
        renderCell: (r) =>
          r.idTipoContaCaixa === TIPO_CONTA_CAIXA.Cofre ? 'Cofre' : r.idTipoContaCaixa === TIPO_CONTA_CAIXA.Normal ? 'Normal' : '',
      },
      {
        header: 'Entra fluxo de caixa',
        field: 'entraFluxoCaixa',
        width: 170,
        renderCell: (r) => (r.entraFluxoCaixa ? 'Sim' : 'Não'),
      },
      {
        header: 'Situação',
        field: 'ativo',
        width: 120,
        renderCell: (r) => (
          <Box component="span" sx={{ fontWeight: 600, color: r.ativo ? 'success.main' : 'error.main' }}>
            {r.ativo ? 'Ativo' : 'Inativo'}
          </Box>
        ),
      },
    ];
  }

  getFiltros(): IFiltroConfig[] {
    return [
      { field: 'texto', label: 'Buscar por código ou descrição', type: 'text' },
      {
        field: 'idTipoConta',
        label: 'Tipo',
        type: 'select',
        options: [
          { label: 'Todos', value: '' },
          { label: 'Caixa', value: CONTA_TIPO.Caixa },
          { label: 'Banco', value: CONTA_TIPO.Banco },
        ],
      },
      {
        field: 'ativo',
        label: 'Situação',
        type: 'select',
        options: [
          { label: 'Todos', value: '' },
          { label: 'Ativo', value: 'true' },
          { label: 'Inativo', value: 'false' },
        ],
      },
      {
        field: 'entraFluxoCaixa',
        label: 'Entra fluxo de caixa',
        type: 'select',
        options: [
          { label: 'Todos', value: '' },
          { label: 'Sim', value: 'true' },
          { label: 'Não', value: 'false' },
        ],
      },
    ];
  }
}


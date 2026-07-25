import type { ZodTypeAny } from 'zod';
import type { IColunaConfig, IFiltroConfig } from '../../../types/cadastro';
import { BaseCadastroConfig } from '../../../types/cadastro';
import type { ICondicaoPagamentoForm, ICondicaoPagtoRegraParceladaForm, ICondicaoPagtoRegraFaixaForm } from './types';
import { defaultValues } from './types';
import { condicaoPagamentoSchema } from './schema';
import { buildApiEndpoint } from '../../../config/api';

export class CondicaoPagamentoCadastroConfig extends BaseCadastroConfig<ICondicaoPagamentoForm> {
  getTitulo(): string {
    return 'Condições de Pagamento';
  }

  getApiEndpoint(): string {
    return buildApiEndpoint('/api/condicaopagamento');
  }

  getDefaultValues(): ICondicaoPagamentoForm {
    return defaultValues;
  }

  getValidationSchema(): ZodTypeAny {
    return condicaoPagamentoSchema;
  }

  override mapBackendToForm(backend: any): ICondicaoPagamentoForm {
    if (!backend) return defaultValues;

    const regras = backend.regras || [];
    const idTipo = backend.idTipoCondicaoPagto ?? 36;

    const parceladas: ICondicaoPagtoRegraParceladaForm[] = idTipo === 36
      ? regras.map((r: any) => ({
          idCondicaoPagtoParcela: r.idCondicaoPagtoParcela ?? 0,
          numeroDias: r.numeroDias ?? 0,
          numeroParcela: r.numeroParcela ?? 1,
          percentualDivisao: r.percentualDivisao ?? 0,
          diasLiberado: r.diasLiberado ?? 0,
          percentualValorMinimo: r.percentualValorMinimo ?? 0,
        }))
      : [];

    const faixas: ICondicaoPagtoRegraFaixaForm[] = idTipo === 37
      ? regras.map((r: any) => ({
          idCondicaoPagtoParcela: r.idCondicaoPagtoParcela ?? 0,
          numeroDias: r.numeroDias ?? 1, // Dia de vencimento
          numeroParcela: r.numeroParcela ?? 1,
          diaInicial: r.diaInicial ?? 1,
          diaFinal: r.diaFinal ?? 1,
        }))
      : [];

    return {
      idCondicaoPagamento: backend.idCondicaoPagamento ?? 0,
      descricao: backend.descricao || '',
      ativo: backend.ativo ?? true,
      idTipoCondicaoPagto: idTipo,
      idDisponibilidade: backend.idDisponibilidade ?? 57,
      idTipoVencimento: backend.idTipoVencimento ?? 59,
      
      idGrupoCondicaoPagamento: backend.idGrupoCondicaoPagamento ?? undefined,
      idFormaCobranca: backend.idFormaCobranca ?? undefined,
      idFormaPagamento: backend.idFormaPagamento ?? undefined,
      idFormaPagamentoVista: backend.idFormaPagamentoVista ?? undefined,

      ordemConsulta: backend.ordemConsulta ?? 0,
      utilizarPdv: backend.utilizarPdv ?? false,

      recebeAcrescimo: backend.recebeAcrescimo ?? false,
      acrescimo: backend.acrescimo ?? 0,
      recebeDesconto: backend.recebeDesconto ?? false,
      desconto: backend.desconto ?? 0,

      // Parcelamento
      alteraParcelas: backend.alteraParcelas ?? false,
      alteraNroParcela: backend.alteraNroParcela ?? false,
      idParcelamentoTipo: backend.idParcelamentoTipo ?? 120,
      tipoDivisaoParcelamento: backend.tipoDivisaoParcelamento ?? 604,
      quantidadeParcela: backend.quantidadeParcela ?? 0,
      diasParcelamento: backend.diasParcelamento ?? 0,
      usarMesComercial: backend.usarMesComercial ?? false,
      diasMinimoProximoMes: backend.diasMinimoProximoMes ?? 0,
      primeiraParcelaAVista: backend.primeiraParcelaAVista ?? false,
      obrigatorioFormaPagamento: backend.obrigatorioFormaPagamento ?? false,
      idParcelaArredondamento: backend.idParcelaArredondamento ?? 46,

      // Faixas
      quantidadeFaixa: backend.quantidadeFaixa ?? 0,

      // Semanal
      idDiaSemana: backend.idDiaSemana ?? 165,

      parceladas,
      faixas,
    };
  }

  override mapFormToBackend(form: ICondicaoPagamentoForm): any {
    let regras: any[] = [];
    const idTipo = form.idTipoCondicaoPagto;

    if (idTipo === 36) { // Parcelada
      regras = form.parceladas.map((r: any) => ({
        idCondicaoPagtoParcela: r.idCondicaoPagtoParcela,
        numeroDias: Number(r.numeroDias),
        numeroParcela: Number(r.numeroParcela),
        percentualDivisao: Number(r.percentualDivisao),
        diasLiberado: r.diasLiberado ? Number(r.diasLiberado) : null,
        percentualValorMinimo: r.percentualValorMinimo ? Number(r.percentualValorMinimo) : null,
      }));
    } else if (idTipo === 37) { // Faixa
      regras = form.faixas.map((r: any) => ({
        idCondicaoPagtoParcela: r.idCondicaoPagtoParcela,
        numeroDias: Number(r.numeroDias), // Dia de vencimento
        numeroParcela: Number(r.numeroParcela),
        diaInicial: Number(r.diaInicial),
        diaFinal: Number(r.diaFinal),
      }));
    }

    return {
      idCondicaoPagamento: form.idCondicaoPagamento,
      descricao: form.descricao,
      ativo: form.ativo,
      idTipoCondicaoPagto: form.idTipoCondicaoPagto,
      idDisponibilidade: form.idDisponibilidade,
      idTipoVencimento: form.idTipoVencimento,
      
      idGrupoCondicaoPagamento: form.idGrupoCondicaoPagamento || null,
      idFormaCobranca: form.idFormaCobranca || null,
      idFormaPagamento: form.idFormaPagamento || null,
      idFormaPagamentoVista: form.idFormaPagamentoVista || null,

      ordemConsulta: Number(form.ordemConsulta),
      utilizarPdv: form.utilizarPdv,

      recebeAcrescimo: form.recebeAcrescimo,
      acrescimo: Number(form.acrescimo),
      recebeDesconto: form.recebeDesconto,
      desconto: Number(form.desconto),

      // Parcelamento
      alteraParcelas: form.alteraParcelas,
      alteraNroParcela: form.alteraNroParcela,
      idParcelamentoTipo: form.idParcelamentoTipo,
      tipoDivisaoParcelamento: form.tipoDivisaoParcelamento,
      quantidadeParcela: Number(form.quantidadeParcela),
      diasParcelamento: Number(form.diasParcelamento),
      usarMesComercial: form.usarMesComercial,
      diasMinimoProximoMes: Number(form.diasMinimoProximoMes),
      primeiraParcelaAVista: form.primeiraParcelaAVista,
      obrigatorioFormaPagamento: form.obrigatorioFormaPagamento,
      idParcelaArredondamento: form.idParcelaArredondamento,

      // Faixas
      quantidadeFaixa: Number(form.quantidadeFaixa),

      // Semanal
      idDiaSemana: form.idDiaSemana,

      regras,
    };
  }

  getColunas(): IColunaConfig<ICondicaoPagamentoForm>[] {
    return [
      {
        header: 'Descrição',
        field: 'descricao',
        sortable: true,
      },
      {
        header: 'Tipo',
        field: 'idTipoCondicaoPagto',
        width: 150,
        sortable: true,
        renderCell: (record) => {
          const value = record.idTipoCondicaoPagto;
          if (value === 36) return 'Parcelada';
          if (value === 37) return 'Faixa Dias';
          if (value === 38) return 'Semanal';
          return 'Desconhecido';
        },
      },
      {
        header: 'Disponibilidade',
        field: 'idDisponibilidade',
        width: 150,
        sortable: true,
        renderCell: (record) => {
          const value = record.idDisponibilidade;
          if (value === 56) return 'Pagamento';
          if (value === 57) return 'Recebimento';
          if (value === 101) return 'Ambas';
          return 'Desconhecido';
        },
      },
      {
        header: 'Situação',
        field: 'ativo',
        width: 120,
        sortable: true,
        renderCell: (record) => (record.ativo ? 'Ativo' : 'Inativo'),
      },
    ];
  }

  getFiltros(): IFiltroConfig[] {
    return [
      {
        field: 'search',
        label: 'Buscar por Descrição',
        type: 'text',
      },
      {
        field: 'disponibilidade',
        label: 'Disponibilidade',
        type: 'select',
        options: [
          { label: 'Todos', value: '' },
          { label: 'Pagamento', value: 56 },
          { label: 'Recebimento', value: 57 },
          { label: 'Ambas', value: 101 },
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
    ];
  }
}

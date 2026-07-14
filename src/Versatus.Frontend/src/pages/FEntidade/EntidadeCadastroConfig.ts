import type { ZodTypeAny } from 'zod';
import type { IColunaConfig, IFiltroConfig } from '../../types/cadastro';
import { BaseCadastroConfig } from '../../types/cadastro';
import type { IEntidadeForm } from './types';
import { defaultValues } from './types';
import { entidadeSchema } from './schema';

export class EntidadeCadastroConfig extends BaseCadastroConfig<IEntidadeForm> {
  getTitulo(): string {
    return 'Entidade';
  }

  getApiEndpoint(): string {
    return '/api/entidade';
  }

  getDefaultValues(): IEntidadeForm {
    return defaultValues;
  }

  getValidationSchema(): ZodTypeAny {
    return entidadeSchema;
  }

  getColunas(): IColunaConfig<IEntidadeForm>[] {
    return [
      {
        header: 'Código',
        field: 'codigo',
        width: 120,
        sortable: true
      },
      {
        header: 'Razão Social / Nome',
        field: 'razaoSocial',
        sortable: true
      },
      {
        header: 'Nome Fantasia / Apelido',
        field: 'apelido',
        sortable: true
      },
      {
        header: 'Tipo Pessoa',
        field: 'tipoPessoa',
        width: 140,
        sortable: true,
        renderCell: (record) => (record.tipoPessoa === 1 ? 'Física' : 'Jurídica')
      },
      {
        header: 'CPF / CNPJ',
        field: 'cpf',
        width: 180,
        renderCell: (record) => (record.tipoPessoa === 1 ? record.cpf : record.cnpj)
      },
      {
        header: 'Contribuinte ICMS',
        field: 'contribuinteIcms',
        width: 160
      }
    ];
  }

  getFiltros(): IFiltroConfig[] {
    return [
      {
        field: 'razaoSocial',
        label: 'Razão Social / Nome',
        type: 'text'
      },
      {
        field: 'apelido',
        label: 'Apelido / Fantasia',
        type: 'text'
      },
      {
        field: 'tipoPessoa',
        label: 'Tipo de Pessoa',
        type: 'select',
        options: [
          { label: 'Física', value: 1 },
          { label: 'Jurídica', value: 2 }
        ]
      }
    ];
  }

  // Gancho opcional para processamento antes de salvar (OOP)
  override beforeSave(record: IEntidadeForm): IEntidadeForm {
    console.log('FEntidade - Executando processamento pré-salvamento:', record.codigo);
    // Exemplo: se pessoa física, limpa dados jurídicos irrelevantes e vice-versa
    if (record.tipoPessoa === 1) {
      return {
        ...record,
        cnpj: '',
        inscricaoEstadual: '',
        inscricaoMunicipal: '',
        inscricaoSuframa: '',
        contribuinteIcms: 'Sim'
      };
    }
    return record;
  }
}

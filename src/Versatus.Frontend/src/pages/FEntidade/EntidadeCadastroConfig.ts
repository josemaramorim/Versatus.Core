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
        header: 'Tipo da Entidade',
        field: 'role',
        width: 250,
        renderCell: (record) => {
          const roles: string[] = [];
          if (record.isCliente) roles.push('Cliente');
          if (record.isFornecedor) roles.push('Fornecedor');
          if (record.isFuncionario) roles.push('Funcionário');
          if (record.isTransportadora) roles.push('Transportadora');
          if (record.isComissionado) roles.push('Comissionado');
          if (record.isFilial) roles.push('Filial');
          if (record.isRepresentante) roles.push('Representante');
          if (record.isContador) roles.push('Contador');
          if (record.isAgencia) roles.push('Agência');
          if (record.isFinanceira) roles.push('Inst. Financeira');
          if (record.isObra) roles.push('Obra');
          if (record.isOutro) roles.push('Outro');
          if (record.isProspecto) roles.push('Prospecto');
          if (record.isAluno) roles.push('Aluno');
          if (record.isProfessor) roles.push('Professor');
          if (record.isIntermediador) roles.push('Intermediador');
          
          return roles.length > 0 ? roles.join(', ') : 'Geral';
        }
      }
    ];
  }

  getFiltros(): IFiltroConfig[] {
    return [
      {
        field: 'codigo',
        label: 'Código',
        type: 'text'
      },
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
      },
      {
        field: 'role',
        label: 'Tipo de Entidade (Papel)',
        type: 'select',
        options: [
          { label: 'Cliente', value: 'isCliente' },
          { label: 'Fornecedor', value: 'isFornecedor' },
          { label: 'Funcionário', value: 'isFuncionario' },
          { label: 'Transportadora', value: 'isTransportadora' },
          { label: 'Comissionado', value: 'isComissionado' },
          { label: 'Filial', value: 'isFilial' },
          { label: 'Representante', value: 'isRepresentante' },
          { label: 'Contador', value: 'isContador' },
          { label: 'Agência Bancária', value: 'isAgencia' },
          { label: 'Inst. Financeira', value: 'isFinanceira' },
          { label: 'Obra', value: 'isObra' },
          { label: 'Prospecto', value: 'isProspecto' },
          { label: 'Aluno', value: 'isAluno' },
          { label: 'Professor', value: 'isProfessor' },
          { label: 'Intermediador', value: 'isIntermediador' }
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

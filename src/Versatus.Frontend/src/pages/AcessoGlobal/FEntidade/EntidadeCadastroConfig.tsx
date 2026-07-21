
import { Box, Chip, Tooltip } from '@mui/material';
import type { ZodTypeAny } from 'zod';
import type { IColunaConfig, IFiltroConfig } from '../../../types/cadastro';
import { BaseCadastroConfig } from '../../../types/cadastro';
import type { IEntidadeForm } from './types';
import { defaultValues } from './types';
import { entidadeSchema } from './schema';
import { buildApiEndpoint } from '../../../config/api';


function formatCpfCnpj(value: string | undefined): string {
  if (!value) return '';
  const clean = value.replace(/\D/g, '');
  if (clean.length === 11) {
    return clean.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  }
  if (clean.length === 14) {
    return clean.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
  }
  return value;
}

export class EntidadeCadastroConfig extends BaseCadastroConfig<IEntidadeForm> {
  getTitulo(): string {
    return 'Entidade';
  }

  getApiEndpoint(): string {
    return buildApiEndpoint('/api/entidade');
  }

  getDefaultValues(): IEntidadeForm {
    return defaultValues;
  }

  getValidationSchema(): ZodTypeAny {
    return entidadeSchema;
  }

  /** Transforma a resposta aninhada da API no formato plano do formulário */
  override mapBackendToForm(backend: any): IEntidadeForm {
    return {
      ...defaultValues,
      idEntidade: backend.idEntidade,
      codigo: String(backend.idEntidade),
      razaoSocial: backend.nome || '',
      apelido: backend.pessoaJuridica?.razaoSocial || '',
      tipoPessoa: backend.tipoPessoa, // Nativamente 2 ou 3 do banco
      ativo: backend.ativo ?? true,
      cpf: backend.pessoaFisica?.cpf || '',
      cnpj: backend.pessoaJuridica?.cnpj || '',
      rg: backend.pessoaFisica?.rg || '',
      isCliente: backend.isCliente || false,
      isFornecedor: backend.isFornecedor || false,
      isFuncionario: backend.isFuncionario || false,
      isTransportadora: backend.isTransportadora || false,
      isComissionado: backend.isComissionado || false,
      isAgencia: backend.isAgenciaBancaria || false,
      isFinanceira: backend.isInstituicaoFinanceira || false,
      isFilial: backend.isFilial || false,
      isObra: backend.isObra || false,
      isRepresentante: backend.isRepresentante || false,
      isOutro: backend.isOutro || false,
      isProspecto: backend.isProspecto || false,
      isContador: backend.isContador || false,
      isAluno: backend.isAluno || false,
      isProfessor: backend.isProfessor || false,
      isIntermediador: backend.isIntermediadorComercial || false,
      inscricaoEstadual: backend.inscricaoEstadual || '',
      inscricaoMunicipal: backend.inscricaoMunicipal || '',
      inscricaoSuframa: backend.inscricaoSuframa || '',
      emailPrincipal: backend.email || '',
      emailNfe: backend.emailNFE || '',
      emailFinanceiro: backend.emailFinanceiro || '',
      emailVendas: backend.emailVenda || '',
      emailCompras: backend.emailCompra || '',
      homePage: backend.homePage || '',
      enderecos: (backend.enderecos || []).map((end: any) => ({
        id: end.idEntidadeEndereco,
        tipo: end.tipoEndereco === 1 ? 'ComercialResidencial' :
              end.tipoEndereco === 2 ? 'Comercial' :
              end.tipoEndereco === 3 ? 'Residencial' :
              end.tipoEndereco === 4 ? 'Entrega' :
              end.tipoEndereco === 5 ? 'Cobranca' : 'Outro',
        logradouro: end.logradouro || '',
        numero: String(end.numero || ''),
        bairro: end.bairro?.nome || '',
        cidade: end.cidade?.nome || '',
        uf: end.cidade?.estado?.sigla || '',
        cep: end.cep || ''
      })),
    } as IEntidadeForm;
  }

  /** Transforma o formulário plano no DTO esperado pela API */
  override mapFormToBackend(form: IEntidadeForm): any {
    return {
      nome: form.razaoSocial || '',
      apelido: form.apelido || '',
      email: form.emailPrincipal || '',
      emailNFE: form.emailNfe || '',
      emailFinanceiro: form.emailFinanceiro || '',
      emailVenda: form.emailVendas || '',
      emailCompra: form.emailCompras || '',
      homePage: form.homePage || '',
      observacao: (form as any).observacao || '',
      inscricaoEstadual: form.inscricaoEstadual || '',
      inscricaoMunicipal: form.inscricaoMunicipal || '',
      inscricaoSuframa: form.inscricaoSuframa || '',
      ativo: form.ativo,
      tipoPessoa: Number(form.tipoPessoa), // Passa nativamente o valor numérico
      cpf: form.cpf || '',
      cnpj: form.cnpj || '',
      rg: form.rg || '',
      isCliente: form.isCliente || false,
      isFornecedor: form.isFornecedor || false,
      isFuncionario: form.isFuncionario || false,
      isTransportadora: form.isTransportadora || false,
      isComissionado: form.isComissionado || false,
      isAgencia: form.isAgencia || false,
      isFinanceira: form.isFinanceira || false,
      isFilial: form.isFilial || false,
      isObra: form.isObra || false,
      isRepresentante: form.isRepresentante || false,
      isOutro: form.isOutro || false,
      isProspecto: form.isProspecto || false,
      isContador: form.isContador || false,
      isAluno: form.isAluno || false,
      isProfessor: form.isProfessor || false,
      isIntermediador: form.isIntermediador || false,
      enderecos: (form.enderecos || []).map((end: any) => ({
        id: end.id || 0,
        tipo: end.tipo || 'ComercialResidencial',
        logradouro: end.logradouro || '',
        numero: end.numero || '',
        cep: end.cep || ''
      }))
    };
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
        header: 'Nome / Nome Fantasia',
        field: 'razaoSocial',
        sortable: true
      },
      {
        header: 'Razão Social',
        field: 'apelido',
        sortable: true
      },
      {
        header: 'Tipo Pessoa',
        field: 'tipoPessoa',
        width: 140,
        sortable: true,
        renderCell: (record) => (record.tipoPessoa === 2 ? 'Física' : 'Jurídica')
      },
      {
        header: 'CPF / CNPJ',
        field: 'cpf',
        width: 180,
        renderCell: (record) => {
          const rawValue = record.tipoPessoa === 2 ? record.cpf : record.cnpj;
          return formatCpfCnpj(rawValue);
        }
      },
      {
        header: 'Tipo da Entidade',
        field: 'role',
        width: 320,
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

          if (roles.length === 0) {
            return <Chip label="Geral" variant="outlined" size="small" sx={{ borderRadius: 1 }} />;
          }

          const visibleRoles = roles.slice(0, 2);
          const hiddenRoles = roles.slice(2);

          return (
            <Box sx={{ display: 'flex', gap: 0.7, flexWrap: 'wrap', alignItems: 'center' }}>
              {visibleRoles.map((role, idx) => (
                <Chip
                  key={idx}
                  label={role}
                  color="primary"
                  variant="outlined"
                  size="small"
                  sx={{ borderRadius: 1, fontWeight: 500 }}
                />
              ))}
              {hiddenRoles.length > 0 && (
                <Tooltip title={hiddenRoles.join(', ')} arrow placement="top">
                  <Chip
                    label={`+${hiddenRoles.length}`}
                    size="small"
                    sx={{ 
                      borderRadius: 1, 
                      bgcolor: 'action.hover', 
                      color: 'text.secondary',
                      fontWeight: 600,
                      cursor: 'pointer'
                    }}
                  />
                </Tooltip>
              )}
            </Box>
          );
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
          { label: 'Física', value: 2 },
          { label: 'Jurídica', value: 3 }
        ]
      },
      {
        field: 'role',
        label: 'Tipo de Entidade (Papel)',
        type: 'select',
        multiple: true,
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
    if (record.tipoPessoa === 2) {
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

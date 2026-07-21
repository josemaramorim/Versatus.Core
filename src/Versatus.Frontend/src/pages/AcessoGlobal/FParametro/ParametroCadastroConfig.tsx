import type { ZodTypeAny } from 'zod';
import type { IColunaConfig, IFiltroConfig } from '../../../types/cadastro';
import { BaseCadastroConfig } from '../../../types/cadastro';
import type { IParametroForm } from './types';
import { defaultValues } from './types';
import { parametroSchema } from './schema';
import { buildApiEndpoint } from '../../../config/api';

export class ParametroCadastroConfig extends BaseCadastroConfig<IParametroForm> {
  getTitulo(): string {
    return 'Parâmetros';
  }

  getApiEndpoint(): string {
    return buildApiEndpoint('/api/parametro');
  }

  getDefaultValues(): IParametroForm {
    return defaultValues;
  }

  getValidationSchema(): ZodTypeAny {
    return parametroSchema;
  }

  override mapBackendToForm(backend: any): IParametroForm {
    return {
      id: backend.id ?? 0,
      chave: backend.chave || '',
      descricao: backend.descricao || '',
      valor: backend.valor || '',
      tipo: backend.tipo ?? 155,
      agrupador: backend.agrupador ?? 0,
      visivel: backend.visivel ?? true,
      idRotina: backend.idRotina,
      tipoParametro: backend.tipoParametro,
      idParametroValor: backend.idParametroValor,
      valorConfigurado: backend.valorConfigurado || '',
      marcado: backend.marcado ?? false,
    };
  }

  override mapFormToBackend(form: IParametroForm): any {
    return {
      id: form.id,
      chave: form.chave,
      descricao: form.descricao,
      valor: form.valor,
      tipo: form.tipo,
      agrupador: form.agrupador,
      visivel: form.visivel,
      idRotina: form.idRotina,
      tipoParametro: form.tipoParametro,
      idParametroValor: form.idParametroValor,
      valorConfigurado: form.valorConfigurado,
      marcado: form.marcado,
    };
  }

  getColunas(): IColunaConfig<IParametroForm>[] {
    return [
      {
        header: 'Chave',
        field: 'chave',
        width: 250,
        sortable: true,
      },
      {
        header: 'Descrição',
        field: 'descricao',
        sortable: true,
      },
      {
        header: 'Valor Configurado',
        field: 'valor',
        width: 300,
        sortable: false,
      },
    ];
  }

  getFiltros(): IFiltroConfig[] {
    return [
      {
        field: 'codigo', // Mapeado no hook como 'search'
        label: 'Chave',
        type: 'text',
      },
      {
        field: 'razaoSocial', // Mapeado no hook como 'search'
        label: 'Descrição',
        type: 'text',
      },
    ];
  }
}

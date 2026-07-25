import React from 'react';
import type { ZodTypeAny } from 'zod';

export type CadastroState = 'browse' | 'insert' | 'edit';
export type CadastroModalMode = 'insert' | 'edit' | 'delete' | 'view' | 'none';

export interface BaseCadastroProps {
  titulo: string;
  state: CadastroState;
  onAdicionar: () => void;
  onEditar?: () => void;
  onSalvar: () => void;
  onDesfazer: () => void;
  onExcluir?: () => void;
  onSair?: () => void;
  children: React.ReactNode;
}

export interface UseCadastroOptions<T> {
  defaultValues: T;
  apiEndpoint?: string;
  onSaveSuccess?: (data: T) => void;
}

export interface ISortConfig {
  column: string;
  direction: 'asc' | 'desc';
}

export interface IColunaConfig<T> {
  header: string;
  field: keyof T | string;
  width?: string | number;
  sortable?: boolean;
  renderCell?: (record: T, index: number) => React.ReactNode;
}

export interface IFiltroConfig {
  field: string;
  label: string;
  type: 'text' | 'select' | 'date' | 'boolean';
  options?: { label: string; value: any }[];
  multiple?: boolean;
}

// Classe abstrata para configuração de qualquer tela de cadastro no ERP (OOP)
export abstract class BaseCadastroConfig<T> {
  abstract getTitulo(): string;
  abstract getApiEndpoint(): string;
  abstract getDefaultValues(): T;
  abstract getColunas(): IColunaConfig<T>[];
  abstract getFiltros(): IFiltroConfig[];
  abstract getValidationSchema(): ZodTypeAny;

  /**
   * Obtém a chave primária / ID do registro dinamicamente.
   */
  getRecordId(record: any): any {
    if (!record) return undefined;
    return record.idCondicaoPagamento ?? record.idEntidade ?? record.idParametro ?? record.id ?? record.codigo;
  }

  /**
   * Transforma o objeto retornado pela API no formato do formulário/estado do front-end.
   * Implementação padrão: retorna os dados como recebidos (sem transformação).
   * Sobrescreva em subclasses quando a API usar uma estrutura diferente do formulário.
   */
  mapBackendToForm(backend: any): T {
    return backend as T;
  }

  /**
   * Transforma o objeto do formulário no formato esperado pela API para POST/PUT.
   * Implementação padrão: retorna os dados como estão.
   * Sobrescreva em subclasses quando necessário.
   */
  mapFormToBackend(form: T): any {
    return form;
  }

  // Gancho opcional para processamento antes de salvar
  beforeSave(record: T): T {
    return record;
  }
}

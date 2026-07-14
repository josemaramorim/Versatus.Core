export type CadastroState = 'browse' | 'insert' | 'edit';

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

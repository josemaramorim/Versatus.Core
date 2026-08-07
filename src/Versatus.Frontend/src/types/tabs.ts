// Tipos da infraestrutura de abas estilo browser do Versatus ERP

export type TabFormMode = 'browse' | 'insert' | 'edit';

export interface TabItem {
  /** UUID único por aba — permite múltiplas abas da mesma rota */
  id: string;
  /** Título exibido na aba. Ex: "Condições de Pagamento" */
  titulo: string;
  /** Rota da página. Ex: "/acesso-global/condicao-pagamento" */
  rota: string;
  /** Ícone opcional (nome do ícone Lucide ou MUI) */
  icone?: string;
  /** Indica se há alterações não salvas (para alerta ao fechar) */
  isDirty?: boolean;
  /** Modo atual do formulário dentro da aba */
  formMode?: TabFormMode;
}

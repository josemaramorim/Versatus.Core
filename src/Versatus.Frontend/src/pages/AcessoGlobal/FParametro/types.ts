export interface IParametroForm {
  id: number;
  chave: string;
  descricao: string;
  valor: string; // Valor padrão do parâmetro (Objeto no legado)
  tipo: number;
  agrupador: number;
  visivel: boolean;
  idRotina?: number;
  tipoParametro?: number;
  idParametroValor?: number;
  valorConfigurado?: string;
  marcado: boolean;
}

export const defaultValues: IParametroForm = {
  id: 0,
  chave: '',
  descricao: '',
  valor: '',
  tipo: 155, // Padrão: 155 = String (Texto)
  agrupador: 0,
  visivel: true,
  marcado: false,
};

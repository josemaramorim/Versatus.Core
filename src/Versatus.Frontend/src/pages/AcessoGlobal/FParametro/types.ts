export interface IParametroForm {
  id: number;
  chave: string;
  descricao: string;
  valor: string;
  tipo: number;
}

export const defaultValues: IParametroForm = {
  id: 0,
  chave: '',
  descricao: '',
  valor: '',
  tipo: 155, // Padrão: 155 = String (Texto)
};

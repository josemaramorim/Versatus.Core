import { useState } from 'react';
import type { CadastroState, UseCadastroOptions } from '../types/cadastro';

export function useBaseCadastro<T>(options: UseCadastroOptions<T>) {
  const [state, setState] = useState<CadastroState>('browse');
  const [record, setRecord] = useState<T>(options.defaultValues);
  const [originalRecord, setOriginalRecord] = useState<T>(options.defaultValues);

  // Ação: Adicionar (Novo Registro)
  const handleAdicionar = () => {
    setOriginalRecord(record); // Guarda histórico
    setRecord(options.defaultValues);
    setState('insert');
  };

  // Ação: Editar (Alterar Registro Atual)
  const handleEditar = () => {
    setOriginalRecord(record);
    setState('edit');
  };

  // Ação: Desfazer (Cancelar edição/inserção)
  const handleDesfazer = () => {
    setRecord(originalRecord);
    setState('browse');
  };

  // Ação: Salvar
  const handleSalvar = (values: T) => {
    // Aqui no futuro fará uma chamada API (POST ou PUT) baseada no apiEndpoint
    console.log('Salvando dados via API...', values);
    setRecord(values);
    setOriginalRecord(values);
    setState('browse');
    
    if (options.onSaveSuccess) {
      options.onSaveSuccess(values);
    }
  };

  // Ação: Excluir
  const handleExcluir = () => {
    console.log('Excluindo registro...', record);
    setRecord(options.defaultValues);
    setOriginalRecord(options.defaultValues);
    setState('browse');
  };

  return {
    state,
    record,
    setState,
    setRecord,
    handleAdicionar,
    handleEditar,
    handleDesfazer,
    handleSalvar,
    handleExcluir,
  };
}

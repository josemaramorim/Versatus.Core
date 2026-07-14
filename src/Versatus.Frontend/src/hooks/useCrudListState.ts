import { useState, useMemo } from 'react';
import { BaseCadastroConfig } from '../types/cadastro';
import type { ISortConfig, CadastroModalMode } from '../types/cadastro';

export function useCrudListState<T>(
  config: BaseCadastroConfig<T>,
  initialRecords: T[] = []
) {
  // Estado principal de registros (Grade)
  const [records, setRecords] = useState<T[]>(initialRecords);

  // Estado de Filtros de Busca
  const [filters, setFilters] = useState<Record<string, any>>({});

  // Estado de Ordenação
  const [sortConfig, setSortConfig] = useState<ISortConfig>({
    column: 'codigo',
    direction: 'asc'
  });

  // Estado do Modal
  const [modalMode, setModalMode] = useState<CadastroModalMode>('none');
  const [selectedRecord, setSelectedRecord] = useState<T>(config.getDefaultValues());

  const isModalOpen = modalMode !== 'none';

  // Lógica de alteração e limpeza de filtros
  const handleFilterChange = (field: string, value: any) => {
    setFilters((prev) => ({
      ...prev,
      [field]: value
    }));
  };

  const handleClearFilters = () => {
    setFilters({});
  };

  // Alternar ordenação de coluna
  const handleSort = (column: string) => {
    setSortConfig((prev) => {
      if (prev.column === column) {
        return {
          column,
          direction: prev.direction === 'asc' ? 'desc' : 'asc'
        };
      }
      return { column, direction: 'asc' };
    });
  };

  // Funções de abertura de Modal por Ações
  const onAdicionarClick = () => {
    setSelectedRecord(config.getDefaultValues());
    setModalMode('insert');
  };

  const onEditarClick = (record: T) => {
    setSelectedRecord(record);
    setModalMode('edit');
  };

  const onDeletarClick = (record: T) => {
    setSelectedRecord(record);
    setModalMode('delete');
  };

  const onModalClose = () => {
    setModalMode('none');
  };

  // Confirmação de Operações no Formulário
  const onSave = (recordData: T) => {
    // Gancho de pré-gravação da classe de configuração (OOP)
    const processedRecord = config.beforeSave(recordData);

    if (modalMode === 'insert') {
      // Inserção
      setRecords((prev) => [...prev, processedRecord]);
    } else if (modalMode === 'edit') {
      // Edição (Encontrar e substituir o objeto correspondente)
      setRecords((prev) =>
        prev.map((item) => {
          // Identificador padrão no ERP geralmente é 'codigo' ou 'id'
          const keyField = 'codigo' in (item as any) ? 'codigo' : 'id';
          if ((item as any)[keyField] === (processedRecord as any)[keyField]) {
            return processedRecord;
          }
          return item;
        })
      );
    }
    setModalMode('none');
  };

  const onDeleteConfirm = () => {
    // Exclusão
    setRecords((prev) =>
      prev.filter((item) => {
        const keyField = 'codigo' in (item as any) ? 'codigo' : 'id';
        return (item as any)[keyField] !== (selectedRecord as any)[keyField];
      })
    );
    setModalMode('none');
  };

  // Lógica de Filtragem e Ordenação dos dados em tempo real
  const filteredAndSortedRecords = useMemo(() => {
    let result = [...records];

    // 1. Filtrar registros com base no estado de filtros configurados
    Object.keys(filters).forEach((key) => {
      const filterValue = filters[key];
      if (filterValue === undefined || filterValue === null || filterValue === '') {
        return;
      }

      result = result.filter((item) => {
        const value = (item as any)[key];
        if (value === undefined || value === null) return false;

        // Se for string, busca parcial sem case-sensitive
        if (typeof value === 'string') {
          return value.toLowerCase().includes(filterValue.toString().toLowerCase());
        }
        
        // Se for booleano, correspondência exata
        if (typeof value === 'boolean') {
          return value === filterValue;
        }

        // Outros tipos, correspondência de string exata
        return value.toString() === filterValue.toString();
      });
    });

    // 2. Ordenar registros
    const { column, direction } = sortConfig;
    if (column) {
      result.sort((a, b) => {
        let valA = (a as any)[column];
        let valB = (b as any)[column];

        if (valA === undefined || valA === null) valA = '';
        if (valB === undefined || valB === null) valB = '';

        if (typeof valA === 'string' && typeof valB === 'string') {
          return direction === 'asc' 
            ? valA.localeCompare(valB) 
            : valB.localeCompare(valA);
        }

        if (valA < valB) return direction === 'asc' ? -1 : 1;
        if (valA > valB) return direction === 'asc' ? 1 : -1;
        return 0;
      });
    }

    return result;
  }, [records, filters, sortConfig]);

  return {
    records: filteredAndSortedRecords,
    filters,
    sortConfig,
    modalMode,
    selectedRecord,
    isModalOpen,
    handleFilterChange,
    handleClearFilters,
    handleSort,
    onAdicionarClick,
    onEditarClick,
    onDeletarClick,
    onModalClose,
    onSave,
    onDeleteConfirm,
    setRecords
  };
}

import { useState, useMemo } from 'react';
import { BaseCadastroConfig } from '../types/cadastro';
import type { ISortConfig, CadastroModalMode } from '../types/cadastro';

export function useCrudListState<T>(
  config: BaseCadastroConfig<T>,
  initialRecords: T[] = []
) {
  // Estado principal de registros (Grade)
  const [records, setRecords] = useState<T[]>(initialRecords);

  // Filtros aplicados na Grid
  const [filters, setFilters] = useState<Record<string, any>>({});
  // Filtros temporários digitados na Gaveta (Rascunho)
  const [tempFilters, setTempFilters] = useState<Record<string, any>>({});

  // Controle de abertura da Gaveta de Filtros
  const [isFilterDrawerOpen, setIsFilterDrawerOpen] = useState(false);

  // Estados de Paginação
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  // Estado de Ordenação
  const [sortConfig, setSortConfig] = useState<ISortConfig>({
    column: 'codigo',
    direction: 'asc'
  });

  // Estado do Modal
  const [modalMode, setModalMode] = useState<CadastroModalMode>('none');
  const [selectedRecord, setSelectedRecord] = useState<T>(config.getDefaultValues());

  const isModalOpen = modalMode !== 'none';

  // Lógica de alteração de filtros temporários na Gaveta
  const handleFilterChange = (field: string, value: any) => {
    setTempFilters((prev) => ({
      ...prev,
      [field]: value
    }));
  };

  // Abrir a gaveta e sincronizar o rascunho com os filtros atualmente aplicados
  const handleOpenFilterDrawer = () => {
    setTempFilters(filters);
    setIsFilterDrawerOpen(true);
  };

  // Aplicar os filtros do rascunho na Grid
  const handleApplyFilters = () => {
    setFilters(tempFilters);
    setPage(0); // Volta para a primeira página
    setIsFilterDrawerOpen(false);
  };

  // Limpar todos os filtros
  const handleClearFilters = () => {
    setFilters({});
    setTempFilters({});
    setPage(0);
    setIsFilterDrawerOpen(false);
  };

  // Remover um filtro específico clicando no Chip
  const handleRemoveFilterChip = (field: string) => {
    const updatedFilters = { ...filters };
    delete updatedFilters[field];

    const updatedTempFilters = { ...tempFilters };
    delete updatedTempFilters[field];

    setFilters(updatedFilters);
    setTempFilters(updatedTempFilters);
    setPage(0);
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

  // Funções de paginação
  const handleChangePage = (_: any, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
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
    const processedRecord = config.beforeSave(recordData);

    if (modalMode === 'insert') {
      setRecords((prev) => [...prev, processedRecord]);
    } else if (modalMode === 'edit') {
      setRecords((prev) =>
        prev.map((item) => {
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
    setRecords((prev) =>
      prev.filter((item) => {
        const keyField = 'codigo' in (item as any) ? 'codigo' : 'id';
        return (item as any)[keyField] !== (selectedRecord as any)[keyField];
      })
    );
    setModalMode('none');
  };

  // Lógica de Filtragem e Ordenação dos dados (em memória)
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

        if (typeof value === 'string') {
          return value.toLowerCase().includes(filterValue.toString().toLowerCase());
        }
        
        if (typeof value === 'boolean') {
          return value === filterValue;
        }

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

  // 3. Fazer fatiamento da Paginação sobre a lista filtrada/ordenada
  const paginatedRecords = useMemo(() => {
    const startIndex = page * rowsPerPage;
    return filteredAndSortedRecords.slice(startIndex, startIndex + rowsPerPage);
  }, [filteredAndSortedRecords, page, rowsPerPage]);

  return {
    records: paginatedRecords, // Registros já fatiados para a página atual
    totalRecords: filteredAndSortedRecords.length, // Total após os filtros aplicados
    filters,
    tempFilters,
    isFilterDrawerOpen,
    page,
    rowsPerPage,
    sortConfig,
    modalMode,
    selectedRecord,
    isModalOpen,
    handleFilterChange,
    handleOpenFilterDrawer,
    handleApplyFilters,
    handleClearFilters,
    handleRemoveFilterChip,
    handleSort,
    handleChangePage,
    handleChangeRowsPerPage,
    onAdicionarClick,
    onEditarClick,
    onDeletarClick,
    onModalClose,
    onSave,
    onDeleteConfirm,
    setIsFilterDrawerOpen,
    setRecords
  };
}

import { useState, useEffect, useCallback } from 'react';
import { BaseCadastroConfig } from '../types/cadastro';
import type { ISortConfig, CadastroModalMode } from '../types/cadastro';
import { getApiHeaders } from '../config/api';

export function useCrudListState<T>(
  config: BaseCadastroConfig<T>,
  initialRecords: T[] = [],
  options?: {
    onError?: (message: string) => void;
    onSuccess?: (message: string) => void;
  }
) {
  const [records, setRecords] = useState<T[]>(initialRecords);
  const [totalRecords, setTotalRecords] = useState<number>(initialRecords.length);
  const [loading, setLoading] = useState<boolean>(false);

  const [filters, setFilters] = useState<Record<string, any>>({});
  const [tempFilters, setTempFilters] = useState<Record<string, any>>({});
  const [isFilterDrawerOpen, setIsFilterDrawerOpen] = useState(false);

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [sortConfig, setSortConfig] = useState<ISortConfig>({
    column: 'codigo',
    direction: 'asc'
  });

  const [modalMode, setModalMode] = useState<CadastroModalMode>('none');
  const [selectedRecord, setSelectedRecord] = useState<T>(config.getDefaultValues());

  const isModalOpen = modalMode !== 'none';

  // --- Chamada à API ---
  const fetchRecords = useCallback(async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams({
        page: String(page + 1),
        limit: String(rowsPerPage),
        sortBy: sortConfig.column,
        sortOrder: sortConfig.direction,
        search: filters.razaoSocial || filters.codigo || filters.apelido || filters.termoBusca || '',
        role: Array.isArray(filters.role) ? filters.role.join(',') : (filters.role || ''),
        tipoPessoa: filters.tipoPessoa !== undefined && filters.tipoPessoa !== null ? String(filters.tipoPessoa) : ''
      });

      const response = await fetch(`${config.getApiEndpoint()}/paginado?${params.toString()}`, {
        headers: getApiHeaders()
      });
      if (!response.ok) {
        throw new Error('Erro ao buscar dados na API');
      }

      const data = await response.json();
      const rawItems = data.items || [];
      const mappedItems = rawItems.map((item: any) => config.mapBackendToForm(item));

      setRecords(mappedItems);
      setTotalRecords(data.total || 0);
    } catch (error) {
      console.error('Erro na requisição paginada:', error);
    } finally {
      setLoading(false);
    }
  }, [page, rowsPerPage, sortConfig, filters, config]);

  useEffect(() => {
    fetchRecords();
  }, [fetchRecords]);

  // --- Filtros ---
  const handleFilterChange = (field: string, value: any) => {
    setTempFilters((prev) => ({
      ...prev,
      [field]: value
    }));
  };

  const handleOpenFilterDrawer = () => {
    setTempFilters(filters);
    setIsFilterDrawerOpen(true);
  };

  const handleApplyFilters = () => {
    setFilters(tempFilters);
    setPage(0);
    setIsFilterDrawerOpen(false);
  };

  const handleClearFilters = () => {
    setFilters({});
    setTempFilters({});
    setPage(0);
    setIsFilterDrawerOpen(false);
  };

  const handleRemoveFilterChip = (field: string) => {
    const updatedFilters = { ...filters };
    delete updatedFilters[field];

    const updatedTempFilters = { ...tempFilters };
    delete updatedTempFilters[field];

    setFilters(updatedFilters);
    setTempFilters(updatedTempFilters);
    setPage(0);
  };

  // --- Ordenação & Paginação ---
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

  const handleChangePage = (_: any, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  // --- Modal Ações ---
  const onAdicionarClick = () => {
    setSelectedRecord(config.getDefaultValues());
    setModalMode('insert');
  };

  const onEditarClick = async (record: T) => {
    setLoading(true);
    try {
      const id = config.getRecordId(record);
      if (!id) {
        setSelectedRecord(config.mapBackendToForm(record));
        setModalMode('edit');
        return;
      }

      let data: any = null;

      // 1. Tentar primeiro o endpoint de busca completa /completo/{id}
      try {
        const resCompleto = await fetch(`${config.getApiEndpoint()}/completo/${id}`, {
          headers: getApiHeaders()
        });
        if (resCompleto.ok) {
          data = await resCompleto.json();
        }
      } catch (err) {
        console.warn('Endpoint /completo indisponível, buscando por id direto:', err);
      }

      // 2. Se /completo/{id} não retornou dados, tentar o endpoint padrão /{id}
      if (!data) {
        const resBase = await fetch(`${config.getApiEndpoint()}/${id}`, {
          headers: getApiHeaders()
        });
        if (resBase.ok) {
          data = await resBase.json();
        }
      }

      // 3. Mapear os dados detalhados da API (ou fallback do registro da grid) para a estrutura do formulário
      const mappedRecord = config.mapBackendToForm(data || record);
      setSelectedRecord(mappedRecord);
      setModalMode('edit');
    } catch (err) {
      console.error('Erro ao buscar detalhes para edição:', err);
      setSelectedRecord(config.mapBackendToForm(record));
      setModalMode('edit');
    } finally {
      setLoading(false);
    }
  };

  const onDeletarClick = (record: T) => {
    setSelectedRecord(record);
    setModalMode('delete');
  };

  const onModalClose = () => {
    setModalMode('none');
  };

  // --- Persistência de Dados ---
  const onSave = async (recordData: T) => {
    setLoading(true);
    try {
      // Mescla o estado original (selectedRecord) com as alterações do formulário (recordData)
      // para preservar idEntidade, idCondicaoPagamento, e outras propriedades internas não expostas na tela.
      const mergedRecord = modalMode === 'edit'
        ? { ...selectedRecord, ...recordData }
        : recordData;

      const processedRecord = config.beforeSave(mergedRecord);
      
      const id = config.getRecordId(processedRecord);
      const url = modalMode === 'edit' 
        ? `${config.getApiEndpoint()}/${id}`
        : config.getApiEndpoint();

      const method = modalMode === 'edit' ? 'PUT' : 'POST';
      const body = config.mapFormToBackend(processedRecord);

      const response = await fetch(url, {
        method,
        headers: getApiHeaders({ 'Content-Type': 'application/json' }),
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        const errData = await response.json().catch(() => ({}));
        const mainMessage = errData.message || `Erro ao persistir ${config.getTitulo().toLowerCase()}.`;
        const techDetail = errData.error || '';
        const combinedMessage = techDetail 
          ? `${mainMessage}\n\n[Detalhe técnico para o suporte: ${techDetail}]`
          : mainMessage;
        throw new Error(combinedMessage);
      }

      setModalMode('none');
      await fetchRecords();
    } catch (err: any) {
      console.error(err);
      const msg = err.message || `Erro ao persistir ${config.getTitulo().toLowerCase()}.`;
      if (options?.onError) {
        options.onError(msg);
      } else {
        alert(msg);
      }
    } finally {
      setLoading(false);
    }
  };

  const onDeleteConfirm = async () => {
    setLoading(true);
    try {
      const id = config.getRecordId(selectedRecord);
      const response = await fetch(`${config.getApiEndpoint()}/${id}`, {
        method: 'DELETE',
        headers: getApiHeaders()
      });

      if (!response.ok) {
        const errData = await response.json().catch(() => ({}));
        const mainMessage = errData.message || 'Erro ao excluir registro.';
        const techDetail = errData.error || '';
        const combinedMessage = techDetail 
          ? `${mainMessage}\n\n[Detalhe técnico para o suporte: ${techDetail}]`
          : mainMessage;
        throw new Error(combinedMessage);
      }

      setModalMode('none');
      await fetchRecords();
    } catch (err: any) {
      console.error(err);
      const msg = err.message || 'Erro ao excluir registro.';
      if (options?.onError) {
        options.onError(msg);
      } else {
        alert(msg);
      }
    } finally {
      setLoading(false);
    }
  };

  return {
    records,
    totalRecords,
    loading,
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

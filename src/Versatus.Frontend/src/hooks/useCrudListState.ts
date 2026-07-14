import { useState, useEffect, useCallback } from 'react';
import { BaseCadastroConfig } from '../types/cadastro';
import type { ISortConfig, CadastroModalMode } from '../types/cadastro';

export function useCrudListState<T>(
  config: BaseCadastroConfig<T>,
  initialRecords: T[] = []
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
  const isEntidade = config.getApiEndpoint() === '/api/entidade';

  // --- Mapeadores de Entidade (Front <-> API) ---
  const mapBackendToForm = useCallback((backend: any): any => {
    return {
      idEntidade: backend.idEntidade,
      codigo: `ENT-${String(backend.idEntidade).padStart(4, '0')}`,
      razaoSocial: backend.pessoaJuridica?.razaoSocial || backend.nome || '',
      apelido: backend.nome || '',
      tipoPessoa: backend.tipoPessoa || 1,
      ativo: backend.ativo ?? true,
      cpf: backend.pessoaFisica?.cpf || '',
      cnpj: backend.pessoaJuridica?.cnpj || '',
      rg: backend.pessoaFisica?.rg || '',
      isCliente: backend.isCliente || false,
      isFornecedor: backend.isFornecedor || false,
      isFuncionario: backend.isFuncionario || false,
      isTransportadora: backend.isTransportadora || false,
      isComissionado: backend.isComissionado || false,
      isAgencia: backend.isAgenciaBancaria || false,
      isFinanceira: backend.isInstituicaoFinanceira || false,
      isFilial: backend.isFilial || false,
      isObra: backend.isObra || false,
      isRepresentante: backend.isRepresentante || false,
      isOutro: backend.isOutro || false,
      isProspecto: backend.isProspecto || false,
      isContador: backend.isContador || false,
      isAluno: backend.isAluno || false,
      isProfessor: backend.isProfessor || false,
      isIntermediador: backend.isIntermediadorComercial || false,
      inscricaoEstadual: backend.inscricaoEstadual || '',
      inscricaoMunicipal: backend.inscricaoMunicipal || '',
      inscricaoSuframa: backend.inscricaoSuframa || '',
      emailPrincipal: backend.email || '',
      emailNfe: backend.emailNFE || '',
      emailFinanceiro: backend.emailFinanceiro || '',
      emailVendas: backend.emailVenda || '',
      emailCompras: backend.emailCompra || '',
      homePage: backend.homePage || '',
      observacao: backend.observacao || '',
      enderecos: (backend.enderecos || []).map((end: any) => ({
        id: end.idEntidadeEndereco,
        tipo: end.tipoEndereco === 1 ? 'ComercialResidencial' :
              end.tipoEndereco === 2 ? 'Comercial' :
              end.tipoEndereco === 3 ? 'Residencial' :
              end.tipoEndereco === 4 ? 'Entrega' :
              end.tipoEndereco === 5 ? 'Cobranca' : 'Outro',
        logradouro: end.logradouro || '',
        numero: String(end.numero || ''),
        bairro: end.bairro?.nome || '',
        cidade: end.cidade?.nome || '',
        uf: end.cidade?.estado?.sigla || '',
        cep: end.cep || ''
      })),
      telefones: [],
      contatos: [],
      cnaes: [],
      empresas: []
    };
  }, []);

  const mapFormToBackendDto = useCallback((form: any): any => {
    return {
      nome: form.razaoSocial || '',
      apelido: form.apelido || '',
      email: form.emailPrincipal || '',
      emailNFE: form.emailNfe || '',
      emailFinanceiro: form.emailFinanceiro || '',
      emailVenda: form.emailVendas || '',
      emailCompra: form.emailCompras || '',
      homePage: form.homePage || '',
      observacao: form.observacao || '',
      inscricaoEstadual: form.inscricaoEstadual || '',
      inscricaoMunicipal: form.inscricaoMunicipal || '',
      inscricaoSuframa: form.inscricaoSuframa || '',
      ativo: form.ativo,
      tipoPessoa: Number(form.tipoPessoa),
      cpf: form.cpf || '',
      cnpj: form.cnpj || '',
      rg: form.rg || '',
      isCliente: form.isCliente || false,
      isFornecedor: form.isFornecedor || false,
      isFuncionario: form.isFuncionario || false,
      isTransportadora: form.isTransportadora || false,
      isComissionado: form.isComissionado || false,
      isAgencia: form.isAgencia || false,
      isFinanceira: form.isFinanceira || false,
      isFilial: form.isFilial || false,
      isObra: form.isObra || false,
      isRepresentante: form.isRepresentante || false,
      isOutro: form.isOutro || false,
      isProspecto: form.isProspecto || false,
      isContador: form.isContador || false,
      isAluno: form.isAluno || false,
      isProfessor: form.isProfessor || false,
      isIntermediador: form.isIntermediador || false,
      enderecos: (form.enderecos || []).map((end: any) => ({
        id: end.id || 0,
        tipo: end.tipo || 'ComercialResidencial',
        logradouro: end.logradouro || '',
        numero: end.numero || '',
        cep: end.cep || ''
      }))
    };
  }, []);

  // --- Chamada à API ---
  const fetchRecords = useCallback(async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams({
        page: String(page + 1),
        limit: String(rowsPerPage),
        sortBy: sortConfig.column,
        sortOrder: sortConfig.direction,
        search: filters.termoBusca || '',
        role: filters.role || ''
      });

      const response = await fetch(`${config.getApiEndpoint()}/paginado?${params.toString()}`);
      if (!response.ok) {
        throw new Error('Erro ao buscar dados na API');
      }

      const data = await response.json();
      const rawItems = data.items || [];
      
      const mappedItems = isEntidade 
        ? rawItems.map((item: any) => mapBackendToForm(item)) 
        : rawItems;

      setRecords(mappedItems);
      setTotalRecords(data.total || 0);
    } catch (error) {
      console.error('Erro na requisição paginada:', error);
    } finally {
      setLoading(false);
    }
  }, [page, rowsPerPage, sortConfig, filters, config, isEntidade, mapBackendToForm]);

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
      const id = (record as any).idEntidade || (record as any).id;
      if (!id) {
        setSelectedRecord(record);
        setModalMode('edit');
        return;
      }

      const response = await fetch(`${config.getApiEndpoint()}/completo/${id}`);
      if (!response.ok) {
        throw new Error('Erro ao buscar detalhes da entidade');
      }

      const data = await response.json();
      const mappedRecord = isEntidade ? mapBackendToForm(data) : data;
      setSelectedRecord(mappedRecord);
      setModalMode('edit');
    } catch (err) {
      console.error(err);
      setSelectedRecord(record);
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
      const processedRecord = config.beforeSave(recordData);
      const url = modalMode === 'edit' 
        ? `${config.getApiEndpoint()}/${(processedRecord as any).idEntidade}`
        : config.getApiEndpoint();

      const method = modalMode === 'edit' ? 'PUT' : 'POST';
      const body = isEntidade ? mapFormToBackendDto(processedRecord) : processedRecord;

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        const errData = await response.json().catch(() => ({}));
        throw new Error(errData.message || 'Erro ao persistir entidade');
      }

      setModalMode('none');
      await fetchRecords();
    } catch (err: any) {
      console.error(err);
      alert(err.message || 'Erro ao persistir registro.');
    } finally {
      setLoading(false);
    }
  };

  const onDeleteConfirm = async () => {
    setLoading(true);
    try {
      const id = (selectedRecord as any).idEntidade || (selectedRecord as any).id;
      const response = await fetch(`${config.getApiEndpoint()}/${id}`, {
        method: 'DELETE'
      });

      if (!response.ok) {
        throw new Error('Erro ao excluir registro');
      }

      setModalMode('none');
      await fetchRecords();
    } catch (err) {
      console.error(err);
      alert('Erro ao excluir registro.');
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

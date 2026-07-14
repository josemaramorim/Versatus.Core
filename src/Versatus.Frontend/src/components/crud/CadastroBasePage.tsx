import React from 'react';
import { 
  Box, 
  Card, 
  Button, 
  Typography, 
  Breadcrumbs, 
  Link,
  Snackbar,
  Alert,
  Badge,
  Chip,
  TablePagination,
  Stack
} from '@mui/material';
import { SlidersHorizontal } from 'lucide-react';
import { BaseCadastroConfig } from '../../types/cadastro';
import type { CadastroModalMode } from '../../types/cadastro';
import { useCrudListState } from '../../hooks/useCrudListState';
import { CrudTable } from './CrudTable';
import { CrudModal } from './CrudModal';
import { CrudFilterDrawer } from './CrudFilterDrawer';

export interface ICadastroBasePageProps<T> {
  config: BaseCadastroConfig<T>;
  initialRecords?: T[];
  renderForm: (mode: CadastroModalMode, record: T, onSave: (data: T) => void) => React.ReactNode;
}

export function CadastroBasePage<T>({
  config,
  initialRecords = [],
  renderForm
}: ICadastroBasePageProps<T>) {
  const {
    records,
    totalRecords,
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
    setIsFilterDrawerOpen
  } = useCrudListState<T>(config, initialRecords);

  const [toast, setToast] = React.useState<{ open: boolean; message: string; severity: 'success' | 'error' }>({
    open: false,
    message: '',
    severity: 'success'
  });

  const showToast = (message: string, severity: 'success' | 'error') => {
    setToast({ open: true, message, severity });
  };

  const handleSaveWrapper = (data: T) => {
    onSave(data);
    showToast('Registro salvo com sucesso!', 'success');
  };

  const handleDeleteConfirmWrapper = () => {
    onDeleteConfirm();
    showToast('Registro excluído com sucesso!', 'success');
  };

  const activeFilters = config.getFiltros();
  const activeFilterCount = Object.keys(filters).length;

  // Obter o label amigável de exibição do filtro ativo
  const getFilterDisplayLabel = (key: string, value: any) => {
    const filterConfig = activeFilters.find((f) => f.field === key);
    if (!filterConfig) return `${key}: ${value}`;

    if (filterConfig.type === 'select') {
      const selectedOption = filterConfig.options?.find((o) => o.value === value);
      return `${filterConfig.label}: ${selectedOption ? selectedOption.label : value}`;
    }

    if (filterConfig.type === 'date') {
      // Converte data YYYY-MM-DD para DD/MM/YYYY
      const dateParts = value.split('-');
      if (dateParts.length === 3) {
        return `${filterConfig.label}: ${dateParts[2]}/${dateParts[1]}/${dateParts[0]}`;
      }
    }

    return `${filterConfig.label}: ${value}`;
  };

  return (
    <Box sx={{ p: 4, minHeight: '100vh', bgcolor: 'background.default' }}>
      
      {/* 1. Breadcrumbs */}
      <Breadcrumbs aria-label="breadcrumb" sx={{ mb: 1 }}>
        <Link underline="hover" color="inherit" href="#" sx={{ fontSize: '0.85rem' }}>
          Dashboard
        </Link>
        <Link underline="hover" color="inherit" href="#" sx={{ fontSize: '0.85rem' }}>
          Cadastros Base
        </Link>
        <Typography color="text.primary" sx={{ fontSize: '0.85rem', fontWeight: 500 }}>
          {config.getTitulo()}
        </Typography>
      </Breadcrumbs>

      {/* 2. Cabeçalho Principal da Tela de Consulta */}
      <Box 
        sx={{ 
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 4 
        }}
      >
        <Typography variant="h4" component="h1" sx={{ color: 'text.primary', fontWeight: 800 }}>
          {config.getTitulo()}
        </Typography>

        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          {/* Botão de Filtros com Badge de contagem de filtros ativos */}
          {activeFilters.length > 0 && (
            <Badge 
              badgeContent={activeFilterCount} 
              color="primary" 
              sx={{ '& .MuiBadge-badge': { right: 3, top: 3 } }}
            >
              <Button
                variant="outlined"
                color="inherit"
                startIcon={<SlidersHorizontal size={16} />}
                onClick={handleOpenFilterDrawer}
                sx={{ borderColor: 'divider' }}
              >
                Filtros
              </Button>
            </Badge>
          )}

          <Button
            variant="contained"
            color="primary"
            onClick={onAdicionarClick}
          >
            Novo
          </Button>
        </Stack>
      </Box>

      {/* 3. Chips de Filtros Aplicados */}
      {activeFilterCount > 0 && (
        <Box 
          sx={{ 
            display: 'flex', 
            flexWrap: 'wrap', 
            gap: 1, 
            mb: 3, 
            alignItems: 'center',
            p: 1.5,
            bgcolor: 'background.paper',
            borderRadius: 1,
            border: '1px solid',
            borderColor: 'divider'
          }}
        >
          <Typography variant="body2" sx={{ fontWeight: 600, color: 'text.secondary', mr: 1 }}>
            Filtros aplicados:
          </Typography>
          {Object.keys(filters).map((key) => (
            <Chip
              key={key}
              label={getFilterDisplayLabel(key, filters[key])}
              onDelete={() => handleRemoveFilterChip(key)}
              color="primary"
              variant="outlined"
              size="small"
              sx={{ borderRadius: 1 }}
            />
          ))}
        </Box>
      )}

      {/* 4. Grade Principal de Dados */}
      <Card sx={{ p: 3, borderRadius: 1.5, boxShadow: 'rgba(145, 158, 171, 0.08) 0px 0px 2px 0px, rgba(145, 158, 171, 0.08) 0px 12px 24px -4px' }}>
        <CrudTable
          records={records}
          colunas={config.getColunas()}
          sortConfig={sortConfig}
          handleSort={handleSort}
          onEditarClick={onEditarClick}
          onDeletarClick={onDeletarClick}
        />
        
        {/* Barra de Paginação da Grid */}
        <TablePagination
          rowsPerPageOptions={[5, 10, 25, 50]}
          component="div"
          count={totalRecords}
          rowsPerPage={rowsPerPage}
          page={page}
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Linhas por página:"
          labelDisplayedRows={({ from, to, count }) => `${from}–${to} de ${count}`}
          sx={{ mt: 1, borderTop: '1px solid', borderColor: 'divider' }}
        />
      </Card>

      {/* 5. Gaveta de Filtros Responsiva */}
      <CrudFilterDrawer
        open={isFilterDrawerOpen}
        onClose={() => setIsFilterDrawerOpen(false)}
        filtros={activeFilters}
        filterValues={tempFilters}
        onFilterChange={handleFilterChange}
        onApplyFilters={handleApplyFilters}
        onClearFilters={handleClearFilters}
      />

      {/* 6. Modal Genérico de Inserção / Edição / Exclusão */}
      <CrudModal
        open={isModalOpen}
        mode={modalMode}
        titulo={config.getTitulo()}
        onClose={onModalClose}
        onSave={() => {
          const submitBtn = document.getElementById('crud-submit-btn');
          if (submitBtn) {
            submitBtn.click();
          }
        }}
        onDeleteConfirm={handleDeleteConfirmWrapper}
        onUndo={() => {
          const resetBtn = document.getElementById('crud-reset-btn');
          if (resetBtn) {
            resetBtn.click();
          }
        }}
      >
        {isModalOpen && renderForm(modalMode, selectedRecord, handleSaveWrapper)}
      </CrudModal>

      {/* Toast de Notificações */}
      <Snackbar
        open={toast.open}
        autoHideDuration={4000}
        onClose={() => setToast((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setToast((prev) => ({ ...prev, open: false }))}
          severity={toast.severity}
          variant="filled"
          sx={{ width: '100%' }}
        >
          {toast.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}

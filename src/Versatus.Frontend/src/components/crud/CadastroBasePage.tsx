import React from 'react';
import { 
  Box, 
  Card, 
  Button, 
  Typography, 
  Breadcrumbs, 
  Link,
  Snackbar,
  Alert
} from '@mui/material';
import { Plus } from 'lucide-react';
import { BaseCadastroConfig } from '../../types/cadastro';
import type { CadastroModalMode } from '../../types/cadastro';
import { useCrudListState } from '../../hooks/useCrudListState';
import { CrudFilters } from './CrudFilters';
import { CrudTable } from './CrudTable';
import { CrudModal } from './CrudModal';

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
    onDeleteConfirm
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

        <Button
          variant="contained"
          color="primary"
          startIcon={<Plus size={16} />}
          onClick={onAdicionarClick}
        >
          Novo
        </Button>
      </Box>

      {/* 3. Painel de Filtros Customizados */}
      <CrudFilters
        filtros={config.getFiltros()}
        filterValues={filters}
        onFilterChange={handleFilterChange}
        onClearFilters={handleClearFilters}
      />

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
      </Card>

      {/* 5. Modal Genérico de Inserção / Edição / Exclusão */}
      <CrudModal
        open={isModalOpen}
        mode={modalMode}
        titulo={config.getTitulo()}
        onClose={onModalClose}
        onSave={() => {
          // Dispara o evento de submissão do formulário HTML associado por ID
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
        {/* Injeta o formulário específico de abas */}
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

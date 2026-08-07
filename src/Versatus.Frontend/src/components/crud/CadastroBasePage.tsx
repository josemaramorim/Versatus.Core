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
  Stack,
  LinearProgress
} from '@mui/material';
import { SlidersHorizontal, Plus, Trash2 } from 'lucide-react';
import { BaseCadastroConfig } from '../../types/cadastro';
import type { CadastroModalMode } from '../../types/cadastro';
import { useCrudListState } from '../../hooks/useCrudListState';
import { CrudTable } from './CrudTable';
import { CrudFilterDrawer } from './CrudFilterDrawer';
import { BaseCadastro } from '../layout/BaseCadastro';

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
    loading,
    filters,
    tempFilters,
    isFilterDrawerOpen,
    page,
    rowsPerPage,
    sortConfig,
    modalMode,
    selectedRecord,
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
  } = useCrudListState<T>(config, initialRecords, {
    onError: (msg) => showToast(msg, 'error'),
    onSuccess: (msg) => showToast(msg, 'success')
  });

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
  };

  const handleDeleteConfirmWrapper = () => {
    onDeleteConfirm();
  };

  const activeFilters = config.getFiltros();
  const activeFilterCount = Object.keys(filters).length;

  // Obter o label amigável do registro selecionado para exclusão
  const getRecordDisplayName = (record: any): string => {
    if (!record) return 'selecionado';
    return (
      record.descricao ||
      record.razaoSocial ||
      record.nome ||
      record.nomeFantasia ||
      record.apelido ||
      (record.codigo ? `#${record.codigo}` : 'selecionado')
    );
  };

  // Obter o label amigável de exibição do filtro ativo
  const getFilterDisplayLabel = (key: string, value: any) => {
    const filterConfig = activeFilters.find((f) => f.field === key);
    if (!filterConfig) return `${key}: ${value}`;

    if (filterConfig.type === 'select') {
      if (Array.isArray(value)) {
        const labels = value.map(val => {
          const selectedOption = filterConfig.options?.find((o) => o.value === val);
          return selectedOption ? selectedOption.label : val;
        });
        return `${filterConfig.label}: ${labels.join(', ')}`;
      }
      const selectedOption = filterConfig.options?.find((o) => o.value === value);
      return `${filterConfig.label}: ${selectedOption ? selectedOption.label : value}`;
    }

    if (filterConfig.type === 'date') {
      const dateParts = value.split('-');
      if (dateParts.length === 3) {
        return `${filterConfig.label}: ${dateParts[2]}/${dateParts[1]}/${dateParts[0]}`;
      }
    }

    return `${filterConfig.label}: ${value}`;
  };

  // ---------------------------------------------------------------------------
  // MODO FORMULÁRIO INLINE: Inserção ('insert') ou Edição ('edit')
  // Renderizado ocupando a área de conteúdo da aba (SEM MODAL / SEM OVERLAY DIALOG)
  // ---------------------------------------------------------------------------
  if (modalMode === 'insert' || modalMode === 'edit') {
    const cadastroState = modalMode === 'insert' ? 'insert' : 'edit';

    return (
      <BaseCadastro
        titulo={config.getTitulo()}
        state={cadastroState}
        onAdicionar={onAdicionarClick}
        onSalvar={() => {
          const submitBtn = document.getElementById('crud-submit-btn');
          if (submitBtn) {
            submitBtn.click();
          }
        }}
        onDesfazer={() => {
          const resetBtn = document.getElementById('crud-reset-btn');
          if (resetBtn) {
            resetBtn.click();
          }
        }}
        onSair={onModalClose}
      >
        {renderForm(modalMode, selectedRecord, handleSaveWrapper)}
      </BaseCadastro>
    );
  }

  // ---------------------------------------------------------------------------
  // MODO LISTAGEM (GRID / BROWSE)
  // ---------------------------------------------------------------------------
  return (
    <Box sx={{ px: { xs: 2, sm: 3 }, pt: 2.5, pb: 3, bgcolor: 'background.default' }}>
      
      {/* 1. Breadcrumbs */}
      <Breadcrumbs aria-label="breadcrumb" sx={{ mb: 0.5 }}>
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
          mb: 2.5 
        }}
      >
        <Typography variant="h5" component="h1" sx={{ color: 'text.primary', fontWeight: 700, fontSize: '1.35rem' }}>
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
            startIcon={<Plus size={16} />}
            onClick={onAdicionarClick}
          >
            Novo
          </Button>
        </Stack>
      </Box>

      {/* 3. Banner Inline de Confirmação de Exclusão (SEM MODAL / SEM POPUP DIALOG) */}
      {modalMode === 'delete' && selectedRecord && (
        <Alert
          severity="error"
          variant="outlined"
          action={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <Button
                color="error"
                variant="contained"
                size="small"
                startIcon={<Trash2 size={16} />}
                onClick={handleDeleteConfirmWrapper}
                disabled={loading}
              >
                Confirmar Exclusão
              </Button>
              <Button
                color="inherit"
                variant="outlined"
                size="small"
                onClick={onModalClose}
                disabled={loading}
                sx={{ borderColor: 'divider' }}
              >
                Cancelar
              </Button>
            </Stack>
          }
          sx={{
            mb: 3,
            bgcolor: 'error.50',
            borderColor: 'error.main',
            alignItems: 'center',
            borderRadius: 1.5,
          }}
        >
          <Typography variant="subtitle2" sx={{ fontWeight: 700, color: 'error.main' }}>
            Confirmação de Exclusão
          </Typography>
          <Typography variant="body2" sx={{ color: 'text.primary', mt: 0.25 }}>
            Deseja realmente excluir o registro <strong>"{getRecordDisplayName(selectedRecord)}"</strong>? Esta ação não poderá ser desfeita.
          </Typography>
        </Alert>
      )}

      {/* 4. Chips de Filtros Aplicados */}
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

      {/* 5. Grade Principal de Dados */}
      <Card sx={{ p: 3, borderRadius: 1.5, boxShadow: 'rgba(145, 158, 171, 0.08) 0px 0px 2px 0px, rgba(145, 158, 171, 0.08) 0px 12px 24px -4px', position: 'relative' }}>
        {loading && (
          <LinearProgress 
            sx={{ 
              position: 'absolute', 
              top: 0, 
              left: 0, 
              right: 0, 
              borderTopLeftRadius: 'inherit', 
              borderTopRightRadius: 'inherit' 
            }} 
          />
        )}
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

      {/* 6. Gaveta de Filtros Responsiva */}
      <CrudFilterDrawer
        open={isFilterDrawerOpen}
        onClose={() => setIsFilterDrawerOpen(false)}
        filtros={activeFilters}
        filterValues={tempFilters}
        onFilterChange={handleFilterChange}
        onApplyFilters={handleApplyFilters}
        onClearFilters={handleClearFilters}
        loading={loading}
      />

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

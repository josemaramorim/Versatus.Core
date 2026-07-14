import React from 'react';
import { 
  Dialog, 
  DialogTitle, 
  DialogContent, 
  DialogActions, 
  Button, 
  IconButton,
  Typography,
  Box
} from '@mui/material';
import { X, Save, RotateCcw, Trash2 } from 'lucide-react';
import type { CadastroModalMode } from '../../types/cadastro';

export interface ICrudModalProps {
  open: boolean;
  mode: CadastroModalMode;
  titulo: string;
  onClose: () => void;
  onSave: () => void;
  onDeleteConfirm: () => void;
  onUndo?: () => void;
  children: React.ReactNode;
}

export const CrudModal: React.FC<ICrudModalProps> = ({
  open,
  mode,
  titulo,
  onClose,
  onSave,
  onDeleteConfirm,
  onUndo,
  children
}) => {
  const getModalTitle = () => {
    switch (mode) {
      case 'insert':
        return `Novo ${titulo}`;
      case 'edit':
        return `Alterar ${titulo}`;
      case 'delete':
        return `Excluir ${titulo}`;
      case 'view':
        return `Visualizar ${titulo}`;
      default:
        return titulo;
    }
  };

  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      maxWidth="lg" 
      fullWidth
      slotProps={{
        paper: {
          sx: { borderRadius: 2, overflow: 'hidden' }
        }
      }}
    >
      {/* Cabeçalho do Modal */}
      <DialogTitle 
        sx={{ 
          m: 0, 
          p: 2.5, 
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'center',
          bgcolor: 'background.default',
          borderBottom: '1px solid',
          borderColor: 'divider'
        }}
      >
        <Typography variant="h6" component="div" sx={{ fontWeight: 700 }}>
          {getModalTitle()}
        </Typography>
        <IconButton
          aria-label="close"
          onClick={onClose}
          sx={{
            color: 'text.secondary',
            '&:hover': { bgcolor: 'action.hover' }
          }}
          size="small"
        >
          <X size={18} />
        </IconButton>
      </DialogTitle>

      {/* Conteúdo do Modal */}
      <DialogContent dividers sx={{ p: 3, bgcolor: 'background.paper' }}>
        {children}
      </DialogContent>

      {/* Rodapé do Modal (Ações) */}
      <DialogActions sx={{ p: 2.5, bgcolor: 'background.default', gap: 1.5 }}>
        {(mode === 'insert' || mode === 'edit') && (
          <Box sx={{ display: 'flex', gap: 1.5, ml: 'auto' }}>
            <Button
              variant="contained"
              color="primary"
              startIcon={<Save size={16} />}
              onClick={onSave}
            >
              Salvar
            </Button>
            {onUndo && (
              <Button
                variant="outlined"
                color="inherit"
                startIcon={<RotateCcw size={16} />}
                onClick={onUndo}
                sx={{ borderColor: 'divider' }}
              >
                Desfazer
              </Button>
            )}
            <Button
              variant="outlined"
              color="inherit"
              onClick={onClose}
              sx={{ borderColor: 'divider' }}
            >
              Cancelar
            </Button>
          </Box>
        )}

        {mode === 'delete' && (
          <Box sx={{ display: 'flex', gap: 1.5, ml: 'auto' }}>
            <Button
              variant="contained"
              color="error"
              startIcon={<Trash2 size={16} />}
              onClick={onDeleteConfirm}
            >
              Confirmar Exclusão
            </Button>
            <Button
              variant="outlined"
              color="inherit"
              onClick={onClose}
              sx={{ borderColor: 'divider' }}
            >
              Cancelar
            </Button>
          </Box>
        )}
      </DialogActions>
    </Dialog>
  );
};

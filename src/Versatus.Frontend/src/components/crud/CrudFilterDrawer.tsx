import React from 'react';
import { 
  Drawer, 
  Box, 
  Typography, 
  IconButton, 
  TextField, 
  FormControl, 
  Select, 
  MenuItem, 
  Button, 
  Stack
} from '@mui/material';
import { X, SlidersHorizontal, Trash2, Search } from 'lucide-react';
import type { IFiltroConfig } from '../../types/cadastro';

export interface ICrudFilterDrawerProps {
  open: boolean;
  onClose: () => void;
  filtros: IFiltroConfig[];
  filterValues: Record<string, any>;
  onFilterChange: (field: string, value: any) => void;
  onApplyFilters: () => void;
  onClearFilters: () => void;
}

export const CrudFilterDrawer: React.FC<ICrudFilterDrawerProps> = ({
  open,
  onClose,
  filtros,
  filterValues,
  onFilterChange,
  onApplyFilters,
  onClearFilters
}) => {
  return (
    <Drawer 
      anchor="right" 
      open={open} 
      onClose={onClose}
      slotProps={{
        backdrop: {
          sx: { backdropFilter: 'blur(3px)' }
        },
        paper: {
          sx: { 
            width: { xs: '100%', sm: 380 }, 
            display: 'flex', 
            flexDirection: 'column', 
            height: '100%' 
          }
        }
      }}
    >
      {/* 1. Cabeçalho da Gaveta */}
      <Box 
        sx={{ 
          p: 2.5, 
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'center',
          bgcolor: 'background.default',
          borderBottom: '1px solid',
          borderColor: 'divider'
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.2 }}>
          <SlidersHorizontal size={18} />
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            Filtros de Busca
          </Typography>
        </Box>
        <IconButton onClick={onClose} size="small">
          <X size={18} />
        </IconButton>
      </Box>

      {/* 2. Conteúdo dos Filtros (Rolagem Vertical) */}
      <Box sx={{ flexGrow: 1, overflowY: 'auto', p: 3 }}>
        <Stack spacing={2.5}>
          {filtros.map((filtro, idx) => {
            const val = filterValues[filtro.field] ?? '';
            return (
              <Box key={idx}>
                <Typography variant="body2" sx={{ fontWeight: 600, mb: 1, color: 'text.secondary' }}>
                  {filtro.label}
                </Typography>
                
                {filtro.type === 'select' ? (
                  <FormControl fullWidth size="small">
                    <Select
                      value={val}
                      onChange={(e) => onFilterChange(filtro.field, e.target.value)}
                      displayEmpty
                    >
                      <MenuItem value="">
                        <em>Todos / Nenhum</em>
                      </MenuItem>
                      {filtro.options?.map((opt, oIdx) => (
                        <MenuItem key={oIdx} value={opt.value}>
                          {opt.label}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                ) : filtro.type === 'date' ? (
                  <TextField
                    fullWidth
                    size="small"
                    type="date"
                    value={val}
                    onChange={(e) => onFilterChange(filtro.field, e.target.value)}
                  />
                ) : (
                  <TextField
                    fullWidth
                    size="small"
                    value={val}
                    onChange={(e) => onFilterChange(filtro.field, e.target.value)}
                    placeholder={`Filtrar por ${filtro.label.toLowerCase()}`}
                  />
                )}
              </Box>
            );
          })}
        </Stack>
      </Box>

      {/* 3. Rodapé Fixo (Ações) */}
      <Box sx={{ p: 2.5, bgcolor: 'background.default', borderTop: '1px solid', borderColor: 'divider' }}>
        <Stack spacing={1.5}>
          <Button
            fullWidth
            variant="contained"
            color="primary"
            startIcon={<Search size={16} />}
            onClick={onApplyFilters}
          >
            Filtrar
          </Button>
          <Button
            fullWidth
            variant="outlined"
            color="inherit"
            startIcon={<Trash2 size={16} />}
            onClick={onClearFilters}
            sx={{ borderColor: 'divider' }}
          >
            Limpar Filtros
          </Button>
        </Stack>
      </Box>
    </Drawer>
  );
};

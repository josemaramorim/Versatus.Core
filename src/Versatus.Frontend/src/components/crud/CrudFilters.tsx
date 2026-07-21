import React from 'react';
import { 
  Box, 
  Grid, 
  TextField, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  Button, 
  Card,
  Tooltip
} from '@mui/material';
import { FilterX } from 'lucide-react';
import type { IFiltroConfig } from '../../types/cadastro';

export interface ICrudFiltersProps {
  filtros: IFiltroConfig[];
  filterValues: Record<string, any>;
  onFilterChange: (field: string, value: any) => void;
  onClearFilters: () => void;
}

export const CrudFilters: React.FC<ICrudFiltersProps> = ({
  filtros,
  filterValues,
  onFilterChange,
  onClearFilters
}) => {
  if (filtros.length === 0) return null;

  return (
    <Card variant="outlined" sx={{ p: 3, mb: 3, borderRadius: 1.5 }}>
      <Grid container spacing={2} sx={{ alignItems: 'center' }}>
        {filtros.map((filtro, idx) => {
          const val = filterValues[filtro.field] ?? '';
          return (
            <Grid key={idx} size={{ xs: 12, sm: 3 }}>
              {filtro.type === 'select' ? (
                <FormControl fullWidth size="small">
                  <InputLabel>{filtro.label}</InputLabel>
                  <Select
                    value={val}
                    onChange={(e) => onFilterChange(filtro.field, e.target.value)}
                    label={filtro.label}
                  >
                    <MenuItem value="">
                      <em>Todos</em>
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
                  label={filtro.label}
                  value={val}
                  onChange={(e) => onFilterChange(filtro.field, e.target.value)}
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              ) : (
                <TextField
                  fullWidth
                  size="small"
                  label={filtro.label}
                  value={val}
                  onChange={(e) => onFilterChange(filtro.field, e.target.value)}
                  placeholder={`Buscar por ${filtro.label.toLowerCase()}...`}
                />
              )}
            </Grid>
          );
        })}
        <Grid size={{ xs: 12, sm: 'auto' }} sx={{ ml: 'auto' }}>
          <Box sx={{ display: 'flex', gap: 1.5 }}>
            <Tooltip title="Limpar todos os filtros">
              <Button
                variant="outlined"
                color="inherit"
                size="small"
                startIcon={<FilterX size={16} />}
                onClick={onClearFilters}
                sx={{ borderColor: 'divider' }}
              >
                Limpar Filtros
              </Button>
            </Tooltip>
          </Box>
        </Grid>
      </Grid>
    </Card>
  );
};

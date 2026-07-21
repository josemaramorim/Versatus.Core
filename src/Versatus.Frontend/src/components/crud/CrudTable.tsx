import { 
  TableContainer, 
  Table, 
  TableHead, 
  TableRow, 
  TableCell, 
  TableBody, 
  Paper, 
  IconButton, 
  TableSortLabel, 
  Tooltip,
  Box
} from '@mui/material';
import { Edit, Trash2 } from 'lucide-react';
import type { IColunaConfig, ISortConfig } from '../../types/cadastro';

export interface ICrudTableProps<T> {
  records: T[];
  colunas: IColunaConfig<T>[];
  sortConfig: ISortConfig;
  handleSort: (column: string) => void;
  onEditarClick: (record: T) => void;
  onDeletarClick: (record: T) => void;
}

export function CrudTable<T>({
  records,
  colunas,
  sortConfig,
  handleSort,
  onEditarClick,
  onDeletarClick
}: ICrudTableProps<T>) {
  return (
    <TableContainer component={Paper} variant="outlined" sx={{ borderRadius: 1.5, overflow: 'hidden' }}>
      <Table size="medium">
        <TableHead sx={{ bgcolor: 'background.default' }}>
          <TableRow>
            {colunas.map((col, idx) => (
              <TableCell 
                key={idx} 
                style={{ width: col.width }}
                sortDirection={sortConfig.column === col.field ? sortConfig.direction : false}
                sx={{ py: 2, fontWeight: 600, color: 'text.primary' }}
              >
                {col.sortable ? (
                  <TableSortLabel
                    active={sortConfig.column === col.field}
                    direction={sortConfig.column === col.field ? sortConfig.direction : 'asc'}
                    onClick={() => handleSort(col.field as string)}
                  >
                    {col.header}
                  </TableSortLabel>
                ) : (
                  col.header
                )}
              </TableCell>
            ))}
            <TableCell align="center" style={{ width: 120 }} sx={{ py: 2, fontWeight: 600, color: 'text.primary' }}>
              Ações
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {records.length === 0 ? (
            <TableRow>
              <TableCell colSpan={colunas.length + 1} align="center" sx={{ py: 6, color: 'text.secondary' }}>
                Nenhum registro encontrado.
              </TableCell>
            </TableRow>
          ) : (
            records.map((row, rIdx) => (
              <TableRow 
                key={rIdx} 
                hover
                sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
              >
                {colunas.map((col, cIdx) => {
                  const cellValue = col.renderCell 
                    ? col.renderCell(row, rIdx) 
                    : (row as any)[col.field];
                  return (
                    <TableCell key={cIdx} sx={{ py: 1.5 }}>
                      {cellValue}
                    </TableCell>
                  );
                })}
                <TableCell align="center" sx={{ py: 1 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'center', gap: 1 }}>
                    <Tooltip title="Editar registro">
                      <IconButton 
                        color="primary" 
                        size="small" 
                        onClick={() => onEditarClick(row)}
                        sx={{ bgcolor: 'action.hover' }}
                      >
                        <Edit size={16} />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Excluir registro">
                      <IconButton 
                        color="error" 
                        size="small" 
                        onClick={() => onDeletarClick(row)}
                        sx={{ bgcolor: 'action.hover' }}
                      >
                        <Trash2 size={16} />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

import React from 'react';
import { 
  Box, 
  Typography, 
  Button, 
  TableContainer, 
  Table, 
  TableHead, 
  TableRow, 
  TableCell, 
  TableBody, 
  Paper, 
  IconButton 
} from '@mui/material';
import { Plus, Trash2 } from 'lucide-react';
import { useFormContext, useFieldArray } from 'react-hook-form';

export interface IColunaGrade {
  header: string;
  width?: string | number;
  renderCell: (index: number) => React.ReactNode;
}

export interface IGradeEdicaoProps {
  titulo: string;
  name: string; // Nome da propriedade array no react-hook-form (ex: 'enderecos')
  isBrowse: boolean;
  colunas: IColunaGrade[];
  defaultRow: Record<string, any>; // Valores iniciais de cada linha adicionada
  botaoAdicionarRotulo?: string;
}

export const GradeEdicao: React.FC<IGradeEdicaoProps> = ({
  titulo,
  name,
  isBrowse,
  colunas,
  defaultRow,
  botaoAdicionarRotulo = 'Adicionar'
}) => {
  const { control } = useFormContext();
  
  // hook-form useFieldArray interno que gerencia a grade dinamicamente
  const { fields, append, remove } = useFieldArray({
    control,
    name
  });

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2, width: '100%' }}>
        <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>{titulo}</Typography>
        <Button 
          size="small" 
          variant="outlined" 
          startIcon={<Plus size={16} />} 
          disabled={isBrowse} 
          onClick={() => append(defaultRow)}
        >
          {botaoAdicionarRotulo}
        </Button>
      </Box>
      <TableContainer component={Paper} variant="outlined" sx={{ borderRadius: 1 }}>
        <Table size="small">
          <TableHead sx={{ bgcolor: 'background.default' }}>
            <TableRow>
              {colunas.map((col, cIdx) => (
                <TableCell key={cIdx} style={{ width: col.width }} sx={{ py: 1.5 }}>
                  {col.header}
                </TableCell>
              ))}
              <TableCell align="center" style={{ width: 60 }} sx={{ py: 1.5 }}>Excluir</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {fields.length === 0 ? (
              <TableRow>
                <TableCell colSpan={colunas.length + 1} align="center" sx={{ py: 3, color: 'text.secondary' }}>
                  Nenhum item adicionado ainda.
                </TableCell>
              </TableRow>
            ) : (
              fields.map((row, idx) => (
                <TableRow key={row.id}>
                  {colunas.map((col, cIdx) => (
                    <TableCell key={cIdx} sx={{ py: 1 }}>
                      {col.renderCell(idx)}
                    </TableCell>
                  ))}
                  <TableCell align="center" sx={{ py: 1 }}>
                    <IconButton color="error" disabled={isBrowse} onClick={() => remove(idx)} size="small">
                      <Trash2 size={16} />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
};

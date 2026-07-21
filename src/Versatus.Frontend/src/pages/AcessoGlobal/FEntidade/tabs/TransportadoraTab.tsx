import React from 'react';
import { 
  Box, 
  Grid, 
  TextField, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  FormControlLabel, 
  Checkbox, 
  Switch,
  Divider 
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, ITabProps } from '../types';
import { PlacaMask } from '../../../../components/common/TextMasks';
import { GradeEdicao } from '../../../../components/common/GradeEdicao';

export const TransportadoraTab: React.FC<ITabProps> = ({
  isBrowse
}) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, sm: 4 }}>
          <Controller
            name="tAtivo"
            control={control}
            render={({ field }) => (
              <FormControlLabel
                control={
                  <Switch
                    checked={field.value ?? true}
                    onChange={(e) => field.onChange(e.target.checked)}
                    disabled={isBrowse}
                    color="primary"
                  />
                }
                label="Transportadora Ativa"
                sx={{ mt: 0.5 }}
              />
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <Controller
            name="idCategoriaTransportadora"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth size="small" disabled={isBrowse}>
                <InputLabel>Categoria Transportadora</InputLabel>
                <Select {...field} label="Categoria Transportadora">
                  <MenuItem value={1}>Geral</MenuItem>
                  <MenuItem value={2}>Cargas Perigosas</MenuItem>
                </Select>
              </FormControl>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <TextField 
            {...register('rntrc')}
            size="small" 
            fullWidth 
            label="RNTRC (Registro ANTT)" 
            disabled={isBrowse} 
            error={!!errors.rntrc}
            helperText={errors.rntrc?.message}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <Controller
            name="tipoProprietario"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth size="small" disabled={isBrowse}>
                <InputLabel>Tipo Proprietário</InputLabel>
                <Select {...field} label="Tipo Proprietário">
                  <MenuItem value={1}>TAC (Autônomo)</MenuItem>
                  <MenuItem value={2}>ETC (Empresa)</MenuItem>
                  <MenuItem value={3}>CTC (Cooperativa)</MenuItem>
                </Select>
              </FormControl>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <Controller
            name="tipoTransportador"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth size="small" disabled={isBrowse}>
                <InputLabel>Tipo Transportador</InputLabel>
                <Select {...field} label="Tipo Transportador">
                  <MenuItem value={1}>Próprio</MenuItem>
                  <MenuItem value={2}>Terceiro</MenuItem>
                </Select>
              </FormControl>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 4 }}>
          <Controller
            name="tipoFretePadrao"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth size="small" disabled={isBrowse}>
                <InputLabel>Tipo Frete Padrão</InputLabel>
                <Select {...field} label="Tipo Frete Padrão">
                  <MenuItem value="CIF">CIF</MenuItem>
                  <MenuItem value="FOB">FOB</MenuItem>
                </Select>
              </FormControl>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 3 }}>
          <Controller
            name="tAtivo"
            control={control}
            render={({ field: { value, onChange, ...rest } }) => (
              <FormControlLabel 
                control={
                  <Checkbox 
                    size="small" 
                    checked={!!value} 
                    disabled={isBrowse} 
                    onChange={(e) => onChange(e.target.checked)}
                    {...rest}
                  />
                } 
                label="Ativo" 
              />
            )}
          />
        </Grid>
      </Grid>

      <Divider sx={{ my: 3 }} />
      
      <GradeEdicao
        titulo="Frota / Veículos Associados"
        name="transpVeiculos"
        isBrowse={isBrowse}
        botaoAdicionarRotulo="Adicionar Veículo"
        defaultRow={{ placa: '', uf: 'SP', renavam: '', tara: 0, capacidade: 0, proprietario: '' }}
        colunas={[
          {
            header: 'Placa',
            width: 160,
            renderCell: (idx) => (
              <TextField 
                {...register(`transpVeiculos.${idx}.placa`)}
                size="small" 
                placeholder="ABC-1234" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.transpVeiculos?.[idx]?.placa}
                slotProps={{
                  input: {
                    inputComponent: PlacaMask as any
                  }
                }}
              />
            )
          },
          {
            header: 'UF',
            width: 70,
            renderCell: (idx) => (
              <TextField 
                {...register(`transpVeiculos.${idx}.uf`)}
                size="small" 
                disabled={isBrowse} 
                error={!!errors.transpVeiculos?.[idx]?.uf}
                slotProps={{ htmlInput: { maxLength: 2 } }} 
                onChange={(e) => {
                  e.target.value = e.target.value.toUpperCase();
                }}
              />
            )
          },
          {
            header: 'Renavam',
            renderCell: (idx) => (
              <TextField 
                {...register(`transpVeiculos.${idx}.renavam`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.transpVeiculos?.[idx]?.renavam}
              />
            )
          },
          {
            header: 'Tara (KG)',
            width: 130,
            renderCell: (idx) => (
              <TextField 
                {...register(`transpVeiculos.${idx}.tara`, { valueAsNumber: true })}
                size="small" 
                type="number" 
                fullWidth
                disabled={isBrowse} 
                error={!!errors.transpVeiculos?.[idx]?.tara}
              />
            )
          },
          {
            header: 'Capacidade (KG)',
            width: 130,
            renderCell: (idx) => (
              <TextField 
                {...register(`transpVeiculos.${idx}.capacidade`, { valueAsNumber: true })}
                size="small" 
                type="number" 
                fullWidth
                disabled={isBrowse} 
                error={!!errors.transpVeiculos?.[idx]?.capacidade}
              />
            )
          },
          {
            header: 'Proprietário',
            renderCell: (idx) => (
              <TextField 
                {...register(`transpVeiculos.${idx}.proprietario`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.transpVeiculos?.[idx]?.proprietario}
              />
            )
          }
        ]}
      />
    </Box>
  );
};


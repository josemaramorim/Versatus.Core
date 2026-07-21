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
  Tabs,
  Tab
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, IObraTabProps } from '../types';

export const ObraTab: React.FC<IObraTabProps> = ({
  isBrowse,
  activeObraTab,
  setActiveObraTab
}) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
        <Tabs 
          value={activeObraTab} 
          onChange={(_, val) => setActiveObraTab(val)} 
        >
          <Tab label="1. Geral" />
          <Tab label="2. Integração" />
        </Tabs>
      </Box>

      {/* Obra Subtab 1: Geral */}
      {activeObraTab === 0 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="obraIdCliente"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Cliente Proprietário da Obra</InputLabel>
                  <Select {...field} label="Cliente Proprietário da Obra">
                    <MenuItem value={1}>Cliente Geral 1</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('obraResponsavel')}
              size="small" 
              fullWidth 
              label="Nome do Engenheiro Responsável" 
              disabled={isBrowse} 
              error={!!errors.obraResponsavel}
              helperText={errors.obraResponsavel?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('oDataInicio')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Data Início" 
              disabled={isBrowse} 
              error={!!errors.oDataInicio}
              helperText={errors.oDataInicio?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('oPrevisaoInicio')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Previsão Início" 
              disabled={isBrowse} 
              error={!!errors.oPrevisaoInicio}
              helperText={errors.oPrevisaoInicio?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('oPrevisaoFim')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Previsão Fim" 
              disabled={isBrowse} 
              error={!!errors.oPrevisaoFim}
              helperText={errors.oPrevisaoFim?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('oDataFim')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Data Fim" 
              disabled={isBrowse} 
              error={!!errors.oDataFim}
              helperText={errors.oDataFim?.message}
            />
          </Grid>
          
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="obraSituacao"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Situação Obra</InputLabel>
                  <Select {...field} label="Situação Obra">
                    <MenuItem value="Ativa">Ativa</MenuItem>
                    <MenuItem value="Pausada">Pausada</MenuItem>
                    <MenuItem value="Concluída">Concluída</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('oDataSituacao')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Data Situação" 
              disabled={isBrowse} 
              error={!!errors.oDataSituacao}
              helperText={errors.oDataSituacao?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="obraTabelaPreco"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Tabela de Preço</InputLabel>
                  <Select {...field} label="Tabela de Preço">
                    <MenuItem value={1}>Tabela Padrão Obra</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="obraCategoria"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Categoria Obra</InputLabel>
                  <Select {...field} label="Categoria Obra">
                    <MenuItem value={1}>Residencial Privada</MenuItem>
                    <MenuItem value={2}>Predial Comercial</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="oAtivo"
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
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="oReservaEstoque"
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
                  label="Reserva Estoque Automático" 
                />
              )}
            />
          </Grid>
        </Grid>
      )}

      {/* Obra Subtab 2: Integração */}
      {activeObraTab === 1 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6 }}>
            <Controller
              name="obraIdCentroCusto"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Centro de Custo Obra</InputLabel>
                  <Select {...field} label="Centro de Custo Obra">
                    <MenuItem value={1}>Administrativo Obras</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <Controller
              name="obraIdProjeto"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Projeto Financeiro Obra</InputLabel>
                  <Select {...field} label="Projeto Financeiro Obra">
                    <MenuItem value={1}>Nenhum Projeto</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <Controller
              name="obraIdOperacaoAtendimento"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Operação de Atendimento</InputLabel>
                  <Select {...field} label="Operação de Atendimento">
                    <MenuItem value={1}>101 - Venda de Mercadoria</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <Controller
              name="obraIdOperacaoDevolucao"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Operação de Devolução</InputLabel>
                  <Select {...field} label="Operação de Devolução">
                    <MenuItem value={1}>201 - Devolução Obra</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
        </Grid>
      )}
    </Box>
  );
};


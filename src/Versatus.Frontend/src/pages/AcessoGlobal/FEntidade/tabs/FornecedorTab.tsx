import React from 'react';
import { 
  Box, 
  Tabs, 
  Tab, 
  Grid, 
  TextField, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  FormControlLabel, 
  Checkbox,
  Switch
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, IFornecedorTabProps } from '../types';
import { GradeEdicao } from '../../../../components/common/GradeEdicao';

export const FornecedorTab: React.FC<IFornecedorTabProps> = ({
  isBrowse,
  activeFornecedorTab,
  setActiveFornecedorTab
}) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
        <Tabs 
          value={activeFornecedorTab} 
          onChange={(_, val) => setActiveFornecedorTab(val)} 
        >
          <Tab label="1. Geral" />
          <Tab label="2. Contas Bancárias" />
          <Tab label="3. Filiais Vinculadas" />
        </Tabs>
      </Box>

      {/* Fornecedor Subtab 1: Geral */}
      {activeFornecedorTab === 0 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="fornAtivo"
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
                  label="Fornecedor Ativo"
                  sx={{ mt: 0.5 }}
                />
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idCategoriaFornecedor"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Categoria Fornecedor</InputLabel>
                  <Select {...field} label="Categoria Fornecedor">
                    <MenuItem value={1}>Matéria Prima</MenuItem>
                    <MenuItem value={2}>Serviços</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idCondicaoPagamentoFornecedor"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Condição Pagamento Padrão</InputLabel>
                  <Select {...field} label="Condição Pagamento Padrão">
                    <MenuItem value={1}>30 Dias Net</MenuItem>
                    <MenuItem value={2}>À Vista (Boleto)</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('fornNumAlternativo')}
              size="small" 
              fullWidth 
              label="Número Alternativo / Ramal" 
              disabled={isBrowse} 
              error={!!errors.fornNumAlternativo}
              helperText={errors.fornNumAlternativo?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idCentroCusto"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Centro de Custo Padrão</InputLabel>
                  <Select {...field} label="Centro de Custo Padrão">
                    <MenuItem value={1}>Administrativo</MenuItem>
                    <MenuItem value={2}>Produção Geral</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idProjeto"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Projeto Financeiro Padrão</InputLabel>
                  <Select {...field} label="Projeto Financeiro Padrão">
                    <MenuItem value={1}>Nenhum Projeto</MenuItem>
                    <MenuItem value={2}>Expansão Filial Sul</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="fornCotacaoProd"
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
                  label="Enviar cotação de produtos automático" 
                />
              )}
            />
          </Grid>
        </Grid>
      )}

      {/* Fornecedor Subtab 2: Contas Bancárias */}
      {activeFornecedorTab === 1 && (
        <GradeEdicao
          titulo="Contas Bancárias de Favorecidos"
          name="fornContas"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Conta"
          defaultRow={{ banco: '', agencia: '', conta: '', tipo: 'Corrente', favorecido: '', cpfCnpj: '' }}
          colunas={[
            {
              header: 'Banco',
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornContas.${idx}.banco`)}
                  size="small" 
                  fullWidth 
                  placeholder="Ex: Banco Itaú" 
                  disabled={isBrowse} 
                  error={!!errors.fornContas?.[idx]?.banco}
                />
              )
            },
            {
              header: 'Agência',
              width: 120,
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornContas.${idx}.agencia`)}
                  size="small" 
                  fullWidth
                  disabled={isBrowse} 
                  error={!!errors.fornContas?.[idx]?.agencia}
                />
              )
            },
            {
              header: 'Conta',
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornContas.${idx}.conta`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.fornContas?.[idx]?.conta}
                />
              )
            },
            {
              header: 'Tipo',
              width: 150,
              renderCell: (idx) => (
                <Controller
                  name={`fornContas.${idx}.tipo`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Corrente">Corrente</MenuItem>
                        <MenuItem value="Poupança">Poupança</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Favorecido',
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornContas.${idx}.favorecido`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.fornContas?.[idx]?.favorecido}
                />
              )
            },
            {
              header: 'CPF/CNPJ Favorecido',
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornContas.${idx}.cpfCnpj`)}
                  size="small" 
                  fullWidth 
                  placeholder="Apenas números" 
                  disabled={isBrowse} 
                  error={!!errors.fornContas?.[idx]?.cpfCnpj}
                  helperText={errors.fornContas?.[idx]?.cpfCnpj?.message}
                />
              )
            }
          ]}
        />
      )}

      {/* Fornecedor Subtab 3: Filiais Vinculadas */}
      {activeFornecedorTab === 2 && (
        <GradeEdicao
          titulo="Filiais Habilitadas para Compra"
          name="fornFiliais"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Vincular Filial"
          defaultRow={{ codigoFilial: '', nomeFilial: '', ativo: true }}
          colunas={[
            {
              header: 'Código Filial',
              width: 180,
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornFiliais.${idx}.codigoFilial`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.fornFiliais?.[idx]?.codigoFilial}
                />
              )
            },
            {
              header: 'Nome da Filial',
              renderCell: (idx) => (
                <TextField 
                  {...register(`fornFiliais.${idx}.nomeFilial`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.fornFiliais?.[idx]?.nomeFilial}
                />
              )
            },
            {
              header: 'Ativo?',
              width: 100,
              renderCell: (idx) => (
                <Controller
                  name={`fornFiliais.${idx}.ativo`}
                  control={control}
                  render={({ field: { value, onChange } }) => (
                    <Checkbox 
                      checked={!!value} 
                      disabled={isBrowse} 
                      onChange={(e) => onChange(e.target.checked)} 
                    />
                  )}
                />
              )
            }
          ]}
        />
      )}
    </Box>
  );
};


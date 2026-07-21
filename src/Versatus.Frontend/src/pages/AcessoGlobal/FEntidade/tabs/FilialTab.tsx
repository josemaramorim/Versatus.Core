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
  Switch,
  Stack 
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, IFilialTabProps } from '../types';
import { GradeEdicao } from '../../../../components/common/GradeEdicao';

export const FilialTab: React.FC<IFilialTabProps> = ({
  isBrowse,
  activeFilialTab,
  setActiveFilialTab
}) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
        <Tabs 
          value={activeFilialTab} 
          onChange={(_, val) => setActiveFilialTab(val)} 
        >
          <Tab label="1. Geral" />
          <Tab label="2. Parâmetros Fiscais" />
          <Tab label="3. Autorizados XML" />
        </Tabs>
      </Box>

      {/* Filial Subtab 1: Geral */}
      {activeFilialTab === 0 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="flAtivo"
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
                  label="Filial Ativa"
                  sx={{ mt: 0.5 }}
                />
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('filialCodigoEmpresa')}
              size="small" 
              fullWidth 
              label="Código da Empresa ERP" 
              disabled={isBrowse} 
              error={!!errors.filialCodigoEmpresa}
              helperText={errors.filialCodigoEmpresa?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialRegimeIss"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Regime ISS</InputLabel>
                  <Select {...field} label="Regime ISS">
                    <MenuItem value={1}>Microempresa Municipal</MenuItem>
                    <MenuItem value={2}>Estimativa</MenuItem>
                    <MenuItem value={3}>Sociedade de Profissionais</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialCnaeServicoFiscal"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>CNAE do Serviço Fiscal</InputLabel>
                  <Select {...field} label="CNAE do Serviço Fiscal">
                    <MenuItem value={1}>6201-5/01 (Desenvolvimento)</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('filialContador')}
              size="small" 
              fullWidth 
              label="Nome do Contador Responsável" 
              disabled={isBrowse} 
              error={!!errors.filialContador}
              helperText={errors.filialContador?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialSpedPerfil"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Perfil SPED</InputLabel>
                  <Select {...field} label="Perfil SPED">
                    <MenuItem value="A">Perfil A (Mais detalhado)</MenuItem>
                    <MenuItem value="B">Perfil B</MenuItem>
                    <MenuItem value="C">Perfil C</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialIndicadorAtividade"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Indicador Atividade SPED</InputLabel>
                  <Select {...field} label="Indicador Atividade SPED">
                    <MenuItem value={0}>Industrial ou Equiparado</MenuItem>
                    <MenuItem value={1}>Outros</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialClassificacaoIndustrial"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Classificação Industrial</InputLabel>
                  <Select {...field} label="Classificação Industrial">
                    <MenuItem value={1}>Transformação</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialCarteiraDigital"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Carteira Digital NFe</InputLabel>
                  <Select {...field} label="Carteira Digital NFe">
                    <MenuItem value={1}>Carteira Integrada</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="filialCodigoServicoFiscal"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Código Serviço Fiscal ISS</InputLabel>
                  <Select {...field} label="Código Serviço Fiscal ISS">
                    <MenuItem value={1}>1.01 - Análise e Des.</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="flAtivo"
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
              name="filialIncentivoFiscal"
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
                  label="Incentivo Fiscal" 
                />
              )}
            />
          </Grid>
        </Grid>
      )}

      {/* Filial Subtab 2: Parâmetros Fiscais */}
      {activeFilialTab === 1 && (
        <Box>
          <Stack spacing={2} sx={{ mb: 3 }}>
            <Controller
              name="filialUsaSituacaoTributariaFornecedor"
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
                  label="CST/CSOSN compra pelo fornecedor" 
                />
              )}
            />
            <Controller
              name="filialEnviarInventarioSt"
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
                  label="Enviar inventário substituição" 
                />
              )}
            />
          </Stack>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField 
                {...register('filialCodigoAtividadeCp')}
                size="small" 
                fullWidth 
                label="Código Atividade CP" 
                disabled={isBrowse} 
                error={!!errors.filialCodigoAtividadeCp}
                helperText={errors.filialCodigoAtividadeCp?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <TextField 
                {...register('filialCodigoReceitaCp')}
                size="small" 
                fullWidth 
                label="Código Receita CP" 
                disabled={isBrowse} 
                error={!!errors.filialCodigoReceitaCp}
                helperText={errors.filialCodigoReceitaCp?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Controller
                name="filialTipoEmitenteMdfe"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Tipo Emitente MDF-e</InputLabel>
                    <Select {...field} label="Tipo Emitente MDF-e">
                      <MenuItem value={1}>Prestador de Serviço</MenuItem>
                      <MenuItem value={2}>Não prestador</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
          </Grid>
        </Box>
      )}

      {/* Filial Subtab 3: Autorizados XML */}
      {activeFilialTab === 2 && (
        <GradeEdicao
          titulo="CPF/CNPJ Autorizado download XML NF-e"
          name="filialAutorizadosXml"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Autorizar Documento"
          defaultRow={{ cpfCnpj: '' }}
          colunas={[
            {
              header: 'CPF / CNPJ Autorizado',
              renderCell: (idx) => (
                <TextField 
                  {...register(`filialAutorizadosXml.${idx}.cpfCnpj`)}
                  size="small" 
                  placeholder="Apenas números" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.filialAutorizadosXml?.[idx]?.cpfCnpj}
                  helperText={errors.filialAutorizadosXml?.[idx]?.cpfCnpj?.message}
                />
              )
            }
          ]}
        />
      )}
    </Box>
  );
};


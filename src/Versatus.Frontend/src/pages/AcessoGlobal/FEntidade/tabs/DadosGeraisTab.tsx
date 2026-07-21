import React from 'react';
import { 
  Box, 
  Grid, 
  TextField, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  Checkbox, 
  FormControlLabel, 
  Card, 
  Typography,
  FormHelperText
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, ITabProps } from '../types';
import { CPFMask, CNPJMask } from '../../../../components/common/TextMasks';

export const DadosGeraisTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, watch, formState: { errors } } = useFormContext<IEntidadeForm>();

  // Observar em tempo real os campos que controlam visualização condicional
  const tipoPessoa = watch('tipoPessoa');
  const fisicaTipoJuridica = watch('fisicaTipoJuridica');

  return (
    <Box>
      {tipoPessoa === 2 ? (
        // Pessoa Física
        <Box>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('cpf')}
                fullWidth 
                required
                label="CPF" 
                disabled={isBrowse} 
                error={!!errors.cpf}
                helperText={errors.cpf?.message}
                slotProps={{
                  input: {
                    inputComponent: CPFMask as any
                  }
                }}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('rg')}
                fullWidth 
                label="RG" 
                disabled={isBrowse} 
                error={!!errors.rg}
                helperText={errors.rg?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <TextField 
                {...register('rgEmissor')}
                fullWidth 
                label="Emissor" 
                disabled={isBrowse} 
                error={!!errors.rgEmissor}
                helperText={errors.rgEmissor?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <TextField 
                {...register('rgEmissao')}
                type="date" 
                slotProps={{ inputLabel: { shrink: true } }} 
                fullWidth 
                label="Emissão RG" 
                disabled={isBrowse} 
                error={!!errors.rgEmissao}
                helperText={errors.rgEmissao?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <TextField 
                {...register('dataNascimento')}
                type="date" 
                slotProps={{ inputLabel: { shrink: true } }} 
                fullWidth 
                label="Nascimento" 
                disabled={isBrowse} 
                error={!!errors.dataNascimento}
                helperText={errors.dataNascimento?.message}
              />
            </Grid>
            
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="sexo"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth disabled={isBrowse} error={!!errors.sexo}>
                    <InputLabel>Sexo</InputLabel>
                    <Select 
                      {...field}
                      label="Sexo" 
                    >
                      <MenuItem value="M">Masculino</MenuItem>
                      <MenuItem value="F">Feminino</MenuItem>
                      <MenuItem value="O">Outro</MenuItem>
                    </Select>
                    {errors.sexo && (
                      <FormHelperText>{errors.sexo.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="estadoCivil"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth disabled={isBrowse} error={!!errors.estadoCivil}>
                    <InputLabel>Estado Civil</InputLabel>
                    <Select 
                      {...field}
                      label="Estado Civil" 
                    >
                      <MenuItem value="Solteiro">Solteiro(a)</MenuItem>
                      <MenuItem value="Casado">Casado(a)</MenuItem>
                      <MenuItem value="Divorciado">Divorciado(a)</MenuItem>
                      <MenuItem value="Viuvo">Viúvo(a)</MenuItem>
                    </Select>
                    {errors.estadoCivil && (
                      <FormHelperText>{errors.estadoCivil.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('cidadeNascimento')}
                fullWidth 
                label="Cidade de Nascimento" 
                disabled={isBrowse} 
                error={!!errors.cidadeNascimento}
                helperText={errors.cidadeNascimento?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="grauInstrucao"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth disabled={isBrowse} error={!!errors.grauInstrucao}>
                    <InputLabel>Grau Instrução</InputLabel>
                    <Select 
                      {...field}
                      label="Grau Instrução" 
                    >
                      <MenuItem value="Fundamental">Ensino Fundamental</MenuItem>
                      <MenuItem value="Medio">Ensino Médio</MenuItem>
                      <MenuItem value="Superior">Ensino Superior</MenuItem>
                      <MenuItem value="Pos">Pós-Graduação</MenuItem>
                    </Select>
                    {errors.grauInstrucao && (
                      <FormHelperText>{errors.grauInstrucao.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Controller
                name="fisicaTipoJuridica"
                control={control}
                render={({ field: { value, onChange, ...rest } }) => (
                  <FormControlLabel
                    control={
                      <Checkbox 
                        checked={!!value} 
                        disabled={isBrowse}
                        onChange={(e) => onChange(e.target.checked)} 
                        {...rest}
                      />
                    }
                    label="Pessoa física com característica jurídica"
                  />
                )}
              />
            </Grid>
          </Grid>

          {fisicaTipoJuridica && (
            <Card variant="outlined" sx={{ mt: 3, border: '1px dashed orange', bgcolor: 'warning.light', p: 3 }}>
              <Typography variant="subtitle2" color="warning.contrastText" sx={{ mb: 2, fontWeight: 700 }}>
                Características Fiscais / Tributárias (Pessoa Física com Caract. Jurídica)
              </Typography>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 3 }}>
                  <TextField 
                    {...register('inscricaoEstadual')}
                    size="small" 
                    fullWidth 
                    label="Inscrição Estadual" 
                    disabled={isBrowse} 
                    error={!!errors.inscricaoEstadual}
                    helperText={errors.inscricaoEstadual?.message}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 3 }}>
                  <TextField 
                    {...register('inscricaoMunicipal')}
                    size="small" 
                    fullWidth 
                    label="Inscrição Municipal" 
                    disabled={isBrowse} 
                    error={!!errors.inscricaoMunicipal}
                    helperText={errors.inscricaoMunicipal?.message}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 3 }}>
                  <TextField 
                    {...register('inscricaoSuframa')}
                    size="small" 
                    fullWidth 
                    label="Inscrição SUFRAMA" 
                    disabled={isBrowse} 
                    error={!!errors.inscricaoSuframa}
                    helperText={errors.inscricaoSuframa?.message}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 3 }}>
                  <TextField 
                    {...register('inscricaoRural')}
                    size="small" 
                    fullWidth 
                    label="Inscrição Rural" 
                    disabled={isBrowse} 
                    error={!!errors.inscricaoRural}
                    helperText={errors.inscricaoRural?.message}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="contribuinteIcms"
                    control={control}
                    render={({ field }) => (
                      <FormControl fullWidth size="small" required disabled={isBrowse} error={!!errors.contribuinteIcms}>
                        <InputLabel>Contribuinte ICMS</InputLabel>
                        <Select 
                          {...field}
                          label="Contribuinte ICMS" 
                        >
                          <MenuItem value="Sim">Sim</MenuItem>
                          <MenuItem value="Nao">Não</MenuItem>
                          <MenuItem value="Isento">Isento</MenuItem>
                        </Select>
                        {errors.contribuinteIcms && (
                          <FormHelperText>{errors.contribuinteIcms.message}</FormHelperText>
                        )}
                      </FormControl>
                    )}
                  />
                </Grid>
              </Grid>
            </Card>
          )}
        </Box>
      ) : (
        // Pessoa Jurídica
        <Box>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('cnpj')}
                fullWidth 
                required
                label="CNPJ" 
                disabled={isBrowse} 
                error={!!errors.cnpj}
                helperText={errors.cnpj?.message}
                slotProps={{
                  input: {
                    inputComponent: CNPJMask as any
                  }
                }}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('inscricaoEstadual')}
                fullWidth 
                label="Inscrição Estadual" 
                disabled={isBrowse} 
                error={!!errors.inscricaoEstadual}
                helperText={errors.inscricaoEstadual?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('inscricaoMunicipal')}
                fullWidth 
                label="Inscrição Municipal" 
                disabled={isBrowse} 
                error={!!errors.inscricaoMunicipal}
                helperText={errors.inscricaoMunicipal?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('inscricaoSuframa')}
                fullWidth 
                label="Inscrição SUFRAMA" 
                disabled={isBrowse} 
                error={!!errors.inscricaoSuframa}
                helperText={errors.inscricaoSuframa?.message}
              />
            </Grid>
            
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="contribuinteIcms"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth required disabled={isBrowse} error={!!errors.contribuinteIcms}>
                    <InputLabel>Contribuinte ICMS</InputLabel>
                    <Select 
                      {...field}
                      label="Contribuinte ICMS" 
                    >
                      <MenuItem value="Sim">Sim</MenuItem>
                      <MenuItem value="Nao">Não</MenuItem>
                      <MenuItem value="Isento">Isento</MenuItem>
                    </Select>
                    {errors.contribuinteIcms && (
                      <FormHelperText>{errors.contribuinteIcms.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="regimeTributario"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth required disabled={isBrowse} error={!!errors.regimeTributario}>
                    <InputLabel>Regime Tributário</InputLabel>
                    <Select 
                      {...field}
                      label="Regime Tributário" 
                    >
                      <MenuItem value={1}>Simples Nacional</MenuItem>
                      <MenuItem value={2}>Lucro Presumido</MenuItem>
                      <MenuItem value={3}>Lucro Real</MenuItem>
                    </Select>
                    {errors.regimeTributario && (
                      <FormHelperText>{errors.regimeTributario.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="naturezaJuridica"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth required disabled={isBrowse} error={!!errors.naturezaJuridica}>
                    <InputLabel>Natureza Jurídica</InputLabel>
                    <Select 
                      {...field}
                      label="Natureza Jurídica" 
                    >
                      <MenuItem value={1}>Sociedade Limitada</MenuItem>
                      <MenuItem value={2}>Empresário Individual</MenuItem>
                    </Select>
                    {errors.naturezaJuridica && (
                      <FormHelperText>{errors.naturezaJuridica.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="enquadramento"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth required disabled={isBrowse} error={!!errors.enquadramento}>
                    <InputLabel>Enquadramento</InputLabel>
                    <Select 
                      {...field}
                      label="Enquadramento" 
                    >
                      <MenuItem value="ME">ME</MenuItem>
                      <MenuItem value="EPP">EPP</MenuItem>
                      <MenuItem value="Demais">Demais</MenuItem>
                    </Select>
                    {errors.enquadramento && (
                      <FormHelperText>{errors.enquadramento.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('inscricaoRural')}
                fullWidth 
                label="Inscrição Rural" 
                disabled={isBrowse} 
                error={!!errors.inscricaoRural}
                helperText={errors.inscricaoRural?.message}
              />
            </Grid>
          </Grid>
        </Box>
      )}
    </Box>
  );
};


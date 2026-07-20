import React, { useEffect } from 'react';
import { 
  Box, 
  Grid, 
  TextField, 
  FormControl, 
  InputLabel, 
  Select, 
  MenuItem, 
  FormHelperText
} from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { CadastroBasePage } from '../../components/crud/CadastroBasePage';
import { ParametroCadastroConfig } from './ParametroCadastroConfig';
import type { CadastroModalMode } from '../../types/cadastro';
import type { IParametroForm } from './types';
import { parametroSchema } from './schema';

export interface IParametroFormViewProps {
  mode: CadastroModalMode;
  record: IParametroForm;
  onSave: (data: IParametroForm) => void;
}

export const ParametroFormView: React.FC<IParametroFormViewProps> = ({
  mode,
  record,
  onSave
}) => {
  const isBrowse = mode === 'delete' || mode === 'view';
  const isEdit = mode === 'edit';

  const { handleSubmit, reset, control, formState: { errors } } = useForm<IParametroForm>({
    resolver: zodResolver(parametroSchema) as any,
    defaultValues: record,
    mode: 'onChange',
  });

  useEffect(() => {
    reset(record);
  }, [record, reset]);

  return (
    <form onSubmit={handleSubmit(onSave)}>
      <Box sx={{ p: 1 }}>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 8 }}>
            <Controller
              name="chave"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Chave (Nome)"
                  fullWidth
                  disabled={isBrowse || isEdit}
                  error={!!errors.chave}
                  helperText={errors.chave?.message}
                  variant="outlined"
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 4 }}>
            <FormControl fullWidth error={!!errors.tipo}>
              <InputLabel id="tipo-label">Tipo do Parâmetro</InputLabel>
              <Controller
                name="tipo"
                control={control}
                render={({ field }) => (
                  <Select
                    {...field}
                    labelId="tipo-label"
                    label="Tipo do Parâmetro"
                    disabled={isBrowse}
                  >
                    <MenuItem value={153}>Int (Inteiro)</MenuItem>
                    <MenuItem value={154}>Numeric (Decimal)</MenuItem>
                    <MenuItem value={155}>String (Texto)</MenuItem>
                    <MenuItem value={156}>Smallint (Booleano)</MenuItem>
                    <MenuItem value={157}>DateTime (Data)</MenuItem>
                    <MenuItem value={233}>Lookup (Busca)</MenuItem>
                    <MenuItem value={234}>Enumerado (Enum)</MenuItem>
                    <MenuItem value={374}>Automatico (Auto)</MenuItem>
                    <MenuItem value={1325}>LookupMulti</MenuItem>
                  </Select>
                )}
              />
              {errors.tipo && <FormHelperText>{errors.tipo.message}</FormHelperText>}
            </FormControl>
          </Grid>

          <Grid size={{ xs: 12 }}>
            <Controller
              name="descricao"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Descrição"
                  fullWidth
                  multiline
                  rows={2}
                  disabled={isBrowse}
                  error={!!errors.descricao}
                  helperText={errors.descricao?.message}
                  variant="outlined"
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12 }}>
            <Controller
              name="valor"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Valor Configurado"
                  fullWidth
                  multiline
                  rows={3}
                  disabled={isBrowse}
                  error={!!errors.valor}
                  helperText={errors.valor?.message}
                  variant="outlined"
                />
              )}
            />
          </Grid>
        </Grid>
      </Box>

      {/* Botões ocultos necessários para acionamento pelo CadastroBasePage */}
      <button id="crud-submit-btn" type="submit" style={{ display: 'none' }} />
      <button id="crud-reset-btn" type="button" style={{ display: 'none' }} onClick={() => reset()} />
    </form>
  );
};

// Componente de página principal para o cadastro de Parâmetros
export const FParametro: React.FC = () => {
  return (
    <CadastroBasePage<IParametroForm>
      config={new ParametroCadastroConfig()}
      initialRecords={[]}
      renderForm={(mode, record, onSave) => (
        <ParametroFormView mode={mode} record={record} onSave={onSave} />
      )}
    />
  );
};

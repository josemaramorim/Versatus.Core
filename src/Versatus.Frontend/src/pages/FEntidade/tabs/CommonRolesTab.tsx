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
  Divider 
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, ITabProps } from '../types';
import { CPFMask } from '../../../components/common/TextMasks';
import { GradeEdicao } from '../../../components/common/GradeEdicao';

// CONTADOR TAB
export const ContadorTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('contadorCpf')}
          size="small" 
          fullWidth 
          label="Contador CPF" 
          disabled={isBrowse} 
          error={!!errors.contadorCpf}
          helperText={errors.contadorCpf?.message}
          slotProps={{
            input: {
              inputComponent: CPFMask as any
            }
          }}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('contadorNome')}
          size="small" 
          fullWidth 
          label="Nome do Contador" 
          disabled={isBrowse} 
          error={!!errors.contadorNome}
          helperText={errors.contadorNome?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('contadorCrc')}
          size="small" 
          fullWidth 
          label="Registro CRC" 
          disabled={isBrowse} 
          error={!!errors.contadorCrc}
          helperText={errors.contadorCrc?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="coAtivo"
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
  );
};

// REPRESENTANTE TAB
export const RepresentanteTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, sm: 6 }}>
          <Controller
            name="idCategoriaRepresentante"
            control={control}
            render={({ field }) => (
              <FormControl fullWidth size="small" disabled={isBrowse}>
                <InputLabel>Categoria</InputLabel>
                <Select {...field} label="Categoria">
                  <MenuItem value={1}>Vendas Externas</MenuItem>
                </Select>
              </FormControl>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <Controller
            name="rAtivo"
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
        titulo="Clientes Vinculados"
        name="represClientes"
        isBrowse={isBrowse}
        botaoAdicionarRotulo="Vincular Cliente"
        defaultRow={{ codigoCliente: '', nomeCliente: '', regiao: '' }}
        colunas={[
          {
            header: 'Código Cliente',
            width: 180,
            renderCell: (idx) => (
              <TextField 
                {...register(`represClientes.${idx}.codigoCliente`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.represClientes?.[idx]?.codigoCliente}
              />
            )
          },
          {
            header: 'Nome Cliente',
            renderCell: (idx) => (
              <TextField 
                {...register(`represClientes.${idx}.nomeCliente`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.represClientes?.[idx]?.nomeCliente}
              />
            )
          },
          {
            header: 'Região',
            width: 250,
            renderCell: (idx) => (
              <TextField 
                {...register(`represClientes.${idx}.regiao`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.represClientes?.[idx]?.regiao}
              />
            )
          }
        ]}
      />
    </Box>
  );
};

// COMISSIONADO TAB
export const ComissionadoTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('comissaoNomeComercial')}
          size="small" 
          fullWidth 
          label="Nome Comercial" 
          disabled={isBrowse} 
          error={!!errors.comissaoNomeComercial}
          helperText={errors.comissaoNomeComercial?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('comissaoPercentual', { valueAsNumber: true })}
          size="small" 
          type="number" 
          fullWidth 
          label="Percentual (%)" 
          disabled={isBrowse} 
          error={!!errors.comissaoPercentual}
          helperText={errors.comissaoPercentual?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="comissaoTipo"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Tipo Comissão</InputLabel>
              <Select {...field} label="Tipo Comissão">
                <MenuItem value="Percentual">Percentual</MenuItem>
                <MenuItem value="Fixo">Fixo</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('tipoComissionado')}
          size="small" 
          fullWidth 
          label="Tipo Comissionado" 
          disabled={isBrowse} 
          error={!!errors.tipoComissionado}
          helperText={errors.tipoComissionado?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="idCategoriaComissionado"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Categoria</InputLabel>
              <Select {...field} label="Categoria">
                <MenuItem value={1}>Revendedor Autônomo</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="idUsuarioVinculado"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Usuário do Sistema</InputLabel>
              <Select {...field} label="Usuário do Sistema">
                <MenuItem value={1}>Vendedor Geral</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="cmAtivo"
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
  );
};

// AGENCIA BANCARIA TAB
export const AgenciaTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="agenciaBanco"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Banco correspondente</InputLabel>
              <Select {...field} label="Banco correspondente">
                <MenuItem value={1}>001 - Banco do Brasil</MenuItem>
                <MenuItem value={254}>254 - Itaú Unibanco</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('agenciaNome')}
          size="small" 
          fullWidth 
          label="Nome da Agência" 
          disabled={isBrowse} 
          error={!!errors.agenciaNome}
          helperText={errors.agenciaNome?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('agenciaNumero')}
          size="small" 
          fullWidth 
          label="Número Agência" 
          disabled={isBrowse} 
          error={!!errors.agenciaNumero}
          helperText={errors.agenciaNumero?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="aAtivo"
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
  );
};

// FINANCEIRA TAB
export const FinanceiraTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('financeiraNomeResumido')}
          size="small" 
          fullWidth 
          label="Nome Resumido" 
          disabled={isBrowse} 
          error={!!errors.financeiraNomeResumido}
          helperText={errors.financeiraNomeResumido?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="idCategoriaFinanceira"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Categoria Financeira</InputLabel>
              <Select {...field} label="Categoria Financeira">
                <MenuItem value={1}>Factoring / CDC</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('financeiraContaCorrente')}
          size="small" 
          fullWidth 
          label="Conta Corrente Vinculada" 
          disabled={isBrowse} 
          error={!!errors.financeiraContaCorrente}
          helperText={errors.financeiraContaCorrente?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('financeiraTaxaExtra', { valueAsNumber: true })}
          size="small" 
          type="number" 
          fullWidth 
          label="Taxa Extra (%)" 
          disabled={isBrowse} 
          error={!!errors.financeiraTaxaExtra}
          helperText={errors.financeiraTaxaExtra?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('financeiraDiaVencimento', { valueAsNumber: true })}
          size="small" 
          type="number" 
          fullWidth 
          label="Dia de Vencimento" 
          disabled={isBrowse} 
          error={!!errors.financeiraDiaVencimento}
          helperText={errors.financeiraDiaVencimento?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="fnAtivo"
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
  );
};

// OUTRO TAB
export const OutroTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { control } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 6 }}>
        <Controller
          name="idCategoriaOutro"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Categoria Outros</InputLabel>
              <Select {...field} label="Categoria Outros">
                <MenuItem value={1}>Parceiro Estratégico</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <Controller
          name="ouAtivo"
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
  );
};

// PROSPECTO TAB
export const ProspectoTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { control } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 6 }}>
        <Controller
          name="idCategoriaProspecto"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Categoria Prospecto</InputLabel>
              <Select {...field} label="Categoria Prospecto">
                <MenuItem value={1}>Lead Qualificado</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 6 }}>
        <Controller
          name="prAtivo"
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
  );
};

// ALUNO TAB
export const AlunoTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="alTipoResponsavel"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Tipo Responsável</InputLabel>
              <Select {...field} label="Tipo Responsável">
                <MenuItem value={1}>Próprio Aluno</MenuItem>
                <MenuItem value={2}>Pai / Mãe / Tutor</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="alIdClienteResponsavel"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Responsável Financeiro</InputLabel>
              <Select {...field} label="Responsável Financeiro">
                <MenuItem value={1}>Aluno Principal</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('alDiaPreferenicaPagamento', { valueAsNumber: true })}
          size="small" 
          type="number" 
          fullWidth 
          label="Dia Preferência Pagamento" 
          disabled={isBrowse} 
          error={!!errors.alDiaPreferenicaPagamento}
          helperText={errors.alDiaPreferenicaPagamento?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('alDataCadastro')}
          type="date" 
          slotProps={{ inputLabel: { shrink: true } }} 
          size="small" 
          fullWidth 
          label="Data Cadastro" 
          disabled={isBrowse} 
          error={!!errors.alDataCadastro}
          helperText={errors.alDataCadastro?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="alAtivo"
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
  );
};

// PROFESSOR TAB
export const ProfessorTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField 
            {...register('pfDataCadastro')}
            type="date" 
            slotProps={{ inputLabel: { shrink: true } }} 
            size="small" 
            fullWidth 
            label="Data Cadastro" 
            disabled={isBrowse} 
            error={!!errors.pfDataCadastro}
            helperText={errors.pfDataCadastro?.message}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6 }}>
          <Controller
            name="pfAtivo"
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
        titulo="Filiais Vinculadas"
        name="filiaisVinculadas"
        isBrowse={isBrowse}
        botaoAdicionarRotulo="Vincular Filial"
        defaultRow={{ codigoFilial: '', filialNome: '' }}
        colunas={[
          {
            header: 'Código Filial',
            width: 180,
            renderCell: (idx) => (
              <TextField 
                {...register(`filiaisVinculadas.${idx}.codigoFilial`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.filiaisVinculadas?.[idx]?.codigoFilial}
              />
            )
          },
          {
            header: 'Nome da Filial',
            renderCell: (idx) => (
              <TextField 
                {...register(`filiaisVinculadas.${idx}.filialNome`)}
                size="small" 
                fullWidth 
                disabled={isBrowse} 
                error={!!errors.filiaisVinculadas?.[idx]?.filialNome}
              />
            )
          }
        ]}
      />
    </Box>
  );
};

// INTERMEDIADOR TAB
export const IntermediadorTab: React.FC<ITabProps> = ({ isBrowse }) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();
  return (
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 4 }}>
        <TextField 
          {...register('iiIdentificacaoIntermediador')}
          size="small" 
          fullWidth 
          label="Identificação Intermediador (ID/Chave)" 
          disabled={isBrowse} 
          error={!!errors.iiIdentificacaoIntermediador}
          helperText={errors.iiIdentificacaoIntermediador?.message}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="idCategoriaIntermediador"
          control={control}
          render={({ field }) => (
            <FormControl fullWidth size="small" disabled={isBrowse}>
              <InputLabel>Categoria Intermediador</InputLabel>
              <Select {...field} label="Categoria Intermediador">
                <MenuItem value={1}>Marketplace Geral</MenuItem>
                <MenuItem value={2}>Plataforma de E-commerce</MenuItem>
              </Select>
            </FormControl>
          )}
        />
      </Grid>
      <Grid size={{ xs: 12, sm: 4 }}>
        <Controller
          name="icAtivo"
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
  );
};

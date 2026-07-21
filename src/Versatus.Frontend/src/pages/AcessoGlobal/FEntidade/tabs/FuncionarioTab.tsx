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
  Switch
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, IFuncionarioTabProps } from '../types';
import { CPFMask } from '../../../../components/common/TextMasks';
import { GradeEdicao } from '../../../../components/common/GradeEdicao';

export const FuncionarioTab: React.FC<IFuncionarioTabProps> = ({
  isBrowse,
  activeFuncionarioTab,
  setActiveFuncionarioTab
}) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
        <Tabs 
          value={activeFuncionarioTab} 
          onChange={(_, val) => setActiveFuncionarioTab(val)} 
        >
          <Tab label="1. Ficha de Registro" />
          <Tab label="2. CNH/CTPS" />
          <Tab label="3. PIS, FGTS & Salário" />
          <Tab label="4. Dependentes" />
          <Tab label="5. RAIS" />
        </Tabs>
      </Box>

      {/* Funcionário Subtab 1: Ficha de Registro */}
      {activeFuncionarioTab === 0 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 3 }}>
            <Controller
              name="funcAtivo"
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
                  label="Funcionário Ativo"
                  sx={{ mt: 0.5 }}
                />
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('funcionarioMatricula')}
              size="small" 
              fullWidth 
              label="Matrícula" 
              disabled={isBrowse} 
              error={!!errors.funcionarioMatricula}
              helperText={errors.funcionarioMatricula?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('funcionarioCargo')}
              size="small" 
              fullWidth 
              label="Cargo" 
              disabled={isBrowse} 
              error={!!errors.funcionarioCargo}
              helperText={errors.funcionarioCargo?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <TextField 
              {...register('funcionarioSalario', { valueAsNumber: true })}
              size="small" 
              type="number" 
              fullWidth 
              label="Salário" 
              disabled={isBrowse} 
              error={!!errors.funcionarioSalario}
              helperText={errors.funcionarioSalario?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <Controller
              name="funcionarioTipoSalario"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Tipo Salário</InputLabel>
                  <Select {...field} label="Tipo Salário">
                    <MenuItem value="Mensalista">Mensalista</MenuItem>
                    <MenuItem value="Horista">Horista</MenuItem>
                    <MenuItem value="Comissionado">Comissionado</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <TextField 
              {...register('funcionarioCargaHoraria', { valueAsNumber: true })}
              size="small" 
              type="number" 
              fullWidth 
              label="Carga Horária" 
              disabled={isBrowse} 
              error={!!errors.funcionarioCargaHoraria}
              helperText={errors.funcionarioCargaHoraria?.message}
            />
          </Grid>
          
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('funcionarioDepartamento')}
              size="small" 
              fullWidth 
              label="Departamento" 
              disabled={isBrowse} 
              error={!!errors.funcionarioDepartamento}
              helperText={errors.funcionarioDepartamento?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('funcionarioHorario')}
              size="small" 
              fullWidth 
              label="Horário de Trabalho" 
              disabled={isBrowse} 
              error={!!errors.funcionarioHorario}
              helperText={errors.funcionarioHorario?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <TextField 
              {...register('dataAdmissao')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Data Admissão" 
              disabled={isBrowse} 
              error={!!errors.dataAdmissao}
              helperText={errors.dataAdmissao?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <TextField 
              {...register('dataDemissao')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="Data Demissão" 
              disabled={isBrowse} 
              error={!!errors.dataDemissao}
              helperText={errors.dataDemissao?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <Controller
              name="funcionarioSituacao"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Situação</InputLabel>
                  <Select {...field} label="Situação">
                    <MenuItem value="Ativo">Ativo</MenuItem>
                    <MenuItem value="Afastado">Afastado</MenuItem>
                    <MenuItem value="Demitido">Demitido</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('funcionarioCaged')}
              size="small" 
              fullWidth 
              label="CAGED" 
              disabled={isBrowse} 
              error={!!errors.funcionarioCaged}
              helperText={errors.funcionarioCaged?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('funcionarioNomePai')}
              size="small" 
              fullWidth 
              label="Nome do Pai" 
              disabled={isBrowse} 
              error={!!errors.funcionarioNomePai}
              helperText={errors.funcionarioNomePai?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 5 }}>
            <TextField 
              {...register('funcionarioNomeMae')}
              size="small" 
              fullWidth 
              label="Nome da Mãe" 
              disabled={isBrowse} 
              error={!!errors.funcionarioNomeMae}
              helperText={errors.funcionarioNomeMae?.message}
            />
          </Grid>
        </Grid>
      )}

      {/* Funcionário Subtab 2: CNH/CTPS */}
      {activeFuncionarioTab === 1 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('cnhNumero')}
              size="small" 
              fullWidth 
              label="CNH Número" 
              disabled={isBrowse} 
              error={!!errors.cnhNumero}
              helperText={errors.cnhNumero?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('cnhCategoria')}
              size="small" 
              fullWidth 
              label="CNH Categoria" 
              disabled={isBrowse} 
              error={!!errors.cnhCategoria}
              helperText={errors.cnhCategoria?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('cnhVencimento')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="CNH Vencimento" 
              disabled={isBrowse} 
              error={!!errors.cnhVencimento}
              helperText={errors.cnhVencimento?.message}
            />
          </Grid>
          
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('ctpsNumero')}
              size="small" 
              fullWidth 
              label="CTPS Número" 
              disabled={isBrowse} 
              error={!!errors.ctpsNumero}
              helperText={errors.ctpsNumero?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <TextField 
              {...register('ctpsSerie')}
              size="small" 
              fullWidth 
              label="CTPS Série" 
              disabled={isBrowse} 
              error={!!errors.ctpsSerie}
              helperText={errors.ctpsSerie?.message}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <TextField 
              {...register('ctpsUf')}
              size="small" 
              fullWidth 
              label="CTPS UF" 
              slotProps={{ htmlInput: { maxLength: 2 } }} 
              disabled={isBrowse} 
              error={!!errors.ctpsUf}
              helperText={errors.ctpsUf?.message}
              onChange={(e) => {
                e.target.value = e.target.value.toUpperCase();
              }}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('ctpsEmissao')}
              type="date" 
              slotProps={{ inputLabel: { shrink: true } }} 
              size="small" 
              fullWidth 
              label="CTPS Data Emissão" 
              disabled={isBrowse} 
              error={!!errors.ctpsEmissao}
              helperText={errors.ctpsEmissao?.message}
            />
          </Grid>
        </Grid>
      )}

      {/* Funcionário Subtab 3: PIS, FGTS & Salário */}
      {activeFuncionarioTab === 2 && (
        <Box>
          <Grid container spacing={3} sx={{ mb: 4 }}>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('pisNumero')}
                size="small" 
                fullWidth 
                label="PIS Número" 
                disabled={isBrowse} 
                error={!!errors.pisNumero}
                helperText={errors.pisNumero?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('pisBanco')}
                size="small" 
                fullWidth 
                label="PIS Banco" 
                disabled={isBrowse} 
                error={!!errors.pisBanco}
                helperText={errors.pisBanco?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuAgenciaPis')}
                size="small" 
                fullWidth 
                label="PIS Agência" 
                disabled={isBrowse} 
                error={!!errors.fuAgenciaPis}
                helperText={errors.fuAgenciaPis?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuNomeAgenciaPis')}
                size="small" 
                fullWidth 
                label="PIS Nome Agência" 
                disabled={isBrowse} 
                error={!!errors.fuNomeAgenciaPis}
                helperText={errors.fuNomeAgenciaPis?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('pisInscricao')}
                type="date" 
                slotProps={{ inputLabel: { shrink: true } }} 
                size="small" 
                fullWidth 
                label="PIS Data Inscrição" 
                disabled={isBrowse} 
                error={!!errors.pisInscricao}
                helperText={errors.pisInscricao?.message}
              />
            </Grid>
          </Grid>

          <Grid container spacing={3} sx={{ mb: 4 }}>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fgtsBanco')}
                size="small" 
                fullWidth 
                label="FGTS Banco" 
                disabled={isBrowse} 
                error={!!errors.fgtsBanco}
                helperText={errors.fgtsBanco?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fgtsAgencia')}
                size="small" 
                fullWidth 
                label="FGTS Agência" 
                disabled={isBrowse} 
                error={!!errors.fgtsAgencia}
                helperText={errors.fgtsAgencia?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuNomeAgenciaFgts')}
                size="small" 
                fullWidth 
                label="FGTS Nome Agência" 
                disabled={isBrowse} 
                error={!!errors.fuNomeAgenciaFgts}
                helperText={errors.fuNomeAgenciaFgts?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fgtsConta')}
                size="small" 
                fullWidth 
                label="FGTS Conta" 
                disabled={isBrowse} 
                error={!!errors.fgtsConta}
                helperText={errors.fgtsConta?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuCategoriaFgts')}
                size="small" 
                fullWidth 
                label="FGTS Categoria" 
                disabled={isBrowse} 
                error={!!errors.fuCategoriaFgts}
                helperText={errors.fuCategoriaFgts?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuDataFgts')}
                type="date" 
                slotProps={{ inputLabel: { shrink: true } }} 
                size="small" 
                fullWidth 
                label="FGTS Data Opção" 
                disabled={isBrowse} 
                error={!!errors.fuDataFgts}
                helperText={errors.fuDataFgts?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="fuFgtsOpcao"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Opção FGTS</InputLabel>
                    <Select {...field} label="Opção FGTS">
                      <MenuItem value="Optante">Optante</MenuItem>
                      <MenuItem value="Não Optante">Não Optante</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
          </Grid>

          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuBancoConta')}
                size="small" 
                fullWidth 
                label="Salário Banco" 
                disabled={isBrowse} 
                error={!!errors.fuBancoConta}
                helperText={errors.fuBancoConta?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuAgenciaConta')}
                size="small" 
                fullWidth 
                label="Salário Agência" 
                disabled={isBrowse} 
                error={!!errors.fuAgenciaConta}
                helperText={errors.fuAgenciaConta?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <TextField 
                {...register('fuAgenciaDigito')}
                size="small" 
                fullWidth 
                label="Dígito Agência" 
                disabled={isBrowse} 
                error={!!errors.fuAgenciaDigito}
                helperText={errors.fuAgenciaDigito?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('fuContaSalario')}
                size="small" 
                fullWidth 
                label="Salário Conta" 
                disabled={isBrowse} 
                error={!!errors.fuContaSalario}
                helperText={errors.fuContaSalario?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 1 }}>
              <TextField 
                {...register('fuContaDigito')}
                size="small" 
                fullWidth 
                label="Dígito Conta" 
                disabled={isBrowse} 
                error={!!errors.fuContaDigito}
                helperText={errors.fuContaDigito?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="fuTipoConta"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Tipo Conta</InputLabel>
                    <Select {...field} label="Tipo Conta">
                      <MenuItem value="Corrente">Corrente</MenuItem>
                      <MenuItem value="Poupança">Poupança</MenuItem>
                      <MenuItem value="Salário">Salário</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
          </Grid>
        </Box>
      )}

      {/* Funcionário Subtab 4: Dependentes */}
      {activeFuncionarioTab === 3 && (
        <GradeEdicao
          titulo="Lista de Dependentes"
          name="funcDependentes"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Dependente"
          defaultRow={{ nome: '', dataNascimento: '', cpf: '', parentesco: 'Filho' }}
          colunas={[
            {
              header: 'Nome do Dependente',
              renderCell: (idx) => (
                <TextField 
                  {...register(`funcDependentes.${idx}.nome`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.funcDependentes?.[idx]?.nome}
                />
              )
            },
            {
              header: 'Data Nascimento',
              width: 180,
              renderCell: (idx) => (
                <TextField 
                  {...register(`funcDependentes.${idx}.dataNascimento`)}
                  size="small" 
                  type="date" 
                  slotProps={{ inputLabel: { shrink: true } }} 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.funcDependentes?.[idx]?.dataNascimento}
                />
              )
            },
            {
              header: 'CPF',
              width: 180,
              renderCell: (idx) => (
                <TextField 
                  {...register(`funcDependentes.${idx}.cpf`)}
                  size="small" 
                  fullWidth 
                  placeholder="Apenas números" 
                  disabled={isBrowse} 
                  error={!!errors.funcDependentes?.[idx]?.cpf}
                  helperText={errors.funcDependentes?.[idx]?.cpf?.message}
                  slotProps={{
                    input: {
                      inputComponent: CPFMask as any
                    }
                  }}
                />
              )
            },
            {
              header: 'Parentesco',
              width: 160,
              renderCell: (idx) => (
                <Controller
                  name={`funcDependentes.${idx}.parentesco`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Filho">Filho/Filha</MenuItem>
                        <MenuItem value="Cônjuge">Cônjuge</MenuItem>
                        <MenuItem value="Outro">Outro</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            }
          ]}
        />
      )}

      {/* Funcionário Subtab 5: RAIS */}
      {activeFuncionarioTab === 4 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 3 }}>
            <Controller
              name="fuRaisRaca"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Raça / Cor (RAIS)</InputLabel>
                  <Select {...field} label="Raça / Cor (RAIS)">
                    <MenuItem value="Branca">Branca</MenuItem>
                    <MenuItem value="Preta">Preta</MenuItem>
                    <MenuItem value="Parda">Parda</MenuItem>
                    <MenuItem value="Amarela">Amarela</MenuItem>
                    <MenuItem value="Indígena">Indígena</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <Controller
              name="fuRaisVinculo"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Vínculo RAIS</InputLabel>
                  <Select {...field} label="Vínculo RAIS">
                    <MenuItem value="CLT">CLT (Prazo Indeterminado)</MenuItem>
                    <MenuItem value="Temporário">Temporário / Contrato</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <Controller
              name="fuRaisTipoAdmissao"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Tipo Admissão RAIS</InputLabel>
                  <Select {...field} label="Tipo Admissão RAIS">
                    <MenuItem value="PrimeiroEmprego">Primeiro Emprego</MenuItem>
                    <MenuItem value="Reemprego">Reemprego</MenuItem>
                    <MenuItem value="Transferencia">Transferência</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 3 }}>
            <Controller
              name="fuRaisTipoDeficiencia"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Deficiência RAIS</InputLabel>
                  <Select {...field} label="Deficiência RAIS">
                    <MenuItem value="Nenhuma">Nenhuma</MenuItem>
                    <MenuItem value="Física">Física</MenuItem>
                    <MenuItem value="Auditiva">Auditiva</MenuItem>
                    <MenuItem value="Visual">Visual</MenuItem>
                    <MenuItem value="Intelectual">Intelectual</MenuItem>
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


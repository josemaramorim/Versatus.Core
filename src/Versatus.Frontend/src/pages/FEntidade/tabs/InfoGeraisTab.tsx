import React from 'react';
import { 
  Box, 
  Tabs, 
  Tab, 
  FormControl, 
  Select, 
  MenuItem, 
  TextField, 
  Checkbox 
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, IInfoGeraisTabProps } from '../types';
import { CEPMask, TelefoneMask } from '../../../components/common/TextMasks';
import { GradeEdicao } from '../../../components/common/GradeEdicao';

export const InfoGeraisTab: React.FC<IInfoGeraisTabProps> = ({
  isBrowse,
  activeInfoTab,
  setActiveInfoTab
}) => {
  const { register, control, formState: { errors } } = useFormContext<IEntidadeForm>();

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
        <Tabs 
          value={activeInfoTab} 
          onChange={(_, val) => setActiveInfoTab(val)} 
        >
          <Tab label="1. Endereços" />
          <Tab label="2. Telefones" />
          <Tab label="3. Contatos" />
          <Tab label="4. CNAE" />
          <Tab label="5. Empresas Vinculadas" />
        </Tabs>
      </Box>

      {/* Info Geral Subtab 1: Endereços */}
      {activeInfoTab === 0 && (
        <GradeEdicao
          titulo="Lista de Endereços"
          name="enderecos"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Endereço"
          defaultRow={{ tipo: 'Comercial', logradouro: '', numero: '', bairro: '', cidade: '', uf: 'SP', cep: '' }}
          colunas={[
            {
              header: 'Tipo',
              width: 150,
              renderCell: (idx) => (
                <Controller
                  name={`enderecos.${idx}.tipo`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Comercial">Comercial</MenuItem>
                        <MenuItem value="Residencial">Residencial</MenuItem>
                        <MenuItem value="Cobranca">Cobrança</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Logradouro',
              renderCell: (idx) => (
                <TextField 
                  {...register(`enderecos.${idx}.logradouro`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.enderecos?.[idx]?.logradouro}
                />
              )
            },
            {
              header: 'Nº',
              width: 90,
              renderCell: (idx) => (
                <TextField 
                  {...register(`enderecos.${idx}.numero`)}
                  size="small" 
                  disabled={isBrowse} 
                  error={!!errors.enderecos?.[idx]?.numero}
                />
              )
            },
            {
              header: 'Bairro',
              renderCell: (idx) => (
                <TextField 
                  {...register(`enderecos.${idx}.bairro`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.enderecos?.[idx]?.bairro}
                />
              )
            },
            {
              header: 'Cidade',
              renderCell: (idx) => (
                <TextField 
                  {...register(`enderecos.${idx}.cidade`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.enderecos?.[idx]?.cidade}
                />
              )
            },
            {
              header: 'UF',
              width: 70,
              renderCell: (idx) => (
                <TextField 
                  {...register(`enderecos.${idx}.uf`)}
                  size="small" 
                  disabled={isBrowse} 
                  error={!!errors.enderecos?.[idx]?.uf}
                  slotProps={{ htmlInput: { maxLength: 2 } }}
                  onChange={(e) => {
                    e.target.value = e.target.value.toUpperCase();
                  }}
                />
              )
            },
            {
              header: 'CEP',
              width: 120,
              renderCell: (idx) => (
                <TextField 
                  {...register(`enderecos.${idx}.cep`)}
                  size="small" 
                  disabled={isBrowse} 
                  error={!!errors.enderecos?.[idx]?.cep}
                  slotProps={{
                    input: {
                      inputComponent: CEPMask as any
                    }
                  }}
                />
              )
            }
          ]}
        />
      )}

      {/* Info Geral Subtab 2: Telefones */}
      {activeInfoTab === 1 && (
        <GradeEdicao
          titulo="Lista de Telefones"
          name="telefones"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Telefone"
          defaultRow={{ tipo: 'Celular', numero: '', contato: '' }}
          colunas={[
            {
              header: 'Tipo',
              width: 150,
              renderCell: (idx) => (
                <Controller
                  name={`telefones.${idx}.tipo`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Celular">Celular</MenuItem>
                        <MenuItem value="Fixo">Fixo</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Número',
              renderCell: (idx) => (
                <TextField 
                  {...register(`telefones.${idx}.numero`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.telefones?.[idx]?.numero}
                  slotProps={{
                    input: {
                      inputComponent: TelefoneMask as any
                    }
                  }}
                />
              )
            },
            {
              header: 'Contato Responsável',
              renderCell: (idx) => (
                <TextField 
                  {...register(`telefones.${idx}.contato`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.telefones?.[idx]?.contato}
                />
              )
            }
          ]}
        />
      )}

      {/* Info Geral Subtab 3: Contatos */}
      {activeInfoTab === 2 && (
        <GradeEdicao
          titulo="Lista de Contatos"
          name="contatos"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Contato"
          defaultRow={{ nome: '', cargo: '', email: '', celular: '' }}
          colunas={[
            {
              header: 'Nome do Contato',
              renderCell: (idx) => (
                <TextField 
                  {...register(`contatos.${idx}.nome`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.contatos?.[idx]?.nome}
                />
              )
            },
            {
              header: 'Cargo / Setor',
              renderCell: (idx) => (
                <TextField 
                  {...register(`contatos.${idx}.cargo`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.contatos?.[idx]?.cargo}
                />
              )
            },
            {
              header: 'E-mail',
              renderCell: (idx) => (
                <TextField 
                  {...register(`contatos.${idx}.email`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.contatos?.[idx]?.email}
                  helperText={errors.contatos?.[idx]?.email?.message}
                />
              )
            },
            {
              header: 'Celular',
              renderCell: (idx) => (
                <TextField 
                  {...register(`contatos.${idx}.celular`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.contatos?.[idx]?.celular}
                  slotProps={{
                    input: {
                      inputComponent: TelefoneMask as any
                    }
                  }}
                />
              )
            }
          ]}
        />
      )}

      {/* Info Geral Subtab 4: CNAE */}
      {activeInfoTab === 3 && (
        <GradeEdicao
          titulo="Atividades Econômicas (CNAE)"
          name="cnaes"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Vincular CNAE"
          defaultRow={{ idCnae: 1, principal: false }}
          colunas={[
            {
              header: 'CNAE Atividade',
              renderCell: (idx) => (
                <Controller
                  name={`cnaes.${idx}.idCnae` as any}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value={1}>6201-5/01 - Des. Softwares</MenuItem>
                        <MenuItem value={2}>6202-3/00 - Consultoria TI</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'CNAE Principal?',
              width: 150,
              renderCell: (idx) => (
                <Controller
                  name={`cnaes.${idx}.principal` as any}
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

      {/* Info Geral Subtab 5: Empresas Vinculadas */}
      {activeInfoTab === 4 && (
        <GradeEdicao
          titulo="Grupo Econômico / Empresas"
          name="empresas"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Vincular Empresa"
          defaultRow={{ idEmpresa: 1, ativo: true }}
          colunas={[
            {
              header: 'Empresa',
              renderCell: (idx) => (
                <Controller
                  name={`empresas.${idx}.idEmpresa` as any}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value={1}>Empresa Matriz SP</MenuItem>
                        <MenuItem value={2}>Empresa Filial RJ</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Vínculo Ativo?',
              width: 150,
              renderCell: (idx) => (
                <Controller
                  name={`empresas.${idx}.ativo` as any}
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

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
  Checkbox 
} from '@mui/material';
import { useFormContext, Controller } from 'react-hook-form';
import type { IEntidadeForm, IClienteTabProps } from '../types';
import { CPFMask, TelefoneMask } from '../../../components/common/TextMasks';
import { GradeEdicao } from '../../../components/common/GradeEdicao';

export const ClienteTab: React.FC<IClienteTabProps> = ({
  isBrowse,
  activeClienteTab,
  setActiveClienteTab
}) => {
  const { register, control, watch, formState: { errors } } = useFormContext<IEntidadeForm>();

  const tipoPessoa = watch('tipoPessoa');
  const cliImovel = watch('cliImovel');

  return (
    <Box>
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
        <Tabs 
          value={activeClienteTab} 
          onChange={(_, val) => setActiveClienteTab(val)} 
        >
          <Tab label="1. Venda" />
          <Tab label="2. Financeiro" />
          <Tab label="3. Referências" />
          <Tab label="4. Bens e Imóveis" />
          <Tab label="5. Cartões" />
          <Tab label="6. Parentesco" />
          {tipoPessoa === 2 && <Tab label="7. Sócios / QSA" />}
        </Tabs>
      </Box>

      {/* Cliente Subtab 1: Venda */}
      {activeClienteTab === 0 && (
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idRotaVenda"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Rota de Venda</InputLabel>
                  <Select {...field} label="Rota de Venda">
                    <MenuItem value={1}>Rota Nordeste</MenuItem>
                    <MenuItem value={2}>Rota Sul</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idRotaEntrega"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Rota de Entrega</InputLabel>
                  <Select {...field} label="Rota de Entrega">
                    <MenuItem value={1}>Entrega Rápida SP</MenuItem>
                    <MenuItem value={2}>Transportadora Terceirizada</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idAreaVenda"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Área de Venda</InputLabel>
                  <Select {...field} label="Área de Venda">
                    <MenuItem value={1}>Região Metropolitana</MenuItem>
                    <MenuItem value={2}>Interior SP</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idCategoriaCliente"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Categoria Cliente</InputLabel>
                  <Select {...field} label="Categoria Cliente">
                    <MenuItem value={1}>Revenda Ouro</MenuItem>
                    <MenuItem value={2}>Cliente Final</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idConceito"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Conceito / Classificação</InputLabel>
                  <Select {...field} label="Conceito / Classificação">
                    <MenuItem value={1}>Conceito A (Excelente)</MenuItem>
                    <MenuItem value={2}>Conceito B (Regular)</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="idVendedor"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Vendedor Preferencial</InputLabel>
                  <Select {...field} label="Vendedor Preferencial">
                    <MenuItem value={1}>Carlos Alberto - Vendas</MenuItem>
                    <MenuItem value={2}>Mariana Silva - Telemarketing</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="cliTipoFrete"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Tipo de Frete Padrão</InputLabel>
                  <Select {...field} label="Tipo de Frete Padrão">
                    <MenuItem value="CIF">CIF (Emitente)</MenuItem>
                    <MenuItem value="FOB">FOB (Destinatário)</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="cliMeioPublicidade"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth size="small" disabled={isBrowse}>
                  <InputLabel>Meio de Divulgação / Publicidade</InputLabel>
                  <Select {...field} label="Meio de Divulgação / Publicidade">
                    <MenuItem value="Internet">Internet/Redes Sociais</MenuItem>
                    <MenuItem value="Indicação">Indicação</MenuItem>
                  </Select>
                </FormControl>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <TextField 
              {...register('cliNumAlternativo')}
              size="small" 
              fullWidth 
              label="Número Alternativo / Ramal" 
              disabled={isBrowse} 
              error={!!errors.cliNumAlternativo}
              helperText={errors.cliNumAlternativo?.message}
              slotProps={{
                input: {
                  inputComponent: TelefoneMask as any
                }
              }}
            />
          </Grid>
        </Grid>
      )}

      {/* Cliente Subtab 2: Financeiro */}
      {activeClienteTab === 1 && (
        <Box>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Controller
                name="idPortador"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Portador Financeiro</InputLabel>
                    <Select {...field} label="Portador Financeiro">
                      <MenuItem value={1}>Banco do Brasil - Carteira 17</MenuItem>
                      <MenuItem value={2}>Itaú Cobrança</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 4 }}>
              <Controller
                name="idCondicaoPagamento"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Condição de Pagamento Padrão</InputLabel>
                    <Select {...field} label="Condição de Pagamento Padrão">
                      <MenuItem value={1}>30/60/90 Dias</MenuItem>
                      <MenuItem value={2}>À Vista (Pix)</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <TextField 
                {...register('diaCobranca', { valueAsNumber: true })}
                size="small" 
                type="number" 
                fullWidth 
                label="Melhor Dia de Cobrança" 
                disabled={isBrowse} 
                error={!!errors.diaCobranca}
                helperText={errors.diaCobranca?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <Controller
                name="idRotaCobranca"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Rota Cobrança</InputLabel>
                    <Select {...field} label="Rota Cobrança">
                      <MenuItem value={1}>Cobranca Sedex</MenuItem>
                      <MenuItem value={2}>E-mail Automático</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <TextField 
                {...register('limiteCredito', { valueAsNumber: true })}
                size="small" 
                type="number" 
                fullWidth 
                label="Limite de Crédito (R$)" 
                disabled={isBrowse} 
                error={!!errors.limiteCredito}
                helperText={errors.limiteCredito?.message}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="cliImovel"
                control={control}
                render={({ field }) => (
                  <FormControl fullWidth size="small" disabled={isBrowse}>
                    <InputLabel>Situação Imóvel</InputLabel>
                    <Select {...field} label="Situação Imóvel">
                      <MenuItem value="Próprio">Próprio</MenuItem>
                      <MenuItem value="Alugado">Alugado</MenuItem>
                      <MenuItem value="Financiado">Financiado</MenuItem>
                    </Select>
                  </FormControl>
                )}
              />
            </Grid>
            {cliImovel === 'Alugado' && (
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField 
                  {...register('cliValorAluguel', { valueAsNumber: true })}
                  size="small" 
                  type="number" 
                  fullWidth 
                  label="Valor Aluguel" 
                  disabled={isBrowse} 
                  error={!!errors.cliValorAluguel}
                  helperText={errors.cliValorAluguel?.message}
                />
              </Grid>
            )}
          </Grid>
          
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5, mt: 3 }}>
            <Controller
              name="cliItemFinanceiroPadrao"
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
                  label="Aplicar o item financeiro padrão ?" 
                />
              )}
            />
            <Controller
              name="cliEnviarCND"
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
                  label="Enviar CND Estadual NF-e" 
                />
              )}
            />
            <Controller
              name="cliObrigatorioPedidoB2B"
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
                  label="Obrigatório informar pedido compra (B2B)" 
                />
              )}
            />
          </Box>
        </Box>
      )}

      {/* Cliente Subtab 3: Referências */}
      {activeClienteTab === 2 && (
        <GradeEdicao
          titulo="Referências Comerciais/Pessoais"
          name="cliReferencias"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Referência"
          defaultRow={{ nome: '', telefone: '', tipo: 'Comercial', observacao: '' }}
          colunas={[
            {
              header: 'Nome/Razão Social da Referência',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliReferencias.${idx}.nome`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliReferencias?.[idx]?.nome}
                />
              )
            },
            {
              header: 'Telefone',
              width: 180,
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliReferencias.${idx}.telefone`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliReferencias?.[idx]?.telefone}
                  slotProps={{
                    input: {
                      inputComponent: TelefoneMask as any
                    }
                  }}
                />
              )
            },
            {
              header: 'Tipo',
              width: 150,
              renderCell: (idx) => (
                <Controller
                  name={`cliReferencias.${idx}.tipo`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Comercial">Comercial</MenuItem>
                        <MenuItem value="Pessoal">Pessoal</MenuItem>
                        <MenuItem value="Bancaria">Bancária</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Observação',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliReferencias.${idx}.observacao`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliReferencias?.[idx]?.observacao}
                />
              )
            }
          ]}
        />
      )}

      {/* Cliente Subtab 4: Bens e Imóveis */}
      {activeClienteTab === 3 && (
        <GradeEdicao
          titulo="Declaração de Bens"
          name="cliBens"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Bem"
          defaultRow={{ descricao: '', valor: 0, alienado: false, observacao: '' }}
          colunas={[
            {
              header: 'Descrição do Patrimônio/Bem',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliBens.${idx}.descricao`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliBens?.[idx]?.descricao}
                />
              )
            },
            {
              header: 'Valor Estimado (R$)',
              width: 160,
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliBens.${idx}.valor`, { valueAsNumber: true })}
                  size="small" 
                  type="number" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliBens?.[idx]?.valor}
                />
              )
            },
            {
              header: 'Alienado?',
              width: 100,
              renderCell: (idx) => (
                <Controller
                  name={`cliBens.${idx}.alienado`}
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
            },
            {
              header: 'Observação',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliBens.${idx}.observacao`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliBens?.[idx]?.observacao}
                />
              )
            }
          ]}
        />
      )}

      {/* Cliente Subtab 5: Cartões */}
      {activeClienteTab === 4 && (
        <GradeEdicao
          titulo="Cartões de Crédito Vinculados"
          name="cliCartoes"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Cartão"
          defaultRow={{ bandeira: 'Visa', numero: '', nome: '', vencimento: '' }}
          colunas={[
            {
              header: 'Bandeira',
              width: 160,
              renderCell: (idx) => (
                <Controller
                  name={`cliCartoes.${idx}.bandeira`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Visa">Visa</MenuItem>
                        <MenuItem value="Mastercard">Mastercard</MenuItem>
                        <MenuItem value="Amex">American Express</MenuItem>
                        <MenuItem value="Elo">Elo</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Número do Cartão',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliCartoes.${idx}.numero`)}
                  size="small" 
                  fullWidth 
                  placeholder="Ex: 0000 0000 0000 0000" 
                  disabled={isBrowse} 
                  error={!!errors.cliCartoes?.[idx]?.numero}
                />
              )
            },
            {
              header: 'Nome Impresso',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliCartoes.${idx}.nome`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliCartoes?.[idx]?.nome}
                />
              )
            },
            {
              header: 'Vencimento',
              width: 140,
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliCartoes.${idx}.vencimento`)}
                  size="small" 
                  fullWidth 
                  placeholder="MM/AAAA" 
                  disabled={isBrowse} 
                  error={!!errors.cliCartoes?.[idx]?.vencimento}
                />
              )
            }
          ]}
        />
      )}

      {/* Cliente Subtab 6: Parentesco */}
      {activeClienteTab === 5 && (
        <GradeEdicao
          titulo="Vínculos de Parentesco"
          name="cliParentes"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Parente"
          defaultRow={{ nome: '', parentesco: 'Pai', telefone: '', observacao: '' }}
          colunas={[
            {
              header: 'Nome do Parente',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliParentes.${idx}.nome`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliParentes?.[idx]?.nome}
                />
              )
            },
            {
              header: 'Grau Parentesco',
              width: 180,
              renderCell: (idx) => (
                <Controller
                  name={`cliParentes.${idx}.parentesco`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Pai">Pai</MenuItem>
                        <MenuItem value="Mãe">Mãe</MenuItem>
                        <MenuItem value="Cônjuge">Cônjuge</MenuItem>
                        <MenuItem value="Irmão">Irmão/Irmã</MenuItem>
                        <MenuItem value="Filho">Filho/Filha</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Telefone de Contato',
              width: 180,
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliParentes.${idx}.telefone`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliParentes?.[idx]?.telefone}
                  slotProps={{
                    input: {
                      inputComponent: TelefoneMask as any
                    }
                  }}
                />
              )
            },
            {
              header: 'Observação',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliParentes.${idx}.observacao`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliParentes?.[idx]?.observacao}
                />
              )
            }
          ]}
        />
      )}

      {/* Cliente Subtab 7: Sócios / QSA */}
      {activeClienteTab === 6 && tipoPessoa === 2 && (
        <GradeEdicao
          titulo="Quadro de Sócios e Administradores (QSA)"
          name="cliSocios"
          isBrowse={isBrowse}
          botaoAdicionarRotulo="Adicionar Sócio"
          defaultRow={{ nome: '', cpf: '', cargo: 'Sócio', percentualParticipacao: 0 }}
          colunas={[
            {
              header: 'Nome Completo do Sócio',
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliSocios.${idx}.nome`)}
                  size="small" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliSocios?.[idx]?.nome}
                />
              )
            },
            {
              header: 'CPF do Sócio',
              width: 180,
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliSocios.${idx}.cpf`)}
                  size="small" 
                  fullWidth 
                  placeholder="Apenas números" 
                  disabled={isBrowse} 
                  error={!!errors.cliSocios?.[idx]?.cpf}
                  helperText={errors.cliSocios?.[idx]?.cpf?.message}
                  slotProps={{
                    input: {
                      inputComponent: CPFMask as any
                    }
                  }}
                />
              )
            },
            {
              header: 'Qualificação/Cargo',
              width: 180,
              renderCell: (idx) => (
                <Controller
                  name={`cliSocios.${idx}.cargo`}
                  control={control}
                  render={({ field }) => (
                    <FormControl size="small" fullWidth disabled={isBrowse}>
                      <Select {...field}>
                        <MenuItem value="Sócio">Sócio</MenuItem>
                        <MenuItem value="Sócio-Administrador">Sócio-Administrador</MenuItem>
                        <MenuItem value="Administrador">Administrador</MenuItem>
                        <MenuItem value="Diretor">Diretor</MenuItem>
                      </Select>
                    </FormControl>
                  )}
                />
              )
            },
            {
              header: 'Participação (%)',
              width: 140,
              renderCell: (idx) => (
                <TextField 
                  {...register(`cliSocios.${idx}.percentualParticipacao`, { valueAsNumber: true })}
                  size="small" 
                  type="number" 
                  fullWidth 
                  disabled={isBrowse} 
                  error={!!errors.cliSocios?.[idx]?.percentualParticipacao}
                />
              )
            }
          ]}
        />
      )}
    </Box>
  );
};

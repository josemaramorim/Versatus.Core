import React, { useState, useEffect } from 'react';
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
  Tabs,
  Tab,
  Typography,
  Snackbar,
  Alert,
  FormHelperText,
  Divider,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Card,
  CardContent,
  Switch
} from '@mui/material';
import { useForm, FormProvider, Controller, useFieldArray } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEnumOptions } from '../../../hooks/useEnums';
import { useTabs } from '../../../context/TabsContext';
import { buildApiEndpoint, getApiHeaders } from '../../../config/api';

import { CadastroBasePage } from '../../../components/crud/CadastroBasePage';
import { CondicaoPagamentoCadastroConfig } from './CondicaoPagamentoCadastroConfig';
import type { CadastroModalMode } from '../../../types/cadastro';
import type { ICondicaoPagamentoForm } from './types';
import { condicaoPagamentoSchema } from './schema';

export interface ICondicaoPagamentoFormViewProps {
  mode: CadastroModalMode;
  record: ICondicaoPagamentoForm;
  onSave: (data: ICondicaoPagamentoForm) => void;
}

export const CondicaoPagamentoFormView: React.FC<ICondicaoPagamentoFormViewProps> = ({
  mode,
  record,
  onSave
}) => {
  const [toast, setToast] = useState<{
    open: boolean;
    message: string;
    severity: 'success' | 'error';
  }>({
    open: false,
    message: '',
    severity: 'success',
  });

  const showMessage = (message: string, severity: 'success' | 'error' = 'success') => {
    setToast({
      open: true,
      message,
      severity,
    });
  };

  const isBrowse = mode === 'delete' || mode === 'view';

  const { abaAtivaId, marcarDirty } = useTabs();

  // Configuração do React Hook Form
  const methods = useForm<ICondicaoPagamentoForm>({
    resolver: zodResolver(condicaoPagamentoSchema) as any,
    defaultValues: record,
    mode: 'onChange',
  });

  const { handleSubmit, reset, control, formState: { errors, isDirty }, watch, setValue } = methods;

  // Sincronizar estado de formulário alterado (dirty) com a aba ativa
  useEffect(() => {
    if (abaAtivaId && mode !== 'view' && mode !== 'delete') {
      marcarDirty(abaAtivaId, isDirty);
    }
  }, [isDirty, abaAtivaId, mode, marcarDirty]);

  const { fields: parcelasFields, replace: replaceParcelas } = useFieldArray({
    control,
    name: 'parceladas'
  });

  const { fields: faixasFields, replace: replaceFaixas } = useFieldArray({
    control,
    name: 'faixas'
  });

  // Lookups do backend
  const [grupos, setGrupos] = useState<{ idGrupoCondicaoPagamento: number; descricao: string }[]>([]);
  const [formasCobranca, setFormasCobranca] = useState<{ idFormaCobranca: number; descricao: string }[]>([]);
  const [formasPagamento, setFormasPagamento] = useState<{ idForma: number; nome: string }[]>([]);

  useEffect(() => {
    // Buscar Grupos
    fetch(buildApiEndpoint('/api/condicaopagamento/grupos'), { headers: getApiHeaders() })
      .then(res => res.json())
      .then(data => setGrupos(data))
      .catch(() => console.error('Erro ao buscar grupos de condição de pagamento.'));

    // Buscar Formas de Cobrança
    fetch(buildApiEndpoint('/api/condicaopagamento/formas-cobranca'), { headers: getApiHeaders() })
      .then(res => res.json())
      .then(data => setFormasCobranca(data))
      .catch(() => console.error('Erro ao buscar formas de cobrança.'));

    // Buscar Formas de Pagamento
    fetch(buildApiEndpoint('/api/financeiro/formas-pagamento'), { headers: getApiHeaders() })
      .then(res => res.json())
      .then(data => setFormasPagamento(data))
      .catch(() => console.error('Erro ao buscar formas de pagamento.'));
  }, []);

  // Sincronizar record do CRUD com o Form State
  useEffect(() => {
    reset(record);
  }, [record, reset]);

  // Observar valores dinâmicos para a interface
  const watchedValues = watch([
    'idTipoCondicaoPagto',
    'recebeAcrescimo',
    'recebeDesconto',
    'alteraParcelas',
    'primeiraParcelaAVista',
    'quantidadeParcela',
    'quantidadeFaixa',
    'idParcelamentoTipo',
    'tipoDivisaoParcelamento',
    'parceladas',
    'faixas'
  ]);

  const [
    idTipoCondicaoPagto,
    recebeAcrescimo,
    recebeDesconto,
    alteraParcelas,
    primeiraParcelaAVista,
    quantidadeParcela,
    quantidadeFaixa,
    idParcelamentoTipo,
    tipoDivisaoParcelamento,
    parceladasWatch,
    faixasWatch
  ] = watchedValues;

  // Carregar enums dinâmicos
  const { options: fetchedTipoCondicao } = useEnumOptions(35);
  const tipoCondicaoOptions = fetchedTipoCondicao?.length ? fetchedTipoCondicao : [
    { value: 36, label: 'Parcelada' },
    { value: 37, label: 'Faixa Dias' },
    { value: 38, label: 'Semanal' },
  ];

  const { options: fetchedDisponibilidade } = useEnumOptions(55);
  const disponibilidadeOptions = fetchedDisponibilidade?.length ? fetchedDisponibilidade : [
    { value: 56, label: 'Pagamento' },
    { value: 57, label: 'Recebimento' },
    { value: 101, label: 'Ambas' },
  ];

  const { options: fetchedVencimentoTipo } = useEnumOptions(58);
  const vencimentoTipoOptions = fetchedVencimentoTipo?.length ? fetchedVencimentoTipo : [
    { value: 59, label: 'Normal' },
    { value: 60, label: 'Antecipa' },
    { value: 61, label: 'Posterga' },
  ];

  const { options: fetchedParcelamentoTipo } = useEnumOptions(118);
  const parcelamentoTipoOptions = fetchedParcelamentoTipo?.length ? fetchedParcelamentoTipo : [
    { value: 119, label: 'Fixo' },
    { value: 120, label: 'Dias' },
    { value: 693, label: 'Dia Útil Mês' },
  ];

  const { options: fetchedDiaSemana } = useEnumOptions(163);
  const diaSemanaOptions = fetchedDiaSemana?.length ? fetchedDiaSemana : [
    { value: 164, label: 'Domingo' },
    { value: 165, label: 'Segunda-feira' },
    { value: 166, label: 'Terça-feira' },
    { value: 167, label: 'Quarta-feira' },
    { value: 168, label: 'Quinta-feira' },
    { value: 169, label: 'Sexta-feira' },
    { value: 170, label: 'Sábado' },
  ];

  const { options: fetchedDivisaoTipo } = useEnumOptions(602);
  const divisaoTipoOptions = fetchedDivisaoTipo?.length ? fetchedDivisaoTipo : [
    { value: 603, label: 'Informado' },
    { value: 604, label: 'Igual' },
  ];

  const { options: fetchedArredondamento } = useEnumOptions(45);
  const arredondamentoOptions = fetchedArredondamento?.length ? fetchedArredondamento : [
    { value: 46, label: 'Primeira Parcela' },
    { value: 47, label: 'Última Parcela' },
  ];

  // Monitorar quantidade de parcelas e gerar grade automaticamente
  useEffect(() => {
    if (idTipoCondicaoPagto !== 36 || alteraParcelas) return;

    const qty = Number(quantidadeParcela) || 0;
    const currentList = parceladasWatch || [];

    if (qty !== currentList.length) {
      const newList = [];
      const defaultPct = tipoDivisaoParcelamento === 604 ? 0 : parseFloat((100 / qty).toFixed(2));

      for (let i = 0; i < qty; i++) {
        const existing = currentList[i];
        newList.push({
          idCondicaoPagtoParcela: existing?.idCondicaoPagtoParcela || 0,
          numeroParcela: i + 1,
          numeroDias: existing?.numeroDias || 0,
          percentualDivisao: existing?.percentualDivisao || defaultPct,
          diasLiberado: existing?.diasLiberado || 0,
          percentualValorMinimo: existing?.percentualValorMinimo || 0,
        });
      }
      
      // Ajustar arredondamento percentual na primeira/última se necessário
      if (tipoDivisaoParcelamento === 603 && newList.length > 0) {
        const sum = newList.reduce((acc, curr) => acc + curr.percentualDivisao, 0);
        if (sum !== 100) {
          newList[newList.length - 1].percentualDivisao = parseFloat((newList[newList.length - 1].percentualDivisao + (100 - sum)).toFixed(2));
        }
      }

      replaceParcelas(newList);
    }
  }, [quantidadeParcela, idTipoCondicaoPagto, alteraParcelas, tipoDivisaoParcelamento, replaceParcelas]);

  // Monitorar quantidade de faixas e gerar grade automaticamente
  useEffect(() => {
    if (idTipoCondicaoPagto !== 37) return;

    const qty = Number(quantidadeFaixa) || 0;
    const currentList = faixasWatch || [];

    if (qty !== currentList.length) {
      const newList = [];
      for (let i = 0; i < qty; i++) {
        const existing = currentList[i];
        newList.push({
          idCondicaoPagtoParcela: existing?.idCondicaoPagtoParcela || 0,
          numeroParcela: i + 1,
          numeroDias: existing?.numeroDias || 1, // Dia de vencimento
          diaInicial: existing?.diaInicial || 1,
          diaFinal: existing?.diaFinal || 1,
        });
      }
      replaceFaixas(newList);
    }
  }, [quantidadeFaixa, idTipoCondicaoPagto, replaceFaixas]);

  // Resetar abas inativas
  const [activeTab, setActiveTab] = useState(0);

  // Mapear abas disponíveis
  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setActiveTab(newValue);
  };

  // Label dinâmico para dias
  const getDiasLabel = () => {
    if (idParcelamentoTipo === 119) return 'Dia fixo mês';
    if (idParcelamentoTipo === 693) return 'Dia útil mês';
    return 'Dias entre parcelas';
  };

  return (
    <FormProvider {...methods}>
      <form
        id="crud-form"
        noValidate
        onSubmit={handleSubmit(
          (data) => {
            onSave(data);
          },
          (validationErrors) => {
            console.warn('Validação falhou! Erros no formulário:', validationErrors);
            showMessage('Por favor, corrija os erros sinalizados no formulário antes de salvar.', 'error');
          }
        )}
      >
        {/* Botões ocultos para o modal CRUD do CadastroBasePage */}
        <button id="crud-submit-btn" type="submit" style={{ display: 'none' }} />
        <button id="crud-reset-btn" type="button" style={{ display: 'none' }} onClick={() => reset(record)} />

        <Box sx={{ width: '100%' }}>
          <Card variant="outlined" sx={{ mb: 3, p: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom color="primary" sx={{ fontWeight: 'bold' }}>
                Dados Gerais
              </Typography>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 2 }}>
                  <Controller
                    name="idCondicaoPagamento"
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        label="Código"
                        value={field.value || 'Automático'}
                        disabled
                        slotProps={{ input: { readOnly: true } }}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 7 }}>
                  <Controller
                    name="descricao"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        label="Descrição"
                        required
                        disabled={isBrowse}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="ativo"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Switch
                            checked={field.value}
                            onChange={(e) => field.onChange(e.target.checked)}
                            disabled={isBrowse}
                            color="primary"
                          />
                        }
                        label="Ativo"
                        sx={{ mt: 0.5 }}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <FormControl fullWidth size="small" required error={!!errors.idTipoCondicaoPagto}>
                    <InputLabel id="tipo-cond-label">Tipo Condição</InputLabel>
                    <Controller
                      name="idTipoCondicaoPagto"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="tipo-cond-label"
                          {...field}
                          disabled={isBrowse}
                          label="Tipo Condição"
                        >
                          {tipoCondicaoOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                    <FormHelperText>{errors.idTipoCondicaoPagto?.message}</FormHelperText>
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <FormControl fullWidth size="small" required error={!!errors.idDisponibilidade}>
                    <InputLabel id="disp-label">Disponibilidade</InputLabel>
                    <Controller
                      name="idDisponibilidade"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="disp-label"
                          {...field}
                          disabled={isBrowse}
                          label="Disponibilidade"
                        >
                          {disponibilidadeOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                    <FormHelperText>{errors.idDisponibilidade?.message}</FormHelperText>
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <FormControl fullWidth size="small" required error={!!errors.idTipoVencimento}>
                    <InputLabel id="venc-label">Vencimento Dia Útil</InputLabel>
                    <Controller
                      name="idTipoVencimento"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="venc-label"
                          {...field}
                          disabled={isBrowse}
                          label="Vencimento Dia Útil"
                        >
                          {vencimentoTipoOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                    <FormHelperText>{errors.idTipoVencimento?.message}</FormHelperText>
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="grupo-label">Grupo Condição</InputLabel>
                    <Controller
                      name="idGrupoCondicaoPagamento"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="grupo-label"
                          {...field}
                          value={field.value || ''}
                          disabled={isBrowse}
                          label="Grupo Condição"
                        >
                          <MenuItem value="">
                            <em>Nenhum</em>
                          </MenuItem>
                          {grupos.map((g) => (
                            <MenuItem key={g.idGrupoCondicaoPagamento} value={g.idGrupoCondicaoPagamento}>
                              {g.descricao}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="forma-cobranca-label">Forma de Cobrança</InputLabel>
                    <Controller
                      name="idFormaCobranca"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="forma-cobranca-label"
                          {...field}
                          value={field.value || ''}
                          disabled={isBrowse}
                          label="Forma de Cobrança"
                        >
                          <MenuItem value="">
                            <em>Nenhuma</em>
                          </MenuItem>
                          {formasCobranca.map((f) => (
                            <MenuItem key={f.idFormaCobranca} value={f.idFormaCobranca}>
                              {f.descricao}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="forma-pagto-label">Forma de Pagamento</InputLabel>
                    <Controller
                      name="idFormaPagamento"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="forma-pagto-label"
                          {...field}
                          value={field.value || ''}
                          disabled={isBrowse}
                          label="Forma de Pagamento"
                        >
                          <MenuItem value="">
                            <em>Nenhuma</em>
                          </MenuItem>
                          {formasPagamento.map((f) => (
                            <MenuItem key={f.idForma} value={f.idForma}>
                              {f.nome}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 2 }}>
                  <Controller
                    name="ordemConsulta"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label="Ordem de Consulta"
                        required
                        disabled={isBrowse}
                        onChange={(e) => field.onChange(Number(e.target.value))}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 2 }}>
                  <Controller
                    name="utilizarPdv"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => field.onChange(e.target.checked)}
                            disabled={isBrowse}
                            color="primary"
                          />
                        }
                        label="Utilizar no PDV"
                        sx={{ mt: 0.5 }}
                      />
                    )}
                  />
                </Grid>
              </Grid>
            </CardContent>
          </Card>

          <Card variant="outlined" sx={{ mb: 3, p: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom color="primary" sx={{ fontWeight: 'bold' }}>
                Regras de Acréscimo e Desconto
              </Typography>
              <Grid container spacing={3}>
                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="recebeAcrescimo"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                              if (e.target.checked) setValue('recebeDesconto', false);
                            }}
                            disabled={isBrowse}
                          />
                        }
                        label="Recebe Acréscimo"
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="acrescimo"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label="Acréscimo (%)"
                        disabled={isBrowse || !recebeAcrescimo}
                        onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="recebeDesconto"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                              if (e.target.checked) setValue('recebeAcrescimo', false);
                            }}
                            disabled={isBrowse}
                          />
                        }
                        label="Recebe Desconto"
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="desconto"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label="Desconto (%)"
                        disabled={isBrowse || !recebeDesconto}
                        onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>
              </Grid>
            </CardContent>
          </Card>

          {/* Abas dinâmicas baseadas no Tipo da Condição */}
          <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
            <Tabs value={activeTab} onChange={handleTabChange} aria-label="Abas da Condição de Pagamento">
              {idTipoCondicaoPagto === 36 && <Tab label="Aba 1: Parcelamento" />}
              {idTipoCondicaoPagto === 37 && <Tab label="Aba 2: Faixas" />}
              {idTipoCondicaoPagto === 38 && <Tab label="Aba 3: Semanal" />}
            </Tabs>
          </Box>

          {/* Conteúdo da Aba 1: Parcelamento */}
          {idTipoCondicaoPagto === 36 && activeTab === 0 && (
            <Paper variant="outlined" sx={{ p: 3 }}>
              <Grid container spacing={2} sx={{ mb: 3 }}>
                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="alteraParcelas"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                              if (e.target.checked) {
                                setValue('quantidadeParcela', 1);
                                setValue('tipoDivisaoParcelamento', 604); // Quantidade
                                setValue('idParcelaArredondamento', 47); // Última
                              }
                            }}
                            disabled={isBrowse}
                          />
                        }
                        label="Condição do tipo livre (Livre)"
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="alteraNroParcela"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => field.onChange(e.target.checked)}
                            disabled={isBrowse}
                          />
                        }
                        label="Permitir alterar nº parcela"
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="arred-label">Arredondamento da Parcela</InputLabel>
                    <Controller
                      name="idParcelaArredondamento"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="arred-label"
                          {...field}
                          disabled={isBrowse || alteraParcelas}
                          label="Arredondamento da Parcela"
                        >
                          {arredondamentoOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="parc-tipo-label">Tipo de Parcelamento</InputLabel>
                    <Controller
                      name="idParcelamentoTipo"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="parc-tipo-label"
                          {...field}
                          disabled={isBrowse}
                          label="Tipo de Parcelamento"
                        >
                          {parcelamentoTipoOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="divisao-label">Tipo de Divisão</InputLabel>
                    <Controller
                      name="tipoDivisaoParcelamento"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="divisao-label"
                          {...field}
                          disabled={isBrowse || alteraParcelas}
                          label="Tipo de Divisão"
                        >
                          {divisaoTipoOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="quantidadeParcela"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label="Quantidade de Parcelas"
                        disabled={isBrowse || alteraParcelas}
                        onChange={(e) => field.onChange(Math.max(0, Number(e.target.value)))}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 3 }}>
                  <Controller
                    name="diasParcelamento"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label={getDiasLabel()}
                        disabled={isBrowse}
                        onChange={(e) => field.onChange(Number(e.target.value))}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="usarMesComercial"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => field.onChange(e.target.checked)}
                            disabled={isBrowse || idParcelamentoTipo !== 120}
                          />
                        }
                        label="Usar mês comercial?"
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="diasMinimoProximoMes"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label="Dias limite para mesmo mês"
                        disabled={isBrowse || idParcelamentoTipo !== 120}
                        onChange={(e) => field.onChange(Number(e.target.value))}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="primeiraParcelaAVista"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                              if (e.target.checked) setValue('obrigatorioFormaPagamento', true);
                            }}
                            disabled={isBrowse || idParcelamentoTipo !== 120}
                          />
                        }
                        label="1ª Parcela à vista"
                      />
                    )}
                  />
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <FormControl fullWidth size="small">
                    <InputLabel id="forma-vista-label">Forma de pagamento padrão da 1ª parcela</InputLabel>
                    <Controller
                      name="idFormaPagamentoVista"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="forma-vista-label"
                          {...field}
                          value={field.value || ''}
                          disabled={isBrowse || !primeiraParcelaAVista}
                          label="Forma de pagamento padrão da 1ª parcela"
                        >
                          <MenuItem value="">
                            <em>Nenhuma</em>
                          </MenuItem>
                          {formasPagamento.map((f) => (
                            <MenuItem key={f.idForma} value={f.idForma}>
                              {f.nome}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>

                <Grid size={{ xs: 12, sm: 6 }}>
                  <Controller
                    name="obrigatorioFormaPagamento"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={
                          <Checkbox
                            checked={field.value}
                            onChange={(e) => field.onChange(e.target.checked)}
                            disabled={isBrowse || !primeiraParcelaAVista}
                          />
                        }
                        label="Obrigatório liquidar a 1ª parcela"
                        sx={{ mt: 0.5 }}
                      />
                    )}
                  />
                </Grid>
              </Grid>

              <Divider sx={{ my: 3 }} />

              <Typography variant="subtitle1" sx={{ fontWeight: 'bold', mb: 2 }} color="textSecondary">
                Configuração da Grade de Parcelas
              </Typography>

              {errors.parceladas && (
                <Alert severity="error" sx={{ mb: 2 }}>{errors.parceladas.message || 'Erro de validação na grade.'}</Alert>
              )}

              <TableContainer component={Paper} variant="outlined">
                <Table size="small">
                  <TableHead sx={{ bgcolor: 'action.hover' }}>
                    <TableRow>
                      <TableCell width={80}><strong>Nº Parcela</strong></TableCell>
                      <TableCell><strong>Dias</strong></TableCell>
                      <TableCell><strong>Liberado</strong></TableCell>
                      <TableCell><strong>% Divisão</strong></TableCell>
                      <TableCell><strong>% Mínimo</strong></TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {parcelasFields.map((field, index) => (
                      <TableRow key={field.id}>
                        <TableCell>{index + 1}</TableCell>
                        <TableCell>
                          <Controller
                            name={`parceladas.${index}.numeroDias`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse}
                                onChange={(e) => inputField.onChange(Number(e.target.value))}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                        <TableCell>
                          <Controller
                            name={`parceladas.${index}.diasLiberado`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse || alteraParcelas}
                                onChange={(e) => inputField.onChange(Number(e.target.value))}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                        <TableCell>
                          <Controller
                            name={`parceladas.${index}.percentualDivisao`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse || alteraParcelas || tipoDivisaoParcelamento === 604}
                                onChange={(e) => inputField.onChange(parseFloat(e.target.value) || 0)}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                        <TableCell>
                          <Controller
                            name={`parceladas.${index}.percentualValorMinimo`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse}
                                onChange={(e) => inputField.onChange(parseFloat(e.target.value) || 0)}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                      </TableRow>
                    ))}
                    {parcelasFields.length === 0 && (
                      <TableRow>
                        <TableCell colSpan={5} align="center" sx={{ py: 3, color: 'text.secondary' }}>
                          Nenhuma parcela cadastrada. Altere a 'Quantidade de Parcelas' acima para gerar.
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          )}

          {/* Conteúdo da Aba 2: Faixas */}
          {idTipoCondicaoPagto === 37 && activeTab === 0 && (
            <Paper variant="outlined" sx={{ p: 3 }}>
              <Grid container spacing={2} sx={{ mb: 3 }}>
                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="quantidadeFaixa"
                    control={control}
                    render={({ field, fieldState: { error } }) => (
                      <TextField
                        {...field}
                        fullWidth
                        size="small"
                        type="number"
                        label="Quantidade de Faixas"
                        disabled={isBrowse}
                        onChange={(e) => field.onChange(Math.max(0, Number(e.target.value)))}
                        error={!!error}
                        helperText={error?.message}
                      />
                    )}
                  />
                </Grid>
              </Grid>

              {errors.faixas && (
                <Alert severity="error" sx={{ mb: 2 }}>{errors.faixas.message || 'Erro de validação na grade.'}</Alert>
              )}

              <TableContainer component={Paper} variant="outlined">
                <Table size="small">
                  <TableHead sx={{ bgcolor: 'action.hover' }}>
                    <TableRow>
                      <TableCell width={80}><strong>Faixa Nº</strong></TableCell>
                      <TableCell><strong>Dia Inicial</strong></TableCell>
                      <TableCell><strong>Dia Final</strong></TableCell>
                      <TableCell><strong>Dia Vencimento</strong></TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {faixasFields.map((field, index) => (
                      <TableRow key={field.id}>
                        <TableCell>{index + 1}</TableCell>
                        <TableCell>
                          <Controller
                            name={`faixas.${index}.diaInicial`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse}
                                onChange={(e) => inputField.onChange(Number(e.target.value))}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                        <TableCell>
                          <Controller
                            name={`faixas.${index}.diaFinal`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse}
                                onChange={(e) => inputField.onChange(Number(e.target.value))}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                        <TableCell>
                          <Controller
                            name={`faixas.${index}.numeroDias`}
                            control={control}
                            render={({ field: inputField, fieldState: { error } }) => (
                              <TextField
                                {...inputField}
                                size="small"
                                type="number"
                                error={!!error}
                                disabled={isBrowse}
                                onChange={(e) => inputField.onChange(Number(e.target.value))}
                                sx={{ width: 100 }}
                              />
                            )}
                          />
                        </TableCell>
                      </TableRow>
                    ))}
                    {faixasFields.length === 0 && (
                      <TableRow>
                        <TableCell colSpan={4} align="center" sx={{ py: 3, color: 'text.secondary' }}>
                          Nenhuma faixa cadastrada. Altere a 'Quantidade de Faixas' acima para gerar.
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Paper>
          )}

          {/* Conteúdo da Aba 3: Semanal */}
          {idTipoCondicaoPagto === 38 && activeTab === 0 && (
            <Paper variant="outlined" sx={{ p: 3 }}>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth size="small" error={!!errors.idDiaSemana}>
                    <InputLabel id="dia-semana-label">Dia da Semana</InputLabel>
                    <Controller
                      name="idDiaSemana"
                      control={control}
                      render={({ field }) => (
                        <Select
                          labelId="dia-semana-label"
                          {...field}
                          disabled={isBrowse}
                          label="Dia da Semana"
                        >
                          {diaSemanaOptions.map((opt) => (
                            <MenuItem key={opt.value} value={opt.value}>
                              {opt.label}
                            </MenuItem>
                          ))}
                        </Select>
                      )}
                    />
                    <FormHelperText>{errors.idDiaSemana?.message}</FormHelperText>
                  </FormControl>
                </Grid>
              </Grid>
            </Paper>
          )}
        </Box>
      </form>

      <Snackbar
        open={toast.open}
        autoHideDuration={4000}
        onClose={() => setToast((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setToast((prev) => ({ ...prev, open: false }))}
          severity={toast.severity}
          variant="filled"
          sx={{ width: '100%' }}
        >
          {toast.message}
        </Alert>
      </Snackbar>
    </FormProvider>
  );
};

// Componente Exportado principal
export const FCondicaoPagamento: React.FC = () => {
  return (
    <CadastroBasePage<ICondicaoPagamentoForm>
      config={new CondicaoPagamentoCadastroConfig()}
      initialRecords={[]}
      renderForm={(mode, record, onSave) => (
        <CondicaoPagamentoFormView mode={mode} record={record} onSave={onSave} />
      )}
    />
  );
};
export default FCondicaoPagamento;

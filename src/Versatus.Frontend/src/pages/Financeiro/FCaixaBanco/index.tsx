import React, { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Snackbar,
  Stack,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tabs,
  TextField,
  Typography,
  Switch,
} from '@mui/material';
import { Trash2 } from 'lucide-react';
import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEnumOptions } from '../../../hooks/useEnums';
import { useCurrentTabId, useTabs } from '../../../context/TabsContext';
import { buildApiEndpoint, getApiHeaders } from '../../../config/api';
import { CadastroBasePage } from '../../../components/crud/CadastroBasePage';
import type { CadastroModalMode } from '../../../types/cadastro';
import { CaixaBancoCadastroConfig } from './CaixaBancoCadastroConfig';
import { LookupField } from './LookupField';
import type { IItemLookup } from './LookupField';
import { caixaBancoSchema, MSG } from './schema';
import { CONTA_TIPO, TIPO_CONTA_BANCARIA, TIPO_CONTA_CAIXA } from './types';
import type { ICaixaBancoForm } from './types';

// Tela Caixa/Banco — docs/spec_fcaixabanco.md v1.0 (E3-T07). IDs UI-xx / VAL-E3-xx: spec §6.

const LOOKUP = {
  agencias: '/api/financeiro/lookups/agencias',
  usuarios: '/api/financeiro/lookups/usuarios',
  instituicoes: '/api/financeiro/lookups/instituicoes-financeiras',
  planosContabeis: '/api/financeiro/lookups/planos-contabeis',
  contasCorrentes: '/api/financeiro/lookups/contas-correntes',
};

const MSG_SEM_USUARIOS = // FCaixaBanco.cs:3009 (LanguageManager item 1) — UI-04a
  'Está configurado para controlar caixa/banco por usuário, sendo necessário informar qual(is) usuário(s) ' +
  'terá(ão) acesso a esta conta. Deseja salvar este caixa/banco sem informar usuário(s)?';

const MSG_VINCULADA_CORRENTE = // FCaixaBanco.cs:2431 — UI-12
  "Para conta vinculada, deve ser informada uma conta bancária do tipo 'Conta corrente'.";

/** Valor booleano de um parâmetro do sistema (GET /api/parametro/{chave}); ausente = false. */
const useParametroBool = (chave: string): boolean => {
  const [valor, setValor] = useState(false);
  useEffect(() => {
    let ativo = true;
    fetch(buildApiEndpoint(`/api/parametro/${chave}`), { headers: getApiHeaders() })
      .then((r) => (r.ok ? r.json() : null))
      .then((d) => ativo && setValor(String(d?.valor ?? '').toLowerCase() === 'true'))
      .catch(() => ativo && setValor(false));
    return () => {
      ativo = false;
    };
  }, [chave]);
  return valor;
};

const numeroOuNull = (v: string): number | null => (v.trim() === '' || Number.isNaN(Number(v)) ? null : Number(v));

export interface ICaixaBancoFormViewProps {
  mode: CadastroModalMode;
  record: ICaixaBancoForm;
  onSave: (data: ICaixaBancoForm) => void;
}

export const CaixaBancoFormView: React.FC<ICaixaBancoFormViewProps> = ({ mode, record, onSave }) => {
  const [toast, setToast] = useState<{ open: boolean; message: string; severity: 'success' | 'error' | 'info' }>({
    open: false,
    message: '',
    severity: 'info',
  });
  const avisar = (message: string, severity: 'success' | 'error' | 'info' = 'info') => setToast({ open: true, message, severity });

  const edicao = mode === 'edit';
  const somenteLeitura = mode === 'delete' || mode === 'view';

  const vinculaCaixaBancoUsuario = useParametroBool('VINCULACAIXABANCOUSUARIO'); // UI-04
  const habilitaContabil = useParametroBool('HABILITAPROCESSOCONTABILIZACAO'); // UI-15

  const { options: optTipoConta } = useEnumOptions(174);
  const tipoContaOptions = optTipoConta?.length ? optTipoConta : [
    { value: CONTA_TIPO.Caixa, label: 'Caixa' },
    { value: CONTA_TIPO.Banco, label: 'Banco' },
  ];
  const { options: optTipoCaixa } = useEnumOptions(1481);
  const tipoCaixaOptions = optTipoCaixa?.length ? optTipoCaixa : [
    { value: TIPO_CONTA_CAIXA.Normal, label: 'Normal' },
    { value: TIPO_CONTA_CAIXA.Cofre, label: 'Cofre' },
  ];
  const { options: optTipoBancaria } = useEnumOptions(1478);
  const tipoBancariaOptions = optTipoBancaria?.length ? optTipoBancaria : [
    { value: TIPO_CONTA_BANCARIA.ContaCorrente, label: 'Conta corrente' },
    { value: TIPO_CONTA_BANCARIA.Investimento, label: 'Investimento' },
  ];

  const methods = useForm<ICaixaBancoForm>({
    resolver: zodResolver(caixaBancoSchema) as any,
    defaultValues: record,
    mode: 'onChange',
  });
  const { control, handleSubmit, reset, watch, setValue, getValues, formState: { errors, isDirty } } = methods;

  const { marcarDirty } = useTabs();
  const tabId = useCurrentTabId();
  useEffect(() => {
    if (tabId && !somenteLeitura) marcarDirty(tabId, isDirty);
  }, [isDirty, tabId, somenteLeitura, marcarDirty]);

  useEffect(() => {
    reset(record);
  }, [record, reset]);

  const [abaAtiva, setAbaAtiva] = useState(0);
  const [pendenteSemUsuarios, setPendenteSemUsuarios] = useState<ICaixaBancoForm | null>(null);

  const idTipoConta = watch('idTipoConta');
  const tipoBancaria = watch('contaBancaria.idTipoContaBancaria');
  const contaTerceiro = watch('contaBancaria.contaTerceiro');
  const enviarSped = watch('contaBancaria.enviarSped');
  const usuarios = watch('usuarios');

  const ehBanco = idTipoConta === CONTA_TIPO.Banco;
  const ehCorrente = tipoBancaria === TIPO_CONTA_BANCARIA.ContaCorrente;
  const ehInvestimento = tipoBancaria === TIPO_CONTA_BANCARIA.Investimento;
  const tipoEscolhido = idTipoConta != null; // UI-01
  const tipoBancariaSalvo = edicao && record.contaBancaria.idTipoContaBancaria != null; // UI-06

  // Abas visíveis (UI-05, UI-15)
  const abas = [
    { chave: 'geral', rotulo: 'Geral', visivel: true },
    { chave: 'conta', rotulo: 'Conta bancária', visivel: ehBanco },
    { chave: 'contabil', rotulo: 'Contábil', visivel: habilitaContabil },
  ].filter((a) => a.visivel);
  const abaAtual = abas[abaAtiva]?.chave ?? 'geral';
  useEffect(() => {
    if (abaAtiva >= abas.length) setAbaAtiva(0);
  }, [abas.length, abaAtiva]);

  const trocarAba = (_: React.SyntheticEvent, nova: number) => {
    // UI-07 — FCaixaBanco.cs:2439-2462: aba Conta bancária exige o tipo da conta bancária.
    if (abas[nova]?.chave === 'conta' && getValues('contaBancaria.idTipoContaBancaria') == null) {
      avisar(MSG.tipoContaBancaria);
      setAbaAtiva(0);
      return;
    }
    setAbaAtiva(nova);
  };

  // UI-03 — Caixa assume tipo de caixa Normal; Banco não tem tipo de caixa.
  const aoTrocarTipoConta = (novo: number) => {
    setValue('idTipoConta', novo, { shouldDirty: true, shouldValidate: true });
    setValue('idTipoContaCaixa', novo === CONTA_TIPO.Caixa ? TIPO_CONTA_CAIXA.Normal : null, { shouldDirty: true });
  };

  // UI-16 — ContaBancaria.cs:1347-1375.
  const aoTrocarTipoBancaria = (novo: number) => {
    setValue('contaBancaria.idTipoContaBancaria', novo, { shouldDirty: true, shouldValidate: true });
    setValue('contaBancaria.idContaBancariaVinculada', null, { shouldDirty: true });
    if (novo === TIPO_CONTA_BANCARIA.Investimento) {
      setValue('contaBancaria.idAgencia', null, { shouldDirty: true });
      setValue('contaBancaria.numeroConta', '', { shouldDirty: true });
      setValue('contaBancaria.digitoConta', '', { shouldDirty: true });
      setValue('contaBancaria.limite', 0, { shouldDirty: true });
      setValue('contaBancaria.contaTerceiro', false, { shouldDirty: true });
      setValue('contaBancaria.titular', '', { shouldDirty: true });
      setValue('contaBancaria.cpfCnpj', '', { shouldDirty: true });
      setValue('contaBancaria.permiteEmitirCheque', false, { shouldDirty: true });
    }
  };

  // UI-09 — desmarcar conta de terceiro limpa titular e CPF/CNPJ.
  const aoMarcarTerceiro = (marcado: boolean) => {
    setValue('contaBancaria.contaTerceiro', marcado, { shouldDirty: true, shouldValidate: true });
    if (!marcado) {
      setValue('contaBancaria.titular', '', { shouldDirty: true, shouldValidate: true });
      setValue('contaBancaria.cpfCnpj', '', { shouldDirty: true, shouldValidate: true });
    }
  };

  // UI-13 — desmarcar Enviar SPED limpa a instituição financeira.
  const aoMarcarSped = (marcado: boolean) => {
    setValue('contaBancaria.enviarSped', marcado, { shouldDirty: true, shouldValidate: true });
    if (!marcado) setValue('contaBancaria.idInstituicaoFinanceira', null, { shouldDirty: true, shouldValidate: true });
  };

  // UI-17 — escolher a conta vinculada copia os dados bancários dela.
  const aoEscolherVinculada = async (id: number | null) => {
    setValue('contaBancaria.idContaBancariaVinculada', id, { shouldDirty: true, shouldValidate: true });
    if (id == null) return;
    try {
      const lista = await fetch(
        `${buildApiEndpoint('/api/financeiro/caixa-banco/paginado')}?texto=${id}&idTipoConta=${CONTA_TIPO.Banco}&limit=50`,
        { headers: getApiHeaders() },
      ).then((r) => r.json());
      const item = (lista.items ?? []).find((i: any) => i.idCaixaBanco === id);
      if (!item) return;
      const conta = await fetch(buildApiEndpoint(`/api/financeiro/conta-bancaria/${item.idFilial}/${id}`), {
        headers: getApiHeaders(),
      }).then((r) => (r.ok ? r.json() : null));
      if (!conta) return;
      if (conta.idTipoContaBancaria !== TIPO_CONTA_BANCARIA.ContaCorrente) {
        avisar(MSG_VINCULADA_CORRENTE, 'error'); // UI-12
        setValue('contaBancaria.idContaBancariaVinculada', null, { shouldDirty: true, shouldValidate: true });
        return;
      }
      setValue('contaBancaria.idAgencia', conta.idAgencia || null, { shouldDirty: true });
      setValue('contaBancaria.numeroConta', conta.numeroConta ?? '', { shouldDirty: true });
      setValue('contaBancaria.digitoConta', conta.digitoConta ?? '', { shouldDirty: true });
      setValue('contaBancaria.limite', conta.limite ?? 0, { shouldDirty: true });
      setValue('contaBancaria.contaTerceiro', conta.contaTerceiro ?? false, { shouldDirty: true });
      setValue('contaBancaria.titular', conta.titular ?? '', { shouldDirty: true });
      setValue('contaBancaria.cpfCnpj', conta.cpfCnpj ?? '', { shouldDirty: true });
    } catch {
      // O backend repete a cópia ao salvar (VAL-E3-26).
    }
  };

  // Grade de usuários — adicionar pela busca; linha salva não troca de usuário (VAL-E3-12).
  const adicionarUsuario = (item: IItemLookup | null) => {
    if (!item) return;
    const atuais = getValues('usuarios');
    if (atuais.some((u) => u.idUsuario === item.id)) {
      avisar(MSG.usuarioJaInformado, 'error'); // VAL-E3-11
      return;
    }
    setValue('usuarios', [...atuais, { idUsuario: item.id, idUsuarioSalvo: null, descricao: item.descricao }], {
      shouldDirty: true,
      shouldValidate: true,
    });
  };
  const removerUsuario = (idUsuario: number) =>
    setValue('usuarios', getValues('usuarios').filter((u) => u.idUsuario !== idUsuario), { shouldDirty: true, shouldValidate: true });
  const [chaveNovoUsuario, setChaveNovoUsuario] = useState(0);

  const enviar = (dados: ICaixaBancoForm) => {
    // UI-04a — FCaixaBanco.cs:2412-2421.
    if (vinculaCaixaBancoUsuario && dados.usuarios.length === 0) {
      setPendenteSemUsuarios(dados);
      return;
    }
    onSave(dados);
  };

  const erroConta = errors.contaBancaria;

  return (
    <>
      <form
        id="crud-form"
        noValidate
        onSubmit={handleSubmit(enviar, () =>
          avisar('Por favor, corrija os erros sinalizados no formulário antes de salvar.', 'error'),
        )}
      >
        <button id="crud-submit-btn" type="submit" style={{ display: 'none' }} />
        <button id="crud-reset-btn" type="button" style={{ display: 'none' }} onClick={() => reset(record)} />

        {pendenteSemUsuarios && (
          <Alert
            severity="warning"
            variant="outlined"
            sx={{ mb: 2, alignItems: 'center' }}
            action={
              <Stack direction="row" spacing={1}>
                <Button
                  size="small"
                  variant="contained"
                  color="warning"
                  onClick={() => {
                    const dados = pendenteSemUsuarios;
                    setPendenteSemUsuarios(null);
                    onSave(dados);
                  }}
                >
                  Salvar sem usuários
                </Button>
                <Button size="small" variant="outlined" color="inherit" onClick={() => setPendenteSemUsuarios(null)}>
                  Voltar
                </Button>
              </Stack>
            }
          >
            {MSG_SEM_USUARIOS}
          </Alert>
        )}

        {/* Cabeçalho — Dados gerais */}
        <Card variant="outlined" sx={{ mb: 3 }}>
          <CardContent>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 2 }}>
                <TextField
                  fullWidth
                  size="small"
                  variant="outlined"
                  label="Código"
                  value={record.idCaixaBanco || 'Automático'}
                  disabled
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 8 }}>
                <Controller
                  name="descricao"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      label="Descrição"
                      required
                      disabled={somenteLeitura}
                      slotProps={{ htmlInput: { maxLength: 100 } }}
                      error={!!errors.descricao}
                      helperText={errors.descricao?.message}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 2 }} sx={{ display: 'flex', alignItems: 'center' }}>
                <Controller
                  name="ativo"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Switch checked={field.value} onChange={(e) => field.onChange(e.target.checked)} disabled={somenteLeitura} />}
                      label={field.value ? 'Ativo' : 'Inativo'}
                      sx={{ color: field.value ? 'success.main' : 'error.main' }}
                    />
                  )}
                />
              </Grid>

              {/* UI-02 — Tipo bloqueado depois de salvo. */}
              <Grid size={{ xs: 12, sm: 4 }}>
                <FormControl fullWidth size="small" variant="outlined" required error={!!errors.idTipoConta}>
                  <InputLabel id="tipo-conta-label">Tipo conta</InputLabel>
                  <Select
                    labelId="tipo-conta-label"
                    label="Tipo conta"
                    value={idTipoConta ?? ''}
                    disabled={somenteLeitura || edicao}
                    onChange={(e) => aoTrocarTipoConta(Number(e.target.value))}
                  >
                    {tipoContaOptions.map((o) => (
                      <MenuItem key={o.value} value={o.value}>
                        {o.label}
                      </MenuItem>
                    ))}
                  </Select>
                  <FormHelperText>{errors.idTipoConta?.message}</FormHelperText>
                </FormControl>
              </Grid>

              {/* UI-03 — Tipo conta caixa (só Caixa; bloqueado depois de salvo). */}
              {idTipoConta === CONTA_TIPO.Caixa && (
                <Grid size={{ xs: 12, sm: 4 }}>
                  <Controller
                    name="idTipoContaCaixa"
                    control={control}
                    render={({ field }) => (
                      <FormControl fullWidth size="small" variant="outlined">
                        <InputLabel id="tipo-caixa-label">Tipo conta caixa</InputLabel>
                        <Select
                          labelId="tipo-caixa-label"
                          label="Tipo conta caixa"
                          value={field.value ?? ''}
                          disabled={somenteLeitura || edicao}
                          onChange={(e) => field.onChange(Number(e.target.value))}
                        >
                          {tipoCaixaOptions.map((o) => (
                            <MenuItem key={o.value} value={o.value}>
                              {o.label}
                            </MenuItem>
                          ))}
                        </Select>
                      </FormControl>
                    )}
                  />
                </Grid>
              )}

              {/* Tipo conta bancária (só Banco; bloqueado depois de salvo — UI-06). */}
              {ehBanco && (
                <Grid size={{ xs: 12, sm: 4 }}>
                  <FormControl fullWidth size="small" variant="outlined" required error={!!erroConta?.idTipoContaBancaria}>
                    <InputLabel id="tipo-bancaria-label">Tipo conta bancária</InputLabel>
                    <Select
                      labelId="tipo-bancaria-label"
                      label="Tipo conta bancária"
                      value={tipoBancaria ?? ''}
                      disabled={somenteLeitura || tipoBancariaSalvo}
                      onChange={(e) => aoTrocarTipoBancaria(Number(e.target.value))}
                    >
                      {tipoBancariaOptions.map((o) => (
                        <MenuItem key={o.value} value={o.value}>
                          {o.label}
                        </MenuItem>
                      ))}
                    </Select>
                    <FormHelperText>{erroConta?.idTipoContaBancaria?.message}</FormHelperText>
                  </FormControl>
                </Grid>
              )}

              <Grid size={{ xs: 12, sm: 4 }} sx={{ display: 'flex', alignItems: 'center' }}>
                <Controller
                  name="entraFluxoCaixa"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Checkbox checked={field.value} onChange={(e) => field.onChange(e.target.checked)} disabled={somenteLeitura} />}
                      label="Entra na previsão / fluxo de caixa"
                    />
                  )}
                />
              </Grid>
            </Grid>
          </CardContent>
        </Card>

        {/* Abas — desabilitadas até escolher o Tipo (UI-01). */}
        <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 2 }}>
          <Tabs value={abaAtiva} onChange={trocarAba}>
            {abas.map((a) => (
              <Tab key={a.chave} label={a.rotulo} disabled={!tipoEscolhido} />
            ))}
          </Tabs>
        </Box>

        {tipoEscolhido && abaAtual === 'geral' && vinculaCaixaBancoUsuario && (
          // UI-04 — Usuários autorizados (só com VinculaCaixaBancoUsuario).
          <Paper variant="outlined" sx={{ p: 2 }}>
            <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
              Usuários com acesso a esta conta
            </Typography>
            {!somenteLeitura && (
              <Box sx={{ maxWidth: 420, mb: 2 }}>
                <LookupField
                  key={chaveNovoUsuario}
                  label="Adicionar usuário"
                  endpoint={LOOKUP.usuarios}
                  value={null}
                  onChange={(_, item) => {
                    adicionarUsuario(item);
                    setChaveNovoUsuario((k) => k + 1);
                  }}
                />
              </Box>
            )}
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Usuário</TableCell>
                    <TableCell width={80} />
                  </TableRow>
                </TableHead>
                <TableBody>
                  {usuarios.map((u, i) => (
                    <TableRow key={u.idUsuario}>
                      <TableCell>
                        {/* Linha já salva não troca de usuário (VAL-E3-12): só exibe. */}
                        <LookupField label="" endpoint={LOOKUP.usuarios} value={u.idUsuario} onChange={() => undefined} disabled />
                        {errors.usuarios?.[i]?.idUsuario && (
                          <FormHelperText error>{errors.usuarios[i]?.idUsuario?.message}</FormHelperText>
                        )}
                      </TableCell>
                      <TableCell align="right">
                        <IconButton size="small" color="error" disabled={somenteLeitura} onClick={() => removerUsuario(u.idUsuario)}>
                          <Trash2 size={16} />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                  {usuarios.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={2} align="center" sx={{ py: 3, color: 'text.secondary' }}>
                        Nenhum usuário informado.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        )}

        {tipoEscolhido && abaAtual === 'geral' && !vinculaCaixaBancoUsuario && (
          <Typography variant="body2" color="text.secondary" sx={{ px: 1 }}>
            O controle de caixa/banco por usuário está desligado (parâmetro "Vincular caixa/banco por usuário").
          </Typography>
        )}

        {abaAtual === 'conta' && ehBanco && (
          <Paper variant="outlined" sx={{ p: 3 }}>
            <Grid container spacing={2}>
              {/* UI-08 — dados da conta só editáveis em Conta corrente. */}
              <Grid size={{ xs: 12, sm: 6 }}>
                <LookupField
                  label="Agência"
                  endpoint={LOOKUP.agencias}
                  value={watch('contaBancaria.idAgencia')}
                  onChange={(id) => setValue('contaBancaria.idAgencia', id, { shouldDirty: true, shouldValidate: true })}
                  required={ehCorrente}
                  disabled={somenteLeitura || !ehCorrente}
                  error={!!erroConta?.idAgencia}
                  helperText={erroConta?.idAgencia?.message}
                />
              </Grid>
              <Grid size={{ xs: 8, sm: 4 }}>
                <Controller
                  name="contaBancaria.numeroConta"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      label="Número da conta"
                      required={ehCorrente}
                      disabled={somenteLeitura || !ehCorrente}
                      slotProps={{ htmlInput: { maxLength: 15 } }}
                      error={!!erroConta?.numeroConta}
                      helperText={erroConta?.numeroConta?.message}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 4, sm: 2 }}>
                <Controller
                  name="contaBancaria.digitoConta"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      label="Dígito"
                      disabled={somenteLeitura || !ehCorrente}
                      slotProps={{ htmlInput: { maxLength: 2 } }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <Controller
                  name="contaBancaria.limite"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      fullWidth
                      size="small"
                      variant="outlined"
                      type="number"
                      label="Limite"
                      value={field.value ?? ''}
                      onChange={(e) => field.onChange(numeroOuNull(e.target.value))}
                      disabled={somenteLeitura || !ehCorrente}
                    />
                  )}
                />
              </Grid>

              {/* UI-10 — Permite emitir cheque só em Conta corrente. */}
              {ehCorrente && (
                <Grid size={{ xs: 12, sm: 4 }} sx={{ display: 'flex', alignItems: 'center' }}>
                  <Controller
                    name="contaBancaria.permiteEmitirCheque"
                    control={control}
                    render={({ field }) => (
                      <FormControlLabel
                        control={<Checkbox checked={field.value} onChange={(e) => field.onChange(e.target.checked)} disabled={somenteLeitura} />}
                        label="Permite emitir cheque"
                      />
                    )}
                  />
                </Grid>
              )}

              {/* UI-11/12 — Conta vinculada só em Investimento; só lista Conta corrente. */}
              {ehInvestimento && (
                <Grid size={{ xs: 12, sm: 6 }}>
                  <LookupField
                    label="Conta vinculada"
                    endpoint={LOOKUP.contasCorrentes}
                    value={watch('contaBancaria.idContaBancariaVinculada')}
                    onChange={(id) => aoEscolherVinculada(id)}
                    required
                    disabled={somenteLeitura}
                    error={!!erroConta?.idContaBancariaVinculada}
                    helperText={erroConta?.idContaBancariaVinculada?.message}
                  />
                </Grid>
              )}

              {/* Conta de terceiro — VAL-E3-17 / UI-09. */}
              <Grid size={{ xs: 12 }}>
                <Paper variant="outlined" sx={{ p: 2 }}>
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={contaTerceiro}
                        onChange={(e) => aoMarcarTerceiro(e.target.checked)}
                        disabled={somenteLeitura || !ehCorrente}
                      />
                    }
                    label="Conta de terceiro"
                  />
                  {!!erroConta?.contaTerceiro && <FormHelperText error>{erroConta.contaTerceiro.message}</FormHelperText>}
                  <Grid container spacing={2} sx={{ mt: 0.5 }}>
                    <Grid size={{ xs: 12, sm: 8 }}>
                      <Controller
                        name="contaBancaria.titular"
                        control={control}
                        render={({ field }) => (
                          <TextField
                            {...field}
                            fullWidth
                            size="small"
                            variant="outlined"
                            label="Titular"
                            required={contaTerceiro}
                            disabled={somenteLeitura || !contaTerceiro || !ehCorrente}
                            slotProps={{ htmlInput: { maxLength: 50 } }}
                            error={!!erroConta?.titular}
                            helperText={erroConta?.titular?.message}
                          />
                        )}
                      />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 4 }}>
                      <Controller
                        name="contaBancaria.cpfCnpj"
                        control={control}
                        render={({ field }) => (
                          <TextField
                            {...field}
                            onChange={(e) => field.onChange(e.target.value.replace(/\D/g, ''))}
                            fullWidth
                            size="small"
                            variant="outlined"
                            label="CPF/CNPJ titular"
                            required={contaTerceiro}
                            disabled={somenteLeitura || !contaTerceiro || !ehCorrente}
                            slotProps={{ htmlInput: { maxLength: 14, inputMode: 'numeric' } }}
                            error={!!erroConta?.cpfCnpj}
                            helperText={erroConta?.cpfCnpj?.message}
                          />
                        )}
                      />
                    </Grid>
                  </Grid>
                </Paper>
              </Grid>

              {/* SPED — VAL-E3-02..05 / UI-13 / UI-14. */}
              <Grid size={{ xs: 12 }}>
                <Paper variant="outlined" sx={{ p: 2 }}>
                  <FormControlLabel
                    control={<Checkbox checked={enviarSped} onChange={(e) => aoMarcarSped(e.target.checked)} disabled={somenteLeitura} />}
                    label="Enviar SPED (Bloco 1601)"
                  />
                  <Box sx={{ mt: 1, maxWidth: 520 }}>
                    <LookupField
                      label="Instituição financeira fiscal"
                      endpoint={LOOKUP.instituicoes}
                      value={watch('contaBancaria.idInstituicaoFinanceira')}
                      onChange={(id) => setValue('contaBancaria.idInstituicaoFinanceira', id, { shouldDirty: true, shouldValidate: true })}
                      required={enviarSped}
                      disabled={somenteLeitura || !enviarSped}
                      error={!!erroConta?.idInstituicaoFinanceira}
                      helperText={erroConta?.idInstituicaoFinanceira?.message}
                    />
                  </Box>
                </Paper>
              </Grid>

              {/* Mantidos pelos movimentos — só exibição. */}
              {(['creditoPendente', 'debitoPendente', 'chequePendente'] as const).map((campo) => (
                <Grid key={campo} size={{ xs: 12, sm: 4 }}>
                  <TextField
                    fullWidth
                    size="small"
                    variant="outlined"
                    label={{ creditoPendente: 'Crédito pendente', debitoPendente: 'Débito pendente', chequePendente: 'Cheque pendente' }[campo]}
                    value={watch(`contaBancaria.${campo}`) ?? 0}
                    disabled
                  />
                </Grid>
              ))}
            </Grid>
          </Paper>
        )}

        {abaAtual === 'contabil' && (
          // UI-15 — Aba Contábil (só com HabilitaProcessoContabilizacao); plano contábil analítico.
          <Paper variant="outlined" sx={{ p: 3 }}>
            <Box sx={{ maxWidth: 520 }}>
              <LookupField
                label="Plano contábil"
                endpoint={LOOKUP.planosContabeis}
                value={watch('idPlanoContabil')}
                onChange={(id) => setValue('idPlanoContabil', id, { shouldDirty: true })}
                disabled={somenteLeitura}
              />
            </Box>
          </Paper>
        )}
      </form>

      <Snackbar
        open={toast.open}
        autoHideDuration={5000}
        onClose={() => setToast((t) => ({ ...t, open: false }))}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert onClose={() => setToast((t) => ({ ...t, open: false }))} severity={toast.severity} variant="filled" sx={{ width: '100%' }}>
          {toast.message}
        </Alert>
      </Snackbar>
    </>
  );
};

export const FCaixaBanco: React.FC = () => (
  <CadastroBasePage<ICaixaBancoForm>
    config={new CaixaBancoCadastroConfig()}
    initialRecords={[]}
    renderForm={(mode, record, onSave) => <CaixaBancoFormView mode={mode} record={record} onSave={onSave} />}
  />
);

export default FCaixaBanco;

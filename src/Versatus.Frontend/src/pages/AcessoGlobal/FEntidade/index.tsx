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
  Switch
} from '@mui/material';
import { useForm, FormProvider, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEnumOptions } from '../../../hooks/useEnums';
import { useTabs } from '../../../context/TabsContext';

import { CadastroBasePage } from '../../../components/crud/CadastroBasePage';
import { EntidadeCadastroConfig } from './EntidadeCadastroConfig';
import type { CadastroModalMode } from '../../../types/cadastro';

import type { IEntidadeForm } from './types';
import { entidadeSchema } from './schema';

import { DadosGeraisTab } from './tabs/DadosGeraisTab';
import { InfoGeraisTab } from './tabs/InfoGeraisTab';
import { ClienteTab } from './tabs/ClienteTab';
import { FornecedorTab } from './tabs/FornecedorTab';
import { FuncionarioTab } from './tabs/FuncionarioTab';
import { TransportadoraTab } from './tabs/TransportadoraTab';
import { FilialTab } from './tabs/FilialTab';
import { ObraTab } from './tabs/ObraTab';
import { 
  ContadorTab, 
  RepresentanteTab, 
  ComissionadoTab, 
  AgenciaTab, 
  FinanceiraTab, 
  OutroTab, 
  ProspectoTab, 
  AlunoTab, 
  ProfessorTab, 
  IntermediadorTab 
} from './tabs/CommonRolesTab';

export interface IEntidadeFormViewProps {
  mode: CadastroModalMode;
  record: IEntidadeForm;
  onSave: (data: IEntidadeForm) => void;
}

export const EntidadeFormView: React.FC<IEntidadeFormViewProps> = ({
  mode,
  record,
  onSave
}) => {
  const { options: tipoPessoaOptions, loading: loadingTipoPessoa } = useEnumOptions(1); // 1 = EntidadeFisicaJuridica

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

  const [activeMainTab, setActiveMainTab] = useState(0);
  const [activeInfoTab, setActiveInfoTab] = useState(0);
  const [activeClienteTab, setActiveClienteTab] = useState(0);
  const [activeFornecedorTab, setActiveFornecedorTab] = useState(0);
  const [activeFuncionarioTab, setActiveFuncionarioTab] = useState(0);
  const [activeFilialTab, setActiveFilialTab] = useState(0);
  const [activeObraTab, setActiveObraTab] = useState(0);

  const isBrowse = mode === 'delete' || mode === 'view';

  const { abaAtivaId, marcarDirty } = useTabs();

  // Configuração do React Hook Form
  const methods = useForm<IEntidadeForm>({
    resolver: zodResolver(entidadeSchema) as any,
    defaultValues: record,
    mode: 'onChange',
  });

  const { handleSubmit, reset, control, formState: { errors, isDirty }, watch } = methods;

  // Sincronizar record do CRUD com o Form State
  useEffect(() => {
    reset(record);
  }, [record, reset]);

  // Sincronizar estado de formulário alterado (dirty) com a aba ativa
  useEffect(() => {
    if (abaAtivaId && mode !== 'view' && mode !== 'delete') {
      marcarDirty(abaAtivaId, isDirty);
    }
  }, [isDirty, abaAtivaId, mode, marcarDirty]);

  // Observar mudanças em tempo real para campos que controlam abas e seções condicionais
  const watchedValues = watch([
    'tipoPessoa',
    'isCliente',
    'isFornecedor',
    'isFuncionario',
    'isTransportadora',
    'isComissionado',
    'isFilial',
    'isAgencia',
    'isFinanceira',
    'isObra',
    'isRepresentante',
    'isOutro',
    'isProspecto',
    'isContador',
    'isAluno',
    'isProfessor',
    'isIntermediador',
  ]);

  const [
    tipoPessoa,
    isCliente,
    isFornecedor,
    isFuncionario,
    isTransportadora,
    isComissionado,
    isFilial,
    isAgencia,
    isFinanceira,
    isObra,
    isRepresentante,
    isOutro,
    isProspecto,
    isContador,
    isAluno,
    isProfessor,
    isIntermediador,
  ] = watchedValues;

  const currentRecord = {
    tipoPessoa,
    isCliente,
    isFornecedor,
    isFuncionario,
    isTransportadora,
    isComissionado,
    isFilial,
    isAgencia,
    isFinanceira,
    isObra,
    isRepresentante,
    isOutro,
    isProspecto,
    isContador,
    isAluno,
    isProfessor,
    isIntermediador,
  };

  // Detectar quantidade de erros de validação por aba
  const getTabErrorCount = (tabName: string) => {
    const errorKeys = Object.keys(errors);
    if (errorKeys.length === 0) return 0;

    let fields: string[] = [];
    switch (tabName) {
      case 'dados_gerais':
        fields = [
          'razaoSocial', 'apelido', 'tipoEspecificoEntidade', 'tipoPessoa',
          'cpf', 'rg', 'rgEmissor', 'rgEmissao', 'dataNascimento', 'sexo',
          'estadoCivil', 'cidadeNascimento', 'grauInstrucao', 'fisicaTipoJuridica',
          'inscricaoEstadual', 'inscricaoMunicipal', 'inscricaoSuframa', 'inscricaoRural',
          'contribuinteIcms', 'cnpj', 'regimeTributario', 'naturezaJuridica', 'enquadramento'
        ];
        break;
      case 'info_gerais':
        fields = [
          'enderecos', 'telefones', 'contatos', 'emailPrincipal', 'emailVendas',
          'emailCompras', 'emailFinanceiro', 'emailNfe', 'homePage', 'cnaes', 'empresas'
        ];
        break;
      case 'cliente':
        fields = [
          'idRotaVenda', 'idRotaEntrega', 'idAreaVenda', 'idCategoriaCliente',
          'idConceito', 'idVendedor', 'cliTipoFrete', 'cliMeioPublicidade',
          'cliNumAlternativo', 'idPortador', 'idCondicaoPagamento', 'diaCobranca',
          'idRotaCobranca', 'limiteCredito', 'cliImovel', 'cliValorAluguel',
          'cliItemFinanceiroPadrao', 'cliEnviarCND', 'cliObrigatorioPedidoB2B',
          'idComissionado', 'cliLocalTrabalho', 'cliProfissao', 'cliFoneTrabalho',
          'cliDataAdmissaoTrabalho', 'cliRendaMensalTrabalho', 'cliReferencias',
          'cliBens', 'cliCartoes', 'cliParentes', 'cliSocios'
        ];
        break;
      case 'fornecedor':
        fields = [
          'idCategoriaFornecedor', 'idCondicaoPagamentoFornecedor', 'fornNumAlternativo',
          'fornCotacaoProd', 'idCentroCusto', 'idProjeto', 'fornContas', 'fornFiliais'
        ];
        break;
      case 'funcionario':
        fields = [
          'funcionarioMatricula', 'funcionarioCargo', 'funcionarioSalario',
          'funcionarioTipoSalario', 'funcionarioCargaHoraria', 'funcionarioDepartamento',
          'funcionarioHorario', 'dataAdmissao', 'dataDemissao', 'funcionarioSituacao',
          'funcionarioCaged', 'funcionarioNomePai', 'funcionarioNomeMae', 'cnhNumero',
          'cnhCategoria', 'cnhVencimento', 'ctpsNumero', 'ctpsSerie', 'ctpsUf',
          'ctpsEmissao', 'pisNumero', 'pisBanco', 'fuAgenciaPis', 'fuNomeAgenciaPis',
          'pisInscricao', 'fgtsBanco', 'fgtsAgencia', 'fuNomeAgenciaFgts', 'fgtsConta',
          'fuCategoriaFgts', 'fuDataFgts', 'fuFgtsOpcao', 'fuBancoConta', 'fuAgenciaConta',
          'fuAgenciaDigito', 'fuContaSalario', 'fuContaDigito', 'fuTipoConta', 'fuRaisRaca',
          'fuRaisVinculo', 'fuRaisTipoAdmissao', 'fuRaisTipoDeficiencia', 'funcDependentes'
        ];
        break;
      case 'transportadora':
        fields = [
          'idCategoriaTransportadora', 'rntrc', 'tipoProprietario', 'tipoTransportador',
          'tipoFretePadrao', 'tAtivo', 'transpVeiculos'
        ];
        break;
      case 'filial':
        fields = [
          'filialCodigoEmpresa', 'filialRegimeIss', 'filialCnaeServicoFiscal',
          'filialContador', 'filialSpedPerfil', 'filialIndicadorAtividade',
          'filialClassificacaoIndustrial', 'filialCarteiraDigital', 'filialCodigoServicoFiscal',
          'flAtivo', 'filialIncentivoFiscal', 'filialUsaSituacaoTributariaFornecedor',
          'filialEnviarInventarioSt', 'filialCodigoAtividadeCp', 'filialCodigoReceitaCp',
          'filialTipoEmitenteMdfe', 'filialAutorizadosXml'
        ];
        break;
      case 'obra':
        fields = [
          'obraIdCliente', 'obraResponsavel', 'oDataInicio', 'oPrevisaoInicio',
          'oPrevisaoFim', 'oDataFim', 'obraSituacao', 'oDataSituacao',
          'obraTabelaPreco', 'obraCategoria', 'oAtivo', 'oReservaEstoque',
          'obraIdCentroCusto', 'obraIdProjeto', 'obraIdOperacaoAtendimento',
          'obraIdOperacaoDevolucao'
        ];
        break;
      case 'contador':
        fields = ['contadorCpf', 'contadorNome', 'contadorCrc', 'coAtivo'];
        break;
      case 'representante':
        fields = ['idCategoriaRepresentante', 'rAtivo', 'represClientes'];
        break;
      case 'comissionado':
        fields = ['comissaoNomeComercial', 'comissaoPercentual', 'comissaoTipo', 'tipoComissionado', 'idCategoriaComissionado', 'idUsuarioVinculado', 'cmAtivo'];
        break;
      case 'agencia':
        fields = ['agenciaBanco', 'agenciaNome', 'agenciaNumero', 'aAtivo'];
        break;
      case 'financeira':
        fields = ['financeiraNomeResumido', 'idCategoriaFinanceira', 'financeiraContaCorrente', 'financeiraTaxaExtra', 'financeiraDiaVencimento', 'fnAtivo'];
        break;
      case 'outro':
        fields = ['idCategoriaOutro', 'ouAtivo'];
        break;
      case 'prospecto':
        fields = ['idCategoriaProspecto', 'prAtivo'];
        break;
      case 'aluno':
        fields = ['alTipoResponsavel', 'alIdClienteResponsavel', 'alDiaPreferenicaPagamento', 'alDataCadastro', 'alAtivo'];
        break;
      case 'professor':
        fields = ['pfDataCadastro', 'pfAtivo', 'filiaisVinculadas'];
        break;
      case 'intermediador':
        fields = ['iiIdentificacaoIntermediador', 'idCategoriaIntermediador', 'icAtivo'];
        break;
    }

    return errorKeys.filter(key => fields.includes(key)).length;
  };

  // Renderizar rótulo da aba com indicador de erros
  const renderTabLabel = (label: string, tabName: string) => {
    const errorCount = getTabErrorCount(tabName);
    return (
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Typography variant="body2" sx={{ fontWeight: errorCount > 0 ? 600 : 400 }}>{label}</Typography>
        {errorCount > 0 && (
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              bgcolor: 'error.main',
              color: 'error.contrastText',
              borderRadius: '50%',
              width: 16,
              height: 16,
              fontSize: '0.65rem',
              fontWeight: 'bold',
            }}
          >
            {errorCount}
          </Box>
        )}
      </Box>
    );
  };

  // Dinamizar cabeçalho de abas corporativas
  const renderRoleTabs = () => {
    const roleTabs: React.ReactNode[] = [];
    if (currentRecord.isCliente) roleTabs.push(<Tab label={renderTabLabel("Cliente", "cliente")} key="isCliente" />);
    if (currentRecord.isFornecedor) roleTabs.push(<Tab label={renderTabLabel("Fornecedor", "fornecedor")} key="isFornecedor" />);
    if (currentRecord.isFuncionario) roleTabs.push(<Tab label={renderTabLabel("Funcionário", "funcionario")} key="isFuncionario" />);
    if (currentRecord.isTransportadora) roleTabs.push(<Tab label={renderTabLabel("Transportadora", "transportadora")} key="isTransportadora" />);
    if (currentRecord.isFilial) roleTabs.push(<Tab label={renderTabLabel("Filial", "filial")} key="isFilial" />);
    if (currentRecord.isRepresentante) roleTabs.push(<Tab label={renderTabLabel("Representante", "representante")} key="isRepresentante" />);
    if (currentRecord.isContador) roleTabs.push(<Tab label={renderTabLabel("Contador", "contador")} key="isContador" />);
    if (currentRecord.isComissionado) roleTabs.push(<Tab label={renderTabLabel("Comissionado", "comissionado")} key="isComissionado" />);
    if (currentRecord.isAgencia) roleTabs.push(<Tab label={renderTabLabel("Agência", "agencia")} key="isAgencia" />);
    if (currentRecord.isFinanceira) roleTabs.push(<Tab label={renderTabLabel("Inst. Financeira", "financeira")} key="isFinanceira" />);
    if (currentRecord.isObra) roleTabs.push(<Tab label={renderTabLabel("Obra", "obra")} key="isObra" />);
    if (currentRecord.isOutro) roleTabs.push(<Tab label={renderTabLabel("Outro", "outro")} key="isOutro" />);
    if (currentRecord.isProspecto) roleTabs.push(<Tab label={renderTabLabel("Prospecto", "prospecto")} key="isProspecto" />);
    if (currentRecord.isAluno) roleTabs.push(<Tab label={renderTabLabel("Aluno", "aluno")} key="isAluno" />);
    if (currentRecord.isProfessor) roleTabs.push(<Tab label={renderTabLabel("Professor", "professor")} key="isProfessor" />);
    if (currentRecord.isIntermediador) roleTabs.push(<Tab label={renderTabLabel("Intermediador", "intermediador")} key="isIntermediador" />);
    return roleTabs;
  };

  // Mapear índice das abas de papéis baseado nas ativas
  const getRoleTabContent = (index: number) => {
    const roles: string[] = [];
    if (currentRecord.isCliente) roles.push('cliente');
    if (currentRecord.isFornecedor) roles.push('fornecedor');
    if (currentRecord.isFuncionario) roles.push('funcionario');
    if (currentRecord.isTransportadora) roles.push('transportadora');
    if (currentRecord.isFilial) roles.push('filial');
    if (currentRecord.isRepresentante) roles.push('representante');
    if (currentRecord.isContador) roles.push('contador');
    if (currentRecord.isComissionado) roles.push('comissionado');
    if (currentRecord.isAgencia) roles.push('agencia');
    if (currentRecord.isFinanceira) roles.push('financeira');
    if (currentRecord.isObra) roles.push('obra');
    if (currentRecord.isOutro) roles.push('outro');
    if (currentRecord.isProspecto) roles.push('prospecto');
    if (currentRecord.isAluno) roles.push('aluno');
    if (currentRecord.isProfessor) roles.push('professor');
    if (currentRecord.isIntermediador) roles.push('intermediador');

    // Subtrai as duas abas fixas ("Dados Gerais" = 0, "Informações Gerais" = 1)
    const targetRole = roles[index - 2];
    return targetRole || '';
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
        {/* Botões ocultos para acionamento remoto no rodapé do Modal */}
        <button id="crud-submit-btn" type="submit" style={{ display: 'none' }} />
        <button id="crud-reset-btn" type="button" style={{ display: 'none' }} onClick={() => reset(record)} />

        <Box sx={{ width: '100%' }}>
          
          {/* Cabeçalho Fixo do Formulário */}
          <Grid container spacing={2.5} sx={{ mb: 3 }}>
            <Grid size={{ xs: 12, sm: 2 }}>
              <Controller
                name="codigo"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label="Código"
                    value={field.value || 'Automático'}
                    disabled
                    slotProps={{ input: { readOnly: true } }}
                  />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: tipoPessoa === 3 ? 4 : 7 }}>
              <Controller
                name="razaoSocial"
                control={control}
                render={({ field, fieldState: { error } }) => (
                  <TextField
                    {...field}
                    fullWidth
                    label="Nome / Nome Fantasia"
                    disabled={isBrowse}
                    required
                    error={!!error}
                    helperText={error?.message}
                  />
                )}
              />
            </Grid>
            {tipoPessoa === 3 && (
              <Grid size={{ xs: 12, sm: 3 }}>
                <Controller
                  name="apelido"
                  control={control}
                  render={({ field, fieldState: { error } }) => (
                    <TextField
                      {...field}
                      fullWidth
                      label="Razão Social"
                      disabled={isBrowse}
                      required
                      error={!!error}
                      helperText={error?.message}
                    />
                  )}
                />
              </Grid>
            )}
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="tipoEspecificoEntidade"
                control={control}
                render={({ field, fieldState: { error } }) => (
                  <FormControl fullWidth required disabled={isBrowse} error={!!error}>
                    <InputLabel>Tipo Específico</InputLabel>
                    <Select
                      {...field}
                      label="Tipo Específico"
                    >
                      <MenuItem value="Geral">Geral</MenuItem>
                      <MenuItem value="ProdutorRural">Produtor Rural</MenuItem>
                      <MenuItem value="OrgaoPublico">Órgão Público</MenuItem>
                      <MenuItem value="Microempreendedor">Microempreendedor (MEI)</MenuItem>
                    </Select>
                    {error && (
                      <FormHelperText>{error.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 2 }}>
              <Controller
                name="ativo"
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
                    label="Ativo"
                    sx={{ mt: 0.5 }}
                  />
                )}
              />
            </Grid>
          </Grid>

          {/* Checkbox de Papéis Corporativos */}
          <Grid container spacing={2} sx={{ mb: 4 }}>
            <Grid size={{ xs: 12, sm: 3 }}>
              <Controller
                name="tipoPessoa"
                control={control}
                render={({ field, fieldState: { error } }) => (
                  <FormControl fullWidth required disabled={isBrowse} error={!!error}>
                    <InputLabel>Tipo de Pessoa</InputLabel>
                    <Select
                      {...field}
                      label="Tipo de Pessoa"
                      disabled={isBrowse || loadingTipoPessoa}
                    >
                      {tipoPessoaOptions.map((opt) => (
                        <MenuItem key={opt.value} value={opt.value}>
                          {opt.label === 'Fisica' ? 'Física (CPF)' :
                           opt.label === 'Juridica' ? 'Jurídica (CNPJ)' : opt.label}
                        </MenuItem>
                      ))}
                    </Select>
                    {error && (
                      <FormHelperText>{error.message}</FormHelperText>
                    )}
                  </FormControl>
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 9 }}>
              <Box sx={{ 
                p: 1.5, 
                border: '1px solid', 
                borderColor: (errors.isCliente && !watch('isCliente')) ? 'error.main' : 'divider', 
                borderRadius: 1.5, 
                bgcolor: 'background.default' 
              }}>
                <Typography variant="caption" sx={{ display: 'block', mb: 1, color: (errors.isCliente && !watch('isCliente')) ? 'error.main' : 'text.secondary', fontWeight: 600 }}>
                  Papéis Corporativos
                </Typography>
                <Grid container spacing={1}>
                  {[
                    { name: 'isCliente', label: 'Cliente' },
                    { name: 'isFornecedor', label: 'Fornecedor' },
                    { name: 'isFuncionario', label: 'Funcionário' },
                    { name: 'isTransportadora', label: 'Transportadora' },
                    { name: 'isComissionado', label: 'Comissionado' },
                    { name: 'isFilial', label: 'Filial' },
                    { name: 'isAgencia', label: 'Agência Bancária' },
                    { name: 'isFinanceira', label: 'Inst. Financeira' },
                    { name: 'isObra', label: 'Obra' },
                    { name: 'isRepresentante', label: 'Representante' },
                    { name: 'isOutro', label: 'Outro' },
                    { name: 'isProspecto', label: 'Prospecto' },
                    { name: 'isContador', label: 'Contador' },
                    { name: 'isAluno', label: 'Aluno' },
                    { name: 'isProfessor', label: 'Professor' },
                    { name: 'isIntermediador', label: 'Intermediador' },
                  ].map((role) => (
                    <Grid key={role.name}>
                      <Controller
                        name={role.name as any}
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
                            label={<Typography variant="body2">{role.label}</Typography>}
                          />
                        )}
                      />
                    </Grid>
                  ))}
                </Grid>
                {errors.isCliente && !watch('isCliente') && (
                  <FormHelperText error sx={{ mt: 1 }}>{errors.isCliente.message}</FormHelperText>
                )}
              </Box>
            </Grid>
          </Grid>

          {/* Abas Principais */}
          <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
            <Tabs 
              value={activeMainTab} 
              onChange={(_, val) => setActiveMainTab(val)} 
              textColor="primary" 
              indicatorColor="primary"
              variant="scrollable"
              scrollButtons="auto"
            >
              <Tab label={renderTabLabel("Dados Gerais", "dados_gerais")} />
              <Tab label={renderTabLabel("Informações Gerais", "info_gerais")} />
              {renderRoleTabs()}
            </Tabs>
          </Box>

          {/* -------------------- ABA DADOS GERAIS -------------------- */}
          {activeMainTab === 0 && (
            <DadosGeraisTab isBrowse={isBrowse} />
          )}

          {/* -------------------- ABA INFORMAÇÕES GERAIS -------------------- */}
          {activeMainTab === 1 && (
            <InfoGeraisTab
              isBrowse={isBrowse}
              activeInfoTab={activeInfoTab}
              setActiveInfoTab={setActiveInfoTab}
            />
          )}

          {/* -------------------- ABAS DINÂMICAS DE PAPÉIS -------------------- */}
          {getRoleTabContent(activeMainTab) === 'cliente' && (
            <ClienteTab
              isBrowse={isBrowse}
              activeClienteTab={activeClienteTab}
              setActiveClienteTab={setActiveClienteTab}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'fornecedor' && (
            <FornecedorTab
              isBrowse={isBrowse}
              activeFornecedorTab={activeFornecedorTab}
              setActiveFornecedorTab={setActiveFornecedorTab}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'funcionario' && (
            <FuncionarioTab
              isBrowse={isBrowse}
              activeFuncionarioTab={activeFuncionarioTab}
              setActiveFuncionarioTab={setActiveFuncionarioTab}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'transportadora' && (
            <TransportadoraTab
              isBrowse={isBrowse}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'filial' && (
            <FilialTab
              isBrowse={isBrowse}
              activeFilialTab={activeFilialTab}
              setActiveFilialTab={setActiveFilialTab}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'obra' && (
            <ObraTab
              isBrowse={isBrowse}
              activeObraTab={activeObraTab}
              setActiveObraTab={setActiveObraTab}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'contador' && (
            <ContadorTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'representante' && (
            <RepresentanteTab
              isBrowse={isBrowse}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'comissionado' && (
            <ComissionadoTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'agencia' && (
            <AgenciaTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'financeira' && (
            <FinanceiraTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'outro' && (
            <OutroTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'prospecto' && (
            <ProspectoTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'aluno' && (
            <AlunoTab isBrowse={isBrowse} />
          )}

          {getRoleTabContent(activeMainTab) === 'professor' && (
            <ProfessorTab
              isBrowse={isBrowse}
            />
          )}

          {getRoleTabContent(activeMainTab) === 'intermediador' && (
            <IntermediadorTab isBrowse={isBrowse} />
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

// Componente principal FEntidade ligado ao CadastroBasePage (OOP)
export const FEntidade: React.FC = () => {
  return (
    <CadastroBasePage<IEntidadeForm>
      config={new EntidadeCadastroConfig()}
      initialRecords={[]}
      renderForm={(mode, record, onSave) => (
        <EntidadeFormView mode={mode} record={record} onSave={onSave} />
      )}
    />
  );
};

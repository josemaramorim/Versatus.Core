
export interface IEndereco {
  key?: string;
  tipo: string;
  logradouro: string;
  numero: string;
  bairro: string;
  cidade: string;
  uf: string;
  cep: string;
}

export interface ITelefone {
  key?: string;
  tipo: string;
  numero: string;
  contato: string;
}

export interface IContato {
  key?: string;
  nome: string;
  cargo: string;
  email: string;
  celular: string;
}

export interface ICnae {
  key?: string;
  codigo: string;
  descricao: string;
  principal: boolean;
}

export interface IEmpresa {
  key?: string;
  codigoEmpresa: string;
  nomeEmpresa: string;
  vinculada: boolean;
}

export interface ICliReferencia {
  key?: string;
  nome: string;
  telefone: string;
  tipo: string;
  observacao: string;
}

export interface ICliBem {
  key?: string;
  descricao: string;
  valor: number;
  alienado: boolean;
  observacao: string;
}

export interface ICliCartao {
  key?: string;
  bandeira: string;
  numero: string;
  nome: string;
  vencimento: string;
}

export interface ICliParente {
  key?: string;
  nome: string;
  parentesco: string;
  telefone: string;
  observacao: string;
}

export interface ICliSocio {
  key?: string;
  nome: string;
  cpf: string;
  cargo: string;
  percentualParticipacao: number;
}

export interface IFornConta {
  key?: string;
  banco: string;
  agencia: string;
  conta: string;
  tipo: string;
  favorecido: string;
  cpfCnpj: string;
}

export interface IFornFilial {
  key?: string;
  codigoFilial: string;
  nomeFilial: string;
  ativo: boolean;
}

export interface ITranspVeiculo {
  key?: string;
  placa: string;
  uf: string;
  renavam: string;
  tara: number;
  capacidade: number;
  proprietario: string;
}

export interface IFilialAutorizadoXml {
  key?: string;
  cpfCnpj: string;
}

export interface IFuncDependente {
  key?: string;
  nome: string;
  dataNascimento: string;
  cpf: string;
  parentesco: string;
}

export interface IRepresCliente {
  key?: string;
  codigoCliente: string;
  nomeCliente: string;
  regiao: string;
}

export interface IFilialVinculada {
  key?: string;
  codigoFilial: string;
  filialNome: string;
}

export interface IEntidadeForm {
  codigo: string;
  razaoSocial: string;
  apelido: string;
  tipoEspecificoEntidade: string;
  tipoPessoa: number; // 1 = Fisica, 2 = Juridica
  isCliente: boolean;
  isFornecedor: boolean;
  isFuncionario: boolean;
  isTransportadora: boolean;
  isComissionado: boolean;
  isFilial: boolean;
  isAgencia: boolean;
  isFinanceira: boolean;
  isObra: boolean;
  isRepresentante: boolean;
  isOutro: boolean;
  isProspecto: boolean;
  isContador: boolean;
  isAluno: boolean;
  isProfessor: boolean;
  isIntermediador: boolean;
  ativo: boolean;

  // Pessoa Física
  cpf: string;
  rg: string;
  rgEmissor: string;
  rgEmissao: string;
  dataNascimento: string;
  sexo: string;
  estadoCivil: string;
  cidadeNascimento: string;
  grauInstrucao: string;
  fisicaTipoJuridica: boolean;

  // Caracteristicas Fiscais (PF com caracteristica juridica ou PJ)
  inscricaoEstadual: string;
  inscricaoMunicipal: string;
  inscricaoSuframa: string;
  inscricaoRural: string;
  contribuinteIcms: string;

  // Pessoa Jurídica
  cnpj: string;
  regimeTributario: number;
  naturezaJuridica: number;
  enquadramento: string;

  // E-mail / Internet
  emailPrincipal: string;
  emailVendas: string;
  emailCompras: string;
  emailFinanceiro: string;
  emailNfe: string;
  homePage: string;
  anexarBoleto: boolean;

  // Auditoria
  usuarioInclusao: string;
  dataInclusao: string;
  horaInclusao: string;
  usuarioAlteracao: string;
  dataAlteracao: string;
  horaAlteracao: string;

  // Cliente tab fields
  idRotaVenda: number;
  idRotaEntrega: number;
  idAreaVenda: number;
  idCategoriaCliente: number;
  idConceito: number;
  idVendedor: number;
  cliTipoFrete: string;
  cliMeioPublicidade: string;
  cliNumAlternativo: string;
  idPortador: number;
  idCondicaoPagamento: number;
  diaCobranca: number;
  idRotaCobranca: number;
  limiteCredito: number;
  cliImovel: string;
  cliValorAluguel: number;
  cliItemFinanceiroPadrao: boolean;
  cliEnviarCND: boolean;
  cliObrigatorioPedidoB2B: boolean;
  cliAtivo: boolean;
  situacaoClienteSpc: number;
  fornAtivo: boolean;
  funcAtivo: boolean;
  idComissionado: number;
  cliLocalTrabalho: string;
  cliProfissao: string;
  cliFoneTrabalho: string;
  cliDataAdmissaoTrabalho: string;
  cliRendaMensalTrabalho: number;

  // Fornecedor fields
  idCategoriaFornecedor: number;
  idCondicaoPagamentoFornecedor: number;
  fornNumAlternativo: string;
  fornCotacaoProd: boolean;
  idCentroCusto: number;
  idProjeto: number;

  // Funcionário fields
  funcionarioMatricula: string;
  funcionarioCargo: string;
  funcionarioSalario: number;
  funcionarioTipoSalario: string;
  funcionarioCargaHoraria: number;
  funcionarioDepartamento: string;
  funcionarioHorario: string;
  dataAdmissao: string;
  dataDemissao: string;
  funcionarioSituacao: string;
  funcionarioCaged: string;
  funcionarioNomePai: string;
  funcionarioNomeMae: string;
  cnhNumero: string;
  cnhCategoria: string;
  cnhVencimento: string;
  ctpsNumero: string;
  ctpsSerie: string;
  ctpsUf: string;
  ctpsEmissao: string;
  pisNumero: string;
  pisBanco: string;
  fuAgenciaPis: string;
  fuNomeAgenciaPis: string;
  pisInscricao: string;
  fgtsBanco: string;
  fgtsAgencia: string;
  fuNomeAgenciaFgts: string;
  fgtsConta: string;
  fuCategoriaFgts: string;
  fuDataFgts: string;
  fuFgtsOpcao: string;
  fuBancoConta: string;
  fuAgenciaConta: string;
  fuAgenciaDigito: string;
  fuContaSalario: string;
  fuContaDigito: string;
  fuTipoConta: string;
  fuRaisRaca: string;
  fuRaisVinculo: string;
  fuRaisTipoAdmissao: string;
  fuRaisTipoDeficiencia: string;

  // Transportadora fields
  idCategoriaTransportadora: number;
  rntrc: string;
  tipoProprietario: number;
  tipoTransportador: number;
  tipoFretePadrao: string;
  tAtivo: boolean;

  // Filial fields
  filialCodigoEmpresa: string;
  filialRegimeIss: number;
  filialCnaeServicoFiscal: number;
  filialContador: string;
  filialSpedPerfil: string;
  filialIndicadorAtividade: number;
  filialClassificacaoIndustrial: number;
  filialCarteiraDigital: number;
  filialCodigoServicoFiscal: number;
  flAtivo: boolean;
  filialIncentivoFiscal: boolean;
  filialUsaSituacaoTributariaFornecedor: boolean;
  filialEnviarInventarioSt: boolean;
  filialCodigoAtividadeCp: string;
  filialCodigoReceitaCp: string;
  filialTipoEmitenteMdfe: number;

  // Representante fields
  idCategoriaRepresentante: number;
  rAtivo: boolean;

  // Contador fields
  contadorCpf: string;
  contadorNome: string;
  contadorCrc: string;
  coAtivo: boolean;

  // Comissionado fields
  comissaoNomeComercial: string;
  comissaoPercentual: number;
  comissaoTipo: string;
  tipoComissionado: string;
  idCategoriaComissionado: number;
  idUsuarioVinculado: number;
  cmAtivo: boolean;

  // Agência fields
  agenciaBanco: number;
  agenciaNome: string;
  agenciaNumero: string;
  aAtivo: boolean;

  // Financeira fields
  financeiraNomeResumido: string;
  idCategoriaFinanceira: number;
  financeiraContaCorrente: string;
  financeiraTaxaExtra: number;
  financeiraDiaVencimento: number;
  fnAtivo: boolean;

  // Obra fields
  obraIdCliente: number;
  obraResponsavel: string;
  oDataInicio: string;
  oPrevisaoInicio: string;
  oPrevisaoFim: string;
  oDataFim: string;
  obraSituacao: string;
  oDataSituacao: string;
  obraTabelaPreco: number;
  obraCategoria: number;
  oAtivo: boolean;
  oReservaEstoque: boolean;
  obraIdCentroCusto: number;
  obraIdProjeto: number;
  obraIdOperacaoAtendimento: number;
  obraIdOperacaoDevolucao: number;

  // Outro, Prospecto, Aluno, Professor, Intermediador
  idCategoriaOutro: number;
  ouAtivo: boolean;
  idCategoriaProspecto: number;
  prAtivo: boolean;
  alTipoResponsavel: number;
  alIdClienteResponsavel: number;
  alDiaPreferenicaPagamento: number;
  alDataCadastro: string;
  alAtivo: boolean;
  pfDataCadastro: string;
  pfAtivo: boolean;
  iiIdentificacaoIntermediador: string;
  idCategoriaIntermediador: number;
  icAtivo: boolean;

  // Tabelas Grid Vinculadas (Sub-arrays)
  enderecos: IEndereco[];
  telefones: ITelefone[];
  contatos: IContato[];
  cnaes: ICnae[];
  empresas: IEmpresa[];
  cliReferencias: ICliReferencia[];
  cliBens: ICliBem[];
  cliCartoes: ICliCartao[];
  cliParentes: ICliParente[];
  cliSocios: ICliSocio[];
  fornContas: IFornConta[];
  fornFiliais: IFornFilial[];
  transpVeiculos: ITranspVeiculo[];
  filialAutorizadosXml: IFilialAutorizadoXml[];
  funcDependentes: IFuncDependente[];
  represClientes: IRepresCliente[];
  filiaisVinculadas: IFilialVinculada[];
}

export const defaultValues: IEntidadeForm = {
  codigo: '',
  razaoSocial: '',
  apelido: '',
  tipoEspecificoEntidade: 'Geral',
  tipoPessoa: 2,
  isCliente: false,
  isFornecedor: false,
  isFuncionario: false,
  isTransportadora: false,
  isComissionado: false,
  isFilial: false,
  isAgencia: false,
  isFinanceira: false,
  isObra: false,
  isRepresentante: false,
  isOutro: false,
  isProspecto: false,
  isContador: false,
  isAluno: false,
  isProfessor: false,
  isIntermediador: false,
  ativo: true,
  cpf: '',
  rg: '',
  rgEmissor: '',
  rgEmissao: '',
  dataNascimento: '',
  sexo: '',
  estadoCivil: '',
  cidadeNascimento: '',
  grauInstrucao: '',
  fisicaTipoJuridica: false,
  inscricaoEstadual: '',
  inscricaoMunicipal: '',
  inscricaoSuframa: '',
  inscricaoRural: '',
  contribuinteIcms: 'Sim',
  cnpj: '',
  regimeTributario: 1,
  naturezaJuridica: 1,
  enquadramento: 'ME',
  emailPrincipal: '',
  emailVendas: '',
  emailCompras: '',
  emailFinanceiro: '',
  emailNfe: '',
  homePage: '',
  anexarBoleto: false,
  usuarioInclusao: '',
  dataInclusao: '',
  horaInclusao: '',
  usuarioAlteracao: '',
  dataAlteracao: '',
  horaAlteracao: '',
  idRotaVenda: 1,
  idRotaEntrega: 1,
  idAreaVenda: 1,
  idCategoriaCliente: 1,
  idConceito: 1,
  idVendedor: 1,
  cliTipoFrete: 'CIF',
  cliMeioPublicidade: 'Internet',
  cliNumAlternativo: '',
  idPortador: 1,
  idCondicaoPagamento: 1,
  diaCobranca: 5,
  idRotaCobranca: 1,
  limiteCredito: 0,
  cliImovel: '',
  cliValorAluguel: 0,
  cliItemFinanceiroPadrao: false,
  cliEnviarCND: false,
  cliObrigatorioPedidoB2B: false,
  cliAtivo: true,
  situacaoClienteSpc: 892,
  idComissionado: 1,
  cliLocalTrabalho: '',
  cliProfissao: '',
  cliFoneTrabalho: '',
  cliDataAdmissaoTrabalho: '',
  cliRendaMensalTrabalho: 0,
  idCategoriaFornecedor: 1,
  idCondicaoPagamentoFornecedor: 1,
  fornNumAlternativo: '',
  fornCotacaoProd: false,
  fornAtivo: true,
  funcAtivo: true,
  idCentroCusto: 1,
  idProjeto: 1,
  funcionarioMatricula: '',
  funcionarioCargo: '',
  funcionarioSalario: 0,
  funcionarioTipoSalario: 'Mensalista',
  funcionarioCargaHoraria: 44,
  funcionarioDepartamento: '',
  funcionarioHorario: '08:00 às 18:00',
  dataAdmissao: '',
  dataDemissao: '',
  funcionarioSituacao: 'Ativo',
  funcionarioCaged: '',
  funcionarioNomePai: '',
  funcionarioNomeMae: '',
  cnhNumero: '',
  cnhCategoria: '',
  cnhVencimento: '',
  ctpsNumero: '',
  ctpsSerie: '',
  ctpsUf: '',
  ctpsEmissao: '',
  pisNumero: '',
  pisBanco: '',
  fuAgenciaPis: '',
  fuNomeAgenciaPis: '',
  pisInscricao: '',
  fgtsBanco: '',
  fgtsAgencia: '',
  fuNomeAgenciaFgts: '',
  fgtsConta: '',
  fuCategoriaFgts: '',
  fuDataFgts: '',
  fuFgtsOpcao: 'Optante',
  fuBancoConta: '',
  fuAgenciaConta: '',
  fuAgenciaDigito: '',
  fuContaSalario: '',
  fuContaDigito: '',
  fuTipoConta: 'Corrente',
  fuRaisRaca: 'Branca',
  fuRaisVinculo: 'CLT',
  fuRaisTipoAdmissao: 'PrimeiroEmprego',
  fuRaisTipoDeficiencia: 'Nenhuma',
  idCategoriaTransportadora: 1,
  rntrc: '',
  tipoProprietario: 1,
  tipoTransportador: 1,
  tipoFretePadrao: 'CIF',
  tAtivo: true,
  filialCodigoEmpresa: '',
  filialRegimeIss: 1,
  filialCnaeServicoFiscal: 1,
  filialContador: '',
  filialSpedPerfil: 'A',
  filialIndicadorAtividade: 0,
  filialClassificacaoIndustrial: 1,
  filialCarteiraDigital: 1,
  filialCodigoServicoFiscal: 1,
  flAtivo: true,
  filialIncentivoFiscal: false,
  filialUsaSituacaoTributariaFornecedor: false,
  filialEnviarInventarioSt: false,
  filialCodigoAtividadeCp: '',
  filialCodigoReceitaCp: '',
  filialTipoEmitenteMdfe: 1,
  idCategoriaRepresentante: 1,
  rAtivo: true,
  contadorCpf: '',
  contadorNome: '',
  contadorCrc: '',
  coAtivo: true,
  comissaoNomeComercial: '',
  comissaoPercentual: 0,
  comissaoTipo: 'Percentual',
  tipoComissionado: '',
  idCategoriaComissionado: 1,
  idUsuarioVinculado: 1,
  cmAtivo: true,
  agenciaBanco: 1,
  agenciaNome: '',
  agenciaNumero: '',
  aAtivo: true,
  financeiraNomeResumido: '',
  idCategoriaFinanceira: 1,
  financeiraContaCorrente: '',
  financeiraTaxaExtra: 0,
  financeiraDiaVencimento: 1,
  fnAtivo: true,
  obraIdCliente: 1,
  obraResponsavel: '',
  oDataInicio: '',
  oPrevisaoInicio: '',
  oPrevisaoFim: '',
  oDataFim: '',
  obraSituacao: 'Ativa',
  oDataSituacao: '',
  obraTabelaPreco: 1,
  obraCategoria: 1,
  oAtivo: true,
  oReservaEstoque: false,
  obraIdCentroCusto: 1,
  obraIdProjeto: 1,
  obraIdOperacaoAtendimento: 1,
  obraIdOperacaoDevolucao: 1,
  idCategoriaOutro: 1,
  ouAtivo: true,
  idCategoriaProspecto: 1,
  prAtivo: true,
  alTipoResponsavel: 1,
  alIdClienteResponsavel: 1,
  alDiaPreferenicaPagamento: 5,
  alDataCadastro: '',
  alAtivo: true,
  pfDataCadastro: '',
  pfAtivo: true,
  iiIdentificacaoIntermediador: '',
  idCategoriaIntermediador: 1,
  icAtivo: true,

  // Sub-arrays de Grilhas vazios
  enderecos: [],
  telefones: [],
  contatos: [],
  cnaes: [],
  empresas: [],
  cliReferencias: [],
  cliBens: [],
  cliCartoes: [],
  cliParentes: [],
  cliSocios: [],
  fornContas: [],
  fornFiliais: [],
  transpVeiculos: [],
  filialAutorizadosXml: [],
  funcDependentes: [],
  represClientes: [],
  filiaisVinculadas: [],
};

export interface ITabProps {
  isBrowse: boolean;
}

export interface IInfoGeraisTabProps extends ITabProps {
  activeInfoTab: number;
  setActiveInfoTab: (val: number) => void;
}

export interface IClienteTabProps extends ITabProps {
  activeClienteTab: number;
  setActiveClienteTab: (val: number) => void;
}

export interface IFornecedorTabProps extends ITabProps {
  activeFornecedorTab: number;
  setActiveFornecedorTab: (val: number) => void;
}

export interface IFuncionarioTabProps extends ITabProps {
  activeFuncionarioTab: number;
  setActiveFuncionarioTab: (val: number) => void;
}

export interface IFilialTabProps extends ITabProps {
  activeFilialTab: number;
  setActiveFilialTab: (val: number) => void;
}

export interface IObraTabProps extends ITabProps {
  activeObraTab: number;
  setActiveObraTab: (val: number) => void;
}

export interface IProfessorTabProps extends ITabProps {}

export interface IRepresentanteTabProps extends ITabProps {}

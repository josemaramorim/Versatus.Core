import { z } from 'zod';

export function isValidCPF(cpf: string): boolean {
  const cleanCpf = cpf.replace(/[^\d]/g, '');
  if (cleanCpf.length !== 11) return false;
  if (/^(\d)\1{10}$/.test(cleanCpf)) return false;

  let sum = 0;
  let remainder;

  for (let i = 1; i <= 9; i++) {
    sum += parseInt(cleanCpf.substring(i - 1, i)) * (11 - i);
  }

  remainder = (sum * 10) % 11;
  if (remainder === 10 || remainder === 11) remainder = 0;
  if (remainder !== parseInt(cleanCpf.substring(9, 10))) return false;

  sum = 0;
  for (let i = 1; i <= 10; i++) {
    sum += parseInt(cleanCpf.substring(i - 1, i)) * (12 - i);
  }

  remainder = (sum * 10) % 11;
  if (remainder === 10 || remainder === 11) remainder = 0;
  if (remainder !== parseInt(cleanCpf.substring(10, 11))) return false;

  return true;
}

export function isValidCNPJ(cnpj: string): boolean {
  const cleanCnpj = cnpj.replace(/[^\d]/g, '');
  if (cleanCnpj.length !== 14) return false;
  if (/^(\d)\1{13}$/.test(cleanCnpj)) return false;

  let size = cleanCnpj.length - 2;
  let numbers = cleanCnpj.substring(0, size);
  const digits = cleanCnpj.substring(size);
  let sum = 0;
  let pos = size - 7;

  for (let i = size; i >= 1; i--) {
    sum += parseInt(numbers.charAt(size - i)) * pos--;
    if (pos < 2) pos = 9;
  }

  let result = sum % 11 < 2 ? 0 : 11 - (sum % 11);
  if (result !== parseInt(digits.charAt(0))) return false;

  size = size + 1;
  numbers = cleanCnpj.substring(0, size);
  sum = 0;
  pos = size - 7;

  for (let i = size; i >= 1; i--) {
    sum += parseInt(numbers.charAt(size - i)) * pos--;
    if (pos < 2) pos = 9;
  }

  result = sum % 11 < 2 ? 0 : 11 - (sum % 11);
  if (result !== parseInt(digits.charAt(1))) return false;

  return true;
}

// Sub-schemas para tabelas vinculadas
export const enderecoSchema = z.object({
  tipo: z.string(),
  logradouro: z.string().min(1, 'Logradouro é obrigatório'),
  numero: z.string().min(1, 'Nº é obrigatório'),
  bairro: z.string().min(1, 'Bairro é obrigatório'),
  cidade: z.string().min(1, 'Cidade é obrigatória'),
  uf: z.string().length(2, 'UF deve ter 2 caracteres'),
  cep: z.string().min(1, 'CEP é obrigatório'),
});

export const telefoneSchema = z.object({
  tipo: z.string(),
  numero: z.string().min(1, 'Número é obrigatório'),
  contato: z.string().optional().default(''),
});

export const contatoSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  cargo: z.string().optional().default(''),
  email: z.string().email('E-mail inválido').or(z.literal('')).optional().default(''),
  celular: z.string().optional().default(''),
});

export const cnaeSchema = z.object({
  codigo: z.string().min(1, 'Código é obrigatório'),
  descricao: z.string().min(1, 'Descrição é obrigatória'),
  principal: z.boolean(),
});

export const empresaSchema = z.object({
  codigoEmpresa: z.string().min(1, 'Código é obrigatório'),
  nomeEmpresa: z.string().min(1, 'Nome é obrigatório'),
  vinculada: z.boolean(),
});

export const cliReferenciaSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  telefone: z.string().min(1, 'Telefone é obrigatório'),
  tipo: z.string(),
  observacao: z.string().optional().default(''),
});

export const cliBemSchema = z.object({
  descricao: z.string().min(1, 'Descrição é obrigatória'),
  valor: z.number().min(0, 'Valor deve ser maior ou igual a 0'),
  alienado: z.boolean(),
  observacao: z.string().optional().default(''),
});

export const cliCartaoSchema = z.object({
  bandeira: z.string(),
  numero: z.string().min(1, 'Número é obrigatório'),
  nome: z.string().min(1, 'Nome no cartão é obrigatório'),
  vencimento: z.string().min(1, 'Vencimento é obrigatório'),
});

export const cliParenteSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  parentesco: z.string(),
  telefone: z.string().optional().default(''),
  observacao: z.string().optional().default(''),
});

export const cliSocioSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  cpf: z.string().refine(val => !val || isValidCPF(val), { message: 'CPF inválido' }),
  cargo: z.string(),
  percentualParticipacao: z.number().min(0).max(100, 'Participação deve ser entre 0% e 100%'),
});

export const fornContaSchema = z.object({
  banco: z.string().min(1, 'Banco é obrigatório'),
  agencia: z.string().min(1, 'Agência é obrigatória'),
  conta: z.string().min(1, 'Conta é obrigatória'),
  tipo: z.string(),
  favorecido: z.string().min(1, 'Favorecido é obrigatório'),
  cpfCnpj: z.string().refine(val => !val || isValidCPF(val) || isValidCNPJ(val), { message: 'CPF ou CNPJ inválido' }),
});

export const fornFilialSchema = z.object({
  codigoFilial: z.string().min(1, 'Código é obrigatório'),
  nomeFilial: z.string().min(1, 'Nome é obrigatório'),
  ativo: z.boolean(),
});

export const transpVeiculoSchema = z.object({
  placa: z.string().min(1, 'Placa é obrigatória'),
  uf: z.string().length(2, 'UF deve ter 2 caracteres'),
  renavam: z.string().optional().default(''),
  tara: z.number().min(0, 'Tara inválida'),
  capacidade: z.number().min(0, 'Capacidade inválida'),
  proprietario: z.string().optional().default(''),
});

export const filialAutorizadoXmlSchema = z.object({
  cpfCnpj: z.string().refine(val => !val || isValidCPF(val) || isValidCNPJ(val), { message: 'CPF/CNPJ inválido' }),
});

export const funcDependenteSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório'),
  dataNascimento: z.string().optional().default(''),
  cpf: z.string().refine(val => !val || isValidCPF(val), { message: 'CPF inválido' }),
  parentesco: z.string(),
});

export const represClienteSchema = z.object({
  codigoCliente: z.string().min(1, 'Código é obrigatório'),
  nomeCliente: z.string().min(1, 'Nome é obrigatório'),
  regiao: z.string().optional().default(''),
});

export const filialVinculadaSchema = z.object({
  codigoFilial: z.string().min(1, 'Código é obrigatório'),
  filialNome: z.string().min(1, 'Nome é obrigatório'),
});

// Schema principal da FEntidade com validações básicas e condicionais
export const entidadeSchema = z.object({
  codigo: z.string().optional().default(''),
  razaoSocial: z.string().min(3, 'Nome / Razão Social deve ter no mínimo 3 caracteres'),
  apelido: z.string().optional().default(''),
  tipoEspecificoEntidade: z.string(),
  tipoPessoa: z.number(), // 1 = Fisica, 2 = Juridica
  isCliente: z.boolean(),
  isFornecedor: z.boolean(),
  isFuncionario: z.boolean(),
  isTransportadora: z.boolean(),
  isComissionado: z.boolean(),
  isFilial: z.boolean(),
  isAgencia: z.boolean(),
  isFinanceira: z.boolean(),
  isObra: z.boolean(),
  isRepresentante: z.boolean(),
  isOutro: z.boolean(),
  isProspecto: z.boolean(),
  isContador: z.boolean(),
  isAluno: z.boolean(),
  isProfessor: z.boolean(),
  isIntermediador: z.boolean(),
  ativo: z.boolean(),

  // Pessoa Física
  cpf: z.string().optional().default(''),
  rg: z.string().optional().default(''),
  rgEmissor: z.string().optional().default(''),
  rgEmissao: z.string().optional().default(''),
  dataNascimento: z.string().optional().default(''),
  sexo: z.string().optional().default(''),
  estadoCivil: z.string().optional().default(''),
  cidadeNascimento: z.string().optional().default(''),
  grauInstrucao: z.string().optional().default(''),
  fisicaTipoJuridica: z.boolean(),

  // Caracteristicas Fiscais
  inscricaoEstadual: z.string().optional().default(''),
  inscricaoMunicipal: z.string().optional().default(''),
  inscricaoSuframa: z.string().optional().default(''),
  inscricaoRural: z.string().optional().default(''),
  contribuinteIcms: z.string(),

  // Pessoa Jurídica
  cnpj: z.string().optional().default(''),
  regimeTributario: z.number(),
  naturezaJuridica: z.number(),
  enquadramento: z.string(),

  // E-mail / Internet
  emailPrincipal: z.string().email('E-mail principal inválido').or(z.literal('')).optional().default(''),
  emailVendas: z.string().email('E-mail vendas inválido').or(z.literal('')).optional().default(''),
  emailCompras: z.string().email('E-mail compras inválido').or(z.literal('')).optional().default(''),
  emailFinanceiro: z.string().email('E-mail financeiro inválido').or(z.literal('')).optional().default(''),
  emailNfe: z.string().email('E-mail NFe inválido').or(z.literal('')).optional().default(''),
  homePage: z.string().optional().default(''),
  anexarBoleto: z.boolean(),

  // Auditoria
  usuarioInclusao: z.string().optional().default(''),
  dataInclusao: z.string().optional().default(''),
  horaInclusao: z.string().optional().default(''),
  usuarioAlteracao: z.string().optional().default(''),
  dataAlteracao: z.string().optional().default(''),
  horaAlteracao: z.string().optional().default(''),

  // Cliente tab fields
  idRotaVenda: z.number().optional().default(1),
  idRotaEntrega: z.number().optional().default(1),
  idAreaVenda: z.number().optional().default(1),
  idCategoriaCliente: z.number().optional().default(1),
  idConceito: z.number().optional().default(1),
  idVendedor: z.number().optional().default(1),
  cliTipoFrete: z.string().optional().default('CIF'),
  cliMeioPublicidade: z.string().optional().default('Internet'),
  cliNumAlternativo: z.string().optional().default(''),
  idPortador: z.number().optional().default(1),
  idCondicaoPagamento: z.number().optional().default(1),
  diaCobranca: z.number().min(1, 'Dia de cobrança inválido').max(31, 'Dia de cobrança inválido').optional().default(5),
  idRotaCobranca: z.number().optional().default(1),
  limiteCredito: z.number().min(0, 'Limite de crédito deve ser positivo').optional().default(0),
  cliImovel: z.string().optional().default(''),
  cliValorAluguel: z.number().min(0).optional().default(0),
  cliItemFinanceiroPadrao: z.boolean(),
  cliEnviarCND: z.boolean(),
  cliObrigatorioPedidoB2B: z.boolean(),
  idComissionado: z.number().optional().default(1),
  cliLocalTrabalho: z.string().optional().default(''),
  cliProfissao: z.string().optional().default(''),
  cliFoneTrabalho: z.string().optional().default(''),
  cliDataAdmissaoTrabalho: z.string().optional().default(''),
  cliRendaMensalTrabalho: z.number().min(0).optional().default(0),

  // Fornecedor fields
  idCategoriaFornecedor: z.number().optional().default(1),
  idCondicaoPagamentoFornecedor: z.number().optional().default(1),
  fornNumAlternativo: z.string().optional().default(''),
  fornCotacaoProd: z.boolean(),
  idCentroCusto: z.number().optional().default(1),
  idProjeto: z.number().optional().default(1),

  // Funcionário fields
  funcionarioMatricula: z.string().optional().default(''),
  funcionarioCargo: z.string().optional().default(''),
  funcionarioSalario: z.number().min(0).optional().default(0),
  funcionarioTipoSalario: z.string().optional().default('Mensalista'),
  funcionarioCargaHoraria: z.number().min(0).optional().default(44),
  funcionarioDepartamento: z.string().optional().default(''),
  funcionarioHorario: z.string().optional().default(''),
  dataAdmissao: z.string().optional().default(''),
  dataDemissao: z.string().optional().default(''),
  funcionarioSituacao: z.string().optional().default('Ativo'),
  funcionarioCaged: z.string().optional().default(''),
  funcionarioNomePai: z.string().optional().default(''),
  funcionarioNomeMae: z.string().optional().default(''),
  cnhNumero: z.string().optional().default(''),
  cnhCategoria: z.string().optional().default(''),
  cnhVencimento: z.string().optional().default(''),
  ctpsNumero: z.string().optional().default(''),
  ctpsSerie: z.string().optional().default(''),
  ctpsUf: z.string().optional().default(''),
  ctpsEmissao: z.string().optional().default(''),
  pisNumero: z.string().optional().default(''),
  pisBanco: z.string().optional().default(''),
  fuAgenciaPis: z.string().optional().default(''),
  fuNomeAgenciaPis: z.string().optional().default(''),
  pisInscricao: z.string().optional().default(''),
  fgtsBanco: z.string().optional().default(''),
  fgtsAgencia: z.string().optional().default(''),
  fuNomeAgenciaFgts: z.string().optional().default(''),
  fgtsConta: z.string().optional().default(''),
  fuCategoriaFgts: z.string().optional().default(''),
  fuDataFgts: z.string().optional().default(''),
  fuFgtsOpcao: z.string().optional().default('Optante'),
  fuBancoConta: z.string().optional().default(''),
  fuAgenciaConta: z.string().optional().default(''),
  fuAgenciaDigito: z.string().optional().default(''),
  fuContaSalario: z.string().optional().default(''),
  fuContaDigito: z.string().optional().default(''),
  fuTipoConta: z.string().optional().default('Corrente'),
  fuRaisRaca: z.string().optional().default('Branca'),
  fuRaisVinculo: z.string().optional().default('CLT'),
  fuRaisTipoAdmissao: z.string().optional().default('PrimeiroEmprego'),
  fuRaisTipoDeficiencia: z.string().optional().default('Nenhuma'),

  // Transportadora fields
  idCategoriaTransportadora: z.number().optional().default(1),
  rntrc: z.string().optional().default(''),
  tipoProprietario: z.number().optional().default(1),
  tipoTransportador: z.number().optional().default(1),
  tipoFretePadrao: z.string().optional().default('CIF'),
  tAtivo: z.boolean(),

  // Filial fields
  filialCodigoEmpresa: z.string().optional().default(''),
  filialRegimeIss: z.number().optional().default(1),
  filialCnaeServicoFiscal: z.number().optional().default(1),
  filialContador: z.string().optional().default(''),
  filialSpedPerfil: z.string().optional().default('A'),
  filialIndicadorAtividade: z.number().optional().default(0),
  filialClassificacaoIndustrial: z.number().optional().default(1),
  filialCarteiraDigital: z.number().optional().default(1),
  filialCodigoServicoFiscal: z.number().optional().default(1),
  flAtivo: z.boolean(),
  filialIncentivoFiscal: z.boolean(),
  filialUsaSituacaoTributariaFornecedor: z.boolean(),
  filialEnviarInventarioSt: z.boolean(),
  filialCodigoAtividadeCp: z.string().optional().default(''),
  filialCodigoReceitaCp: z.string().optional().default(''),
  filialTipoEmitenteMdfe: z.number().optional().default(1),

  // Representante fields
  idCategoriaRepresentante: z.number().optional().default(1),
  rAtivo: z.boolean(),

  // Contador fields
  contadorCpf: z.string().optional().default(''),
  contadorNome: z.string().optional().default(''),
  contadorCrc: z.string().optional().default(''),
  coAtivo: z.boolean(),

  // Comissionado fields
  comissaoNomeComercial: z.string().optional().default(''),
  comissaoPercentual: z.number().min(0).max(100, 'Percentual deve ser entre 0 e 100').optional().default(0),
  comissaoTipo: z.string().optional().default('Percentual'),
  tipoComissionado: z.string().optional().default(''),
  idCategoriaComissionado: z.number().optional().default(1),
  idUsuarioVinculado: z.number().optional().default(1),
  cmAtivo: z.boolean(),

  // Agência fields
  agenciaBanco: z.number().optional().default(1),
  agenciaNome: z.string().optional().default(''),
  agenciaNumero: z.string().optional().default(''),
  aAtivo: z.boolean(),

  // Financeira fields
  financeiraNomeResumido: z.string().optional().default(''),
  idCategoriaFinanceira: z.number().optional().default(1),
  financeiraContaCorrente: z.string().optional().default(''),
  financeiraTaxaExtra: z.number().min(0).optional().default(0),
  financeiraDiaVencimento: z.number().min(1).max(31).optional().default(1),
  fnAtivo: z.boolean(),

  // Obra fields
  obraIdCliente: z.number().optional().default(1),
  obraResponsavel: z.string().optional().default(''),
  oDataInicio: z.string().optional().default(''),
  oPrevisaoInicio: z.string().optional().default(''),
  oPrevisaoFim: z.string().optional().default(''),
  oDataFim: z.string().optional().default(''),
  obraSituacao: z.string().optional().default('Ativa'),
  oDataSituacao: z.string().optional().default(''),
  obraTabelaPreco: z.number().optional().default(1),
  obraCategoria: z.number().optional().default(1),
  oAtivo: z.boolean(),
  oReservaEstoque: z.boolean(),
  obraIdCentroCusto: z.number().optional().default(1),
  obraIdProjeto: z.number().optional().default(1),
  obraIdOperacaoAtendimento: z.number().optional().default(1),
  obraIdOperacaoDevolucao: z.number().optional().default(1),

  // Outro, Prospecto, Aluno, Professor, Intermediador
  idCategoriaOutro: z.number().optional().default(1),
  ouAtivo: z.boolean(),
  idCategoriaProspecto: z.number().optional().default(1),
  prAtivo: z.boolean(),
  alTipoResponsavel: z.number().optional().default(1),
  alIdClienteResponsavel: z.number().optional().default(1),
  alDiaPreferenicaPagamento: z.number().min(1).max(31).optional().default(5),
  alDataCadastro: z.string().optional().default(''),
  alAtivo: z.boolean(),
  pfDataCadastro: z.string().optional().default(''),
  pfAtivo: z.boolean(),
  iiIdentificacaoIntermediador: z.string().optional().default(''),
  idCategoriaIntermediador: z.number().optional().default(1),
  icAtivo: z.boolean(),

  // Sub-arrays de Grilhas
  enderecos: z.array(enderecoSchema).default([]),
  telefones: z.array(telefoneSchema).default([]),
  contatos: z.array(contatoSchema).default([]),
  cnaes: z.array(cnaeSchema).default([]),
  empresas: z.array(empresaSchema).default([]),
  cliReferencias: z.array(cliReferenciaSchema).default([]),
  cliBens: z.array(cliBemSchema).default([]),
  cliCartoes: z.array(cliCartaoSchema).default([]),
  cliParentes: z.array(cliParenteSchema).default([]),
  cliSocios: z.array(cliSocioSchema).default([]),
  fornContas: z.array(fornContaSchema).default([]),
  fornFiliais: z.array(fornFilialSchema).default([]),
  transpVeiculos: z.array(transpVeiculoSchema).default([]),
  filialAutorizadosXml: z.array(filialAutorizadoXmlSchema).default([]),
  funcDependentes: z.array(funcDependenteSchema).default([]),
  represClientes: z.array(represClienteSchema).default([]),
  filiaisVinculadas: z.array(filialVinculadaSchema).default([]),
}).superRefine((data, ctx) => {
  // 1. Validação Condicional de CPF (se Pessoa Física)
  if (data.tipoPessoa === 1) {
    if (!data.cpf) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'CPF é obrigatório para Pessoa Física',
        path: ['cpf'],
      });
    } else if (!isValidCPF(data.cpf)) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'CPF inválido',
        path: ['cpf'],
      });
    }
  }

  // 2. Validação Condicional de CNPJ (se Pessoa Jurídica)
  if (data.tipoPessoa === 2) {
    if (!data.cnpj) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'CNPJ é obrigatório para Pessoa Jurídica',
        path: ['cnpj'],
      });
    } else if (!isValidCNPJ(data.cnpj)) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'CNPJ inválido',
        path: ['cnpj'],
      });
    }
  }

  // 3. Validação Condicional de Inscrição Estadual (se PJ ou PF com Carac. Jurídica)
  const precisaInscEstadual = data.tipoPessoa === 2 || data.fisicaTipoJuridica;
  if (precisaInscEstadual && data.contribuinteIcms === 'Sim') {
    if (!data.inscricaoEstadual) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Inscrição Estadual é obrigatória para Contribuinte ICMS',
        path: ['inscricaoEstadual'],
      });
    }
  }

  // 4. Validação Condicional de Cliente (isCliente)
  if (data.isCliente) {
    if (data.cliImovel === 'Alugado' && (!data.cliValorAluguel || data.cliValorAluguel <= 0)) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Valor do aluguel é obrigatório se o imóvel for Alugado',
        path: ['cliValorAluguel'],
      });
    }
  }

  // 5. Validação Condicional de Funcionário (isFuncionario)
  if (data.isFuncionario) {
    if (!data.funcionarioMatricula) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Matrícula é obrigatória',
        path: ['funcionarioMatricula'],
      });
    }
    if (!data.dataAdmissao) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Data de Admissão é obrigatória',
        path: ['dataAdmissao'],
      });
    }
    if (data.dataAdmissao && data.dataDemissao) {
      const admissao = new Date(data.dataAdmissao);
      const demissao = new Date(data.dataDemissao);
      if (demissao < admissao) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: 'Data de Demissão não pode ser anterior à Admissão',
          path: ['dataDemissao'],
        });
      }
    }
  }

  // 6. Validação Condicional de Filial (isFilial)
  if (data.isFilial) {
    if (!data.filialCodigoEmpresa) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Código da Empresa é obrigatório para Filial',
        path: ['filialCodigoEmpresa'],
      });
    }
  }

  // 7. Validação Condicional de Contador (isContador)
  if (data.isContador) {
    if (!data.contadorNome) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'Nome do contador é obrigatório',
        path: ['contadorNome'],
      });
    }
    if (data.contadorCpf && !isValidCPF(data.contadorCpf)) {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: 'CPF do contador inválido',
        path: ['contadorCpf'],
      });
    }
  }
});

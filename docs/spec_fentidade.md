# Spec Funcional: Cadastro de Entidade (FEntidade)

> **Tipo:** Spec Funcional  
> **Versão:** 1.0  
> **Baseado em:** Golden Pattern – este cadastro serve como modelo para todos os outros formulários do sistema.

---

## 1. Objetivo

Gerenciar o cadastro unificado de todas as pessoas (físicas e jurídicas) que se relacionam com a empresa: clientes, fornecedores, funcionários, transportadoras, filiais, representantes, contadores, agências bancárias, instituições financeiras, entre outros.

Uma mesma entidade pode ter **múltiplos papéis simultaneamente** (ex.: uma empresa que é Cliente e Fornecedor ao mesmo tempo).

---

## 2. Endpoints da API

| Método | Rota | Função |
|---|---|---|
| `GET` | `/api/entidade/paginado?page&limit&sortBy&sortOrder&search&role&tipoPessoa` | Listagem paginada com filtros |
| `GET` | `/api/entidade/{id}` | Obter registro simples por ID |
| `GET` | `/api/entidade/completo/{id}` | Obter registro completo (com papéis e sub-dados) |
| `POST` | `/api/entidade` | Criar nova entidade |
| `PUT` | `/api/entidade/{id}` | Atualizar entidade existente |
| `DELETE` | `/api/entidade/{id}` | Remover entidade |

---

## 3. Conversão de Dados (Legado vs. Frontend)

> [!IMPORTANT]
> O banco de dados legado usa valores de enum diferentes dos que o formulário exibe. A conversão bidirecional é obrigatória no `mapBackendToForm` e `mapFormToBackend`.

| Campo | Valor no Banco/API | Valor no Frontend |
|---|---|---|
| `tipoPessoa` Física | `2` | `1` |
| `tipoPessoa` Jurídica | `3` | `2` |

---

## 4. Colunas da Listagem (Grid)

| Coluna | Campo interno | Observação |
|---|---|---|
| Código | `codigo` | Exibe o `idEntidade` |
| Nome / Nome Fantasia | `razaoSocial` | Campo `nome` da API |
| Razão Social | `apelido` | Campo `pessoaJuridica.razaoSocial` da API |
| Tipo Pessoa | `tipoPessoa` | Exibe "Física" ou "Jurídica" |
| CPF / CNPJ | `cpf` / `cnpj` | Formatado com máscara. Exibe CPF se Física, CNPJ se Jurídica |
| Tipo da Entidade | `role` | Chips com os papéis ativos (Cliente, Fornecedor, etc.) |

---

## 5. Campos do Formulário

### 5.1 Cabeçalho (sempre visível)

| Campo | Tipo | Obrigatório | Regra |
|---|---|---|---|
| Código | Texto (readonly) | — | Gerado automaticamente. Exibe `idEntidade` ou "Automático" se novo |
| Nome / Nome Fantasia | Texto | ✅ | Mínimo 3 caracteres |
| Razão Social | Texto | ✅ só PJ | Visível somente se `tipoPessoa = 2` (Jurídica) |
| Tipo Específico | Select | ✅ | Opções: `Geral`, `Produtor Rural`, `Órgão Público`, `Microempreendedor (MEI)` |
| Tipo de Pessoa | Select | ✅ | Opções: `1 = Física (CPF)`, `2 = Jurídica (CNPJ)` |
| Papéis Corporativos | Checkboxes | ✅ (mínimo 1) | `Cliente`, `Fornecedor`, `Funcionário`, `Transportadora`, `Comissionado`, `Filial`, `Agência Bancária`, `Inst. Financeira`, `Obra`, `Representante`, `Outro`, `Prospecto`, `Contador`, `Aluno`, `Professor`, `Intermediador` |

### 5.2 Aba – Dados Gerais

**Campos de Pessoa Física** (visível quando `tipoPessoa = 1`):

| Campo | Tipo | Obrigatório |
|---|---|---|
| CPF | Texto com máscara | Condicional (ver Reg. Neg. 3) |
| RG | Texto | — |
| Órgão Emissor RG | Texto | — |
| Data Emissão RG | Data | — |
| Data de Nascimento | Data | — |
| Sexo | Select | — |
| Estado Civil | Select | — |
| Cidade Nascimento | Texto | — |
| Grau de Instrução | Select | — |
| Pessoa Física c/ caract. Jurídica | Checkbox | — |

**Campos de Pessoa Jurídica** (visível quando `tipoPessoa = 2` ou PF com caract. jurídica):

| Campo | Tipo | Obrigatório |
|---|---|---|
| CNPJ | Texto com máscara | Condicional (ver Reg. Neg. 3) |
| Regime Tributário | Select | — |
| Natureza Jurídica | Número | — |
| Enquadramento | Select | — |

**Campos Fiscais** (visível para PJ ou PF com caract. jurídica):

| Campo | Tipo | Obrigatório |
|---|---|---|
| Inscrição Estadual | Texto | — |
| Inscrição Municipal | Texto | — |
| Inscrição SUFRAMA | Texto | — |
| Inscrição Rural | Texto | — |
| Contribuinte ICMS | Select | — |

### 5.3 Aba – Informações Gerais (sub-abas)

| Sub-aba | Conteúdo |
|---|---|
| 1. Endereços | Grid editável: Tipo, Logradouro, Nº, Bairro, Cidade, UF, CEP |
| 2. Telefones | Grid editável: Tipo, Número, Contato |
| 3. Contatos | Grid editável: Nome, Cargo, E-mail, Celular |
| 4. CNAE | Grid editável: Código, Descrição, Principal (checkbox) |
| 5. Empresas Vinculada | Grid editável: Código Empresa, Nome Empresa, Vinculada (checkbox) |
| E-mail / Internet | Campos: E-mail Principal, Vendas, Compras, Financeiro, NF-e, Home Page, Boleto |

### 5.4 Abas de Papéis (visíveis conforme papéis ativos)

Cada papel ativo no cabeçalho exibe uma aba dedicada com seus campos específicos:

| Aba | Visível quando | Campos chave |
|---|---|---|
| Cliente | `isCliente = true` | Limite crédito, condição pgto, portador, rota venda/entrega, vendedor, referências, bens, cartões |
| Fornecedor | `isFornecedor = true` | Categoria fornecedor, condição pgto, contas bancárias, filiais |
| Funcionário | `isFuncionario = true` | Matrícula, cargo, salário, CNH, CTPS, PIS, FGTS, dependentes |
| Transportadora | `isTransportadora = true` | RNTRC, tipo proprietário/transportador, veículos |
| Filial | `isFilial = true` | Regime ISS, CNAE fiscal, contador, SPED, autorizados XML, filiais vinculadas |
| Representante | `isRepresentante = true` | Categoria, clientes atendidos |
| Contador | `isContador = true` | CPF, nome, CRC |
| Comissionado | `isComissionado = true` | Nome comercial, percentual, tipo |
| Agência Bancária | `isAgencia = true` | Banco, nome, número |
| Inst. Financeira | `isFinanceira = true` | Nome resumido, categoria, conta corrente |
| Obra | `isObra = true` | Cliente, responsável, datas, situação |
| Outro / Prospecto / Aluno / Professor / Intermediador | respectivos flags | Campos específicos de cada papel |

---

## 6. Regras de Negócio (Back-end)

| Nº | Regra | Severidade |
|---|---|---|
| 1 | **Papel obrigatório:** pelo menos um papel corporativo deve ser selecionado. | ❌ Bloqueia |
| 2 | **Filial com PF:** se o papel for Filial e o tipo de pessoa for Física, a flag "Pessoa física com característica jurídica" deve estar marcada. | ❌ Bloqueia |
| 3 | **Funcionário:** deve ser obrigatoriamente Pessoa Física e sem característica jurídica. | ❌ Bloqueia |
| 4 | **Intermediador:** deve ser obrigatoriamente Pessoa Jurídica e ter CNPJ informado. O CNPJ não pode ser igual ao da filial logada. | ❌ Bloqueia |
| 5 | **CPFCNPJOBRIGATORIO (parâmetro):** se o parâmetro do sistema for `BloquearSalvar`, CPF (PF) ou CNPJ (PJ) é obrigatório. | ❌ Bloqueia ou ⚠️ Avisa |
| 6 | **ACEITACNPJCPFINVALIDO (parâmetro):** se o parâmetro for `false`, CPF e CNPJ são validados matematicamente (dígitos verificadores). | ❌ Bloqueia |
| 7 | **TIPOBLOQUEIOCPFCNPJDUPLICADO (parâmetro):** se `BloquearSalvar`, impede cadastro de CPF/CNPJ já existente no banco. | ❌ Bloqueia ou ⚠️ Avisa |
| 8 | **Inscrição Estadual:** se preenchida e diferente de "ISENTO", deve conter de 2 a 14 dígitos numéricos. | ❌ Bloqueia |
| 9 | **Contribuinte Isento:** ao salvar, a Inscrição Estadual é automaticamente substituída por "ISENTO". | Auto |
| 10 | **Entidade estrangeira:** se o endereço principal for de país diferente da filial logada, as validações de CPF/CNPJ são ignoradas. | — |
| 11 | **Auditoria automática:** `dataInclusao`, `horaInclusao`, `idUsuarioInclusao` (criação) e `dataAlteracao`, `horaAlteracao`, `idUsuarioAlteracao` (edição) são preenchidos automaticamente pelo servidor. | Auto |

---

## 7. Filtros disponíveis na listagem

| Filtro | Tipo |
|---|---|
| Código | Texto |
| Razão Social / Nome | Texto |
| Apelido / Fantasia | Texto |
| Tipo de Pessoa | Select: Física / Jurídica |
| Tipo da Entidade (Papel) | Multiselect: todos os papéis |

---

## 8. Arquivos Relacionados

| Papel | Arquivo |
|---|---|
| Tipos e defaultValues | [types.ts](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FEntidade/types.ts) |
| Validação Zod | [schema.ts](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FEntidade/schema.ts) |
| Config OOP (colunas, filtros, mapeamento) | [EntidadeCadastroConfig.tsx](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FEntidade/EntidadeCadastroConfig.tsx) |
| Formulário / View | [index.tsx](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.Frontend/src/pages/AcessoGlobal/FEntidade/index.tsx) |
| Controller C# | [EntidadeController.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Api/Controllers/EntidadeController.cs) |
| DTOs C# | [EntidadeDto.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Domain/DTOs/EntidadeDto.cs) |
| Serviço C# (regras de negócio) | [EntidadeService.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Domain/Services/EntidadeService.cs) |
| Enums C# | [Enums.cs](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/src/Versatus.AcessoGlobal/Domain/Entities/Enums.cs) |

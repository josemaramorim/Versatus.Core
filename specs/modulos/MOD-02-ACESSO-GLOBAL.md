# SPEC — MOD-02: Acesso Global
## Módulo: servidor/objeto de negócio/acesso.global

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 1  
> **Status:** 📝 Rascunho  
> **Prioridade:** ALTA — base de entidades usada por todos os módulos

---

## 1. Visão Geral do Módulo

O módulo **Acesso Global** contém as entidades fundamentais do sistema: clientes, fornecedores,
funcionários, usuários, filiais, configurações de pagamento, e toda a estrutura de acesso
e segurança. É o módulo mais rico em arquivos (301 arquivos .cs) e deve ser o **primeiro
módulo de negócio** a ser migrado após o Framework Base.

### Localização no legado
```
servidor/objeto de negócio/acesso.global/
```

### Dependências deste módulo
- **Recebe:** MOD-01 (Framework Base)
- **É usado por:** todos os outros módulos

---

## 2. Inventário de Classes por Grupo

### 2.1 Entidades de Localização Geográfica

| Classe Legada | Tabela (estimada) | Tipo | Descrição |
|---|---|---|---|
| `Pais.cs` | `GerPais` | Entidade | País |
| `Estado.cs` | `GerEstado` | Entidade | Estado/UF |
| `Cidade.cs` | `GerCidade` | Entidade | Município/Cidade |
| `Bairro.cs` | `GerBairro` | Entidade | Bairro |
| `Endereco.cs` | `GerEndereco` | Entidade | Endereço base |
| `TipoLogradouro.cs` | `GerTipoLogradouro` | Entidade | Tipo de logradouro |

### 2.2 Entidade Base — Entidade (Pessoa Física/Jurídica)

> ⚠️ CRÍTICO: O sistema usa um modelo unificado de "Entidade" para Clientes, Fornecedores,
> Funcionários e outros. NÃO separar em classes distintas sem analisar o banco.

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Entidade.cs` | Entidade base | Pessoa física ou jurídica (90 KB — classe central) |
| `EntidadeFisica.cs` | Complemento | Dados específicos de pessoa física |
| `EntidadeJuridica.cs` | Complemento | Dados específicos de pessoa jurídica |
| `EntidadeEndereco.cs` | Relacionamento | Endereços da entidade |
| `EntidadeEnderecoLista.cs` | Lista | Lista de endereços |
| `EntidadeContato.cs` | Relacionamento | Contatos (email, tel) |
| `EntidadeContatoLista.cs` | Lista | Lista de contatos |
| `EntidadeTelefone.cs` | Relacionamento | Telefones da entidade |
| `EntidadeTelefoneLista.cs` | Lista | Lista de telefones |
| `EntidadeEmpresa.cs` | Relacionamento | Vínculo entidade↔empresa |
| `EntidadeEmpresaLista.cs` | Lista | Lista de vínculos |
| `EntidadeMovimento.cs` | Relacionamento | Movimentos da entidade |
| `EntidadeSaldo.cs` | Saldo | Saldo financeiro da entidade |
| `EntidadeJuridicaCnae.cs` | Relacionamento | CNAEs da entidade jurídica |
| `EntidadeJuridicaCnaeLista.cs` | Lista | Lista de CNAEs |
| `EntidadeLista.cs` | Lista | Lista de entidades |
| `EntidadeOutro.cs` | Entidade | Outros tipos de entidade |

### 2.3 Cliente

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Cliente.cs` | Entidade | Dados específicos de cliente (33 KB) |
| `ClienteFilial.cs` | Relacionamento | Vínculo cliente↔filial |
| `ClienteFilialLista.cs` | Lista | |
| `ClienteBem.cs` | Relacionamento | Bens do cliente |
| `ClienteBemLista.cs` | Lista | |
| `ClienteCartao.cs` | Relacionamento | Cartões do cliente |
| `ClienteCartaoLista.cs` | Lista | |
| `ClienteConceito.cs` | Configuração | Conceito de crédito do cliente |
| `ClienteConceitoFormaPagamento.cs` | Relacionamento | Formas de pagamento permitidas pelo conceito |
| `ClienteConceitoFormaPagamentoLista.cs` | Lista | |
| `ClienteEmpresa.cs` | Relacionamento | Vínculo cliente↔empresa |
| `ClienteParente.cs` | Relacionamento | Parentes do cliente |
| `ClienteReferenciaLista.cs` | Lista | |
| `ClienteSPC.cs` | Entidade | Informações SPC do cliente |
| `ClienteSocio.cs` | Relacionamento | Sócios da empresa cliente |
| `BloqueioCliente.cs` | Entidade | Bloqueio de crédito |
| `AlteracaoLimiteCredito.cs` | Entidade | Histórico de alteração de limite |
| `EstatisticaCliente.cs` | Entidade | Estatísticas de compras (56 KB) |
| `EstatisticaClientePersist.cs` | Persistência | Persiste estatísticas |

### 2.4 Fornecedor

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Fornecedor.cs` | Entidade | Dados do fornecedor (14 KB) |
| `FornecedorFilial.cs` | Relacionamento | Vínculo fornecedor↔filial |
| `FornecedorFilialLista.cs` | Lista | |
| `FornecedorConta.cs` | Relacionamento | Contas bancárias do fornecedor |
| `FornecedorContaLista.cs` | Lista | |
| `FornecedorRepresentante.cs` | Relacionamento | Representantes do fornecedor |

### 2.5 Funcionário

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Funcionario.cs` | Entidade | Dados do funcionário (21 KB) |
| `FuncionarioDependente.cs` | Relacionamento | Dependentes |
| `FuncionarioDependenteLista.cs` | Lista | |
| `FuncionarioFilial.cs` | Relacionamento | Vínculo funcionário↔filial (28 KB) |

### 2.6 Usuário e Segurança

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Usuario.cs` | Entidade | Usuário do sistema (47 KB) |
| `UsuarioFilial.cs` | Relacionamento | Filiais permitidas |
| `UsuarioFilialLista.cs` | Lista | |
| `UsuarioAviso.cs` | Entidade | Avisos ao usuário (22 KB) |
| `UsuarioSuporte.cs` | Entidade | Usuário de suporte |
| `Perfil.cs` | Entidade | Perfil de acesso (16 KB) |
| `PerfilAcesso.cs` | Relacionamento | Acessos do perfil |
| `PerfilAcessoLista.cs` | Lista | |
| `PerfilEmpresa.cs` | Relacionamento | Perfis por empresa |
| `PerfilModulo.cs` | Relacionamento | Módulos do perfil |
| `PerfilModuloLista.cs` | Lista | |
| `PerfilUsuario.cs` | Relacionamento | Usuários do perfil |
| `PerfilUsuarioLista.cs` | Lista | |
| `PerfilMenu.cs` | Relacionamento | Menus do perfil |
| `PerfilMenuLista.cs` | Lista | |
| `PerfilSuporte.cs` | Entidade | Perfil de suporte |

### 2.7 Empresa, Filial e Grupo

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Empresa.cs` | Entidade | Empresa (5 KB) |
| `EmpresaPersistencia.cs` | Entidade | Dados completos da empresa (26 KB) |
| `EmpresaRamo.cs` | Relacionamento | Ramos da empresa |
| `EmpresaRegimeTributoFiscal.cs` | Relacionamento | Regimes tributários |
| `EmpresaTipoNaturezaOperacaoTributoFiscal.cs` | Configuração | Naturezas de operação |
| `Filial.cs` | Entidade | Filial da empresa (31 KB) |
| `FilialPersistencia.cs` | Entidade | Dados completos da filial |
| `FilialAutorizacaoNFeXml.cs` | Configuração | Autorização NF-e |
| `Grupo.cs` | Entidade | Grupo empresarial |
| `GrupoPersistencia.cs` | Entidade | Dados completos do grupo |
| `GrupoAmbienteServidor.cs` | Servidor | Ambiente do grupo no servidor |
| `GrupoCondicaoPagamento.cs` | Configuração | Condições de pagamento do grupo |
| `ImplantacaoFilial.cs` | Processo | Implantação de nova filial (118 KB) |

### 2.8 Formas de Pagamento e Condições

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `FormaPagamento.cs` | Entidade | Forma de pagamento (34 KB) |
| `FormaPagamentoFilial.cs` | Relacionamento | Configuração por filial (15 KB) |
| `FormaPagamentoFilialLista.cs` | Lista | |
| `CondicaoPagamento.cs` | Entidade | Condição de pagamento (59 KB — central!) |
| `CondicaoPagtoParcela.cs` | Relacionamento | Parcelas da condição |
| `CondicaoPagtoRegra.cs` | Relacionamento | Regras da condição |
| `CondicaoPagtoRegraFaixa.cs` | Relacionamento | Faixas das regras |
| `CondicaoPagtoRegraParcelada.cs` | Relacionamento | Regras parceladas |

### 2.9 Estrutura Comercial

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Comissionado.cs` | Entidade | Representante/vendedor (15 KB) |
| `AreaVenda.cs` | Entidade | Área de vendas |
| `Representante.cs` | Entidade | Representante comercial |
| `Rota.cs` | Entidade | Rota de visitas |
| `TipoComissao.cs` | Entidade | Tipo de comissão |
| `TipoComissaoRegra.cs` | Relacionamento | Regras de comissão |

### 2.10 Financeiro Global

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Banco.cs` | Entidade | Banco (21 KB) |
| `Agencia.cs` | Entidade | Agência bancária |
| `Portador.cs` | Entidade | Portador de títulos |
| `HistoricoPadrao.cs` | Entidade | Histórico padrão financeiro |
| `CentroCusto.cs` | Entidade | Centro de custo |
| `CentroCustoUsuario.cs` | Relacionamento | Usuários por centro de custo |
| `IndiceEconomico.cs` | Entidade | Índice econômico (INPC, IPCA etc.) |
| `IndiceEconomicoValor.cs` | Relacionamento | Valores do índice |
| `ItemFinanceiroLancto.cs` | Entidade | Lançamento financeiro |
| `InstituicaoFinanceira.cs` | Entidade | Instituição financeira (14 KB) |
| `BandeiraCartao.cs` | Entidade | Bandeira de cartão |
| `RegraCartao.cs` | Entidade | Regras de cartão |

### 2.11 Configurações e Parâmetros

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Parametro.cs` | Entidade | Parâmetro do sistema (12 KB) |
| `ParametroServidor.cs` | Entidade | Parâmetro do servidor (12 KB) |
| `ParametroValor.cs` | Entidade | Valor do parâmetro |
| `Departamento.cs` | Entidade | Departamento |
| `Categoria.cs` | Entidade | Categoria |
| `Classe.cs` | Entidade | Classe de entidade (16 KB) |
| `CertificadoDigital.cs` | Entidade | Certificado digital (12 KB) |
| `CertificadoDigitalFilial.cs` | Relacionamento | Certificado por filial |
| `EmailConfiguracao.cs` | Entidade | Configuração de email (20 KB) |
| `WebService.cs` | Entidade | WebService configurado |
| `ComputadorDocumentoImpressao.cs` | Configuração | Impressoras por computador |
| `Desktop.cs` | Entidade | Desktop do usuário (12 KB) |
| `Sequencial.cs` | Entidade | Sequencial (configuração) |
| `SequencialItem.cs` | Relacionamento | Item do sequencial |

### 2.12 Outros

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Agenda.cs` | Entidade | Agenda (25 KB) |
| `Recado.cs` | Entidade | Recados/Avisos (15 KB) |
| `Operacao.cs` | Entidade | Operação financeira (47 KB) |
| `OperacaoFilial.cs` | Relacionamento | Operação por filial |
| `Menu.cs` | Entidade | Menu do sistema |
| `Modulo.cs` | Entidade | Módulo do sistema |
| `Aprovacao/` | Subpasta | Workflow de aprovações |
| `Veiculo/` | Subpasta | Veículos vinculados |
| `Saldo/` | Subpasta | Saldos por entidade |
| `SerieDocumento.cs` | Entidade | Série de documentos (40 KB) |
| `SerieDocumentoFilial.cs` | Entidade | Série por filial (52 KB — muito grande) |

---

## 3. Mapeamento Legado → Novo (resumido)

| Classe Legada | Equivalente Novo | Observação |
|---|---|---|
| `Entidade` | `Entidade` (Domain) | Manter como base |
| `EntidadeFisica` | `DadosPessoaFisica` (ValueObject) | Considerar ValueObject |
| `EntidadeJuridica` | `DadosPessoaJuridica` (ValueObject) | Considerar ValueObject |
| `Cliente` | `Cliente` (Domain, herda Entidade) | |
| `Fornecedor` | `Fornecedor` (Domain, herda Entidade) | |
| `Funcionario` | `Funcionario` (Domain, herda Entidade) | |
| `Usuario` | `Usuario` (Domain) | Separar auth em serviço próprio |
| `Perfil` | `Perfil` (Domain) | |
| `Empresa` / `EmpresaPersistencia` | `Empresa` (Domain) | Unificar as duas |
| `Filial` / `FilialPersistencia` | `Filial` (Domain) | Unificar as duas |
| `FormaPagamento` | `FormaPagamento` (Domain) | |
| `CondicaoPagamento` | `CondicaoPagamento` (Domain) | Preservar lógica complexa |
| `Banco` | `Banco` (Domain) | |
| `CentroCusto` | `CentroCusto` (Domain) | |
| `Operacao` | `Operacao` (Domain) | Grande — dividir análise |

---

## 4. Regras de Negócio Críticas (identificadas)

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-02-001 — Entidade Unificada
Cliente, Fornecedor e Funcionário compartilham a mesma tabela base de `Entidade`.
Uma entidade pode ser simultaneamente cliente E fornecedor.
**NÃO** criar tabelas separadas na migração.

### RN-02-002 — Filial e Empresa
Todo dado de negócio é sempre associado a uma filial.
A filial pertence a uma empresa que pertence a um grupo.
Hierarquia: `Grupo → Empresa → Filial`

### RN-02-003 — Sequência de Documentos por Série
`SerieDocumento` e `SerieDocumentoFilial` controlam numeração de documentos.
Esta numeração é independente por filial e por série.
Replicar fielmente — não usar AUTO_INCREMENT do banco.

### RN-02-004 — Condição de Pagamento é Complexa
`CondicaoPagamento.cs` tem 59 KB de lógica com múltiplas regras de parcelamento.
**Só implementar após análise completa** — não simplificar.

### RN-02-005 — Aprovações
O módulo tem um subsistema de aprovações (`Aprovacao/`) para operações que requerem
autorização. Migrar apenas após o fluxo base funcionar.

---

## 5. Ordem de Implementação Recomendada

```
1. Entidades geográficas (Pais, Estado, Cidade, Bairro)
2. Entidade base (Entidade, EntidadeFisica, EntidadeJuridica)
3. Empresa → Filial (hierarquia de contexto)
4. Usuario e Perfil (necessário para contexto JWT)
5. Banco, Agencia, Portador
6. FormaPagamento
7. CondicaoPagamento (analisar bem antes)
8. CentroCusto, Categoria, Classe
9. Cliente (herda Entidade)
10. Fornecedor (herda Entidade)
11. Funcionario (herda Entidade)
12. Comissionado, AreaVenda, Representante
13. Parametro, ParametroServidor
14. SerieDocumento, SerieDocumentoFilial
15. Demais entidades auxiliares
```

---

## 6. Estrutura do Projeto Novo (Versatus.AcessoGlobal)

```
Versatus.AcessoGlobal/
├── Domain/
│   ├── Location/
│   │   ├── Pais.cs
│   │   ├── Estado.cs
│   │   ├── Cidade.cs
│   │   ├── Bairro.cs
│   │   └── Endereco.cs
│   ├── Entities/
│   │   ├── Entidade.cs
│   │   ├── DadosPessoaFisica.cs
│   │   ├── DadosPessoaJuridica.cs
│   │   ├── Cliente.cs
│   │   ├── Fornecedor.cs
│   │   └── Funcionario.cs
│   ├── Organization/
│   │   ├── Grupo.cs
│   │   ├── Empresa.cs
│   │   └── Filial.cs
│   ├── Security/
│   │   ├── Usuario.cs
│   │   └── Perfil.cs
│   ├── Finance/
│   │   ├── Banco.cs
│   │   ├── FormaPagamento.cs
│   │   └── CondicaoPagamento.cs
│   └── Configuration/
│       ├── Parametro.cs
│       └── SerieDocumento.cs
├── Application/
│   ├── Clientes/
│   ├── Fornecedores/
│   ├── Usuarios/
│   └── Configuracoes/
├── Infrastructure/
│   ├── AcessoGlobalDbContext.cs
│   └── Repositories/
└── Api/
    └── Controllers/
```

---

## 7. Checklist de Conclusão do Módulo

Acompanhe o progresso usando o **Roteiro de Tarefas** na seção 8 abaixo. Cada tarefa tem sua própria verificação.

- [x] Fase 1: Setup e Estrutura — Completa
- [ ] Fase 2: Localização Geográfica — Completa
- [ ] Fase 3: Hierarquia de Organização — Completa
- [ ] Fase 4: Segment ação / Classificação — Completa
- [ ] Fase 5: Entidade Base — Completa
- [ ] Fase 6: Especializações (Cliente/Fornecedor/Funcionário) — Completa
- [ ] Fase 7: Segurança e Usuários — Completa
- [ ] Fase 8: Financeiro — Completa
- [ ] Fase 9: Configurações e Série — Completa
- [ ] Fase 10: Repositórios e Context — Completa

---

## 8. Roteiro de Tarefas (Ordem de Execução)

Siga esta sequência de tarefas para implementar MOD-02. Cada tarefa deve resultar em um commit Git, um PR para revisão, e marca de concluído na SPEC.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.AcessoGlobal**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.AcessoGlobal`
- Adicione referência a `Versatus.Framework`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/acesso-global-project` [x]
- Commit: `setup: Create Versatus.AcessoGlobal project with Framework reference` [x]

**Tarefa 1.2 — Criar estrutura de pastas e arquivos base**
- Crie as pastas conforme seção 6: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Localizacao/`, `Entidades/`, `Organizacao/`, `Seguranca/`, `Financeiro/`, `Configuracao/`
- Crie arquivos vazios para cada classe listada
- Branch: `setup/acesso-global-structure` [x]
- Commit: `setup: Create AcessoGlobal folder structure` [x]

**Tarefa 1.3 — Criar DbContext Base**
- Crie `Infrastructure/AcessoGlobalDbContext.cs` (classe vazia por enquanto)
- Branch: `setup/acesso-global-dbcontext-base` [x]
- Commit: `setup: Create AcessoGlobalDbContext base` [x]
- Marque no checklist: ✅ Fase 1 — Setup e Estrutura completa

### Fase 2: Localização Geográfica

**Tarefa 2.1 — Implementar Pais** [x]
- Crie `Domain/Location/Pais.cs` (record simples com propriedades: IdPais, Sigla, Nome)
- Mapeamento EF Core — tabela `GerPais` (verificar nome real)
- Branch: `feat/pais-entity`
- Commit: `feat: Implement Pais geographic entity` [x]

**Tarefa 2.2 — Implementar Estado** [x]
- Crie `Domain/Location/Estado.cs`
- Propriedades: IdEstado, IdPais (FK), Sigla, Nome
- Branch: `feat/estado-entity`
- Commit: `feat: Implement Estado geographic entity` [x]

**Tarefa 2.3 — Implementar Cidade**
- Crie `Domain/Localizacao/Cidade.cs`
- Propriedades: IdCidade, IdEstado (FK), Nome, CodigoIbge
- Branch: `feat/cidade-entity`
- Commit: `feat: Implement Cidade geographic entity`

**Tarefa 2.4 — Implementar Bairro**
- Crie `Domain/Localizacao/Bairro.cs`
- Propriedades: IdBairro, IdCidade (FK), Nome
- Branch: `feat/bairro-entity`
- Commit: `feat: Implement Bairro geographic entity`

**Tarefa 2.5 — Implementar TipoLogradouro**
- Crie `Domain/Localizacao/TipoLogradouro.cs`
- Propriedades: IdTipo, Nome, Abreviatura (ex: Rua, Avenida, Travessa)
- Branch: `feat/tipo-logradouro-entity`
- Commit: `feat: Implement TipoLogradouro entity`

**Tarefa 2.6 — Implementar Endereco**
- Crie `Domain/Localizacao/Endereco.cs`
- Propriedades: IdEndereco, IdTipoLogradouro, IdBairro, IdCidade, Logradouro, Numero, Complemento, CEP
- Branch: `feat/endereco-entity`
- Commit: `feat: Implement Endereco entity`

**Tarefa 2.7 — Configurar Relacionamentos no DbContext**
- Atualize `AcessoGlobalDbContext` com `DbSet<Pais>`, `DbSet<Estado>`, etc.
- Configure ForeignKeys e Cascade Rules
- Branch: `feat/localizacao-dbcontext`
- Commit: `feat: Configure geographic entities in DbContext`
- Marque no checklist: ✅ Fase 2 — Localização Geográfica completa

### Fase 3: Hierarquia de Organização

**Tarefa 3.1 — Implementar Grupo**
- Crie `Domain/Organizacao/Grupo.cs`
- Propriedades: IdGrupo, Nome, Descricao
- Branch: `feat/grupo-entity`
- Commit: `feat: Implement Grupo (group) entity`

**Tarefa 3.2 — Implementar Empresa**
- Crie `Domain/Organizacao/Empresa.cs`
- Propriedades: IdEmpresa, IdGrupo (FK), RazaoSocial, NomeFantasia, Cnpj, Inscricao
- Branch: `feat/empresa-entity`
- Commit: `feat: Implement Empresa entity`

**Tarefa 3.3 — Implementar Filial**
- Crie `Domain/Organizacao/Filial.cs`
- Propriedades: IdFilial, IdEmpresa (FK), NomeDaFilial, Cnpj, Inscricao, IdCidade (FK)
- Branch: `feat/filial-entity`
- Commit: `feat: Implement Filial entity`

**Tarefa 3.4 — Configurar Hierarquia no DbContext**
- Atualize `AcessoGlobalDbContext` com `DbSet<Grupo>`, `DbSet<Empresa>`, `DbSet<Filial>`
- Configure relacionamentos Grupo→Empresa→Filial
- Branch: `feat/organizacao-dbcontext`
- Commit: `feat: Configure organization hierarchy in DbContext`
- Marque no checklist: ✅ Fase 3 — Hierarquia de Organização completa

### Fase 4: Segmentação / Classificação

**Tarefa 4.1 — Implementar CentroCusto**
- Crie `Domain/Configuracao/CentroCusto.cs`
- Propriedades: IdCentroCusto, IdFilial (FK), Codigo, Nome
- Branch: `feat/centro-custo-entity`
- Commit: `feat: Implement CentroCusto entity`

**Tarefa 4.2 — Implementar Categoria**
- Crie `Domain/Configuracao/Categoria.cs`
- Propriedades: IdCategoria, Codigo, Nome, Tipo (enum)
- Branch: `feat/categoria-entity`
- Commit: `feat: Implement Categoria entity`

**Tarefa 4.3 — Implementar Classe**
- Crie `Domain/Configuracao/Classe.cs`
- Propriedades: IdClasse, Codigo, Nome
- Branch: `feat/classe-entity`
- Commit: `feat: Implement Classe entity`

**Tarefa 4.4 — Configurar Segmentação no DbContext**
- Atualize `AcessoGlobalDbContext` com entidades de classificação
- Branch: `feat/segmentacao-dbcontext`
- Commit: `feat: Configure segmentation entities in DbContext`
- Marque no checklist: ✅ Fase 4 — Segmentação completa

### Fase 5: Entidade Base (CRÍTICO)

**Tarefa 5.1 — Analisar Entidade.cs Legado**
- ⚠️ Leia completamente `servidor/objeto de negócio/acesso.global/Entidade.cs` (90 KB)
- Documente todas as propriedades, relacionamentos e métodos
- Documente lógica de Pessoa Física vs. Jurídica
- Branch: `analysis/entidade-base`
- Commit: `docs: Analyze legacy Entidade.cs and document structure`

**Tarefa 5.2 — Implementar Entidade**
- Crie `Domain/Entidades/Entidade.cs`
- Propriedades: IdEntidade, TipoEntidade (enum), RazaoSocial/NomePessoa, CPF/CNPJ, IdEndereco (FK), DataCadastro
- Branch: `feat/entidade-base`
- Commit: `feat: Implement Entidade base entity`

**Tarefa 5.3 — Implementar DadosPessoaFisica**
- Crie `Domain/Entidades/DadosPessoaFisica.cs`
- Propriedades: IdDados, IdEntidade (FK), CPF, RG, NomeMae, DataNascimento
- Branch: `feat/dados-pessoa-fisica`
- Commit: `feat: Implement DadosPessoaFisica complementary entity`

**Tarefa 5.4 — Implementar DadosPessoaJuridica**
- Crie `Domain/Entidades/DadosPessoaJuridica.cs`
- Propriedades: IdDados, IdEntidade (FK), CNPJ, InscricaoEstadual, Tipo (enum)
- Branch: `feat/dados-pessoa-juridica`
- Commit: `feat: Implement DadosPessoaJuridica complementary entity`

**Tarefa 5.5 — Implementar EntidadeEndereco**
- Crie `Domain/Entidades/EntidadeEndereco.cs`
- Propriedades: IdRelation, IdEntidade (FK), IdEndereco (FK), Tipo (enum: Residencial, Comercial, etc.)
- Collection: `IReadOnlyList<EntidadeEndereco> Enderecos` na Entidade
- Branch: `feat/entidade-endereco`
- Commit: `feat: Implement EntidadeEndereco relationship`

**Tarefa 5.6 — Configurar Entidade no DbContext**
- Atualize `AcessoGlobalDbContext` com as 5 entidades acima
- Configure TPT (Table Per Type) ou TPH (Table Per Hierarchy) conforme análise
- Branch: `feat/entidade-dbcontext`
- Commit: `feat: Configure Entidade hierarchy in DbContext`
- Marque no checklist: ✅ Fase 5 — Entidade Base completa

### Fase 6: Especializações (Cliente/Fornecedor/Funcionário)

**Tarefa 6.1 — Implementar Cliente**
- Crie `Domain/Entidades/Cliente.cs` (herda ou compõe Entidade)
- Propriedades adicionais: IdCliente, LimiteCredito, CondicaoPagamento (FK), EstatusBloqueio
- Branch: `feat/cliente-entity`
- Commit: `feat: Implement Cliente specialized entity`

**Tarefa 6.2 — Implementar Fornecedor**
- Crie `Domain/Entidades/Fornecedor.cs`
- Propriedades adicionais: IdFornecedor, ContaBancaria, Ativo
- Branch: `feat/fornecedor-entity`
- Commit: `feat: Implement Fornecedor specialized entity`

**Tarefa 6.3 — Implementar Funcionario**
- Crie `Domain/Entidades/Funcionario.cs`
- Propriedades adicionais: IdFuncionario, Matricula, Admissao, Demissao (nullable)
- Branch: `feat/funcionario-entity`
- Commit: `feat: Implement Funcionario specialized entity`

**Tarefa 6.4 — Configurar Especializações no DbContext**
- Atualize `AcessoGlobalDbContext` com Cliente, Fornecedor, Funcionario
- Branch: `feat/especializacoes-dbcontext`
- Commit: `feat: Configure Client, Supplier, Employee specializations in DbContext`
- Marque no checklist: ✅ Fase 6 — Especializações completa

### Fase 7: Segurança e Usuários

**Tarefa 7.1 — Implementar Perfil**
- Crie `Domain/Seguranca/Perfil.cs`
- Propriedades: IdPerfil, Nome, Permissoes (IReadOnlyList<string>)
- Branch: `feat/perfil-entity`
- Commit: `feat: Implement Perfil (role) entity`

**Tarefa 7.2 — Implementar Usuario**
- Crie `Domain/Seguranca/Usuario.cs`
- Propriedades: IdUsuario, IdFuncionario (FK), Login, PasswordHash, IdPerfil (FK), Ativo, UltimoLogon
- Branch: `feat/usuario-entity`
- Commit: `feat: Implement Usuario entity`

**Tarefa 7.3 — Configurar Segurança no DbContext**
- Atualize `AcessoGlobalDbContext` com Perfil e Usuario
- Branch: `feat/seguranca-dbcontext`
- Commit: `feat: Configure security entities in DbContext`
- Marque no checklist: ✅ Fase 7 — Segurança completa

### Fase 8: Financeiro

**Tarefa 8.1 — Implementar Banco**
- Crie `Domain/Financeiro/Banco.cs`
- Propriedades: IdBanco, Codigo, Nome
- Branch: `feat/banco-entity`
- Commit: `feat: Implement Banco entity`

**Tarefa 8.2 — Implementar FormaPagamento**
- Crie `Domain/Financeiro/FormaPagamento.cs`
- Propriedades: IdForma, Codigo, Nome, Tipo (enum: Dinheiro, Cheque, Cartao, Boleto, Transferencia)
- Branch: `feat/forma-pagamento-entity`
- Commit: `feat: Implement FormaPagamento entity`

**Tarefa 8.3 — Analisar CondicaoPagamento Legado**
- ⚠️ Leia `servidor/objeto de negócio/acesso.global/CondicaoPagamento.cs` (59 KB)
- Documente lógica de parcelamento
- **Não implemente ainda** — marcado como análise para próxima fase
- Branch: `analysis/condicao-pagamento`
- Commit: `docs: Analyze CondicaoPagamento complex logic`

**Tarefa 8.4 — Configurar Financeiro no DbContext**
- Atualize `AcessoGlobalDbContext` com Banco e FormaPagamento
- Branch: `feat/financeiro-dbcontext`
- Commit: `feat: Configure financial entities in DbContext`
- Marque no checklist: ✅ Fase 8 — Financeiro (básico) completa

### Fase 9: Configurações e Série

**Tarefa 9.1 — Implementar SerieDocumento**
- Crie `Domain/Configuracao/SerieDocumento.cs`
- Propriedades: IdSerie, Codigo, Nome, Prefixo, ProximoNumero, Ativa
- Branch: `feat/serie-documento-entity`
- Commit: `feat: Implement SerieDocumento entity`

**Tarefa 9.2 — Implementar SerieDocumentoFilial**
- Crie `Domain/Configuracao/SerieDocumentoFilial.cs`
- Propriedades: IdRelacao, IdSerie (FK), IdFilial (FK), ProximoNumero
- Branch: `feat/serie-documento-filial-entity`
- Commit: `feat: Implement SerieDocumentoFilial relationship`

**Tarefa 9.3 — Implementar Parametro**
- Crie `Domain/Configuracao/Parametro.cs`
- Propriedades: IdParam, Chave, Valor, Tipo (string, int, bool)
- Branch: `feat/parametro-entity`
- Commit: `feat: Implement Parametro configuration entity`

**Tarefa 9.4 — Configurar Série e Config no DbContext**
- Atualize `AcessoGlobalDbContext` com SerieDocumento, SerieDocumentoFilial, Parametro
- Branch: `feat/config-dbcontext`
- Commit: `feat: Configure configuration and series entities in DbContext`
- Marque no checklist: ✅ Fase 9 — Configurações completa

### Fase 10: Repositórios e Context

**Tarefa 10.1 — Criar Repositórios para Entidades-Chave**
- Crie `Infrastructure/Repositorio/IClienteRepository.cs` e `ClienteRepository.cs`
- Crie `Infrastructure/Repositorio/IFornecedorRepository.cs` e `FornecedorRepository.cs`
- Crie `Infrastructure/Repositorio/IUsuarioRepository.cs` e `UsuarioRepository.cs`
- Branch: `feat/repositorios-chave`
- Commit: `feat: Implement repository interfaces and implementations for key entities`

**Tarefa 10.2 — Configurar Migrations EF Core**
- Crie primeira migration do `AcessoGlobalDbContext`
- Validar que todas as tabelas e relacionamentos estão corretos
- Branch: `setup/acesso-global-migrations`
- Commit: `setup: Create initial EF Core migrations for AcessoGlobal`

**Tarefa 10.3 — Registrar DI e Serviços**
- Crie método de extensão para registrar contexto e repositórios no IServiceCollection
- Branch: `setup/acesso-global-di`
- Commit: `setup: Configure dependency injection for AcessoGlobal`
- Marque no checklist: ✅ Fase 10 — Repositórios e Context completa

---

## Resumo de Commits Esperados

```
1. setup: Create Versatus.AcessoGlobal project with Framework reference
2. setup: Create AcessoGlobal folder structure
3. setup: Create AcessoGlobalDbContext base
4. feat: Implement Pais geographic entity
5. feat: Implement Estado geographic entity
6. feat: Implement Cidade geographic entity
7. feat: Implement Bairro geographic entity
8. feat: Implement TipoLogradouro entity
9. feat: Implement Endereco entity
10. feat: Configure geographic entities in DbContext
11. feat: Implement Grupo (group) entity
12. feat: Implement Empresa entity
13. feat: Implement Filial entity
14. feat: Configure organization hierarchy in DbContext
15. feat: Implement CentroCusto entity
16. feat: Implement Categoria entity
17. feat: Implement Classe entity
18. feat: Configure segmentation entities in DbContext
19. docs: Analyze legacy Entidade.cs and document structure
20. feat: Implement Entidade base entity
21. feat: Implement DadosPessoaFisica complementary entity
22. feat: Implement DadosPessoaJuridica complementary entity
23. feat: Implement EntidadeEndereco relationship
24. feat: Configure Entidade hierarchy in DbContext
25. feat: Implement Cliente specialized entity
26. feat: Implement Fornecedor specialized entity
27. feat: Implement Funcionario specialized entity
28. feat: Configure Client, Supplier, Employee specializations in DbContext
29. feat: Implement Perfil (role) entity
30. feat: Implement Usuario entity
31. feat: Configure security entities in DbContext
32. feat: Implement Banco entity
33. feat: Implement FormaPagamento entity
34. docs: Analyze CondicaoPagamento complex logic
35. feat: Configure financial entities in DbContext
36. feat: Implement SerieDocumento entity
37. feat: Implement SerieDocumentoFilial relationship
38. feat: Implement Parametro configuration entity
39. feat: Configure configuration and series entities in DbContext
40. feat: Implement repository interfaces and implementations for key entities
41. setup: Create initial EF Core migrations for AcessoGlobal
42. setup: Configure dependency injection for AcessoGlobal
```

---

## Histórico de Alterações
- [ ] Usuário e Perfil criados com JWT
- [ ] Banco e Formas de Pagamento criados
- [ ] CondicaoPagamento analisada e implementada
- [ ] Cliente, Fornecedor, Funcionário criados
- [ ] Parâmetros do sistema criados
- [ ] Todos os repositórios implementados
- [ ] Endpoints API criados e documentados (Swagger)
- [ ] Testes de integração cobrindo CRUD
- [ ] Testes de paridade com legado executados

---

## Histórico de Alterações

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise de `servidor/objeto de negócio/acesso.global/` — 301 arquivos — `projeto_tag_1906`*

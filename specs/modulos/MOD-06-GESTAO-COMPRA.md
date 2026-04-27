# SPEC — MOD-06: Gestão de Compras
## Módulo: servidor/objeto de negócio/gestao.compra

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 2  
> **Status:** 📝 Rascunho  
> **Prioridade:** MÉDIA-ALTA

---

## 1. Visão Geral do Módulo

O módulo de **Gestão de Compras** controla o ciclo completo de compras:
solicitações, cotações, pedidos de compra (recebimentos) e devoluções ao fornecedor.

### Localização no legado
```
servidor/objeto de negócio/gestao.compra/
```

### Dependências
- **Requer:** MOD-01, MOD-02 (fornecedores), MOD-03 (produtos)
- **Alimenta:** MOD-05 (gera contas a pagar), MOD-03 (entrada em estoque)

---

## 2. Inventário de Classes por Grupo

### 2.1 Cotação

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TipoDoctoCotacao.cs` | 13 KB | Configuração | Tipo de documento de cotação |
| `TipoDoctoCotacaoImpresso.cs` | 7 KB | Relacionamento | Impresso da cotação |
| `TipoDoctoCotacaoPerfil.cs` | 6 KB | Relacionamento | Perfil da cotação |
| `Cotacao.cs` | **32 KB** | **Entidade Central** | Cotação com fornecedores |
| `CotacaoEstoque.cs` | 24 KB | Relacionamento | Produtos na cotação |
| `CotacaoEstoqueLista.cs` | 2 KB | Lista | |
| `CotacaoEstoquePreco.cs` | 13 KB | Relacionamento | Preços cotados |
| `CotacaoFornecedor.cs` | 8 KB | Relacionamento | Fornecedor na cotação |
| `CotacaoFornecedorLista.cs` | 1 KB | Lista | |
| `CotacaoFornecedorEscolhido.cs` | 4 KB | Relacionamento | Fornecedor escolhido |
| `CotacaoCentroCusto.cs` | 10 KB | Relacionamento | Centro de custo |
| `CotacaoProjeto.cs` | 9 KB | Relacionamento | Projeto da cotação |
| `CotacaoSolicitacao.cs` | 7 KB | Relacionamento | Solicitação vinculada |
| `CotacaoDoctoCompra.cs` | 6 KB | Relacionamento | Documento de compra gerado |

### 2.2 Recebimento (Nota Fiscal de Compra)

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TipoDocumentoCompra.cs` | **79 KB** | Configuração | Tipo de documento de compra (GRANDE) |
| `TipoDocumentoCompraImpresso.cs` | 9 KB | Relacionamento | Impresso |
| `TipoDocumentoCompraNaturezaOperacao.cs` | 6 KB | Relacionamento | Naturezas de operação |
| `TipoDocumentoCompraPerfil.cs` | 4 KB | Relacionamento | Perfis |
| `TipoDocumentoCompraClasseTipoProduto.cs` | 4 KB | Relacionamento | Classes de produto |
| `Recebimento.cs` | **170 KB** | **Entidade Central** | Nota fiscal de compra (ENORME) |
| `RecebimentoItem.cs` | **86 KB** | Entidade | Item do recebimento |
| `RecebimentoItemLista.cs` | 18 KB | Lista | |
| `RecebimentoItemSituacao.cs` | 14 KB | Situação | Estado do item |
| `RecebimentoItemTributo.cs` | 7 KB | Relacionamento | Tributos do item |
| `RecebimentoItemLote.cs` | 11 KB | Relacionamento | Lote do item |
| `RecebimentoItemSerie.cs` | 7 KB | Relacionamento | Série do item |
| `RecebimentoItemLocalizacao.cs` | 4 KB | Relacionamento | Localização no estoque |
| `RecebimentoItemAtendido.cs` | 4 KB | Relacionamento | Atendimento do item |
| `RecebimentoSituacao.cs` | 33 KB | Situação | Estado do recebimento |
| `RecebimentoParcela.cs` | 13 KB | Relacionamento | Parcelas financeiras |
| `RecebimentoParcelaLista.cs` | 3 KB | Lista | |
| `RecebimentoObservacao.cs` | 3 KB | Relacionamento | Observações |
| `RecebimentoAtendimento.cs` | 3 KB | Relacionamento | Atendimentos |
| `RecebimentoTotalLista.cs` | 3 KB | Lista de totais | |
| `RecebimentoUpdateOrigem.cs` | 2 KB | Auxiliar | Atualizar origem |
| `RecebimentoCfg.cs` | 6 KB | Configuração | Configurações extras |

### 2.3 Solicitação de Compra

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Solicitacao.cs` | Entidade | Solicitação de compra (16 KB) |
| `SolicitacaoEstoque.cs` | Relacionamento | Produtos solicitados (20 KB) |
| `SolicitacaoEstoqueLista.cs` | Lista | |

### 2.4 Requisição Interna

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Requisicao.cs` | Entidade | Requisição interna (22 KB) |
| `Requisitante.cs` | Entidade | Solicitante da requisição (16 KB) |
| `RequisitanteCentroCusto.cs` | Relacionamento | Centro de custo |
| `RequisitanteProjeto.cs` | Relacionamento | Projeto |
| `RequisicaoEstoque.cs` | Relacionamento | Produtos requisitados (23 KB) |
| `RequisicaoEstoqueComposto.cs` | Relacionamento | Compostos |
| `RequisicaoEstoqueLista.cs` | Lista | |
| `RequisicaoMovimento.cs` | Entidade | Movimento da requisição (16 KB) |
| `RequisicaoMovimentoItem.cs` | Relacionamento | Item do movimento (14 KB) |

### 2.5 Auxiliares

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `CompraTributo.cs` | Entidade | Tributo na compra |
| `ProdutoFornecedor.cs` | Relacionamento | Produto × Fornecedor (11 KB) |

---

## 3. Regras de Negócio Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-06-001 — Ciclo de Compra
O fluxo normal é: Solicitação → Cotação → Recebimento
A cotação pode gerar automaticamente o recebimento quando aprovada.

### RN-06-002 — Recebimento gera Financeiro
Ao confirmar um recebimento, `RecebimentoSituacao` gera as contas a pagar em MOD-05.

### RN-06-003 — Recebimento movimenta Estoque
A confirmação do recebimento gera entrada em estoque via MOD-03.

### RN-06-004 — Tributos na Entrada
O `RecebimentoItemTributo` captura ICMS, IPI, PIS, COFINS da nota de compra.
Alguns tributos recuperáveis afetam o custo do produto.

---

## 4. Ordem de Implementação Recomendada

```
1. Cotação e sua configuração de tipos de documento
2. TipoDocumentoCompra e configurações de nota fiscal de compra
3. Recebimento e seus itens
4. Situação do recebimento e transições
5. Solicitação de compra e requisição interna
6. Integração com MOD-05 (Financeiro) e MOD-03 (Estoque)
7. Tributos de compra e produto-fornecedor
8. Repositórios, DB Context e migrations
9. Testes de integração do ciclo completo
```

---

## 5. Roteiro de Tarefas (Ordem de Execução)

Siga a sequência abaixo para implementar MOD-06. Cada tarefa deve gerar um commit Git e um PR de revisão. Mantenha a análise do legado antes de implementar os arquivos grandes e use `Result<T>`/`ValidationResult` para validações de regras de negócio.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.GestaoCompra**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.GestaoCompra`
- Adicione referências a `Versatus.Framework`, `Versatus.AcessoGlobal`, `Versatus.GestaoMaterial`, `Versatus.Faturamento`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/gestao-compra-project`
- Commit: `setup: Create Versatus.GestaoCompra project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Cotacao/`, `Recebimento/`, `Solicitacao/`, `Requisicao/`, `Auxiliares/`
- Branch: `setup/gestao-compra-structure`
- Commit: `setup: Create GestaoCompra folder structure`

**Tarefa 1.3 — Criar DbContext base**
- Crie `Infrastructure/GestaoCompraDbContext.cs` com `DbSet` vazios inicialmente
- Branch: `setup/gestao-compra-dbcontext-base`
- Commit: `setup: Create GestaoCompraDbContext base`

### Fase 2: Cotação

**Tarefa 2.1 — Implementar TipoDoctoCotacao e configurações**
- Crie `Domain/Cotacao/TipoDoctoCotacao.cs`, `TipoDoctoCotacaoImpresso.cs`, `TipoDoctoCotacaoPerfil.cs`
- Defina regras de impressão e perfis para cotação
- Branch: `feat/cotacao-tipo-documento`
- Commit: `feat: Implement cotacao document type entities`

**Tarefa 2.2 — Implementar Cotacao**
- Crie `Domain/Cotacao/Cotacao.cs`, `CotacaoEstoque.cs`, `CotacaoEstoquePreco.cs`, `CotacaoFornecedor.cs`, `CotacaoFornecedorEscolhido.cs`, `CotacaoCentroCusto.cs`, `CotacaoProjeto.cs`, `CotacaoSolicitacao.cs`, `CotacaoDoctoCompra.cs`
- Use coleções genéricas para todos os relacionamentos
- Branch: `feat/cotacao-entity`
- Commit: `feat: Implement Cotacao entity and relationships`

**Tarefa 2.3 — Configurar cotação no DbContext**
- Atualize `GestaoCompraDbContext` com os DbSet de cotação
- Branch: `feat/cotacao-dbcontext`
- Commit: `feat: Configure cotacao entities in DbContext`
- Marque no checklist: ✅ Fase 2 completa

### Fase 3: Tipo de Documento de Compra

**Tarefa 3.1 — Analisar TipoDocumentoCompra.cs**
- Leia completamente `servidor/objeto de negócio/gestao.compra/TipoDocumentoCompra.cs` (79 KB)
- Documente regras de operação, perfil e natureza de operação
- Branch: `analysis/tipo-documento-compra`
- Commit: `docs: Analyze legacy TipoDocumentoCompra behavior`

**Tarefa 3.2 — Implementar TipoDocumentoCompra e configurações**
- Crie `Domain/Recebimento/TipoDocumentoCompra.cs`, `TipoDocumentoCompraImpresso.cs`, `TipoDocumentoCompraNaturezaOperacao.cs`, `TipoDocumentoCompraPerfil.cs`, `TipoDocumentoCompraClasseTipoProduto.cs`
- Garanta suporte às configurações fiscais do documento de compra
- Branch: `feat/tipo-documento-compra-entity`
- Commit: `feat: Implement TipoDocumentoCompra entities`

**Tarefa 3.3 — Configurar tipo de documento no DbContext**
- Atualize `GestaoCompraDbContext` com os DbSet de tipo de documento
- Branch: `feat/tipo-compra-dbcontext`
- Commit: `feat: Configure purchase document type entities in DbContext`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: Recebimento de Compra

**Tarefa 4.1 — Analisar Recebimento.cs**
- Leia completamente `servidor/objeto de negócio/gestao.compra/Recebimento.cs` (170 KB)
- Documente as propriedades, regras de cálculo, comportamento de confirmação e efeitos de estoque/financeiro
- Branch: `analysis/recebimento-critico`
- Commit: `docs: Complete analysis of legacy Recebimento.cs`

**Tarefa 4.2 — Analisar RecebimentoItem.cs**
- Leia completamente `servidor/objeto de negócio/gestao.compra/RecebimentoItem.cs` (86 KB)
- Documente cálculos, tributos, lotes, séries e localizações
- Branch: `analysis/recebimento-item`
- Commit: `docs: Analyze legacy RecebimentoItem.cs`

**Tarefa 4.3 — Implementar Recebimento**
- Crie `Domain/Recebimento/Recebimento.cs`, `RecebimentoItem.cs`, `RecebimentoItemSituacao.cs`, `RecebimentoItemTributo.cs`, `RecebimentoItemLote.cs`, `RecebimentoItemSerie.cs`, `RecebimentoItemLocalizacao.cs`, `RecebimentoItemAtendido.cs`, `RecebimentoSituacao.cs`, `RecebimentoParcela.cs`, `RecebimentoObservacao.cs`, `RecebimentoAtendimento.cs`, `RecebimentoCfg.cs`
- Use `IReadOnlyList<T>` para coleções e mantenha a paridade com o legado
- Branch: `feat/recebimento-entity`
- Commit: `feat: Implement Recebimento entity and related structures`

**Tarefa 4.4 — Configurar recebimento no DbContext**
- Atualize `GestaoCompraDbContext` com os DbSet de recebimento
- Branch: `feat/recebimento-dbcontext`
- Commit: `feat: Configure Recebimento entities in DbContext`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: Situação e Transições de Recebimento

**Tarefa 5.1 — Implementar RecebimentoSituacao**
- Crie arquitetura de transições de estado para `RecebimentoSituacao`
- Inclua etapas como criação, confirmação, estorno e cancelamento
- Branch: `feat/recebimento-situacao-entity`
- Commit: `feat: Implement RecebimentoSituacao state entity`

**Tarefa 5.2 — Implementar handlers de situação**
- Crie handlers de aplicação para confirmação, estorno e atualização de situação
- Use `Result<T>`/`ValidationResult` para validações de regras de negócio
- Branch: `feat/recebimento-handlers`
- Commit: `feat: Implement Recebimento state handlers`

**Tarefa 5.3 — Testar transições de situação**
- Adicione testes de estado para confirmar, estornar e cancelar recebimentos
- Branch: `test/recebimento-situacao`
- Commit: `test: Add Recebimento state transition tests`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Solicitação e Requisição Interna

**Tarefa 6.1 — Implementar Solicitacao de Compra**
- Crie `Domain/Solicitacao/Solicitacao.cs`, `SolicitacaoEstoque.cs`
- Garanta a conexão com produtos e centro de custo
- Branch: `feat/solicitacao-compra-entity`
- Commit: `feat: Implement purchase request entities`

**Tarefa 6.2 — Implementar Requisição Interna**
- Crie `Domain/Requisicao/Requisicao.cs`, `Requisitante.cs`, `RequisitanteCentroCusto.cs`, `RequisitanteProjeto.cs`, `RequisicaoEstoque.cs`, `RequisicaoEstoqueComposto.cs`, `RequisicaoMovimento.cs`, `RequisicaoMovimentoItem.cs`
- Branch: `feat/requisicao-entity`
- Commit: `feat: Implement internal requisition entities`

**Tarefa 6.3 — Configurar solicitações e requisições no DbContext**
- Atualize `GestaoCompraDbContext` com os DbSet de solicitação e requisição
- Branch: `feat/solicitacao-requisicao-dbcontext`
- Commit: `feat: Configure request entities in DbContext`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Integração com Financeiro e Estoque

**Tarefa 7.1 — Implementar integração com MOD-05 Financeiro**
- Crie ponte para geração de contas a pagar no `RecebimentoSituacao` quando confirmado
- Teste a criação de `DocumentoParcela` no fluxo de compra
- Branch: `feat/compra-financeiro-integration`
- Commit: `feat: Implement purchase to finance integration`

**Tarefa 7.2 — Implementar integração com MOD-03 Estoque**
- Crie lógica para entrada em estoque no recebimento confirmado
- Inclua controle de lote/série quando exigido pelo produto
- Branch: `feat/compra-estoque-integration`
- Commit: `feat: Implement purchase to stock integration`

**Tarefa 7.3 — Testar integrações críticas**
- Adicione testes de integração para recebimento → estoque → financeiro
- Branch: `test/compra-integrations`
- Commit: `test: Add purchase integration tests`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Tributos e Produto-Fornecedor

**Tarefa 8.1 — Implementar CompraTributo**
- Crie `Domain/Auxiliares/CompraTributo.cs`
- Garanta que ICMS, IPI, PIS e COFINS sejam registrados por item de recebimento
- Branch: `feat/compra-tributo-entity`
- Commit: `feat: Implement purchase tax entity`

**Tarefa 8.2 — Implementar ProdutoFornecedor**
- Crie `Domain/Auxiliares/ProdutoFornecedor.cs`
- Relacione produto e fornecedor com preços e condições de compra
- Branch: `feat/produto-fornecedor-entity`
- Commit: `feat: Implement product-supplier entity`

**Tarefa 8.3 — Configurar auxiliares no DbContext**
- Atualize `GestaoCompraDbContext` com `CompraTributo` e `ProdutoFornecedor`
- Branch: `feat/auxiliares-dbcontext`
- Commit: `feat: Configure purchase auxiliary entities in DbContext`
- Marque no checklist: ✅ Fase 8 completa

### Fase 9: Repositórios e Migrations

**Tarefa 9.1 — Criar repositórios chave**
- Crie `Infrastructure/Repositorios/ICotacaoRepository.cs`, `IRecebimentoRepository.cs`, `ISolicitacaoRepository.cs`, `IRequisicaoRepository.cs`
- Branch: `feat/compra-repositories`
- Commit: `feat: Implement purchase repository interfaces`

**Tarefa 9.2 — Criar migrations EF Core**
- Crie a primeira migration para `GestaoCompraDbContext`
- Valide todas as tabelas e relacionamentos
- Branch: `setup/gestao-compra-migrations`
- Commit: `setup: Create initial EF Core migrations for GestaoCompra`

**Tarefa 9.3 — Registrar DI**
- Crie método de extensão para registrar DbContext e repositórios
- Branch: `setup/gestao-compra-di`
- Commit: `setup: Configure dependency injection for GestaoCompra`
- Marque no checklist: ✅ Fase 9 completa

### Fase 10: Testes de Paridade e Integração

**Tarefa 10.1 — Testar cenários de recebimento e cotação**
- Crie testes para fluxo de cotação, pedido e recebimento completo
- Branch: `test/compra-parity`
- Commit: `test: Add purchase parity tests`

**Tarefa 10.2 — Testar fluxo financeiro e estoque**
- Adicione testes de ponta a ponta com integração a MOD-05 e MOD-03
- Branch: `test/compra-e2e`
- Commit: `test: Add purchase end-to-end integration tests`
- Marque no checklist: ✅ Fase 10 completa

---

## 6. Checklist de Conclusão do Módulo

- [ ] Fase 1: Setup e Estrutura — Completa
- [ ] Fase 2: Cotação — Completa
- [ ] Fase 3: Tipo de Documento de Compra — Completa
- [ ] Fase 4: Recebimento — Completa
- [ ] Fase 5: Situação e Transições — Completa
- [ ] Fase 6: Solicitação e Requisição — Completa
- [ ] Fase 7: Integrações Financeiro/Estoque — Completa
- [ ] Fase 8: Tributos e Produto-Fornecedor — Completa
- [ ] Fase 9: Repositórios e Migrations — Completa
- [ ] Fase 10: Testes de Paridade e Integração — Completa

---

## 7. Checklist de Conclusão do Módulo

- [ ] `Recebimento.cs` analisado (170 KB)
- [ ] `RecebimentoItem.cs` analisado (86 KB)
- [ ] `TipoDocumentoCompra.cs` analisado (79 KB)
- [ ] Solicitação e Requisição implementadas
- [ ] Cotação implementada
- [ ] Recebimento implementado com geração de estoque e financeiro
- [ ] Testes de integração do ciclo compra completo

---

- [ ] `Recebimento.cs` analisado (170 KB)
- [ ] `RecebimentoItem.cs` analisado (86 KB)
- [ ] `TipoDocumentoCompra.cs` analisado (79 KB)
- [ ] Solicitação e Requisição implementadas
- [ ] Cotação implementada
- [ ] Recebimento implementado com geração de estoque e financeiro
- [ ] Testes de integração do ciclo compra completo

---

## Histórico de Alterações

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise de `servidor/objeto de negócio/gestao.compra/` — 75 arquivos — `projeto_tag_1906`*

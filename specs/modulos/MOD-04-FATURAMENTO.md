# SPEC — MOD-04: Faturamento
## Módulo: servidor/objeto de negócio/faturamento

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 3  
> **Status:** 📝 Rascunho  
> **Prioridade:** ALTA — coração do negócio

---

## 1. Visão Geral do Módulo

O módulo de **Faturamento** é o coração operacional do Versatus. Cobre todo o ciclo de vendas:
pedidos, orçamentos, notas fiscais, devoluções, tabelas de preço e comissões.
É o módulo mais complexo do sistema, com classes que chegam a 584 KB (`DocumentoVenda.cs`).

### Localização no legado
```
servidor/objeto de negócio/faturamento/
```

### Dependências
- **Requer:** MOD-01, MOD-02 (Acesso Global), MOD-03 (Material), MOD-07 (Tributo)
- **Alimenta:** MOD-05 (Financeiro — gera contas a receber), MOD-08 (NFe)

---

## 2. Inventário de Classes por Grupo

### 2.1 Documento de Venda — NÚCLEO DO MÓDULO

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `DocumentoVenda.cs` | **584 KB** | **Entidade Central** | Documento de venda (ENORME) |
| `DocumentoVendaSituacao.cs` | 70 KB | **Situação** | Operações de estado da venda |
| `DocumentoVendaLista.cs` | 1 KB | Lista | |
| `DocumentoVendaEndereco.cs` | 14 KB | Relacionamento | Endereços do documento |
| `DocumentoVendaEnderecoLista.cs` | 1 KB | Lista | |
| `DocumentoVendaParcela.cs` | 10 KB | Relacionamento | Parcelas financeiras |
| `DocumentoVendaParcelaLista.cs` | 3 KB | Lista | |
| `DocumentoVendaParcelaForma.cs` | 3 KB | Relacionamento | Forma de pagamento da parcela |
| `DocumentoVendaTransporte.cs` | 4 KB | Relacionamento | Dados de transporte |
| `DocumentoVendaTransporteLista.cs` | 2 KB | Lista | |
| `DocumentoVendaTotal.cs` | 0.9 KB | Entidade | Totais calculados |
| `DocumentoVendaTotalLista.cs` | 3 KB | Lista | |
| `DocumentoVendaAtender.cs` | 2 KB | Auxiliar | Atender documento |
| `DocumentoVendaAtendimento.cs` | 4 KB | Relacionamento | Atendimento do documento |
| `DocumentoVendaAtendimentoLista.cs` | 2 KB | Lista | |
| `DocumentoVendaAtendimentoOutros.cs` | 6 KB | Relacionamento | Outros atendimentos |
| `DocumentoVendaComissionado.cs` | 8 KB | Relacionamento | Comissionados da venda |
| `DocumentoVendaComissionadoLista.cs` | 5 KB | Lista | |
| `DocumentoVendaDefensivoAgricola.cs` | 7 KB | Relacionamento | Dados de defensivos agrícolas |
| `DocumentoVendaAutorizacaoNFeXml.cs` | 3 KB | Relacionamento | Autorização NF-e |
| `DocumentoVendaUpdateOrigem.cs` | 3 KB | Auxiliar | Atualizar origem do documento |

### 2.2 Item do Documento de Venda

> Nota: As classes `*Lista.cs` são artefatos do legado. No novo sistema, não criar classes `*Lista`; usar coleções genéricas nativas do .NET (`List<T>`, `IReadOnlyList<T>`) para representar listas de itens.

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `DocumentoItemVenda.cs` | **174 KB** | **Entidade Central** | Item da venda (MUITO GRANDE) |
| `DocumentoItemVendaLista.cs` | 117 KB | Lista | Lista de itens (também enorme!) |
| `DocumentoItemVendaSituacao.cs` | 23 KB | Situação | Estado do item |
| `DocumentoItemVendaTributo.cs` | 7 KB | Relacionamento | Tributos do item |
| `DocumentoItemVendaTributoLista.cs` | 1 KB | Lista | |
| `DocumentoItemVendaComposto.cs` | 5 KB | Relacionamento | Item composto (kit) |
| `DocumentoItemVendaCompostoLista.cs` | 2 KB | Lista | |
| `DocumentoItemVendaImportacao.cs` | 17 KB | Relacionamento | Dados de importação |
| `DocumentoItemVendaImportacaoAdicao.cs` | 7 KB | Relacionamento | Adição de importação |
| `DocumentoItemComissionado.cs` | 9 KB | Relacionamento | Comissionados do item |
| `DocumentoItemComissionadoLista.cs` | 4 KB | Lista | |
| `DocumentoItemLocalizacao.cs` | 6 KB | Relacionamento | Localização no estoque |
| `DocumentoItemLocalizacaoDistribuicao.cs` | 7 KB | Relacionamento | Distribuição por localização |
| `DocumentoItemLote.cs` | 13 KB | Relacionamento | Lote do item |
| `DocumentoItemLoteLista.cs` | 1 KB | Lista | |
| `DocumentoItemSerie.cs` | 7 KB | Relacionamento | Série do item |
| `DocumentoItemSerieLista.cs` | 2 KB | Lista | |
| `DocumentoItemAtendidoVenda.cs` | 5 KB | Relacionamento | Documento atendido |

### 2.3 Tipo de Documento de Venda (Configuração)

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TipoDocumentoVenda.cs` | **88 KB** | Entidade de Config | Configuração do tipo de documento (GRANDE) |
| `TipoDocumentoVendaNaturezaOperacao.cs` | 6 KB | Relacionamento | Naturezas de operação |
| `TipoDocumentoVendaNaturezaOperacaoLista.cs` | 2 KB | Lista | |
| `TipoDocumentoVendaObservacao.cs` | 8 KB | Relacionamento | Observações do tipo |
| `TipoDocumentoVendaObservacaoLista.cs` | 1 KB | Lista | |
| `TipoDocumentoVendaClasseTipoProduto.cs` | 4 KB | Relacionamento | Classes de produto permitidas |
| `TipoDocumentoImpresso.cs` | 10 KB | Relacionamento | Impressos do tipo |
| `TipoDocumentoSerieComputador.cs` | 7 KB | Relacionamento | Série por computador |
| `TipoDocumentoPerfil.cs` | 3 KB | Relacionamento | Perfis do tipo |
| `TipoDocumentoItemFinanceiro.cs` | 5 KB | Relacionamento | Itens financeiros |

### 2.4 Tipos Específicos de Documento

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TipoFaturamento.cs` | 48 KB | Configuração | Configuração do faturamento |
| `TipoNotaFiscal.cs` | 39 KB | Configuração | Configuração de NF-e de saída |
| `TipoNotaFiscalEntrada.cs` | 46 KB | Configuração | Configuração de NF-e de entrada |
| `TipoOrcamento.cs` | 9 KB | Configuração | Configuração de orçamento |
| `TipoPedido.cs` | 12 KB | Configuração | Configuração de pedido |

### 2.5 Devolução de Venda

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `DevolucaoVenda.cs` | 29 KB | Entidade | Devolução de venda |
| `DevolucaoVendaItem.cs` | 6 KB | Relacionamento | Item da devolução |
| `DevolucaoVendaItemLista.cs` | 1 KB | Lista | |
| `DevolucaoVendaAcerto.cs` | 4 KB | Auxiliar | Acerto da devolução |
| `DevolucaoCond.cs` | 8 KB | Entidade | Devolução condicional |
| `DevolucaoItemCond.cs` | 3 KB | Relacionamento | Item da devolução condicional |

### 2.6 Tabela de Preço

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TabelaPreco.cs` | 36 KB | Entidade | Tabela de preços |
| `TabelaPrecoEstoque.cs` | 36 KB | Relacionamento | Preços por produto |
| `TabelaPrecoEstoqueLista.cs` | 2 KB | Lista | |
| `TabelaPrecoRegra.cs` | 8 KB | Relacionamento | Regras da tabela |
| `TabelaPrecoRegraLista.cs` | 1 KB | Lista | |
| `TabelaPrecoAreaVenda.cs` | 3 KB | Relacionamento | Área de venda |
| `TabelaPrecoCliente.cs` | 3 KB | Relacionamento | Preços especiais por cliente |
| `TabelaPrecoComissionado.cs` | 4 KB | Relacionamento | Preços por vendedor |
| `TabelaPrecoPerfil.cs` | 3 KB | Relacionamento | Perfis da tabela |
| `TabelaPrecoUsuario.cs` | 2 KB | Relacionamento | Acesso por usuário |
| `CalculoPrecoVenda.cs` | 23 KB | Processo | Motor de cálculo de preço |

### 2.7 Comissões

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `VendaComissao.cs` | Entidade | Comissão de venda (10 KB) |
| `VendaComissaoDetalhe.cs` | Entidade | Detalhes da comissão |
| `VendaComissaoFechamento.cs` | Entidade | Fechamento de comissões |
| `VendaComissaoLancto.cs` | Entidade | Lançamento de comissão (13 KB) |
| `VendaComissaoMovto.cs` | Entidade | Movimento de comissão (22 KB) |
| `VendaRegraComissao.cs` | Entidade | Regras de comissão (25 KB) |
| `VendaEquipe.cs` | Entidade | Equipe de vendas |
| `VendaEquipeComissionado.cs` | Relacionamento | Comissionados da equipe |
| `VendaComissionado.cs` | Entidade | Comissionado específico |

### 2.8 Outros

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `OrdemExpedicao.cs` | Entidade | Ordem de expedição (7 KB) |
| `OrdemExpedicaoItem.cs` | Relacionamento | Item da expedição (15 KB) |
| `ProgramacaoEntrega.cs` | Entidade | Programação de entrega (15 KB) |
| `DocumentoObservacao.cs` | Entidade | Observação do documento |
| `AlteracaoPreco.cs` | Entidade | Alteração de preço (9 KB) |
| `AlteracaoPrecoEstoque.cs` | Entidade | Alteração de preço por produto |
| `ConsignadoMovimento.cs` | Entidade | Movimento de consignado (11 KB) |
| `ConsignadoSaldo.cs` | Entidade | Saldo de consignado |
| `TipoDadosFrete.cs` | Configuração | Configuração de frete (13 KB) |
| `TipoCondicional.cs` | Configuração | Tipo condicional (12 KB) |

---

## 3. Classes Críticas — Alerta de Complexidade

### 3.1 `DocumentoVenda.cs` — 584 KB ⚠️

> Este é o arquivo mais complexo de todo o sistema legado.
> Representa um documento de venda (NF-e, pedido, orçamento, etc.) e contém
> toda a lógica de cálculo, validação e persistência de uma venda.

**NUNCA implementar de uma vez. Dividir em:**
1. Propriedades básicas (cabeçalho do documento)
2. Relacionamentos (endereços, parcelas, transportes)
3. Métodos de cálculo (totais, impostos)
4. Operações de estado (via `DocumentoVendaSituacao`)

#### 3.1.1 Análise do DocumentoVenda (a preencher)
```
STATUS: PENDENTE DE ANÁLISE
Arquivo a analisar: servidor/objeto de negócio/faturamento/DocumentoVenda.cs
Prioridade: ALTA — análise obrigatória antes de qualquer implementação
```

### 3.2 `DocumentoItemVenda.cs` — 174 KB ⚠️

Representa cada linha (item) de um documento de venda.
Contém cálculo de tributos, descontos, comissões por item.

#### 3.2.1 Análise do DocumentoItemVenda (a preencher)
```
STATUS: PENDENTE DE ANÁLISE
```

### 3.3 `DocumentoVendaSituacao.cs` — 70 KB ⚠️

Contém as transições de estado do documento:
- Incluir → Rascunho → Confirmar → Faturar → Cancelar
- Cada transição pode gerar: movimentos financeiros, movimento de estoque, NF-e

```
STATUS: PENDENTE DE ANÁLISE — deve ser completamente mapeada antes da implementação
```

---

## 4. Regras de Negócio Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-04-001 — Um Tipo Determina Tudo
`TipoDocumentoVenda` configura praticamente todo o comportamento do documento:
obrigatoriedade de campos, integração fiscal, geração financeira, controle de estoque.
**Analisar completamente** antes de implementar DocumentoVenda.

### RN-04-002 — Cálculo de Preço é Multicamada
O `CalculoPrecoVenda.cs` aplica tabelas de preço, descontos, acréscimos e tributação
em múltiplas camadas. A ordem de aplicação importa.

### RN-04-003 — Faturamento gera Financeiro
Ao confirmar um documento, `DocumentoVendaSituacao` gera automaticamente
as parcelas em `gestao.financeira`. Essa integração é crítica.

### RN-04-004 — Faturamento movimenta Estoque
Ao confirmar/faturar, o documento baixa o estoque via `MovimentoEstoque`.
O cancelamento reverte o movimento com lançamento inverso.

### RN-04-005 — Lotes e Séries são obrigatórios quando configurados
Se o produto/documento exigir controle de lote ou série, o sistema bloqueia
a confirmação sem essa informação.

### RN-04-006 — Comissão automática
Comissões são calculadas automaticamente baseadas em `VendaRegraComissao`
e acumuladas em `VendaComissaoMovto`.

---

## 5. Estrutura do Projeto Novo (Versatus.Faturamento)

```
Versatus.Faturamento/
├── Domain/
│   ├── Documentos/
│   │   ├── DocumentoVenda.cs
│   │   ├── DocumentoVendaItem.cs
│   │   ├── DocumentoVendaParcela.cs
│   │   └── DocumentoVendaEndereco.cs
│   ├── Tipos/
│   │   ├── TipoDocumentoVenda.cs
│   │   └── TipoFaturamento.cs
│   ├── Precos/
│   │   ├── TabelaPreco.cs
│   │   └── CalculoPrecoVenda.cs
│   ├── Comissoes/
│   │   ├── VendaComissao.cs
│   │   └── VendaRegraComissao.cs
│   └── Devolucoes/
│       └── DevolucaoVenda.cs
├── Application/
│   ├── Vendas/
│   │   ├── IncluirDocumentoVendaHandler.cs
│   │   ├── ConfirmarDocumentoVendaHandler.cs
│   │   ├── FaturarDocumentoVendaHandler.cs
│   │   └── CancelarDocumentoVendaHandler.cs
│   └── Precos/
│       └── CalcularPrecoVendaHandler.cs
├── Infrastructure/
│   ├── FaturamentoDbContext.cs
│   └── Repositorios/
└── Api/
    └── Controllers/
```

---

## 6. Ordem de Implementação Recomendada

```
1. TipoDocumentoVenda (analisar completamente primeiro)
2. Configurações (TipoFaturamento, TipoPedido, TipoOrcamento)
3. TabelaPreco e CalculoPrecoVenda
4. DocumentoVenda — propriedades básicas
5. DocumentoItemVenda — propriedades básicas
6. DocumentoVendaParcela, Endereço, Transporte
7. DocumentoVendaSituacao → Handlers (Incluir, Confirmar, Faturar, Cancelar)
8. Integração com Financeiro (geração de parcelas)
9. Integração com Estoque (movimentação)
10. Integração com NFe
11. Devoluções
12. Comissões
13. Ordens de Expedição
14. Tabela de Alteração de Preço
```

---

## 7. Roteiro de Tarefas (Ordem de Execução)

Siga a sequência abaixo para implementar MOD-04. Cada tarefa deve ser aprovada por revisão de SPEC, ter um commit Git e um PR específico. **IMPORTANTE:** as análises de `DocumentoVenda.cs`, `DocumentoItemVenda.cs` e `DocumentoVendaSituacao.cs` devem ser concluídas antes das implementações principais.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.Faturamento**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.Faturamento`
- Adicione referências a `Versatus.Framework`, `Versatus.AcessoGlobal` e `Versatus.GestaoMaterial`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/faturamento-project`
- Commit: `setup: Create Versatus.Faturamento project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Documentos/`, `Tipos/`, `Precos/`, `Comissoes/`, `Devolucoes/`, `Expedicao/`
- Branch: `setup/faturamento-structure`
- Commit: `setup: Create Faturamento folder structure`

**Tarefa 1.3 — Criar DbContext Base**
- Crie `Infrastructure/FaturamentoDbContext.cs` com `DbSet` vazios inicialmente
- Branch: `setup/faturamento-dbcontext-base`
- Commit: `setup: Create FaturamentoDbContext base`

### Fase 2: Tipos de Documento e Configurações

**Tarefa 2.1 — Analisar TipoDocumentoVenda.cs**
- Leia completamente `servidor/objeto de negócio/faturamento/TipoDocumentoVenda.cs` (88 KB)
- Documente obrigatoriedades, operações fiscais e regras de comportamento por tipo
- Branch: `analysis/tipo-documento-venda`
- Commit: `docs: Analyze TipoDocumentoVenda legacy behavior`

**Tarefa 2.2 — Implementar TipoDocumentoVenda**
- Crie `Domain/Tipos/TipoDocumentoVenda.cs` conforme análise
- Defina relacionamentos para `TipoDocumentoVendaNaturezaOperacao`, `Observacao`, `ClasseTipoProduto`, `SerieComputador`, `Perfil`, `ItemFinanceiro`
- Branch: `feat/tipo-documento-venda-entity`
- Commit: `feat: Implement TipoDocumentoVenda entity`

**Tarefa 2.3 — Implementar Tipos de Documento Auxiliares**
- Crie `TipoFaturamento`, `TipoNotaFiscal`, `TipoNotaFiscalEntrada`, `TipoOrcamento`, `TipoPedido`
- Documente cada tipo de documento e sua atuação no fluxo de vendas
- Branch: `feat/documento-tipos-config`
- Commit: `feat: Implement billing document type configuration`

**Tarefa 2.4 — Configurar Tipos no DbContext**
- Atualize `FaturamentoDbContext` com `DbSet` dessas entities
- Branch: `feat/tipos-dbcontext`
- Commit: `feat: Configure document types in DbContext`
- Marque no checklist: ✅ Fase 2 completa

### Fase 3: Tabela de Preço e Cálculo

**Tarefa 3.1 — Analisar TabelaPreco.cs e CalculoPrecoVenda.cs**
- Leia os arquivos legados `TabelaPreco.cs`, `TabelaPrecoEstoque.cs`, `CalculoPrecoVenda.cs`
- Documente regras de descontos, acréscimos, perfis e exceções
- Branch: `analysis/tabela-preco`
- Commit: `docs: Analyze legacy pricing engine`

**Tarefa 3.2 — Implementar TabelaPreco**
- Crie `Domain/Precos/TabelaPreco.cs` e `TabelaPrecoEstoque.cs`
- Implemente regras de relacionamento com `Cliente`, `Produto` e `Perfil`
- Branch: `feat/tabela-preco-entity`
- Commit: `feat: Implement TabelaPreco entities`

**Tarefa 3.3 — Implementar CalculoPrecoVenda**
- Crie `Domain/Precos/CalculoPrecoVenda.cs`
- Implemente o motor de cálculo com entradas de tabela, descontos e regras de comissão
- Branch: `feat/calculo-preco-venda`
- Commit: `feat: Implement CalculoPrecoVenda pricing engine`

**Tarefa 3.4 — Configurar Preços no DbContext**
- Atualize `FaturamentoDbContext` com os `DbSet` de preço
- Branch: `feat/precos-dbcontext`
- Commit: `feat: Configure pricing tables in DbContext`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: Documento de Venda Básico

**Tarefa 4.1 — Analisar DocumentoVenda.cs**
- Leia completamente `servidor/objeto de negócio/faturamento/DocumentoVenda.cs` (584 KB)
- Documente propriedades principais, relacionamentos e regras de persistência
- Preencha a seção 3.1.1 desta SPEC
- Branch: `analysis/documento-venda-critico`
- Commit: `docs: Complete analysis of legacy DocumentoVenda.cs`

**Tarefa 4.2 — Implementar DocumentoVenda básico**
- Crie `Domain/Documentos/DocumentoVenda.cs` com cabeçalho, dados do cliente, emissor e documento
- Exclua `*Lista.cs` do legado; use `List<T>`/`IReadOnlyList<T>`
- Branch: `feat/documento-venda-entity`
- Commit: `feat: Implement basic DocumentoVenda entity`

**Tarefa 4.3 — Implementar DocumentoVendaEnderecos e Transporte**
- Crie `DocumentoVendaEndereco.cs` e `DocumentoVendaTransporte.cs`
- Configure relacionamento 1:N com documento
- Branch: `feat/documento-venda-relacionamentos`
- Commit: `feat: Implement DocumentoVenda address and transport entities`

**Tarefa 4.4 — Implementar DocumentoVendaParcela**
- Crie `DocumentoVendaParcela.cs` e `DocumentoVendaParcelaForma.cs`
- Garanta integração futura com MOD-05 Financeiro
- Branch: `feat/documento-venda-parcela-entity`
- Commit: `feat: Implement DocumentoVenda installment entities`

**Tarefa 4.5 — Configurar DocumentoVenda no DbContext**
- Atualize `FaturamentoDbContext` com `DocumentoVenda`, `DocumentoVendaEndereco`, `DocumentoVendaTransporte`, `DocumentoVendaParcela`
- Branch: `feat/documento-venda-dbcontext`
- Commit: `feat: Configure DocumentoVenda in DbContext`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: Itens de Venda

**Tarefa 5.1 — Analisar DocumentoItemVenda.cs**
- Leia o arquivo legado `DocumentoItemVenda.cs` (174 KB)
- Documente cálculos de tributos, descontos e regras de lote/série
- Branch: `analysis/documento-item-venda`
- Commit: `docs: Analyze legacy DocumentoItemVenda.cs`

**Tarefa 5.2 — Implementar DocumentoItemVenda**
- Crie `Domain/Documentos/DocumentoItemVenda.cs` com campos principais e cálculo de totais
- Inclua coleções de tributos e composições como `IReadOnlyList<T>`
- Branch: `feat/documento-item-venda-entity`
- Commit: `feat: Implement DocumentoItemVenda entity`

**Tarefa 5.3 — Implementar Relacionamentos de Item**
- Crie `DocumentoItemVendaTributo.cs`, `DocumentoItemVendaComposto.cs`, `DocumentoItemLocalizacao.cs`, `DocumentoItemLote.cs`, `DocumentoItemSerie.cs`
- Branch: `feat/documento-item-relacionamentos`
- Commit: `feat: Implement DocumentoItemVenda related entities`

**Tarefa 5.4 — Configurar Itens no DbContext**
- Atualize `FaturamentoDbContext` com `DocumentoItemVenda` e suas entidades relacionadas
- Branch: `feat/documento-item-dbcontext`
- Commit: `feat: Configure DocumentoItemVenda in DbContext`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Situação do Documento e Handlers

**Tarefa 6.1 — Analisar DocumentoVendaSituacao.cs**
- Mapeie todas as transições de estado e seus efeitos secundários
- Documente fluxo de `Incluir → Rascunho → Confirmar → Faturar → Cancelar`
- Branch: `analysis/documento-venda-situacao`
- Commit: `docs: Analyze DocumentoVendaSituacao state machine`

**Tarefa 6.2 — Implementar DocumentoVendaSituacao**
- Crie `Domain/Documentos/DocumentoVendaSituacao.cs` com transições e regras de autorização
- Garanta que o estado controle a geração de financeiro, estoque e NF-e
- Branch: `feat/documento-venda-situacao-entity`
- Commit: `feat: Implement DocumentoVendaSituacao state entity`

**Tarefa 6.3 — Criar Handlers de Estado**
- Crie handlers de aplicação: `IncluirDocumentoVendaHandler`, `ConfirmarDocumentoVendaHandler`, `FaturarDocumentoVendaHandler`, `CancelarDocumentoVendaHandler`
- Cada handler deve usar `Result<T>`/`ValidationResult` e não lançar exceções para validação esperada
- Branch: `feat/documento-venda-handlers`
- Commit: `feat: Implement DocumentoVenda state handlers`

**Tarefa 6.4 — Testar Transições de Estado**
- Adicione testes de fluxo para cada transição de situação
- Verifique efeitos em financeiro, estoque e NF-e de forma isolada
- Branch: `test/documento-venda-situacao`
- Commit: `test: Add state transition tests for DocumentoVenda`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Integração Financeiro e Estoque

**Tarefa 7.1 — Implementar integração com MOD-05 Financeiro**
- Crie ponte para geração de parcelas ao confirmar documento
- Garanta que `DocumentoVendaParcela` e `FormaPagamento` se integrem corretamente
- Branch: `feat/faturamento-financeiro-integration`
- Commit: `feat: Implement billing to finance integration`

**Tarefa 7.2 — Implementar integração com MOD-03 Estoque**
- Crie lógica para geração de `MovimentoEstoque` na confirmação/faturamento
- Garanta reversão correta no cancelamento
- Branch: `feat/faturamento-estoque-integration`
- Commit: `feat: Implement billing to stock integration`

**Tarefa 7.3 — Implementar integração com MOD-08 NFe**
- Crie classes de apoio para autorização e protocolo de NF-e
- Defina os pontos de extensão em `ConfirmarDocumentoVendaHandler` e `FaturarDocumentoVendaHandler`
- Branch: `feat/faturamento-nfe-integration`
- Commit: `feat: Implement billing to NFe integration`

**Tarefa 7.4 — Testar integrações críticas**
- Adicione testes de integração para financeiro, estoque e NF-e separados por cenário
- Branch: `test/faturamento-integrations`
- Commit: `test: Add billing integration tests`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Devoluções e Condicionais

**Tarefa 8.1 — Implementar DevolucaoVenda**
- Crie `Domain/Devolucoes/DevolucaoVenda.cs` e `DevolucaoVendaItem.cs`
- Garanta relacionamento com documento original e controle de acerto
- Branch: `feat/devolucao-venda-entity`
- Commit: `feat: Implement DevolucaoVenda entity`

**Tarefa 8.2 — Implementar DevolucaoCondicional**
- Crie `DevolucaoCond.cs` e `DevolucaoItemCond.cs`
- Documente regras de uso em casos de devolução condicional
- Branch: `feat/devolucao-condicional-entity`
- Commit: `feat: Implement conditional return entities`

**Tarefa 8.3 — Testar fluxo de devoluções**
- Adicione testes que cobrem devolução simples e condicional
- Verifique impacto em financeiro e estoque
- Branch: `test/devolucao-venda`
- Commit: `test: Add return flow tests`
- Marque no checklist: ✅ Fase 8 completa

### Fase 9: Comissões e Equipe de Vendas

**Tarefa 9.1 — Implementar Regras de Comissão**
- Crie `Domain/Comissoes/VendaRegraComissao.cs` e `VendaComissaoMovto.cs`
- Implemente regras de cálculo com base em `TipoDocumentoVenda`, `Produto` e `Perfil`
- Branch: `feat/comissao-rules`
- Commit: `feat: Implement commission calculation rules`

**Tarefa 9.2 — Implementar VendaComissao e Detalhes**
- Crie `VendaComissao.cs`, `VendaComissaoDetalhe.cs`, `VendaEquipe.cs`, `VendaEquipeComissionado.cs`
- Relacione o cálculo com o documento e comissionados
- Branch: `feat/comissao-entities`
- Commit: `feat: Implement commission entities`

**Tarefa 9.3 — Testar comissões**
- Adicione testes de cálculo e fechamento de comissão
- Branch: `test/comissao`
- Commit: `test: Add commission tests`
- Marque no checklist: ✅ Fase 9 completa

### Fase 10: Expedição e Alteração de Preço

**Tarefa 10.1 — Implementar OrdemExpedicao**
- Crie `Domain/Expedicao/OrdemExpedicao.cs` e `OrdemExpedicaoItem.cs`
- Documente como a expedição se relaciona com o documento de venda
- Branch: `feat/ordem-expedicao-entity`
- Commit: `feat: Implement OrdemExpedicao entities`

**Tarefa 10.2 — Implementar AlteracaoPreco**
- Crie `AlteracaoPreco.cs` e `AlteracaoPrecoEstoque.cs`
- Garanta que alterações de preço por produto e estoque sejam registradas
- Branch: `feat/alteracao-preco-entity`
- Commit: `feat: Implement price change entities`

**Tarefa 10.3 — Configurar Expedição e Alterações no DbContext**
- Atualize `FaturamentoDbContext` com as novas entities de expedição e alteração de preço
- Branch: `feat/expedicao-dbcontext`
- Commit: `feat: Configure expedition and price change entities in DbContext`
- Marque no checklist: ✅ Fase 10 completa

### Fase 11: Repositórios e Migrations

**Tarefa 11.1 — Criar Repositórios-Chave**
- Crie `Infrastructure/Repositorios/IDocumentoVendaRepository.cs` e `DocumentoVendaRepository.cs`
- Crie `IFaturamentoRepository.cs` para consultas específicas de faturamento
- Branch: `feat/faturamento-repositories`
- Commit: `feat: Implement Faturamento repository interfaces`

**Tarefa 11.2 — Criar Migrations EF Core**
- Crie a primeira migration para `FaturamentoDbContext`
- Valide todas as tabelas e relacionamentos
- Branch: `setup/faturamento-migrations`
- Commit: `setup: Create initial EF Core migrations for Faturamento`

**Tarefa 11.3 — Registrar DI**
- Crie método de extensão para registrar DbContext, handlers e repositórios
- Branch: `setup/faturamento-di`
- Commit: `setup: Configure dependency injection for Faturamento`
- Marque no checklist: ✅ Fase 11 completa

---

## 8. Checklist de Conclusão do Módulo

Acompanhe o progresso usando o **Roteiro de Tarefas** acima. Cada fase tem sua própria verificação.

- [ ] Fase 1: Setup e Estrutura — Completa
- [ ] Fase 2: TipoDocumentoVenda e configurações — Completa
- [ ] Fase 3: Tabela de Preço e Cálculo — Completa
- [ ] Fase 4: DocumentoVenda básico e relacionamentos — Completa
- [ ] Fase 5: DocumentoItemVenda e relacionamentos — Completa
- [ ] Fase 6: Situação do Documento e Handlers — Completa
- [ ] Fase 7: Integrações Financeiro/Estoque/NFe — Completa
- [ ] Fase 8: Devoluções e Condicionais — Completa
- [ ] Fase 9: Comissões — Completa
- [ ] Fase 10: Expedição e Alterações de Preço — Completa
- [ ] Fase 11: Repositórios e Migrations — Completa

---

## Histórico de Alterações

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise de `servidor/objeto de negócio/faturamento/` — 125 arquivos — `projeto_tag_1906`*

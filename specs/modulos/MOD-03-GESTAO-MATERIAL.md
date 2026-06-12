# SPEC — MOD-03: Gestão de Material
## Módulo: servidor/objeto de negócio/gestao.material

> **Versão:** 1.0 | **Data:** 2026-04-27 | **Fase:** 2  
> **Status:** 📝 Rascunho  
> **Prioridade:** ALTA — produtos e estoque são base de faturamento e compras

> ⛔ **Regra Git:** Nunca commite diretamente em `develop` ou `main`.  
> Crie sempre uma branch `feat/gestao-material-xxx` ou `docs/analise-xxx` antes de qualquer código ou documentação.  
> Veja os prompts prontos em `specs/prompts-execucao/` e o guia em `specs/prompts-execucao/00-GUIA-HANDOFF.md`.

---

## 1. Visão Geral do Módulo

O módulo **Gestão de Material** centraliza o cadastro de produtos/mercadorias e o controle
de estoque. É o segundo módulo de negócio mais complexo do sistema, com 84 arquivos cobrindo
produtos, grades, composições, movimentação de estoque e localizações.

### Localização no legado
```
servidor/objeto de negócio/gestao.material/
```

### Dependências
- **Requer:** MOD-01 (Framework), MOD-02 (Acesso Global — para Filial, Unidade)
- **É usado por:** MOD-04 (Faturamento), MOD-06 (Compras), MOD-07 (Tributo), MOD-12 (Produção)

---

## 2. Inventário de Classes por Grupo

### 2.1 Produto — Centro do Módulo

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Produto.cs` | 193 KB | **Entidade Central** | Produto/mercadoria (ENORME — ler com cuidado) |
| `ProdutoFilial.cs` | 26 KB | Relacionamento | Configuração do produto por filial |
| `ProdutoFilialLista.cs` | 1 KB | Lista | |
| `ProdutoClasse.cs` | 6 KB | Relacionamento | Classes do produto |
| `ProdutoClasseLista.cs` | 1 KB | Lista | |
| `ProdutoConversor.cs` | 10 KB | Entidade | Conversor de unidades do produto |
| `ProdutoConversorLista.cs` | 1 KB | Lista | |
| `ProdutoTributacao.cs` | 13 KB | Relacionamento | Tributação por produto |
| `ProdutoTributacaoLista.cs` | 4 KB | Lista | |
| `ProdutoOrigemCombustivel.cs` | 7 KB | Relacionamento | Origem para combustíveis |
| `ProdutoOrigemCombustivelLista.cs` | 2 KB | Lista | |

### 2.2 Grade (Variação de Produto)

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Grade.cs` | Entidade | Grade de variações (cor/tamanho) (20 KB) |
| `GradeVariacao.cs` | Relacionamento | Variações da grade |
| `GradeVariacaoLista.cs` | Lista | |
| `Variante.cs` | Entidade | Variante (ex: Cor) |
| `SubVariante.cs` | Entidade | Sub-variante (ex: Azul) |
| `SubVarianteLista.cs` | Lista | |
| `ItemGrade.cs` | Relacionamento | Item da grade (produto + variações) |
| `ItemGradeLista.cs` | Lista | |
| `ItemVariacao.cs` | Relacionamento | Variação específica do item |
| `ItemVariacaoLista.cs` | Lista | |

### 2.3 Composição (Produto Composto / Kit)

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Composicao.cs` | Entidade | Composição (receita / kit) (9 KB) |
| `ComposicaoItem.cs` | Relacionamento | Item da composição |
| `ComposicaoItemFilial.cs` | Relacionamento | Item por filial |
| `ComposicaoItemLista.cs` | Lista | |

### 2.4 Estoque e Saldos

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Estoque.cs` | 39 KB | **Entidade Central** | Estoque principal |
| `EstoqueComposicao.cs` | 3 KB | Relacionamento | Composição do estoque |
| `EstoqueComposicaoBase.cs` | 6 KB | Base | |
| `EstoqueReserva.cs` | 6 KB | Entidade | Reservas de estoque |
| `EstoqueSaldoFinanceiro.cs` | 5 KB | Entidade | Saldo financeiro do estoque |
| `EstoqueSimilar.cs` | 7 KB | Relacionamento | Produtos similares |
| `EstoqueSimilarLista.cs` | 2 KB | Lista | |
| `SaldoEstoque.cs` | 7 KB | Entidade | Saldo de estoque |
| `SaldoProduto.cs` | 6 KB | Entidade | Saldo por produto |
| `SaldoLocalizacao.cs` | 8 KB | Entidade | Saldo por localização |
| `SaldoEstoqueLista.cs` | — | Lista | |

### 2.5 Localização de Estoque (WMS básico)

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Localizacao.cs` | Entidade | Localização física no armazém |
| `LocalizacaoLista.cs` | Lista | |
| `EstoqueLocalizacao.cs` | Relacionamento | Produto em localização |
| `EstoqueLocalizacaoEndereco.cs` | Relacionamento | Endereço da localização |
| `EstoqueLocalizacaoLista.cs` | Lista | |

### 2.6 Movimentação de Estoque

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `MovimentoEstoque.cs` | 73 KB | **Entidade Central** | Movimento de estoque (GRANDE) |
| `MovimentoEstoqueLista.cs` | 2 KB | Lista | |
| `MovtoEstoqueRateio.cs` | 5 KB | Relacionamento | Rateio no movimento |

### 2.7 Lote e Série

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `Lote.cs` | Entidade | Lote de produto (19 KB) |
| `LoteLista.cs` | Lista | |
| `MovimentoLote.cs` | Entidade | Movimento de lote (13 KB) |
| `MovimentoLoteLista.cs` | Lista | |
| `Serie.cs` | Entidade | Série de produto (11 KB) |
| `SerieLista.cs` | Lista | |
| `SerieMovimento.cs` | Entidade | Movimento de série |
| `SerieMovimentoLista.cs` | Lista | |

### 2.8 Estrutura de Classificação de Produtos

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `GrupoEstoque.cs` | Entidade | Grupo/família de produtos (67 KB — GRANDE) |
| `GrupoEstoqueFilial.cs` | Relacionamento | Grupo por filial |
| `TipoProdutoClasse.cs` | Entidade | Tipo e classe de produto |
| `Linha.cs` | Entidade | Linha de produto |
| `Marca.cs` | Entidade | Marca |
| `Fabricante.cs` | Entidade | Fabricante (10 KB) |
| `Modelo.cs` | Entidade | Modelo |
| `Unidade.cs` | Entidade | Unidade de medida (14 KB) |
| `Almoxarifado.cs` | Entidade | Almoxarifado |

### 2.9 Auxiliares

| Classe Legada | Tipo | Descrição |
|---|---|---|
| `CodigoReferencia.cs` | Entidade | Código de referência alternativo (18 KB) |
| `Conversor.cs` | Entidade | Conversor de unidades genérico (19 KB) |
| `Equipamento.cs` | Entidade | Equipamento |
| `RegraEstoque.cs` | Entidade | Regras de estoque (20 KB) |
| `EstatisticaEstoque.cs` | Entidade | Estatísticas de estoque (19 KB) |
| `EstatisticaEstoqueCompra.cs` | Entidade | Estatísticas de compras |
| `ManutencaoRateioEstoque.cs` | Processo | Manutenção de rateio |
| `FornecedorEstoquePreco.cs` | Relacionamento | Preços do fornecedor por produto |
| `EstoqueUtil.cs` | Utilitário | Utilitários de estoque |

---

## 3. Entidades Centrais — Análise Detalhada

### 3.1 `Produto.cs` — 193 KB (CRÍTICO)

> ⚠️ Este é o arquivo mais crítico do módulo. Com 193 KB, contém lógica suficiente
> para ser analisada em várias sessões separadas.

**Tarefas antes de implementar:**
1. Ler `Produto.cs` completo e listar todas as propriedades — seção 3.1.1 abaixo
2. Identificar tabela real via `[TableName]`
3. Identificar relacionamentos (outras tabelas que referencia)
4. Identificar métodos de regra de negócio

#### 3.1.1 Propriedades do Produto (a preencher)
```
STATUS: PENDENTE DE ANÁLISE
Arquivo a analisar: servidor/objeto de negócio/gestao.material/Produto.cs
```

### 3.2 `Estoque.cs` — 39 KB

Similar ao Produto, este arquivo contém a lógica de gerência do estoque incluindo:
- Controle de saldo mínimo/máximo
- Cálculo de custo médio
- Reservas de estoque

#### 3.2.1 Propriedades do Estoque (a preencher)
```
STATUS: PENDENTE DE ANÁLISE
Arquivo a analisar: servidor/objeto de negócio/gestao.material/Estoque.cs
```

### 3.3 `MovimentoEstoque.cs` — 73 KB

Registra toda movimentação de entrada e saída. Afeta saldos e custos.

> ⚠️ Nunca implementar sem entender completamente as regras de custo médio.

---

## 4. Regras de Negócio Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-03-001 — Produto com Grade
Um produto pode ter grades (ex: camiseta em P/M/G com cores).
Cada combinação de variações é um `ItemGrade` com estoque independente.
**Não simplificar** — o sistema de grades é multi-nível.

### RN-03-002 — Custo Médio
O `MovimentoEstoque` recalcula o custo médio ponderado após cada entrada.
A fórmula está em `MovimentoEstoque.cs` — documentar na seção 3.3.1 antes de implementar.

### RN-03-003 — Lote e Rastreabilidade
Produtos com controle de lote requerem que cada movimento informe o lote.
O saldo de lote é mantido separadamente do saldo geral.

### RN-03-004 — Estoque por Almoxarifado e Localização
O saldo de estoque pode ser controlado por almoxarifado e por localização física.
O saldo total é a soma de todas as localizações.

### RN-03-005 — Reserva de Estoque
Pedidos confirmados reservam estoque antes da saída efetiva.
O saldo disponível = saldo total − reservado.

---

## 5. Estrutura do Projeto Novo (Versatus.GestaoMaterial)

```
Versatus.GestaoMaterial/
├── Domain/
│   ├── Produtos/
│   │   ├── Produto.cs
│   │   ├── ProdutoFilial.cs
│   │   ├── GrupoEstoque.cs
│   │   ├── Marca.cs
│   │   ├── Fabricante.cs
│   │   ├── Unidade.cs
│   │   └── TipoProdutoClasse.cs
│   ├── Grades/
│   │   ├── Grade.cs
│   │   ├── Variante.cs
│   │   └── ItemGrade.cs
│   ├── Composicao/
│   │   ├── Composicao.cs
│   │   └── ComposicaoItem.cs
│   ├── Estoque/
│   │   ├── Estoque.cs
│   │   ├── SaldoEstoque.cs
│   │   ├── EstoqueReserva.cs
│   │   └── MovimentoEstoque.cs
│   ├── Lote/
│   │   ├── Lote.cs
│   │   └── MovimentoLote.cs
│   └── Serie/
│       ├── Serie.cs
│       └── SerieMovimento.cs
├── Application/
│   ├── Produtos/
│   ├── Estoque/
│   └── Movimentos/
├── Infrastructure/
│   ├── GestaoMaterialDbContext.cs
│   └── Repositorios/
└── Api/
    └── Controllers/
```

---

## 6. Ordem de Implementação Recomendada

```
1. Unidade de medida (Unidade)
2. Classificação (GrupoEstoque, TipoProdutoClasse, Linha, Marca, Fabricante, Modelo)
3. Produto (analisar completamente antes)
4. ProdutoFilial
5. Grade e Variantes
6. Composição
7. Almoxarifado, Localização
8. Lote e Série
9. Estoque e Saldo
10. MovimentoEstoque (analisar completamente antes)
11. Reservas
12. Estatísticas e Relatórios
```

---

## 7. Roteiro de Tarefas (Ordem de Execução)

Siga esta sequência de tarefas para implementar MOD-03. Cada tarefa deve resultar em um commit Git, um PR para revisão, e marca de concluído na SPEC. **IMPORTANTE:** As análises críticas de Produto (3.1), Estoque (3.2) e MovimentoEstoque (3.3) devem ser feitas ANTES de qualquer implementação.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.GestaoMaterial**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.GestaoMaterial`
- Adicione referências a `Versatus.Framework` e `Versatus.AcessoGlobal`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/gestao-material-project`
- Commit: `setup: Create Versatus.GestaoMaterial project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Produtos/`, `Grades/`, `Composicao/`, `Estoque/`, `Lote/`, `Serie/`
- Branch: `setup/gestao-material-structure`
- Commit: `setup: Create GestaoMaterial folder structure`

**Tarefa 1.3 — Criar DbContext Base**
- Crie `Infrastructure/GestaoMaterialDbContext.cs` (classe vazia por enquanto)
- Branch: `setup/gestao-material-dbcontext-base`
- Commit: `setup: Create GestaoMaterialDbContext base`

### Fase 2: Unidades de Medida

**Tarefa 2.1 — Implementar Unidade**
- Crie `Domain/Produtos/Unidade.cs`
- Propriedades: IdUnidade, Codigo, Nome, Abreviatura, Tipo (enum: Peso, Volume, Comprimento, Area, Quantidade)
- Tabela: `PrcUnidade` (verificar)
- Branch: `feat/unidade-entity`
- Commit: `feat: Implement Unidade (unit of measure) entity`

**Tarefa 2.2 — Implementar Conversor**
- Crie `Domain/Produtos/Conversor.cs`
- Propriedades: IdConversor, IdUnidade1 (FK), IdUnidade2 (FK), Fator (decimal)
- Permite conversão entre unidades (ex: 1 kg = 1000 g)
- Branch: `feat/conversor-entity`
- Commit: `feat: Implement Conversor (unit conversion) entity`

**Tarefa 2.3 — Configurar Unidades no DbContext**
- Atualize `GestaoMaterialDbContext` com `DbSet<Unidade>` e `DbSet<Conversor>`
- Branch: `feat/unidades-dbcontext`
- Commit: `feat: Configure units in DbContext`
- Marque no checklist: ✅ Fase 2 completa

### Fase 3: Classificação de Produtos

**Tarefa 3.1 — Implementar GrupoEstoque**
- Crie `Domain/Produtos/GrupoEstoque.cs`
- Propriedades: IdGrupo, Codigo, Nome, Descricao, Ativo
- Tabela: `PrcGrupoEstoque` (verificar)
- Branch: `feat/grupo-estoque-entity`
- Commit: `feat: Implement GrupoEstoque entity`

**Tarefa 3.2 — Implementar TipoProdutoClasse**
- Crie `Domain/Produtos/TipoProdutoClasse.cs`
- Propriedades: IdTipo, Tipo (enum: ProdutoAcabado, MateriaPrima, Servico), Classe (enum: A, B, C)
- Branch: `feat/tipo-produto-classe-entity`
- Commit: `feat: Implement TipoProdutoClasse entity`

**Tarefa 3.3 — Implementar Linha**
- Crie `Domain/Produtos/Linha.cs`
- Propriedades: IdLinha, Codigo, Nome
- Branch: `feat/linha-entity`
- Commit: `feat: Implement Linha (product line) entity`

**Tarefa 3.4 — Implementar Marca**
- Crie `Domain/Produtos/Marca.cs`
- Propriedades: IdMarca, Codigo, Nome
- Branch: `feat/marca-entity`
- Commit: `feat: Implement Marca (brand) entity`

**Tarefa 3.5 — Implementar Fabricante**
- Crie `Domain/Produtos/Fabricante.cs`
- Propriedades: IdFabricante, Codigo, Nome, IdEntidade (FK para Acesso Global)
- Branch: `feat/fabricante-entity`
- Commit: `feat: Implement Fabricante (manufacturer) entity`

**Tarefa 3.6 — Implementar Modelo**
- Crie `Domain/Produtos/Modelo.cs`
- Propriedades: IdModelo, Codigo, Nome, IdFabricante (FK)
- Branch: `feat/modelo-entity`
- Commit: `feat: Implement Modelo entity`

**Tarefa 3.7 — Configurar Classificação no DbContext**
- Atualize `GestaoMaterialDbContext` com todas as entities de classificação
- Branch: `feat/classificacao-dbcontext`
- Commit: `feat: Configure product classification in DbContext`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: Análise Crítica — Produto (193 KB)

**Tarefa 4.1 — Analisar Produto.cs Legado**
- ⚠️ Leia completamente `servidor/objeto de negócio/gestao.material/Produto.cs` (193 KB — CRÍTICO)
- Documente TODAS as propriedades
- Identifique a tabela real via `[TableName]`
- Identifique todos os relacionamentos (FK para outras entidades)
- Identifique métodos de regra de negócio e validações
- Documente tudo na seção 3.1.1 desta SPEC
- Branch: `analysis/produto-critico`
- Commit: `docs: Complete analysis of legacy Produto.cs (193 KB)`
- **Não prossiga para 4.2 até que 4.1 esteja 100% documentado**

**Tarefa 4.2 — Implementar Produto (após análise 4.1)**
- Crie `Domain/Produtos/Produto.cs`
- EXATAMENTE como analisado em 4.1 — sem simplificações
- Propriedades principais: IdProduto, IdGrupo (FK), Descricao, CodigoBarras, IdUnidade (FK), CustoAtual, PrecoVenda, Ativo
- Propriedades adicionais conforme 4.1
- Branch: `feat/produto-entity`
- Commit: `feat: Implement Produto entity with full legacy parity`

**Tarefa 4.3 — Implementar ProdutoFilial**
- Crie `Domain/Produtos/ProdutoFilial.cs`
- Propriedades: IdRelacao, IdProduto (FK), IdFilial (FK), ConfiguracaoEspecifica
- Branch: `feat/produto-filial-entity`
- Commit: `feat: Implement ProdutoFilial relationship`

**Tarefa 4.4 — Configurar Produto no DbContext**
- Atualize `GestaoMaterialDbContext` com Produto e ProdutoFilial
- Branch: `feat/produto-dbcontext`
- Commit: `feat: Configure Produto in DbContext`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: Grades e Variantes

**Tarefa 5.1 — Implementar Variante**
- Crie `Domain/Grades/Variante.cs`
- Propriedades: IdVariante, Nome, Tipo (enum: Cor, Tamanho, Sabor, etc.)
- Branch: `feat/variante-entity`
- Commit: `feat: Implement Variante entity`

**Tarefa 5.2 — Implementar SubVariante**
- Crie `Domain/Grades/SubVariante.cs`
- Propriedades: IdSubVariante, IdVariante (FK), Nome, Valor
- Branch: `feat/sub-variante-entity`
- Commit: `feat: Implement SubVariante entity`

**Tarefa 5.3 — Implementar Grade**
- Crie `Domain/Grades/Grade.cs`
- Propriedades: IdGrade, IdProduto (FK), Nome, Ativa
- Relacionamento: múltiplos Variantes
- Branch: `feat/grade-entity`
- Commit: `feat: Implement Grade (product variation) entity`

**Tarefa 5.4 — Implementar ItemGrade**
- Crie `Domain/Grades/ItemGrade.cs`
- Propriedades: IdItem, IdGrade (FK), Combinacao (string de sub-variantes), CodigoBarras, EstoqueDisponivel
- Branch: `feat/item-grade-entity`
- Commit: `feat: Implement ItemGrade entity`

**Tarefa 5.5 — Configurar Grades no DbContext**
- Atualize `GestaoMaterialDbContext` com Variante, SubVariante, Grade, ItemGrade
- Branch: `feat/grades-dbcontext`
- Commit: `feat: Configure grades in DbContext`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Composição (Kits/Receitas)

**Tarefa 6.1 — Implementar Composicao**
- Crie `Domain/Composicao/Composicao.cs`
- Propriedades: IdComposicao, IdProdutoComposto (FK), Nome, Ativa
- Coleção: `IReadOnlyList<ComposicaoItem> Itens`
- Branch: `feat/composicao-entity`
- Commit: `feat: Implement Composicao (recipe/kit) entity`

**Tarefa 6.2 — Implementar ComposicaoItem**
- Crie `Domain/Composicao/ComposicaoItem.cs`
- Propriedades: IdItem, IdComposicao (FK), IdProdutoComponente (FK), Quantidade, IdUnidade (FK)
- Branch: `feat/composicao-item-entity`
- Commit: `feat: Implement ComposicaoItem entity`

**Tarefa 6.3 — Configurar Composição no DbContext**
- Atualize `GestaoMaterialDbContext` com Composicao e ComposicaoItem
- Branch: `feat/composicao-dbcontext`
- Commit: `feat: Configure composition in DbContext`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Localização e Almoxarifado

**Tarefa 7.1 — Implementar Almoxarifado**
- Crie `Domain/Estoque/Almoxarifado.cs`
- Propriedades: IdAlmoxarifado, IdFilial (FK), Nome, Descricao
- Branch: `feat/almoxarifado-entity`
- Commit: `feat: Implement Almoxarifado (warehouse) entity`

**Tarefa 7.2 — Implementar Localizacao**
- Crie `Domain/Estoque/Localizacao.cs`
- Propriedades: IdLocalizacao, IdAlmoxarifado (FK), Endereco (string: Corredor/Prateleira/Nível), Capacidade
- Branch: `feat/localizacao-entity`
- Commit: `feat: Implement Localizacao (storage location) entity`

**Tarefa 7.3 — Configurar Localização no DbContext**
- Atualize `GestaoMaterialDbContext` com Almoxarifado e Localizacao
- Branch: `feat/localizacao-dbcontext`
- Commit: `feat: Configure warehouse and locations in DbContext`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Lote e Série

**Tarefa 8.1 — Implementar Lote**
- Crie `Domain/Lote/Lote.cs`
- Propriedades: IdLote, IdProduto (FK), NumeroLote, DataFabricacao, DataValidade
- Branch: `feat/lote-entity`
- Commit: `feat: Implement Lote (batch/lot) entity`

**Tarefa 8.2 — Implementar MovimentoLote**
- Crie `Domain/Lote/MovimentoLote.cs`
- Propriedades: IdMovimento, IdLote (FK), IdMovimentoEstoque (FK), Quantidade
- Branch: `feat/movimento-lote-entity`
- Commit: `feat: Implement MovimentoLote entity`

**Tarefa 8.3 — Implementar Serie**
- Crie `Domain/Serie/Serie.cs`
- Propriedades: IdSerie, IdProduto (FK), NumeroSerie, DataFabricacao
- Branch: `feat/serie-entity`
- Commit: `feat: Implement Serie (serial) entity`

**Tarefa 8.4 — Implementar SerieMovimento**
- Crie `Domain/Serie/SerieMovimento.cs`
- Propriedades: IdMovimento, IdSerie (FK), IdMovimentoEstoque (FK)
- Branch: `feat/serie-movimento-entity`
- Commit: `feat: Implement SerieMovimento entity`

**Tarefa 8.5 — Configurar Lote e Série no DbContext**
- Atualize `GestaoMaterialDbContext` com todas as entities
- Branch: `feat/lote-serie-dbcontext`
- Commit: `feat: Configure batch and serial tracking in DbContext`
- Marque no checklist: ✅ Fase 8 completa

### Fase 9: Análise Crítica — Estoque (39 KB) e Saldos

**Tarefa 9.1 — Analisar Estoque.cs Legado**
- ⚠️ Leia completamente `servidor/objeto de negócio/gestão.material/Estoque.cs` (39 KB — CRÍTICO)
- Documente lógica de saldo mínimo/máximo
- Documente cálculo de custo médio
- Documente gestão de reservas
- Preencha a seção 3.2.1 da SPEC
- Branch: `analysis/estoque-critico`
- Commit: `docs: Complete analysis of legacy Estoque.cs (39 KB)`

**Tarefa 9.2 — Implementar Estoque (após análise 9.1)**
- Crie `Domain/Estoque/Estoque.cs`
- Propriedades conforme análise 9.1
- Branch: `feat/estoque-entity`
- Commit: `feat: Implement Estoque entity with legacy parity`

**Tarefa 9.3 — Implementar SaldoEstoque**
- Crie `Domain/Estoque/SaldoEstoque.cs`
- Propriedades: IdSaldo, IdEstoque (FK), SaldoFisico, SaldoFinanceiro, CustoUnitario, DataAtualizacao
- Branch: `feat/saldo-estoque-entity`
- Commit: `feat: Implement SaldoEstoque entity`

**Tarefa 9.4 — Implementar EstoqueReserva**
- Crie `Domain/Estoque/EstoqueReserva.cs`
- Propriedades: IdReserva, IdEstoque (FK), IdLocalizacao (FK), Quantidade, Observacoes
- Branch: `feat/estoque-reserva-entity`
- Commit: `feat: Implement EstoqueReserva entity`

**Tarefa 9.5 — Configurar Estoque no DbContext**
- Atualize `GestaoMaterialDbContext` com Estoque, SaldoEstoque, EstoqueReserva
- Branch: `feat/estoque-dbcontext`
- Commit: `feat: Configure stock management in DbContext`
- Marque no checklist: ✅ Fase 9 completa

### Fase 10: Análise Crítica — MovimentoEstoque (73 KB)

**Tarefa 10.1 — Analisar MovimentoEstoque.cs Legado**
- ⚠️ Leia completamente `servidor/objeto de negócio/gestão.material/MovimentoEstoque.cs` (73 KB — CRÍTICO)
- **Especial atenção:** Fórmula de custo médio ponderado
- Documente tipo de movimento (Entrada, Saída, Ajuste, Devolução)
- Documente impacto em saldos e custos
- Preencha a seção 3.3 da SPEC com a fórmula
- Branch: `analysis/movimento-estoque-critico`
- Commit: `docs: Complete analysis of legacy MovimentoEstoque.cs with cost formulas (73 KB)`

**Tarefa 10.2 — Implementar MovimentoEstoque (após análise 10.1)**
- Crie `Domain/Estoque/MovimentoEstoque.cs`
- Propriedades: IdMovimento, IdEstoque (FK), IdLocalizacao (FK), Tipo (enum), Quantidade, PrecoUnitario, DataMovimento
- Método: `CalcularCustoMedio()` fiel ao legado
- Branch: `feat/movimento-estoque-entity`
- Commit: `feat: Implement MovimentoEstoque with cost calculation`

**Tarefa 10.3 — Testes de Paridade de Custo Médio**
- Crie testes unitários para validar o cálculo de custo médio
- Compare com dados de teste do legado
- Branch: `test/movimento-estoque-parity`
- Commit: `test: Add parity tests for MovimentoEstoque cost calculation`

**Tarefa 10.4 — Configurar MovimentoEstoque no DbContext**
- Atualize `GestaoMaterialDbContext` com MovimentoEstoque
- Branch: `feat/movimento-estoque-dbcontext`
- Commit: `feat: Configure stock movements in DbContext`
- Marque no checklist: ✅ Fase 10 completa

### Fase 11: Entidades Auxiliares

**Tarefa 11.1 — Implementar CodigoReferencia**
- Crie `Domain/Produtos/CodigoReferencia.cs`
- Propriedades: IdRef, IdProduto (FK), Codigo, Tipo (enum: Referencia, EAN, SKU)
- Branch: `feat/codigo-referencia-entity`
- Commit: `feat: Implement CodigoReferencia (alternative codes) entity`

**Tarefa 11.2 — Implementar RegraEstoque**
- Crie `Domain/Estoque/RegraEstoque.cs`
- Propriedades: IdRegra, IdProduto (FK), SaldoMinimo, SaldoMaximo, QuantidadeFixa, AlertaEstoque
- Branch: `feat/regra-estoque-entity`
- Commit: `feat: Implement RegraEstoque entity`

**Tarefa 11.3 — Implementar EstatisticaEstoque**
- Crie `Domain/Estoque/EstatisticaEstoque.cs`
- Propriedades: IdEstat, IdProduto (FK), QuantidadeVendida, QuantidadeComprada, GiroMedio, DataAtualizacao
- Branch: `feat/estatistica-estoque-entity`
- Commit: `feat: Implement EstatisticaEstoque entity`

**Tarefa 11.4 — Configurar Auxiliares no DbContext**
- Atualize `GestaoMaterialDbContext` com CodigoReferencia, RegraEstoque, EstatisticaEstoque
- Branch: `feat/auxiliares-dbcontext`
- Commit: `feat: Configure auxiliary stock entities in DbContext`
- Marque no checklist: ✅ Fase 11 completa

### Fase 12: Repositórios

**Tarefa 12.1 — Criar Repositórios-Chave**
- Crie `Infrastructure/Repositorio/IProdutoRepository.cs` e `ProdutoRepository.cs`
- Crie `Infrastructure/Repositorio/IEstoqueRepository.cs` e `EstoqueRepository.cs`
- Crie `Infrastructure/Repositorio/IMovimentoEstoqueRepository.cs` e `MovimentoEstoqueRepository.cs`
- Branch: `feat/repositorios-chave`
- Commit: `feat: Implement repository interfaces for Material Management`

**Tarefa 12.2 — Configurar Migrations EF Core**
- Crie primeira migration do `GestaoMaterialDbContext`
- Validar todas as tabelas e relacionamentos
- Branch: `setup/gestao-material-migrations`
- Commit: `setup: Create initial EF Core migrations for GestaoMaterial`

**Tarefa 12.3 — Registrar DI**
- Crie método de extensão para registrar DbContext e repositórios
- Branch: `setup/gestao-material-di`
- Commit: `setup: Configure dependency injection for GestaoMaterial`
- Marque no checklist: ✅ Fase 12 completa

---

## 8. Checklist de Conclusão do Módulo

Acompanhe o progresso usando o **Roteiro de Tarefas** acima. Cada fase tem sua própria verificação.

- [ ] Fase 1: Setup e Estrutura — Completa
- [ ] Fase 2: Unidades de Medida — Completa
- [ ] Fase 3: Classificação — Completa
- [ ] Fase 4: Análise + Produto — Completa
- [ ] Fase 5: Grades e Variantes — Completa
- [ ] Fase 6: Composição — Completa
- [ ] Fase 7: Localização — Completa
- [ ] Fase 8: Lote e Série — Completa
- [ ] Fase 9: Análise + Estoque — Completa
- [ ] Fase 10: Análise + MovimentoEstoque + Testes Paridade — Completa
- [ ] Fase 11: Auxiliares — Completa
- [ ] Fase 12: Repositórios e Migrations — Completa

---

## Histórico de Alterações
- [ ] Unidade de medida implementada
- [ ] Classificação de produtos implementada (Grupo, Tipo, Linha, Marca)
- [ ] Produto implementado e testado
- [ ] Grade e Variantes implementados
- [ ] Composição implementada
- [ ] Controle de Lote e Série implementados
- [ ] Estoque e saldos implementados
- [ ] MovimentoEstoque implementado com custo médio correto
- [ ] Testes de paridade na movimentação de estoque
- [ ] Testes de integração cobrindo fluxo entrada→saída

---

## Histórico de Alterações

| Data | Autor | Alteração |
|---|---|---|
| 2026-04-27 | Gerado por análise | Criação inicial |

---

*Baseado em análise de `servidor/objeto de negócio/gestao.material/` — 84 arquivos — `projeto_tag_1906`*

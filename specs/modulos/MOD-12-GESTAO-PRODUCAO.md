# MOD-12 — Gestão de Produção

> **Fase:** 5 | **Localização:** `servidor/objeto de negócio/Gestao.Producao/`

## Visão Geral
Ordens de produção com consumo de matéria-prima e geração de produto acabado.

## Inventário

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `ProducaoEstoque.cs` | 15 KB | Entidade | Produto de produção |
| `ProducaoEstoqueVigencia.cs` | 16 KB | Entidade | Vigência da produção |
| `ProducaoEstoqueVigenciaItem.cs` | 22 KB | Relacionamento | Insumos da produção |
| `ProducaoMovimento.cs` | 38 KB | Entidade Central | Ordem de produção |
| `ProducaoMovimentoItem.cs` | 33 KB | Relacionamento | Item consumido/produzido |
| `ProducaoMovimentoCancelamento.cs` | 23 KB | Operação | Cancelamento |
| `ProducaoMovimentoLote.cs` | 13 KB | Relacionamento | Lote produzido |
| `TipoDocumentoProducao.cs` | 14 KB | Configuração | Tipo de produção |

## Regras Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-12-001 — Produção movimenta estoque duplo
Consumo de matéria prima (saída) + geração de produto acabado (entrada).
Ambos devem ser atômicos (mesma transação).

## Checklist MOD-12
- [ ] Estrutura de produção (vigência, insumos) implementada
- [ ] Ordem de produção implementada
- [ ] Movimentação de estoque bidirecional testada

---

## Roteiro de Tarefas — MOD-12 (Gestão de Produção)

Este roteiro descreve a implementação de MOD-12 em etapas claras, com foco em ordens de produção, consumo de insumos e geração de produto acabado em uma única transação.

### Fase 1: Setup e modelo de produção

**Tarefa 1.1 — Criar projeto Versatus.Producao**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.Producao`
- Adicione referências a `Versatus.Framework`, `Versatus.GestaoMaterial`, `Versatus.Estoque` e `Versatus.Ambiente`
- Configure `<Nullable>enable</Nullable>` no `.csproj`
- Branch: `setup/producao-project`
- Commit: `setup: Create Versatus.Producao project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Estoque/`, `Movimento/`, `Vigencia/`, `Relacionamentos/`
- Branch: `setup/producao-structure`
- Commit: `setup: Create Gestao.Producao folder structure`

### Fase 2: Implementar estoque de produção

**Tarefa 2.1 — Implementar entidade ProducaoEstoque**
- Crie `Domain/Estoque/ProducaoEstoque.cs` e `ProducaoEstoqueVigencia.cs`
- Defina regras de vigência e validade para matérias-primas e produtos acabados
- Branch: `feat/producao-estoque`
- Commit: `feat: Implement production stock entities`

**Tarefa 2.2 — Implementar itens de insumo e produção**
- Crie `Domain/Relacionamentos/ProducaoEstoqueVigenciaItem.cs`
- Defina relacionamentos de insumos por vigência e regras de consumo
- Branch: `feat/producao-insumos`
- Commit: `feat: Implement production input item entities`

### Fase 3: Implementar ordem de produção

**Tarefa 3.1 — Implementar movimento de produção**
- Crie `Domain/Movimento/ProducaoMovimento.cs` e `ProducaoMovimentoItem.cs`
- Defina o fluxo de criação de ordem de produção com itens consumidos e produzidos
- Use `Result<T>`/`ValidationResult` para regras de negócio de quantidade e custos
- Branch: `feat/producao-movimento`
- Commit: `feat: Implement production order entity`

**Tarefa 3.2 — Implementar lote e cancelamento**
- Crie `Domain/Relacionamentos/ProducaoMovimentoLote.cs` e `ProducaoMovimentoCancelamento.cs`
- Garanta cancelamento seguro da ordem e reversão ou marcação adequada do estoque
- Branch: `feat/producao-lote-cancelamento`
- Commit: `feat: Implement production lot and cancellation`

### Fase 4: Movimentação de estoque bidirecional

**Tarefa 4.1 — Consumir matéria-prima e gerar produto acabado**
- Implemente serviço de domínio que movimente estoque de insumos e estoque de produto acabado na mesma transação
- Garanta que a produção seja atômica e que falhas revertam ambas as movimentações
- Branch: `feat/producao-estoque-bidirecional`
- Commit: `feat: Implement bidirectional production stock movement`

**Tarefa 4.2 — Conferir disponibilidade e regra atômica**
- Valide disponibilidade de insumos antes de iniciar a ordem de produção
- Use `TransactionScope` ou padrões de unidade de trabalho para manter atomicidade
- Branch: `feat/producao-atomicidade`
- Commit: `feat: Ensure atomic production execution`

### Fase 5: Infraestrutura e testes

**Tarefa 5.1 — Configurar DbContext e repositórios**
- Crie `Infrastructure/ProducaoDbContext.cs` e configure `DbSet` para todas as entidades de produção
- Implemente repositórios e mapeamentos conforme padrão do projeto
- Branch: `feat/producao-dbcontext`
- Commit: `feat: Configure production DbContext and repositories`

**Tarefa 5.2 — Implementar Application Service de produção**
- Crie `Application/ProducaoService.cs` com métodos para criar ordem de produção, consumir insumos, finalizar e cancelar
- Separe lógica de domínio e aplicação usando `Result<T>` e serviços de domínio
- Branch: `feat/producao-service`
- Commit: `feat: Implement production application service`

**Tarefa 5.3 — Testes de unidade e integração**
- Crie testes para ordem de produção, consumo de insumos, geração de produto acabado e cancelamento
- Verifique transações atômicas e controle de estoque bidirecional
- Branch: `test/producao-scenarios`
- Commit: `test: Add production domain and inventory integration tests`

### Conclusão
- Valide o fluxo completo de produção: criação da ordem, consumo de insumos, geração de produto acabado e fechamento do lote.
- Branch final de entrega: `release/mod-12-gestao-producao`
- Commit final: `chore: Complete MOD-12 Gestão de Produção roadmap`

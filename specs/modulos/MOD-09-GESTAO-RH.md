# MOD-09 — Gestão de RH / Folha de Pagamento

> **Fase:** 5 | **Localização:** `servidor/objeto de negócio/Gestao.RH/`

## Visão Geral
Folha de pagamento, eventos, cálculos de FGTS, INSS, IRRF, rescisões, CAGED.

## Inventário (resumido)

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `Folha.cs` | 17 KB | Entidade Central | Folha de pagamento |
| `FolhaFechamento.cs` | 27 KB | Entidade | Fechamento da folha |
| `FolhaLiquidacao.cs` | 24 KB | Entidade | Liquidação financeira da folha |
| `Evento.cs` | 27 KB | Entidade | Evento de folha (provento/desconto) |
| `EventoProcessoBase.cs` | 10 KB | Base | Processamento de evento |
| `CalculoFolha.cs` | 9 KB | Processo | Motor de cálculo |
| `CalculoFuncionario.cs` | 15 KB | Processo | Cálculo por funcionário |
| `Encargo.cs` | 13 KB | Entidade | Encargo (FGTS, INSS) |
| `EncargoFilial.cs` | 9 KB | Relacionamento | Encargo por filial |
| `TabelaCalculo.cs` | 9 KB | Entidade | Tabela de cálculo (IRRF/INSS) |
| `RescisaoContrato.cs` | 16 KB | Entidade | Rescisão de contrato |
| `Caged.cs` | 7 KB | Entidade | CAGED |
| `Cargo.cs` | 7 KB | Entidade | Cargo |
| `TipoFolha.cs` | 6 KB | Configuração | Tipo de folha |

## Regras Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-09-001 — Tabelas INSS/IRRF têm vigência
Consultar sempre pela competência do mês de cálculo.

### RN-09-002 — Cálculo por competência
Cada folha é calculada por mês de competência. Reprocessamentos devem respeitar
o estado vigente das tabelas e eventos naquela competência.

### RN-09-003 — Folha gera Financeiro
O fechamento da folha (`FolhaFechamento`) gera pagamentos em MOD-05.

## Checklist MOD-09
- [ ] Cargo e estrutura básica criados
- [ ] Evento e tipos de evento criados
- [ ] Tabelas de cálculo (IRRF/INSS) criadas
- [ ] Motor de cálculo de folha implementado
- [ ] Fechamento e liquidação implementados
- [ ] Rescisão implementada
- [ ] CAGED implementado
- [ ] Testes de cálculo de folha com paridade

---

## Roteiro de Tarefas — MOD-09 (Gestão de RH / Folha de Pagamento)

Siga esta sequência para migrar MOD-09. Cada fase deve ser implementada em commits separados e revisada por PR. A folha é calculada por competência; por isso, a análise de tabelas e eventos deve vir antes da geração de pagamentos.

### Fase 1: Setup e Estrutura

**Tarefa 1.1 — Criar projeto Versatus.GestaoRH**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.GestaoRH`
- Adicione referências a `Versatus.Framework`, `Versatus.AcessoGlobal`, `Versatus.GestaoFinanceira`
- Configure `<Nullable>enable</Nullable>` no arquivo .csproj
- Branch: `setup/gestao-rh-project`
- Commit: `setup: Create Versatus.GestaoRH project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Folha/`, `Eventos/`, `Calculo/`, `Encargos/`, `Tabelas/`, `Rescisao/`, `Caged/`
- Branch: `setup/gestao-rh-structure`
- Commit: `setup: Create GestaoRH folder structure`

**Tarefa 1.3 — Criar DbContext base**
- Crie `Infrastructure/GestaoRHDbContext.cs` com `DbSet` vazios inicialmente
- Branch: `setup/gestao-rh-dbcontext-base`
- Commit: `setup: Create GestaoRHDbContext base`

### Fase 2: Estrutura Básica de Folha

**Tarefa 2.1 — Implementar Folha e estruturas principais**
- Crie `Domain/Folha/Folha.cs`, `FolhaFechamento.cs`, `FolhaLiquidacao.cs`
- Defina campos de competência, estado, valores totais e links para financeiro
- Branch: `feat/folha-entity`
- Commit: `feat: Implement Folha entities`

**Tarefa 2.2 — Implementar Evento de Folha**
- Crie `Domain/Eventos/Evento.cs`, `EventoProcessoBase.cs`
- Defina tipos de evento (provento, desconto, benefícios)
- Branch: `feat/evento-entity`
- Commit: `feat: Implement payroll event entities`

**Tarefa 2.3 — Configurar folha no DbContext**
- Atualize `GestaoRHDbContext` com `DbSet` de folha e eventos
- Branch: `feat/folha-dbcontext`
- Commit: `feat: Configure payroll entities in DbContext`
- Marque no checklist: ✅ Fase 2 completa

### Fase 3: Tabelas de Cálculo

**Tarefa 3.1 — Implementar tabelas de IRRF/INSS**
- Crie `Domain/Tabelas/TabelaCalculo.cs` e outros objetos de vigência necessários
- Garanta que as tabelas sejam consultadas por competência
- Branch: `feat/tabelas-calculo-entities`
- Commit: `feat: Implement payroll tables entities`

**Tarefa 3.2 — Testar vigência de tabelas**
- Crie testes que verifiquem a seleção correta das tabelas por data de competência
- Branch: `test/tabelas-vigencia`
- Commit: `test: Add table validity tests`
- Marque no checklist: ✅ Fase 3 completa

### Fase 4: Motor de Cálculo de Folha

**Tarefa 4.1 — Implementar CalculoFolha**
- Crie `Domain/Calculo/CalculoFolha.cs` e `CalculoFuncionario.cs`
- Implemente regras de cálculo para proventos, descontos e encargos
- Branch: `feat/calculo-folha-entity`
- Commit: `feat: Implement payroll calculation engine`

**Tarefa 4.2 — Criar serviços de cálculo**
- Crie handlers ou serviços em `Application/` para processar a folha por competência
- Branch: `feat/calculo-folha-services`
- Commit: `feat: Implement payroll calculation services`

**Tarefa 4.3 — Testar cálculo de folha**
- Adicione testes unitários para cenários de cálculo básicos e complexos
- Branch: `test/calculo-folha`
- Commit: `test: Add payroll calculation tests`
- Marque no checklist: ✅ Fase 4 completa

### Fase 5: Fechamento, Liquidação e Integração Financeira

**Tarefa 5.1 — Implementar Fechamento da Folha**
- Crie `FolhaFechamento.cs` e integre com `FolhaLiquidacao.cs`
- Garanta que o fechamento gere lançamentos para MOD-05 Financeiro
- Branch: `feat/folha-fechamento`
- Commit: `feat: Implement payroll closing entity`

**Tarefa 5.2 — Implementar Liquidação da Folha**
- Crie lógica de liquidação financeira da folha e integração com contas a pagar/receber
- Branch: `feat/folha-liquidacao`
- Commit: `feat: Implement payroll liquidation logic`

**Tarefa 5.3 — Testar integração financeira**
- Adicione testes de integração entre MOD-09 e MOD-05
- Branch: `test/folha-financeiro-integration`
- Commit: `test: Add payroll finance integration tests`
- Marque no checklist: ✅ Fase 5 completa

### Fase 6: Rescisão e CAGED

**Tarefa 6.1 — Implementar Rescisão**
- Crie `Domain/Rescisao/RescisaoContrato.cs`
- Inclua regras de cálculo e liquidação de rescisão
- Branch: `feat/rescisao-entity`
- Commit: `feat: Implement contract termination entity`

**Tarefa 6.2 — Implementar CAGED**
- Crie `Domain/Caged/Caged.cs`
- Garanta que os dados necessários para o CAGED sejam gerados a partir da folha
- Branch: `feat/caged-entity`
- Commit: `feat: Implement CAGED entity`

**Tarefa 6.3 — Testar rescisão e CAGED**
- Adicione testes para cálculo de rescisão e geração de CAGED
- Branch: `test/rescisao-caged`
- Commit: `test: Add termination and CAGED tests`
- Marque no checklist: ✅ Fase 6 completa

### Fase 7: Repositórios e Migrations

**Tarefa 7.1 — Criar repositórios chave**
- Crie `Infrastructure/Repositorios/IFolhaRepository.cs`, `IEventoRepository.cs`, `IFolhaLiquidacaoRepository.cs`
- Branch: `feat/gestao-rh-repositories`
- Commit: `feat: Implement payroll repository interfaces`

**Tarefa 7.2 — Criar migrations EF Core**
- Crie a primeira migration para `GestaoRHDbContext`
- Valide todos os relacionamentos e tabelas
- Branch: `setup/gestao-rh-migrations`
- Commit: `setup: Create initial EF Core migrations for GestaoRH`

**Tarefa 7.3 — Registrar DI**
- Crie método de extensão para registrar DbContext, serviços e repositórios
- Branch: `setup/gestao-rh-di`
- Commit: `setup: Configure dependency injection for GestaoRH`
- Marque no checklist: ✅ Fase 7 completa

### Fase 8: Testes de Paridade e Integração

**Tarefa 8.1 — Testar paridade de folha**
- Crie testes que comparem resultados de cálculo com casos do legado
- Branch: `test/folha-parity`
- Commit: `test: Add payroll parity tests`

**Tarefa 8.2 — Testar ciclo completo de pagamento**
- Adicione testes de ponta a ponta para geração de folha, fechamento e liquidação
- Branch: `test/folha-e2e`
- Commit: `test: Add payroll end-to-end tests`
- Marque no checklist: ✅ Fase 8 completa

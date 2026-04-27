# MOD-10 — Gestão de Contratos

> **Fase:** 5 | **Localização:** `servidor/objeto de negócio/Gestao.Contrato/`

## Visão Geral
Contratos de serviços recorrentes com financeiro automático, reajustes e prorrogações.

## Inventário

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TipoContrato.cs` | 33 KB | Configuração | Tipo de contrato |
| `TipoNumeroContrato.cs` | 16 KB | Configuração | Numeração |
| `Contrato.cs` | **51 KB** | Entidade Central | Contrato |
| `ContratoFase.cs` | 10 KB | Relacionamento | Fase do contrato |
| `ContratoFinanceiro.cs` | 26 KB | Entidade | Financeiro do contrato |
| `ContratoFinanceiroParcela.cs` | 10 KB | Relacionamento | Parcelas geradas |
| `ContratoReajusteFinanceiro.cs` | 22 KB | Operação | Reajuste de valores |
| `ContratoProrrogacao.cs` | 7 KB | Operação | Prorrogação do contrato |
| `CancelamentoContrato.cs` | 13 KB | Operação | Cancelamento |
| `ObjetoContrato.cs` | 21 KB | Entidade | Objeto do contrato |
| `ObjetoContratoItem.cs` | 27 KB | Relacionamento | Item do objeto |
| `TemplateContrato.cs` | 8 KB | Entidade | Template de contrato |
| `GrupoContrato.cs` | 9 KB | Entidade | Grupo |

## Regras Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-10-001 — Geração automática de financeiro
Contratos geram parcelas financeiras automaticamente conforme periodicidade.

### RN-10-002 — Reajuste por índice
`ContratoReajusteFinanceiro` aplica reajuste baseado em `IndiceEconomico` (MOD-02).

## Checklist MOD-10
- [ ] TipoContrato implementado
- [ ] Contrato básico implementado
- [ ] Financeiro do contrato implementado
- [ ] Reajuste implementado
- [ ] Prorrogação e cancelamento implementados

---

## Roteiro de Tarefas — MOD-10 (Gestão de Contratos)

Este roteiro descreve a implementação de MOD-10 em fases pequenas, com foco em domínio rico, validação sem exceções de fluxo e geração automática de financeiro.

### Fase 1: Setup e modelo de domínio

**Tarefa 1.1 — Criar projeto Versatus.Contrato**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.Contrato`
- Adicione referências a `Versatus.Framework`, `Versatus.GestaoFinanceira`, `Versatus.GestaoTributo` e `Versatus.Ambiente`
- Configure `<Nullable>enable</Nullable>` no `.csproj`
- Branch: `setup/contrato-project`
- Commit: `setup: Create Versatus.Contrato project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Configuracao/`, `Contrato/`, `Financeiro/`, `Operacoes/`, `Templates/`
- Branch: `setup/contrato-structure`
- Commit: `setup: Create Gestao.Contrato folder structure`

### Fase 2: Implementar contratos e configurações

**Tarefa 2.1 — Implementar entidades de configuração**
- Crie `Domain/Configuracao/TipoContrato.cs` e `TipoNumeroContrato.cs`
- Garanta que as configurações permitam identificação de formato e numeração automática
- Branch: `feat/contrato-config`
- Commit: `feat: Implement contract configuration entities`

**Tarefa 2.2 — Implementar entidade Contrato**
- Crie `Domain/Contrato/Contrato.cs` com regras de validade, vigência e estado
- Inclua relacionamentos a `ObjetoContrato`, `ContratoFase`, `ContratoFinanceiro` e `TemplateContrato`
- Use `Result<T>`/`ValidationResult` para validações de entrada e invariantes
- Branch: `feat/contrato-entity`
- Commit: `feat: Implement Contrato entity`

**Tarefa 2.3 — Implementar objeto do contrato**
- Crie `Domain/Contrato/ObjetoContrato.cs` e `ObjetoContratoItem.cs`
- Defina regras para itens, quantidades, preços e responsabilidades
- Branch: `feat/contrato-objeto`
- Commit: `feat: Implement contract object and items`

### Fase 3: Financeiro automático do contrato

**Tarefa 3.1 — Implementar contrato financeiro**
- Crie `Domain/Financeiro/ContratoFinanceiro.cs` e `ContratoFinanceiroParcela.cs`
- Defina o ciclo de giro financeiro que gera parcelas conforme periodicidade do contrato
- Branch: `feat/contrato-financeiro`
- Commit: `feat: Implement contract financial entity`

**Tarefa 3.2 — Gerar parcelas automaticamente**
- Implemente serviço de domínio que cria `ContratoFinanceiroParcela` ao confirmar contrato
- Garanta cálculos corretos de data de vencimento, valor e status
- Branch: `feat/contrato-parcela-generator`
- Commit: `feat: Implement automatic contract installment generation`

### Fase 4: Reajustes e prorrogações

**Tarefa 4.1 — Implementar reajuste financeiro**
- Crie `Domain/Operacoes/ContratoReajusteFinanceiro.cs`
- Use `IndiceEconomico` de MOD-02 para calcular ajustes de valor
- Garanta histórico de reajustes e aplicação ao financeiro do contrato
- Branch: `feat/contrato-reajuste`
- Commit: `feat: Implement contract financial adjustment`

**Tarefa 4.2 — Implementar prorrogação**
- Crie `Domain/Operacoes/ContratoProrrogacao.cs`
- Atualize vigência, calendário de parcelas e status do contrato
- Considere regras de prorrogação automática e manual
- Branch: `feat/contrato-prorrogacao`
- Commit: `feat: Implement contract extension`

### Fase 5: Cancelamento, infra e testes

**Tarefa 5.1 — Implementar cancelamento de contrato**
- Crie `Domain/Operacoes/CancelamentoContrato.cs`
- Garanta que contratos cancelados não gerem novas parcelas e que as existentes sejam marcadas corretamente
- Branch: `feat/contrato-cancelamento`
- Commit: `feat: Implement contract cancellation`

**Tarefa 5.2 — Configurar DbContext e repositórios**
- Crie `Infrastructure/ContratoDbContext.cs` e configure `DbSet` para todas as entidades de contrato
- Implemente repositórios e mapeamentos se necessário
- Branch: `feat/contrato-dbcontext`
- Commit: `feat: Configure contract DbContext and repositories`

**Tarefa 5.3 — Implementar Application Service de contrato**
- Crie `Application/ContratoService.cs` com métodos para criar, alterar, prorrogar, reajustar e cancelar contratos
- Use `Result<T>` e padrões de validação para separar domínio de aplicação
- Branch: `feat/contrato-service`
- Commit: `feat: Implement contract application service`

**Tarefa 5.4 — Testes de unidade e integração**
- Crie testes para validação de contrato, geração de parcelas, reajuste e prorrogação
- Valide cenários de cancelamento e regra de gestão financeira recorrente
- Branch: `test/contrato-scenarios`
- Commit: `test: Add contract domain and financial scenario tests`

### Conclusão
- Depois que as entidades de contrato, financeiro, reajuste e prorrogação estiverem implementadas, valide o fluxo completo com um contrato gerado e uma parcela financeira criada automaticamente.
- Branch final de entrega: `release/mod-10-gestao-contratos`
- Commit final: `chore: Complete MOD-10 Gestão de Contratos roadmap`

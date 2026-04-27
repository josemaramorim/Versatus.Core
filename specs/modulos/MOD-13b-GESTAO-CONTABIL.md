# MOD-13b — Gestão Contábil

> **Fase:** 5 | **Prioridade:** BAIXA

## Visão Geral
Sistema de gestão contábil para registro de lançamentos, fechamento de períodos e conformidade fiscal. Inclui plano de contas, diário contábil, balancetes e integrações automáticas com faturamento, financeiro e tributário. Suporte a legislação brasileira (SPED Contábil, ECF) e múltiplas moedas.

## Inventário Técnico

### Estrutura do Projeto
- `Versatus.Gestao.Contabil.csproj` (Class Library .NET 8)
- `Domain/Contabil/`: Entidades, value objects e regras contábeis
- `Application/Contabil/`: Casos de uso, comandos, queries e validações
- `Infrastructure/Contabil/`: Repositórios, DbContext e exportações SPED

### Entidades Principais
- `LancamentoContabil`: Registro de débito/crédito com data, valor e histórico
- `PlanoContas`: Estrutura hierárquica de contas contábeis
- `PeriodoContabil`: Períodos de apuração (mensal, trimestral)
- `DiarioContabil`: Conjunto de lançamentos por período
- `Balancete`: Relatório de saldos por conta
- `SpedContabil`: Estrutura para exportação SPED

### Serviços e Interfaces
- `ILancamentoService`: Criação e validação de lançamentos
- `IFechamentoService`: Processamento de fechamento contábil
- `IBalanceteService`: Geração de relatórios e balanços
- `ISpedService`: Exportação para SPED e validações fiscais
- `IPlanoContasService`: Manutenção da estrutura de contas

### Dependências
- `Versatus.Gestao.Financeira`: Para lançamentos automáticos de transações
- `Versatus.Gestao.Tributo`: Para cálculos fiscais e integrações
- `Versatus.Framework`: Para validações e infraestrutura
- Entity Framework Core para persistência

## Regras de Negócio
- **Partida Dobrada**: Todo lançamento deve equilibrar débito/crédito
- **Imutabilidade**: Lançamentos fechados não podem ser alterados
- **Conformidade Fiscal**: Validações para SPED e legislação brasileira
- **Centro de Custos**: Alocação opcional por departamento/projeto
- **Múltiplas Moedas**: Conversão automática com taxas de câmbio
- **Fechamento Automático**: Geração de lançamentos de ajuste

## Checklist de Implementação
- [ ] Auditar lançamentos contábeis do legado
- [ ] Definir schema para plano de contas e lançamentos
- [ ] Implementar entidades com validações de partida dobrada
- [ ] Criar serviços de lançamento e fechamento
- [ ] Integrar com Gestao.Financeira para automação
- [ ] Implementar exportação SPED Contábil
- [ ] Adicionar relatórios de balancete e balanço
- [ ] Configurar testes e validações fiscais
- [ ] Otimizar queries para relatórios pesados
- [ ] Documentar integrações com sistemas externos

## Roadmap de Migração

### Fase 1: Setup e Estrutura Base (1-2 semanas)
- Criar projeto .NET 8 `Versatus.Gestao.Contabil`
- Definir entidades básicas (LancamentoContabil, PlanoContas)
- Configurar DbContext e migrations
- Implementar repositórios base
- **Entrega:** Estrutura compilável com CRUD básico

### Fase 2: Core de Lançamentos (2-3 semanas)
- Implementar LancamentoService com validações
- Adicionar PeriodoContabil e fechamento básico
- Integrar com Gestao.Financeira
- Criar DiárioContabil e relatórios simples
- **Entrega:** Lançamentos manuais funcionais

### Fase 3: Relatórios e SPED (2 semanas)
- Implementar BalanceteService e relatórios avançados
- Adicionar exportação SPED Contábil
- Integrar com Gestao.Tributo para fiscais
- Otimizar performance de queries
- **Entrega:** Sistema contábil completo

### Fase 4: Testes e Conformidade (1 semana)
- Cobertura de testes >80%
- Validações finais de legislação
- Documentação técnica
- **Entrega:** Módulo pronto para produção

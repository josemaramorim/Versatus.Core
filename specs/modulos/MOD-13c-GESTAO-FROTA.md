# MOD-13c — Gestão Frota

> **Fase:** 5 | **Prioridade:** BAIXA

## Visão Geral
Sistema de gestão de frota para controle de veículos, motoristas, manutenção preventiva/corretiva e custos operacionais. Inclui alocação de veículos, rastreamento de quilometragem, abastecimentos e integrações com financeiro para rateio de custos. Suporte a diferentes tipos de veículos e conformidade com regulamentações de transporte.

## Inventário Técnico

### Estrutura do Projeto
- `Versatus.Gestao.Frota.csproj` (Class Library .NET 8)
- `Domain/Frota/`: Entidades, value objects e regras de frota
- `Application/Frota/`: Casos de uso, comandos, queries e validações
- `Infrastructure/Frota/`: Repositórios, DbContext e integrações GPS

### Entidades Principais
- `Veiculo`: Cadastro de veículos com placa, modelo, capacidade
- `Motorista`: Condutores com CNH, treinamentos e alocações
- `Manutencao`: Ordens de serviço preventiva/corretiva
- `Abastecimento`: Registros de combustível e lubrificantes
- `Viagem`: Roteiros com origem/destino e quilometragem
- `CustoFrota`: Rateio de custos por veículo/período

### Serviços e Interfaces
- `IVeiculoService`: Gerenciamento de frota e alocações
- `IManutencaoService`: Agendamento e controle de manutenções
- `IViagemService`: Registro e rastreamento de viagens
- `ICustoService`: Cálculo e rateio de custos operacionais
- `IRelatorioService`: Dashboards de performance da frota

### Dependências
- `Versatus.Gestao.Financeira`: Para rateio de custos
- `Versatus.Gestao.Transporte`: Para integração com rotas
- `Versatus.Framework`: Para validações e infraestrutura
- Entity Framework Core para persistência

## Regras de Negócio
- **Manutenção Preventiva**: Agendamento automático por quilometragem/tempo
- **Controle de CNH**: Validação de vencimento e categorias
- **Rateio de Custos**: Alocação proporcional por uso
- **Limites de Quilometragem**: Alertas para revisões obrigatórias
- **Conformidade**: Validações para regulamentações de transporte
- **Rastreamento**: Integração opcional com GPS/telemática

## Checklist de Implementação
- [ ] Auditar dados de frota do legado
- [ ] Definir schema para veículos e manutenções
- [ ] Implementar entidades com validações
- [ ] Criar serviços de alocação e manutenção
- [ ] Integrar com Gestao.Financeira para custos
- [ ] Adicionar rastreamento de viagens
- [ ] Implementar relatórios de performance
- [ ] Configurar alertas automáticos
- [ ] Otimizar queries para dashboards
- [ ] Documentar integrações com sistemas GPS

## Roadmap de Migração

### Fase 1: Setup e Estrutura Base (1-2 semanas)
- Criar projeto .NET 8 `Versatus.Gestao.Frota`
- Definir entidades básicas (Veiculo, Motorista)
- Configurar DbContext e migrations
- Implementar repositórios base
- **Entrega:** Estrutura compilável com CRUD

### Fase 2: Core de Operações (2-3 semanas)
- Implementar ManutencaoService e agendamentos
- Adicionar Abastecimento e controle de custos
- Criar Viagem e rastreamento básico
- Integrar com Gestao.Financeira
- **Entrega:** Operações diárias funcionais

### Fase 3: Relatórios e Otimização (2 semanas)
- Implementar RelatorioService e dashboards
- Adicionar alertas automáticos
- Integrar com Gestao.Transporte
- Otimizar performance de queries
- **Entrega:** Sistema completo de frota

### Fase 4: Testes e Refinamentos (1 semana)
- Cobertura de testes >80%
- Validações finais e ajustes
- Documentação técnica
- **Entrega:** Módulo pronto para produção

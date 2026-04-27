# MOD-13a — Gestão Armazém

> **Fase:** 5 | **Prioridade:** BAIXA

## Visão Geral
Sistema de gestão de armazém/WMS (Warehouse Management System) para controle completo do fluxo de mercadorias em depósitos. Inclui processos de recepção, armazenagem, separação e expedição, com rastreamento em tempo real de estoques, localizações e movimentações. Integra-se ao módulo de estoque para atualizações atômicas e ao transporte para otimização de rotas e cargas.

## Inventário Técnico

### Estrutura do Projeto
- `Versatus.Gestao.Armazem.csproj` (Class Library .NET 8)
- `Domain/Armazem/`: Entidades, value objects e regras de domínio
- `Application/Armazem/`: Casos de uso, comandos, queries e validações
- `Infrastructure/Armazem/`: Repositórios, DbContext e integrações externas

### Entidades Principais
- `Armazem`: Representa um depósito físico com endereços, capacidades e configurações
- `Localizacao`: Posições no armazém (prateleiras, corredores, pallets)
- `ItemArmazenado`: Item específico em localização com quantidade, lote e validade
- `MovimentacaoArmazem`: Registro de entradas/saídas com tipos (recepção, putaway, picking, shipping)
- `OrdemSeparacao`: Ordem de picking para expedição
- `InventarioArmazem`: Contagem cíclica ou anual de estoques

### Serviços e Interfaces
- `IRecebimentoService`: Processa chegada de mercadorias e alocação inicial
- `IArmazenamentoService`: Gerencia putaway e otimização de localizações
- `ISeparacaoService`: Coordena picking e packing para ordens
- `IExpedicaoService`: Finaliza expedição e integração com transporte
- `IInventarioService`: Executa contagens e ajustes de estoque

### Dependências
- `Versatus.Gestao.Estoque`: Para atualizações de saldos e reservas
- `Versatus.Gestao.Transporte`: Para roteirização e cargas
- `Versatus.Framework`: Para logging, validação e infraestrutura base
- Entity Framework Core para persistência

## Regras de Negócio
- **Controle de Capacidade**: Validação de peso/volume por localização para evitar sobrecarga
- **LIFO/FIFO**: Aplicação por produto/lote conforme configuração
- **Rastreamento de Lotes**: Controle de validade e recall obrigatório
- **Separação Otimizada**: Algoritmos para minimizar distâncias de picking
- **Inventário Cíclico**: Contagens automáticas por zona/prioridade
- **Integração Atômica**: Movimentações sincronizadas com estoque para consistência

## Checklist de Implementação
- [ ] Auditar código legado para mapear classes de armazém e movimentação
- [ ] Definir schema do banco para tabelas de armazém e localizações
- [ ] Implementar entidades de domínio com validações
- [ ] Criar repositórios com queries otimizadas para localização
- [ ] Desenvolver serviços de movimentação com transações
- [ ] Integrar com Gestao.Estoque para atualizações de saldo
- [ ] Implementar algoritmos de otimização de armazenagem
- [ ] Adicionar testes unitários e de integração
- [ ] Configurar migrations do EF Core
- [ ] Documentar APIs para integração com OMS/transporte

## Roadmap de Migração

### Fase 1: Setup e Estrutura Base (1-2 semanas)
- Criar projeto .NET 8 `Versatus.Gestao.Armazem`
- Definir entidades básicas (Armazem, Localizacao)
- Configurar DbContext e migrations iniciais
- Implementar repositórios base
- **Entrega:** Estrutura compilável com testes básicos

### Fase 2: Core de Movimentação (2-3 semanas)
- Implementar MovimentacaoArmazem e serviços de CRUD
- Adicionar validações de negócio (capacidade, lotes)
- Integrar com Gestao.Estoque para sincronização
- Criar OrdemSeparacao e lógica de picking
- **Entrega:** Movimentações básicas funcionais

### Fase 3: Otimização e Integrações (2 semanas)
- Implementar algoritmos de putaway e separação otimizada
- Integrar com Gestao.Transporte para expedição
- Adicionar inventário cíclico
- Otimizar queries para performance
- **Entrega:** WMS funcional com integrações

### Fase 4: Testes e Refinamentos (1 semana)
- Cobertura de testes >80%
- Validações finais e ajustes de performance
- Documentação técnica completa
- **Entrega:** Módulo pronto para produção

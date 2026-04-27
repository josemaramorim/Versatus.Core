# MOD-13d — Gestão Transporte

> **Fase:** 5 | **Prioridade:** BAIXA

## Visão Geral
Sistema de gestão de transporte para planejamento, execução e controle de distribuição de mercadorias. Inclui romaneios, roteirização otimizada, rastreamento de entregas e integração com transportadoras. Suporte a diferentes modalidades (rodoviário, aéreo, marítimo) e conformidade com CT-e e MDF-e.

## Inventário Técnico

### Estrutura do Projeto
- `Versatus.Gestao.Transporte.csproj` (Class Library .NET 8)
- `Domain/Transporte/`: Entidades, value objects e regras logísticas
- `Application/Transporte/`: Casos de uso, comandos, queries e validações
- `Infrastructure/Transporte/`: Repositórios, DbContext e integrações CT-e/MDF-e

### Entidades Principais
- `Romaneio`: Manifesto de carga com itens e destinos
- `Transporte`: Veículo/carga com rota e status
- `Entrega`: Registro de entrega com confirmação
- `Rota`: Planejamento otimizado de entregas
- `Transportadora`: Cadastro de terceiros logísticos
- `Cte`: Conhecimento de Transporte Eletrônico

### Serviços e Interfaces
- `IRomaneioService`: Criação e gerenciamento de romaneios
- `IRoteirizacaoService`: Otimização de rotas e cargas
- `IEntregaService`: Rastreamento e confirmação de entregas
- `ICteService`: Emissão e transmissão de CT-e
- `IRelatorioService`: Dashboards de performance logística

### Dependências
- `Versatus.Gestao.Estoque`: Para reserva e expedição
- `Versatus.Gestao.Faturamento`: Para frete e cobrança
- `Versatus.CTe.Comm`: Para comunicação SEFAZ
- `Versatus.Framework`: Para validações
- Entity Framework Core para persistência

## Regras de Negócio
- **Otimização de Rotas**: Algoritmos para minimizar distâncias/custos
- **Capacidade de Carga**: Validação de peso/volume por veículo
- **Rastreamento em Tempo Real**: Atualizações de status de entrega
- **Conformidade Fiscal**: Emissão automática de CT-e/MDF-e
- **Integração com Transportadoras**: API para terceiros
- **Janelas de Entrega**: Controle de horários e SLAs

## Checklist de Implementação
- [ ] Auditar romaneios e rotas do legado
- [ ] Definir schema para transporte e entregas
- [ ] Implementar entidades com validações
- [ ] Criar serviços de roteirização
- [ ] Integrar com CT-e para fiscais
- [ ] Adicionar rastreamento GPS
- [ ] Implementar relatórios logísticos
- [ ] Configurar APIs para transportadoras
- [ ] Otimizar algoritmos de otimização
- [ ] Documentar integrações com SEFAZ

## Roadmap de Migração

### Fase 1: Setup e Estrutura Base (1-2 semanas)
- Criar projeto .NET 8 `Versatus.Gestao.Transporte`
- Definir entidades básicas (Romaneio, Transporte)
- Configurar DbContext e migrations
- Implementar repositórios base
- **Entrega:** Estrutura compilável com CRUD

### Fase 2: Core de Distribuição (2-3 semanas)
- Implementar RoteirizacaoService
- Adicionar Entrega e rastreamento básico
- Criar RomaneioService
- Integrar com Gestao.Estoque
- **Entrega:** Distribuição básica funcional

### Fase 3: Fiscal e Otimização (2 semanas)
- Implementar CteService e transmissão
- Adicionar algoritmos de otimização
- Integrar com Gestao.Faturamento
- Otimizar performance
- **Entrega:** Sistema logístico completo

### Fase 4: Testes e Refinamentos (1 semana)
- Cobertura de testes >80%
- Validações finais
- Documentação técnica
- **Entrega:** Módulo pronto para produção

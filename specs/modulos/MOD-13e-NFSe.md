# MOD-13e — NFSe

> **Fase:** 5 | **Prioridade:** BAIXA

## Visão Geral
Sistema de emissão e gestão de Nota Fiscal de Serviços Eletrônica (NFS-e) para prestação de serviços. Complementa o módulo NF-e com suporte a legislação municipal, diferentes padrões por cidade (ABRASF, ISS-Net) e integração com faturamento. Inclui emissão, consulta, cancelamento e relatórios fiscais.

## Inventário Técnico

### Estrutura do Projeto
- `Versatus.NFSe.csproj` (Class Library .NET 8)
- `Domain/NFSe/`: Entidades, value objects e regras fiscais
- `Application/NFSe/`: Casos de uso, comandos, queries e validações
- `Infrastructure/NFSe/`: Repositórios, DbContext e comunicação municipal

### Entidades Principais
- `NotaFiscalServico`: NFS-e com tomador, prestador e serviços
- `Tomador`: Cliente que recebe o serviço
- `Prestador`: Empresa emissora
- `Servico`: Item de serviço com códigos ISS
- `LoteRps`: Lote de RPS para transmissão
- `ConsultaNfse`: Resultados de consultas

### Serviços e Interfaces
- `IEmissaoService`: Geração e transmissão de NFS-e
- `IConsultaService`: Consulta e download de notas
- `ICancelamentoService`: Cancelamento e correção
- `ILoteService`: Gerenciamento de lotes RPS
- `IRelatorioService`: Relatórios fiscais e municipais

### Dependências
- `Versatus.Gestao.Faturamento`: Para integração com vendas
- `Versatus.Gestao.Contabil`: Para lançamentos fiscais
- `Versatus.NFSe.Comm`: Para comunicação municipal
- `Versatus.Framework`: Para validações
- Entity Framework Core para persistência

## Regras de Negócio
- **Legislação Municipal**: Padrões variam por cidade (ABRASF 2.0, etc.)
- **Códigos de Serviço**: Classificação ISS obrigatória
- **Tomador Obrigatório**: Identificação completa do cliente
- **Transmissão por Lote**: RPS agrupados para eficiência
- **Cancelamento Restrito**: Regras específicas por município
- **Integração Fiscal**: Lançamentos automáticos contábeis

## Checklist de Implementação
- [ ] Auditar NFS-e do legado
- [ ] Definir schema para notas e serviços
- [ ] Implementar entidades com validações
- [ ] Criar serviços de emissão e transmissão
- [ ] Integrar com NFSe.Comm para municipais
- [ ] Adicionar consulta e cancelamento
- [ ] Implementar relatórios fiscais
- [ ] Configurar testes por município
- [ ] Otimizar transmissão em lote
- [ ] Documentar variações municipais

## Roadmap de Migração

### Fase 1: Setup e Estrutura Base (1-2 semanas)
- Criar projeto .NET 8 `Versatus.NFSe`
- Definir entidades básicas (NotaFiscalServico, Servico)
- Configurar DbContext e migrations
- Implementar repositórios base
- **Entrega:** Estrutura compilável com CRUD

### Fase 2: Core de Emissão (2-3 semanas)
- Implementar EmissaoService e transmissão
- Adicionar LoteRps e processamento
- Criar ConsultaService
- Integrar com Gestao.Faturamento
- **Entrega:** Emissão básica funcional

### Fase 3: Gestão e Relatórios (2 semanas)
- Implementar CancelamentoService
- Adicionar RelatorioService
- Integrar com Gestao.Contabil
- Suporte a múltiplos padrões municipais
- **Entrega:** Sistema NFS-e completo

### Fase 4: Testes e Conformidade (1 semana)
- Cobertura de testes >80%
- Validações municipais
- Documentação técnica
- **Entrega:** Módulo pronto para produção

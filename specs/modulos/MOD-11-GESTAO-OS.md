# MOD-11 — Gestão de Ordem de Serviço (OS)

> **Fase:** 5 | **Localização:** `servidor/objeto de negócio/Gestao.OS/`

## Visão Geral
Ordem de serviço para assistência técnica e manutenção.

## Inventário

| Classe Legada | Tamanho | Tipo | Descrição |
|---|---|---|---|
| `TipoDocumentoOS.cs` | 49 KB | Configuração | Tipo de OS |
| `TipoDocumentoOSFilial.cs` | 21 KB | Relacionamento | Tipo por filial |
| `DocumentoOS.cs` | **167 KB** | Entidade Central | Ordem de serviço (GRANDE) |
| `DocumentoOSItem.cs` | 18 KB | Relacionamento | Peça/produto na OS |
| `DocumentoOSItemBase.cs` | 37 KB | Base | Base do item |
| `DocumentoOSServico.cs` | 16 KB | Relacionamento | Serviço na OS |
| `DocumentoOSParcela.cs` | 9 KB | Relacionamento | Parcelas financeiras |
| `DocumentoOSSituacao.cs` | 5 KB | Situação | Estado da OS |
| `DocumentoOSAcessorio.cs` | 5 KB | Relacionamento | Acessórios recebidos |
| `DocumentoOSComissionado.cs` | 7 KB | Relacionamento | Técnico comissionado |
| `DocumentoOSLacre.cs` | 4 KB | Relacionamento | Lacres |
| `Tecnico.cs` | 8 KB | Entidade | Técnico responsável |
| `Acessorio.cs` | 7 KB | Entidade | Acessório |

## Regras Críticas

> **Regra de validação:** não use exceções para fluxo de validação esperado. Erros de entrada e regras de negócio comuns devem retornar `Result<T>`/`ValidationResult` ou usar padrão Notification. Exceções `VersatusException` ficam reservadas para falhas inesperadas, invariantes violados ou erros graves.

### RN-11-001 — OS gera Faturamento
A conclusão da OS pode gerar uma NF de serviço ou venda.

### RN-11-002 — Comissão por técnico
Serviços podem gerar comissão para o técnico responsável.

## Checklist MOD-11
- [ ] TipoDocumentoOS analisado
- [ ] DocumentoOS básico implementado
- [ ] Peças e serviços implementados
- [ ] Situação (abertura, execução, conclusão, cancelamento) implementada
- [ ] Integração com faturamento testada

---

## Roteiro de Tarefas — MOD-11 (Gestão de Ordem de Serviço)

Este roteiro descreve a implementação de MOD-11 em etapas claras, com foco em domínio rico, estrutura de OS e integração com faturamento.

### Fase 1: Setup e estrutura de domínio

**Tarefa 1.1 — Criar projeto Versatus.OS**
- Crie um novo projeto .NET 8 Class Library chamado `Versatus.OS`
- Adicione referências a `Versatus.Framework`, `Versatus.GestaoFinanceira`, `Versatus.GestaoTributo` e `Versatus.GestaoMaterial`
- Configure `<Nullable>enable</Nullable>` no `.csproj`
- Branch: `setup/os-project`
- Commit: `setup: Create Versatus.OS project`

**Tarefa 1.2 — Criar estrutura de pastas base**
- Crie as pastas: `Domain/`, `Application/`, `Infrastructure/`, `Api/`
- Subdivida `Domain/` em: `Documento/`, `Servico/`, `Item/`, `Situacao/`, `Relacionamentos/`
- Branch: `setup/os-structure`
- Commit: `setup: Create Gestao.OS folder structure`

### Fase 2: Implementar documento OS e itens

**Tarefa 2.1 — Implementar entidades principais**
- Crie `Domain/Documento/DocumentoOS.cs`, `DocumentoOSItem.cs`, `DocumentoOSServico.cs`
- Defina o modelo de OS com cabeçalho, itens, serviços e condições de atendimento
- Branch: `feat/os-documento`
- Commit: `feat: Implement OS document entities`

**Tarefa 2.2 — Implementar acessórios e técnico**
- Crie `Domain/Relacioamentos/DocumentoOSAcessorio.cs`, `DocumentoOSComissionado.cs`, `DocumentoOSLacre.cs`
- Implemente `Domain/Tecnico.cs` e vincule o técnico à OS
- Branch: `feat/os-relacionamentos`
- Commit: `feat: Implement OS accessory, commission and seal entities`

**Tarefa 2.3 — Implementar situação da OS**
- Crie `Domain/Situacao/DocumentoOSSituacao.cs`
- Defina estados de OS: aberto, em execução, concluído, cancelado
- Garanta transições válidas por meio de serviços de domínio
- Branch: `feat/os-situacao`
- Commit: `feat: Implement OS status entity`

### Fase 3: Financeiro e parcelamento

**Tarefa 3.1 — Implementar parcelas financeiras da OS**
- Crie `Domain/Documento/DocumentoOSParcela.cs`
- Defina regras de vencimento, cobrança e faturamento associado à OS
- Branch: `feat/os-financeiro`
- Commit: `feat: Implement OS financial installment entity`

**Tarefa 3.2 — Integrar OS com faturamento**
- Crie serviço de aplicação que, ao concluir a OS, produza um documento de faturamento ou nota fiscal de serviços
- Verifique regras de comissão e valor total
- Branch: `feat/os-faturamento-integration`
- Commit: `feat: Integrate OS with billing`

### Fase 4: Processos de execução e cancelamento

**Tarefa 4.1 — Implementar execução da OS**
- Crie método de domínio para iniciar a execução, registrar serviços e peças utilizadas
- Atualize estados e controle de horas/trabalhos
- Branch: `feat/os-execution`
- Commit: `feat: Implement OS execution flow`

**Tarefa 4.2 — Implementar cancelamento e reversão**
- Crie mecanismo de cancelamento que preserve histórico e evite duplicidade de faturamento
- Garanta que cancelamentos limitem comissões e parcelas financeiras já geradas
- Branch: `feat/os-cancelamento`
- Commit: `feat: Implement OS cancellation`

### Fase 5: Infraestrutura, serviço de aplicação e testes

**Tarefa 5.1 — Configurar DbContext e repositórios**
- Crie `Infrastructure/OSDbContext.cs` e configure `DbSet` para todas as entidades de OS
- Implemente repositórios ou mapeamentos de ORM conforme padrão do projeto
- Branch: `feat/os-dbcontext`
- Commit: `feat: Configure OS DbContext and repositories`

**Tarefa 5.2 — Implementar Application Service de OS**
- Crie `Application/OSService.cs` com métodos para criar OS, adicionar itens/serviços, mudar situação, concluir e cancelar
- Use `Result<T>` e padrões de validação para separar domínio de aplicação
- Branch: `feat/os-service`
- Commit: `feat: Implement OS application service`

**Tarefa 5.3 — Testes de unidade e integração**
- Crie testes para criação, execução, conclusão, cancelamento e integração com faturamento
- Valide cenários de comissão, peças e serviços em diferentes estados
- Branch: `test/os-scenarios`
- Commit: `test: Add OS domain and billing integration tests`

### Conclusão
- Valide o fluxo completo de uma OS desde abertura até conclusão ou cancelamento, incluindo geração de faturamento.
- Branch final de entrega: `release/mod-11-gestao-os`
- Commit final: `chore: Complete MOD-11 Gestão de Ordem de Serviço roadmap`

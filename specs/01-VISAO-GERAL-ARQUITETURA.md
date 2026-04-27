# SPEC — Visão Geral da Arquitetura
## Documento: 01-VISAO-GERAL-ARQUITETURA

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Status:** Referência permanente — não alterar sem consenso da equipe

---

## 1. Arquitetura Legada (projeto_tag_1906)

### 1.1 Tecnologias em uso

| Componente | Tecnologia Legada | Problema na Migração |
|---|---|---|
| Runtime | .NET Framework 4.x | Incompatível com Linux/containers |
| ORM | **Gentle.NET** (framework próprio) | Descontinuado, sem suporte |
| Comunicação | **.NET Remoting** (MarshalByRefObject) | Removido no .NET Core |
| Banco de dados | SQL Server | Mantido |
| Transações | `Gentle.Framework.Transaction` | Depende do Gentle.NET |
| Serialização | BinaryFormatter (via Remoting) | Removido no .NET 5+ |

### 1.2 Camadas da solução legada

```
┌─────────────────────────────────────────────────────────────┐
│  CLIENTE (WinForms)                                          │
│  → servidor.interface (contratos COM .NET Remoting)          │
└──────────────────────────┬──────────────────────────────────┘
                           │  .NET Remoting (TCP/HTTP)
┌──────────────────────────▼──────────────────────────────────┐
│  SERVIDOR                                                    │
│  ├── servidor.framework    (ObjectBase, ListBase, Transacao) │
│  ├── objeto de negócio/    (regras de negócio por módulo)    │
│  │   ├── acesso.global     (entidades, usuários, config)     │
│  │   ├── faturamento       (venda, NF-e)                     │
│  │   ├── gestao.financeira (contas a pagar/receber)          │
│  │   ├── gestao.material   (produtos, estoque)               │
│  │   ├── gestao.compra     (cotações, recebimento)           │
│  │   ├── gestao.tributo    (ICMS, PIS, COFINS, SPED)         │
│  │   ├── NFe               (emissão NF-e)                    │
│  │   ├── Gestao.RH         (folha, funcionários)             │
│  │   ├── Gestao.Contrato   (contratos)                       │
│  │   ├── Gestao.OS         (ordem de serviço)                │
│  │   ├── Gestao.Producao   (ordens de produção)              │
│  │   └── [outros módulos]                                    │
│  └── Gentle.NET           (ORM + SQL gerado)                 │
└──────────────────────────┬──────────────────────────────────┘
                           │  ADO.NET / Gentle.NET
┌──────────────────────────▼──────────────────────────────────┐
│  SQL Server                                                  │
└─────────────────────────────────────────────────────────────┘
```

### 1.3 Padrões chave identificados no código legado

#### 1.3.1 Classe base `ObjectBase`
- Herda de `MarshalByRefObject` (necessário para .NET Remoting)
- Implementa `IObjectBase`  
- Gerencia `IAmbiente` (contexto de execução: usuário, filial, transação)
- Todo objeto de negócio herda desta classe

#### 1.3.2 Classe base `ListBase` ❌ ELIMINADA NO NOVO SISTEMA
- Herda de `ObjectBase`
- Implementa `IList`, `ICollection`, `IEnumerable` via `ArrayList` (não genérico!)
- Existia exclusivamente para compatibilidade com .NET Remoting
- Cada entidade tinha uma lista correspondente (ex: `Cliente` → `ClienteLista`)

> **Decisão (DEC-005):** As classes `*Lista` **não são migradas**. No novo sistema
> usar `IReadOnlyList<T>` para expor coleções e `List<T>` privado dentro de agregados.
> Em .NET Core, as coleções devem ser genéricas: `List<T>`, `IList<T>`, `IReadOnlyList<T>`,
> `IEnumerable<T>`. Não usar `IList` não genérico, `ArrayList` ou `ListBase`.
> Ver `decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md`.

#### 1.3.3 Atributos ORM do Gentle.NET
```csharp
// Exemplo real do código:
[TableName("VenDocumento"), AutoSequencial("IdDistribuicao", SequencialTipo.Filial)]
public class DocumentoVenda : Distribuicao, IDocumentoVenda { ... }
```
- `[TableName]` → nome da tabela no banco
- `[AutoSequencial]` → gerador de ID customizado (não é IDENTITY do SQL)
- `[TableColumn]` → mapeamento de colunas (não visto mas esperado)

#### 1.3.4 Padrão de `IAmbiente`
O **Ambiente** é a unidade de contexto que viaja por todas as camadas:
```
IAmbiente contém:
  - IFilial    → filial corrente
  - IUsuario   → usuário logado  
  - ITransacao → transação de banco ativa (pode ser null)
  - IEmpresa   → empresa corrente
```
Absolutamente **todos os objetos de negócio** recebem o Ambiente no construtor ou via propriedade.

#### 1.3.5 Padrão de Transação
```csharp
// Legado — via Gentle.NET
public class Transacao : TransacaoBase {
    private TransactionEx trans; // Gentle.Framework.Transaction
    // ...
}
```

#### 1.3.6 Padrão de Situação
- Cada documento complexo possui uma classe `[Nome]Situacao`
- Ex: `DocumentoVendaSituacao`, `RecebimentoSituacao`
- Responsável por transições de estado do documento (confirmar, cancelar, etc.)

---

## 2. Arquitetura Alvo (.NET Core 8)

### 2.1 Stack tecnológico alvo

| Componente | Tecnologia Legada | Tecnologia Alvo |
|---|---|---|
| Runtime | .NET Framework 4.x | **.NET 8** |
| ORM | Gentle.NET | **Entity Framework Core 8** |
| Comunicação | .NET Remoting | **API REST / gRPC** |
| Autenticação | Proprietário | **JWT Bearer** |
| Banco de dados | SQL Server | **SQL Server** (mantido) |
| Transações | Gentle Transaction | **IDbContextTransaction (EF Core)** |
| Serialização | BinaryFormatter | **System.Text.Json** |

### 2.2 Arquitetura alvo (Camadas)

```
┌─────────────────────────────────────────────────────────────┐
│  CLIENTE (Web/Mobile/WinForms novo)                          │
└──────────────────────────┬──────────────────────────────────┘
                           │  HTTP/REST ou gRPC
┌──────────────────────────▼──────────────────────────────────┐
│  API Layer (.NET 8 — Minimal API ou Controllers)            │
│  → Autenticação JWT                                          │
│  → Validação de entrada                                      │
│  → Mapeamento Request → Command/Query                        │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  Application Layer (Use Cases / Handlers)                    │
│  → Um Handler por operação de negócio                        │
│  → Equivalente às classes *Situacao* do legado               │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  Domain Layer (Entidades + Regras de Negócio)                │
│  → Equivalente aos objetos de negócio legados                │
│  → Sem dependência de infraestrutura                         │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  Infrastructure Layer (EF Core + Repositórios)              │
│  → DbContext por módulo                                      │
│  → Repositórios implementam interfaces do Domain             │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  SQL Server (mesmo banco — migração de esquema gradual)      │
└─────────────────────────────────────────────────────────────┘
```

### 2.3 O que NÃO muda durante o estrangulamento
- **Banco de dados SQL Server** — mesmo esquema de tables, sem renomear
- **Regras de negócio** — migradas fielmente, sem "melhorias" não solicitadas
- **Sequencial de IDs** — o gerador customizado deve ser replicado exatamente
- **Tratamento de validação** — validações esperadas devem retornar `Result<T>` / `ValidationResult`; não usar exceções como controle de fluxo para regras de negócio comuns

---

## 3. Estratégia de Estrangulamento (Strangler Fig)

### 3.1 Princípio

```
LEGADO ainda roda em produção
         │
         ▼
   [Proxy / Facade] ← ponto de interceptação
         │               ↓ redireciona rotas migradas
         ├──────────────►[NOVO — módulo já migrado]
         │
         └──────────────►[LEGADO — módulo ainda não migrado]
```

### 3.2 Regras do estrangulamento

1. **O sistema legado NUNCA é desligado** durante a migração de um módulo
2. **Feature Flags** controlam quais chamadas vão para o novo sistema
3. **Um módulo é migrado por completo** antes de passar para o próximo
4. **Testes de paridade** validam que novo e legado produzem o mesmo resultado
5. **Rollback** deve ser possível reativando a feature flag do módulo

### 3.3 Ponto de corte — O que é um "módulo completo"?

Um módulo está **completo** para fins de estrangulamento quando:
- [ ] Todas as entidades do módulo têm equivalente no novo sistema
- [ ] Todas as operações de CRUD funcionam via nova API
- [ ] Todas as operações de negócio (Situacao) estão implementadas
- [ ] Testes de integração cobrem os principais fluxos
- [ ] Validação de paridade foi feita comparando legado vs. novo

---

## 4. Mapeamento de Tecnologias Críticas

### 4.1 MarshalByRefObject → eliminar

**Legado:** Todo objeto de negócio herda de `MarshalByRefObject` para trafegar via .NET Remoting.  
**Novo:** Não existe. Os dados trafegam como DTOs via HTTP. As entidades de domínio ficam no servidor.

**Regra:** Ao migrar uma classe, remover a herança de `MarshalByRefObject` e de `ObjectBase`.
Criar um DTO correspondente para cada entidade que precisar trafegar na rede.

### 4.2 Gentle.NET → Entity Framework Core

| Gentle.NET | EF Core Equivalente |
|---|---|
| `[TableName("tabela")]` | `[Table("tabela")]` ou `modelBuilder.Entity<T>().ToTable("tabela")` |
| `Retrieve<T>(id)` | `dbContext.Set<T>().FindAsync(id)` |
| `Persist()` | `dbContext.SaveChangesAsync()` |
| `Remove()` | `dbContext.Remove(entity)` |
| `Broker.RetrieveList<T>(key)` | `dbContext.Set<T>().Where(...)` |
| `Transaction` | `dbContext.Database.BeginTransactionAsync()` |

### 4.3 IAmbiente → IContexto (padrão novo)

O conceito de `IAmbiente` deve ser preservado mas adaptado:

| Legado (IAmbiente) | Novo (IContexto via DI) |
|---|---|
| Passado manualmente objeto a objeto | Injetado via `IHttpContextAccessor` + claims |
| Contém referência a ITransacao | EF Core gerencia transação no DbContext |
| Contém IFilial, IUsuario, IEmpresa | Extraído do JWT Token / claims |

### 4.4 Sequencial de IDs → Replicar comportamento

O legado usa um **gerador de sequencial customizado** (`GeradorSequencial.cs`) que cria IDs
por filial — NÃO usa IDENTITY do SQL Server.  
**IMPORTANTE:** Esse padrão deve ser replicado fielmente no novo sistema para manter
compatibilidade com dados existentes.

---

## 5. Convenções de Nomenclatura

| Padrão Legado | Descrição | Manter? | Substituto |
|---|---|---|---|
| `[Entidade]Lista` | Coleção da entidade | ❌ **ELIMINADO** | `IReadOnlyList<Entidade>` |
| `[Entidade]Situacao` | Operações de estado | ✅ Transformar | Handler / UseCase |
| `I[Entidade]` (interfaces Remoting) | Contrato do objeto | ❌ **ELIMINADO** | DTO de Request/Response |
| `Gestao.[Modulo]` | Namespace de módulo | ✅ Manter | Como prefixo de projeto |
| `Tipo[Documento]` | Tipo/configuração do documento | ✅ Manter | Entidade de configuração |
| `ObjectBase` / `ListBase` | Classes base do framework | ❌ **ELIMINADO** | Sem herança de infra no domínio |

> 📌 **Regra:** Classes `*Lista` do legado **nunca** têm equivalente no novo sistema.
> Ver `decisoes/DEC-005-COLECOES-E-BOAS-PRATICAS.md`.

---

## 6. Referências

- Arquivo base do framework: `servidor/framework/servidor.framework/ObjetoNegocio.cs`
- Ambiente: `servidor/framework/servidor.framework/AmbienteServidorGlobal.cs`
- Transação: `servidor/framework/servidor.framework/Transacao.cs`
- Cliente-Servidor: `servidor/framework/servidor.framework/ClienteServidor.cs`
- Exemplo de entidade principal: `servidor/objeto de negócio/faturamento/DocumentoVenda.cs`

---

*Documento gerado com base em análise de `projeto_tag_1906` — 2026-04-27*

# Guia de Arquitetura — Conexões de Banco de Dados (Leitura vs. Escrita) & DbContexts

Este documento detalha a estratégia de segregação de conexões no banco de dados (CQRS Leve) e comprova o estado de prontidão dos `DbContexts` do projeto Versatus.Net8.

---

## 1. Conexão Única vs. Separação Leitura / Escrita

### Cenário Atual (Conexão Única)
Atualmente o projeto utiliza uma única *Connection String* para leitura e escrita em cada módulo.
- **Vantagens:** Simplicidade de configuração e consistência ACID imediata.
- **Limitação:** Em cenários de alta carga, relatórios pesados ou paginações em massa competem pelos mesmos recursos do banco que processa cadastros e vendas.

### Cenário Futuro (Separação Leitura / Escrita — CQRS Leve)
Segregação da conexão em duas:
1. **Conexão de Escrita (Master / Primary):** Processa operações de gravação (`POST`, `PUT`, `DELETE`).
2. **Conexão de Leitura (Read Replica):** Processa consultas e relatórios (`GET`), apontando para uma réplica do banco com `QueryTrackingBehavior.NoTracking`.

#### Comparativo Técnico:

| Critério | Conexão Única (Atual) | Separação Leitura / Escrita |
|---|---|---|
| **Complexidade de Código** | Mínima (1 DbContext) | Baixa (2 DbContexts / DI) |
| **Alívio de Carga no Banco** | Nenhum | Alto (consultas vão para réplica) |
| **Consistência de Dados** | Imediata | Eventual (delay de milissegundos) |
| **Uso de AsNoTracking()** | Manual em cada query | Nativo na conexão de leitura |
| **Recomendado para ERP?** | Fase de migração | Produção em alta escala |

---

## 2. Status de Prontidão dos DbContexts Atuais

Mapeamos todos os `DbContexts` existentes no repositório:

1. **`AcessoGlobalDbContext`** (`src/Versatus.AcessoGlobal/Infrastructure/AcessoGlobalDbContext.cs`)
2. **`TributoDbContext`** (`src/Versatus.GestaoTributo/Infrastructure/TributoDbContext.cs`)
3. **`VersatusDbContext`** (`src/Versatus.Infra.Data/Context/VersatusDbContext.cs`)

### ✅ Todos os DbContexts JÁ ESTÃO 100% PREPARADOS para a separação.

#### Motivos de Prontidão Arquitetural:
- **Injeção via `DbContextOptions<TContext>`:** Todos os contextos recebem opções via construtor padrão do EF Core.
- **Mapeamentos 100% via Fluent API:** Nenhuma entidade do domínio possui anotações de banco (`[Table]`, `[Column]`).
- **Entidades POCO Puras:** O domínio está totalmente desacoplado da infraestrutura de banco.

---

## 3. Como Ativar Réplicas de Leitura sem Alterar o Código de Negócio

Para ativar a separação no futuro, **NENHUMA classe de serviço ou entidade precisará ser alterada**. A alteração é restrita à camada de Injeção de Dependências (DI):

```csharp
// Exemplo de configuração no Program.cs / DependencyInjection:

// 1. Contexto de ESCRITA (Banco Master)
builder.Services.AddDbContext<AcessoGlobalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionEscrita")));

// 2. Contexto de LEITURA (Réplica de Leitura sem tracking de memória)
builder.Services.AddDbContext<AcessoGlobalReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionLeitura"))
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
```

---

## 4. Diretrizes de Injeção de Dependência nos Serviços

- **Serviços de Alteração de Estado (Commands):** Injetam a interface do serviço responsável por escrita.
- **Serviços de Relatórios e Consultas Paginadas (Queries):** Injetam o contexto configurado com `NoTracking`.

---

## 5. Resumo e Recomendação

1. **Fase Atual (Migração):** Manter a conexão única atual. O código já está padronizado e limpo.
2. **Fase Futura (Produção em Carga):** Ativar a réplica de leitura configurando a *ConnectionString* de leitura no DI, aproveitando a arquitetura limpa já construída no .NET 10.

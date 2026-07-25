# Guia de Arquitetura — Conexões de Banco de Dados (Leitura vs. Escrita) & DbContexts

Este documento detalha a arquitetura de segregação de conexões no banco de dados (CQRS) implementada no projeto **Versatus.Net8** na branch `feat/cqrs-db-read-write-split`.

---

## 1. Visão Geral da Arquitetura Implementada

A aplicação opera com duas *Connection Strings* distintas e contextos segregados por responsabilidade:

1. **Conexão de Escrita (`WriteConnection`):** Utilizada pela instância master (`AcessoGlobalDbContext` / `TributoDbContext`) para gravações e mutações (`POST`, `PUT`, `DELETE`).
2. **Conexão de Leitura (`ReadConnection`):** Utilizada pela instância de réplica de leitura (`AcessoGlobalReadDbContext` / `TributoReadDbContext`) para consultas e relatórios (`GET`), configurada nativamente com `QueryTrackingBehavior.NoTracking`.

---

## 2. Configuração no `appsettings.json`

O arquivo `src/Versatus.WebAPI/appsettings.json` declara explicitamente as duas conexões:

```json
{
  "ConnectionStrings": {
    "WriteConnection": "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;",
    "ReadConnection": "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;"
  }
}
```

> **Nota:** Em ambientes de desenvolvimento/local, ambas apontam para a mesma instância. Em homologação/produção, `ReadConnection` aponta para o endereço IP / DNS da Réplica de Leitura.

---

## 3. Registro dos DbContexts no `Program.cs`

No arquivo `src/Versatus.WebAPI/Program.cs`, os contextos são registrados no *Dependency Injection* do ASP.NET Core:

```csharp
// Connection Strings (CQRS: Write vs Read Split)
var writeConnectionString = builder.Configuration.GetConnectionString("WriteConnection")
    ?? "Server=localhost\\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;";

var readConnectionString = builder.Configuration.GetConnectionString("ReadConnection")
    ?? writeConnectionString;

// DbContexts para ESCRITA (Master DB)
builder.Services.AddDbContext<AcessoGlobalDbContext>(options =>
    options.UseSqlServer(writeConnectionString).EnableSensitiveDataLogging());

builder.Services.AddDbContext<TributoDbContext>(options =>
    options.UseSqlServer(writeConnectionString));

// DbContexts para LEITURA (Read Replica DB com NoTracking)
builder.Services.AddDbContext<AcessoGlobalReadDbContext>(options =>
    options.UseSqlServer(readConnectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddDbContext<TributoReadDbContext>(options =>
    options.UseSqlServer(readConnectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
```

---

## 4. Repositórios com Suporte CQRS NATIVO

Na camada de infraestrutura (`AcessoGlobalRepositorioBase<TEntity>`), o redirecionamento de chamadas é automático:

- **Operações de Leitura (`GetByIdAsync`, `GetAllAsync`, `ListarPaginadoAsync`, buscas):** Utilizam `ReadContext` / `ReadDbSet` (sem tracking de memória, alta performance, réplica de leitura).
- **Operações de Escrita (`AddAsync`, `UpdateAsync`, `DeleteAsync`, `SaveChangesAsync`):** Utilizam `Context` / `DbSet` (rastreados, apontam para a instância principal de gravação).

---

## 5. Matriz de Componentes Ativos

| Componente | Tipo de Operação | Contexto Injetado | Connection String Utilizada |
|---|---|---|---|
| `EntidadeRepository` | Consultas / Grids / Paginação | `AcessoGlobalReadDbContext` | `ReadConnection` |
| `EntidadeRepository` | Inserção / Edição / Exclusão | `AcessoGlobalDbContext` | `WriteConnection` |
| `LocalizacaoRepository` | Lookups (País, Estado, Cidade) | `AcessoGlobalReadDbContext` | `ReadConnection` |
| `OrganizacaoRepository` | Grupos, Empresas, Filiais | `AcessoGlobalReadDbContext` | `ReadConnection` |
| `UsuarioRepository` | Busca de usuário / Login | `AcessoGlobalReadDbContext` | `ReadConnection` |

---

## 6. Resumo dos Benefícios Garantidos

1. **Alívio de Carga no Banco Principal:** Todas as buscas de telas e Grids paginadas leem da réplica sem travar locks nas transações de gravação.
2. **Economia de RAM do Servidor:** `QueryTrackingBehavior.NoTracking` pré-configurado no contexto de leitura evita o consumo de memória do Change Tracker do EF Core em consultas de listagem.
3. **Transparência para a Aplicação:** Os controllers e serviços de domínio mantêm suas interfaces intactas sem precisar saber qual conexão está sendo utilizada.

---
name: create-api-module
description: Use ao criar um novo módulo de negócio no backend C# (.NET 10) para estruturar pastas, DbContexts (Write/Read) e injeção de dependência.
---

# Skill: create-api-module

Esta skill guia o passo a passo para criar um novo módulo de negócio no backend C# do projeto Versatus.Net8 (ex: `Versatus.Estoque`, `Versatus.Financeiro`, `Versatus.Vendas`).

---

## 1. Entrada Esperada

- **Nome do Módulo:** Ex: `Estoque`, `Financeiro`, `Vendas`.
- **Primeira Entidade:** Ex: `Produto`, `ContaPagar`, `PedidoVenda`.

---

## 2. Estrutura de Pastas do Novo Módulo

Crie o novo projeto ou subpasta em `src/Versatus.[NomeDoModulo]/`:

```
src/Versatus.[NomeDoModulo]/
├── Api/
│   └── Controllers/
│       └── [Entidade]Controller.cs
├── Application/
├── DependencyInjection/
│   └── ServiceCollectionExtensions.cs
├── Domain/
│   ├── DTOs/
│   │   └── [Entidade]Dtos.cs
│   ├── Entities/
│   │   └── [Entidade].cs (POCO Pura)
│   ├── Repositories/
│   │   └── I[Entidade]Repository.cs
│   └── Services/
│       ├── I[Entidade]Service.cs
│       └── [Entidade]Service.cs
└── Infrastructure/
    ├── Mappings/
    │   └── Glo[Entidade]Mapping.cs (Fluent API)
    ├── Repositories/
    │   └── [Entidade]Repository.cs
    ├── [Modulo]DbContext.cs
    └── [Modulo]ReadDbContext.cs
```

---

## 3. Passo a Passo de Implementação

### Passo 1 — Entidade POCO Pura (`Domain/Entities/`)
- Declarar apenas propriedades simples e navegáveis.
- **Proibido:** usar `[Table]`, `[Column]`, `[Key]`, `[Required]`.
- File-scoped namespace: `namespace Versatus.[Modulo].Domain.Entities;`

### Passo 2 — Mapeamento Fluent API (`Infrastructure/Mappings/`)
- Herdar `IEntityTypeConfiguration<[Entidade]>`.
- Configurar nome da tabela legada (`ToTable("Glo[Entidade]")`), chave primária e colunas (`HasColumnName`).
- Anulabilidade: colunas que aceitam `NULL` no banco não devem chamar `.IsRequired()`.

### Passo 3 — DbContexts Write e Read (`Infrastructure/`)
- **DbContext Principal (Write):**
  ```csharp
  public class [Modulo]DbContext : DbContext
  {
      public [Modulo]DbContext(DbContextOptions<[Modulo]DbContext> options) : base(options) { }
      protected [Modulo]DbContext(DbContextOptions options) : base(options) { }
      protected override void OnModelCreating(ModelBuilder modelBuilder) =>
          modelBuilder.ApplyConfigurationsFromAssembly(typeof([Modulo]DbContext).Assembly);
  }
  ```
- **DbContext Leitura (Read Replica):**
  ```csharp
  public class [Modulo]ReadDbContext : [Modulo]DbContext
  {
      public [Modulo]ReadDbContext(DbContextOptions<[Modulo]ReadDbContext> options) : base(options) { }
  }
  ```

### Passo 4 — Repositório CQRS (`Infrastructure/Repositories/`)
- Mutações (`AddAsync`, `UpdateAsync`, `DeleteAsync`) chamam `Context` (`WriteConnection`).
- Consultas (`GetByIdAsync`, `GetAllAsync`, paginações) chamam `ReadContext` (`ReadConnection` com `NoTracking`).

### Passo 5 — Serviço de Domínio & Result Pattern (`Domain/Services/`)
- Interface desacoplada `I[Entidade]Service`.
- Implementação retornando `Result<T>` funcional para validações.
- Paginação SQL Server 2008: `await query.ToListAsync()` primeiro e `.Skip(offset).Take(limit)` em memória.

### Passo 6 — Injeção de Dependência (`DependencyInjection/ServiceCollectionExtensions.cs`)
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection Add[Modulo](this IServiceCollection services)
    {
        services.AddScoped<I[Entidade]Repository, [Entidade]Repository>();
        services.AddScoped<I[Entidade]Service, [Entidade]Service>();
        return services;
    }
}
```

### Passo 7 — Registro na WebAPI (`Program.cs`)
1. Registrar os dois DbContexts (Write e Read) no `src/Versatus.WebAPI/Program.cs`.
2. Registrar `builder.Services.Add[Modulo]();`.
3. Adicionar o assembly do módulo como Application Part:
   ```csharp
   .AddApplicationPart(typeof(Versatus.[Modulo].Api.Controllers.[Entidade]Controller).Assembly)
   ```

---

## 4. Validação do Módulo Criado

1. Matar processos `dotnet run` ativos com `manage_task kill`.
2. Executar `dotnet build` na raiz da solução e garantir **0 erros**.

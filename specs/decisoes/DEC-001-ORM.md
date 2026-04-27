# Decisão Técnica — DEC-001: Substituição do ORM Gentle.NET
## Documento: decisoes/DEC-001-ORM.md

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Status:** ✅ Decisão tomada

---

## Problema

O sistema legado usa **Gentle.NET** como ORM. Gentle.NET:
- É um projeto abandonado (sem atualizações desde ~2010)
- Não existe versão compatível com .NET Core/5+
- Usa `System.Runtime.Remoting` internamente
- Sua API é baseada em reflexão com atributos próprios (`[TableName]`, `[TableColumn]`)

## Opções Avaliadas

| Opção | Prós | Contras |
|---|---|---|
| **Entity Framework Core 8** | Maduro, suportado pela Microsoft, LINQ nativo, migrations | Curva de aprendizado |
| Dapper | Simples, SQL puro, performático | Sem migrations, mais trabalho manual |
| NHibernate | Feature-rich, padrão enterprise | Complexo, menor adoção atual |
| ADO.NET puro | Máximo controle | Muito trabalho, sem produtividade |

## Decisão: Entity Framework Core 8

**Justificativa:**
1. Suporte oficial Microsoft de longo prazo
2. Migrations integradas para evolução do schema
3. Melhor integração com ASP.NET Core e DI
4. Ampla documentação e comunidade

## Regras de Migração (Gentle → EF Core)

| Conceito Gentle | Equivalente EF Core |
|---|---|
| `[TableName("tabela")]` | `[Table("tabela")]` ou `modelBuilder.Entity<T>().ToTable("tabela")` |
| `[TableColumn("coluna")]` | `[Column("coluna")]` |
| `Broker.Retrieve<T>(id)` | `dbContext.Set<T>().FindAsync(id)` |
| `obj.Persist()` | `dbContext.Update(obj); await dbContext.SaveChangesAsync()` |
| `obj.Remove()` | `dbContext.Remove(obj); await dbContext.SaveChangesAsync()` |
| `Broker.RetrieveList<T>(key)` | `dbContext.Set<T>().Where(...).ToListAsync()` |
| `new Transaction()` | `await dbContext.Database.BeginTransactionAsync()` |

## Organização dos DbContexts

Cada módulo terá seu próprio `DbContext` para manter isolamento:

```csharp
// Exemplo
public class AcessoGlobalDbContext : DbContext
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    // ...
}
```

## Regra sobre Nomes de Tabelas

> ⚠️ CRÍTICO: O EF Core por padrão pluraliza nomes de tabelas.  
> **SEMPRE** mapear explicitamente para o nome original do banco.

```csharp
// ERRADO — EF Core tentaria "Clientes" (plural)
public DbSet<Cliente> Clientes { get; set; }

// CORRETO — mapeando para o nome original
modelBuilder.Entity<Cliente>().ToTable("EntCliente"); // nome real do banco legado
```

---

*Decisão: 2026-04-27*

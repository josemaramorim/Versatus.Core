# Decisão Técnica — DEC-003: Padrão de Transações
## Documento: decisoes/DEC-003-TRANSACAO.md

> **Versão:** 1.0 | **Data:** 2026-04-27  
> **Status:** ✅ Decisão tomada

---

## Problema

O legado usa `Transacao` (wrapper sobre `Gentle.Framework.Transaction`) para gerenciar
transações de banco de dados. Este objeto:
- Viaja pelo `IAmbiente` de objeto em objeto
- É compartilhado entre múltiplos objetos de negócio dentro de uma operação
- Depende do Gentle.NET (eliminado)
- Usa padrão síncrono (`BeginTransaction`, `Commit`, `Rollback`)

## Como funciona hoje (legado)

```csharp
// Padrão legado simplificado
public void ConfirmarVenda(IAmbiente amb) {
    Transacao trans = new Transacao();
    try {
        this.Persist(amb.ComTransacao(trans));    // persiste o documento
        estoque.Baixar(amb.ComTransacao(trans));  // baixa estoque
        financeiro.GerarParcelas(amb.ComTransacao(trans)); // gera financeiro
        trans.Commit();
    } catch {
        trans.Rollback();
        throw;
    }
}
```

## Decisão: IDbContextTransaction do EF Core

**Padrão adotado:** Uma transação por **Handler/UseCase**, gerenciada pelo `DbContext`.

```csharp
// Padrão novo
public class ConfirmarDocumentoVendaHandler
{
    private readonly FaturamentoDbContext _dbContext;
    private readonly IEstoqueService _estoque;
    private readonly IFinanceiroService _financeiro;

    public async Task<DocumentoVendaResponse> Handle(
        ConfirmarDocumentoVendaCommand command,
        CancellationToken ct)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var documento = await _dbContext.DocumentosVenda
                .FindAsync(command.IdDocumento, ct);

            documento.Confirmar(); // regra de negócio pura

            _estoque.BaixarEstoque(documento, _dbContext); // usa o mesmo dbContext
            _financeiro.GerarParcelas(documento, _dbContext); // usa o mesmo dbContext

            await _dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return DocumentoVendaResponse.From(documento);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
```

## Regras do Padrão de Transações

### REGRA-TRANS-001 — Uma transação por operação de negócio
Cada Handler/UseCase abre e fecha sua própria transação.
Nunca compartilhar transação entre dois Handlers diferentes.

### REGRA-TRANS-002 — DbContext é o portador da transação
O `DbContext` já contém a transação internamente.
Passar o `DbContext` para os serviços de domínio é suficiente — não passar a `IDbTransaction`.

### REGRA-TRANS-003 — SaveChanges apenas no Handler
Apenas o Handler chama `SaveChangesAsync()`.
Entidades de domínio e serviços **nunca** chamam SaveChanges diretamente.

### REGRA-TRANS-004 — Domínio não conhece transação
Classes de domínio puras (`Entidade`, `DocumentoVenda`, etc.) não recebem `DbContext`
nem `IDbTransaction`. Suas regras operam apenas sobre os próprios dados.

### REGRA-TRANS-005 — Operações entre módulos
Quando uma operação envolve múltiplos módulos (ex: Faturamento → Estoque → Financeiro),
todos os DbContexts participantes devem usar a **mesma conexão de banco**,
ou usar `TransactionScope` para envolver múltiplos DbContexts.

**Padrão para operações cross-módulo:**
```csharp
// Opção A: DbContext compartilhado (preferida quando módulos são deployados juntos)
var connection = _dbContext.Database.GetDbConnection();
await connection.OpenAsync(ct);
await using var transaction = await connection.BeginTransactionAsync(ct);

_estoqueDbContext.Database.UseTransaction(transaction);
_financeiroDbContext.Database.UseTransaction(transaction);
```

### REGRA-TRANS-006 — Sem transações distribuídas
Não usar MSDTC ou transações distribuídas. Se necessário escalar para microserviços
no futuro, usar padrão **Saga** / **Outbox**. Por ora, tudo roda no mesmo banco SQL Server.

## Mapeamento de Padrão Legado → Novo

| Legado | Novo |
|---|---|
| `new Transacao()` | `await dbContext.Database.BeginTransactionAsync()` |
| `trans.Commit()` | `await transaction.CommitAsync()` |
| `trans.Rollback()` | `await transaction.RollbackAsync()` |
| `amb.ComTransacao(trans)` | Não necessário — DbContext carrega a transação |
| `ITransacao` no `IAmbiente` | Removido — DbContext gerencia internamente |
| `Transacao` passada entre objetos | Não necessário — `DbContext` injetado por DI |

---

*Decisão: 2026-04-27*

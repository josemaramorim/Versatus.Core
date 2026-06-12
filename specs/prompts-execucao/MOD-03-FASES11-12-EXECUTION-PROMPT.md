# Prompt de Execução — MOD-03 Fases 11–12: Auxiliares e Repositórios Finais

> **Pré-requisito OBRIGATÓRIO:** Fases 1–10 devem estar **completamente implementadas e commitadas**.  
> Verifique que existem:  
> ✅ `Domain/Produtos/Produto.cs` (Fase 4)  
> ✅ `Domain/Estoque/Estoque.cs` (Fase 9)  
> ✅ `Domain/Estoque/MovimentoEstoque.cs` (Fase 10)  

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**  
> Qualquer código novo deve ir em uma branch `feat/` dedicada.

### Antes de começar qualquer código:
```powershell
git checkout develop
git pull origin develop
git checkout -b feat/gestao-material-fases11-12
```

### Durante o trabalho — commite ao final de cada FASE:
```powershell
# Ao concluir Fase 11 (Auxiliares):
git add . && git commit -m "feat(gestao-material): implementa entidades auxiliares — Fase 11"

# Ao concluir Fase 12 (Repos + Migration):
git add . && git commit -m "feat(gestao-material): conclui repositórios, DI e migration inicial — Fase 12"
```

### Ao finalizar:
```powershell
dotnet build
dotnet test --no-build
# NÃO faça merge — deixe a branch aberta para o usuário revisar e aprovar o PR
```

> **Regra:** Nunca use `git merge develop`, `git push origin develop` ou `git rebase` sem permissão do usuário.

---



## Contexto

Projeto: `C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\`  
Módulo a editar: `src\Versatus.GestaoMaterial\`

Este prompt cobre:
- **Fase 11:** Entidades auxiliares (CodigoReferencia, RegraEstoque, EstatisticaEstoque, FornecedorEstoquePreco)
- **Fase 12:** Repositórios principais restantes, migration inicial do EF Core e DI final

---

## FASE 11 — Entidades Auxiliares

### `Domain/Produtos/CodigoReferencia.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>
/// Código de referência alternativo de um produto (código interno, EAN, SKU, código do fornecedor).
/// Mapeada da tabela legada PrcCodigoReferencia.
/// </summary>
public class CodigoReferencia
{
    public int IdCodigoReferencia { get; set; }
    public int IdProduto { get; set; }
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Tipo do código: 1=Referência, 2=EAN, 3=SKU, 4=CódigoFornecedor (confirmar no legado).</summary>
    public int TipoCodigo { get; set; }
    public bool Ativo { get; set; } = true;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

### `Domain/Estoque/RegraEstoque.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Estoque;

/// <summary>
/// Regras de reposição e alertas para um produto em um almoxarifado.
/// Mapeada da tabela legada PrcRegraEstoque.
/// </summary>
public class RegraEstoque
{
    public int IdRegraEstoque { get; set; }
    public int IdProduto { get; set; }
    public int IdAlmoxarifado { get; set; }
    public decimal SaldoMinimo { get; set; }
    public decimal SaldoMaximo { get; set; }
    /// <summary>Quantidade sugerida para reposição.</summary>
    public decimal QuantidadeFixa { get; set; }
    public bool AlertaEstoqueMinimo { get; set; } = true;
    public bool Ativo { get; set; } = true;

    // Navegação
    public Almoxarifado? Almoxarifado { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

### `Domain/Estoque/EstatisticaEstoque.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Estoque;

/// <summary>
/// Estatísticas de movimentação de estoque por produto/período.
/// Mapeada da tabela legada PrcEstatisticaEstoque.
/// </summary>
public class EstatisticaEstoque
{
    public int IdEstatistica { get; set; }
    public int IdProduto { get; set; }
    /// <summary>FK para Filial (referência lógica — cross-module, sem navegação EF).</summary>
    public int IdFilial { get; set; }
    public decimal QuantidadeVendida { get; set; }
    public decimal QuantidadeComprada { get; set; }
    /// <summary>Giro médio mensal calculado.</summary>
    public decimal GiroMedio { get; set; }
    public DateTime DataAtualizacao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

### `Domain/Produtos/FornecedorEstoquePreco.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>
/// Preço de compra de um produto por fornecedor.
/// IdFornecedor é referência lógica ao AcessoGlobal (cross-module, sem navegação EF).
/// Mapeada da tabela legada PrcFornecedorEstoquePreco.
/// </summary>
public class FornecedorEstoquePreco
{
    public int IdFornecedorEstoquePreco { get; set; }
    public int IdProduto { get; set; }
    /// <summary>Referência lógica para GloFornecedor (cross-module — sem navegação EF).</summary>
    public int IdFornecedor { get; set; }
    public decimal PrecoCompra { get; set; }
    public DateTime? DataUltimaCompra { get; set; }
    public bool Principal { get; set; } = false;

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

### Mappings da Fase 11

Crie um mapping por entidade em `Infrastructure/Mappings/`. Use o padrão padrão:
- `CodigoReferenciaMapping.cs` → tabela `PrcCodigoReferencia`
- `RegraEstoqueMapping.cs` → tabela `PrcRegraEstoque`
- `EstatisticaEstoqueMapping.cs` → tabela `PrcEstatisticaEstoque`
- `FornecedorEstoquePrecoMapping.cs` → tabela `PrcFornecedorEstoquePreco`

**Importante para `FornecedorEstoquePreco` e `EstatisticaEstoque`:**
```csharp
// IdFornecedor e IdFilial são cross-module — mapeie apenas a coluna, SEM HasOne/HasForeignKey
builder.Property(f => f.IdFornecedor)
    .HasColumnName("IdGloFornecedor");
// NÃO adicionar: builder.HasOne(...).WithMany(...)...
```

### Atualizar DbContext (Fase 11)

```csharp
// Auxiliares
public DbSet<CodigoReferencia> CodigosReferencia => Set<CodigoReferencia>();
public DbSet<RegraEstoque> RegrasEstoque => Set<RegraEstoque>();
public DbSet<EstatisticaEstoque> EstatisticasEstoque => Set<EstatisticaEstoque>();
public DbSet<FornecedorEstoquePreco> FornecedoresEstoquePrecos => Set<FornecedorEstoquePreco>();
```

---

## FASE 12 — Repositórios Finais, Migration e DI

### Repositórios a Criar

#### `Domain/Repositories/IProdutoRepository.cs`
```csharp
using Versatus.Framework.Repositories;
using Versatus.GestaoMaterial.Domain.Produtos;

namespace Versatus.GestaoMaterial.Domain.Repositories;

public interface IProdutoRepository : IRepositorio<Produto>
{
    Task<Produto?> GetByCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> ListarPorGrupoAsync(int idGrupoEstoque, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> BuscarPorDescricaoAsync(string termo, CancellationToken cancellationToken = default);
}
```

#### `Infrastructure/Repositories/ProdutoRepository.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoMaterial.Domain.Produtos;
using Versatus.GestaoMaterial.Domain.Repositories;

namespace Versatus.GestaoMaterial.Infrastructure.Repositories;

public class ProdutoRepository : GestaoMaterialRepositorioBase<Produto>, IProdutoRepository
{
    public ProdutoRepository(GestaoMaterialDbContext context) : base(context) { }

    public async Task<Produto?> GetByCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default)
        => await Context.Produtos
            .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras, cancellationToken);

    public async Task<IEnumerable<Produto>> ListarPorGrupoAsync(int idGrupoEstoque, CancellationToken cancellationToken = default)
        => await Context.Produtos
            .Where(p => p.IdGrupoEstoque == idGrupoEstoque && p.Ativo)
            .OrderBy(p => p.Descricao)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Produto>> BuscarPorDescricaoAsync(string termo, CancellationToken cancellationToken = default)
        => await Context.Produtos
            .Where(p => p.Descricao.Contains(termo) && p.Ativo)
            .OrderBy(p => p.Descricao)
            .Take(100)
            .ToListAsync(cancellationToken);
}
```

> **Nota:** As propriedades `CodigoBarras`, `Descricao`, `IdGrupoEstoque` e `Ativo` de `Produto` devem existir conforme a análise da Fase 4. Ajuste se os nomes forem diferentes.

#### Repositórios de Estoque e Movimento

```csharp
// Domain/Repositories/IEstoqueRepository.cs
public interface IEstoqueRepository : IRepositorio<Estoque>
{
    Task<Estoque?> GetByProdutoFilialAsync(int idProduto, int idFilial, int idAlmoxarifado, CancellationToken cancellationToken = default);
    Task<IEnumerable<Estoque>> ListarAbaixoMinimoAsync(int idFilial, CancellationToken cancellationToken = default);
}

// Domain/Repositories/IMovimentoEstoqueRepository.cs
public interface IMovimentoEstoqueRepository : IRepositorio<MovimentoEstoque>
{
    Task<IEnumerable<MovimentoEstoque>> ListarPorEstoqueAsync(int idEstoque, DateTime? dataInicio, DateTime? dataFim, CancellationToken cancellationToken = default);
}
```

Crie as implementações concretas em `Infrastructure/Repositories/` seguindo o mesmo padrão.

### DI Final Completo

Atualize `DependencyInjection/ServiceCollectionExtensions.cs` para incluir TODOS os repositórios:

```csharp
public static IServiceCollection AddGestaoMaterial(this IServiceCollection services)
{
    // Produtos e Classificação (Fases 1-3)
    services.AddScoped<IUnidadeRepository, UnidadeRepository>();
    services.AddScoped<IGrupoEstoqueRepository, GrupoEstoqueRepository>();

    // Grades (Fase 5)
    services.AddScoped<IGradeRepository, GradeRepository>();

    // Composição (Fase 6)
    services.AddScoped<IComposicaoRepository, ComposicaoRepository>();

    // Lote e Série (Fase 8)
    services.AddScoped<ILoteRepository, LoteRepository>();
    services.AddScoped<ISerieRepository, SerieRepository>();

    // Produto (Fase 4)
    services.AddScoped<IProdutoRepository, ProdutoRepository>();

    // Estoque e Movimento (Fases 9-10)
    services.AddScoped<IEstoqueRepository, EstoqueRepository>();
    services.AddScoped<IMovimentoEstoqueRepository, MovimentoEstoqueRepository>();

    return services;
}
```

### Migration Inicial

Crie o projeto de testes de integração se ainda não existir.  
Use o projeto Demo como startup para migrations:

```powershell
# Verificar que não há pending model changes
dotnet ef migrations has-pending-model-changes `
    --project src\Versatus.GestaoMaterial\Versatus.GestaoMaterial.csproj `
    --startup-project src\Versatus.AcessoGlobal.Demo\Versatus.AcessoGlobal.Demo\Versatus.AcessoGlobal.Demo.csproj

# Se houver mudanças (esperado — primeira migration do módulo):
dotnet ef migrations add InitialCreate `
    --project src\Versatus.GestaoMaterial\Versatus.GestaoMaterial.csproj `
    --startup-project src\Versatus.AcessoGlobal.Demo\Versatus.AcessoGlobal.Demo\Versatus.AcessoGlobal.Demo.csproj

# Revisar o arquivo de migration gerado antes de continuar
```

> ⚠️ **Após gerar a migration, revise o arquivo `.cs` gerado** para confirmar que não há operações `DROP` inesperadas.

---

## Verificação Final do Módulo Completo

```powershell
dotnet build
dotnet test --no-build --logger "console;verbosity=normal"
```

**Critérios de sucesso:**
- ✅ `Compilação com êxito. 0 Aviso(s) 0 Erro(s)`
- ✅ Todos os testes passando
- ✅ Migration `InitialCreate` do GestaoMaterial criada em `src\Versatus.GestaoMaterial\Migrations\`

**Marque na spec `MOD-03-GESTAO-MATERIAL.md`:**
```markdown
- [x] Fase 11: Auxiliares — Completa
- [x] Fase 12: Repositórios e Migrations — Completa
```

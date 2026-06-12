# Prompt de Execução — MOD-03 Fases 5–8

> **Pré-requisito OBRIGATÓRIO:** A Fase 4 (análise + implementação de `Produto.cs`) deve estar  
> **completamente concluída e commitada** antes de iniciar este documento.  
> Verifique que `Domain/Produtos/Produto.cs` existe no projeto `Versatus.GestaoMaterial`.

---

## ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**  
> Qualquer código novo deve ir em uma branch `feat/` dedicada.

### Antes de começar qualquer código:
```powershell
git checkout develop
git pull origin develop
git checkout -b feat/gestao-material-fases5-8
```

### Durante o trabalho — commite ao final de cada FASE:
```powershell
# Ao concluir Fase 5 (Grades):
git add . && git commit -m "feat(gestao-material): implementa Grades e Variantes — Fase 5"

# Ao concluir Fase 6 (Composição):
git add . && git commit -m "feat(gestao-material): implementa Composição de produtos — Fase 6"

# Ao concluir Fase 7 (Localização):
git add . && git commit -m "feat(gestao-material): implementa LocalizacaoEstoque — Fase 7"

# Ao concluir Fase 8 (Lote/Série):
git add . && git commit -m "feat(gestao-material): implementa Lote e Série — Fase 8"
```

### Ao finalizar todas as tarefas:
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
- **Fase 5:** Grades e Variantes (variações de produto: cor, tamanho, etc.)
- **Fase 6:** Composição (kits e receitas de produção)
- **Fase 7:** Localização e Almoxarifado (WMS básico — localização já criada na Fase 1-3, expandir)
- **Fase 8:** Lote e Série (rastreabilidade)

**Regras invioláveis:**
1. Sem `IDENTITY` / `ValueGeneratedOnAdd()` — sempre `.ValueGeneratedNever()`
2. Sem DataAnnotations nas entidades — só Fluent API nos Mappings
3. Sem navegações EF Core cross-module (IDs para AcessoGlobal ficam como `int`)
4. `bool` → `short` via convenção do DbContext (não fazer nada especial, já está configurado)
5. Pastas em inglês, entidades em português

---

## FASE 5 — Grades e Variantes

### Entidades a Criar

#### `Domain/Grades/Variante.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Grades;

/// <summary>
/// Variante de grade (ex: Cor, Tamanho, Sabor).
/// Mapeada da tabela legada PrcVariante.
/// </summary>
public class Variante
{
    public int IdVariante { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    // Coleções
    public ICollection<SubVariante> SubVariantes { get; set; } = new List<SubVariante>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Grades/SubVariante.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Grades;

/// <summary>
/// Sub-variante de grade (ex: Azul, Vermelho, P, M, G).
/// Mapeada da tabela legada PrcSubVariante.
/// </summary>
public class SubVariante
{
    public int IdSubVariante { get; set; }
    public int IdVariante { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Valor { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public Variante? Variante { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Grades/Grade.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Grades;

/// <summary>
/// Grade de variações de um produto (ex: Grade de Camiseta: Cor × Tamanho).
/// Mapeada da tabela legada PrcGrade.
/// </summary>
public class Grade
{
    public int IdGrade { get; set; }
    /// <summary>FK para Produto (mesmo módulo).</summary>
    public int IdProduto { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;

    // Coleções
    public ICollection<GradeVariacao> Variacoes { get; set; } = new List<GradeVariacao>();
    public ICollection<ItemGrade> Itens { get; set; } = new List<ItemGrade>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Grades/GradeVariacao.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Grades;

/// <summary>
/// Eixo de variação de uma grade (ex: Grade "Camiseta" tem Variante "Cor" e "Tamanho").
/// Mapeada da tabela legada PrcGradeVariacao.
/// </summary>
public class GradeVariacao
{
    public int IdGradeVariacao { get; set; }
    public int IdGrade { get; set; }
    public int IdVariante { get; set; }
    public int Ordem { get; set; }

    // Navegação
    public Grade? Grade { get; set; }
    public Variante? Variante { get; set; }
}
```

#### `Domain/Grades/ItemGrade.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Grades;

/// <summary>
/// Item de grade — combinação específica de sub-variantes de um produto.
/// Ex: Camiseta Azul Tamanho M.
/// Mapeada da tabela legada PrcItemGrade.
/// </summary>
public class ItemGrade
{
    public int IdItemGrade { get; set; }
    public int IdGrade { get; set; }
    public string? CodigoBarras { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public Grade? Grade { get; set; }
    public ICollection<ItemVariacao> Variacoes { get; set; } = new List<ItemVariacao>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Grades/ItemVariacao.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Grades;

/// <summary>
/// Sub-variante específica do item de grade.
/// Ex: ItemGrade "Camiseta Azul M" → ItemVariacao "Cor=Azul" + ItemVariacao "Tamanho=M".
/// Mapeada da tabela legada PrcItemVariacao.
/// </summary>
public class ItemVariacao
{
    public int IdItemVariacao { get; set; }
    public int IdItemGrade { get; set; }
    public int IdSubVariante { get; set; }

    // Navegação
    public ItemGrade? ItemGrade { get; set; }
    public SubVariante? SubVariante { get; set; }
}
```

### Mappings da Fase 5

Crie um arquivo por entidade em `Infrastructure/Mappings/`. Padrão a seguir:

```csharp
// Infrastructure/Mappings/VarianteMapping.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoMaterial.Domain.Grades;

namespace Versatus.GestaoMaterial.Infrastructure.Mappings;

public class VarianteMapping : IEntityTypeConfiguration<Variante>
{
    public void Configure(EntityTypeBuilder<Variante> builder)
    {
        builder.ToTable("PrcVariante"); // TODO: confirmar nome no legado

        builder.HasKey(v => v.IdVariante);
        builder.Property(v => v.IdVariante)
            .HasColumnName("IdPrcVariante")
            .ValueGeneratedNever();

        builder.Property(v => v.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Ativo).HasColumnName("Ativo");

        builder.HasMany(v => v.SubVariantes)
            .WithOne(s => s.Variante)
            .HasForeignKey(s => s.IdVariante)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.IdUsuarioInclusao).HasColumnName("IdPrcUsuarioInclusao");
        builder.Property(u => u.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(u => u.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(u => u.IdUsuarioAlteracao).HasColumnName("IdPrcUsuarioAlteracao");
        builder.Property(u => u.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(u => u.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}
```

Crie mappings análogos para: `SubVariante`, `Grade`, `GradeVariacao`, `ItemGrade`, `ItemVariacao`.

**Nota para `Grade`:** A FK `IdProduto` aponta para `Produto` dentro do mesmo módulo — declare `HasOne`/`HasForeignKey` normalmente.

### Atualizar DbContext (Fase 5)

Adicione ao `GestaoMaterialDbContext.cs`:
```csharp
// Grades
public DbSet<Variante> Variantes => Set<Variante>();
public DbSet<SubVariante> SubVariantes => Set<SubVariante>();
public DbSet<Grade> Grades => Set<Grade>();
public DbSet<GradeVariacao> GradesVariacoes => Set<GradeVariacao>();
public DbSet<ItemGrade> ItensGrade => Set<ItemGrade>();
public DbSet<ItemVariacao> ItensVariacao => Set<ItemVariacao>();
```

---

## FASE 6 — Composição (Kits e Receitas)

### Entidades a Criar

#### `Domain/Composicao/Composicao.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Composicao;

/// <summary>
/// Composição de produto — kit ou receita de fabricação.
/// Mapeada da tabela legada PrcComposicao.
/// </summary>
public class Composicao
{
    public int IdComposicao { get; set; }
    /// <summary>FK para o Produto composto/kit (mesmo módulo).</summary>
    public int IdProduto { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;

    // Coleções
    public ICollection<ComposicaoItem> Itens { get; set; } = new List<ComposicaoItem>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Composicao/ComposicaoItem.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Composicao;

/// <summary>
/// Item de uma composição — componente com quantidade.
/// Mapeada da tabela legada PrcComposicaoItem.
/// </summary>
public class ComposicaoItem
{
    public int IdComposicaoItem { get; set; }
    public int IdComposicao { get; set; }
    /// <summary>FK para o Produto componente (mesmo módulo).</summary>
    public int IdProdutoComponente { get; set; }
    public decimal Quantidade { get; set; }
    /// <summary>FK para Unidade de medida (mesmo módulo).</summary>
    public int IdUnidade { get; set; }

    // Navegação
    public Composicao? Composicao { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

Crie mappings `ComposicaoMapping.cs` e `ComposicaoItemMapping.cs` seguindo o mesmo padrão.

Adicione ao DbContext:
```csharp
// Composição
public DbSet<Composicao> Composicoes => Set<Composicao>();
public DbSet<ComposicaoItem> ComposicaoItens => Set<ComposicaoItem>();
```

---

## FASE 7 — Localização de Estoque (WMS básico)

O `Almoxarifado` já foi criado nas Fases 1-3. Agora expanda com `LocalizacaoEstoque`.

#### `Domain/Estoque/LocalizacaoEstoque.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Estoque;

/// <summary>
/// Localização física no armazém (corredor, prateleira, nível).
/// Mapeada da tabela legada PrcLocalizacao.
/// Nota: não confundir com Versatus.AcessoGlobal.Domain.Location (geográfica).
/// </summary>
public class LocalizacaoEstoque
{
    public int IdLocalizacaoEstoque { get; set; }
    public int IdAlmoxarifado { get; set; }
    /// <summary>Endereço físico: ex "A-01-03" (Corredor A, Prateleira 01, Nível 03).</summary>
    public string Endereco { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativa { get; set; } = true;

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

Crie `Infrastructure/Mappings/LocalizacaoEstoqueMapping.cs`.

Adicione ao DbContext:
```csharp
// Localização
public DbSet<LocalizacaoEstoque> LocalizacoesEstoque => Set<LocalizacaoEstoque>();
```

---

## FASE 8 — Lote e Série

#### `Domain/Lote/Lote.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Lote;

/// <summary>
/// Lote de produto para rastreabilidade.
/// Mapeada da tabela legada PrcLote.
/// </summary>
public class Lote
{
    public int IdLote { get; set; }
    public int IdProduto { get; set; }
    public string NumeroLote { get; set; } = string.Empty;
    public DateTime? DataFabricacao { get; set; }
    public DateTime? DataValidade { get; set; }
    public bool Ativo { get; set; } = true;

    // Coleções
    public ICollection<MovimentoLote> Movimentos { get; set; } = new List<MovimentoLote>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Lote/MovimentoLote.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Lote;

/// <summary>
/// Rastreio de lote em um movimento de estoque.
/// Mapeada da tabela legada PrcMovimentoLote.
/// </summary>
public class MovimentoLote
{
    public int IdMovimentoLote { get; set; }
    public int IdLote { get; set; }
    /// <summary>FK para MovimentoEstoque (será criado na Fase 10).</summary>
    public int IdMovimentoEstoque { get; set; }
    public decimal Quantidade { get; set; }

    // Navegação
    public Lote? Lote { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Serie/Serie.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Serie;

/// <summary>
/// Número de série de produto para rastreabilidade individual.
/// Mapeada da tabela legada PrcSerie.
/// </summary>
public class Serie
{
    public int IdSerie { get; set; }
    public int IdProduto { get; set; }
    public string NumeroSerie { get; set; } = string.Empty;
    public DateTime? DataFabricacao { get; set; }
    public bool Ativo { get; set; } = true;

    // Coleções
    public ICollection<SerieMovimento> Movimentos { get; set; } = new List<SerieMovimento>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Serie/SerieMovimento.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Serie;

/// <summary>Rastreio de série em um movimento de estoque. Mapeada de PrcSerieMovimento.</summary>
public class SerieMovimento
{
    public int IdSerieMovimento { get; set; }
    public int IdSerie { get; set; }
    /// <summary>FK para MovimentoEstoque (será criado na Fase 10).</summary>
    public int IdMovimentoEstoque { get; set; }

    // Navegação
    public Serie? Serie { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

Crie os mappings para `Lote`, `MovimentoLote`, `Serie`, `SerieMovimento`.

Adicione ao DbContext:
```csharp
// Lote
public DbSet<Lote> Lotes => Set<Lote>();
public DbSet<MovimentoLote> MovimentosLote => Set<MovimentoLote>();

// Série
public DbSet<Serie> Series => Set<Serie>();
public DbSet<SerieMovimento> SeriesMovimentos => Set<SerieMovimento>();
```

---

## Repositórios das Fases 5-8

Crie interfaces e implementações para as entidades principais:

```
Domain/Repositories/IGradeRepository.cs  + Infrastructure/Repositories/GradeRepository.cs
Domain/Repositories/IComposicaoRepository.cs + Infrastructure/Repositories/ComposicaoRepository.cs
Domain/Repositories/ILoteRepository.cs + Infrastructure/Repositories/LoteRepository.cs
Domain/Repositories/ISerieRepository.cs + Infrastructure/Repositories/SerieRepository.cs
```

Padrão a seguir (adapte para cada entidade):
```csharp
// Domain/Repositories/IGradeRepository.cs
using Versatus.Framework.Repositories;
using Versatus.GestaoMaterial.Domain.Grades;

namespace Versatus.GestaoMaterial.Domain.Repositories;

public interface IGradeRepository : IRepositorio<Grade>
{
    Task<IEnumerable<Grade>> ListarPorProdutoAsync(int idProduto, CancellationToken cancellationToken = default);
}
```

```csharp
// Infrastructure/Repositories/GradeRepository.cs
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoMaterial.Domain.Grades;
using Versatus.GestaoMaterial.Domain.Repositories;

namespace Versatus.GestaoMaterial.Infrastructure.Repositories;

public class GradeRepository : GestaoMaterialRepositorioBase<Grade>, IGradeRepository
{
    public GradeRepository(GestaoMaterialDbContext context) : base(context) { }

    public async Task<IEnumerable<Grade>> ListarPorProdutoAsync(int idProduto, CancellationToken cancellationToken = default)
        => await Context.Grades
            .Include(g => g.Variacoes)
            .Include(g => g.Itens)
            .Where(g => g.IdProduto == idProduto)
            .ToListAsync(cancellationToken);
}
```

Registre todos no `DependencyInjection/ServiceCollectionExtensions.cs`:
```csharp
services.AddScoped<IGradeRepository, GradeRepository>();
services.AddScoped<IComposicaoRepository, ComposicaoRepository>();
services.AddScoped<ILoteRepository, LoteRepository>();
services.AddScoped<ISerieRepository, SerieRepository>();
```

---

## Verificação Final

Execute na raiz da solução:
```powershell
dotnet build
dotnet test --no-build --logger "console;verbosity=normal"
```

**Critérios de sucesso:**
- ✅ `Compilação com êxito. 0 Aviso(s) 0 Erro(s)`
- ✅ Todos os testes existentes continuam passando
- ✅ `dotnet ef migrations has-pending-model-changes` retorna limpo (use o projeto Demo como startup)

**Marque na spec:**
- `- [x] Fase 5: Grades e Variantes — Completa`
- `- [x] Fase 6: Composição — Completa`
- `- [x] Fase 7: Localização — Completa`
- `- [x] Fase 8: Lote e Série — Completa`

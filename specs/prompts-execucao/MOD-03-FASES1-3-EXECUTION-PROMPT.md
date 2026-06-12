# Prompt de Execução — MOD-03: Gestão de Material (Fases 1–3)

> **Para a IA executora:** Este documento é autocontido. Você receberá tudo que precisa aqui.  
> **Não invente nomes de tabela, propriedades ou padrões** — use exatamente os padrões descritos.  
> **Ao final de cada fase**, execute `dotnet build` e confirme `0 erros, 0 avisos`.

---

## 1. Contexto do Projeto

### Localização no disco
```
Raiz da solução: C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\
```

### Projetos existentes relevantes
```
src\Versatus.Framework\          — base do framework (não editar)
src\Versatus.AcessoGlobal\       — módulo de entidades base (já implementado, referenciá-lo)
src\Versatus.GestaoTributo\      — módulo de tributos (use como modelo de estrutura)
src\Versatus.WebAPI\             — API principal
tests\Versatus.AcessoGlobal.Tests\
tests\Versatus.GestaoTributo.Tests\
```

### O que você vai criar
```
src\Versatus.GestaoMaterial\     — NOVO — criar do zero
tests\Versatus.GestaoMaterial.Tests\  — NOVO — criar do zero
```

---

## 2. ⚠️ Workflow Git — OBRIGATÓRIO

> **NUNCA commite diretamente em `develop` ou `main`.**  
> Qualquer código novo deve ir em uma branch `feat/` dedicada.

### Antes de começar qualquer código:
```powershell
# 1. Certifique-se de estar na develop atualizada
git checkout develop
git pull origin develop

# 2. Crie a branch de feature para este trabalho
git checkout -b feat/gestao-material-fases1-3
```

### Durante o trabalho — commite por tarefa concluída:
```powershell
# Após cada TAREFA do prompt (quando o build estiver passando):
git add .
git commit -m "feat(gestao-material): adiciona estrutura do projeto e entidades base (Fases 1-3)"
```

**Convenção de commits (Conventional Commits):**
| Tipo | Quando usar |
|---|---|
| `feat(modulo):` | Nova entidade, repositório, serviço |
| `fix(modulo):` | Correção de bug ou mapping incorreto |
| `docs(modulo):` | Atualização de spec ou documentação |
| `test(modulo):` | Adição ou correção de testes |
| `refactor(modulo):` | Mudança sem adicionar funcionalidade |

### Ao finalizar todas as tarefas do prompt:
```powershell
# 1. Build e testes passando
dotnet build
dotnet test --no-build

# 2. Commit final se houver algo pendente
git add .
git commit -m "feat(gestao-material): conclui Fases 1-3 — DbContext, DI e testes básicos"

# 3. NÃO faça merge — deixe a branch aberta para o usuário revisar e aprovar o PR
```

> **Regra:** A branch só vai para `develop` depois que o USUÁRIO revisar e aprovar.  
> **Nunca use:** `git merge develop`, `git push origin develop`, `git rebase` sem permissão.

---

## 3. Regras Invioláveis (Anti-Alucinação)

1. **Nunca use `IDENTITY` / `[DatabaseGenerated(DatabaseGeneratedOption.Identity)]`** — IDs são gerados pelo sistema legado via `GeradorSequencialService`. Use sempre `.ValueGeneratedNever()` nos mappings.
2. **`bool` é mapeado para `short`** — nunca use `bit` diretamente. A convenção do DbContext já faz isso automaticamente.
3. **Não crie navegações EF Core cross-module** — `Fabricante.IdEntidade` (referência ao AcessoGlobal) fica como `int` simples, sem `Include` ou `HasOne` apontando para `Versatus.AcessoGlobal`.
4. **Nomes de coluna no legado são diferentes dos nomes C#** — sempre mapear com `.HasColumnName()`.
5. **Um arquivo de mapping por entidade** — padrão `NomeDaEntidadeMapping.cs` em `Infrastructure/Mappings/`.
6. **`ValueGeneratedNever()` em toda PK** — sem exceção.

---

## 3. Padrões de Código — Exemplos Reais do Projeto

### 3.1 Entidade de Domínio (copie este padrão)

```csharp
// src\Versatus.AcessoGlobal\Domain\Location\Bairro.cs — EXEMPLO REAL
namespace Versatus.AcessoGlobal.Domain.Location;

/// <summary>
/// Entidade que representa um Bairro.
/// Mapeada da tabela legada GloBairro.
/// </summary>
public class Bairro
{
    public int IdBairro { get; set; }
    public int IdCidade { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    // Propriedade de Navegação
    public Cidade? Cidade { get; set; }

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

**Regras:**
- Namespace = `Versatus.GestaoMaterial.Domain.{Subpasta}`
- `string` sempre com `= string.Empty`
- `bool` nunca é `bool?` — é simples com `= true` ou `= false`
- Campos de auditoria (`IdUsuarioInclusao`, `DataInclusao`, etc.) são **nullable** (`int?`, `DateTime?`)
- Propriedades de navegação são **nullable** (`Cidade?`)

### 3.2 Mapping Fluent API (copie este padrão)

```csharp
// src\Versatus.AcessoGlobal\Infrastructure\Mappings\BairroMapping.cs — EXEMPLO REAL
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.AcessoGlobal.Domain.Location;

namespace Versatus.AcessoGlobal.Infrastructure.Mappings;

public class BairroMapping : IEntityTypeConfiguration<Bairro>
{
    public void Configure(EntityTypeBuilder<Bairro> builder)
    {
        builder.ToTable("GloBairro");

        builder.HasKey(b => b.IdBairro);

        builder.Property(b => b.IdBairro)
            .HasColumnName("IdGloBairro")
            .ValueGeneratedNever();      // ← OBRIGATÓRIO em toda PK

        builder.Property(b => b.IdCidade)
            .HasColumnName("IdGloCidade")
            .IsRequired();

        builder.Property(b => b.Nome)
            .HasColumnName("Descricao")   // ← nome no banco pode ser diferente
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Ativo).HasColumnName("Ativo");

        // Relacionamentos
        builder.HasOne(b => b.Cidade)
            .WithMany()
            .HasForeignKey(b => b.IdCidade)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoria (padrão fixo — copie sempre igual)
        builder.Property(e => e.IdUsuarioInclusao).HasColumnName("IdGloUsuarioInclusao");
        builder.Property(e => e.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(e => e.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(p => p.IdUsuarioAlteracao).HasColumnName("IdGloUsuarioAlteracao");
        builder.Property(p => p.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(p => p.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}
```

**Atenção:** Para o módulo GestaoMaterial, o prefixo de coluna de auditoria provavelmente é `IdPrcUsuarioInclusao` (prefixo `Prc` ao invés de `Glo`). Use `IdPrcUsuarioInclusao` / `IdPrcUsuarioAlteracao`. Se não souber com certeza, use comentário `// TODO: confirmar nome da coluna no legado`.

### 3.3 DbContext (copie este padrão)

```csharp
// src\Versatus.GestaoTributo\Infrastructure\TributoDbContext.cs — EXEMPLO REAL
using Microsoft.EntityFrameworkCore;
// ... using das entidades

namespace Versatus.GestaoTributo.Infrastructure;

public class TributoDbContext : DbContext
{
    public TributoDbContext(DbContextOptions<TributoDbContext> options)
        : base(options) { }

    public DbSet<ClassificacaoFiscal> ClassificacoesFiscais => Set<ClassificacaoFiscal>();
    // ... outros DbSets

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TributoDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Converte bool → short (smallint no SQL Server legado)
        configurationBuilder.Properties<bool>()
            .HaveConversion<short>();
    }
}
```

### 3.4 DI Extensions (copie este padrão)

```csharp
// src\Versatus.GestaoTributo\DependencyInjection\ServiceCollectionExtensions.cs — EXEMPLO REAL
using Microsoft.Extensions.DependencyInjection;

namespace Versatus.GestaoTributo.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGestaoTributo(this IServiceCollection services)
    {
        services.AddScoped<IClassificacaoFiscalRepository, ClassificacaoFiscalRepository>();
        services.AddScoped<ITributoService, TributoService>();
        return services;
    }
}
```

### 3.5 Base de Repositório (copie este padrão)

```csharp
// src\Versatus.AcessoGlobal\Infrastructure\Repositories\AcessoGlobalRepositorioBase.cs — EXEMPLO REAL
using Microsoft.EntityFrameworkCore;
using Versatus.Framework.Repositories;

namespace Versatus.AcessoGlobal.Infrastructure.Repositories;

public abstract class AcessoGlobalRepositorioBase<TEntity> : IRepositorio<TEntity> where TEntity : class
{
    protected readonly AcessoGlobalDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected AcessoGlobalRepositorioBase(AcessoGlobalDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(new[] { id }, cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.ToListAsync(cancellationToken);

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}
```

### 3.6 .csproj (copie este padrão)

```xml
<!-- src\Versatus.GestaoTributo\Versatus.GestaoTributo.csproj — EXEMPLO REAL -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Versatus.Framework\Versatus.Framework.csproj" />
    <ProjectReference Include="..\Versatus.AcessoGlobal\Versatus.AcessoGlobal.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.3" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.1" />
    <PackageReference Include="Azure.Identity" Version="1.11.4" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="6.34.0" />
    <PackageReference Include="System.Formats.Asn1" Version="6.0.1" />
  </ItemGroup>

</Project>
```

---

## 4. Estrutura de Pastas a Criar

```
src\Versatus.GestaoMaterial\
├── Versatus.GestaoMaterial.csproj
├── DependencyInjection\
│   └── ServiceCollectionExtensions.cs
├── Domain\
│   ├── Produtos\
│   │   ├── Unidade.cs
│   │   ├── Conversor.cs
│   │   ├── GrupoEstoque.cs
│   │   ├── Linha.cs
│   │   ├── Marca.cs
│   │   ├── Fabricante.cs
│   │   └── Modelo.cs
│   ├── Estoque\
│   │   └── Almoxarifado.cs
│   └── Repositories\
│       ├── IUnidadeRepository.cs
│       └── IGrupoEstoqueRepository.cs
├── Infrastructure\
│   ├── GestaoMaterialDbContext.cs
│   ├── Mappings\
│   │   ├── UnidadeMapping.cs
│   │   ├── ConversorMapping.cs
│   │   ├── GrupoEstoqueMapping.cs
│   │   ├── LinhaMapping.cs
│   │   ├── MarcaMapping.cs
│   │   ├── FabricanteMapping.cs
│   │   ├── ModeloMapping.cs
│   │   └── AlmoxarifadoMapping.cs
│   └── Repositories\
│       ├── GestaoMaterialRepositorioBase.cs
│       ├── UnidadeRepository.cs
│       └── GrupoEstoqueRepository.cs

tests\Versatus.GestaoMaterial.Tests\
├── Versatus.GestaoMaterial.Tests.csproj
└── BasicTests.cs
```

---

## 5. Tarefas de Execução (Passo a Passo)

### TAREFA 1 — Criar o arquivo .csproj

**Arquivo:** `src\Versatus.GestaoMaterial\Versatus.GestaoMaterial.csproj`

Use exatamente o padrão da seção 3.6, substituindo o nome do projeto. Não adicione nem remova pacotes.

---

### TAREFA 2 — Adicionar o projeto à solução

Execute no terminal (na raiz da solução):
```powershell
dotnet sln add src\Versatus.GestaoMaterial\Versatus.GestaoMaterial.csproj
```

Verifique que o build ainda passa:
```powershell
dotnet build
```
✅ Esperado: `Compilação com êxito. 0 Aviso(s) 0 Erro(s)`

---

### TAREFA 3 — Criar as Entidades de Domínio

Crie cada arquivo abaixo. Use o padrão da seção 3.1.

#### `Domain/Produtos/Unidade.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>
/// Unidade de medida dos produtos.
/// Mapeada da tabela legada PrcUnidade.
/// </summary>
public class Unidade
{
    public int IdUnidade { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Abreviatura { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    // Metadados de Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Produtos/Conversor.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>
/// Conversor entre unidades de medida.
/// Permite definir fator de conversão (ex: 1 cx = 12 un).
/// Mapeada da tabela legada PrcConversor.
/// </summary>
public class Conversor
{
    public int IdConversor { get; set; }
    public int IdUnidade1 { get; set; }
    public int IdUnidade2 { get; set; }
    /// <summary>Fator de conversão: Unidade1 × Fator = Unidade2</summary>
    public decimal Fator { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public Unidade? Unidade1 { get; set; }
    public Unidade? Unidade2 { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Produtos/GrupoEstoque.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>
/// Grupo/família de produtos com suporte a hierarquia (grupo → subgrupo).
/// Mapeada da tabela legada PrcGrupoEstoque.
/// </summary>
public class GrupoEstoque
{
    public int IdGrupoEstoque { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    /// <summary>FK para o grupo pai — nullable para suportar hierarquia.</summary>
    public int? IdGrupoEstoquePai { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public GrupoEstoque? GrupoPai { get; set; }
    public ICollection<GrupoEstoque> SubGrupos { get; set; } = new List<GrupoEstoque>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Produtos/Linha.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>Linha de produto. Mapeada da tabela legada PrcLinha.</summary>
public class Linha
{
    public int IdLinha { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
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

#### `Domain/Produtos/Marca.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>Marca do produto. Mapeada da tabela legada PrcMarca.</summary>
public class Marca
{
    public int IdMarca { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
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

#### `Domain/Produtos/Fabricante.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>
/// Fabricante do produto.
/// Mapeada da tabela legada PrcFabricante.
/// IdEntidade é referência lógica (int) ao AcessoGlobal — sem navegação EF cross-module.
/// </summary>
public class Fabricante
{
    public int IdFabricante { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    /// <summary>Referência lógica para GloEntidade (sem navegação EF).</summary>
    public int? IdEntidade { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação intra-módulo
    public ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Produtos/Modelo.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Produtos;

/// <summary>Modelo do produto. Mapeada da tabela legada PrcModelo.</summary>
public class Modelo
{
    public int IdModelo { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int IdFabricante { get; set; }
    public bool Ativo { get; set; } = true;

    // Navegação
    public Fabricante? Fabricante { get; set; }

    // Auditoria
    public int? IdUsuarioInclusao { get; set; }
    public DateTime? DataInclusao { get; set; }
    public DateTime? HoraInclusao { get; set; }
    public int? IdUsuarioAlteracao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public DateTime? HoraAlteracao { get; set; }
}
```

#### `Domain/Estoque/Almoxarifado.cs`
```csharp
namespace Versatus.GestaoMaterial.Domain.Estoque;

/// <summary>
/// Almoxarifado/depósito físico vinculado a uma filial.
/// IdFilial é referência lógica (int) ao AcessoGlobal — sem navegação EF cross-module.
/// Mapeada da tabela legada PrcAlmoxarifado.
/// </summary>
public class Almoxarifado
{
    public int IdAlmoxarifado { get; set; }
    /// <summary>Referência lógica para GloFilial (sem navegação EF).</summary>
    public int IdFilial { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
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

---

### TAREFA 4 — Criar os Mappings Fluent API

Crie um arquivo por entidade em `Infrastructure/Mappings/`. Use o padrão da seção 3.2.

> **Atenção sobre nomes de colunas:** Os nomes abaixo são **estimados** com base no prefixo `Prc` do legado. Adicione comentário `// TODO: confirmar no legado` onde indicado.

#### `Infrastructure/Mappings/UnidadeMapping.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoMaterial.Domain.Produtos;

namespace Versatus.GestaoMaterial.Infrastructure.Mappings;

public class UnidadeMapping : IEntityTypeConfiguration<Unidade>
{
    public void Configure(EntityTypeBuilder<Unidade> builder)
    {
        builder.ToTable("PrcUnidade"); // TODO: confirmar nome no legado

        builder.HasKey(u => u.IdUnidade);
        builder.Property(u => u.IdUnidade)
            .HasColumnName("IdPrcUnidade")
            .ValueGeneratedNever();

        builder.Property(u => u.Codigo)
            .HasColumnName("Codigo")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Abreviatura)
            .HasColumnName("Abreviatura")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.Ativo).HasColumnName("Ativo");

        // Auditoria
        builder.Property(u => u.IdUsuarioInclusao).HasColumnName("IdPrcUsuarioInclusao");
        builder.Property(u => u.DataInclusao).HasColumnName("DataInclusao");
        builder.Property(u => u.HoraInclusao).HasColumnName("HoraInclusao");
        builder.Property(u => u.IdUsuarioAlteracao).HasColumnName("IdPrcUsuarioAlteracao");
        builder.Property(u => u.DataAlteracao).HasColumnName("DataAlteracao");
        builder.Property(u => u.HoraAlteracao).HasColumnName("HoraAlteracao");
    }
}
```

#### `Infrastructure/Mappings/ConversorMapping.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoMaterial.Domain.Produtos;

namespace Versatus.GestaoMaterial.Infrastructure.Mappings;

public class ConversorMapping : IEntityTypeConfiguration<Conversor>
{
    public void Configure(EntityTypeBuilder<Conversor> builder)
    {
        builder.ToTable("PrcConversor"); // TODO: confirmar

        builder.HasKey(c => c.IdConversor);
        builder.Property(c => c.IdConversor)
            .HasColumnName("IdPrcConversor")
            .ValueGeneratedNever();

        builder.Property(c => c.IdUnidade1).HasColumnName("IdPrcUnidade1").IsRequired();
        builder.Property(c => c.IdUnidade2).HasColumnName("IdPrcUnidade2").IsRequired();
        builder.Property(c => c.Fator).HasColumnName("Fator").HasPrecision(18, 6).IsRequired();
        builder.Property(c => c.Ativo).HasColumnName("Ativo");

        builder.HasOne(c => c.Unidade1)
            .WithMany()
            .HasForeignKey(c => c.IdUnidade1)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Unidade2)
            .WithMany()
            .HasForeignKey(c => c.IdUnidade2)
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

#### `Infrastructure/Mappings/GrupoEstoqueMapping.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versatus.GestaoMaterial.Domain.Produtos;

namespace Versatus.GestaoMaterial.Infrastructure.Mappings;

public class GrupoEstoqueMapping : IEntityTypeConfiguration<GrupoEstoque>
{
    public void Configure(EntityTypeBuilder<GrupoEstoque> builder)
    {
        builder.ToTable("PrcGrupoEstoque"); // TODO: confirmar

        builder.HasKey(g => g.IdGrupoEstoque);
        builder.Property(g => g.IdGrupoEstoque)
            .HasColumnName("IdPrcGrupoEstoque")
            .ValueGeneratedNever();

        builder.Property(g => g.Codigo).HasColumnName("Codigo").HasMaxLength(20).IsRequired();
        builder.Property(g => g.Nome).HasColumnName("Nome").HasMaxLength(100).IsRequired();
        builder.Property(g => g.Descricao).HasColumnName("Descricao").HasMaxLength(500);
        builder.Property(g => g.IdGrupoEstoquePai).HasColumnName("IdPrcGrupoEstoquePai");
        builder.Property(g => g.Ativo).HasColumnName("Ativo");

        // Self-referencing hierarchy
        builder.HasOne(g => g.GrupoPai)
            .WithMany(g => g.SubGrupos)
            .HasForeignKey(g => g.IdGrupoEstoquePai)
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

#### `Infrastructure/Mappings/LinhaMapping.cs`
Mesma estrutura de `UnidadeMapping`, adaptando para `Linha` / `PrcLinha` / `IdPrcLinha`.

#### `Infrastructure/Mappings/MarcaMapping.cs`
Mesma estrutura de `UnidadeMapping`, adaptando para `Marca` / `PrcMarca` / `IdPrcMarca`.

#### `Infrastructure/Mappings/FabricanteMapping.cs`
```csharp
// (mesmo padrão) — IdEntidade mapeado como int simples, sem FK declarada no EF
builder.Property(f => f.IdEntidade).HasColumnName("IdGloEntidade");
// Não declarar HasOne/HasForeignKey para IdEntidade — cross-module
```

#### `Infrastructure/Mappings/ModeloMapping.cs`
```csharp
// Relação com Fabricante dentro do mesmo módulo — declarar HasOne normalmente
builder.HasOne(m => m.Fabricante)
    .WithMany(f => f.Modelos)
    .HasForeignKey(m => m.IdFabricante)
    .OnDelete(DeleteBehavior.Restrict);
```

#### `Infrastructure/Mappings/AlmoxarifadoMapping.cs`
```csharp
// IdFilial mapeado como int simples, sem FK declarada no EF
builder.Property(a => a.IdFilial).HasColumnName("IdGloFilial");
// Não declarar HasOne/HasForeignKey para IdFilial — cross-module
```

---

### TAREFA 5 — Criar o DbContext

**Arquivo:** `Infrastructure/GestaoMaterialDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoMaterial.Domain.Produtos;
using Versatus.GestaoMaterial.Domain.Estoque;

namespace Versatus.GestaoMaterial.Infrastructure;

/// <summary>
/// Contexto do Entity Framework Core para o módulo de Gestão de Material.
/// </summary>
public class GestaoMaterialDbContext : DbContext
{
    public GestaoMaterialDbContext(DbContextOptions<GestaoMaterialDbContext> options)
        : base(options) { }

    // Unidades
    public DbSet<Unidade> Unidades => Set<Unidade>();
    public DbSet<Conversor> Conversores => Set<Conversor>();

    // Classificação
    public DbSet<GrupoEstoque> GruposEstoque => Set<GrupoEstoque>();
    public DbSet<Linha> Linhas => Set<Linha>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Fabricante> Fabricantes => Set<Fabricante>();
    public DbSet<Modelo> Modelos => Set<Modelo>();

    // Estoque
    public DbSet<Almoxarifado> Almoxarifados => Set<Almoxarifado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestaoMaterialDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Converte bool → short (smallint no SQL Server legado)
        configurationBuilder.Properties<bool>()
            .HaveConversion<short>();
    }
}
```

---

### TAREFA 6 — Criar a Base de Repositório e Repositórios

#### `Infrastructure/Repositories/GestaoMaterialRepositorioBase.cs`

Copie exatamente o padrão da seção 3.5, substituindo:
- `AcessoGlobalRepositorioBase` → `GestaoMaterialRepositorioBase`
- `AcessoGlobalDbContext` → `GestaoMaterialDbContext`
- namespace → `Versatus.GestaoMaterial.Infrastructure.Repositories`

#### `Domain/Repositories/IUnidadeRepository.cs`
```csharp
using Versatus.Framework.Repositories;
using Versatus.GestaoMaterial.Domain.Produtos;

namespace Versatus.GestaoMaterial.Domain.Repositories;

public interface IUnidadeRepository : IRepositorio<Unidade>
{
    Task<Unidade?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}
```

#### `Infrastructure/Repositories/UnidadeRepository.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoMaterial.Domain.Produtos;
using Versatus.GestaoMaterial.Domain.Repositories;

namespace Versatus.GestaoMaterial.Infrastructure.Repositories;

public class UnidadeRepository : GestaoMaterialRepositorioBase<Unidade>, IUnidadeRepository
{
    public UnidadeRepository(GestaoMaterialDbContext context) : base(context) { }

    public async Task<Unidade?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => await Context.Unidades.FirstOrDefaultAsync(u => u.Codigo == codigo, cancellationToken);
}
```

#### `Domain/Repositories/IGrupoEstoqueRepository.cs`
```csharp
using Versatus.Framework.Repositories;
using Versatus.GestaoMaterial.Domain.Produtos;

namespace Versatus.GestaoMaterial.Domain.Repositories;

public interface IGrupoEstoqueRepository : IRepositorio<GrupoEstoque>
{
    Task<IEnumerable<GrupoEstoque>> ListarRaizesAsync(CancellationToken cancellationToken = default);
    Task<GrupoEstoque?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
}
```

#### `Infrastructure/Repositories/GrupoEstoqueRepository.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoMaterial.Domain.Produtos;
using Versatus.GestaoMaterial.Domain.Repositories;

namespace Versatus.GestaoMaterial.Infrastructure.Repositories;

public class GrupoEstoqueRepository : GestaoMaterialRepositorioBase<GrupoEstoque>, IGrupoEstoqueRepository
{
    public GrupoEstoqueRepository(GestaoMaterialDbContext context) : base(context) { }

    public async Task<IEnumerable<GrupoEstoque>> ListarRaizesAsync(CancellationToken cancellationToken = default)
        => await Context.GruposEstoque
            .Where(g => g.IdGrupoEstoquePai == null)
            .Include(g => g.SubGrupos)
            .OrderBy(g => g.Codigo)
            .ToListAsync(cancellationToken);

    public async Task<GrupoEstoque?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        => await Context.GruposEstoque
            .FirstOrDefaultAsync(g => g.Codigo == codigo, cancellationToken);
}
```

---

### TAREFA 7 — Criar o DI Extensions

**Arquivo:** `DependencyInjection/ServiceCollectionExtensions.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using Versatus.GestaoMaterial.Domain.Repositories;
using Versatus.GestaoMaterial.Infrastructure.Repositories;

namespace Versatus.GestaoMaterial.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGestaoMaterial(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUnidadeRepository, UnidadeRepository>();
        services.AddScoped<IGrupoEstoqueRepository, GrupoEstoqueRepository>();

        return services;
    }
}
```

---

### TAREFA 8 — Criar o Projeto de Testes

**Arquivo:** `tests\Versatus.GestaoMaterial.Tests\Versatus.GestaoMaterial.Tests.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\Versatus.GestaoMaterial\Versatus.GestaoMaterial.csproj" />
  </ItemGroup>
</Project>
```

Execute no terminal:
```powershell
dotnet sln add tests\Versatus.GestaoMaterial.Tests\Versatus.GestaoMaterial.Tests.csproj
```

**Arquivo:** `tests\Versatus.GestaoMaterial.Tests\BasicTests.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Versatus.GestaoMaterial.Domain.Produtos;
using Versatus.GestaoMaterial.Infrastructure;

namespace Versatus.GestaoMaterial.Tests;

public class BasicTests
{
    private static GestaoMaterialDbContext CriarContextoInMemory()
    {
        var options = new DbContextOptionsBuilder<GestaoMaterialDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new GestaoMaterialDbContext(options);
    }

    [Fact]
    public async Task DeveSalvarERecuperarUnidade()
    {
        // Arrange
        using var ctx = CriarContextoInMemory();
        var unidade = new Unidade
        {
            IdUnidade = 1,
            Codigo = "UN",
            Nome = "Unidade",
            Abreviatura = "un",
            Ativo = true
        };

        // Act
        ctx.Unidades.Add(unidade);
        await ctx.SaveChangesAsync();
        var resultado = await ctx.Unidades.FirstOrDefaultAsync(u => u.IdUnidade == 1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("UN", resultado.Codigo);
    }

    [Fact]
    public async Task DeveSalvarERecuperarGrupoEstoqueComHierarquia()
    {
        // Arrange
        using var ctx = CriarContextoInMemory();
        var grupoPai = new GrupoEstoque { IdGrupoEstoque = 1, Codigo = "G01", Nome = "Grupo Pai", Ativo = true };
        var grupoFilho = new GrupoEstoque { IdGrupoEstoque = 2, Codigo = "G01.01", Nome = "Subgrupo", IdGrupoEstoquePai = 1, Ativo = true };

        // Act
        ctx.GruposEstoque.AddRange(grupoPai, grupoFilho);
        await ctx.SaveChangesAsync();
        var filhos = await ctx.GruposEstoque.Where(g => g.IdGrupoEstoquePai == 1).ToListAsync();

        // Assert
        Assert.Single(filhos);
        Assert.Equal("G01.01", filhos[0].Codigo);
    }
}
```

---

### TAREFA 9 — Verificação Final

Execute na raiz da solução:

```powershell
# 1. Build completo (deve incluir GestaoMaterial agora)
dotnet build

# 2. Todos os testes (32 existentes + 2 novos = 34 total)
dotnet test --no-build --logger "console;verbosity=normal"
```

**Critérios de sucesso:**
- ✅ `Compilação com êxito. 0 Aviso(s) 0 Erro(s)`
- ✅ `Versatus.GestaoMaterial` aparece na lista de projetos compilados
- ✅ `Total de testes: 34 — Aprovados: 34`

---

## 6. O Que NÃO Fazer

- ❌ **Não implementar `Produto.cs`** — aguarda análise do legado (193 KB)
- ❌ **Não implementar `Estoque.cs`** — aguarda análise
- ❌ **Não implementar `MovimentoEstoque.cs`** — aguarda análise + testes de paridade
- ❌ **Não criar navegações EF entre projetos** (`GestaoMaterial` → `AcessoGlobal`)
- ❌ **Não usar `ValueGeneratedOnAdd()`** — sempre `ValueGeneratedNever()`
- ❌ **Não editar** `Versatus.Framework`, `Versatus.AcessoGlobal` ou outros projetos existentes
- ❌ **Não adicionar** pacotes além dos listados no .csproj de exemplo

---

## 7. Spec de Referência

O arquivo de spec completo do módulo está em:
```
C:\Pasta de Trabalho\Projetos\Analises\Versatus\Versatus.Net8\specs\modulos\MOD-03-GESTAO-MATERIAL.md
```
Consulte-o para detalhes adicionais sobre regras de negócio (seção 4) e ordem de implementação (seção 6).

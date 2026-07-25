# Prompt de Execução — Sistema de Navegação ERP (AppShell + Módulos + Sidebar + Favoritos)

> **Branch:** `feat/nav-menu-modulos-shell`
> **Base:** `develop` (atualizada em 2026-07-25)
> **Executor:** Qualquer agente IA com acesso ao workspace

---

## 1. LEITURA OBRIGATÓRIA ANTES DE QUALQUER AÇÃO

Leia os seguintes arquivos **nesta ordem** antes de escrever qualquer código:

1. `.agents/AGENTS.md` — 9 regras ativas do projeto (obrigatórias)
2. `.agents/skills/migrate-crud/SKILL.md` — Pipeline de migração (referência de padrões)
3. `specs/03-REGRAS-ANTI-ALUCINACAO.md` — Regras de fidelidade ao legado
4. `specs/01-VISAO-GERAL-ARQUITETURA.md` — Arquitetura atual do projeto

---

## 2. CONTEXTO DO PROJETO

- **Banco de dados:** SQL Server 2008 — SEM suporte a OFFSET/FETCH. Paginação SEMPRE em memória
- **Backend:** .NET 10, Clean Architecture, EF Core com Fluent API, Result<T> Pattern
- **Frontend:** React 19 + TypeScript + Material-UI v9 (MUI), react-router-dom v7, Vite 8
- **Tema:** Minimals (detalhado na Seção 2a abaixo)
- **Branch de trabalho:** `feat/nav-menu-modulos-shell` (já criada a partir de `develop`)
- **PROIBIDO:** Commitar em `main` ou `develop` diretamente

### Telas já migradas (NÃO alterar o comportamento delas)
- `FEntidade` → rota `/acesso-global/entidade`
- `FParametro` → rota `/acesso-global/parametro`
- `FCondicaoPagamento` → rota `/acesso-global/condicao-pagamento`

---

## 2a. TEMPLATE FRONTEND — MINIMALS (VERSÃO FREE + MELHORIAS PRO)

### Contexto
O projeto utiliza como base visual o template **Minimals** do [minimal-ui-kit](https://github.com/minimal-ui-kit/material-kit-react) (licença MIT — versão free/open-source).

- **Demo Free:** https://free.minimals.cc
- **Demo Pago (referência visual):** https://minimals.cc/dashboard
- **O código pago NÃO será utilizado** — apenas o free como base, com melhorias implementadas manualmente.

### Tema atual do projeto (`src/Versatus.Frontend/src/theme.ts`) — NÃO alterar
O tema já está configurado no projeto com as cores e tipografia do Minimals:
```
Cor primária:   #2065D1 (Azul Royal)
Cor secundária: #845ADF (Roxo sutil)
Fundo default:  #F4F6F8 (Cinza extra-claro)
Fundo paper:    #FFFFFF
Texto primário: #212B36
Texto secundário:#637381
Fonte: "Public Sans", "Inter", "Outfit", Roboto, sans-serif
Border radius:  12px (cards: 16px)
```

### Melhorias "Pro" a implementar no layout desta feature

A versão paga do Minimals tem features visuais que a free não tem. Implemente **todas** as abaixo:

| Feature Pro | Como implementar |
|---|---|
| **Sidebar colapsável** (modo mini com apenas ícones) | Botão toggle no topo da sidebar: `useState(sidebarOpen)`. Quando fechada: width=64px mostrando só ícones. Quando aberta: width=240px com labels. Transição CSS `width 0.3s ease` |
| **Seções com título em uppercase** na sidebar | Antes de cada grupo de accordion, renderizar `<Typography variant="overline" sx={{ color: 'text.disabled', px: 2, pt: 2, pb: 1 }}>` com o nome do grupo |
| **Highlight na rota ativa** com borda colorida | NavLink do react-router-dom: quando ativo, aplicar `borderLeft: '3px solid {moduloAtivo.corHex}'` + `bgcolor: alpha(corHex, 0.08)` + `fontWeight: 700` |
| **Cards com sombra Minimals** | Box shadow: `'0 0 2px 0 rgba(145,158,171,0.2), 0 12px 24px -4px rgba(145,158,171,0.12)'` — já configurado no tema |
| **TopBar sticky** com blur de fundo | `position: sticky; top: 0; backdropFilter: blur(6px); bgcolor: rgba(255,255,255,0.8)` |
| **Botão de módulo estilizado** | Pill com borda, ponto colorido (cor do módulo), nome do módulo e chevron. Hover com bgcolor suave |
| **Grid de módulos no popup** | MUI `Popover` com `Paper` elevado. Grid 4 colunas em desktop, 3 em mobile. Cada card: ícone sobre fundo colorido suave `alpha(corHex, 0.12)` + nome centralizado abaixo. Card ativo com borda colorida |
| **Animação de abertura do popup** | MUI Popover já tem `TransitionComponent`. Adicionar `transformOrigin` e `anchorOrigin` adequados |
| **Accordion sem borda padrão do MUI** | Sobrescrever estilo: `boxShadow: 'none'`, `'&:before': { display: 'none' }`, `borderRadius: 0` |
| **Hover nos itens da sidebar** | `'&:hover': { bgcolor: alpha('#000', 0.04), borderRadius: 1 }` |
| **Toast de favoritos** | MUI `Snackbar` + `Alert` com severity `success` e `error`. Auto-hide em 2500ms |
| **Badge de módulo nos favoritos** | `<Box component="span" sx={{ width: 8, height: 8, borderRadius: '50%', bgcolor: corHex, mr: 1, flexShrink: 0 }} />` |
| **Tooltip com caminho completo** | MUI `Tooltip` com `title={caminhoCompleto}` e `placement="right"` nos itens de favorito |
| **Ícone ⭐ ao hover na rotina** | Usar `useState(hoveredRotina)`. Renderizar `<StarBorderIcon>` com opacity 0 → 1 no hover via sx |
| **Scroll suave na sidebar** | `overflowY: 'auto'`, `'&::-webkit-scrollbar': { width: 4 }`, `'&::-webkit-scrollbar-thumb': { bgcolor: alpha('#000', 0.12), borderRadius: 2 }` |

### Paleta de cores dos módulos (para os cards do popup)
Cada módulo tem sua cor `corHex` vinda do banco. Para o fundo do ícone no card do popup usar: `alpha(corHex, 0.12)`. Para a borda do card ativo: `2px solid corHex`.

### Referência visual alvo
O resultado deve ser visualmente próximo de: https://minimals.cc/dashboard
Sidebar com grupos, topbar limpa com seletor de módulo, cards com sombra suave, tipografia elegante.

---

### Telas já migradas (NÃO alterar o comportamento delas)
- `FEntidade` → rota `/acesso-global/entidade`
- `FParametro` → rota `/acesso-global/parametro`
- `FCondicaoPagamento` → rota `/acesso-global/condicao-pagamento`

---

## 3. DECISÕES DE DESIGN (TODAS APROVADAS — NÃO QUESTIONAR)

| Decisão | Escolha |
|---|---|
| Padrão de navegação | **Opção 3**: Botão Seletor de Módulo no TopBar + Grid Dropdown + Sidebar Contextual |
| Mapeamento de rota | `GloModulo.PrefixoRota` (nova coluna) + `GloRotina.Objeto` como sufixo |
| Cor e ícone dos módulos | Colunas `CorHex` e `IconeMui` na `GloModulo` — dinâmico, sem arquivo estático |
| Fallback de cor/ícone | Se NULL, gerar cor via hash do `IdModulo` + ícone `Apps` |
| Módulo default | Último módulo visitado salvo em `localStorage` (`versatus_modulo_ativo`) |
| Accordion da sidebar | **Não-exclusivo** — múltiplos grupos abertos simultaneamente |
| Estado do accordion | Salvo em `localStorage` com chave `versatus_sidebar_{idModulo}` |
| Favoritos — persistência | **Banco de dados** — tabela `GloFavorito` |
| Favoritos — IdUsuario | **Placeholder = 1** nesta fase (autenticação é fase futura) |
| Favoritos — seção | Colapsável com chevron. Estado salvo em `localStorage` |
| Favoritos — identificação | Badge colorida (cor do módulo) + tooltip com caminho completo ao hover |
| Favoritos — quantidade | Sem limite. Scroll interno na seção (max-height: 180px) |
| Favoritos — adicionar | Hover sobre item na sidebar exibe estrela → clique adiciona |
| Favoritos — remover | Hover sobre item nos favoritos exibe X → clique remove (sem confirmação) |

---

## 4. ESTRUTURA DO BANCO DE DADOS

### Tabelas legadas (NÃO alterar estrutura, apenas adicionar colunas)

```
GloModulo     → IdGloModulo, Nome, Ordem, ChaveModulo, IdTipoModulo
GloMenu       → IdGloMenu, Descricao, Ordem
GloMenuModulo → IdGloMenu, IdGloModulo, Ordem   (Menu raiz de um Módulo)
GloMenuMenu   → IdGloMenu, IdGloMenuPai, Ordem  (Hierarquia: pai → filho)
GloMenuRotina → IdGloMenu, IdGloRotina, Ordem   (Menu → Rotina/Tela)
GloRotina     → IdGloRotina, Nome, IdTipoRotina, Objeto
```

### Script SQL idempotente — execute PRIMEIRO

```sql
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloModulo') AND name = 'PrefixoRota')
    ALTER TABLE GloModulo ADD PrefixoRota VARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloModulo') AND name = 'CorHex')
    ALTER TABLE GloModulo ADD CorHex VARCHAR(7) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('GloModulo') AND name = 'IconeMui')
    ALTER TABLE GloModulo ADD IconeMui VARCHAR(50) NULL;

UPDATE GloModulo SET PrefixoRota='/acesso-global',    CorHex='#637381', IconeMui='ManageAccounts'     WHERE IdGloModulo = 1;
UPDATE GloModulo SET PrefixoRota='/financeiro',       CorHex='#10B981', IconeMui='AccountBalance'     WHERE IdGloModulo = 2;
UPDATE GloModulo SET PrefixoRota='/faturamento',      CorHex='#2065D1', IconeMui='ReceiptLong'        WHERE IdGloModulo = 3;
UPDATE GloModulo SET PrefixoRota='/material',         CorHex='#8B5CF6', IconeMui='Inventory2'         WHERE IdGloModulo = 4;
UPDATE GloModulo SET PrefixoRota='/compras',          CorHex='#06B6D4', IconeMui='ShoppingCart'       WHERE IdGloModulo = 5;
UPDATE GloModulo SET PrefixoRota='/tributos',         CorHex='#EF4444', IconeMui='Gavel'              WHERE IdGloModulo = 6;
UPDATE GloModulo SET PrefixoRota='/rh',               CorHex='#F59E0B', IconeMui='People'             WHERE IdGloModulo = 7;
UPDATE GloModulo SET PrefixoRota='/os',               CorHex='#F97316', IconeMui='Build'              WHERE IdGloModulo = 8;
UPDATE GloModulo SET PrefixoRota='/ativo-fixo',       CorHex='#64748B', IconeMui='Apartment'          WHERE IdGloModulo = 9;
UPDATE GloModulo SET PrefixoRota='/logistica',        CorHex='#0EA5E9', IconeMui='LocalShipping'      WHERE IdGloModulo = 10;
UPDATE GloModulo SET PrefixoRota='/mrp',              CorHex='#7C3AED', IconeMui='PrecisionManufacturing' WHERE IdGloModulo = 11;
UPDATE GloModulo SET PrefixoRota='/contrato',         CorHex='#DB2777', IconeMui='Description'        WHERE IdGloModulo = 12;
UPDATE GloModulo SET PrefixoRota='/frota',            CorHex='#16A34A', IconeMui='LocalShipping'      WHERE IdGloModulo = 13;
UPDATE GloModulo SET PrefixoRota='/contabil',         CorHex='#0891B2', IconeMui='BarChart'           WHERE IdGloModulo = 14;
UPDATE GloModulo SET PrefixoRota='/versatus',         CorHex='#1E293B', IconeMui='AdminPanelSettings' WHERE IdGloModulo = 15;
UPDATE GloModulo SET PrefixoRota='/obra',             CorHex='#92400E', IconeMui='Engineering'        WHERE IdGloModulo = 16;
UPDATE GloModulo SET PrefixoRota='/pdv',              CorHex='#DC2626', IconeMui='PointOfSale'        WHERE IdGloModulo = 17;
UPDATE GloModulo SET PrefixoRota='/garagem',          CorHex='#15803D', IconeMui='DirectionsCar'      WHERE IdGloModulo = 18;
UPDATE GloModulo SET PrefixoRota='/small',            CorHex='#4338CA', IconeMui='Storefront'         WHERE IdGloModulo = 19;
UPDATE GloModulo SET PrefixoRota='/pesagem',          CorHex='#B45309', IconeMui='Scale'              WHERE IdGloModulo = 20;
UPDATE GloModulo SET PrefixoRota='/locacao',          CorHex='#0E7490', IconeMui='MeetingRoom'        WHERE IdGloModulo = 21;
UPDATE GloModulo SET PrefixoRota='/ecommerce',        CorHex='#BE185D', IconeMui='ShoppingBag'        WHERE IdGloModulo = 22;
UPDATE GloModulo SET PrefixoRota='/educacional',      CorHex='#1D4ED8', IconeMui='School'             WHERE IdGloModulo = 23;
UPDATE GloModulo SET PrefixoRota='/transporte',       CorHex='#065F46', IconeMui='DirectionsBus'      WHERE IdGloModulo = 24;
UPDATE GloModulo SET PrefixoRota='/producao',         CorHex='#7C2D12', IconeMui='Factory'            WHERE IdGloModulo = 25;
UPDATE GloModulo SET PrefixoRota='/mdfe',             CorHex='#374151', IconeMui='Article'            WHERE IdGloModulo = 26;
UPDATE GloModulo SET PrefixoRota='/armazem',          CorHex='#78350F', IconeMui='Warehouse'          WHERE IdGloModulo = 27;
UPDATE GloModulo SET PrefixoRota='/nfse',             CorHex='#0369A1', IconeMui='Receipt'            WHERE IdGloModulo = 28;
UPDATE GloModulo SET PrefixoRota='/epay',             CorHex='#065F46', IconeMui='Payment'            WHERE IdGloModulo = 29;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GloFavorito')
BEGIN
    CREATE TABLE GloFavorito (
        IdGloFavorito INT IDENTITY(1,1) PRIMARY KEY,
        IdUsuario     INT NOT NULL DEFAULT 1,
        IdGloRotina   INT NOT NULL,
        Ordem         INT NOT NULL DEFAULT 0,
        CONSTRAINT FK_GloFavorito_GloRotina FOREIGN KEY (IdGloRotina) REFERENCES GloRotina(IdGloRotina)
    );
    CREATE INDEX IX_GloFavorito_Usuario ON GloFavorito(IdUsuario);
END
```

Connection string: `Server=localhost\SQLEXPRESS2008;Database=versatus;User Id=sa;Password=V#v070804s;TrustServerCertificate=True;`

---

## 5. FASES DE EXECUÇÃO

### FASE 1 — Banco de Dados
Execute o script acima. Verifique:
```sql
SELECT IdGloModulo, Nome, PrefixoRota, CorHex, IconeMui FROM GloModulo ORDER BY Ordem;
```
Commit: `git commit -m "feat(db): Add PrefixoRota, CorHex, IconeMui to GloModulo + Create GloFavorito"`

### FASE 2 — Backend

Antes de build: verificar servidor ativo com `manage_task list` e matar se necessário.

#### 2.1 — Entidades POCO (SEM DataAnnotations — Regra 1 do AGENTS.md)

Criar em `src/Versatus.AcessoGlobal/Domain/Entities/`:
- `GloModulo.cs` — props: IdModulo, Nome, Ordem, ChaveModulo, TipoModulo, PrefixoRota?, CorHex?, IconeMui?
- `GloMenu.cs` — IdMenu, Descricao, Ordem
- `GloMenuModulo.cs` — IdMenu, IdModulo, Ordem + props de navegação
- `GloMenuMenu.cs` — IdMenu, IdMenuPai, Ordem + props de navegação
- `GloMenuRotina.cs` — IdMenu, IdRotina, Ordem + props de navegação
- `GloRotina.cs` — IdRotina, Nome, TipoRotina, Objeto?
- `GloFavorito.cs` — IdFavorito, IdUsuario, IdRotina, Ordem + prop de navegação

#### 2.2 — Fluent API no DbContext (código COMPLETO obrigatório)

Adicionar em `OnModelCreating` do `VersatusDbContext`:

```csharp
// GloModulo — tabela e colunas legadas
modelBuilder.Entity<GloModulo>(e => {
    e.ToTable("GloModulo");
    e.HasKey(x => x.IdModulo);
    e.Property(x => x.IdModulo).HasColumnName("IdGloModulo");
    e.Property(x => x.TipoModulo).HasColumnName("IdTipoModulo");
    e.Property(x => x.PrefixoRota).HasMaxLength(100).IsRequired(false);
    e.Property(x => x.CorHex).HasMaxLength(7).IsRequired(false);
    e.Property(x => x.IconeMui).HasMaxLength(50).IsRequired(false);
});

// GloMenu
modelBuilder.Entity<GloMenu>(e => {
    e.ToTable("GloMenu");
    e.HasKey(x => x.IdMenu);
    e.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
    e.Property(x => x.Descricao).HasMaxLength(200).IsRequired();
});

// GloMenuModulo — PK composta
modelBuilder.Entity<GloMenuModulo>(e => {
    e.ToTable("GloMenuModulo");
    e.HasKey(x => new { x.IdMenu, x.IdModulo });
    e.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
    e.Property(x => x.IdModulo).HasColumnName("IdGloModulo");
    e.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.IdMenu);
    e.HasOne(x => x.Modulo).WithMany().HasForeignKey(x => x.IdModulo);
});

// GloMenuMenu — PK composta + sem cascade delete (auto-referência)
modelBuilder.Entity<GloMenuMenu>(e => {
    e.ToTable("GloMenuMenu");
    e.HasKey(x => new { x.IdMenu, x.IdMenuPai });
    e.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
    e.Property(x => x.IdMenuPai).HasColumnName("IdGloMenuPai");
    e.HasOne(x => x.Menu)
        .WithMany()
        .HasForeignKey(x => x.IdMenu)
        .OnDelete(DeleteBehavior.NoAction);
    e.HasOne(x => x.MenuPai)
        .WithMany()
        .HasForeignKey(x => x.IdMenuPai)
        .OnDelete(DeleteBehavior.NoAction);
});

// GloMenuRotina — PK composta
modelBuilder.Entity<GloMenuRotina>(e => {
    e.ToTable("GloMenuRotina");
    e.HasKey(x => new { x.IdMenu, x.IdRotina });
    e.Property(x => x.IdMenu).HasColumnName("IdGloMenu");
    e.Property(x => x.IdRotina).HasColumnName("IdGloRotina");
    e.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.IdMenu);
    e.HasOne(x => x.Rotina).WithMany().HasForeignKey(x => x.IdRotina);
});

// GloRotina
modelBuilder.Entity<GloRotina>(e => {
    e.ToTable("GloRotina");
    e.HasKey(x => x.IdRotina);
    e.Property(x => x.IdRotina).HasColumnName("IdGloRotina");
    e.Property(x => x.TipoRotina).HasColumnName("IdTipoRotina");
    e.Property(x => x.Objeto).HasMaxLength(200).IsRequired(false);
});

// GloFavorito
modelBuilder.Entity<GloFavorito>(e => {
    e.ToTable("GloFavorito");
    e.HasKey(x => x.IdFavorito);
    e.Property(x => x.IdFavorito).HasColumnName("IdGloFavorito");
    e.Property(x => x.IdRotina).HasColumnName("IdGloRotina");
    e.HasOne(x => x.Rotina)
        .WithMany()
        .HasForeignKey(x => x.IdRotina)
        .OnDelete(DeleteBehavior.NoAction);
});
```

#### 2.3 — DTOs (incluindo CorHex e IconeMui obrigatórios)

```csharp
// ModuloMenuDto — CorHex e IconeMui DEVEM estar presentes (vêm do banco)
public record ModuloMenuDto(
    int IdModulo,
    string Nome,
    string? PrefixoRota,
    string? IconeMui,   // nome do ícone MUI ex: "ReceiptLong"
    string? CorHex,     // hexadecimal ex: "#2065D1"
    int Ordem,
    List<MenuItemDto> Menus
);

public record MenuItemDto(
    int IdMenu, string Descricao, int Ordem,
    List<MenuItemDto> SubMenus,
    List<RotinaItemDto> Rotinas
);

public record RotinaItemDto(
    int IdRotina, string Nome, string? Objeto,
    int Ordem, string RotaCompleta
    // RotaCompleta = modulo.PrefixoRota + "/" + rotina.Objeto (montado no MenuService)
);

public record FavoritoDto(
    int IdFavorito, int IdRotina,
    string NomeRotina, string RotaCompleta,
    string NomeModulo,
    string? CorHex,        // cor do módulo desta rotina (para badge)
    string CaminhoCompleto // ex: "Faturamento → Cadastros → Clientes"
);
```

#### 2.4 — MenuService (SQL Server 2008 — tudo em memória)

Métodos obrigatórios:
- `ObterArvore()` — carregar todos os módulos, menus, submenus e rotinas com `ToListAsync()` e montar a hierarquia em memória. `RotaCompleta = modulo.PrefixoRota + "/" + rotina.Objeto`
- `ObterFavoritos(int idUsuario)` — carregar com `ToListAsync()` incluindo rotina e módulo
- `AdicionarFavorito(int idUsuario, int idRotina)` — verificar se já existe antes de inserir
- `RemoverFavorito(int idUsuario, int idRotina)` — retornar `Result<bool>` se não encontrado

> ⚠️ PROIBIDO usar `.Skip().Take()` diretamente sobre `IQueryable` (Regra 7 do AGENTS.md)

#### 2.5 — MenuController

Endpoints (URL padrão — usar SEMPRE `/api/menu/` como prefixo):
```
GET    /api/menu/arvore              → List<ModuloMenuDto>
GET    /api/menu/favoritos           → List<FavoritoDto>  (IdUsuario = 1 hardcoded)
POST   /api/menu/favoritos           → body: { idRotina: int }
DELETE /api/menu/favoritos/{idRotina}
```

Executar: `dotnet build src/Versatus.WebAPI/Versatus.WebAPI.csproj`
Commit: `git commit -m "feat(backend): Add Menu navigation entities, DTOs, MenuService and MenuController"`

### FASE 3 — Frontend

#### 3.1 — Tipos TypeScript (`src/types/menu.ts`)
As interfaces DEVEM espelhar os DTOs do backend exatamente:
```typescript
export interface ModuloMenuDto {
  idModulo: number;
  nome: string;
  prefixoRota: string | null;
  iconeMui: string | null;   // nome do ícone MUI — pode ser null (usar fallback 'Apps')
  corHex: string | null;     // hex — pode ser null (gerar cor via hash do idModulo)
  ordem: number;
  menus: MenuItemDto[];
}
export interface MenuItemDto {
  idMenu: number; descricao: string; ordem: number;
  subMenus: MenuItemDto[]; rotinas: RotinaItemDto[];
}
export interface RotinaItemDto {
  idRotina: number; nome: string; objeto: string | null;
  ordem: number; rotaCompleta: string;
}
export interface FavoritoDto {
  idFavorito: number; idRotina: number; nomeRotina: string;
  rotaCompleta: string; nomeModulo: string;
  corHex: string | null;    // cor do módulo (para badge colorida)
  caminhoCompleto: string;  // ex: "Faturamento → Cadastros → Clientes"
}
```

#### 3.2 — Hooks (URL padronizada: `/api/menu/`)
- `src/hooks/useMenuArvore.ts` — `GET /api/menu/arvore` + recuperar módulo ativo do `localStorage` key `versatus_modulo_ativo`
- `src/hooks/useFavoritos.ts` — endpoints:
  - `GET /api/menu/favoritos`
  - `POST /api/menu/favoritos`
  - `DELETE /api/menu/favoritos/{idRotina}`
  - Exibir MUI `Snackbar` + `Alert` (severity `success`/`error`, auto-hide 2500ms)

#### 3.3 — Context (`src/context/MenuContext.tsx`)
```typescript
interface MenuContextState {
  modulos: ModuloMenuDto[];
  moduloAtivo: ModuloMenuDto | null;
  setModuloAtivo: (m: ModuloMenuDto) => void; // persiste em localStorage
  favoritos: FavoritoDto[];
  adicionarFavorito: (rotina: RotinaItemDto, modulo: ModuloMenuDto) => void;
  removerFavorito: (idRotina: number) => void;
  isLoading: boolean;
}
```

#### 3.4 — Componentes de Layout
- `src/components/layout/AppShell.tsx` — `<TopBar>` + `<ContextualSidebar>` + `<Outlet>` (react-router-dom)
- `src/components/layout/TopBar.tsx` — logo + `<ModuleSelectorButton>` + ícone busca/avatar (placeholders)
- `src/components/layout/ModuleSelectorButton.tsx` — Pill estilizado + MUI Popover com grid de cards de módulos
- `src/components/layout/ContextualSidebar.tsx` — accordion não-exclusivo + seção `⭐ Favoritos` colapsável

#### 3.5 — Modificações em arquivos existentes
- `src/main.tsx` — envolver app com `<BrowserRouter>`
- `src/App.tsx` — substituir Tabs manuais por:
```tsx
<MenuProvider>
  <Routes>
    <Route path="/" element={<AppShell />}>
      <Route index element={<Navigate to="/acesso-global/parametro" replace />} />
      <Route path="/acesso-global/entidade" element={<FEntidade />} />
      <Route path="/acesso-global/parametro" element={<FParametro />} />
      <Route path="/acesso-global/condicao-pagamento" element={<FCondicaoPagamento />} />
    </Route>
  </Routes>
</MenuProvider>
```

Executar: `cd src/Versatus.Frontend && npm run build`
Commit: `git commit -m "feat(frontend): Add AppShell, TopBar, ModuleSelectorButton, ContextualSidebar with favorites"`

---

## 6. CHECKLIST FINAL

- [ ] Script SQL executado com sucesso (29 módulos com cor/ícone/rota)
- [ ] `dotnet build` limpo (0 erros)
- [ ] `npm run build` limpo (0 erros TypeScript)
- [ ] GET /api/menu/arvore retorna JSON com hierarquia completa
- [ ] AppShell renderiza com TopBar + Sidebar + área de conteúdo
- [ ] Seletor de módulo abre popup com grid de 29 módulos com cores
- [ ] Sidebar muda ao trocar módulo — accordion não-exclusivo funciona
- [ ] Hover em rotina na sidebar exibe ⭐ — clique adiciona favorito
- [ ] Seção Favoritos exibe badge colorida + tooltip com caminho completo
- [ ] Hover em favorito exibe X — clique remove sem confirmação
- [ ] Seção Favoritos colapsa/expande com chevron
- [ ] FEntidade, FParametro, FCondicaoPagamento funcionam via rotas
- [ ] Módulo ativo persiste ao recarregar (localStorage)

---

## 7. ESCOPO — O QUE ESTA FEATURE NÃO COBRE

> [!IMPORTANT]
> Os itens abaixo estão **explicitamente fora do escopo** desta branch. Não implemente, não esboce, não deixe TODOs de código para eles. Serão tratados em features separadas com seus próprios prompts de execução.

| Fora do escopo | Motivo |
|---|---|
| **Autenticação / JWT** | Fase futura dedicada. O `IdUsuario` dos favoritos usa `1` como placeholder nesta fase |
| **Filtragem do menu por permissão de usuário** | Depende de autenticação — o endpoint `/api/menu/arvore` retorna todos os itens sem filtro por ora |
| **Tela de cadastro de Módulos** (`FModulo`) | CRUD separado — será migrado como Padrão A em outra branch |
| **Tela de cadastro de Menus** (`FMenu`) | CRUD separado — será migrado como Padrão A em outra branch |
| **Tela de cadastro de Rotinas** (`FRotina`) | CRUD separado — será migrado como Padrão A em outra branch |
| **Busca global (⌘K) no TopBar** | Feature de UX avançada — placeholder visual (ícone de lupa sem funcionalidade) é suficiente |
| **Itens recentes ("Visitados recentemente")** | Feature futura da sidebar — não implementar nesta fase |
| **Demais telas de negócio** | Cada tela tem seu próprio prompt de execução em `specs/prompts-execucao/` |

---

## 8. FINALIZAÇÃO

```powershell
git push origin feat/nav-menu-modulos-shell
```

Apresente resumo dos arquivos criados/alterados.
Pergunte ao usuário se deseja fazer merge para `develop`.
Após merge: PERGUNTAR se deseja excluir a branch `feat/nav-menu-modulos-shell` local e remota.

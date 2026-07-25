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

Criar em `src/Versatus.AcessoGlobal/Domain/Entities/`:
- `GloModulo.cs` — com props: IdModulo, Nome, Ordem, ChaveModulo, TipoModulo, PrefixoRota?, CorHex?, IconeMui?
- `GloMenu.cs` — IdMenu, Descricao, Ordem
- `GloMenuModulo.cs` — IdMenu, IdModulo, Ordem + navegação
- `GloMenuMenu.cs` — IdMenu, IdMenuPai, Ordem + navegação (sem cascade delete)
- `GloMenuRotina.cs` — IdMenu, IdRotina, Ordem + navegação
- `GloRotina.cs` — IdRotina, Nome, TipoRotina, Objeto?
- `GloFavorito.cs` — IdFavorito, IdUsuario, IdRotina, Ordem + navegação

Adicionar Fluent API no DbContext (nomes de tabelas e colunas legadas conforme Seção 4).

Criar DTOs: `ModuloMenuDto`, `MenuItemDto`, `RotinaItemDto`, `FavoritoDto`.

Criar `MenuService` com métodos:
- `ObterArvore()` — hierarquia recursiva, tudo em memória (ToListAsync)
- `ObterFavoritos(int idUsuario)` — lista com dados completos
- `AdicionarFavorito(int idUsuario, int idRotina)`
- `RemoverFavorito(int idUsuario, int idRotina)`

Criar `MenuController` com:
- `GET /api/menu/arvore`
- `GET /api/menu/favoritos` (idUsuario = 1 hardcoded)
- `POST /api/menu/favoritos`
- `DELETE /api/menu/favoritos/{idRotina}`

Executar: `dotnet build src/Versatus.WebAPI/Versatus.WebAPI.csproj`
Commit: `git commit -m "feat(backend): Add Menu navigation entities, DTOs, MenuService and MenuController"`

### FASE 3 — Frontend

Criar:
- `src/types/menu.ts` — interfaces TypeScript (ModuloMenuDto, MenuItemDto, RotinaItemDto, FavoritoDto)
- `src/context/MenuContext.tsx` — estado global de navegação e favoritos
- `src/hooks/useMenuArvore.ts` — busca árvore da API + localStorage
- `src/hooks/useFavoritos.ts` — CRUD favoritos com toast (MUI Snackbar)
- `src/components/layout/AppShell.tsx` — shell com TopBar + Sidebar + Outlet
- `src/components/layout/TopBar.tsx` — logo + ModuleSelectorButton + search/avatar placeholder
- `src/components/layout/ModuleSelectorButton.tsx` — botão com MUI Popover + grid de módulos
- `src/components/layout/ContextualSidebar.tsx` — accordion não-exclusivo + seção favoritos colapsável

Modificar:
- `src/main.tsx` — envolver com BrowserRouter
- `src/App.tsx` — substituir Tabs por Routes com AppShell + 3 rotas existentes

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

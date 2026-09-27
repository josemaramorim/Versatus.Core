# Spec Funcional: Breadcrumb e Navegação para o Início

> **Tipo:** Spec de infraestrutura de frontend (transversal — vale para todas as telas de cadastro)
> **Versão:** 1.0 (2026-09-27) · **Status:** aprovada pelo usuário (decisões NB1–NB4 em §8)
> **Origem:** relato do usuário com captura de tela de `/financeiro/caixabanco` — o breadcrumb
> mostra `Dashboard / Cadastros Base / Caixa / Banco`, caminho que não existe no menu, e o
> clique em "Dashboard" não leva ao dashboard.
> **Não altera backend nem banco.**

---

## 1. Problema

| # | Sintoma (captura de tela) | Esperado |
|---|---|---|
| P1 | Breadcrumb `Dashboard / Cadastros Base / Caixa / Banco` | Caminho real do menu: `Início › Gestão Financeira › Cadastros › Caixa/Conta` |
| P2 | Clicar em "Dashboard" ou "Cadastros Base" não navega; a URL ganha `#` (`/financeiro/caixabanco#`) | "Início" abre o dashboard; os demais níveis têm uma ação útil ou não parecem clicáveis |
| P3 | Aba e título dizem "Caixa / Banco"; o menu diz "Caixa/Conta" | O nome da aba e do último nível batem com o item de menu clicado |
| P4 | Com uma aba aberta, não há como voltar ao dashboard | Existe um "Início" alcançável a qualquer momento |

## 2. Causa raiz (código atual)

1. **Breadcrumb fixo no código**, em dois componentes:
   - `components/crud/CadastroBasePage.tsx:166-177` (modo lista): `Dashboard / Cadastros Base / {título}`.
   - `components/layout/BaseCadastro.tsx:105-126` (modo formulário): `Dashboard / Cadastros / {título} / Novo|Editar`.
   - Os dois primeiros níveis são `<Link href="#">`: não navegam e sujam a URL com `#` (P2).
2. **O dashboard só aparece quando não há nenhuma aba aberta** — `AppShell.tsx:99`:
   `{abas.length === 0 && <DashboardScreen />}`. O `TabsContext` não tem ação "ir para o início"
   (`abaAtivaId` só vira `null` quando a última aba é fechada) (P4).
3. **O título da aba tem duas fontes:**
   - aberta pelo menu → `rotina.nome` (`ContextualSidebar.tsx:119`) = "Caixa/Conta";
   - aberta pela URL → `ROTA_TITULO_MAP` do `AppShell.tsx` = "Caixa / Banco" (é o caso da captura).
   - O título da página vem de `config.getTitulo()` (P3).
4. **O caminho certo já existe no frontend e não é usado:** a árvore de `GET /api/menu/arvore`
   (já carregada no `MenuContext`) traz módulo → menus → submenus → rotinas, com a
   `rotaCompleta` de cada rotina. O backend até monta esse caminho para os favoritos
   (`MenuService.MapearRotinasDaArvore` → `CaminhoCompleto`), mas ele não chega às telas.

**Caminho real da rotina 29 (`FCaixaBanco`)**, conferido no banco:
`GLOMODULO 2 "GESTÃO FINANCEIRA"` → menu raiz `2 "GESTÃO FINANCEIRA"` → submenu `30 "Cadastros"`
→ rotina `29 "Caixa/Conta"` (rota `/financeiro/caixabanco`). O menu raiz repete o nome do módulo,
então o caminho "cru" sairia `GESTÃO FINANCEIRA › GESTÃO FINANCEIRA › Cadastros › Caixa/Conta`.
Esse nível repetido precisa ser removido (regra R3).

## 3. Impacto

### 3.1 Telas afetadas (todas usam os componentes com breadcrumb fixo)

| Tela | Rota | Breadcrumb hoje | Breadcrumb proposto |
|---|---|---|---|
| Entidade | `/acesso-global/entidade` | Dashboard / Cadastros Base / … | Início › Acesso Global › (menu) › (rotina) |
| Condição de pagamento | `/acesso-global/condicao-pagamento` | idem | Início › Acesso Global › Cadastro financeiro › Condição de pagamento |
| Caixa/Banco | `/financeiro/caixabanco` | idem | Início › Gestão Financeira › Cadastros › Caixa/Conta |
| Parâmetros | `/acesso-global/parametro` | sem breadcrumb (não usa os componentes) | fora de escopo (§7) |

Toda tela nova de cadastro herda a correção de graça (o breadcrumb passa a ser derivado do menu).

### 3.2 Arquivos

| Arquivo | Mudança | Risco |
|---|---|---|
| `components/layout/AppBreadcrumbs.tsx` (**novo**) | Componente único de breadcrumb | baixo |
| `hooks/useCaminhoRotina.ts` (**novo**) | Função pura `montarCaminhoRotina(modulos, rota)` + hook que a usa com o `MenuContext` e a rota da aba (`TabScopeContext`) | baixo — função pura testável |
| `components/crud/CadastroBasePage.tsx` | Troca o bloco fixo por `<AppBreadcrumbs />` | baixo |
| `components/layout/BaseCadastro.tsx` | Troca o bloco fixo por `<AppBreadcrumbs sufixo="Novo|Editar" onVoltarLista={…} />` | baixo — mantém o "voltar para a lista" que já existe |
| `context/TabsContext.tsx` | Nova ação `irParaInicio()` (`abaAtivaId = null`, abas continuam abertas) | **médio** — muda o significado de `abaAtivaId === null` |
| `components/layout/AppShell.tsx` | Dashboard quando `abaAtivaId === null` (não só quando `abas.length === 0`); título da aba vindo do menu, com `ROTA_TITULO_MAP` como reserva | médio — conferir o keep-alive das abas ocultas |
| `components/layout/TabBar.tsx` | Botão fixo "Início" (ícone de casa) à esquerda das abas; nenhuma aba destacada quando está no início | baixo |
| `hooks/useCaminhoRotina.test.ts` (**novo**) | Vitest da função pura | — |

Estimativa: **1 tarefa pequena** (~8 arquivos, ~250 linhas), sem backend, sem banco, sem
migration. Não depende do E3-T07; o E3-T07 só se beneficia.

### 3.3 Comportamentos que mudam (checar na revisão)

- **Início com abas abertas:** hoje é impossível; passa a ser possível. As abas continuam montadas
  (keep-alive) e mantêm o que foi digitado. Voltar a uma aba pela barra devolve o estado dela.
- **Alterações não salvas:** ir ao Início **não** descarta nada nem pergunta — a aba só fica oculta.
  O aviso continua ao **fechar** a aba (regra já existente).
- **URL:** "Início" leva a `/`; clicar numa aba ou numa rotina não muda a URL (igual a hoje).
  Nenhum link produz `#`.

## 4. Proposta — regras

| ID | Regra |
|---|---|
| R1 | O breadcrumb é **derivado da árvore do menu**: `Início › Módulo › Menu(s) › Rotina`, achando a rotina pela `rotaCompleta` igual à rota da aba. |
| R2 | Se a rota aparecer em mais de um lugar do menu, vale a **primeira ocorrência** na ordem da árvore (mesma regra de `MapearRotinasDaArvore` no backend). |
| R3 | Um nível de menu com o **mesmo nome do módulo** (sem diferenciar maiúsculas nem acentos) é omitido — evita "Gestão Financeira › Gestão Financeira". |
| R4 | **Início** é clicável: chama `irParaInicio()`, mostra o dashboard e leva a URL para `/`. |
| R5 | **Módulo** é texto (não clicável) — decisão NB2: a troca de módulo continua pelo seletor do topo; um nível que parece link e não navega repetiria o problema P2. |
| R6 | **Níveis de menu** são texto (não clicáveis): no menu são pastas, sem tela própria. Nomes em caixa alta vindos do banco (ex.: módulo "GESTÃO FINANCEIRA") são exibidos em formato de título — só na exibição (NB4). |
| R7 | **Rotina**: texto na lista; no formulário vira link "voltar para a lista" (com o aviso de alterações não salvas que já existe) e ganha o sufixo `Novo` / `Editar`. |
| R8 | Se a rota **não estiver no menu** (tela aberta por URL sem rotina no banco), o caminho vira `Início › {título da tela}` — nunca um caminho inventado. |
| R9 | Nenhum item do breadcrumb usa `href="#"`; itens clicáveis são botões/links com `onClick`. |
| R10 | Título da aba = nome da rotina no menu (`rotina.nome`), venha ela do menu, dos favoritos ou da URL. `ROTA_TITULO_MAP` só é usado quando a rota não está no menu (R8). O título grande da página (`config.getTitulo()`) não muda (NB3). |

### 4.1 Como fica na tela Caixa/Banco

```
Lista:       Início › Gestão Financeira › Cadastros › Caixa/Conta
Formulário:  Início › Gestão Financeira › Cadastros › Caixa/Conta › Editar
              (link)   (texto)            (texto)     (link: voltar)
Aba:         [Caixa/Conta ×]      Título da página: Caixa / Banco
```

## 5. Plano de implementação (1 branch `feat/`, commits por passo)

1. **Função pura + testes (TDD):** `montarCaminhoRotina(modulos, rota)` →
   `{ modulo, menus[], rotina } | null`, aplicando R2, R3 e R8. Testes Vitest: rota encontrada,
   nível repetido omitido, rota ausente, rota duplicada (primeira vence), submenu em 2+ níveis.
2. **`TabsContext.irParaInicio()`** e `AppShell` mostrando o dashboard com `abaAtivaId === null`
   (R4); `TabBar` com o botão Início e sem destaque de aba nesse estado.
3. **`AppBreadcrumbs`** (R1, R4-R7, R9) consumindo `useCaminhoRotina()`, com a formatação de caixa alta (NB4).
4. Trocar o bloco fixo em **`CadastroBasePage`** e **`BaseCadastro`** pelo componente.
5. **Título da aba pelo menu (R10):** `AppShell` usa `montarCaminhoRotina(...).rotina.nome` ao
   abrir a aba pela URL; `ROTA_TITULO_MAP` fica como reserva.
6. `npm run build` + `npm test` verdes; teste manual nas 3 telas (§6).

## 6. Critérios de aceite

- **C1:** em `/financeiro/caixabanco` o breadcrumb mostra `Início › Gestão Financeira › Cadastros › Caixa/Conta`.
- **C2:** clicar em **Início** mostra o dashboard com a aba ainda aberta na barra; clicar na aba volta à tela com os dados como estavam.
- **C3:** nenhum clique no breadcrumb deixa `#` na URL.
- **C4:** no formulário (Novo/Editar), o nível da rotina volta para a lista, avisando se houver alterações não salvas.
- **C5:** Entidade e Condição de pagamento mostram o caminho real do menu (sem "Cadastros Base").
- **C6:** aba aberta pela URL e aba aberta pelo menu têm o mesmo título.
- **C7:** testes Vitest da função de caminho cobrindo R2, R3 e R8; `npm run build` e `npm test` verdes.

## 7. Fora de escopo

- Tela Parâmetros (não usa os componentes de breadcrumb).
- Sincronizar a URL ao trocar de aba (hoje a URL só muda no carregamento; pode ser outra spec).
- Mudar nomes de menu/rotina no banco.

## 8. Decisões (usuário, 2026-09-27)

- **NB1 — rótulo do primeiro nível:** **"Início"** (não "Dashboard") — diz o que o clique faz.
- **NB2 — clique no módulo:** **texto, não clicável** (ajuste da proposta original, que era
  selecionar o módulo na barra lateral). A troca de módulo continua pelo seletor do topo; só
  "Início" e "voltar para a lista" são clicáveis.
- **NB3 — nomes divergentes:** aba e breadcrumb com o **nome do menu** ("Caixa/Conta"); título da
  página com o **nome da tela** ("Caixa / Banco", como no legado). Unificar os nomes, se desejado,
  é alteração do cadastro de menu no banco — fora desta spec.
- **NB4 — maiúsculas:** exibir em formato de título ("Gestão Financeira") **só na exibição**,
  sem alterar o banco (as tabelas de menu são compartilhadas com o legado).

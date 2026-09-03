# Análise de Cobertura — MOD-05

> **Data:** 2026-09-03 | **Constituição:** v1.0 | **Escopo desta rodada:** SDD sem código
> (spec + clarify + plan + data-model + research + contracts + tasks).
>
> **Veredito parcial:** 🟡 **APROVADO PARA PLANEJAMENTO** — V1, V2, V6, V7 executados e
> sem achado CRÍTICO/ALTO. V3, V4, V5 **não executáveis nesta rodada** (dependem das
> matrizes RTV/ROT e do detalhamento de colunas, que são produzidos por épico nas tarefas
> `E?-T01` durante `/implement`).
>
> **Gate incremental por épico:** V3/V4/V5 **não** ficam represados até o fim. Assim que a
> tarefa `analysis` de um épico gera `matriz-rtv.md#E?` / `matriz-rot.md#E?`, roda-se
> `/analyze MOD-05 --epico E?` (modo incremental de `sdd-analyze §5`) e o veredito ✅ do
> épico é pré-condição para o código daquele épico entrar em `develop`. As rodadas ficam
> registradas na seção "Rodadas incrementais" abaixo.
>
> O **gate duro completo** (`sdd-analyze` modo completo) roda em `Z-T02` como
> **consolidação** — V1–V7 no módulo inteiro + verificação cross-épico — antes de qualquer
> merge final.

---

## Resumo

| Verificação | Resultado | Itens checados | Falhas |
| :--- | :--- | :--- | :--- |
| **V1** Constituição (gate §12) vs. plan/data-model/contracts/tasks | ✅ PASS | §12.1, §12.3, §12.4, §12.6 | 0 |
| **V2** Rastreabilidade spec → tasks (RN, épicos, contratos, entidades) | ✅ PASS | 20 RN, 15 épicos, 3 contratos, catálogo de 59 tabelas | 0 |
| **V3** Rastreabilidade legado → matrizes (varredura de palavras-chave) | ⏸️ ADIADA → gate incremental | — | — (roda em `/analyze --epico E?` por épico; consolida em `Z-T02`) |
| **V4** Matrizes → tasks (VAL/OP/CALC em ≥1 tarefa) | ⏸️ ADIADA → gate incremental | estrutura pronta (`Cobre:` em toda tarefa `service`/`operation`/`parity`/`frontend`) | — (idem V3) |
| **V5** Cobertura de propriedades (toda coluna com destino; NULL→anulável) | 🟡 PARCIAL → gate incremental | E-Caixa/E-Domínio/E-Documento núcleo detalhados; demais por épico | 0 (nos detalhados) |
| **V6** Cross-module e estrangulamento | ✅ PASS | 11 refs `int` lógico classificadas; estrangulamento reconciliado (CLR-10) | 0 |
| **V7** Coerência spec ↔ plan ↔ tasks; clarify sem pendência bloqueante | ✅ PASS | 15 épicos, escopo frontend, 13 CLR | 0 |

---

## V1 — Constituição (Gate §12)

| Item do gate | Resultado | Evidência |
| :--- | :--- | :--- |
| §12.1 `.csproj` novo = `net10.0` | ✅ | `plan.md §1.1` (SharedKernel, GestaoFinanceira, Tests todos `net10.0`); pacotes 10.x |
| §12.1 projeto = `Versatus.<Modulo>` | ✅ | `Versatus.GestaoFinanceira`, `Versatus.SharedKernel` |
| §12.3 `Domain/` sem DataAnnotations; Fluent API 1 arq./entidade | ✅ | `plan.md §1.2` (`Infrastructure/Mappings/`), `data-model.md` (nota reforçada: Fluent no Mapping, não na entidade), tarefas `E?-T03` |
| §12.3 Controllers sem `DbContext`/`IRepository`; sem DTO aninhado | ✅ | `contracts/README.md` (DTOs `sealed record` em `Domain/DTOs/`), tarefas `contract` |
| §12.3 sem classe `*Lista`; coleções Artigo V | ✅ | `spec.md §2.15` (41 `*Lista` eliminadas), tarefas `domain` (`List<T>` privada + `IReadOnlyList<T>`) |
| §12.2 PK `.ValueGeneratedNever()`; sem `IDENTITY` | ✅ | `data-model.md §1` + RN-05-006 + tarefas `E?-T03` |
| §12.2 comentário de origem em cada classe | ✅ | tarefas `domain` (padrão `acesso.global` `// Origem:` / `// Tabela:`) |
| §12.4 1 transação/Handler; `SaveChanges` só no Handler | ✅ | `plan.md §5` (11 operações), tarefas `operation` |
| §12.4 `GET` na ReadConnection; paginação materializada | ✅ | `contracts/README.md`, `S-T03`, tarefas `service`/`E13` |
| §12.4 cross-módulo = `int` lógico | ✅ | `dependency-graph.md §4`, `data-model.md §1` |
| §12.4 estrangulamento reconciliado | ✅ | `plan.md §4` (CLR-10 — nada a reconciliar) |
| §12.5 RTV 1 `[Fact]`/VAL; ROT 1 integração + 1 golden/OP; parity | ✅ (estrutura) | `plan.md §6`, tarefas `E?-T01` geram matrizes, tarefas `service`/`operation`/`parity` consomem |
| §12.6 branch `feat/`/`docs/`; nada em `develop` | ✅ | `tasks.md` (toda tarefa tem branch `feat/mod-05-*`); trabalho SDD em `docs/mod-05-sdd` |
| §12.6 skill nova → fonte + stub + manual | ✅ | Bloco 1 (8 skills + stubs + `MANUAL-SKILLS.md`) |
| §12.6 Log de Progresso previsto | ✅ | `Z-T03` (`/handoff`) |

**Nenhum FAIL.**

---

## V2 — Rastreabilidade descendente (spec → tasks)

- **RN-05-001..020:** todas citadas em ≥1 tarefa (`tasks.md` §"Resumo de cobertura").
  Spot-check: RN-05-001 (E1-T02, E4-T05, E6-T01); RN-05-011 (E9-T01/02/04);
  RN-05-016 (E14-T01/03/04); RN-05-020 (E1-T04, E4-T01/06, E6-T01/05).
- **Épicos `plan.md §3`:** E0(T01-04), E1(T01-04), E3(T01-08), E2(T01-07), E4(T01-10),
  E5(T01-08), E6(T01-08), E7(T01-05), E8(T01-04), E9(T01-06), E10(T01-05), E11(T01-05),
  E12(T01-05), E13(T01-03), E14(T01-07) — todos com tarefas.
- **Contratos:** `caixa-banco.md`→E3-T06; `documento.md`→E4-T08; `liquidacao.md`→E6-T07;
  demais criados na tarefa `contract` do épico.
- **Entidades (`data-model.md` catálogo):** toda tabela do catálogo tem tarefa `domain` +
  `dbcontext` no épico correspondente.

---

## V6 — Cross-module e estrangulamento

- 11 referências `int` lógico classificadas (`dependency-graph.md §4`): `IdEntidade`,
  `IdFilial`, `IdOperacao`, `IdTipoDocumento`, `IdCondicaoPagamento`, `IdIndiceEconomico`,
  `IdUsuario*`, `IdPortador`/`IdFormaCobranca`, `IdCentroCusto`/`IdClasse` (`IdFinClasse`),
  `IdBanco`, `IdOrigem`+`ProcessoOrigem`. Nenhuma navegação EF cross-projeto.
- **`FinClasse`/`FinRateio` são de `acesso.global` (MOD-02)** — confirmado (`Classe.cs`
  lá); no MOD-05 são `int` lógico.
- **Pré-requisito CLR-01:** `RateioMovtoItem`/`RateioMovto`/`ManutencaoRateio` migram no
  MOD-02 antes de `E5-T02` — marcado na abertura do épico E5 e em `plan.md §7 R-6`.
- **Estrangulamento (CLR-10):** `Servidor.Strangler/Gestao.Financeira/DTOs/` só expõe
  `EntidadeDto`/`ClienteDto`/`FornecedorDto`/`ParametroDto` (AcessoGlobal). Nenhum
  contrato financeiro. `plan.md §4` fecha o item.

---

## V7 — Coerência interna

- `spec.md §6` (14 épicos numerados) ↔ `plan.md §3` (15 passos de execução, ordem
  corrigida E3 antes de E2) ↔ `tasks.md` (fases S + E0..E14 + Z) — consistentes; a
  inversão E2/E3 está documentada em ambos (spec §6 nota + plan §3).
- Escopo de frontend (CLR-07): `spec.md §1.5` = E3, E4, E6, E9 núcleo ↔ `tasks.md` tem
  tarefa `frontend` só em E3-T07, E4-T09, E6-T08, E9-T06. ✅
- `clarify.md`: 13 CLR resolvidas, **0 pendência bloqueante** (declarado em `clarify.md`
  "Pendências remanescentes").

---

## Achados

| # | Severidade | Verificação | Descrição | Ação exigida |
| :--- | :--- | :--- | :--- | :--- |
| A-01 | **MÉDIO** | V5 | Banco de dev não contém `FINPROJECAOFLUXOCAIXA(+LACTO)` nem `FINLOGDOMINIOPERIODO` — `data-model.md` desses fica incompleto. | Tarefas `E2-T01` e `E11-T01` reconfirmam e, se ausente, solicitam dump de schema de produção **antes** do `domain` do épico. Já registrado (`plan.md §7 R-1`, `research.md §8`). |
| A-02 | **MÉDIO** | V5 | Maioria das tabelas `Fin*` vazia → golden values de cálculo sem amostra real. | Golden via execução do legado / conferência manual (`research.md §5`); DÚVIDA-R3 (base de homologação) aberta ao usuário — não bloqueia. |
| A-03 | **BAIXO** | V5 | `FINDRETITULOOPERACAO` referencia `IDFINDRETITULOCALCULO` sem tabela `FINDRETITULOCALCULO` no banco. | Classificar (enum vs. FK) na tarefa `E11-T01`. |
| A-04 | **BAIXO** | V2 | `tasks.md` usa `Cobre:` com `VAL-xx#E?`/`OP-xx#E?` que ainda não existem (criados nas tarefas `analysis`). | Esperado no SDD; `sdd-analyze` V3/V4 rodam por épico no **gate incremental** (`/analyze --epico E?`) e consolidam em `Z-T02`. |

Nenhum achado **CRÍTICO** ou **ALTO**.

---

## Rodadas incrementais (`/analyze MOD-05 --epico E?`)

Preenchida durante o `/implement`, uma linha por épico, à medida que a tarefa `analysis`
de cada épico gera suas matrizes.

| Épico | Data | Órfãs V3 | V4 OK? | V5 OK? | Veredito | Achados |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| E1 | — | — | — | — | ⏳ pendente | — |
| E3 | — | — | — | — | ⏳ pendente | — |
| E2 | — | — | — | — | ⏳ pendente | — |
| E4 | — | — | — | — | ⏳ pendente | — |
| E5 | — | — | — | — | ⏳ pendente | — |
| E6 | — | — | — | — | ⏳ pendente | — |
| E7 | — | — | — | — | ⏳ pendente | — |
| E8 | — | — | — | — | ⏳ pendente | — |
| E9 | — | — | — | — | ⏳ pendente | — |
| E10 | — | — | — | — | ⏳ pendente | — |
| E11 | — | — | — | — | ⏳ pendente | — |
| E12 | — | — | — | — | ⏳ pendente | — |
| E13 | — | — | — | — | ⏳ pendente | — |
| E14 | — | — | — | — | ⏳ pendente | — |

---

## Veredito

🟡 **APROVADO PARA PLANEJAMENTO / `/implement`** — a camada SDD do MOD-05 (constituição,
spec, clarificação, plano, modelo de dados, pesquisa, contratos, tarefas) está completa,
consistente e conforme a constituição v1.0. Nenhum bloqueio.

⛔ **Gate incremental por épico é obrigatório** — nenhum código de um épico entra em
`develop` sem `/analyze MOD-05 --epico E?` ✅ (V3/V4/V5 no escopo `#E?`).

⛔ **O gate duro completo (modo completo, V1–V7 + cross-épico) é obrigatório em `Z-T02`** —
antes de mesclar o fecho do módulo, `sdd-analyze` deve dar veredito ✅ verde
(Artigo IX; `sdd-analyze` §5–§6).

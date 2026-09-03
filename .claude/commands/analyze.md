---
description: Etapa 5 do SDD (gate duro) — cruza constitution ↔ spec ↔ plan ↔ tasks ↔ legado ↔ Matriz RTV ↔ Matriz ROT e reprova o módulo se alguma regra/validação/operação legada ficar órfã. Aceita `--epico E?` para o gate incremental.
argument-hint: MOD-XX [--epico E?] (ex. MOD-05 · MOD-05 --epico E4)
---

Módulo alvo: `$ARGUMENTS`

Dois modos (`sdd-analyze §5`):
- **Completo** (`MOD-XX`) — pré-condição: `spec.md`, `clarify.md`, `plan.md`
  (+ `research.md`, `data-model.md`, `contracts/`), `tasks.md` e **todas** as seções de
  `matriz-rtv.md` / `matriz-rot.md`. Roda V1–V7.
- **Incremental** (`MOD-XX --epico E?`) — pré-condição: `spec.md`, `plan.md`, `tasks.md` e
  `matriz-rtv.md#E?` / `matriz-rot.md#E?`. Roda V1/V3/V4/V5 restritos ao épico; registra a
  rodada em `analyze-report.md` ("Rodadas incrementais"). É pré-condição para o código do
  épico entrar em `develop`.

Dispare a skill **sdd-analyze** e siga `.agents/skills/sdd-analyze/SKILL.md` na íntegra.

Execute as verificações (V1–V7 no modo completo; V1/V3/V4/V5 no incremental):
- V1 Gate de Conformidade (Seção 12 da constituição) contra plan/data-model/contracts/tasks.
- V2 spec → tasks (RN, épicos, endpoints, entidades cobertos).
- V3 legado → matrizes (varredura por palavras-chave; ocorrência sem VAL-xx/OP-xx = órfã).
- V4 matrizes → tasks (toda VAL-xx/OP-xx em ≥1 tarefa; OP de cálculo tem tarefa `parity`).
- V5 cobertura de propriedades (toda coluna com destino; NULL → tipo anulável).
- V6 cross-module classificado; DTOs de `Servidor.Strangler` reconciliados.
- V7 coerência spec ↔ plan ↔ tasks; `clarify.md` sem pendência bloqueante.

Gere/atualize `specs/modulos/MOD-XX/analyze-report.md` com resumo, achados (severidade),
órfãs por categoria e **veredito** (no modo incremental, preencha a linha do épico em
"Rodadas incrementais"). Não corrija — aponte a ação exigida. `/implement` do épico é
proibido enquanto o veredito incremental for ⛔; o merge final, enquanto o completo for ⛔.
Commit `docs(mod-XX): relatório de análise de cobertura (SDD etapa 5)`.

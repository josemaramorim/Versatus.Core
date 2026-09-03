---
description: Etapa 4 do SDD — quebra o plano técnico aprovado em tarefas atômicas Git-safe (1 tarefa = 1 branch feat/ = 1 commit) em specs/modulos/MOD-XX/tasks.md.
argument-hint: MOD-XX (ex. MOD-05)
---

Módulo alvo: `$ARGUMENTS`

Pré-condição: `plan.md`, `research.md`, `data-model.md`, `contracts/` aprovados.

Dispare a skill **sdd-tasks** e siga `.agents/skills/sdd-tasks/SKILL.md` na íntegra.

Gere `specs/modulos/MOD-XX/tasks.md`:
- Tarefas na ordem topológica dos épicos; dentro do épico:
  `analysis → domain → dbcontext → service/operation → contract → parity → frontend → migration`.
- `analysis` das classes >1.500 linhas sempre antes do `domain` do épico.
- Cada tarefa: objetivo, arquivos legados de referência, arquivos a criar/alterar,
  `Cobre:` (RN-XX-NNN / VAL-xx / OP-xx — obrigatório e não-vazio para
  domain/service/operation/parity/frontend), `Constituição:` (Artigos), `Pronto quando:`
  (critério objetivo), branch `feat/mod-XX-<slug>`, mensagem de commit, `Depende de:`.
- IDs estáveis `E<n>-T<nn>`.

Verifique: toda `VAL-xx`/`OP-xx`/`RN-XX-NNN` aparece em ≥1 tarefa. Commit
`docs(mod-XX): quebra em tarefas atômicas (SDD etapa 4)`. Siga para `/analyze`.

---
description: Etapa 1 do SDD — gera a especificação de módulo (specs/modulos/MOD-XX/spec.md) a partir do legado, sem decisão técnica. Para tela CRUD isolada use a skill spec-generator.
argument-hint: MOD-XX (ex. MOD-05)
---

Módulo alvo: `$ARGUMENTS`

Dispare a skill **sdd-specify** e siga `.agents/skills/sdd-specify/SKILL.md` na íntegra.

Antes de qualquer coisa:
1. Leia `specs/memory/constitution.md` completa. Se ela não existir, rode `/constitution`
   primeiro.
2. Ritual de início da skill (Índice Geral, spec antiga do módulo, Log de Progresso,
   ALUCINACOES-DETECTADAS, `git branch --show-current`).
3. Trabalhe em `docs/mod-XX-sdd`.

Produza:
- `specs/modulos/MOD-XX/spec.md` (inventário de classes, árvore de herança, mapa de
  dependências, enums, épicos em ordem topológica, RN-XX-NNN macro, Seção 9 DÚVIDAS).
- `specs/modulos/MOD-XX/dependency-graph.md`.

Regras: zero decisão técnica (isso é `/plan`); nada inventado; escopo incerto vira
`DÚVIDA:`. Rode o Gate §12.2 da constituição. Commit `docs(mod-XX): spec de módulo (SDD
etapa 1)`, atualize `specs/00-INDICE-GERAL.md`, **PARE e peça aprovação**.

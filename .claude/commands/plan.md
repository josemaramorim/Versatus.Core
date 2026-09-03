---
description: Etapa 3 do SDD — transforma a spec de módulo aprovada em plan.md, research.md, data-model.md (colunas reais via INFORMATION_SCHEMA) e contracts/.
argument-hint: MOD-XX (ex. MOD-05)
---

Módulo alvo: `$ARGUMENTS`

Pré-condições: `spec.md` aprovada, `clarify.md` sem pendência bloqueante,
`specs/memory/constitution.md` lida.

Dispare a skill **sdd-plan** e siga `.agents/skills/sdd-plan/SKILL.md` na íntegra.

Produza em `specs/modulos/MOD-XX/`:
- `plan.md` — projeto `Versatus.<Modulo>` (net10.0), kernel compartilhado, épicos em ordem
  topológica com arquivos grandes a analisar, reconciliação com `Servidor.Strangler`,
  padrão de transação por operação, estratégia de testes, riscos.
- `research.md` — substitutos .NET 10 de libs legadas, APIs removidas, decisões de
  investigação (cada uma: problema → opções → decisão → impacto; sem decisão → `DÚVIDA:`).
- `data-model.md` — para cada entidade: tabela e colunas **reais** consultadas em
  `localhost\SQLEXPRESS2008 / versatus` via `INFORMATION_SCHEMA.COLUMNS` (nome, tipo, NULL,
  precision); mapa propriedade C# → coluna (coluna NULL → tipo anulável); PK
  `.ValueGeneratedNever()`; relacionamento intra-módulo vs. `int` lógico cross-módulo.
- `contracts/*.md` — endpoints REST por área (rota, verbo, request/response `record`,
  códigos, Read vs. Write connection, paginação materializada).

Cada decisão aponta a Seção da spec que a motiva. Rode o Gate §12.1/§12.3/§12.4. Commit
`docs(mod-XX): plano técnico, data-model e contratos (SDD etapa 3)`. **PARE e peça
aprovação.**

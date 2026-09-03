---
description: Etapa 2 do SDD — rodada estruturada de perguntas sobre as ambiguidades da spec de módulo, registrada em specs/modulos/MOD-XX/clarify.md e reincorporada na spec.
argument-hint: MOD-XX (ex. MOD-05)
---

Módulo alvo: `$ARGUMENTS`

Pré-condição: `specs/modulos/MOD-XX/spec.md` existe e foi aprovada.

Dispare a skill **sdd-clarify** e siga `.agents/skills/sdd-clarify/SKILL.md` na íntegra.

1. Levante perguntas a partir da Seção 9 (DÚVIDAS) da spec + inventário/dependências/épicos/
   enums/estrangulamento/kernel/infra-de-dados ambíguos.
2. Pergunte em blocos pequenos (≤5), com contexto, opções objetivas e impacto no plano.
   Use `AskUserQuestion` quando possível. Não presuma respostas.
3. Registre tudo em `specs/modulos/MOD-XX/clarify.md` (tabela com ID, origem, pergunta,
   opções, resposta+data, efeito na spec).
4. Aplique cada decisão em `spec.md` no mesmo commit; remova as DÚVIDAS resolvidas.
5. Commit `docs(mod-XX): rodada de clarificação (SDD etapa 2)`. **PARE**: `/plan` só roda
   sem pendência bloqueante e com confirmação do usuário.

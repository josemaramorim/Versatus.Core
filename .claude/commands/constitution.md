---
description: Gera ou atualiza a Constituição de Engenharia (specs/memory/constitution.md) consolidando AGENTS.md + Regras Anti-Alucinação + DEC-001..006 num gate executável do fluxo SDD.
argument-hint: (sem argumentos)
---

Dispare a skill **sdd-constitution** e siga `.agents/skills/sdd-constitution/SKILL.md` na
íntegra.

Passos:
1. Releia `.agents/AGENTS.md`, `specs/03-REGRAS-ANTI-ALUCINACAO.md`,
   `specs/decisoes/DEC-001..DEC-006` e `specs/04-CONTRATO-DA-IA.md` — sem confiar em
   memória.
2. Levante o estado real da plataforma (`grep TargetFramework`, versões de pacote em
   `Versatus.GestaoTributo.csproj`, convenção de nomes de projeto).
3. Gere/atualize `specs/memory/constitution.md` com a estrutura da skill (Seção 0
   Plataforma, Artigos I–XI com rastreio de origem, Seção 12 Gate de Conformidade, Seção
   13 Glossário).
4. Não invente cláusula. Contradição entre documentos ratificados → registre em
   `## Conflitos detectados` e reporte.
5. Trabalhe numa branch `docs/`. Commit `docs(sdd): cria/atualiza Constituição vX.Y`.
6. **PARE e peça aprovação do usuário.**

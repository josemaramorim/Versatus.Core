---
description: Etapa 6 do SDD — implementa UMA tarefa atômica de specs/modulos/MOD-XX/tasks.md (1 tarefa = 1 branch feat/ = 1 commit), disparando a skill certa conforme o tipo da tarefa.
argument-hint: MOD-XX <TaskID> (ex. MOD-05 S-T01 · MOD-05 E3-T05)
---

Alvo: `$ARGUMENTS` — o primeiro token é o módulo (`MOD-XX`), o segundo é o ID da tarefa
(`S-T01`, `E3-T05`, `Z-T02`…). Se faltar algum, **PARE e PERGUNTE**.

## Pré-condições (leia antes de tocar em código)

1. `specs/memory/constitution.md` (Seção 12 — Gate de Conformidade) e `.agents/AGENTS.md`.
2. `specs/modulos/MOD-XX/tasks.md` — localize a linha exata da tarefa. Extraia: **Objetivo**,
   **Cria/Altera**, **Cobre**, **Constituição** (Artigos), **Pronto quando**, **Branch**,
   **Commit**, **Depende de**, **tipo**.
3. `specs/modulos/MOD-XX/analyze-report.md` — confira o veredito:
   - Tarefas de tipo `domain` / `service` / `operation` / `frontend` de um épico `E?`:
     **proibido implementar** enquanto o gate incremental daquele épico
     (`/analyze MOD-XX --epico E?`) não estiver ✅. Se estiver ⛔ ou ausente, **PARE** e
     avise que a tarefa `analysis` do épico (`E?-T01`) precisa rodar antes.
   - Tarefas `setup` / `di` / `analysis` da Fase S e do E0 não dependem desse gate.
4. **Depende de:** confirme via `git log` que a(s) tarefa(s) predecessora(s) já foram
   commitadas. Se não, **PARE e PERGUNTE**.
5. Regras de negócio / colunas: nunca invente. Mapeamento físico só contra
   `INFORMATION_SCHEMA` / `legacy-schema/` (incidente 2026-06-06 em
   `specs/decisoes/ALUCINACOES-DETECTADAS.md`).

## Git (Artigo X da constituição / Lei 5)

- `git checkout develop && git pull` → `git checkout -b <Branch da tarefa>`.
  **Nunca** commitar/push em `develop`/`main` (há hook que bloqueia).
- Trabalhe só nos arquivos de **Cria/Altera** da tarefa. Nada de "melhorias" fora do escopo.
- Ao final: **1 commit atômico** com a mensagem **exata** do campo `Commit:` da tarefa, com o
  rodapé de atribuição padrão do repo.
- Não faça merge. Ao concluir, **PERGUNTE** ao usuário se quer abrir PR / mesclar.

## Despache a skill conforme `tipo`

| tipo | Skill / ação |
|---|---|
| `setup`, `di` | `create-api-module` quando for scaffold de módulo; senão implementação direta (csproj, pastas, DI, registro na WebAPI) conforme `plan.md`. |
| `analysis` | `legacy-validation-audit` (Matriz RTV) + `legacy-operation-audit` (Matriz ROT); + `legacy-calc-parity` se a tarefa cita `CALC-xx`. Gera as seções `matriz-rtv.md#E?` / `matriz-rot.md#E?` e roda `/analyze MOD-XX --epico E?` ao final (critério de "Pronto quando"). |
| `domain` | Implementação direta: POCOs puras em `Domain/` (Artigo III — sem DataAnnotations) + mappings Fluent API em `Infrastructure/Mappings/` (nomes/tipos/nulidade do extract real). |
| `dbcontext` | Registrar DbSets; `test-driven-development` para o teste InMemory de mapeamento (PK composta, materialização). |
| `service` | `test-driven-development`: 1 `[Fact]` por `VAL-xx` do `Cobre:` (Matriz RTV) + serviço/repositório CQRS (`Context` escrita / `ReadContext` leitura; paginação materializada). |
| `operation` | `test-driven-development` + `legacy-operation-audit` como referência: 1 handler = 1 transação; ordem de persistência da `matriz-rot.md`; teste de integração + rollback por `OP-xx`. |
| `parity` | `legacy-calc-parity`: transcrever a fórmula **sem refatorar** (Regra 5) + golden tests com igualdade exata de `decimal` (`golden/CALC-*.csv`). |
| `contract` | Implementação direta: controllers finos (Artigo IV / Regra 17 — sem repositório no controller), DTOs `record`, `Result<T>` → 400; atualizar `contracts/`. |
| `frontend` | `spec-generator` (completa `docs/spec_f<nome>.md`, classifica Padrão A/B/C) + `migrate-crud`. `schema.test.ts` (Vitest) por regra RTV; `required` nos obrigatórios (Lei 10). |
| `migration` | `dotnet ef migrations add` no DbContext do módulo; validar contra o banco real (schema-first — não recria tabelas). |

## Pronto quando (sempre, além do critério específico da tarefa)

- Backend: `dotnet build` **0 erro / 0 aviso** (Lei 6) e `dotnet test` verde. Matar
  `dotnet run` antes de buildar.
- Frontend: `npm run build` e `npm test` verdes.
- Todo item de `Cobre:` da tarefa tem teste correspondente.
- Só então o commit. Depois, sugerir rodar `/handoff` se a sessão for encerrar.

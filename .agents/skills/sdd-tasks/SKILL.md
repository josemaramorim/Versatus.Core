---
name: sdd-tasks
description: Quebra o plano técnico aprovado em tarefas atômicas Git-safe (1 tarefa = 1 branch feat/ = 1 commit), cada uma ligada 1:1 a linhas da spec / Matriz RTV / Matriz ROT. Gera specs/modulos/MOD-XX/tasks.md. Etapa 4 do fluxo SDD.
---

# Skill: sdd-tasks

Etapa 4 do fluxo SDD. Converte `plan.md` numa lista de tarefas executáveis por qualquer
IA/dev, cada uma pequena o suficiente para caber em um commit e rastreável até a regra que
a justifica. É a versão estruturada dos `specs/prompts-execucao/*` que o projeto já fazia
à mão.

---

## 1. Pré-condição

`plan.md`, `research.md`, `data-model.md`, `contracts/` aprovados pelo usuário.

---

## 2. Granularidade

Uma tarefa entrega **uma coisa verificável**. Padrões típicos:

| Tipo | Exemplo | Critério de pronto |
| :--- | :--- | :--- |
| `setup` | criar projeto `Versatus.<Modulo>` + adicionar à solução | `dotnet build` 0/0 |
| `analysis` | analisar `LiquidacaoEstorno.cs` (2.617 l) → doc de comportamento | doc revisável commitado |
| `domain` | entidades de um épico + mappings Fluent API | build + teste de mapeamento InMemory |
| `dbcontext` | registrar DbSets do épico no `<Modulo>DbContext` + Read | build |
| `service` | `I<Nome>Service` + impl + testes RTV (1 `[Fact]`/linha VAL) | `dotnet test` verde |
| `operation` | Handler transacional + testes ROT (integração + golden) | `dotnet test` verde |
| `contract` | Controller fino + endpoints do `contracts/<area>.md` | build; retornos `Result<T>`→400 |
| `parity` | golden tests de cálculo (skill legacy-calc-parity) | valores batem com o legado |
| `frontend` | `src/pages/<Modulo>/F<Nome>/` + `schema.test.ts` (só épicos de núcleo) | `npm run build` + `npm test` |
| `migration` | migration EF Core do épico | migration aplica/valida |
| `di` | `ServiceCollectionExtensions.Add<Modulo>()` + registro no `Program.cs` | build |

Se uma tarefa não cabe num commit lógico, quebre.

---

## 3. Ordem

1. Segue a ordem topológica dos épicos do `plan.md`.
2. Dentro do épico: `analysis` → `domain` → `dbcontext` → `service`/`operation` →
   `contract` → `parity` → `frontend` → `migration`.
3. `analysis` das classes grandes (>1.500 linhas) **sempre** antes do `domain` do épico.
4. Épico de integração de terceiros por último.

---

## 4. Arquivo `specs/modulos/MOD-XX/tasks.md`

```markdown
# Tarefas — MOD-XX

> 1 tarefa = 1 branch `feat/mod-XX-<slug>` a partir de `develop` = commits atômicos.
> Nunca commitar em `develop`/`main`. Ao concluir a branch, PERGUNTAR antes de sugerir merge.

## Épico E1 — <nome>

### E1-T01 · <título> · tipo: domain
- **Objetivo:** ...
- **Arquivos legados de referência:** projeto_tag_1906/.../X.cs
- **Cria/altera:** src/Versatus.<Modulo>/Domain/<area>/X.cs, .../Infrastructure/Mappings/XMapping.cs
- **Cobre:** RN-XX-003; VAL-12, VAL-13 (matriz-rtv.md); OP-04 (matriz-rot.md)
- **Constituição:** Artigo III (POCO), Artigo II.5 (ValueGeneratedNever)
- **Pronto quando:** `dotnet build` 0/0 e teste de mapeamento InMemory passa
- **Branch:** feat/mod-XX-e1-x-entity
- **Commit:** feat(mod-XX): entidade X e mapping (E1-T01)
- **Depende de:** E1-T00
```

Regras:
- **`Cobre:`** é obrigatório e não pode ser vazio para tarefas `service`/`operation`/
  `domain`/`parity`/`frontend`. Toda linha `VAL-xx` e `OP-xx` das matrizes tem que
  aparecer em pelo menos uma tarefa — o `/analyze` verifica isso.
- **`Constituição:`** cita os Artigos que a tarefa precisa respeitar.
- **`Pronto quando:`** critério objetivo e executável.
- IDs estáveis (`E<n>-T<nn>`); não renumerar depois.

---

## 5. Saída e gate

- `specs/modulos/MOD-XX/tasks.md`.
- Verifique cobertura: toda `VAL-xx`/`OP-xx`/`RN-XX-NNN` referenciada em ≥1 tarefa.
- Commit: `docs(mod-XX): quebra em tarefas atômicas (SDD etapa 4)`.
- Siga direto para `/analyze` (não precisa de aprovação aqui, mas `/analyze` é gate duro).

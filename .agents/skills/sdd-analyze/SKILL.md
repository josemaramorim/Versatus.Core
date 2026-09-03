---
name: sdd-analyze
description: Auditoria de consistência e cobertura total do fluxo SDD — cruza constitution ↔ spec ↔ plan ↔ tasks ↔ código legado ↔ Matriz RTV ↔ Matriz ROT e reprova o módulo se qualquer regra, validação ou operação legada ficar órfã. Etapa 5 do fluxo SDD, gate duro antes de /implement.
---

# Skill: sdd-analyze

Etapa 5 e **gate duro** do fluxo SDD. É a prova de que a conversão será *fiel*: nenhuma
regra, validação, operação ou campo do legado pode existir sem um destino planejado e uma
tarefa que o cubra. Não gera código; produz um relatório com veredito.

---

## 1. Pré-condições

- **Modo completo** (`/analyze MOD-XX`): `spec.md`, `clarify.md`, `plan.md`
  (+ `research.md`, `data-model.md`, `contracts/`), `tasks.md` e **todas** as seções de
  `matriz-rtv.md` / `matriz-rot.md` existem.
- **Modo incremental** (`/analyze MOD-XX --epico E?`): `spec.md`, `plan.md`, `tasks.md` e
  as seções `matriz-rtv.md#E?` / `matriz-rot.md#E?` **daquele épico** existem. Roda
  V1/V3/V4/V5 restritos ao escopo do épico; V2/V6/V7 ficam para o modo completo.

---

## 2. Verificações (todas obrigatórias)

### V1 — Constituição
Rodar o **Gate de Conformidade** (Seção 12 de `constitution.md`) contra `plan.md`,
`data-model.md`, `contracts/` e `tasks.md`. Qualquer `FAIL` = módulo reprovado.

### V2 — Rastreabilidade descendente (spec → tasks)
Toda `RN-XX-NNN` da spec aparece em ≥1 linha `Cobre:` de `tasks.md`.
Todo épico do `plan.md` tem tarefas. Todo endpoint de `contracts/` tem tarefa `contract`.
Toda entidade de `data-model.md` tem tarefa `domain` + `dbcontext`.

### V3 — Rastreabilidade ascendente (legado → matrizes)
Varrer o código legado do módulo (incluindo classes-pai e do `Geral`) por:
`Validar`, `Validating`, `MessageBox`, `ErrorProvider`, `throw`, `Cancel = true`,
`SetError`, `OnBefore*`, `OnAfter*`, `Executar*`, `Persistir*`, `Estornar*`, `Reverter*`,
`Confirmar*`, `Cancelar*`, `Fechar*`, `Calcular*`, `Ratear*`.
Cada ocorrência relevante tem que ter linha correspondente em `matriz-rtv.md` (validação)
ou `matriz-rot.md` (operação/transação/cálculo). **Ocorrência sem linha = órfã.**

### V4 — Matrizes → tasks
Toda `VAL-xx` de `matriz-rtv.md` e toda `OP-xx` de `matriz-rot.md` referenciada em ≥1
tarefa. Toda `OP-xx` de cálculo tem tarefa `parity`.

### V5 — Cobertura de propriedades
Toda propriedade de cada entidade legada do escopo tem destino em `data-model.md`
(propriedade C# + coluna). Nenhuma coluna `NULL` mapeada como tipo não-anulável.
Nenhum campo legado silenciosamente descartado (Regra 4 — se descartado, tem que estar
escrito e justificado na spec).

### V6 — Cross-module e estrangulamento
Toda dependência cross-módulo do mapa está classificada (`int` lógico / chamada de serviço
/ fora de escopo). Se há `Servidor.Strangler.<Modulo>`, `plan.md §4` reconciliou todos os
DTOs.

### V7 — Coerência interna
`spec.md` ↔ `plan.md` ↔ `tasks.md` sem contradição (contagem de épicos, nomes, escopo de
frontend). `clarify.md` sem pendência marcada `bloqueante`.

---

## 3. Relatório `specs/modulos/MOD-XX/analyze-report.md`

```markdown
# Análise de Cobertura — MOD-XX

> Data · Constituição vX.Y · Veredito: ✅ APROVADO | ⛔ REPROVADO

## Resumo
| Verificação | Resultado | Itens checados | Falhas |
|-------------|-----------|----------------|--------|
| V1 Constituição | ... | ... | ... |
| ... | | | |

## Achados (um por linha, severidade CRÍTICO/ALTO/MÉDIO)
| # | Severidade | Verificação | Descrição | Arquivo legado:linha | Ação exigida |
|---|-----------|-------------|-----------|----------------------|--------------|
| A-01 | CRÍTICO | V3 | `DocumentoParcela.cs:812` valida vencimento < emissão e não há VAL-xx | ... | criar VAL-47 + tarefa |

## Órfãs por categoria
- Validações legadas sem VAL-xx: <lista arquivo:linha>
- Operações legadas sem OP-xx: <lista>
- Propriedades sem destino em data-model: <lista>

## Veredito
⛔ REPROVADO enquanto houver achado CRÍTICO ou ALTO em aberto.
```

---

## 4. Regras

1. **Sem veredito verde com achado CRÍTICO/ALTO aberto.** MÉDIO pode ser aceito pelo
   usuário explicitamente (registrar quem aceitou e quando).
2. `/analyze` **não corrige** — aponta e diz a ação exigida. A correção volta pra
   `/specify` (regra nova), `/clarify` (ambiguidade) ou `/tasks` (cobertura).
3. Rode `/analyze` de novo após cada correção até ✅.
4. `/implement` é **proibido** enquanto o veredito for ⛔.

---

## 5. Gate incremental por épico

Módulos grandes produzem `matriz-rtv.md#E?` / `matriz-rot.md#E?` **por épico**, dentro do
`/implement`. Para não deixar a única passada de V3/V4/V5 para o fim (retrabalho tardio):

1. Assim que a tarefa `analysis` de um épico gera as matrizes `#E?`, rode
   `/analyze MOD-XX --epico E?` **antes** de qualquer código daquele épico entrar em
   `develop`.
2. O modo incremental reprova o épico (⛔) se houver `VAL`/`OP`/`CALC` órfã no escopo `#E?`;
   a implementação do épico não avança enquanto não ficar ✅.
3. O **modo completo** em `Z-T02` deixa de ser a primeira execução de V3/V4/V5 e passa a
   ser **consolidação**: re-roda V1–V7 no módulo inteiro e pega o que é cross-épico
   (regra que só aparece na junção de dois épicos, propriedade compartilhada, etc.).

Registrar cada rodada incremental no `analyze-report.md` (seção "Rodadas incrementais").

---

## 6. Saída

- `specs/modulos/MOD-XX/analyze-report.md`.
- Commit: `docs(mod-XX): relatório de análise de cobertura (SDD etapa 5)`.
- Reporte o veredito e a lista de achados ao usuário.

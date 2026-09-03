---
name: sdd-plan
description: Transforma a spec de módulo aprovada num plano técnico — plan.md (épicos em ordem topológica), research.md (libs, estrangulamento, kernel), data-model.md (tabelas/colunas reais via INFORMATION_SCHEMA) e contracts/ (endpoints REST). Etapa 3 do fluxo SDD.
---

# Skill: sdd-plan

Etapa 3 do fluxo SDD. É **aqui** que entram as decisões técnicas — todas ancoradas na
`spec.md` aprovada, na `constitution.md` e no banco/legado reais. Nada de escopo novo:
se o plano precisa de algo que não está na spec, isso é `DÚVIDA:` / volta pro `/clarify`.

---

## 1. Pré-condições

- `spec.md` aprovada, `clarify.md` sem pendência bloqueante.
- `specs/memory/constitution.md` lida (Seção 0 Plataforma + todos os Artigos).
- Acesso ao banco legado confirmado no `/clarify` (senão, `data-model.md` marca tudo
  `// TODO confirmar` e registra o risco).

---

## 2. Artefatos gerados

### 2.1 `specs/modulos/MOD-XX/plan.md`

```markdown
# Plano Técnico — MOD-XX

## 1. Projeto e estrutura
- Projeto novo: Versatus.<Modulo> (net10.0) + tests/Versatus.<Modulo>.Tests (net10.0)
- Referências: Versatus.Framework, Versatus.AcessoGlobal, <outras já existentes>
- Estrutura de pastas Domain/ (subpastas por área) · Infrastructure/ (Mappings, Repositories,
  <Modulo>DbContext, <Modulo>ReadDbContext) · Api/Controllers · DependencyInjection

## 2. Kernel compartilhado
- Tipos de Projeto.Geral a portar e destino (projeto novo Versatus.SharedKernel OU
  arquivos novos em Versatus.Framework — decisão do /clarify).
- Cada tipo: origem legada, o que se mantém, o que se descarta.

## 3. Épicos em ordem de execução
Tabela por épico: nº · nome · classes-núcleo · depende de · entidades de domínio ·
arquivos grandes a analisar antes (com skill: legacy-validation-audit /
legacy-operation-audit / legacy-calc-parity) · tem frontend? · nº estimado de tarefas.
Ordem topológica: bases → agregados → operações → consultas → integrações externas.

## 4. Reconciliação com o estrangulamento
Para cada DTO de Servidor.Strangler.<Modulo>: campo a campo vs. domínio novo; divergências
e como resolver sem quebrar o contrato que o legado já consome.

## 5. Padrão de transação por operação
Para cada operação transacional do módulo (Liquidar, Estornar, Reverter, Fechar…):
Handler, entidades tocadas, ordem de persistência, ponto de commit, condição de rollback.
(Alimentado pela Matriz ROT.)

## 6. Estratégia de testes
- Projeto de testes, pacotes, uso de InMemory (mapeamento) vs. SQL real (paridade).
- Como os golden values serão capturados do legado.

## 7. Riscos e mitigações
```

### 2.2 `specs/modulos/MOD-XX/research.md`
Decisões de investigação: substitutos .NET 10 para libs legadas (ex.: `BoletoNet`,
`BarcodeLib`, OFX), APIs removidas do .NET e adaptação mínima (Regra 5 exceção),
comportamento de `GeradorSequencialService`, cache de `Lookup`. Cada item: problema →
opções → decisão → impacto. Sem decisão → `DÚVIDA:`.

### 2.3 `specs/modulos/MOD-XX/data-model.md`
Para **cada** entidade que será criada:

- Tabela legada real e cada coluna: nome exato, tipo SQL, **NULL?**, PK/FK, precision de
  decimais.
- Obter via inspeção do banco (`localhost\SQLEXPRESS2008` / `versatus`):
  ```sql
  SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, NUMERIC_PRECISION, NUMERIC_SCALE, CHARACTER_MAXIMUM_LENGTH
  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '<Tabela>' ORDER BY ORDINAL_POSITION;
  ```
  Materializar o primeiro registro de cada tabela também é recomendado (detecta NULL de
  fato em colunas legadas).
- Mapa propriedade C# → coluna, com tipo .NET (coluna `NULL` → tipo anulável **sempre**,
  Artigo III.4).
- PK: confirmar composta quando for o caso; sempre `.ValueGeneratedNever()`.
- Relacionamentos intra-módulo (declarar `HasOne`) vs. cross-módulo (`int` lógico, **sem**
  navegação — Artigo VIII.1).
- Auditoria: prefixo de coluna real do módulo (`IdGlo...`, `IdFin...`, `IdPrc...`).

### 2.4 `specs/modulos/MOD-XX/contracts/`
Um arquivo `.md` por área/épico com os endpoints REST: rota, verbo, request `record`,
response `record`, códigos (`200/201/400`), regra CQRS (Read vs. Write connection),
paginação (materializa antes de `Skip/Take`). Consultas legadas de `Consultas/` viram
endpoints `GET` ou query-methods aqui.

---

## 3. Regras

1. **Ancoragem:** cada decisão do plano aponta a Seção da `spec.md` que a motiva. Decisão
   sem âncora = escopo novo = `DÚVIDA:`.
2. **Constituição manda:** Seção 0 fixa `net10.0`, EF Core 10.x, nome `Versatus.<Modulo>`.
   Não replicar versões antigas de docs (ex.: `net8.0` de prompts do MOD-03).
3. **Nomes reais:** `data-model.md` não chuta coluna. Sem banco → `// TODO confirmar` +
   risco na Seção 7 do `plan.md`.
4. **`*Lista` não entram** em lugar nenhum do plano.
5. **Integrações de terceiros** ficam num épico final isolado.

---

## 4. Saída e gate

- `plan.md`, `research.md`, `data-model.md`, `contracts/*.md`.
- Rode o Gate de Conformidade §12.1, §12.3, §12.4 da constituição.
- Commit: `docs(mod-XX): plano técnico, data-model e contratos (SDD etapa 3)`.
- **PARE e peça aprovação do usuário.** `/tasks` só roda após "Aprovado".

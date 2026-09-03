# Manual e Guia Prático de Skills dos Agentes de IA — Versatus ERP

> **Regra de Manutenção Obrigatória (`AGENTS.md`):** Toda criação, alteração ou remoção de skills no diretório `.agents/skills/` DEVE obrigatoriamente ser refletida e atualizada neste manual.

---

## 📌 Índice Geral

1. [Visão Geral e Conceitos](#1-visão-geral-e-conceitos)
2. [Skill 1: `migrate-crud`](#2-skill-1-migrate-crud)
   - [Propósito](#propósito-migrate-crud)
   - [Quando Usar](#quando-usar-migrate-crud)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-migrate-crud)
3. [Skill 2: `spec-generator`](#3-skill-2-spec-generator)
   - [Propósito](#propósito-spec-generator)
   - [Quando Usar](#quando-usar-spec-generator)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-spec-generator)
4. [Skill 3: `code-auditor`](#4-skill-3-code-auditor)
   - [Propósito](#propósito-code-auditor)
   - [Quando Usar](#quando-usar-code-auditor)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-code-auditor)
5. [Skill 4: `create-api-module`](#5-skill-4-create-api-module)
   - [Propósito](#propósito-create-api-module)
   - [Quando Usar](#quando-usar-create-api-module)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-create-api-module)
6. [Skill 5: `test-driven-development`](#6-skill-5-test-driven-development)
   - [Propósito](#propósito-test-driven-development)
   - [Quando Usar](#quando-usar-test-driven-development)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-test-driven-development)
7. [Skill 6: `writing-skills`](#7-skill-6-writing-skills)
   - [Propósito](#propósito-writing-skills)
   - [Quando Usar](#quando-usar-writing-skills)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-writing-skills)
8. [Skill 7: `legacy-validation-audit`](#8-skill-7-legacy-validation-audit)
   - [Propósito](#propósito-legacy-validation-audit)
   - [Quando Usar](#quando-usar-legacy-validation-audit)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-legacy-validation-audit)
9. [Skill 8: `project-analyzer`](#9-skill-8-project-analyzer)
   - [Propósito](#propósito-project-analyzer)
   - [Quando Usar](#quando-usar-project-analyzer)
   - [Exemplo de Uso Prático](#exemplo-de-uso-prático-project-analyzer)
10. [Skill 9: `sdd-constitution`](#10-skill-9-sdd-constitution)
11. [Skill 10: `sdd-specify`](#11-skill-10-sdd-specify)
12. [Skill 11: `sdd-clarify`](#12-skill-11-sdd-clarify)
13. [Skill 12: `sdd-plan`](#13-skill-12-sdd-plan)
14. [Skill 13: `sdd-tasks`](#14-skill-13-sdd-tasks)
15. [Skill 14: `sdd-analyze`](#15-skill-14-sdd-analyze)
16. [Skill 15: `legacy-operation-audit`](#16-skill-15-legacy-operation-audit)
17. [Skill 16: `legacy-calc-parity`](#17-skill-16-legacy-calc-parity)
18. [Boas Práticas de Manutenção do Manual](#18-boas-práticas-de-manutenção-do-manual)

---

## 1. Visão Geral e Conceitos

As **Skills** são capacidades especializadas e sequências de instruções padronizadas armazenadas na pasta `.agents/skills/`. Elas garantem que a migração de formulários, auditorias de código e criação de novos módulos no **Versatus ERP** sigam rigorosamente as premissas de **SOLID, Clean Architecture, Clean Code, Result Pattern, CQRS DB Split e .NET 10**.

### 1.1 Duas pastas, uma fonte da verdade

- **`.agents/skills/<nome>/SKILL.md`** — conteúdo completo e autoritativo de cada skill (o que este manual descreve). Usado por qualquer IA a quem o conteúdo for colado ou indicado manualmente (ex.: Gemini CLI).
- **`.claude/skills/<nome>/SKILL.md`** — apenas um *stub* de descoberta: mesmo frontmatter (`name`/`description`) da skill original, e um corpo curto instruindo a ler o arquivo completo em `.agents/skills/`. É essa pasta que o Claude Code varre automaticamente para decidir quando disparar uma skill sozinho, sem precisar que o conteúdo seja colado no chat.
- **Regra de manutenção (Lei 12 do `AGENTS.md`):** toda skill criada, alterada ou removida em `.agents/skills/` precisa: (1) manter este manual atualizado, e (2) ter seu stub em `.claude/skills/` criado/atualizado/removido em conjunto, com o frontmatter idêntico ao da fonte.

### 1.2 Fluxo SDD (Spec-Driven Development) e a fronteira com as skills de tela isolada

O projeto adota o fluxo **SDD** para trabalho em **escopo de módulo** (dezenas/centenas de classes legadas, épicos, ordem de dependência). Ele é a evolução formal do que o repositório já fazia com `specs/modulos/` + `specs/prompts-execucao/` + Matriz RTV.

| Etapa | Comando | Skill | Artefato | Gate |
|---|---|---|---|---|
| 0 | `/constitution` | `sdd-constitution` | `specs/memory/constitution.md` | 1x no repo |
| 1 | `/specify MOD-XX` | `sdd-specify` | `specs/modulos/MOD-XX/spec.md` + `dependency-graph.md` | ⛔ aprovação do usuário |
| 2 | `/clarify MOD-XX` | `sdd-clarify` | `clarify.md` (+ edições na spec) | ⛔ respostas do usuário |
| 3 | `/plan MOD-XX` | `sdd-plan` | `plan.md`, `research.md`, `data-model.md`, `contracts/` | ⛔ aprovação do usuário |
| 4 | `/tasks MOD-XX` | `sdd-tasks` | `tasks.md` (1 tarefa = 1 branch `feat/` = 1 commit) | revisão |
| 5 | `/analyze MOD-XX` | `sdd-analyze` | `analyze-report.md` — cobertura 100% vs. legado | ⛔ veredito verde |
| 6 | `/implement MOD-XX <tarefa>` | `migrate-crud` / `create-api-module` / `test-driven-development` | código | — |

Auditorias de apoio, disparadas dentro das etapas 1–3: `legacy-validation-audit` (Matriz RTV — validações), `legacy-operation-audit` (Matriz ROT — operações/transações/máquina de estados), `legacy-calc-parity` (golden tests de cálculo).

**Fronteira:**
- **Escopo de módulo** → fluxo SDD (`sdd-*`).
- **Uma tela CRUD isolada** dentro de um módulo já planejado → `spec-generator` + `migrate-crud` como sempre (essas skills são acionadas por `/implement` ou diretamente).
- Não rode `sdd-specify` para uma única tela, nem `spec-generator` para um módulo inteiro.

---

## 2. Skill 1: `migrate-crud`

<a id="propósito-migrate-crud"></a>
### 🎯 Propósito
Guia o pipeline completo de migração de formulários do ERP legado (WinForms/Delphi) para a stack moderna (.NET 10 + React OOP). O pipeline divide-se em 4 fases: Spec, Backend C#, Frontend React e Integração de Rotas.

<a id="quando-usar-migrate-crud"></a>
### 📅 Quando Usar
Quando for migrar qualquer formulário desktop do sistema legado para o novo sistema web.

<a id="exemplo-de-uso-prático-migrate-crud"></a>
### 💡 Exemplo de Uso Prático

Para utilizar esta skill, copie e preencha o prompt genérico oficial mantido em [`docs/PROMPT-MIGRACAO-LEGADO.md`](file:///c:/Pasta%20de%20Trabalho/Projetos/Analises/Versatus/Versatus.Net8/docs/PROMPT-MIGRACAO-LEGADO.md):

#### Prompt no Chat da IA:
```markdown
Você é um agente especializado na migração do sistema legado Versatus (Delphi/C#) para a arquitetura moderna .NET 10 + React/TypeScript.

## 1. LEITURA OBRIGATÓRIA ANTES DE QUALQUER AÇÃO
Leia os seguintes arquivos nesta ordem antes de escrever qualquer código ou spec:
1. `.agents/AGENTS.md` — Regras e leis do projeto (SOLID, Clean Arch, CQRS, Result Pattern)
2. `.agents/skills/migrate-crud/SKILL.md` — Pipeline de migração completo (4 fases)
3. `docs/spec_fentidade.md` — Template padrão de Spec Funcional
4. `specs/00-INDICE-GERAL.md` — Índice geral dos módulos já mapeados
5. `specs/03-REGRAS-ANTI-ALUCINACAO.md` — Regras de fidelidade ao legado

## 2. CONTEXTO DO PROJETO E PREMISSAS ARQUITETURAIS
- **Módulo Target:** AcessoGlobal
- **Formulário Target:** FCondicaoPagamento
- **Branch atual:** develop

## 3. ARQUIVOS LEGADOS PARA ANÁLISE
- **3a. Objeto de Negócio:** `projeto_tag_1906/.../CondicaoPagamento.cs`
- **3b. Formulário Legado:** `projeto_tag_1906/.../FCondicaoPagamento.cs`
- **3c. Tabelas do Banco:** `GloCondicaoPagamento`, `GloCondicaoPagtoRegra`
```

#### O que a IA fará automaticamente:
1. Classificará a tela como Padrão A (CRUD Padrão) ou Padrão B (Lote).
2. Gerará a Spec Funcional em `docs/spec_fcondicaopagamento.md`.
3. Aguardará sua aprovação da Spec.
4. Após aprovação, gerará DTOs, Entidade POCO, Fluent API Mappings, Service (com CQRS Read/Write) e Controller C#.
5. Gerará a página React em `src/pages/AcessoGlobal/FCondicaoPagamento/` estendendo `BaseCadastroConfig<T>`.
6. Integrará as rotas no `App.tsx` e menu lateral.

---

## 3. Skill 2: `spec-generator`

<a id="propósito-spec-generator"></a>
### 🎯 Propósito
Realiza uma análise minuciosa dos arquivos legados (`.cs` de negócio e `.cs` de formulário) e das tabelas do banco para gerar a Especificação Funcional padronizada em `docs/spec_f[nome].md`.

<a id="quando-usar-spec-generator"></a>
### 📅 Quando Usar
Antes de escrever qualquer linha de código C# ou React, garantindo a abordagem **Spec-First** exigida pelo projeto.

<a id="exemplo-de-uso-prático-spec-generator"></a>
### 💡 Exemplo de Uso Prático

#### Prompt no Chat da IA:
```
Usar a skill spec-generator para analisar a entidade FProduto no módulo Estoque e gerar a spec docs/spec_fproduto.md.
```

#### O que a IA fará automaticamente:
1. Mapeará **todas** as propriedades e suas nulidades (colunas que aceitam NULL no banco viram `int?`, `string?`).
2. Classificará a tela (Padrão A ou B).
3. Elaborará o documento em `docs/spec_fproduto.md` contendo as 8 seções padrão (Requisitos Clean Arch, Endpoints, Mapeamento MUI `required`, Critérios de Aceite).
4. Apresentará a spec e aguardará sua aprovação.

---

## 4. Skill 3: `code-auditor`

<a id="propósito-code-auditor"></a>
### 🎯 Propósito
Skill de auditoria de qualidade que inspeciona o código C# e React de um formulário migrado para garantir 100% de conformidade com as **12 leis do `AGENTS.md`**.

<a id="quando-usar-code-auditor"></a>
### 📅 Quando Usar
Antes de realizar o merge de uma branch de recurso (`feat/migrate-[nome]`) para a branch `develop`.

<a id="exemplo-de-uso-prático-code-auditor"></a>
### 💡 Exemplo de Uso Prático

#### Prompt no Chat da IA:
```
Executar a skill code-auditor para auditar o formulário FEntidade do módulo AcessoGlobal.
```

#### O que a IA fará automaticamente:
1. Verificar se o domínio tem POCOs puras (sem DataAnnotations).
2. Checar se o Result Pattern é usado (sem `throw Exception()`).
3. Confirmar se a busca usa a réplica de leitura (`ReadContext` / `ReadConnection` com `NoTracking`).
4. Checar se a prop `required` está presente nos campos obrigatórios do MUI.
5. Garantir que nenhuma propriedade editável do DTO ficou fora da UI.
6. Gerar um Relatório de Auditoria detalhado apontando conformidades e inconsistências.

---

## 5. Skill 4: `create-api-module`

<a id="propósito-create-api-module"></a>
### 🎯 Propósito
Guia a criação completa de um novo módulo de negócio no backend C# (ex: `Versatus.Estoque`, `Versatus.Financeiro`) seguindo a estrutura padrão de pastas em Clean Architecture, SOLID e CQRS DB Split.

<a id="quando-usar-create-api-module"></a>
### 📅 Quando Usar
Quando for criar uma nova solução/projeto de módulo no backend C#.

<a id="exemplo-de-uso-prático-create-api-module"></a>
### 💡 Exemplo de Uso Prático

#### Prompt no Chat da IA:
```
Usar a skill create-api-module para criar a estrutura do novo módulo Versatus.Estoque com a primeira entidade Produto.
```

#### O que a IA fará automaticamente:
1. Criará a estrutura de pastas: `Api/`, `Application/`, `Domain/`, `Infrastructure/`, `DependencyInjection/`.
2. Criará `EstoqueDbContext` (Write) e `EstoqueReadDbContext` (Read Replica).
3. Criará a injeção `AddEstoque()` no `ServiceCollectionExtensions.cs`.
4. Registrará o módulo e os DbContexts no `src/Versatus.WebAPI/Program.cs`.

---

## 6. Skill 5: `test-driven-development`

<a id="propósito-test-driven-development"></a>
### 🎯 Propósito
Guia o desenvolvimento orientado a testes (TDD — Red-Green-Refactor). Garante que nenhum código de produção seja escrito antes de ter um teste unitário que falhe primeiro.

<a id="quando-usar-test-driven-development"></a>
### 📅 Quando Usar
Ao implementar qualquer nova regra de negócio, serviço de domínio, ajuste de comportamento ou correção de bug.

<a id="exemplo-de-uso-prático-test-driven-development"></a>
### 💡 Exemplo de Uso Prático

#### Prompt no Chat da IA:
```
Usar a skill test-driven-development para implementar o método ObterPorIdAsync no EntidadeService.
```

#### O que a IA fará automaticamente:
1. Escreverá primeiro o teste unitário (`RED`) em `tests/Versatus.AcessoGlobal.Tests/`.
2. Executará `dotnet test` e confirmará que o teste **falhou do jeito certo**.
3. Escreverá o código de produção mínimo (`GREEN`) no `EntidadeService.cs`.
4. Reexecutará `dotnet test` e confirmará que ficou tudo verde.
5. Refatorará o código garantindo legibilidade e boas práticas (`REFACTOR`).

---

## 7. Skill 6: `writing-skills`

<a id="propósito-writing-skills"></a>
### 🎯 Propósito
Aplica a metodologia de **TDD (Test-Driven Development)** para a criação, edição e auditoria de **skills da própria IA**. Garante que qualquer nova skill criada seja concisa, otimizada para descoberta (SDO) e testada na prática contra falhas reais.

<a id="quando-usar-writing-skills"></a>
### 📅 Quando Usar
- Para **criar uma nova skill** no projeto (`.agents/skills/`) ou no ambiente global (`C:\Users\WIN10\.gemini\config\skills\`).
- Para **auditar e refatorar** uma skill existente, ajustando o cabeçalho YAML `description` para focar estritamente em condições de disparo (evitando que a IA pule o corpo da skill).
- Para **reorganizar skills extensas** (>100 linhas) movendo manuais ou esquemas para a subpasta `references/`.

<a id="exemplo-de-uso-prático-writing-skills"></a>
### 💡 Exemplos de Uso Prático por Cenário

#### Cenário A: Criar uma Nova Skill do Zero
```
Usar a skill writing-skills para criar a nova skill gerar-relatorio-tributario em .agents/skills/.
```
- **O que a IA faz:** Simula o cenário de erro sem a skill (baseline), cria a pasta `.agents/skills/gerar-relatorio-tributario/`, grava o `SKILL.md` com YAML `description` iniciando em `"Use ao..."` e **atualiza o `docs/MANUAL-SKILLS.md`** (Regra 12).

#### Cenário B: Auditar ou Refatorar uma Skill Existente
```
Usar a skill writing-skills para auditar e refatorar a skill spec-generator em .agents/skills/spec-generator/SKILL.md.
```
- **O que a IA faz:** Inspeciona o arquivo `SKILL.md`, corrige o campo `description` para focar em momentos de uso (sem resumir o fluxo) e fecha brechas de interpretação.

#### Cenário C: Reorganizar Arquivos Extensos em Subpastas (`references/`, `templates/`)
```
Usar a skill writing-skills para mover a documentação de nulidades da skill spec-generator para a subpasta references/.
```
- **O que a IA faz:** Cria a subpasta `.agents/skills/spec-generator/references/`, move o texto extenso para `sql_nullability_guide.md` e deixa o `SKILL.md` principal curto e enxuto.

---

### 📋 Tabela de Prompts Práticos:

| Ação Desejada | Exemplo de Prompt no Chat da IA |
|---|---|
| **Criar Nova Skill** | `Usar a skill writing-skills para criar a skill [nome-da-skill]` |
| **Auditar Skill Existente** | `Usar a skill writing-skills para auditar a skill [nome-da-skill]` |
| **Refatorar/Enxugar Skill** | `Usar a skill writing-skills para otimizar os tokens da skill [nome-da-skill]` |

---

## 8. Skill 7: `legacy-validation-audit`

<a id="propósito-legacy-validation-audit"></a>
### 🎯 Propósito
Garantir a **Rastreabilidade Total de Validações (Matriz RTV)** e a correta identificação de regras de herança (classes pai backend e frontend) do sistema legado. Impõe a varredura em 4 camadas (UI, Domínio, Helpers e Banco) e a criação de testes unitários TDD no C# para dar 100% de segurança de conversão.

<a id="quando-usar-legacy-validation-audit"></a>
### 📅 Quando Usar
- Durante a **Fase 1 (Elaboração da Spec)** de qualquer formulário legado.
- Sempre que houver suspeita de perda de validações visuais (`_Validating`, `ErrorProvider`, `MessageBox`) ou regras de herança (`EntPessoa`, `FormBaseCadastro`).
- Ao preparar a suíte de testes unitários C# para validar regras de negócio.

<a id="exemplo-de-uso-prático-legacy-validation-audit"></a>
### 💡 Exemplo de Uso Prático
```
Usar a skill legacy-validation-audit para auditar o formulário FCliente.cs, sua entidade EntCliente.cs e a classe pai EntPessoa.cs gerando a Matriz RTV.
```

#### 📋 Tabela de Prompts Práticos:

| Ação Desejada | Exemplo de Prompt no Chat da IA |
|---|---|
| **Auditoria com Herança** | `Usar a skill legacy-validation-audit para mapear a herança de EntPessoa em EntCliente.cs` |
| **Gerar Matriz RTV** | `Usar a skill legacy-validation-audit para gerar a Matriz RTV do formulário FCondicaoPagamento` |
| **Validar Cobertura TDD** | `Usar a skill legacy-validation-audit para criar testes C# cobrindo a Matriz RTV` |

---

## 9. Skill 8: `project-analyzer`

<a id="propósito-project-analyzer"></a>
### 🎯 Propósito
Skill genérica de **análise profunda e estruturada** de qualquer artefato do projeto. Pode analisar código C#, componentes React, arquitetura de módulos, processos de negócio, regras fiscais, cobertura de testes, segurança, performance, documentação e oportunidades de inovação/IA. Produz sempre um **Relatório de Análise padronizado** com descobertas, riscos classificados por severidade e recomendações priorizadas.

<a id="quando-usar-project-analyzer"></a>
### 📅 Quando Usar
- Antes de uma revisão de qualidade em serviços, schemas ou componentes existentes.
- Para investigar gargalos de UX, performance ou violações arquiteturais sem alterar código.
- Para explorar oportunidades de inovação, automação ou IA em um módulo ou processo.
- Para validar se uma spec funcional está 100% refletida no schema Zod e no código C#.
- Para qualquer análise ad-hoc que exija investigação, diagnóstico e recomendações estruturadas.

<a id="exemplo-de-uso-prático-project-analyzer"></a>
### 💡 Exemplos de Uso Prático

#### 📋 Tabela de Prompts Práticos:

| Alvo da Análise | Exemplo de Prompt no Chat da IA |
|---|---|
| **Serviço C#** | `Usar a skill project-analyzer para analisar o EntidadeService.cs e identificar riscos de performance e violações de AGENTS.md.` |
| **Schema Zod** | `Usar a skill project-analyzer para analisar o schema.ts da FEntidade e verificar se todas as regras da spec estão refletidas.` |
| **Arquitetura do Módulo** | `Usar a skill project-analyzer para analisar a arquitetura do módulo Versatus.AcessoGlobal.` |
| **Processo de Negócio** | `Usar a skill project-analyzer para analisar o processo de liquidação de Contas a Pagar e sugerir melhorias de UX.` |
| **Cobertura de Testes** | `Usar a skill project-analyzer para analisar a cobertura de testes do FCondicaoPagamento e apontar cenários não cobertos.` |
| **Inovação / IA** | `Usar a skill project-analyzer para analisar o módulo Financeiro e sugerir onde aplicar IA para ganho de produtividade.` |

---

## 10. Skill 9: `sdd-constitution`

<a id="10-skill-9-sdd-constitution"></a>
### 🎯 Propósito
Etapa 0 do fluxo SDD. Gera/atualiza `specs/memory/constitution.md` consolidando as 13 Leis do `AGENTS.md`, as 17 Regras Anti-Alucinação e as decisões `DEC-001..006` num **gate executável** (checklist `PASS/FAIL`). Não cria regra nova — consolida.

### 📅 Quando Usar
Ao iniciar o fluxo SDD num repositório sem `constitution.md`; ou sempre que `AGENTS.md`, `03-REGRAS-ANTI-ALUCINACAO.md` ou algum `DEC-*` mudar; ou ao mudar plataforma (runtime, ORM, convenção de nomes).

### 💡 Exemplo
```
/constitution
```
A IA relê os documentos ratificados, extrai o estado real da plataforma (`net10.0`, EF Core 10.x) e produz a constituição com Artigos I–XI + Gate de Conformidade (Seção 12) + Glossário. Para e pede aprovação.

---

## 11. Skill 10: `sdd-specify`

<a id="11-skill-10-sdd-specify"></a>
### 🎯 Propósito
Etapa 1 do SDD. Gera `specs/modulos/MOD-XX/spec.md` a partir do módulo legado inteiro: inventário de classes, árvore de herança, mapa de dependências cross-módulo, enums, épicos em ordem topológica e regras de negócio macro (`RN-XX-NNN`). **Zero decisão técnica.**

### 📅 Quando Usar
Ao começar a conversão de um módulo. **Não** usar para uma única tela — nesse caso, `spec-generator`.

### 💡 Exemplo
```
/specify MOD-05
```
A IA descobre os caminhos legados por convenção, varre `servidor/objeto de negócio/gestao.financeira/` + formulários + kernel, e produz `spec.md` + `dependency-graph.md`. Para e pede aprovação.

---

## 12. Skill 11: `sdd-clarify`

<a id="12-skill-11-sdd-clarify"></a>
### 🎯 Propósito
Etapa 2 do SDD. Rodada estruturada de perguntas sobre as `DÚVIDA:` e ambiguidades da spec, registrada em `specs/modulos/MOD-XX/clarify.md` (tabela ID/origem/pergunta/opções/resposta+data/efeito) e reincorporada na spec no mesmo commit.

### 📅 Quando Usar
Depois de `/specify` aprovado e antes de `/plan`. Materializa a Regra 7 ("em dúvida, PARE e PERGUNTE") como passo formal.

### 💡 Exemplo
```
/clarify MOD-05
```
A IA pergunta em blocos ≤5 (contexto + opções + impacto no plano), grava as respostas e edita a spec. Não presume respostas.

---

## 13. Skill 12: `sdd-plan`

<a id="13-skill-12-sdd-plan"></a>
### 🎯 Propósito
Etapa 3 do SDD. Onde entram as decisões técnicas, todas ancoradas na spec aprovada. Produz `plan.md` (projeto `Versatus.<Modulo>` net10.0, kernel compartilhado, épicos em ordem topológica, reconciliação com `Servidor.Strangler`, padrão de transação por operação), `research.md` (substitutos de libs legadas), `data-model.md` (tabelas/colunas **reais** via `INFORMATION_SCHEMA`) e `contracts/` (endpoints REST).

### 📅 Quando Usar
Depois de `/clarify` sem pendência bloqueante.

### 💡 Exemplo
```
/plan MOD-05
```
A IA consulta `localhost\SQLEXPRESS2008 / versatus`, monta o `data-model.md` com nomes e nulidade reais (coluna `NULL` → tipo anulável), define os épicos e contratos. Para e pede aprovação.

---

## 14. Skill 13: `sdd-tasks`

<a id="14-skill-13-sdd-tasks"></a>
### 🎯 Propósito
Etapa 4 do SDD. Quebra o plano em `specs/modulos/MOD-XX/tasks.md` — tarefas atômicas (**1 tarefa = 1 branch `feat/` = 1 commit**), cada uma com `Cobre:` (RN/VAL/OP), `Constituição:` (Artigos), `Pronto quando:` (critério objetivo), branch e mensagem de commit. Versão estruturada dos `specs/prompts-execucao/*`.

### 📅 Quando Usar
Depois de `/plan` aprovado.

### 💡 Exemplo
```
/tasks MOD-05
```
Ordem: `analysis → domain → dbcontext → service/operation → contract → parity → frontend → migration` por épico. Toda `VAL-xx`/`OP-xx` tem que aparecer em ≥1 tarefa.

---

## 15. Skill 14: `sdd-analyze`

<a id="15-skill-14-sdd-analyze"></a>
### 🎯 Propósito
Etapa 5 e **gate duro** do SDD. Cruza `constitution ↔ spec ↔ plan ↔ tasks ↔ código legado ↔ Matriz RTV ↔ Matriz ROT` (verificações V1–V7) e reprova o módulo se qualquer regra, validação, operação ou propriedade legada ficar **órfã**. Produz `analyze-report.md` com veredito.

### 📅 Quando Usar
Depois de `/tasks` e antes de `/implement`. Rodar de novo após cada correção até ✅.

### 💡 Exemplo
```
/analyze MOD-05
```
A IA varre o legado por palavras-chave de validação/operação e confere que cada ocorrência tem linha nas matrizes e cada linha das matrizes tem tarefa. Não corrige — aponta a ação exigida. `/implement` fica proibido enquanto o veredito for ⛔.

---

## 16. Skill 15: `legacy-operation-audit`

<a id="16-skill-15-legacy-operation-audit"></a>
### 🎯 Propósito
Irmã da `legacy-validation-audit` para telas que **não são CRUD** (Liquidar, Estornar, Reverter, Acertar, Fechar caixa, Conciliar). Extrai máquina de estados, pré-condições, **ordem exata de persistência**, efeitos colaterais (saldo, situação, período, sequencial) e condição de rollback, gerando a **Matriz ROT (Rastreabilidade de Operações e Transações)** + a tabela de transições de estado.

### 📅 Quando Usar
Durante `/specify`–`/plan`, sempre em paralelo com `legacy-validation-audit`, para qualquer classe de `Operação` ou formulário cujo botão principal seja "Executar/Confirmar/Estornar/Reverter/Fechar".

### 💡 Exemplo
```
Usar a skill legacy-operation-audit para mapear Liquidacao.cs, LiquidacaoEstorno.cs e a classe pai OperacaoDocumentoBase.cs gerando a Matriz ROT.
```

---

## 17. Skill 16: `legacy-calc-parity`

<a id="17-skill-16-legacy-calc-parity"></a>
### 🎯 Propósito
Isola cada fórmula de cálculo financeiro/fiscal do legado (juros, multa, desconto, conversão por índice, rateio proporcional, arredondamento), documenta parâmetros e regra de arredondamento **sem refatorar** (Regra 5), captura *golden values* do banco/sistema legado e gera esqueletos de teste de paridade numérica com **igualdade exata de `decimal`**.

### 📅 Quando Usar
Em qualquer épico com cálculo monetário. Alimenta as linhas de cálculo da Matriz ROT e as tarefas `parity` do `tasks.md`.

### 💡 Exemplo
```
Usar a skill legacy-calc-parity para extrair CalcularJurosMulta() de Documento.cs e gerar os golden tests de paridade.
```

---

## 18. Boas Práticas de Manutenção do Manual

1. **Atualização Contínua:** Sempre que uma nova skill for adicionada em `.agents/skills/`, inclua sua entrada no Índice e crie uma seção correspondente neste manual.
2. **Exemplos Reais:** Mantenha os prompts de exemplo alinhados aos nomes reais de arquivos e módulos do ERP.
3. **Validação por Commit:** Toda alteração de skill deve ser acompanhada de atualização neste documento na mesma branch.

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
8. [Boas Práticas de Manutenção do Manual](#8-boas-práticas-de-manutenção-do-manual)

---

## 1. Visão Geral e Conceitos

As **Skills** são capacidades especializadas e sequências de instruções padronizadas armazenadas na pasta `.agents/skills/`. Elas garantem que a migração de formulários, auditorias de código e criação de novos módulos no **Versatus ERP** sigam rigorosamente as premissas de **SOLID, Clean Architecture, Clean Code, Result Pattern, CQRS DB Split e .NET 10**.

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
4. Atualizará o manual `docs/MANUAL-SKILLS.md` conforme a Regra 12 do `AGENTS.md`.

---

## 8. Boas Práticas de Manutenção do Manual

1. **Atualização Contínua:** Sempre que uma nova skill for adicionada em `.agents/skills/`, inclua sua entrada no Índice e crie uma seção correspondente neste manual.
2. **Exemplos Reais:** Mantenha os prompts de exemplo alinhados aos nomes reais de arquivos e módulos do ERP.
3. **Validação por Commit:** Toda alteração de skill deve ser acompanhada de atualização neste documento na mesma branch.

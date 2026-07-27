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
6. [Boas Práticas de Manutenção do Manual](#6-boas-práticas-de-manutenção-do-manual)

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

#### Prompt no Chat da IA:
```
Ativar skill migrate-crud para migrar o formulário FCondicaoPagamento do módulo AcessoGlobal.
Objeto legado: projeto_tag_1906/servidor/objeto de negócio/acessoglobal/CondicaoPagamento.cs
Formulário legado: projeto_tag_1906/cliente/cliente.aplicativo/acessoglobal/FCondicaoPagamento.cs
```

#### O que a IA fará automaticamente:
1. Classificará a tela como Padrão A (CRUD Padrão).
2. Gerará a Spec Funcional em `docs/spec_fcondicaopagamento.md`.
3. Aguardará sua aprovação da Spec.
4. Após aprovação, gerará DTOs, Entidade POCO, Fluent API Mappings, Service e Controller C#.
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
Skill de auditoria de qualidade que inspeciona o código C# e React de um formulário migrado para garantir 100% de conformidade com as **11 leis do `AGENTS.md`**.

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

## 6. Boas Práticas de Manutenção do Manual

1. **Atualização Contínua:** Sempre que uma nova skill for adicionada em `.agents/skills/`, inclua sua entrada no Índice e crie uma seção correspondente neste manual.
2. **Exemplos Reais:** Mantenha os prompts de exemplo alinhados aos nomes reais de arquivos e módulos do ERP.
3. **Validação por Commit:** Toda alteração de skill deve ser acompanhada de atualização neste documento na mesma branch.

# CLAUDE.md — Versatus.Net8

> Este arquivo é carregado automaticamente pelo Claude Code no início de toda sessão neste
> repositório. Ele é um resumo operacional — os documentos linkados são a fonte completa e
> têm precedência em caso de dúvida.

## O projeto

Migração do ERP legado Versatus (.NET Framework + Gentle.NET + .NET Remoting, snapshot
congelado em `projeto_tag_1906/`) para .NET 8/10 + EF Core + React, usando o **Strangler Fig
Pattern**: o legado nunca é desligado, um módulo é migrado por completo antes do próximo.

## Ritual obrigatório antes de escrever spec ou código

> Rode `/onboarding` (opcionalmente `/onboarding MOD-03`) para executar este ritual de
> forma guiada e receber um resumo do estado atual antes de agir.

1. Leia `specs/04-CONTRATO-DA-IA.md`, especialmente a **Seção 5 (Log de Progresso e
   Handoff)** — ela diz exatamente onde o trabalho parou, qual a branch ativa e qual o
   próximo passo. Nunca reinicie trabalho já concluído nem ignore decisões já tomadas ali.
2. Leia `specs/00-INDICE-GERAL.md` para o status atual de cada módulo.
3. Leia `specs/03-REGRAS-ANTI-ALUCINACAO.md` (17 regras) e `.agents/AGENTS.md` (13 leis)
   antes de gerar ou alterar qualquer código.
4. Antes de propor uma decisão de arquitetura, confira se ela já não foi tomada em
   `specs/decisoes/` (DEC-001 a DEC-006).

## Regras de ouro (resumo — os documentos acima são a fonte completa)

- **Spec-First**: nada é implementado sem spec aprovada pelo usuário
  (`docs/spec_f[nome].md` ou `specs/modulos/`). Em dúvida, **PARE e PERGUNTE** — nunca
  invente, nunca "melhore" o que não foi pedido.
- **Fidelidade ao legado**: nomes de tabelas/colunas são sagrados; não adicionar nem
  remover campos sem autorização explícita; não refatorar regra de negócio só porque
  parece possível simplificar.
- **Domínio puro**: entidades em `Domain/` são POCOs sem DataAnnotations; todo mapeamento
  é Fluent API em `Infrastructure/`.
- **Result Pattern**: nunca `throw` para validação de negócio esperada — sempre
  `Result<T>` / `ValidationResult`. Exceções só para falha real de infraestrutura.
- **CQRS leve**: leituras via `ReadContext`/`ReadConnection` (`NoTracking`); escritas via
  `Context`/`WriteConnection`.
- **SQL Server 2008**: nunca `.Skip().Take()` direto num `IQueryable` — materialize com
  `ToListAsync()` primeiro e pagine em memória.
- **Sequencial**: nunca `IDENTITY` ou GUID para chave — usar `GeradorSequencialService`.
- **Git**: nunca commitar ou dar push em `develop`/`main`. Trabalhar em `feat/`, `fix/` ou
  `docs/`; ao concluir, perguntar ao usuário antes de sugerir o merge — nunca executá-lo
  sozinho.
- **Testes**: nenhum serviço backend ou schema Zod é considerado pronto sem sua suíte de
  testes (100% da Matriz RTV) e sem `dotnet build` / `dotnet test` / `npm run build` /
  `npm test` passando.
- **Nomenclatura**: pastas físicas, projetos e namespaces técnicos em **inglês**
  (`Repositories`, `Infrastructure`, `Exceptions`); entidades de domínio, exceptions de
  negócio, tabelas e colunas em **português**, idênticos ao legado (`Pais.cs`, `Pedido.cs`).

## Skills deste projeto

O conteúdo completo das skills vive em `.agents/skills/<nome>/SKILL.md` (fonte única).
`.claude/skills/` contém apenas stubs de descoberta para que este client dispare a skill
certa automaticamente — ao acionar qualquer uma, leia o `SKILL.md` completo em
`.agents/skills/` (e a pasta `references/`, se houver) antes de agir. Catálogo com
exemplos de uso: `docs/MANUAL-SKILLS.md`.

## Ao encerrar uma sessão de trabalho relevante

Rode `/handoff` — ele segue a Seção 4 de `specs/04-CONTRATO-DA-IA.md`: confirma a branch,
resume os commits reais desta sessão via git e **atualiza o Log de Progresso (Seção 5)**
antes de finalizar. Isso é o que evita que a próxima sessão repita trabalho ou perca
contexto.

## Guarda-corpo técnico de Git

Um hook (`.claude/settings.json` + `.claude/hooks/block-protected-git.js`) bloqueia de fato
`git push`/`git merge` em `develop`/`main` e pede confirmação extra em `git rebase` — a Lei 5
do `AGENTS.md` deixou de ser só política escrita.

# Prompt Genérico — Migração de Formulário Legado para .NET 10 + React

> **Como usar:**
> 1. Copie o bloco de prompt abaixo
> 2. Substitua `[Nome]` e `[Modulo]` pelo formulário que vai migrar
> 3. Preencha as seções `3a`, `3b` e `3c` com os arquivos/tabelas reais
> 4. Cole no chat da IA e aguarde o **mapeamento da seção 4** antes de aprovar a Spec

---

```
Você é um agente especializado na migração do sistema legado Versatus (Delphi/C#) para a arquitetura moderna .NET 10 + React/TypeScript.

## 1. LEITURA OBRIGATÓRIA ANTES DE QUALQUER AÇÃO
Leia os seguintes arquivos nesta ordem antes de escrever qualquer código ou spec:

1. `.agents/AGENTS.md` — Regras e leis do projeto (10 regras ativas, incluindo SOLID, Clean Architecture e Clean Code)
2. `.agents/skills/migrate-crud/SKILL.md` — Pipeline de migração completo (4 fases)
3. `docs/spec_fentidade.md` — Template padrão de Spec Funcional
4. `specs/00-INDICE-GERAL.md` — Índice geral dos módulos já mapeados
5. `specs/03-REGRAS-ANTI-ALUCINACAO.md` — Regras de fidelidade ao legado

## 2. CONTEXTO DO PROJETO E PREMISSAS ARQUITETURAIS
- **Princípios Fundamentais:** SOLID, Clean Architecture, Clean Code e Boas Práticas do .NET 10
- **Banco de dados:** SQL Server 2008 (sem OFFSET/FETCH — paginação obrigatória em memória)
- **Backend:** .NET 10, Clean Architecture (Domain POCOs puras, Infrastructure Fluent API, CQRS DB Split Write/Read Connection, Controllers limpos, Result<T> Pattern para erros funcionais)
- **Frontend:** React + TypeScript + Material-UI (MUI), BaseCadastroConfig<T> OOP, Zod Validation
- **Branch atual:** develop (limpa e atualizada)
- **Formulários já migrados:** FParametro (Padrão B), FCondicaoPagamento e FEntidade (Padrão A)

## 3. ARQUIVOS LEGADOS PARA ANÁLISE
Analise os seguintes arquivos do sistema legado antes de gerar qualquer artefato.
Use `view_file` para lê-los integralmente — nunca assuma o conteúdo sem ler.

### 3a. Objeto de Negócio / Regras (.cs) e Classes Pai (Herança)
> Classe de entidade com propriedades, validações, regras de negócio,
> enums e valores padrão do legado.
> SE A CLASSE HERDAR DE UMA CLASSE PAI (Ex: EntPessoa), INFORME TAMBÉM A CLASSE PAI!

[INFORME O CAMINHO OU COLE O CONTEÚDO DA CLASSE FILHA E DA CLASSE PAI]
Exemplo: `projeto_tag_1906/servidor/objeto de negócio/acessoglobal/[Nome].cs`

### 3b. Formulário Legado (.cs — classe de Form) e Forms Pai
> Classe de formulário com a definição visual da tela: campos, abas,
> grids, botões, eventos de UI (_Validating, _Leave, _Click) e lógica de apresentação.
> SE O FORMULARIO HERDAR DE UM FORM PAI, INFORME O FORM PAI!

[INFORME O CAMINHO OU COLE O CONTEÚDO]
Exemplo: `projeto_tag_1906/cliente/formularios/F[Nome].cs`

### 3c. Tabelas do Banco de Dados
> Liste as tabelas e colunas principais envolvidas.
> Identifique quais colunas aceitam NULL (precisarão de tipos
> anuláveis no C# moderno).

Tabela principal: `Glo[Nome]`
Colunas: Id[Nome], Descricao (NOT NULL), Ativo (NOT NULL), IdTipo (NULL), ...
Tabelas filhas: `Glo[Nome]Item` — Colunas: ...

## 4. O QUE EXTRAIR DA ANÁLISE DOS ARQUIVOS LEGADOS

Ao ler os arquivos acima (incluindo herança e eventos de formulário), produza dois mapeamentos completos antes de gerar qualquer spec:

### 4a. Mapeamento de Propriedades vs. UI
| # | Propriedade Legada | Tipo Legado | Nullable? | Campo no Formulário? | Obrigatório? | Observação |
|---|-------------------|-------------|-----------|---------------------|-------------|------------|
| 1 | Descricao         | string      | NÃO       | Sim                 | Sim         | Label "Descrição" |
| 2 | IdTipo            | int         | SIM       | Sim                 | Sim         | Select/Enum |
| 3 | Observacao        | string      | SIM       | Sim                 | Não         | Textarea   |
| 4 | DataAuditoria     | DateTime    | SIM       | Não (sistema)       | —           | Apenas auditoria |

### 4b. Matriz RTV (Rastreabilidade Total de Validações e Regras de Negócio)
| ID | Origem Legada (Arquivo:Linha) | Camada / Nível | Regra / Condição Legada | Mensagem Legada Exata | Destino Backend (.NET Result<T>) | Destino Frontend (Zod + MUI) |
|---|---|---|---|---|---|---|
| VAL-01 | `F[Nome].cs:tbCPF_Validating` | UI / Filho | CPF válido se `TipoPessoa == 'F'` | "CPF inválido." | `ValidadorCpf.Validar(dto.Cpf)` | `zod.refine(validaCPF)` |
| VAL-02 | `EntPessoa.cs:Validar()` | Domínio / Pai | UF obrigatória se Brasil | "Informe a UF." | `if (string.IsNullOrEmpty(dto.Uf))` | `zod.string().length(2)` |

AVISO: Nenhuma propriedade editável pode ficar fora da UI (Regra 3 do AGENTS.md).
AVISO: 100% das validações legadas (inclusive herdadas e de eventos de UI) devem estar mapeadas na Matriz RTV.
AVISO: Campos com NULL no banco devem virar tipos anuláveis no C# (int?, string?, etc).

## 5. TAREFA — PIPELINE DE MIGRAÇÃO
Siga rigorosamente o pipeline da SKILL.md fase a fase:

**Fase 1 — Spec Funcional:**
- Classifique o formulário como Padrão A (CRUD) ou Padrão B (Lote)
- Use os mapeamentos das seções 4a e 4b para garantir cobertura 100% das propriedades e validações
- Inclua: requisitos de Clean Architecture, Result Pattern, endpoints, Matriz RTV e regras de herança
- Inclua: layout de abas e campos extraídos do .cs de formulário
- Gere a spec em `docs/spec_f[nome].md` com Critérios de Aceite e plano de testes unitários TDD
- PARE e aguarde minha aprovação antes de gerar qualquer código

**Fase 2 — Backend C#:**
- Somente após aprovação da Spec
- Entidade POCO pura em Domain (sem DataAnnotations) + Fluent API na Infrastructure
- Respeitar SOLID (Inversão de Dependência via interfaces IService/IRepository) e Clean Code
- File-scoped namespaces e Nullable Reference Types habilitados
- Colunas NULL no banco → tipos anuláveis (int?, string?) na entidade C#
- Validações e regras de negócio extraídas do .cs legado → Result<T> (nunca usar throw new Exception() para falhas de negócio)
- Execute `dotnet build` e corrija todos os erros antes de continuar

**Fase 3 — Frontend React:**
- Types, Schema Zod, Config (BaseCadastroConfig<T>), View (index.tsx)
- Reproduzir abas e estrutura visual conforme o .cs de formulário legado
- Todos os campos obrigatórios com prop `required` no MUI (asterisco vermelho)
- Checklist: cada linha do mapeamento da seção 4 com Sim deve ter campo na UI
- Organizar em `src/pages/[Modulo]/F[Nome]/`
- Execute `npm run build` e corrija todos os erros

**Fase 4 — Integração e Finalização:**
- Registrar rota em App.tsx e menu de navegação
- Após merge na develop, perguntar se devo excluir a branch de recurso
  e alternar o ambiente de trabalho para develop

## 6. RESTRIÇÕES CRÍTICAS (AGENTS.md)
- Obrigatorio: seguir SOLID, Clean Architecture, Clean Code e recursos modernos do .NET 10
- Proibido: commitar em `main` ou `develop` diretamente
- Proibido: usar .Skip().Take() sobre IQueryable não materializado
- Proibido: omitir campos editáveis da entidade na UI
- Proibido: avançar para código sem aprovação da Spec
- Proibido: lançar throw new Exception() para validações de negócio
- Proibido: inventar ou otimizar funcionalidades além do que está no legado
- Obrigatório: criar branch `feat/migrate-[nome]` para todo desenvolvimento
- Obrigatório: manter nomes de tabelas e colunas idênticos ao legado
- Obrigatório: sempre perguntar sobre exclusão da branch após merge na develop
```

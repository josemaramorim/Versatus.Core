---
name: spec-generator
description: Skill para análise rigorosa do código C# legado e geração automatizada de Especificações Funcionais padronizadas em docs/spec_f[nome].md.
---

# Skill: spec-generator

Esta skill guia a análise detalhada dos arquivos do sistema legado (objeto de negócio `.cs`, formulário `.cs` e tabelas do banco) para produzir Especificações Funcionais completas e sem lacunas em `docs/spec_f[nome].md`.

---

## 1. Entrada Esperada

O usuário ou agente deve fornecer:
1. **Nome do Formulário**: Ex: `FProduto`, `FCliente`, `FPermissao`.
2. **Módulo de Negócio**: Ex: `AcessoGlobal`, `GestaoTributo`, `Estoque`, `Financeiro`.
3. **Arquivo de Objeto de Negócio Legado**: Objeto `.cs` com propriedades, validações e enums.
4. **Arquivo de Formulário Legado**: Formulário `.cs` com layout, abas, campos visíveis, eventos e regras de UI.
5. **Esquema de Banco de Dados**: Colunas das tabelas principal e filhas, identificando quais aceitam `NULL`.

---

## 2. Passo 0 — Classificação Obrigatória da Tela

Antes de elaborar a spec, analise o código do formulário e classifique em:

### Padrão A: CRUD Padrão
- Formulário possui botões de Inserção/Edição/Exclusão e Grid paginado.
- Endpoints: `GET` paginado, `GET/{id}`, `GET/completo/{id}`, `POST`, `PUT/{id}`, `DELETE/{id}`.
- herda `BaseCadastroConfig<T>` com modal/gaveta lateral.

### Padrão B: Configuração em Lote
- Formulário edita valores de itens já existentes em lote (Accordion/TreeList).
- Endpoints: `GET /escopo` + `PUT /salvar-valores`.
- Botões Novo e Excluir **totalmente ocultados**.

---

## 3. Passo 1 — Tabela de Mapeamento de Propriedades (Obrigatória)

Monte a tabela completa de auditoria de propriedades antes de escrever o corpo da spec:

| # | Propriedade Legada | Tipo Legado | Aceita NULL no Banco? | Tipo no C# Moderno | Campo na UI? | Componente UI | Prop `required`? | Observação |
|---|---|---|---|---|---|---|---|---|
| 1 | `Descricao` | `string` | NÃO | `string` | Sim | `TextField` | Sim (`required`) | Label "Descrição" |
| 2 | `IdTipo` | `int` | SIM | `int?` | Sim | `Select` | Sim (`required`) | Enum mapeado |
| 3 | `Ativo` | `bool` / `short` | NÃO | `bool` | Sim | `Switch` | Não | Cor: verde se ativo, vermelho se inativo |

> [!CAUTION]
> **Anti-Alucinação & Cobertura Total:**
> - NENHUMA propriedade editável pode ser omitida da UI (Regra 3 do `AGENTS.md`).
> - Toda coluna que aceita `NULL` no banco DEVE ser mapeada como tipo nulo (`int?`, `decimal?`, `DateTime?`) no C# moderno para evitar `SqlNullValueException`.

---

## 4. Passo 2 — Estrutura Padrão do Arquivo `docs/spec_f[nome].md`

A spec gerada DEVE conter rigorosamente as 8 seções abaixo:

```markdown
# Especificação Funcional — [Nome do Formulário] (F[Nome])

> **Módulo:** [NomeDoModulo]
> **Padrão de Tela:** [Padrão A: CRUD Padrão / Padrão B: Configuração em Lote]
> **Arquivo Legado Objeto:** `projeto_tag_1906/.../[Nome].cs`
> **Arquivo Legado Form:** `projeto_tag_1906/.../F[Nome].cs`

---

## 1. Resumo e Objetivo
[Descrição curta da finalidade do cadastro/configuração no ERP]

## 2. Mapeamento de Entidades e Banco de Dados
- **Tabela Principal:** `Glo[Nome]`
- **Mapeamento de Nulidade:** [Listar campos anuláveis e seus tipos C#]

## 3. Requisitos Arquiteturais & Boas Práticas (.NET 10)
- **SOLID & Clean Architecture:** Domain POCO pura em `src/Versatus.[Modulo]/Domain/Entities/`, Mappings Fluent API na Infrastructure, Services via DI e Controllers finos.
- **CQRS DB Split:** Consultas e paginação via `ReadContext` (`ReadConnection` com `NoTracking`), mutações via `Context` (`WriteConnection`).
- **Result Pattern:** Erros de validação retornam `Result<T>` funcional (`400 BadRequest`), sem exceções de fluxo.

## 4. Endpoints REST da Web API
- `GET /api/[nome]/paginado` — Listagem paginada (skip/take em memória para SQL Server 2008)
- `GET /api/[nome]/{id}` — Detalhes por ID
- `POST /api/[nome]` — Inserção
- `PUT /api/[nome]/{id}` — Atualização
- `DELETE /api/[nome]/{id}` — Exclusão

## 5. Interface Gráfica Frontend (React + MUI)
- **Estrutura de Pastas:** `src/pages/[Modulo]/F[Nome]/` (`types.ts`, `schema.ts`, `[Nome]CadastroConfig.tsx`, `index.tsx`)
- **Sinalização Visual de Obrigatoriedade:** Prop `required` nos campos obrigatórios do MUI (asterisco vermelho `*`).
- **Indicador de Status:** Coluna `Ativo` com cor verde quando ativa e vermelha quando inativa.

## 6. Regras de Negócio e Validações
- [Regra 1 extraída do .cs legado]
- [Regra 2 extraída do .cs legado]

## 7. Critérios de Aceite (Cenários de Teste)
- **Cenário 1 (Sucesso na listagem):** GET paginado retorna status `200 OK` com os itens e o total.
- **Cenário 2 (Validação de negócio):** Dados inválidos retornam `400 BadRequest` com mensagens do Result Pattern.
- **Cenário 3 (Sinalização visual):** Campos obrigatórios exibem `*` vermelho e validam via Zod schema.

## 8. Pendências e Dúvidas
- [Listar qualquer ponto incerto para validação do usuário]
```

---

## 5. Validação da Spec Antes do Envio

Antes de apresentar a spec ao usuário:
1. Confirme que nenhuma propriedade editável ficou de fora da UI.
2. Confirme que todos os campos com `NULL` no banco possuem `?` no tipo C#.
3. Pare e **aguarde a aprovação explícita do usuário** antes de gerar qualquer código.

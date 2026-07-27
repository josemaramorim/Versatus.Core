# Template Padrão de Spec Funcional — `docs/spec_f[nome].md`

> **Módulo:** [NomeDoModulo]
> **Padrão de Tela:** [Padrão A: CRUD Padrão / Padrão B: Configuração em Lote]
> **Arquivo Legado Objeto:** `projeto_tag_1906/.../[Nome].cs`
> **Arquivo Legado Form:** `projeto_tag_1906/.../F[Nome].cs`

---

## 1. Resumo e Objetivo
[Descrição clara do propósito da tela e entidade no ERP]

## 2. Mapeamento de Entidades e Banco de Dados
- **Tabela Principal:** `Glo[Nome]`
- **Mapeamento de Nulidade:**

| # | Propriedade Legada | Tipo Legado | Aceita NULL no Banco? | Tipo no C# Moderno | Campo na UI? | Componente UI | Prop `required`? |
|---|---|---|---|---|---|---|---|
| 1 | `Descricao` | `string` | NÃO | `string` | Sim | `TextField` | Sim |
| 2 | `IdTipo` | `int` | SIM | `int?` | Sim | `Select` | Sim |

## 3. Requisitos Arquiteturais & Boas Práticas (.NET 10)
- **Clean Architecture & SOLID:** Domain POCO puras, Infrastructure Fluent API, Services via DI, Controllers finos.
- **CQRS DB Split:** Consultas e paginação via `ReadContext` (`ReadConnection` com `NoTracking`), mutações via `Context` (`WriteConnection`).
- **Result Pattern:** Retorno funcional `Result<T>` (`400 BadRequest`), sem `throw Exception()`.

## 4. Endpoints REST da Web API
- `GET /api/[nome]/paginado`
- `GET /api/[nome]/{id}`
- `POST /api/[nome]`
- `PUT /api/[nome]/{id}`
- `DELETE /api/[nome]/{id}`

## 5. Interface Gráfica Frontend (React + MUI)
- **Estrutura:** `src/pages/[Modulo]/F[Nome]/`
- **Sinalização `required`:** Prop `required` nos componentes MUI para campos obrigatórios.
- **Coluna Status:** Cor verde se `Ativo` = true, vermelha se false.

## 6. Regras de Negócio
1. [Regra de negócio extraída do .cs]

## 7. Critérios de Aceite (Cenários de Teste)
1. **Listagem Paginada:** Retorna `200 OK` com dados e total.
2. **Validação:** Retorna `400 BadRequest` com Result Pattern em erros de dados.

## 8. Pendências e Dúvidas
- [Dúvidas para confirmação]

# Contrato — Caixa e Banco (E3)

> Rascunho do `/plan`. Campos finais dos `record`s = todas as colunas de
> `data-model.md §3.1–3.4` (Regra 3/4 — nenhuma propriedade editável fora do DTO).
> Padrão de tela: **CRUD Padrão** (tem Novo/Editar/Excluir) — recebe React nesta rodada.

## `CaixaBanco`

| Rota | Verbo | Request | Response | Códigos | CQRS |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/caixa-banco/paginado` | GET | `[FromQuery] FiltroCaixaBancoDto` (texto, ativo?, idTipoConta?, entraFluxoCaixa?, page, limit) | `PagedResult<CaixaBancoListaDto>` | 200 | Read |
| `/api/financeiro/caixa-banco/{idFilial}/{id}` | GET | — | `CaixaBancoDto` | 200 / 404 | Read |
| `/api/financeiro/caixa-banco` | POST | `CriarCaixaBancoDto` | `CaixaBancoDto` | 201 / 400 | Write |
| `/api/financeiro/caixa-banco/{idFilial}/{id}` | PUT | `AtualizarCaixaBancoDto` | `CaixaBancoDto` | 200 / 400 / 404 | Write |
| `/api/financeiro/caixa-banco/{idFilial}/{id}` | DELETE | — | — | 204 / 400 (em uso) / 404 | Write |
| `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | GET | — | `IReadOnlyList<CaixaBancoUsuarioDto>` | 200 | Read |
| `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | PUT | `SalvarCaixaBancoUsuariosDto` (lista) | `IReadOnlyList<CaixaBancoUsuarioDto>` | 200 / 400 | Write |

`CaixaBancoDto` inclui, quando `IdTipoConta` = banco, o bloco `ContaBancaria`
(`ContaBancariaDto` — **campos núcleo** de `data-model.md §3.2`; os campos de integração
bancária ficam em `ContaBancariaIntegracaoDto`, editados pelas telas de E14).

**Regras (Matriz RTV — a extrair no `analysis` de E3):** `Descricao` obrigatória;
`IdTipoConta` obrigatório; excluir bloqueado se houver `Dominio`, `SaldoCaixaBanco`,
`DocumentoParcela` ou `Movimento` vinculado; `ContaBancaria.NumeroConta` obrigatória
quando conta bancária.

## `ContaBancaria` (núcleo — 1:1 de `CaixaBanco`)

| Rota | Verbo | Request | Response | Códigos | CQRS |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/conta-bancaria/{idFilial}/{id}` | GET | — | `ContaBancariaDto` | 200 / 404 | Read |
| `/api/financeiro/conta-bancaria/{idFilial}/{id}` | PUT | `AtualizarContaBancariaDto` (núcleo) | `ContaBancariaDto` | 200 / 400 / 404 | Write |

> `id` = `IdCaixaBanco` (PK compartilhada). Criação/exclusão da `ContaBancaria` acompanha
> o `CaixaBanco` (mesmo Handler).

## `Cobrador` — CRUD Padrão

`/api/financeiro/cobrador/paginado` (GET, Read) · `/{idFilial}/{id}` (GET/PUT/DELETE) ·
`POST`. `Nome` e `IdEntidade` obrigatórios; `Ativo` default true.

## `SaldoCaixaBanco` — somente leitura

`/api/financeiro/caixa-banco/{idFilial}/{id}/saldo?data=` (GET, Read) → `SaldoCaixaBancoDto`
(saldo anterior, débito, crédito, conciliado). Sem escrita direta — atualizado pelos
Handlers de movimento/liquidação.

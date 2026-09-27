# Contrato — Caixa e Banco (E3)

> Rascunho do `/plan`, **implementado no E3-T06 (2026-09-27)**: controllers em
> `Api/Controllers/{CaixaBanco,ContaBancaria,Cobrador}Controller.cs`, `record`s em
> `Domain/DTOs/{CaixaBanco,Cobrador}Dtos.cs` (+ `CaixaBancoUsuarioItemDto`). Campos = colunas de
> `analysis/E3-caixa-banco.md §2` (núcleo E3 + `EnviarSped`/`CpfCnpj`); enums pelo valor inteiro
> do banco. Erro de negócio → `400 { message, errors[] }` (`ValidationError`).
> Padrão de tela: **CRUD Padrão** (tem Novo/Editar/Excluir) — recebe React nesta rodada.

## `CaixaBanco`

| Rota | Verbo | Request | Response | Códigos | CQRS |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/caixa-banco/paginado` | GET | `[FromQuery] FiltroCaixaBancoDto` (texto, ativo?, idTipoConta?, entraFluxoCaixa?, page, limit) | `PagedResult<CaixaBancoListaDto>` | 200 | Read |
| `/api/financeiro/caixa-banco/{idFilial}/{id}` | GET | — | `CaixaBancoDto` | 200 / 404 | Read |
| `/api/financeiro/caixa-banco` | POST | `CriarCaixaBancoDto` | `CaixaBancoDto` | 201 / 400 | Write |
| `/api/financeiro/caixa-banco/{idFilial}/{id}` | PUT | `AtualizarCaixaBancoDto` | `CaixaBancoDto` | 200 / 400 / 404 | Write |
| `/api/financeiro/caixa-banco/{idFilial}/{id}` | DELETE | — | — | 204 / 400 / 404 | Write |
| `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | GET | — | `IReadOnlyList<CaixaBancoUsuarioDto>` | 200 | Read |
| `/api/financeiro/caixa-banco/{idFilial}/{id}/usuarios` | PUT | `SalvarCaixaBancoUsuariosDto` (`Itens: CaixaBancoUsuarioItemDto(IdUsuario, IdUsuarioSalvo?)`) | `IReadOnlyList<CaixaBancoUsuarioDto>` | 200 / 400 | Write |

`CaixaBancoDto` inclui, quando `IdTipoConta` = banco, o bloco `ContaBancaria`
(`ContaBancariaDto` — **campos núcleo** de `data-model.md §3.2`; os campos de integração
bancária ficam em `ContaBancariaIntegracaoDto`, editados pelas telas de E14). No POST/PUT o
bloco vem como `AtualizarContaBancariaDto`.

**Regras implementadas (E3-T05):** as da Matriz RTV — `VAL-E3-01..12, 17` (ver
`matriz-rtv.md#E3`). As linhas abaixo eram hipóteses do rascunho **sem linha na Matriz RTV**
e **não** foram implementadas (Regra 7.2) — as restrições NOT NULL/FK do banco continuam
valendo: `Descricao` obrigatória;
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
`POST` — `SalvarCobradorDto` → `CobradorDto`; 200 / 201 / 204 / 400 / 404. `Ativo` default
true no DTO. (`Nome`/`IdEntidade` obrigatórios: sem linha na Matriz RTV — só NOT NULL do banco.)

## `SaldoCaixaBanco` — somente leitura

`/api/financeiro/caixa-banco/{idFilial}/{id}/saldo?data=` (GET, Read) → `SaldoCaixaBancoDto`
(saldo anterior, débito, crédito, conciliado). Sem escrita direta — atualizado pelos
Handlers de movimento/liquidação. **Não implementado no E3-T06:** depende de `OP-E3-08`/
`CALC-E3-03`, movidas para o **E3-T09** (2026-09-24).

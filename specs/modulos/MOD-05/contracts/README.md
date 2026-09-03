# MOD-05 — Contratos REST

Um arquivo por área/épico. Cada endpoint: rota · verbo · request `record` · response
`record` · códigos · conexão CQRS (Read/Write) · paginação.

**Convenções (constituição Artigos IV, VI, VII):**
- Rota base: `/api/financeiro/<area>`.
- `GET` (listagem/lookup/detalhe) → `GestaoFinanceiraReadDbContext` (`NoTracking`).
- `POST`/`PUT`/`DELETE` e operações → `GestaoFinanceiraDbContext` (Write), 1 transação/Handler.
- Paginação SQL Server 2008: `ToListAsync()` **antes** de `Skip/Take` (Artigo VII.5).
  Response: `PagedResult<T>(IReadOnlyList<T> Itens, int Total)`.
- Falha de validação → `Result<T>` → `400 BadRequest` com `IReadOnlyList<ValidationError>`.
- DTOs são `sealed record` independentes em `Domain/DTOs/` (nunca aninhados no controller).
- Referências cross-módulo nos DTOs são `int` (`IdEntidade`, `IdFilial`, `IdOperacao`…),
  nunca objetos de outro módulo.

| Área | Arquivo | Épico | Status |
| :--- | :--- | :--- | :--- |
| Caixa e Banco | [`caixa-banco.md`](./caixa-banco.md) | E3 | rascunho (plan) |
| Documento e Parcela | [`documento.md`](./documento.md) | E4 | rascunho (plan) |
| Liquidação | [`liquidacao.md`](./liquidacao.md) | E6 | rascunho (plan) |
| Domínio e Período | `dominio-periodo.md` | E2 | a criar no `analysis` do épico |
| Movimento e Formas | `movimento.md` | E5 | idem |
| Estorno | `estorno.md` | E7 | idem |
| Reversão | `reversao.md` | E8 | idem |
| Cheques | `cheques.md` | E9 | idem |
| Adiantamentos | `adiantamentos.md` | E10 | idem |
| DRE / Projeção / Seleção | `dre-projecao.md` | E11 | idem |
| Cobrança / Transação filial | `cobranca-transacao-filial.md` | E12 | idem |
| Consultas | `consultas.md` | E13 | idem |
| Integração bancária | `integracao-bancaria.md` | E14 | idem |

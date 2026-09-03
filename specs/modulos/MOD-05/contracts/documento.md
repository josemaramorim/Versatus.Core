# Contrato — Documento Financeiro e Parcela (E4)

> Rascunho do `/plan`. `record`s = colunas de `data-model.md §3.8–3.9`.
> Padrão de tela: **operação/consulta** (`FDocumento`, `FDocumentoManutencao`,
> `FDocumentoConsulta` herdam `FBaseProcesso`). Não é CRUD simples — inclusão gera
> parcelas; alterações passam por Handlers.

## Consulta

| Rota | Verbo | Request | Response | Códigos | CQRS |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/documento/paginado` | GET | `[FromQuery] FiltroDocumentoDto` (idEntidade?, receberPagar?, idSituacao?, idOperacao?, numeroDocumento?, emissaoDe/Ate?, vencimentoDe/Ate?, page, limit) | `PagedResult<DocumentoListaDto>` | 200 | Read |
| `/api/financeiro/documento/{idFilial}/{id}` | GET | — | `DocumentoDto` (cabeçalho) | 200 / 404 | Read |
| `/api/financeiro/documento/{idFilial}/{id}/completo` | GET | — | `DocumentoCompletoDto` (parcelas, itens financeiros, tributos, movimentos, comissionados) | 200 / 404 | Read |
| `/api/financeiro/parcela/paginado` | GET | `[FromQuery] FiltroParcelaDto` (idEntidade?, idSituacao?, portador?, vencimentoDe/Ate?, boleto?, page, limit) | `PagedResult<DocumentoParcelaListaDto>` | 200 | Read |
| `/api/financeiro/parcela/{idFilial}/{id}` | GET | — | `DocumentoParcelaDto` | 200 / 404 | Read |

## Operações

| Rota | Verbo | Request | Response | Códigos | Handler |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/documento` | POST | `IncluirDocumentoDto` (cabeçalho + itens financeiros + rateio; parcelas geradas pela `IdCondicaoPagamento` ou informadas) | `DocumentoCompletoDto` | 201 / 400 | `IncluirDocumentoHandler` |
| `/api/financeiro/documento/{idFilial}/{id}` | PUT | `AlterarDocumentoDto` (campos permitidos conforme situação/origem) | `DocumentoDto` | 200 / 400 / 404 | `AlterarDocumentoHandler` |
| `/api/financeiro/documento/{idFilial}/{id}/cancelar` | POST | `CancelarDocumentoDto` (motivo) | `DocumentoDto` | 200 / 400 (tem liquidação / período fechado) | `CancelarDocumentoHandler` |
| `/api/financeiro/parcela/{idFilial}/{id}/manutencao` | PUT | `ManutencaoParcelaDto` (vencimento, valor, portador, forma cobrança, dados bancários) | `DocumentoParcelaDto` | 200 / 400 | `ManutencaoParcelaHandler` |
| `/api/financeiro/parcela/{idFilial}/{id}/aprovar` | POST | `AprovarParcelaDto` | `DocumentoParcelaDto` | 200 / 400 | `AprovarParcelaHandler` (E14/RN-05-015) |

## Regras-chave (Matriz RTV/ROT — a extrair no `analysis` de E4)

- `Valor` > 0; `DataEmissao` ≤ data do sistema; `IdEntidade`, `IdOperacao`,
  `IdCondicaoPagamento` obrigatórios; `ValidarOperacao` (tipo da operação × `PagarReceber`).
- Soma das parcelas = `Valor` do documento (com tolerância de arredondamento do legado).
- Alteração/cancelamento bloqueados se `ValorLiquidado > 0` ou período do movimento fechado.
- `ProcessoOrigem`/`IdOrigem` preservados; documento de origem externa tem regras de
  edição restritas (`validarProcessoOrigem`).
- Cálculo de juros/multa/desconto por `DoctoItemFinanceiro` → golden tests (`CALC-xx`).

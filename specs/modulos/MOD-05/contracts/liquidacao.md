# Contrato — Liquidação (E6)

> Rascunho do `/plan`. Tela `FLiquidacaoDocumento` / `FLiquidacaoDocumentoDetalhe`
> (`FBaseProcesso`) — recebe React nesta rodada. Operação transacional; sem CRUD.

## Preparação (leitura)

| Rota | Verbo | Request | Response | Códigos | CQRS |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/liquidacao/parcelas-abertas` | GET | `[FromQuery] SelecaoParcelaDto` (idEntidade?, receberPagar, portador?, vencimentoDe/Ate?, idDominioPeriodo, page, limit) | `PagedResult<ParcelaLiquidavelDto>` (valor, saldo, juros/multa/desconto calculados até a data) | 200 | Read (usa `SelecaoDocumento` — CLR-06) |
| `/api/financeiro/liquidacao/simular` | POST | `SimularLiquidacaoDto` (parcelas + data liquidação + formas) | `SimulacaoLiquidacaoDto` (por parcela: principal, juros, multa, desconto, líquido; total por forma; troco) | 200 / 400 | Read (cálculo, sem persistir) |

## Operação

| Rota | Verbo | Request | Response | Códigos | Handler |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `/api/financeiro/liquidacao` | POST | `LiquidarParcelaDto` | `LiquidacaoResultadoDto` (movimento(s) gerado(s), parcelas baixadas, novo saldo) | 201 / 400 | `LiquidarParcelaHandler` |
| `/api/financeiro/liquidacao/{idFilial}/{idMovimento}/estornar` | POST | `EstornarLiquidacaoDto` (motivo) | `EstornoResultadoDto` | 200 / 400 | `EstornarLiquidacaoHandler` (E7) |

### `LiquidarParcelaDto`
```
idFilial: int
idDominioPeriodo: int
dataLiquidacao: date
parcelas: [ { idFilial, idDocumentoParcela, valorPrincipal, valorJuros, valorMulta, valorDesconto } ]
formas: [ { idFormaPagamento, tipo: FormaMovimento, valor, idCaixaBanco?,
            infoCartao? | infoPix? | infoChequeCliente? | infoChequeEmpresa? |
            infoDeposito? | infoCredito? | infoAbatimento? } ]
rateio?: [ { idCentroCusto, idFinClasse, valor|percentual } ]
```

### Regras (Matriz ROT `OP-Liquidar` — extrair no `analysis` de E6)
- Parcela em `Aberta` ou `Parcial`; período (`idDominioPeriodo`) **aberto**; forma
  habilitada em `DominioPeriodoFormaPagto`.
- `Σ formas` = `Σ (principal + juros + multa − desconto)` das parcelas + troco.
- Ordem de persistência: valida formas → cria `FinMovimento` (+ `FinMovimentoRateio`) →
  `FinLiquidacaoFormaMovimento` → baixa `FinDocumentoParcela.VALORLIQUIDADO`
  (+ `DATAULTIMALIQUIDACAO`, `IDSITUACAO`) → atualiza `FinDocumento` (valores/situação) →
  `FinSaldoCaixaBanco` → lança forma em `FinDominioPeriodoFormaPagto`.
- Rollback: qualquer passo falha → nada persiste.
- Cheque recebido como forma: cria/vincula `ChequeRecebido` + `ChequeRecebidoMovto`
  (custódia) — coordena com E9.
- Cálculo de juros/multa/desconto e conversão por índice → golden tests (`CALC-xx`).

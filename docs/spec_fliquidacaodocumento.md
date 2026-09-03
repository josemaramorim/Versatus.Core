# Spec Funcional: Liquidação de Documento (FLiquidacaoDocumento)

> **Tipo:** Spec Funcional — **template de referência do MOD-05 (Padrão C)**
> **Módulo:** Gestão Financeira (`Versatus.GestaoFinanceira`)
> **Versão:** 0.1 (rascunho — a completar na tarefa `E6-T08` com `legacy-validation-audit`
> + `legacy-operation-audit`)
> **Padrão de Tela:** **Padrão C — Operação / Assistente** (herda `FDocumentoSelecaoBase`)
> **Legado:** `.../aplicativo.gestao.financeira/FLiquidacaoDocumento.cs` (2580 l) +
> `FLiquidacaoDocumentoDetalhe.cs` (862 l) · objeto
> `servidor/objeto de negócio/gestao.financeira/Liquidacao.cs` (735 l) +
> `OperacaoDocumentoBase.cs` (1098 l)
> **Contrato backend:** [`specs/modulos/MOD-05/contracts/liquidacao.md`](../specs/modulos/MOD-05/contracts/liquidacao.md)
> **Padrão C:** [`.agents/skills/migrate-crud/references/padrao-c-operacao.md`](../.agents/skills/migrate-crud/references/padrao-c-operacao.md)
>
> ⚠️ Template. A extração 100% da Matriz RTV (composição) e da **Matriz ROT** (efeitos) é
> feita em `E6-T01` (análise) e `E6-T08` (frontend).

---

## 1. Objetivo

Liquidar **uma ou mais parcelas** (títulos a pagar/receber) selecionadas, informando
**uma ou mais formas de pagamento/recebimento** (dinheiro, cartão, PIX, cheque de cliente,
cheque da empresa, depósito, crédito, abatimento), com cálculo de **juros, multa e
desconto** até a data da liquidação, **rateio** opcional e **troco**. Ao confirmar, gera
`MovimentoFinanceiro`, baixa as parcelas, atualiza saldo do caixa/banco, situação do
documento e lança as formas no período do domínio — **tudo em uma transação**.

**Não é cadastro.** Sem Novo/Excluir. Botão principal: **Confirmar**.

---

## 2. Endpoints da API

Conforme `contracts/liquidacao.md`:

| Fase | Método | Rota | CQRS |
|---|---|---|---|
| Seleção | `GET` | `/api/financeiro/liquidacao/parcelas-abertas?idEntidade&receberPagar&portador&vencimentoDe&vencimentoAte&idDominioPeriodo&page&limit` | Read (usa `SelecaoDocumentoService`) |
| Simulação | `POST` | `/api/financeiro/liquidacao/simular` | Read — não persiste; devolve por parcela: principal, juros, multa, desconto, líquido; total por forma; troco |
| Execução | `POST` | `/api/financeiro/liquidacao` | **Write — `LiquidarParcelaHandler`, 1 transação** |
| Estorno | `POST` | `/api/financeiro/liquidacao/{idFilial}/{idMovimento}/estornar` | Write — `EstornarLiquidacaoHandler` (épico E7) |

`Command` (`LiquidarParcelaDto`): `idFilial`, `idDominioPeriodo`, `dataLiquidacao`,
`parcelas[]` (`{idFilial, idDocumentoParcela, valorPrincipal, valorJuros, valorMulta,
valorDesconto}`), `formas[]` (`{idFormaPagamento, tipo, valor, idCaixaBanco?, info<Tipo>?}`),
`rateio[]?`.

---

## 3. Conversão de Dados e Enums

| Enumeração | Uso | Origem |
|---|---|---|
| **PagarReceberTipo** | filtro e direção da operação | `FINDOCUMENTO.IDRECEBERPAGAR` |
| **FormaMovimento** | tipo de cada linha de forma | `formapagamentomov` / `FormaMovInfo*` |
| **SituacaoParcela** | Aberta / Parcial / Liquidada — pré-condição e efeito | `FINDOCUMENTOPARCELA.IDSITUACAO` |
| Bandeira de cartão, tipo de cheque, tipo de conta (depósito) | sub-forms condicionais | `E0-T01` / `E5` |

Valores inteiros preservados — inventário em `E0-T01` / `E5-T01`.

---

## 4. Tela — estrutura (Padrão C)

### 4.1 Cabeçalho / filtros de seleção

| Campo | Componente | Obrigatório | Regra |
|---|---|---|---|
| Domínio / Período | `Select` | ✅ | período **aberto**; alimenta `idDominioPeriodo` |
| Data da Liquidação | `DatePicker` | ✅ | base do cálculo de juros/multa/desconto; ≤ hoje (confirmar) |
| Pagar / Receber | `Select` / toggle | ✅ | filtra parcelas |
| Entidade | `Autocomplete` | — | filtro |
| Portador | `Select` | — | filtro (`lookupPortador` no legado) |
| Vencimento de / até | `DatePicker` x2 | — | filtro |

### 4.2 Grid de parcelas elegíveis (seleção)

Colunas (do legado `gridControlLiquidacao`): checkbox, Título, Parcela, Vencimento, Valor,
Acréscimo/Desconto (calculado), Total, Cliente/Fornecedor, Portador, Valor IE (índice).
Paginação materializada. Só parcelas `Aberta`/`Parcial`.

### 4.3 Painel de composição

- **Formas de pagamento** (`gridControlForma`): grid editável — Tipo, Valor, Caixa/Banco,
  botão "Detalhe" que abre modal condicional ao tipo (`FDetalheChequeCliente`,
  `FDetalhePix`, `FDetalheCartao`, `FDetalheDeposito`, `FDetalheFormaPagto` do legado →
  dialogs React). Cada forma carrega o `info<Tipo>` específico.
- **Rateio** (`rateioEditor`): centro de custo × classe (`IdFinClasse`) × valor|percentual;
  soma do rateio = valor rateável.
- **Troco** (`gridControlTroco`): quando o total informado em dinheiro excede o líquido.

### 4.4 Barra de resumo (rodapé)

`Total Pendente` (soma das parcelas selecionadas) · `Total a Liquidar` (principal + juros +
multa − desconto) · `Total Informado` (Σ formas − troco) · **`Diferença`** — Confirmar
**bloqueado enquanto `Diferença ≠ 0`** (dentro da tolerância de arredondamento do legado).

### 4.5 Fluxo

1. Filtra → grid carrega elegíveis.
2. Marca parcelas → tela chama `simular` → preenche juros/multa/desconto e totais.
3. Informa formas (+ detalhe por tipo) / rateio / troco.
4. `Diferença = 0` → **Confirmar** → `POST /liquidacao`.
5. Sucesso: toast com nº do movimento + novo saldo; limpa a seleção.
6. Erro `400`: `ValidationError[]` inline com mensagem legada.

---

## 5. Frontend — implementação

- Pasta `src/pages/Financeiro/FLiquidacaoDocumento/` (`types.ts`, `schema.ts`,
  `LiquidacaoOperacaoConfig.tsx` estendendo `BaseOperacaoConfig<LiquidarParcelaDto,
  LiquidacaoResultadoDto>`, `index.tsx`, `schema.test.ts`).
- **Se `BaseOperacaoConfig` / `useOperacaoState` ainda não existirem, esta tarefa os cria**
  (`src/components/operacao/`, `src/hooks/`) — 1ª tela Padrão C do projeto.
- `schema.ts` — `superRefine` da operação: seleção não vazia; `Σ formas` = líquido;
  período informado; rateio fechado quando presente; cada `info<Tipo>` válido conforme o tipo.
- Cálculo **nunca** recomputado no front como verdade — sempre de `simular`.
- MUI `variant="outlined"` floating label; `required` nos obrigatórios (Lei 10).
- Sem "Novo"/"Excluir" em nenhum ponto do JSX.

---

## 6. Matriz RTV (composição) — a completar em `E6-T01`/`E6-T08`

| ID | Origem Legada | Camada | Regra de composição | Mensagem Legada | Destino Backend | Destino Frontend |
|---|---|---|---|---|---|---|
| VAL-01 | `Liquidacao.cs:612 ValidarLiquidacao` | Domínio | `Σ formas` = principal + juros + multa − desconto (+ troco) | *(confirmar)* | `Result.Fail` se `!= liquido` | `superRefine` |
| VAL-02 | `Liquidacao.cs` / `DominioPeriodo` | Domínio/Estado | Período do domínio **aberto** | *"Período financeiro fechado."* (confirmar) | `if (periodo.Fechado)` | desabilita Confirmar + mensagem |
| VAL-03 | `Liquidacao.cs` | Domínio | Forma habilitada em `DominioPeriodoFormaPagto` | *(confirmar)* | valida contra o período | filtra opções de Tipo |
| VAL-04 | grid seleção | UI | Ao menos uma parcela selecionada | *"Selecione ao menos uma parcela."* | `if (!cmd.Parcelas.Any())` | `z.array().min(1)` |
| VAL-05 | `rateioEditor` | Domínio | Soma do rateio = valor rateável | *"Rateio não confere com o valor."* | validador de rateio | `superRefine` |
| VAL-06 | `FDetalheChequeCliente` etc. | UI/Filho | Campos obrigatórios do `info<Tipo>` da forma | *(por tipo)* | validação por tipo de forma | schema condicional |
| … | | | | | | |

> **Efeitos da operação** (gera movimento, baixa parcela, atualiza saldo/situação/período,
> vincula cheque recebido) → **Matriz ROT `OP-Liquidar`** em `matriz-rot.md#E6` — cobertos
> por teste de **integração + golden no backend** (`E6-T06`), **não** por `schema.test.ts`.

---

## 7. Critérios de Aceite

- **C1 (Seleção):** `parcelas-abertas` retorna só `Aberta`/`Parcial` do filtro, paginado.
- **C2 (Simulação):** `simular` devolve juros/multa/desconto por parcela e totais **iguais
  ao legado** (paridade — golden tests `CALC-xx#E6`); a tela só exibe.
- **C3 (Barra de resumo):** `Diferença` calculada; **Confirmar desabilitado enquanto ≠ 0**.
- **C4 (Múltiplas formas):** aceita N formas; cada uma com seu `info<Tipo>`; troco quando
  dinheiro excede.
- **C5 (Execução transacional):** `POST /liquidacao` → `201`; em falha de qualquer passo,
  **nada persiste** (teste de rollback em `E6-T06`).
- **C6 (Efeitos):** movimento gerado, `VALORLIQUIDADO` da parcela atualizado, situação →
  `Liquidada`/`Parcial`, `SaldoCaixaBanco` ajustado, forma lançada no período — validado
  por teste de backend (ROT), não no front.
- **C7 (Erro de negócio):** `400` com `ValidationError[]` e mensagem legada exata.
- **C8 (Sem CRUD):** nenhum botão Novo/Excluir; `schema.test.ts` cobre `VAL-xx` de
  composição.
- **C9 (Inverso):** o estorno (`E7`) reabre a parcela e reverte saldo/período.

---

## 8. Pendências e Dúvidas

- `DÚVIDA-LQ1`: mensagens legadas exatas de `VAL-01..06` (resolver em `E6-T01` com o legado).
- `DÚVIDA-LQ2`: tolerância de arredondamento aceita na `Diferença` (regra do legado —
  `legacy-calc-parity`).
- `DÚVIDA-LQ3`: ordem exata de persistência e todas as tabelas tocadas — confirmar em
  `matriz-rot.md#E6` (`plan.md §5` tem a versão preliminar).
- `DÚVIDA-LQ4`: liquidação parcial de uma parcela — permitida? Como o legado trata resíduo?
- `DÚVIDA-LQ5`: `FLiquidacaoDocumentoDetalhe.cs` (862 l) — o que ele adiciona (edição de
  item financeiro por parcela?); incluir no escopo de `E6-T01`.
- `DÚVIDA-LQ6`: cheque recebido como forma — cria o `ChequeRecebido` aqui ou exige
  cadastro prévio? Coordena com E9.

# Análise E1 — Classes-base legadas do MOD-05

> **Tarefa:** E1-T01 (`tipo: analysis`) · **Data:** 2026-09-08 · **Depende de:** E0-T04
> **Regra guia:** CLR-04 / spec §3.3 — cada base legada vira **classe abstrata POCO só de
> dados** em `Domain/Bases/` **+** **serviço/handler compartilhado** em `Application/Bases/`
> para o comportamento. Esta análise separa DADOS de COMPORTAMENTO por classe.
> Alimenta `matriz-rtv.md#E1` (31 `VAL`), `matriz-rot.md#E1` (11 `OP` + 9 `CALC`) e as
> tarefas `E1-T02`/`E1-T03`/`E1-T04`.

---

## 1. Cadeia de herança legada e destino (spec §3.3)

```
ObjectBase (MarshalByRefObject)              → removido (DEC-002)
  └ ObjectPersist                            → persistência: DbContext + Repository; orquestração no Handler (DEC-003)
      └ ObjectMaster                         → agregado: List<T> privada + IReadOnlyList<T> (Artigo V) + change tracking EF
          └ ObjectGenerated                  → ValidarOrigemGerador (OP-E1-04)
              └ ObjectGenerator              → geração de Id: GeradorSequencialService + ValueGeneratedNever()
```

Hooks herdados relevantes (todos em `servidor/framework/servidor.framework/ObjetoNegocio.cs`):

| Hook legado | Linha | Destino |
| :--- | :--- | :--- |
| `ExecutarPersistir(trans)` | 1588 | `PersistenciaDocumentoBase` abstrato — orquestra (OP-E1-01) |
| `ExecutarExcluir(trans)` | 1537 | idem — `ExcluirAsync` (OP-E1-02) |
| `OnBeforeExecutarPersistir` / `OnAfterExecutarPersistir` | 1632 / 1640 | ganchos `virtual` no handler de base |
| `PersistirRateio(trans)` | 1656 | `RateioServiceBase.PersistirAsync` (motor real = MOD-02, CLR-01) |
| `PersistirPeriodoFormaPagto(trans)` | 1679 | E2 (período) — `IDadosPeriodoFormaPagto` fica no `GestaoFinanceira.Domain` (CLR-05) |
| `ValidarRateio()` | 1743 | `RateioServiceBase.ValidarAsync` → `ValidationRateioContainer` (E0-T03) (OP-E1-03) |
| `ValidarOrigemGerador()` | 2329 | `virtual` no handler (OP-E1-04) |
| `GenerateId(trans)` / `[AutoSequencial]` | (ObjectGenerator) | `GeradorSequencialService` + `.ValueGeneratedNever()` |

**Sincronização (`CreateNewSincronizacao`) e cache (`InvalidarCache`) do legado: fora de escopo** —
não têm equivalente no novo sistema (change tracking do EF + sem cache de objeto).

---

## 2. Por classe — DADOS (→ POCO abstrata) × COMPORTAMENTO (→ serviço)

### 2.1 `DocumentoFinanceiroBase` (1303) → `DocumentoFinanceiroBase` (POCO) + `PersistenciaDocumentoBase` (serviço)

**Dados compartilhados (POCO abstrata `Domain/Bases/DocumentoFinanceiroBase.cs`):**
`NumeroDocumento`, `NumeroCedente`, `Historico`, `Valor` (`decimal`), `ValorConvertido` (`decimal`),
`DataEmissao`, `DataInclusao?`, `HoraInclusao?`, `PagarReceber` (`PagarReceberTipo`),
`IdEntidade`, `IdOperacao`, `IdTipoDocumento`, `IdCondicaoPagamento`, `IdIndiceEconomico`,
`IdIndiceConversao`, `IdPortador`, `IdFormaCobranca`, `IdOrigem`, `IdProcessoOrigem` (`ProcessoOrigem`),
`IdFilial`. Sem os `Lookup<>` (viram `int` + navegação opcional resolvida por serviço/consulta).

**Comportamento (serviço `Application/Bases/PersistenciaDocumentoBase.cs` abstrato):**
- `ValidarOperacao` (VAL-E1-01), `ValidarEntidade` (VAL-E1-02..05), `ValidarNumeroDocumento`
  (VAL-E1-06), `ValidarTipoDocumento` (VAL-E1-07).
- `AplicarOperacao` (OP-E1-07), `AplicarCondicaoPagamento` (OP-E1-08),
  `CalcularValorConvertido` (OP-E1-09 / CALC-E1-09).
- `ExecutarPersistir`/`ExecutarExcluir` (OP-E1-01/02) — orquestração no handler.
- Ganchos `virtual` que as subclasses (Documento, E4) implementam: `SetParcelasCondicaoPagto`,
  `RecalcularTributoRateio`, `CarregarComissionadoPadrao`, `CarregaItemFinanceiroOperacao`,
  `GetIdOrigemGerador`, `GetProcessoOrigemGerador`, `GetIdDocumentoFinanceiro`, `RetornarValorRateio`.
- **Estrangulamento já existente:** `ValidarEntidade`/`CarregarCondicaoPagamentoDefault`/
  `GetPortadorDefault` já chamam a API do AcessoGlobal via `FinancialStranglerHelper` — no novo
  sistema é chamada `int` lógica ao serviço do AcessoGlobal (Artigo VIII, CLR-10).

### 2.2 `OperacaoDocumentoBase` (1098) → `OperacaoDocumentoBase` (POCO parcial) + `OperacaoDocumentoBaseHandler` (abstrato)

Pai de `Liquidacao` (E6) e `Reversao` (E8). É mais **comportamento** que dados.

**Dados (POCO abstrata):** `PagarReceber`, `IdOperacao`, `LiquidacaoParcial` (`bool`),
flags `usaClasse`/`usaCentroCusto`/`usaProjeto` (derivadas de parâmetro — no novo sistema
resolvidas por serviço, **não** persistidas), coleção `Documentos` (seleção — `SelecaoDocumento`
é serviço, CLR-06).

**Comportamento (`Application/Bases/OperacaoDocumentoBaseHandler.cs` abstrato):**
- `AtualizarRateio` (OP-E1-05) — usa `RateioContainer` (E0-T03) + porta para o motor do MOD-02.
- `GeraRateioSelecionado` (VAL-E1-08), `CarregarItemFinanceiroRateio` (VAL-E1-09),
  `AddItemFinanceiroRateio`, `ValidarOperacao(int)` (VAL-E1-10).
- `SetDefaultAcao`, `SetLiquidacaoParcial`, `SetPagarReceber` (com rollback do tipo anterior),
  `ReiniciarValoresIntegrais`.
- `virtual`/`abstract` implementados por E6/E8: `GerarRateioItemFinanceiro` (OP-E1-06),
  `CriarItemFinanceiroLista`, `RetornarValorBaseRateio`, `LimparDados`, `RetornarRateioMovto`,
  `CarregarOperacoesDefault`, `GetOperacaoTipoReceber/Pagar/MovimentoCartao`.

### 2.3 `ItemFinanceiroBase` (456) → `ItemFinanceiroBase` (POCO) + `CalculadoraItemFinanceiroBase` (serviço)

**Dados (POCO abstrata `Domain/Bases/ItemFinanceiroBase.cs`):**
`IdItemFinanceiro`, `Dias` (`int`), `Valor` (`decimal`), `IdAplicar` (`ItemFinanceiroAplicar`),
`IdAplicacao` (`ItemFinanceiroAplicacao`), `AplicarComissao` (`bool`), `IdCalculo`
(`CalculoItemFinanceiro`), `IdIndiceEconomico?`. **Sem** `[TableColumn]` (Fluent API — Artigo III).

**Comportamento (serviço `Application/Bases/CalculadoraItemFinanceiroBase.cs`):**
- **Cálculo puro** (`legacy-calc-parity` — CALC-E1-01..06): `ExecutarCalculo`, `CalcularDias`,
  `CalcularComposto`, `ConsiderarDiasParaCalculo` (×2), `RetornarIndiceConvertido`, `DefinirValor`,
  `CalcularItem`, `GetValorItemFinanceiroBoleto`. **Transcrever sem refatorar (Regra 5); `decimal` exato.**
- `ValidarItemFinanceiro` (VAL-E1-11), `SetarInfoItemFinanceiro`, `Assimilar`.
- Faixa de valor/dias (VAL-E1-12) → guard no serviço.

### 2.4 `ParcelaGeral` (609) → `ParcelaGeralBase` (POCO — **só dados**, sem serviço próprio)

**100% dados, zero validação/operação** (auditoria confirmou: só propriedades). Candidata a
POCO abstrata pura. Campos: número da parcela, situação (`SituacaoDocumento` — VAL: não;
a parcela reusa o enum do documento, ver `enums.md`), valores, datas, portador, item
financeiro acumulado, colunas de boleto/remessa (**marcar `[E14]`** — integração bancária,
CLR-03). O detalhamento coluna-a-coluna é do `data-model.md` no `E4-T02`.

### 2.5 `ParcelaBase` (772) → `ParcelaBase` (POCO) + `GeracaoParcelasService` (serviço)

**Dados (POCO abstrata `Domain/Bases/ParcelaBase.cs`):**
`NumeroParcela`, `Valor` (`decimal`), `DataVencimento`, `DataCobranca?`, `PercentualDivisao`,
`PercentualDivisaoNovo`, `dataVencimentoDefinida?`, `IdCaixaBancoParcela?`, flag `validar`
(controle — não persiste), `Liquidada` (`bool`).

**Comportamento (serviço `Application/Bases/GeracaoParcelasService.cs`):**
- Validações de vencimento: `ValidarVencimento` (VAL-E1-13..15), `ValidarVenctoParcelaAlterada`
  (VAL-E1-16).
- Validações de número: `ValidarNumeroParcela` (VAL-E1-17..21) — inclui **renumeração** das
  parcelas subsequentes.
- Validações de valor: `ValidarValorMinimo` (VAL-E1-22 / CALC-E1-07), `ValidarValor` (VAL-E1-23),
  `CalcularTotalParcelas` (VAL-E1-24..26).
- `ValidarCaixaBanco` (VAL-E1-27 — depende de E3 `CaixaBanco`/`ContaBancaria`).
- Rebalanceamento (OP-E1-10): `RecalcularParcelas`, `RecalcularParcelasRestante`,
  `CorrigirDiferenca` (arredondamento na 1ª/última por `ParcelamentoArredondamento`),
  `CalcularPercentualNovo` (CALC-E1-08).
- `SetParcela`, `SetValor`, `SetNumeroParcela`, `SetDataVencimento`, `SetDataCobranca`,
  `RepetirDiaVencimento`, `SetCaixaBancoBoletoDefault`.

### 2.6 `FormaMovInfo` (146) → `FormaMovInfoBase` (POCO) + gancho no serviço de forma de movimento

**Dados:** referência à `FormaPagamentoMov` (por `int`/composição), `ValorDisponivel` (`decimal`),
`LimitarValorDisponivel` (`bool`), `Info` (abstrato — cada especialização de forma define).

**Comportamento:** `RetornarValor` (identidade — gancho) e `DefinirValor` (VAL-E1-31 — teto em
`ValorDisponivel`), `GetDisponivel` (texto — vira dado no DTO). Base pequena; o comportamento
concreto está nas 11 `FormaMovInfo*` do E5.

### 2.7 `FechamentoCaixaBase` (448) → `FechamentoCaixaBase` (POCO) + `FechamentoCaixaService` (serviço)

**Dados (POCO abstrata `Domain/Bases/FechamentoCaixaBase.cs`):**
`Situacao` (`int` — status do fechamento do domínio/período; enum definido no E2),
`Quantidade` (`int`), `ValorInformado` (`decimal`), `ValorCalculado` (`decimal`),
`ValorConferencia` (`decimal`), `MoedaContagem` (`string`), `FechamentoPdv` (`bool`),
`TipoFormaPagto` (`FormaPagtoTipo`).

**Comportamento (serviço `Application/Bases/FechamentoCaixaService.cs`):**
- `PermiteDigitarQtde` (VAL-E1-29), `PermiteDigitarValor` (VAL-E1-30), `PermiteEditarColuna`,
  `PermiteEditarChequeRecebido`.
- `RetornarListaFormaDinheiroEditado` (contagem de dinheiro por moeda — parâmetro
  `MoedaParaContagem`), `RetornarListaFormaChequeRecebidoEditado`.
- `CalcularQtdeValorEditado` / `SetQtdeValor` (OP-E1-11).
- **A máquina de estados do período (abrir/fechar/fechar tesouraria) é do E2** — aqui só o
  fechamento por forma de pagamento.

---

## 3. Resumo — o que cada tarefa seguinte consome

| Tarefa | Consome desta análise |
| :--- | :--- |
| **E1-T02** (POCOs) | §2 — os blocos "Dados" de cada classe → `Domain/Bases/*.cs` (6 POCOs; `ParcelaGeralBase` é pura). `bool` simples, `decimal` para dinheiro, auditoria anulável (Artigo III). |
| **E1-T03** (serviços) | §2 — os blocos "Comportamento" → `Application/Bases/*.cs` (`PersistenciaDocumentoBase`, `OperacaoDocumentoBaseHandler`, `CalculadoraItemFinanceiroBase`, `GeracaoParcelasService`, `FechamentoCaixaService`, `RateioServiceBase`). Só esqueleto + contratos; lógica concreta nos épicos que herdam. `OP-E1-01..11`. |
| **E1-T04** (testes) | `matriz-rtv.md#E1` (1 `[Fact]` por `VAL-E1-xx`) + `matriz-rot.md#E1` `CALC-E1-01..09` (golden, `decimal` exato). |

## 4. Pendências / dependências cross-épico

- **VAL-E1-27** (`ValidarCaixaBanco`) precisa de `CaixaBanco`/`ContaBancaria` do **E3** — o
  teste de `E1-T04` mocka a porta; a integração real fecha no E3.
- **OP-E1-05** (`AtualizarRateio`) precisa do **motor de rateio do MOD-02** (CLR-01) — o
  `RateioServiceBase` de E1-T03 expõe a porta; a lógica completa entra no **E5**.
- `Situacao` de `FechamentoCaixaBase` e `PersistirPeriodoFormaPagto` → enum e máquina de
  estados definidos no **E2**.
- Colunas de boleto/remessa de `ParcelaGeral` → **E14**.
- **VAL-E1-28** (setter linha 609 de `ParcelaBase`) — detalhe a extrair no `E1-T02` ao portar
  o setter; linha registrada para não ficar órfã.

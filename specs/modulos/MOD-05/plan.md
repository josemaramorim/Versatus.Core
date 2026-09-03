# Plano Técnico — MOD-05: Gestão Financeira

> **Versão:** 1.0 | **Data:** 2026-09-03 | **SDD etapa 3** (`sdd-plan`)
> **Baseia-se em:** [`spec.md`](./spec.md) v2.1 · [`clarify.md`](./clarify.md) (CLR-01..13) ·
> [`data-model.md`](./data-model.md) · [`constitution.md`](../../memory/constitution.md) v1.0
> **Companion:** [`research.md`](./research.md) · [`contracts/`](./contracts/)

Todas as decisões deste plano estão ancoradas na spec aprovada. Nenhum escopo novo — o que
não estiver aqui e for necessário volta para `/specify` ou `/clarify`.

---

## 1. Projeto e estrutura

### 1.1 Projetos novos

| Projeto | TFM | Referências | Papel |
| :--- | :--- | :--- | :--- |
| `src/Versatus.SharedKernel` | `net10.0` | `Versatus.Framework` | Épico E0 — enums financeiros, `Lookup`, container de rateio, interfaces transversais |
| `src/Versatus.GestaoFinanceira` | `net10.0` | `Versatus.Framework`, `Versatus.SharedKernel`, `Versatus.AcessoGlobal` | Domain + Application + Infrastructure + Api do módulo |
| `tests/Versatus.GestaoFinanceira.Tests` | `net10.0` | acima + `Microsoft.EntityFrameworkCore.InMemory`, `xunit` | RTV + ROT + paridade |

Pacotes EF Core **10.x** (alinhado a `Versatus.GestaoTributo.csproj`). **Sem**
`ProjectReference` para `Versatus.Faturamento` (CLR-09).

### 1.2 Estrutura de `Versatus.GestaoFinanceira` (CLR-13)

```
Api/Controllers/                 — controllers finos por área
Application/
  Services/                      — I<X>Service + <X>Service (CRUD e consultas)
  Handlers/                      — <Operacao>Handler (Liquidar, Estornar, Reverter, Fechar, Acertar…) — 1 transação cada
  Bases/                        — serviços/handlers compartilhados que substituem o comportamento das bases legadas (CLR-04)
Domain/
  Bases/                        — POCOs abstratas só de dados (DocumentoFinanceiroBase, ItemFinanceiroBase, ParcelaBase, FormaMovInfo, FechamentoCaixaBase)
  Dominio/  Bancos/  Documentos/  Movimentos/  Liquidacao/  Reversao/
  Cheques/  Adiantamentos/  DRE/  Cobranca/  TransacaoFilial/
  Consultas/                    — DTOs/records de leitura (views vw*) + SelecaoDocumento (serviço, CLR-06)
  Repositories/                 — I<X>Repository
  DTOs/                         — records de request/response (por área)
Infrastructure/
  GestaoFinanceiraDbContext.cs  (Write) / GestaoFinanceiraReadDbContext.cs (Read, NoTracking)
  Mappings/                     — 1 arquivo por entidade (IEntityTypeConfiguration)
  Repositories/                 — GestaoFinanceiraRepositorioBase<T> + implementações
DependencyInjection/ServiceCollectionExtensions.cs  — AddGestaoFinanceira()
```

`ConfigureConventions`: `Properties<bool>().HaveConversion<short>()` (colunas `smallint`).
Registro no `src/Versatus.WebAPI/Program.cs`: 2 DbContexts + `AddSharedKernel()` +
`AddGestaoFinanceira()` + `AddApplicationPart(...)`.

---

## 2. Épico E0 — `Versatus.SharedKernel` (escopo mínimo — CLR-02)

| Item | Origem legada | Nota |
| :--- | :--- | :--- |
| Enums financeiros | `Projeto.Geral.Enumerado` / `.EnumeradoObjeto` | `ProcessoOrigem`, `PagarReceberTipo`, `OperacaoTipo`, `SituacaoDocumento`, `SituacaoParcela`, `SituacaoMovimento`, `SituacaoCheque`, `TipoMovimento`, `TipoLancamento`, `PeriodoTipo`, `FormaMovimento`, `SequencialTipo` + os finos de `research.md §2`. **Valores inteiros preservados.** |
| `Lookup<T>` | `Servidor.Framework` `Lookup` | Reescrito como wrapper leve sobre cache/repo (não a implementação Gentle). Ver `research.md §4`. |
| Container de rateio | `Projeto.Geral` `RateioMovto` container / `ValidationRateioContainer` | Estrutura de acumulação de rateio usada por documento/movimento. |
| Interfaces transversais (CLR-05) | `Interface.GestaoFinanceira` | `IMovimentoPeriodo`, `IDadosPeriodoFormaPagto`, `IDadosRateioFinanceiro`, `IDadosComissao` |
| **Reusa de `Versatus.Framework`** | — | `Result<T>` / `ValidationResult` / `ValidationError`, `IContextoExecucao`, `PagedResult<T>`, `GeradorSequencialService`, `VersatusException`. **Não duplicar.** |

Ampliar o SharedKernel só quando um épico posterior comprovar necessidade (registrar no
`clarify.md` como novo CLR).

---

## 3. Épicos — ordem de execução

> **Correção de ordem (achado do `data-model.md`):** `FINDOMINIO.IDFINCAIXABANCO` é
> **`NOT NULL`** → `Dominio` depende de `CaixaBanco`. Portanto **Caixa/Banco é executado
> antes de Domínio/Período**, invertendo o que a `spec.md §6` listou provisoriamente. Os
> números de épico da spec são identificadores, não sequência; a sequência real é a
> abaixo.

| # exec | Épico (id da spec) | Entidades / operações | Depende de | Análise prévia (skill) |
| :--- | :--- | :--- | :--- | :--- |
| 1 | **E0** SharedKernel | §2 | Framework | — |
| 2 | **E1** Bases | `DocumentoFinanceiroBase`, `OperacaoDocumentoBase`, `ItemFinanceiroBase`, `ParcelaGeral`/`ParcelaBase`, `FormaMovInfo`, `FechamentoCaixaBase` → POCO abstrata + serviço (CLR-04) | E0 | `legacy-validation-audit` + `legacy-operation-audit` em `DocumentoFinanceiroBase` (1303) |
| 3 | **E3** Caixa e Banco | `CaixaBanco`, `CaixaBancoUsuario`, `ContaBancaria` (só entidade — CLR-03), `SaldoCaixaBanco`, `SaldoRateio`, `Cobrador`, `IndiceConversor` | E1 | `ContaBancaria` (1704) — marcar colunas E14 |
| 4 | **E2** Domínio e Período | `Dominio`, `DominioPeriodo`, `DominioPeriodoFormaPagto`, `DominioPeriodoLacto`, `DominioPeriodoFechamento`(+Detalhe), `DominioPeriodoLog`¹, `DominioResponsavel`, `DominioUsuario`, `PeriodosAbertos` (serviço). Operações: **AbrirPeriodo**, **FecharPeriodo**, **FecharTesouraria**, **FecharCaixa** (`legacy-operation-audit`) | E3 | `Dominio` (1717), `DominioPeriodo` (1017) |
| 5 | **E4** Documento e Parcela | `Documento`, `DocumentoParcela`, `DocumentoMovto`, `DoctoItemFinanceiro`, `DoctoMovtoItemFinanceiro`, `DocumentoTributo`, `DocumentoCartao`, `DocumentoComissionado`, `DoctoCancelado`(+Parcela). Operações: **IncluirDocumento** (+ parcelamento pela condição de pagamento), **CancelarDocumento**, **ManutençãoParcela**, **cálculo juros/multa/desconto** (`legacy-calc-parity`) | E1, E3 | `Documento` (2803), `DocumentoParcela` (2111) — 55 col. cada |
| 6 | **E5** Movimento e Formas de Pagamento | `MovimentoFinanceiro`, `MovimentoFinanceiroUpdate`, `MovtoFinanceiroCheque`, `MovtoFinanceiroRateio`, `formapagamentomov`, `FormaMovInfo*` (11), `TrocoDinheiro`, `ItemFinanceiro`, `ItemFinanceiroOperacao`, `AplicacaoItemFin`(+Geral), `ContraPartidaRateioDoctoFinanceiro`, `ManutencaoRateioFinanceiro`. Operações: **IncluirMovimento**, **ExcluirMovimento**, **Conciliar**, **cálculo/validação de saldo** (`legacy-calc-parity`) | E2, E4, **+ MOD-02: rateio migrado** (CLR-01) | `MovimentoFinanceiro` (2136) |
| 7 | **E6** Liquidação | `Liquidacao`, `LiquidacaoFormaMovimento`. Operação: **LiquidarParcela** (1..N formas, gera `MovimentoFinanceiro`, baixa parcela, atualiza saldo/situação/período) | E4, E5 | — (`legacy-operation-audit` + `legacy-calc-parity`) |
| 8 | **E7** Estorno de Liquidação | `LiquidacaoEstorno`, `LiquidacaoEstornoFormaPagto`, `LiquidacaoEstornoItemFin`. Operação: **EstornarLiquidação** (inverso de E6, reabre parcela, ajusta saldo/período, valida domínio) | E6 | `LiquidacaoEstorno` (2617), `LiquidacaoEstornoFormaPagto` (1074) |
| 9 | **E8** Reversão | `Reversao`, `ReversaoDoctoParcela`, `ReversaoItemFinanceiro`, `AplicacaoItemFinReversao`. Operação: **ReverterMovimento/Parcela** (lançamento espelho + correção de rateio) | E4, E6 | `Reversao` (1985) |
| 10 | **E9** Cheques | `Cheque` (VO), `ChequeRecebido`, `ChequeRecebidoMovto`, `ChequeEmitidoMovto`, `ChequeEmitidoMovtoUpdate`, `TalaoCheque`, `SuprimentoCheque`, `MotivoDevolucaoCheque`. Operações: custódia/depósito/compensação/devolução(alínea)/repasse/cancelamento; **máquina de estados `SituacaoCheque`** | E3, E5 | `ChequeRecebidoMovto` (1692), `ChequeEmitidoMovto` (1667), `TalaoCheque` (1103), `ChequeRecebido` (1105) |
| 11 | **E10** Adiantamentos e Acertos | `Adiantamento` (CRUD), `AdtoAcerto` (**operação** — CLR-12), `AdtoAcertoDistribuicao`(+Rateio), `AdtoAcertoMovto`, `AdtoLanctoEntidade`. Operação: **AcertarAdiantamento** (distribui saldo, rateio, movimento, participa do período) | E4, E5 | `AdtoAcerto` (1527) |
| 12 | **E11** DRE, Projeção, Seleção | `DRE`, `DRETitulo`(+Classe/+Operacao), `ProjecaoFluxoCaixa`¹, `ProjecaoFluxoCaixaLacto`¹, **`SelecaoDocumento` → serviço** (CLR-06), `View/{ClasseAvulsaDRE, DREImpressaoView}` (modelo de dados — CLR-08) | E4, E5 | `SelecaoDocumento` (1226) |
| 13 | **E12** Cobrança e Transação entre Filiais | `ProgramacaoCobranca`(+Parcela), `TransacaoFilial/{TransacaoFilial, TransacaoFinanceira, FilialMovimento}`. Operação: **ExecutarTransacaoFilial** (lançamentos casados nas 2 filiais, sequencial global) | E4 | — |
| 14 | **E13** Consultas | ~30 `Consultas/*` (views `vw*`) → records de leitura + endpoints `GET` + `SelecaoDocumento` | épicos correspondentes | `Consultas/DocumentoParcConsulta` (1508) |
| 15 | **E14** Integração Bancária | `View/ArquivoRemessaView`, `View/ArquivoRetorno{Base,BaseView,240View,400View,ViewOutro}`, `View/{AprovacaoDoctoPagarView, LiberacaoDoctoPagarView}`, boleto, OFX, `FINCNAB(+DETALHE)`. Consome as colunas E14 de `ContaBancaria` e `DocumentoParcela` | E3, E6, E9 | ver `research.md §1` |

¹ `FINPROJECAOFLUXOCAIXA(+LACTO)` e `FINLOGDOMINIOPERIODO` **não estão no banco de dev** —
a tarefa `analysis` do épico deve reconfirmar (ver §7 R-1).

E9/E10/E11/E12 podem ser paralelizados após E5+E6.

---

## 4. Reconciliação com o estrangulamento (CLR-10)

`Servidor.Strangler/Gestao.Financeira/DTOs/` expõe só `EntidadeDto`, `ClienteDto`,
`FornecedorDto`, `ParametroDto` — **entidades do AcessoGlobal**. O `Documento.cs` legado já
chama a API nova do AcessoGlobal para esses lookups. **Nada a reconciliar do lado
financeiro**; nenhum contrato financeiro foi publicado. Quando a API do
`Versatus.GestaoFinanceira` entrar, o legado continua inalterado nesse ponto. Se
futuramente formos estrangular consultas financeiras do legado, isso será um novo
`prompts-execucao/` + CLR.

---

## 5. Padrão de transação por operação (DEC-003 / Artigo VII)

Cada operação = **um `Handler`** que abre `BeginTransactionAsync`, chama serviços de
domínio (que não tocam `SaveChanges`), chama `SaveChangesAsync` uma vez, `CommitAsync`;
rollback em qualquer falha. Entidades de domínio puras não recebem `DbContext`.

| Operação | Handler | Entidades/tabelas tocadas (ordem) | Efeitos colaterais | Inverso |
| :--- | :--- | :--- | :--- | :--- |
| Liquidar parcela | `LiquidarParcelaHandler` | valida formas → `FinMovimento`(+`FinMovimentoRateio`) → `FinLiquidacaoFormaMovimento` → baixa `FinDocumentoParcela.VALORLIQUIDADO` → `FinDocumento` (situação/valores) → `FinSaldoCaixaBanco` → `FinDominioPeriodoFormaPagto` | saldo caixa/banco += valor; parcela→Liquidada/Parcial; sequencial de movimento consumido; forma de pagto lançada no período | `EstornarLiquidacaoHandler` |
| Estornar liquidação | `EstornarLiquidacaoHandler` | `FinLiquidacaoEstorno`(+`FormaPagto`/`ItemFin`) → reverte `FinMovimento` → repõe `FinDocumentoParcela` → `FinDocumento` → `FinSaldoCaixaBanco` → período | valida período aberto; parcela volta a Aberta/Parcial; saldo −= valor | — |
| Reverter movimento/parcela | `ReverterHandler` | `FinReversao`(+`DoctoParcela`/`ItemFinanceiro`) → lançamento espelho em `FinMovimento` → correção de rateio → parcela/documento | `PersistirCorrecaoRateioReversao`; valida origem geradora | — |
| Incluir documento | `IncluirDocumentoHandler` | `FinDocumento` → gera N `FinDocumentoParcela` pela condição de pagamento → `FinDoctoItemFinanceiro` → rateio | sequenciais consumidos; cálculo de datas/valores das parcelas | `CancelarDocumentoHandler` |
| Cancelar documento | `CancelarDocumentoHandler` | `FinDoctoCancelado`(+`Parcela`) → `FinDocumento`/`FinDocumentoParcela` (situação/valorCancelado) | bloqueia se houver liquidação; valida `ProcessoOrigem` | — |
| Abrir/Fechar período | `AbrirPeriodoHandler` / `FecharPeriodoHandler` | `FinDominioPeriodo` → `FinDominioPeriodoFechamento`(+`Detalhe`) → `FinLogDominioPeriodo` → `FinDominio.IDFINDOMINIOPERIODO` | `CalcularTotalFormasPagto`: conferido × calculado; divergência em detalhe | reabertura (permissão) |
| Acertar adiantamento | `AcertarAdiantamentoHandler` | `FinAdtoAcerto` → `FinAdtoAcertoDistribuicao`(+`Rateio`) → `FinAdtoAcertoMovto` → `FinMovimento` → período | consome saldo do `FinAdiantamento`; rateio | estorno de acerto |
| Fechar caixa | `FecharCaixaHandler` | `FinDominioPeriodoFechamento` totaliza por forma; `FinSaldoCaixaBanco` | divergência registrada | reabrir |
| Transação entre filiais | `ExecutarTransacaoFilialHandler` | `FinTransacaoFilial` → `FinTransacaoFinanceira` + `FinFilialMovto` → `FinMovimento` (2 filiais) | lançamentos casados; sequencial global | — |
| Gerar remessa (E14) | `GerarRemessaHandler` | seleciona parcelas (`SelecaoDocumento`) → arquivo CNAB → `FinCnab`(+`Detalhe`) → `FinDocumentoParcela` (nosso-número/remessa) | `ContaBancaria.ULTIMONOSSONUMERO`/`ULTIMONUMEROARQUIVO` incrementados | — |
| Processar retorno (E14) | `ProcessarRetornoHandler` | lê CNAB → por ocorrência: liquida/atualiza `FinDocumentoParcela` → dispara `LiquidarParcelaHandler` quando baixa | idem liquidação | — |

A **ordem exata de persistência e as pré-condições** de cada operação são extraídas linha a
linha pela `legacy-operation-audit` (Matriz ROT) na tarefa `analysis` do épico.

---

## 6. Estratégia de testes

| Camada | Ferramenta | Cobertura |
| :--- | :--- | :--- |
| Mapeamento EF | `Microsoft.EntityFrameworkCore.InMemory` | 1 teste por entidade: materializa e persiste; valida PK composta e nulidade |
| RTV (validações) | `xunit` `[Fact]` | 1 por linha `VAL-xx` de `matriz-rtv.md` |
| ROT (operações) | `xunit` integração (InMemory ou SQL de teste) | 1 por linha `OP-xx`: afirma estado final de **todas** as entidades tocadas + rollback |
| Paridade de cálculo | `xunit` `[Theory]` + golden `csv` | 1 por linha `CALC-xx`; igualdade **exata** de `decimal` |
| Contrato/API | teste de integração de controller | retornos `Result<T>` → `400`; paginação materializada |

**Captura de golden values:** o banco de dev está **quase todo vazio** (ver §7 R-2) — os
golden values de cálculo virão de (a) execução do legado com entradas controladas, ou
(b) cálculo manual conferido pelo usuário. Documentado em `research.md §5`.

---

## 7. Riscos e mitigações

| # | Risco | Mitigação |
| :--- | :--- | :--- |
| **R-1** | Banco de dev tem só 59 tabelas `Fin%`; faltam `FINPROJECAOFLUXOCAIXA(+LACTO)` e `FINLOGDOMINIOPERIODO`. | Tarefa `analysis` de cada épico reconfirma a existência; se ausente, solicitar dump de schema de **produção** antes de mapear (`data-model.md §4`). |
| **R-2** | Maioria das tabelas vazia → sem golden values reais para paridade de cálculo. | Golden via execução do legado / conferência manual (`research.md §5`); marcar origem de cada caso. |
| **R-3** | Nulidade de auditoria **não-uniforme** entre tabelas (`FINDOCUMENTO` tem `DATAALTERACAO`/`HORA*` `NOT NULL`). | `data-model.md` obriga extração coluna a coluna do `fin_columns.txt`; `sdd-analyze` V5 valida. |
| **R-4** | PK composta com **ordem de coluna variável** (`FINDOCUMENTOPARCELA`/`FINCHEQUERECEBIDOMOVTO` têm filial 1º). | `HasKey(new { ... })` na ordem física; teste de mapeamento por entidade. |
| **R-5** | `ContaBancaria` e `DocumentoParcela` (55 col. cada) misturam núcleo + integração bancária na mesma tabela. | Entidade com todas as colunas (Regra 4); comportamento de E14 em serviços dedicados (CLR-03). |
| **R-6** | Base de rateio (`RateioMovtoItem`/`RateioMovto`/`ManutencaoRateio` + `FinRateio`/`FinClasse`) é do MOD-02 e não migrada. | Pré-tarefa no MOD-02 antes do E5 (CLR-01); `plan.md` do MOD-02 assume essa dependência. |
| **R-7** | `text` (TEXT legado) em `HISTORICO` etc. — deprecado no SQL Server. | `string?` + `.HasColumnType("text")`; não alterar schema (Artigo II). |
| **R-8** | `BoletoNet`/`BarcodeLib` são .NET Framework. | Substituto .NET 10 avaliado em `research.md §1`; E14 isolado no fim. |
| **R-9** | `ProcessoOrigem` (563 usos) amarra o financeiro a 8 módulos, a maioria inexistente no novo sistema. | `int` + `enum`; nenhuma navegação; regras de origem replicadas por parâmetro (RN-05-007/019). |
| **R-10** | Enums com valores inteiros não documentados (`IDSITUACAO`, `IDAPLICAR`, `IDCALCULO`…). | Inventário fino em `research.md §2` a partir de `Projeto.Geral` + `docs/analise_integracao_enumerados.md`, por épico. |

---

## 8. Saída deste plano

- [x] `plan.md` (este) · `research.md` · `data-model.md` · `contracts/` (E3, E4, E6 + índice)
- [x] `legacy-schema/` (extract real do banco — ground truth)
- Gate `constitution §12.1/§12.3/§12.4`: ver `analyze-report.md` (a gerar no `/analyze`)
- **PARE — aprovação do usuário antes de `/tasks`.**

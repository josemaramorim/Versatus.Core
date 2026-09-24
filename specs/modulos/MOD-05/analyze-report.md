# Análise de Cobertura — MOD-05

> **Data:** 2026-09-03 | **Constituição:** v1.0 | **Escopo desta rodada:** SDD sem código
> (spec + clarify + plan + data-model + research + contracts + tasks).
>
> **Veredito parcial:** 🟡 **APROVADO PARA PLANEJAMENTO** — V1, V2, V6, V7 executados e
> sem achado CRÍTICO/ALTO. V3, V4, V5 **não executáveis nesta rodada** (dependem das
> matrizes RTV/ROT e do detalhamento de colunas, que são produzidos por épico nas tarefas
> `E?-T01` durante `/implement`).
>
> **Gate incremental por épico:** V3/V4/V5 **não** ficam represados até o fim. Assim que a
> tarefa `analysis` de um épico gera `matriz-rtv.md#E?` / `matriz-rot.md#E?`, roda-se
> `/analyze MOD-05 --epico E?` (modo incremental de `sdd-analyze §5`) e o veredito ✅ do
> épico é pré-condição para o código daquele épico entrar em `develop`. As rodadas ficam
> registradas na seção "Rodadas incrementais" abaixo.
>
> O **gate duro completo** (`sdd-analyze` modo completo) roda em `Z-T02` como
> **consolidação** — V1–V7 no módulo inteiro + verificação cross-épico — antes de qualquer
> merge final.

---

## Resumo

| Verificação | Resultado | Itens checados | Falhas |
| :--- | :--- | :--- | :--- |
| **V1** Constituição (gate §12) vs. plan/data-model/contracts/tasks | ✅ PASS | §12.1, §12.3, §12.4, §12.6 | 0 |
| **V2** Rastreabilidade spec → tasks (RN, épicos, contratos, entidades) | ✅ PASS | 20 RN, 15 épicos, 3 contratos, catálogo de 59 tabelas | 0 |
| **V3** Rastreabilidade legado → matrizes (varredura de palavras-chave) | ⏸️ ADIADA → gate incremental | — | — (roda em `/analyze --epico E?` por épico; consolida em `Z-T02`) |
| **V4** Matrizes → tasks (VAL/OP/CALC em ≥1 tarefa) | ⏸️ ADIADA → gate incremental | estrutura pronta (`Cobre:` em toda tarefa `service`/`operation`/`parity`/`frontend`) | — (idem V3) |
| **V5** Cobertura de propriedades (toda coluna com destino; NULL→anulável) | 🟡 PARCIAL → gate incremental | E-Caixa/E-Domínio/E-Documento núcleo detalhados; demais por épico | 0 (nos detalhados) |
| **V6** Cross-module e estrangulamento | ✅ PASS | 11 refs `int` lógico classificadas; estrangulamento reconciliado (CLR-10) | 0 |
| **V7** Coerência spec ↔ plan ↔ tasks; clarify sem pendência bloqueante | ✅ PASS | 15 épicos, escopo frontend, 13 CLR | 0 |

---

## V1 — Constituição (Gate §12)

| Item do gate | Resultado | Evidência |
| :--- | :--- | :--- |
| §12.1 `.csproj` novo = `net10.0` | ✅ | `plan.md §1.1` (SharedKernel, GestaoFinanceira, Tests todos `net10.0`); pacotes 10.x |
| §12.1 projeto = `Versatus.<Modulo>` | ✅ | `Versatus.GestaoFinanceira`, `Versatus.SharedKernel` |
| §12.3 `Domain/` sem DataAnnotations; Fluent API 1 arq./entidade | ✅ | `plan.md §1.2` (`Infrastructure/Mappings/`), `data-model.md` (nota reforçada: Fluent no Mapping, não na entidade), tarefas `E?-T03` |
| §12.3 Controllers sem `DbContext`/`IRepository`; sem DTO aninhado | ✅ | `contracts/README.md` (DTOs `sealed record` em `Domain/DTOs/`), tarefas `contract` |
| §12.3 sem classe `*Lista`; coleções Artigo V | ✅ | `spec.md §2.15` (41 `*Lista` eliminadas), tarefas `domain` (`List<T>` privada + `IReadOnlyList<T>`) |
| §12.2 PK `.ValueGeneratedNever()`; sem `IDENTITY` | ✅ | `data-model.md §1` + RN-05-006 + tarefas `E?-T03` |
| §12.2 comentário de origem em cada classe | ✅ | tarefas `domain` (padrão `acesso.global` `// Origem:` / `// Tabela:`) |
| §12.4 1 transação/Handler; `SaveChanges` só no Handler | ✅ | `plan.md §5` (11 operações), tarefas `operation` |
| §12.4 `GET` na ReadConnection; paginação materializada | ✅ | `contracts/README.md`, `S-T03`, tarefas `service`/`E13` |
| §12.4 cross-módulo = `int` lógico | ✅ | `dependency-graph.md §4`, `data-model.md §1` |
| §12.4 estrangulamento reconciliado | ✅ | `plan.md §4` (CLR-10 — nada a reconciliar) |
| §12.5 RTV 1 `[Fact]`/VAL; ROT 1 integração + 1 golden/OP; parity | ✅ (estrutura) | `plan.md §6`, tarefas `E?-T01` geram matrizes, tarefas `service`/`operation`/`parity` consomem |
| §12.6 branch `feat/`/`docs/`; nada em `develop` | ✅ | `tasks.md` (toda tarefa tem branch `feat/mod-05-*`); trabalho SDD em `docs/mod-05-sdd` |
| §12.6 skill nova → fonte + stub + manual | ✅ | Bloco 1 (8 skills + stubs + `MANUAL-SKILLS.md`) |
| §12.6 Log de Progresso previsto | ✅ | `Z-T03` (`/handoff`) |

**Nenhum FAIL.**

---

## V2 — Rastreabilidade descendente (spec → tasks)

- **RN-05-001..020:** todas citadas em ≥1 tarefa (`tasks.md` §"Resumo de cobertura").
  Spot-check: RN-05-001 (E1-T02, E4-T05, E6-T01); RN-05-011 (E9-T01/02/04);
  RN-05-016 (E14-T01/03/04); RN-05-020 (E1-T04, E4-T01/06, E6-T01/05).
- **Épicos `plan.md §3`:** E0(T01-04), E1(T01-04), E3(T01-08), E2(T01-07), E4(T01-10),
  E5(T01-08), E6(T01-08), E7(T01-05), E8(T01-04), E9(T01-06), E10(T01-05), E11(T01-05),
  E12(T01-05), E13(T01-03), E14(T01-07) — todos com tarefas.
- **Contratos:** `caixa-banco.md`→E3-T06; `documento.md`→E4-T08; `liquidacao.md`→E6-T07;
  demais criados na tarefa `contract` do épico.
- **Entidades (`data-model.md` catálogo):** toda tabela do catálogo tem tarefa `domain` +
  `dbcontext` no épico correspondente.

---

## V6 — Cross-module e estrangulamento

- 11 referências `int` lógico classificadas (`dependency-graph.md §4`): `IdEntidade`,
  `IdFilial`, `IdOperacao`, `IdTipoDocumento`, `IdCondicaoPagamento`, `IdIndiceEconomico`,
  `IdUsuario*`, `IdPortador`/`IdFormaCobranca`, `IdCentroCusto`/`IdClasse` (`IdFinClasse`),
  `IdBanco`, `IdOrigem`+`ProcessoOrigem`. Nenhuma navegação EF cross-projeto.
- **`FinClasse`/`FinRateio` são de `acesso.global` (MOD-02)** — confirmado (`Classe.cs`
  lá); no MOD-05 são `int` lógico.
- **Pré-requisito CLR-01:** `RateioMovtoItem`/`RateioMovto`/`ManutencaoRateio` migram no
  MOD-02 antes de `E5-T02` — marcado na abertura do épico E5 e em `plan.md §7 R-6`.
- **Estrangulamento (CLR-10):** `Servidor.Strangler/Gestao.Financeira/DTOs/` só expõe
  `EntidadeDto`/`ClienteDto`/`FornecedorDto`/`ParametroDto` (AcessoGlobal). Nenhum
  contrato financeiro. `plan.md §4` fecha o item.

---

## V7 — Coerência interna

- `spec.md §6` (14 épicos numerados) ↔ `plan.md §3` (15 passos de execução, ordem
  corrigida E3 antes de E2) ↔ `tasks.md` (fases S + E0..E14 + Z) — consistentes; a
  inversão E2/E3 está documentada em ambos (spec §6 nota + plan §3).
- Escopo de frontend (CLR-07): `spec.md §1.5` = E3, E4, E6, E9 núcleo ↔ `tasks.md` tem
  tarefa `frontend` só em E3-T07, E4-T09, E6-T08, E9-T06. ✅
- `clarify.md`: 13 CLR resolvidas, **0 pendência bloqueante** (declarado em `clarify.md`
  "Pendências remanescentes").

---

## Achados

| # | Severidade | Verificação | Descrição | Ação exigida |
| :--- | :--- | :--- | :--- | :--- |
| A-01 | **MÉDIO** | V5 | Banco de dev não contém `FINPROJECAOFLUXOCAIXA(+LACTO)` nem `FINLOGDOMINIOPERIODO` — `data-model.md` desses fica incompleto. | Tarefas `E2-T01` e `E11-T01` reconfirmam e, se ausente, solicitam dump de schema de produção **antes** do `domain` do épico. Já registrado (`plan.md §7 R-1`, `research.md §8`). |
| A-02 | **MÉDIO** | V5 | Maioria das tabelas `Fin*` vazia → golden values de cálculo sem amostra real. | Golden via execução do legado / conferência manual (`research.md §5`); DÚVIDA-R3 (base de homologação) aberta ao usuário — não bloqueia. |
| A-03 | **BAIXO** | V5 | `FINDRETITULOOPERACAO` referencia `IDFINDRETITULOCALCULO` sem tabela `FINDRETITULOCALCULO` no banco. | Classificar (enum vs. FK) na tarefa `E11-T01`. |
| A-04 | **BAIXO** | V2 | `tasks.md` usa `Cobre:` com `VAL-xx#E?`/`OP-xx#E?` que ainda não existem (criados nas tarefas `analysis`). | Esperado no SDD; `sdd-analyze` V3/V4 rodam por épico no **gate incremental** (`/analyze --epico E?`) e consolidam em `Z-T02`. |

Nenhum achado **CRÍTICO** ou **ALTO**.

---

## Rodadas incrementais (`/analyze MOD-05 --epico E?`)

Preenchida durante o `/implement`, uma linha por épico, à medida que a tarefa `analysis`
de cada épico gera suas matrizes.

| Épico | Data | Órfãs V3 | V4 OK? | V5 OK? | Veredito | Achados |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| E1 | 2026-09-08 | 0 (após correção) | ✅ | ✅ (bases abstratas — coluna por E3/E4) | ✅ **APROVADO** | 3 MÉDIO de completude de matriz, **corrigidos na rodada** (ver E1 abaixo) |
| E3 | 2026-09-09 | 0 (após correção) | ✅ (E3-T09 `parity` adicionada) | ✅ (colunas confirmadas vs. `fin_columns.txt`) | ✅ **APROVADO** | 2 MÉDIO (VAL órfã) + 1 MÉDIO (V4: faltava tarefa `parity`), **corrigidos na rodada** (ver E3 abaixo) |
| E3 | — | — | — | — | ⏳ pendente | — |
| E2 | — | — | — | — | ⏳ pendente | — |
| E4 | — | — | — | — | ⏳ pendente | — |
| E5 | — | — | — | — | ⏳ pendente | — |
| E6 | — | — | — | — | ⏳ pendente | — |
| E7 | — | — | — | — | ⏳ pendente | — |
| E8 | — | — | — | — | ⏳ pendente | — |
| E9 | — | — | — | — | ⏳ pendente | — |
| E10 | — | — | — | — | ⏳ pendente | — |
| E11 | — | — | — | — | ⏳ pendente | — |
| E12 | — | — | — | — | ⏳ pendente | — |
| E13 | — | — | — | — | ⏳ pendente | — |
| E14 | — | — | — | — | ⏳ pendente | — |

---

## Rodada incremental E1 — `/analyze MOD-05 --epico E1` (2026-09-08)

**Escopo:** `matriz-rtv.md#E1` (31 `VAL-E1`), `matriz-rot.md#E1` (13 `OP-E1` + 10 `CALC-E1`),
`analysis/E1-bases.md`. Classes: `DocumentoFinanceiroBase`, `OperacaoDocumentoBase`,
`ItemFinanceiroBase`, `ParcelaGeral`, `ParcelaBase`, `FormaMovInfo`, `FechamentoCaixaBase`
+ herança até `servidor.framework/ObjetoNegocio.cs`.

| Verif. | Resultado | Nota |
| :--- | :--- | :--- |
| **V1** Constituição (escopo E1) | ✅ PASS | `E1-bases.md` fixa: POCO só de dados (Art. III), Fluent API, `decimal` p/ dinheiro, `bool` simples, auditoria anulável, `IReadOnlyList<T>`, sem herança de framework (spec §3.3), comportamento em serviço (CLR-04), 1 transação/handler (Art. VII), cross-módulo por `int` (Art. VIII). Nenhum `.csproj`/código ainda — gate real de código nas tarefas E1-T02/T03. |
| **V3** legado → matrizes | ✅ PASS (após correção) | Varredura de `Validar*`/`throw`/`Executar*`/`Persistir*`/`Calcular*`/`Aplicar*`/`Atualizar*`/`Recalcular*`/`OnBefore*`/`OnAfter*` nos 7 arquivos + pai: **todas as ocorrências relevantes têm linha**. 3 lacunas de completude encontradas e corrigidas nesta rodada (ver abaixo). `throw new NotImplementedException()` em `OperacaoDocumentoBase.GetIdOrigemRateio` = stub legado, não é regra (N/A). |
| **V4** matrizes → tasks | ✅ PASS | `E1-T04` (`tipo: parity`) cobre "todas as `VAL-xx#E1`, `CALC-xx#E1`"; `E1-T03` cobre "`OP-xx` de base"; `E1-T02` cobre as `VAL` de forma/atributo. Toda `CALC-E1` está em tarefa `parity` (E1-T04). |
| **V5** cobertura de propriedades | ✅ PASS (no escopo E1) | Bases são **abstratas, sem tabela própria** — o mapa coluna↔propriedade das entidades concretas é das tarefas `E3-T01`/`E4-T01`/`E4-T02` (`data-model.md §223`). `E1-bases.md §2` lista todos os campos compartilhados por classe; nenhum descarte silencioso — colunas de boleto/remessa marcadas `[E14]` (Regra 4, justificado). |

### Achados (3 · todos MÉDIO · corrigidos nesta rodada)

| # | Sev. | Verif. | Descrição | Ação (aplicada) |
| :--- | :--- | :--- | :--- | :--- |
| E1-A01 | MÉDIO | V3 | Ganchos `virtual` vazios de `DocumentoFinanceiroBase` (`RecalcularTributoRateio`, `SetParcelasCondicaoPagto`, `CarregarComissionadoPadrao`, `CarregaItemFinanceiroOperacao`) sem linha `OP`. | Adicionada **OP-E1-12** (ganchos de extensão → E4). |
| E1-A02 | MÉDIO | V3 | `OperacaoDocumentoBase.AplicarOperacao()` / `CarregarDefault` / `CarregarPortadorDefault` / `CarregarOperacoesDefault` sem linha `OP`. | Adicionada **OP-E1-13** (carga de defaults da operação → E6/E8). |
| E1-A03 | MÉDIO | V3 | `ItemFinanceiroBase.CalcularItem` / `AplicarItemFinanceiro` / `GetValorItemFinanceiroBoleto` (entradas públicas que compõem as fórmulas) sem linha `CALC`. | Adicionada **CALC-E1-10**. |

Nenhum achado **CRÍTICO** ou **ALTO**.

### Dependências cross-épico registradas (não bloqueiam E1)

- **VAL-E1-27** (`ValidarCaixaBanco`) precisa de `CaixaBanco`/`ContaBancaria` — **E3**; teste de E1-T04 mocka a porta.
- **OP-E1-05** (`AtualizarRateio`) precisa do motor de rateio do **MOD-02** (CLR-01) → integração no **E5**.
- `Situacao` de `FechamentoCaixaBase` + `PersistirPeriodoFormaPagto` → enum/máquina de estados no **E2**.
- **VAL-E1-28** (setter linha 609 de `ParcelaBase`) — detalhe do texto a extrair no `E1-T02`.

### Veredito E1

✅ **APROVADO** — `domain`/`service`/`operation` do épico E1 (E1-T02, E1-T03, E1-T04)
liberados para entrar em `develop`. As correções (OP-E1-12/13, CALC-E1-10) já estão em
`matriz-rot.md#E1` neste mesmo commit.

---

## Rodada incremental E3 — `/analyze MOD-05 --epico E3` (2026-09-09)

**Escopo:** `matriz-rtv.md#E3` (21 `VAL-E3`, 6 `[E14]`), `matriz-rot.md#E3` (8 `OP-E3` + 7 `CALC-E3`),
`analysis/E3-caixa-banco.md`. Classes: `CaixaBanco`, `ContaBancaria`, `CaixaBancoUsuario`,
`SaldoCaixaBanco`, `SaldoRateio`, `Cobrador`, `IndiceConversor`.

| Verif. | Resultado | Nota |
| :--- | :--- | :--- |
| **V1** Constituição (escopo E3) | ✅ PASS | `E3-caixa-banco.md §2` fixa PKs compostas reais (`.ValueGeneratedNever()`), `numeric(23,8)`→`decimal` `HasPrecision(23,8)`, `smallint`→`bool`, enums E0 com `HasConversion<int>()`, cross-módulo por `int`, 1 transação/serviço. Nenhum `.csproj`/código ainda. |
| **V3** legado → matrizes | ✅ PASS (após correção) | Varredura de `Validar*`/`throw`/`Executar*`/`Persistir*`/`Calcular*` nos 7 arquivos. 2 lacunas encontradas e corrigidas: guardas do `IndiceConversor` (`ValorNulo`/`DataSemIndiceEconomico`) → **VAL-E3-20**; agência obrigatória nos setters `GeraBoleto`/`GeraRemessa`/`ProcessaRetorno` → **VAL-E3-21** `[E14]`. `IndiceConversor.GetIdOrigemRateio` inexistente (era stub em `OperacaoDocumentoBase`). |
| **V4** matrizes → tasks | ✅ PASS (após correção) | `E3-T05` cobre "todas as `VAL-xx#E3`". **Faltava tarefa `parity` para as `CALC-E3`** (V4: "toda OP de cálculo tem tarefa `parity`") → adicionada **`E3-T09 · Paridade de saldo e conversão por índice · tipo: parity`** em `tasks.md` (IDs estáveis — não renumera). VAL `[E14]` → `E14-T0x`. |
| **V5** cobertura de propriedades | ✅ PASS | Todas as colunas das 7 tabelas mapeadas em `E3-caixa-banco.md §2` (contra `legacy-schema/fin_columns.txt`). `NULL`→anulável respeitado. ~40 colunas `[E14]` de `FINCONTABANCARIA` **listadas** e delegadas ao E14 (Regra 4 — escrito e justificado). `FININDICECONVERSOR` não existe → `IndiceConversor` é serviço (confirmado). |

### Achados (3 · todos MÉDIO · corrigidos nesta rodada)

| # | Sev. | Verif. | Descrição | Ação (aplicada) |
| :--- | :--- | :--- | :--- | :--- |
| E3-A01 | MÉDIO | V3 | Guardas de `IndiceConversor.ConverterIndice`/`RetornarIndice` (`ValorNulo`, `DataSemIndiceEconomico`) sem linha `VAL`. | Adicionada **VAL-E3-20**. |
| E3-A02 | MÉDIO | V3 | Setters `GeraBoleto`/`GeraRemessa`/`ProcessaRetorno` de `ContaBancaria` exigem agência — sem linha `VAL`. | Adicionada **VAL-E3-21** `[E14]`. |
| E3-A03 | MÉDIO | V4 | Épico E3 sem tarefa `parity` para `CALC-E3-01..07` (saldos + conversão por índice). | Adicionada **`E3-T09`** (`tipo: parity`) em `tasks.md`. |

Nenhum achado **CRÍTICO** ou **ALTO**.

### Dependências cross-épico registradas (não bloqueiam E3)

- **VAL-E1-27** (`ValidarCaixaBanco` da parcela — E1) **fecha aqui** — o `[Fact]` vai para `E3-T05`.
- **VAL-E3-08 / -10** dependem da máquina de estados do período (**E2**) — teste de `E3-T05` mocka a porta.
- **`FINSALDORATEIO`** tem colunas de PK anuláveis (`IDFINCLASSE`/`IDGLOCENTROCUSTO`/`IDGLOPROJETOS`) — `DÚVIDA-E3-1`, confirmar no `E3-T03`.
- ~40 colunas `[E14]` de `FINCONTABANCARIA` + `VAL-E3-13..16, 18, 19, 21` + sequenciais de arquivo (`OP-E3-05/06`) → épico **E14**.

### Veredito E3

✅ **APROVADO** — `domain`/`dbcontext`/`service`/`parity` do épico E3 (E3-T02..E3-T09)
liberados para entrar em `develop`. As correções (VAL-E3-20/21, `E3-T09`) estão neste commit.

---

## Veredito

🟡 **APROVADO PARA PLANEJAMENTO / `/implement`** — a camada SDD do MOD-05 (constituição,
spec, clarificação, plano, modelo de dados, pesquisa, contratos, tarefas) está completa,
consistente e conforme a constituição v1.0. Nenhum bloqueio.

⛔ **Gate incremental por épico é obrigatório** — nenhum código de um épico entra em
`develop` sem `/analyze MOD-05 --epico E?` ✅ (V3/V4/V5 no escopo `#E?`).

⛔ **O gate duro completo (modo completo, V1–V7 + cross-épico) é obrigatório em `Z-T02`** —
antes de mesclar o fecho do módulo, `sdd-analyze` deve dar veredito ✅ verde
(Artigo IX; `sdd-analyze` §5–§6).

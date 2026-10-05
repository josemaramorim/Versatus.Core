# Análise E2 — Domínio e Período

> **Tarefa:** E2-T01 (`tipo: analysis`) · **Data:** 2026-10-05 · **Depende de:** E3-T08
> Alimenta `matriz-rtv.md#E2` (52 `VAL`, 1 `[E9]`, 2 `[UI]`) · `matriz-rot.md#E2` (18 `OP` + 8 `CALC`
> + máquina de estados do período).
> Colunas confirmadas contra `legacy-schema/fin_columns.txt` + `fin_meta.txt` e, para índices,
> FKs e views, contra o banco de dev (`localhost\SQLEXPRESS2008`, consulta só-leitura em 2026-10-05).

---

## 0. Decisões do usuário (2026-10-05)

| # | Ponto | Decisão |
| :--- | :--- | :--- |
| D1 | Regras de **hierarquia** (quem abre/fecha/supre o caixa de quem, via `FINDOMINIORESPONSAVEL`) existem **só nas telas** do legado (`FDominioFinanceiroAbertura`, `FDominioFinanceiroFechamento`, `FSuprimentoBase`); o servidor aceita qualquer usuário. | **Também no backend**: viram `VAL-E2-43..48` nos handlers (`Result.Fail`). O E2 não tem React nesta rodada e expõe API. |
| D2 | Não existe operação `FecharTesouraria` no legado (§4.3). | **Ajustar `E2-T05`**: tesouraria é um efeito do `FecharPeriodoHandler`; os handlers listados passam a ser os reais (§4). |
| D3 | `FINLOGDOMINIOPERIODO` ausente no banco de dev **e** `DominioPeriodoLog` sem nenhum uso no legado. | **Não migrar** (código morto). O dump de produção **deixa de ser pré-requisito do E2** (continua valendo para `FINPROJECAOFLUXOCAIXA*` no E11). R-1 fechado para o E2. |
| D4 | Abrir/fechar período em dinheiro gera `MovimentoFinanceiro` (E5) e atualiza talão/cheque recebido (E9), que ainda não existem. | **Portas + implementação real no E5/E9**: o E2 define as portas e testa com fake (mesmo padrão dos adapters temporários do E3). |

---

## 1. Entidades do épico e destino

| Legado | Linhas | Tabela | Papel | Destino |
| :--- | :--- | :--- | :--- | :--- |
| `Dominio` | 1717 | `FINDOMINIO` | Agregado-raiz: domínio financeiro (caixa do usuário) + `Usuarios[]` + `Responsaveis[]`; dono das operações de período (abrir, fechar, suprir, padrão) | `Domain/Dominio/Dominio.cs` (POCO) + `DominioService` (CRUD) + handlers de período (`Application/Handlers/Periodo/`) |
| `DominioPeriodo` | 1017 | `FINDOMINIOPERIODO` | Período (abertura → fechamento) de um domínio; carrega lançamentos e fechamentos na persistência | `Domain/Dominio/DominioPeriodo.cs` + handlers |
| `DominioPeriodoLacto` | 823 | `FINDOMINIOPERIODOLANCTO` | Lançamento entre períodos (abertura, fechamento, suprimento) — gera movimento financeiro e saldo | `Domain/Dominio/DominioPeriodoLacto.cs` + `DominioPeriodoLactoService` |
| `DominioPeriodoFechamento` | 331 | `FINDOMINIOPERIODOFECHAMENTO` | Linha de fechamento por forma de pagamento (calculado × informado) | `Domain/Dominio/DominioPeriodoFechamento.cs` (filho de `DominioPeriodo`) |
| `DominioPeriodoFechamentoLista` | 58 | — | Lista de fechamentos (`AddFechamento`) | `List<>` privada do agregado `DominioPeriodo` |
| `DominioPeriodoFechamentoDetalhe` | 112 | **nenhuma** (`ObjectBase`) | Contagem de dinheiro por moeda / cheques recebidos na tela de fechamento | DTO + `CALC-E2-04` (sem POCO persistida) |
| `DominioPeriodoFormaPagto` | 421 | `FINDOMINIOPERIODOFORMAPAGTO` | Movimento por forma de pagamento no período (gerado por outros processos via `IDadosPeriodoFormaPagto`) | `Domain/Dominio/DominioPeriodoFormaPagto.cs` + `DominioPeriodoFormaPagtoService` |
| `DominioPeriodoLog` | 143 | `FINLOGDOMINIOPERIODO` (**ausente no banco**) | **Código morto** — nenhuma referência no legado fora do próprio arquivo e da interface | **Não migra** (D3) |
| `DominioResponsavel` (+`Lista`) | 356 + 130 | `FINDOMINIORESPONSAVEL` | Hierarquia: domínio-pai responsável por um domínio-filho | `Domain/Dominio/DominioResponsavel.cs` (agregado de `Dominio`) |
| `DominioUsuario` (+`Lista`) | 508 + 109 | `FINDOMINIOUSUARIO` | Usuários do domínio (1 principal) | `Domain/Dominio/DominioUsuario.cs` (agregado de `Dominio`) |
| `DominioPeriodoLista` | 56 | — | Lista sem uso real (`AddPeriodo` com corpo comentado) | Não migra |
| `DominioUtil` | 51 | — | `ListarFormasPagto` (formas listáveis no fechamento) + `UPDATE FINCHEQUERECEBIDO.IDFINDOMINIOPERIODO` | Função pura no `FechamentoPeriodoService` + porta de cheque (D4) |
| `PeriodosAbertos` | 551 | — (cache em memória) | Resolve o período aberto do usuário logado (`PeriodoStatus`) | `PeriodosAbertosService` (consulta; substitui o adapter temporário `DominioFinanceiroConsulta` do E3-T05) |
| `ResumoDominioPeriodoFormaPagtoConsulta` | 286 | view `VWRESUMODOMINIOPERIODOFORMAPAGTO` | Resumo por forma para o fechamento (herda `FechamentoCaixaBase` — E1) | Consulta no `ReadContext` + `CALC-E2-02/03` |
| `DominioUsuarioSuprimentoSangriaConsulta` | 280 | view `VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA` | Hierarquia achatada (usuário × domínio × pai/filho) usada pelas telas | Consulta no `ReadContext` — base de `VAL-E2-43..46` |
| `SaldoDominioLactoHelper` / `SaldoDominioFormaHelper` (acesso.global) | 309 + 484 | atualizam `FINDOMINIO.SALDO*` | Saldo do domínio por lançamento / por forma + recálculo | `SaldoDominioService` (E2) — `CALC-E2-05..07` |

Telas auditadas (camada 1 da RTV): `FDominioFinanceiro` (cadastro), `FDominioFinanceiroAbertura`,
`FDominioFinanceiroFechamento`, `FDominioFinanceiroFechamentoDetalhe`, `FCentralDominio`,
`FDominioPadraoAbertura`, `FDominioPadraoFechamento` (acesso.global), `FSuprimentoBase`,
`FSuprimentoDinheiro`, `FSuprimentoCheque`, `FBase` (pré-verificações `UsaDominioFinanceiroPerfil` /
`DominioPeriodo`). Herança: `Dominio`/`DominioPeriodo` → `ObjectMaster`; `ValidarVinculoCaixaBancoUsuario`
vem de `servidor.framework/ObjetoNegocio.cs:1856` e delega a `ValidarControleCaixaBanco` (= **VAL-E3-09**).

Enums já no SharedKernel (E0): `StatusDominioFinanceiro` (estado transitório da operação),
`PeriodoStatus`, `LanctoDominioPeriodoTipo` (`Abertura=263, Fechamento=264, Suprimento=290, Sangria=291`),
`FormaPagtoTipo`, `ProcessoOrigem` (`DominioPeriodoLancto=292`).

---

## 2. Mapa coluna → propriedade

Regras gerais do `data-model.md` §2 (PK composta `.ValueGeneratedNever()`, `smallint`→`bool`,
`numeric(23,8)`→`decimal` `HasPrecision(23,8)`, auditoria anulável). Nenhuma tabela do E2 tem
`CHECK`, trigger ou default no banco — a camada 4 da RTV são só PK, FK e o índice único da §2.4.

### 2.1 `FINDOMINIO` (23) → `Dominio`

PK **`(IDFINDOMINIO, IDGLOFILIAL)`**. Sequencial por filial (`GLOSEQUENCIAL.NOMEOBJETO = Dominio`).

| Coluna | Tipo (NULL) | Propriedade C# | Nota |
| :--- | :--- | :--- | :--- |
| IDFINDOMINIO / IDGLOFILIAL | int NO | `IdDominio` / `IdFilial` | PK |
| IDFINCAIXABANCO | int NO | `IdCaixaBanco` (int) | FK `FINCAIXABANCO (IDFINCAIXABANCO, IDGLOFILIAL)` — caixa tipo Caixa/Normal (VAL-E2-14) |
| IDFINDOMINIOPERIODO | int YES | `IdDominioPeriodo` (int?) | FK `FINDOMINIOPERIODO` — **ponteiro para o período corrente** (aberto ou o último fechado); só muda na abertura (OP-E2-03/06) |
| DESCRICAO | varchar(100) NO | `Descricao` (string) | |
| ATIVO / TESOURARIA / ABRIRPERIODO / FECHARPERIODO / ABRIROUTROSPERIODOS / FECHAROUTROSPERIODOS / MOVIMENTOBANCO / CONSULTATODOSPERIODOS | smallint NO | `bool` (8) | construtor legado: `Ativo = true`, demais `false` |
| SALDO / SALDODINHEIRO / SALDOCHEQUERECEBIDO / SALDOCARTAO | numeric(23,8) YES | `decimal?` (4) | **somente leitura no domínio** — gravados só pelo `SaldoDominioService` (CALC-E2-05..07) |
| IDGLOUSUARIOINCLUSAO … HORAALTERACAO | int/datetime YES | auditoria (6) | |

Agregados: `IReadOnlyList<DominioUsuario> Usuarios`, `IReadOnlyList<DominioResponsavel> Responsaveis`.
Não persistidos (estado da operação → parâmetros do handler, não propriedade): `Status`,
`SetarSaldoCaixaBanco`, `ValorAbertura`, `SaldoCaixaBanco`, `FormasFechamento`.

### 2.2 `FINDOMINIOPERIODO` (16) → `DominioPeriodo`

PK **`(IDFINDOMINIOPERIODO, IDGLOFILIAL)`**. Sequencial por filial.

| Coluna | Tipo (NULL) | Propriedade | Nota |
| :--- | :--- | :--- | :--- |
| IDFINDOMINIOPERIODO / IDGLOFILIAL | int NO | PK | |
| IDFINDOMINIO | int NO | `IdDominio` | FK `FINDOMINIO` |
| IDGLOUSUARIOFECHAMENTO | int YES | `IdUsuarioFechamento` (int?) | |
| DATAABERTURA | datetime NO | `DataAbertura` (DateTime) | data do sistema |
| DATAHORAABERTURA | datetime NO | `HoraAbertura` (DateTime) | o legado chama de "Hora" e grava a hora do sistema (comentário: nome mantido por relatórios) |
| DATAFECHAMENTO / DATAHORAFECHAMENTO | datetime YES | `DataFechamento` / `HoraFechamento` (DateTime?) | **`DataFechamento != null` ⇔ período fechado** |
| DATAFECHAMENTOTESOURARIA / DATAHORAFECHAMENTOTESOURARIA | datetime YES | `DataFechamentoTesouraria` / `HoraFechamentoTesouraria` (DateTime?) | preenchidas no fechamento quando `ControleCaixaTesouraria` (§4.3) |
| IDGLOUSUARIOINCLUSAO … HORAALTERACAO | YES | auditoria (6) | |

Agregado: `IReadOnlyList<DominioPeriodoFechamento> Fechamentos`. Os lançamentos **não** são agregado
(têm origem e destino em períodos diferentes) — o handler os grava na mesma transação.

### 2.3 `FINDOMINIOPERIODOLANCTO` (17) → `DominioPeriodoLacto`

PK **`(IDFINDOMINIOPERIODOLANCTO, IDGLOFILIAL)`**. Sequencial por filial (`DominioPeriodoLacto`).

| Coluna | Tipo (NULL) | Propriedade | Nota |
| :--- | :--- | :--- | :--- |
| IDFINPERIODOORIGEM / IDFINPERIODODESTINO | int YES | `IdPeriodoOrigem` / `IdPeriodoDestino` (int?) | FKs para `FINDOMINIOPERIODO`; `null` = padrão (abertura: sem origem; fechamento padrão: sem destino) |
| IDTIPOLANCAMENTO | int NO | `TipoLancto` (`LanctoDominioPeriodoTipo`) | FK `GLOTIPOENUMERADO` |
| IDTIPOFORMA | int NO | `TipoForma` (`FormaPagtoTipo`) | FK `GLOTIPOENUMERADO` |
| IDFINCHEQUERECEBIDO | int YES | `IdChequeRecebido` (int?) | FK `FINCHEQUERECEBIDO` (E9) |
| IDFINCAIXABANCO + CHEQUE | int YES + int YES | `IdCaixaBanco` (int?) / `Cheque` (int?) | **FK composta para `FINTALAOCHEQUE (IDFINCAIXABANCO, IDGLOFILIAL, CHEQUE)`**, não para `FINCAIXABANCO`. Em dinheiro, `IdCaixaBanco` é o caixa e `Cheque` fica `null` (FK não verificada). |
| DATAHORALANCTO | datetime NO | `DataLancto` (DateTime) | construtor legado: `DateTime.Now` (com hora) |
| VALOR | numeric(23,8) YES | `Valor` (decimal?) | |
| auditoria (6) | YES | | |

Não persistidos: `ValorInformado`, `Status` (estado da operação).

### 2.4 `FINDOMINIOPERIODOFECHAMENTO` (12) → `DominioPeriodoFechamento`

PK **`(IDFINDOMINIOPERIODOFECHAMENTO, IDGLOFILIAL)`**; **índice UNIQUE `IDX_FINDOMINIOPERIODOFECHAMENTO01
(IDGLOFILIAL, IDFINDOMINIOPERIODO, IDTIPOFORMA)`** — uma linha por forma por período (`HasIndex(...).IsUnique()`).
Colunas: `IDFINDOMINIOPERIODO int NO`, `IDTIPOFORMA int NO` → `TipoForma` (`FormaPagtoTipo`),
**`CALCULADO numeric(23,8) NO`** → `ValorCalculado` (decimal), **`INFORMADO numeric(23,8) NO`** →
`ValorInformado` (decimal), auditoria (6).

### 2.5 `FINDOMINIOPERIODOFORMAPAGTO` (10) → `DominioPeriodoFormaPagto`

PK **`(IDFINDOMINIOPERIODOFORMAPAGTO, IDGLOFILIAL)`**. `IDFINDOMINIOPERIODO int NO`,
`IDGLOFORMAPAGAMENTO int NO` (FK MOD-02), `IDPROCESSOORIGEM int NO` (`ProcessoOrigem`),
`IDORIGEM int NO`, `CREDITO/DEBITO numeric(23,8) YES`, `DATA datetime NO`,
**`HORA varchar(8) NO` → `string`** (`DateTime.Now.ToLongTimeString()`). Sem auditoria.

### 2.6 `FINDOMINIOUSUARIO` (10) → `DominioUsuario`

PK **`(IDFINDOMINIO, IDGLOUSUARIO, IDGLOFILIAL)`** — **ordem física com a filial em 3º**.
`USUARIOPRINCIPAL smallint NO` → `bool`; auditoria (6). FK `GLOUSUARIO`.

### 2.7 `FINDOMINIORESPONSAVEL` (10) → `DominioResponsavel`

PK **`(IDFINDOMINIO, IDGLOFILIAL, IDFINDOMINIOPAI)`**. `DOMINIOPAIPRINCIPAL smallint NO` → `bool`;
auditoria (6). Duas FKs para `FINDOMINIO` (filho e pai).

### 2.8 Views (só leitura — `ReadContext`, `ToView`)

- **`VWLDOMINIOUSUARIOSUPRIMENTOSANGRIA`** — `UNION ALL` de 3 blocos: (a) cada domínio com seus usuários
  (`IDDOMINIOFILHO = NULL`, `CAIXA = 1`); (b) para cada par pai→filho, os usuários do **filho** vistos
  pelo pai (`IDDOMINIO = pai`, `IDDOMINIOFILHO = filho`, `CAIXA = 1`); (c) os usuários do **pai** vistos
  pelo filho (`IDDOMINIO = pai`, `IDDOMINIOFILHO = filho`, `CAIXA = 0`, retorno = pai). É a fonte de
  todas as regras de hierarquia (VAL-E2-43..46).
- **`VWRESUMODOMINIOPERIODOFORMAPAGTO`** — `SUM(CREDITO)`, `SUM(DEBITO)` por
  `(filial, período, descrição, IDTIPOFORMAPAGAMENTO)` sobre `VWDOMINIOPERIODOFORMAPAGTO`
  (= `FINDOMINIOPERIODOFORMAPAGTO` ⋈ `GLOFORMAPAGAMENTO` **∪** `VWDOMINIOPERIODOLANCTO`
  com suprimento como crédito e sangria como débito).

---

## 3. Comportamento (VAL / OP / CALC) — resumo por classe

| Classe / tela | Validações | Operações / fórmulas |
| :--- | :--- | :--- |
| `Dominio` (cadastro) | VAL-E2-01..15, -28 | OP-E2-01/02 |
| `Dominio` (período) | VAL-E2-16..27 | OP-E2-03 (abrir), -04 (fechar, + tesouraria), -06/07 (padrão), -08/09 (suprimento), -14, -18 |
| `DominioUsuario(+Lista)` | VAL-E2-29..34 | (agregado — OP-E2-01) |
| `DominioResponsavel(+Lista)` | VAL-E2-35..37 | (agregado — OP-E2-01) |
| `DominioPeriodo` | — | OP-E2-03/04/06/07 (persistência complementar), -15 · CALC-E2-01, -08 |
| `DominioPeriodoLacto` | VAL-E2-01, -38..41 | OP-E2-10/11 |
| `DominioPeriodoFormaPagto` | — | OP-E2-12 · CALC-E2-06 |
| `DominioPeriodoFechamento(+Detalhe/+Lista)` | — | OP-E2-04 · CALC-E2-04 |
| `PeriodosAbertos` | VAL-E2-42 | OP-E2-13 |
| Saldo do domínio (helpers acesso.global) | — | OP-E2-16 · CALC-E2-05..07 |
| `ResumoDominioPeriodoFormaPagtoConsulta` | — | OP-E2-15 · CALC-E2-02/03 |
| Telas (D1 → backend) | VAL-E2-43..50 | OP-E2-17 (central) |
| Telas (só tela) | VAL-E2-51 `[UI]`, -52 `[E9]` | — |

---

## 4. Operações de período — o que muda no `E2-T05` (D2)

### 4.1 Handlers reais (1 transação cada)

| Handler | Legado | OP |
| :--- | :--- | :--- |
| `AbrirPeriodoHandler` | `Dominio.PersistirAberturaDominioPeriodo` | OP-E2-03 |
| `FecharPeriodoHandler` | `Dominio.PersistirFechamentoDominioPeriodo` (inclui tesouraria) | OP-E2-04 / -05 |
| `AbrirDominioPadraoHandler` | `FDominioPadraoAbertura` → `AssimilarDominioPadrao` + `Persist` + `PersitirDominioPadrao` | OP-E2-06 |
| `FecharDominioPadraoHandler` | `FDominioPadraoFechamento` → `PersitirDominioPadrao` | OP-E2-07 |
| `SuprimentoDinheiroHandler` | `Dominio.EfetuarSuprimentoDinheiro` | OP-E2-08 |
| — (`SuprimentoChequeHandler` já previsto no **E9-T05**) | `Dominio.EfetuarSuprimentoCheque` | OP-E2-09 `[E9]` |

`CalcularTotalFormasPagto` não é handler: é a consulta de fechamento (OP-E2-15 + CALC-E2-01).

### 4.2 Portas (D4)

| Porta | Usada por | Implementação real |
| :--- | :--- | :--- |
| `IGeradorMovimentoPeriodo` | OP-E2-10 (dinheiro → 2 `MovimentoFinanceiro`: sangria no caixa de origem, suprimento no de destino) | **E5** |
| `IAtualizadorChequePeriodo` | OP-E2-06/07/10 (talão: `FINTALAOCHEQUE.IDFINDOMINIOPERIODO`; cheque recebido: `FINCHEQUERECEBIDO.IDFINDOMINIOPERIODO`; listas de cheques do fechamento) | **E9** |
| `IParametroFinanceiroEscrita` | OP-E2-06/07 (grava 4 parâmetros e `GLOPERFIL.USADOMINIOFINANCEIRO`) | adapter SQL para MOD-02 (Artigo VIII — escrita cross-módulo explícita) |
| `IOperacaoCaixaConsulta` | VAL-E2-38..41 (tipo, natureza e `LancaMovtoEntidade` da operação padrão) | adapter só-leitura MOD-02 |

### 4.3 Tesouraria

Não há `FecharTesouraria` no servidor nem nas telas. Em `DominioPeriodo.OnBeforeExecutarPersistir`
(linhas 318–330), no status `Fechamento` com parâmetro `ControleCaixaTesouraria` ligado, o fechamento
grava também `DATAFECHAMENTOTESOURARIA`/`DATAHORAFECHAMENTOTESOURARIA` com a data/hora do sistema.
`FechamentoPadrao` **não** preenche as colunas de tesouraria. O flag `FINDOMINIO.TESOURARIA` só é
usado em `EditarCaixaBancoDominioPeriodo` (OP-E2-14) e copiado para o parâmetro na abertura padrão (OP-E2-06).

---

## 5. Comportamentos do legado preservados (Regra 5 — não "consertar")

| # | Onde | Comportamento | Tratamento |
| :--- | :--- | :--- | :--- |
| Q1 | `DominioPeriodo.cs:987-988` | `Funcoes.Arredondar(total, 2)` é chamado e o **retorno descartado** — `CalcularTotalFormasPagto` devolve o total **sem arredondar**. | CALC-E2-01 transcreve sem arredondamento; golden obrigatório. |
| Q2 | `Dominio.cs:1470`, `1582` | Sem `AbrirPeriodo`/`AbrirOutrosPeriodos` (ou sem `Fechar*`), a operação **retorna sem erro** e a tela faz `Commit` sem gravar nada. | VAL-E2-20/26: o handler devolve **sucesso sem efeito** (paridade). Mas a tela já bloqueia antes (VAL-E2-50); registrado para o teste. |
| Q3 | `Dominio.cs:1409` | Mensagem 18 (`"O domínio ({0}) está fechado."`) usada **sem `String.Format`** — o texto sai com `{0}` literal. | VAL-E2-24 reproduz o texto literal. |
| Q4 | `DominioPeriodoLacto.cs:234-238`, `255-259` | Bloqueia quando `LancaMovtoEntidade` **está** marcada, mas a mensagem diz "deve estar com a opção … MARCADA". | VAL-E2-40: condição do código + mensagem literal. |
| Q5 | `FDominioFinanceiroFechamento.cs:1078-1098` | Confirmação de fechamento verifica só a **primeira** forma com valor zero/divergente e sai do laço. | `[UI]` — VAL-E2-51. |
| Q6 | `Dominio.cs:1615` + tela | Sem domínio-pai com usuário, `idUsuarioSuprimento = 0` e o servidor quebra com `NullReferenceException`. | VAL-E2-47 exige o usuário do suprimento quando há formas (regra da tela, D1) — o backend devolve `Result.Fail` em vez da exceção. |
| Q7 | `DominioPeriodo.cs:251`, `SuprimentoCheque.cs:266` | Para cheque de empresa, `SuprimentoCheque.IdCheque` contém o **IdCaixaBanco** do talão (campo sobrecarregado); o lançamento grava esse valor em `IDFINCAIXABANCO`. | Não é bug: OP-E2-09 documenta; DTO do E9 separa os campos. |
| Q8 | `ResumoPeriodoFormaPagtoConsulta.cs:227-249` | Para `ChequeEmpresa`, `ValorCalculado` = **quantidade** de folhas e `ValorConferencia` = `Quantidade` (não é dinheiro); por isso o total (CALC-E2-01) ignora `ChequeEmpresa`. | CALC-E2-02. |
| Q9 | `PeriodosAbertos.cs:275-276` | `IAmbiente a = ambiente; a.IdFilial = idFilial;` — **altera a filial do ambiente do chamador** dentro do laço (não é cópia). | OP-E2-13: o serviço recebe a filial por parâmetro; o efeito colateral não é reproduzido (era só cache em memória). |

---

## 6. Pendências / dependências cross-épico

- **Fecha aqui (E3):** o adapter temporário `DominioFinanceiroConsulta` (E3-T05, VAL-E3-08/10) é
  substituído pelo `PeriodosAbertosService` + repositórios do E2; o `PeriodoStatus` de
  `ValidarControleCaixaBancoAsync` (VAL-E3-09) passa a vir do E2 (E2-T04).
- **E1:** `PersistirPeriodoFormaPagto` (OP-E1-01) cria `DominioPeriodoFormaPagto` via `IDadosPeriodoFormaPagto` → OP-E2-12. `Diferenca` de `FechamentoCaixaBase` (E1) não tinha `CALC` → **CALC-E2-03**.
- **E5:** `IGeradorMovimentoPeriodo` (OP-E2-10). `MovimentoFinanceiro` e o recálculo de saldo usam `IdDominioPeriodo` e `EditarCaixaBancoDominioPeriodo` (OP-E2-14).
- **E9:** `IAtualizadorChequePeriodo`; `SuprimentoChequeHandler` (OP-E2-09) e a validação de conta de `FSuprimentoCheque` (VAL-E2-52 `[E9]`).
- **E10:** `DominioPeriodoFormaPagto.Assimilar(IAdtoAcerto)` (OP-E2-12).
- **MOD-02:** escrita de parâmetros (`ControleCaixaTesouraria`, `TrabalhaComDominio`, `ChequeEmpresaPorSuprimento`, `DominioPadrao`) e de `GLOPERFIL.USADOMINIOFINANCEIRO` (OP-E2-06/07); leitura de perfis/usuários/operações. **DÚVIDA herdada do E3:** `IParametroRepository.GetParametroValorAsync` não filtra por filial — `TrabalhaComDominio`, `DominioPadrao` e `ControleCaixaTesouraria` são por filial no legado; o E2 depende disso em quase todas as regras. Deve ser resolvida **antes do E2-T04**.
- **Fora de escopo:** impressão pós-fechamento (`ModeloPosFechamentoPeriodo`/`ImpressaoFechamentoPeriodo` — CLR-09, impressão no frontend); `DominioPeriodoLog` (D3); `DominioPeriodoLista` (sem uso).

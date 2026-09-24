# Análise E3 — Caixa e Banco

> **Tarefa:** E3-T01 (`tipo: analysis`) · **Data:** 2026-09-09 · **Depende de:** E1-T04
> **Roda antes do E2** (`plan.md §3` — `FINDOMINIO`→`FINCAIXABANCO` inverte a ordem).
> Alimenta `matriz-rtv.md#E3` (19 `VAL`, 5 `[E14]`) · `matriz-rot.md#E3` (8 `OP` + 7 `CALC`).
> Colunas confirmadas contra `legacy-schema/fin_columns.txt` (extract real do banco).

---

## 1. Entidades do épico e destino

| Legado | Linhas | Tabela | Papel | Destino |
| :--- | :--- | :--- | :--- | :--- |
| `CaixaBanco` | 869 | `FINCAIXABANCO` | Agregado (Caixa **ou** Banco) + `Usuarios[]` | `Domain/Bancos/CaixaBanco.cs` (POCO) + `CaixaBancoService` |
| `ContaBancaria` | 1704 | `FINCONTABANCARIA` (1:1 com `FINCAIXABANCO`) | Dados bancários da conta (Banco) | `Domain/Bancos/ContaBancaria.cs` — **só o núcleo**; ~40 colunas de integração → **E14** (CLR-03) |
| `CaixaBancoUsuario` | 350 | `FINCAIXABANCOUSUARIO` | Usuários vinculados ao caixa (agregado de `CaixaBanco`) | `Domain/Bancos/CaixaBancoUsuario.cs` |
| `SaldoCaixaBanco` | 322 | `FINSALDOCAIXABANCO` | Saldo diário (normal + conciliado) | `Domain/Bancos/SaldoCaixaBanco.cs` + `SaldoCaixaBancoService` (consulta) |
| `SaldoRateio` | 382 | `FINSALDORATEIO` | Saldo diário por dimensão de rateio (econômico + financeiro) | `Domain/Bancos/SaldoRateio.cs` |
| `Cobrador` | 367 | `FINCOBRADOR` | Cadastro de cobrador | `Domain/Bancos/Cobrador.cs` + `CobradorService` (CRUD) |
| `IndiceConversor` | 222 | **nenhuma** (`ObjectPersist` nunca persistido) | Serviço de conversão por índice econômico | `Application/Bases/ConversorIndiceService` — realiza o gancho `ConverterIndice` deixado em `CalculadoraItemFinanceiroBase` (E1-T03) |

---

## 2. Mapa coluna → propriedade

### 2.1 `FINCAIXABANCO` (17 colunas) → `CaixaBanco`

PK composta **`(IDFINCAIXABANCO, IDGLOFILIAL)`** (ordinal 1, 2) — `HasKey(new { IdCaixaBanco, IdFilial })`, `.ValueGeneratedNever()`.

| Coluna | Tipo (NULL) | Propriedade C# | Nota |
| :--- | :--- | :--- | :--- |
| IDFINCAIXABANCO | int NO | `IdCaixaBanco` (int) | PK; `GeradorSequencialService` |
| IDGLOFILIAL | int NO | `IdFilial` (int) | PK |
| DESCRICAO | varchar(100) NO | `Descricao` (string) | `= string.Empty` |
| IDTIPOCONTA | int NO | `TipoConta` (`ContaTipo`) | enum E0 — `HasConversion<int>()` |
| ATIVO | smallint NO | `Ativo` (bool) | `smallint`→`bool` (ConfigureConventions) |
| ENTRAFLUXOCAIXA | smallint NO | `EntraFluxoCaixa` (bool) | |
| ULTIMADATACONFERIDA | datetime YES | `UltimaDataConferida` (DateTime?) | |
| SALDO | numeric(23,8) YES | `Saldo` (decimal?) | `HasPrecision(23, 8)` |
| CONTACONTABIL | varchar(20) YES | `ContaContabil` (string?) | |
| IDCONPLANOCONTABIL | int YES | `IdPlanoContabil` (int?) | FK lógica módulo contábil |
| IDTIPOCONTACAIXA | int YES | `TipoContaCaixa` (`TipoContaCaixa?`) | enum E0; nulo/`0` quando conta = Banco |
| IDGLOUSUARIOINCLUSAO / DATAINCLUSAO / HORAINCLUSAO | int/datetime YES | `IdUsuarioInclusao` / `DataInclusao` / `HoraInclusao` (int?/DateTime?) | auditoria (Artigo III.4) |
| IDGLOUSUARIOALTERACAO / DATAALTERACAO / HORAALTERACAO | int/datetime YES | `IdUsuarioAlteracao` / `DataAlteracao` / `HoraAlteracao` | auditoria |

Agregado: `IReadOnlyList<CaixaBancoUsuario> Usuarios` (`List<>` privada — Artigo V).

### 2.2 `FINCAIXABANCOUSUARIO` (9) → `CaixaBancoUsuario`

PK **`(IDFINCAIXABANCO, IDGLOFILIAL, IDGLOUSUARIO)`**. Colunas: `IdCaixaBanco`, `IdFilial`,
`IdUsuario` + as 6 de auditoria (todas `int?`/`DateTime?`).

### 2.3 `FINCONTABANCARIA` (55) → `ContaBancaria`

PK **`(IDFINCAIXABANCO, IDGLOFILIAL)`** — 1:1 com `FINCAIXABANCO`
(`HasOne<CaixaBanco>().WithOne().HasForeignKey<ContaBancaria>(x => new { x.IdCaixaBanco, x.IdFilial })`).

**Núcleo E3** (12 colunas + auditoria):

| Coluna | Tipo (NULL) | Propriedade | Nota |
| :--- | :--- | :--- | :--- |
| IDGLOAGENCIA | int NO | `IdAgencia` (int) | FK lógica MOD-02 |
| TITULAR | varchar(50) YES | `Titular` (string?) | |
| NUMEROCONTA | varchar(15) NO | `NumeroConta` (string) | `= string.Empty` |
| DIGITOCONTA | varchar(2) YES | `DigitoConta` (string?) | |
| LIMITE / CREDITOPENDENTE / DEBITOPENDENTE / CHEQUEPENDENTE | numeric(23,8) YES | `Limite` / `CreditoPendente` / `DebitoPendente` / `ChequePendente` (decimal?) | `HasPrecision(23, 8)` |
| CONTATERCEIRO | smallint NO | `ContaTerceiro` (bool) | |
| PERMITEEMITIRCHEQUE | smallint NO | `PermiteEmitirCheque` (bool) | |
| IDFINCONTABANCARIAVINCULADA | int YES | `IdContaBancariaVinculada` (int?) | conta de investimento (VAL-E3-06/07) |
| IDTIPOCONTABANCARIA | int NO | `ContaBancariaTipo` (`TipoContaBancaria`) | enum E0 |
| IDGLOINSTITUICAOFINANCEIRA | int YES | `IdInstituicaoFinanceira` (int?) | FK lógica MOD-02 (SPED — VAL-E3-02) |
| IDGLOUSUARIOINCLUSAO … HORAALTERACAO | int/datetime YES | auditoria (6) | |

**`[E14]` — integração bancária (~40 colunas, CLR-03)** — ficam fora do `Domain/Bancos/ContaBancaria.cs`
do E3; o mapeamento entra no épico E14:
`TIPOCARTEIRA, ULTIMONOSSONUMERO numeric(17,2), POSTOCOBRANCA, ACEITE, ESPECIEDOCTO,
LOCALPAGAMENTO, IDTIPOARQUIVOREMESSARETORNO, IDFINITEMFINANCEIRO{MULTA,DESCONTO,JUROS,ABATIMENTO,ACRECIMOS},
DIASPARAPROTESTO, OUTRASINSTRUCOES1/2, REGISTROCOBRANCA, DIRETORIOARQUIVO{RETORNO,REMESSA},
DATAULTIMOENVIO, QUANTIDADEULTIMOENVIO, IDENTIFICACAOEMPRESA, CPFCNPJ varchar(14), NUMEROCONVENIO,
ULTIMONUMEROARQUIVO, GERABOLETO, GERAREMESSA, PROCESSARETORNO, CODIGOCARTEIRAREMESSA, ENVIARSPED,
BOLETOBENEFICIARIODIFERENTE, IDGLOENTIDADEBENEFICIARIOBOLETO, BOLETOSACADOAVALISTA,
IDGLOENTIDADESACADOAVALISTABOLETO, IDIMPDOCUMENTOIMPRESSAO`.
Regras dessas colunas: `VAL-E3-13..16, 18, 19` (marcadas `[E14]` na Matriz RTV).

### 2.4 `FINSALDOCAIXABANCO` (10) → `SaldoCaixaBanco`

PK **`(IDFINCAIXABANCO, IDGLOFILIAL, DATASALDO)`**.

| Coluna | Tipo (NULL) | Propriedade | Nota |
| :--- | :--- | :--- | :--- |
| IDFINCAIXABANCO / IDGLOFILIAL | int NO | `IdCaixaBanco` / `IdFilial` | PK |
| DATASALDO | datetime NO | `DataSaldo` (DateTime) | PK |
| SALDOANTERIOR / TOTALDEBITO / TOTALCREDITO | numeric(23,8) YES | `SaldoAnterior` / `TotalDebito` / `TotalCredito` (decimal?) | |
| CONFERIDO | smallint NO | `Conferido` (bool) | |
| SALDOANTERIORCONCILIADO / TOTALDEBITOCONCILIADO / TOTALCREDITOCONCILIADO | numeric(23,8) YES | `SaldoAnteriorConciliado` / `TotalDebitoConciliado` / `TotalCreditoConciliado` (decimal?) | |

`Saldo` e `SaldoConciliado` = **propriedades calculadas** (não colunas) — `CALC-E3-01/02`.

### 2.5 `FINSALDORATEIO` (11) → `SaldoRateio`

PK **`(IDGLOFILIAL, IDFINCLASSE, IDGLOCENTROCUSTO, IDGLOPROJETOS, DATASALDO)`** — as 3 dimensões
são **`int?`** no schema (parte da PK sendo anulável — preservar como está; `DÚVIDA-E3-1` para o
`E3-T03` confirmar o comportamento do EF com PK anulável, provável uso de valor sentinela `0`).

| Coluna | Tipo (NULL) | Propriedade |
| :--- | :--- | :--- |
| IDGLOFILIAL | int NO | `IdFilial` |
| IDFINCLASSE / IDGLOCENTROCUSTO / IDGLOPROJETOS | int YES | `IdClasse?` / `IdCentroCusto?` / `IdProjeto?` |
| DATASALDO | datetime NO | `DataSaldo` |
| SALDOANTERIORECONOMICO / TOTALCREDITOECONOMICO / TOTALDEBITOECONOMICO | numeric(23,8) YES | `SaldoAnteriorEconomico` / `TotalCreditoEconomico` / `TotalDebitoEconomico` (decimal?) |
| SALDOANTERIORFINANCEIRO / TOTALCREDITOFINANCEIRO / TOTALDEBITOFINANCEIRO | numeric(23,8) YES | `SaldoAnteriorFinanceiro` / `TotalCreditoFinanceiro` / `TotalDebitoFinanceiro` (decimal?) |

`SaldoEconomico` / `SaldoFinanceiro` = calculadas, **8 casas** — `CALC-E3-04`.

### 2.6 `FINCOBRADOR` (13) → `Cobrador`

PK **`(IDFINCOBRADOR, IDGLOFILIAL)`**. `IdCobrador`, `IdFilial`, `IdEntidade` (int, FK lógica MOD-02),
`Nome` (varchar(100) NO → `string = string.Empty`), `Ativo` (smallint NO → bool),
`IdUsuario?` (int), `IdMeioContato?` (int) + auditoria (6).

### 2.7 `IndiceConversor` — **serviço, sem tabela**

`ObjectPersist` mas nunca persiste. Vira `ConversorIndiceService` em `Application/Bases/`.
Consome `IIndiceEconomico` do MOD-02 por `int` lógico. Fórmulas: `CALC-E3-05..07`.

---

## 3. Comportamento (VAL / OP) — resumo por classe

| Classe | Validações | Operações |
| :--- | :--- | :--- |
| `CaixaBanco` | VAL-E3-01 (usuário duplicado), -02..07 (conta bancária SPED / investimento), -08 (inativar caixa com período aberto), -09 (parâmetros mutuamente exclusivos), -10 (`ValidarCaixaPeriodo`) | OP-E3-01/02 (persistir/excluir), -03 (gancho pré-persist), -04 (máquina tipo de conta), -07 (usuários em lote) |
| `ContaBancaria` | VAL-E3-13..17 (boleto/SPED/terceiro), -18 (dias de protesto), -19 (CPF/CNPJ) — **maioria `[E14]`** | OP-E3-05/06 (persistir/excluir; sequenciais de arquivo `[E14]`) |
| `CaixaBancoUsuario` | VAL-E3-11 (unicidade), -12 (não altera usuário salvo) | (agregado de `CaixaBanco` — OP-E3-07) |
| `SaldoCaixaBanco` | — | OP-E3-08 (consulta por data/tipo) · CALC-E3-01..03 |
| `SaldoRateio` | — (só dados) | CALC-E3-04 |
| `Cobrador` | — (CRUD simples) | — |
| `IndiceConversor` | `ValorNulo` (índice origem/destino), `DataSemIndiceEconomico` | CALC-E3-05..07 |

---

## 4. Pendências / dependências cross-épico

- **VAL-E1-27** (`ValidarCaixaBanco` de parcela — E1) **fecha aqui**: a regra "conta Banco → conta
  corrente" depende de `CaixaBanco.TipoConta` + `ContaBancaria.ContaBancariaTipo` (E3). O
  `[Fact]` de VAL-E1-27 vai para `E3-T05`.
- **VAL-E3-08 / -10** dependem da **máquina de estados do período (E2)** — o teste de `E3-T05`
  mocka a porta de "período aberto/fechado"; a integração real fecha no E2.
- **`SaldoRateio` PK com colunas anuláveis** (`DÚVIDA-E3-1`) — confirmar no `E3-T03` (mapeamento).
- **`FININDICECONVERSOR` não existe** no schema — confirmado que `IndiceConversor` é serviço.
- **~40 colunas `[E14]` de `FINCONTABANCARIA`** + `VAL-E3-13..16, 18, 19` + os sequenciais de
  arquivo de `OP-E3-05/06` → épico **E14**.
- `IDCONPLANOCONTABIL` (plano de contas contábil) — FK lógica para módulo contábil fora do MOD-05.

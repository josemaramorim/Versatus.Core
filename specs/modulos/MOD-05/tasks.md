# Tarefas — MOD-05: Gestão Financeira

> **Versão:** 1.0 | **Data:** 2026-09-03 | **SDD etapa 4** (`sdd-tasks`)
> **Base:** [`plan.md`](./plan.md) v1.0 · [`data-model.md`](./data-model.md) ·
> [`contracts/`](./contracts/) · [`constitution.md`](../../memory/constitution.md) v1.0
>
> **Regra Git (Artigo X):** 1 tarefa = 1 branch `feat/mod-05-<slug>` a partir de `develop`
> = commits atômicos. **Nunca** commitar em `develop`/`main`. Ao concluir a branch,
> **PERGUNTAR** antes de sugerir merge. IDs estáveis — não renumerar.
>
> **Legenda `tipo`:** `setup` · `analysis` · `domain` · `dbcontext` · `service` ·
> `operation` · `contract` · `parity` · `frontend` · `migration` · `di`.
>
> **`Cobre:`** — `RN-05-NNN` da spec (existem hoje); `VAL-xx`/`OP-xx`/`CALC-xx` são
> criados na tarefa `analysis` do épico e referenciados pelas tarefas seguintes.
> `sdd-analyze` V2/V4 exige que toda `VAL-xx`/`OP-xx`/`CALC-xx` apareça em ≥1 tarefa.
>
> **`Pronto quando:`** sempre inclui `dotnet build` 0 erro/0 aviso (Lei 6) — omitido
> abaixo por brevidade quando é o único critério; matar `dotnet run` antes do build.

---

## Fase S — Setup (pré-requisito de tudo)

### S-T01 · Criar `Versatus.SharedKernel` · tipo: setup
- **Objetivo:** projeto Class Library `net10.0`, adicionado à solução, referência a `Versatus.Framework`.
- **Cria:** `src/Versatus.SharedKernel/Versatus.SharedKernel.csproj` (padrão do `Versatus.GestaoTributo.csproj`, pacotes 10.x).
- **Constituição:** Seção 0 (net10.0), Artigo XI.
- **Pronto quando:** `dotnet sln add` + `dotnet build` 0/0.
- **Branch:** `feat/mod-05-sharedkernel-project` · **Commit:** `setup(mod-05): cria projeto Versatus.SharedKernel` · **Depende de:** —

### S-T02 · Criar `Versatus.GestaoFinanceira` + estrutura de pastas · tipo: setup
- **Objetivo:** projeto `net10.0` com referências (`Framework`, `SharedKernel`, `AcessoGlobal`) e as pastas de `plan.md §1.2`.
- **Cria:** `.csproj` + pastas `Api/Controllers`, `Application/{Services,Handlers,Bases}`, `Domain/{Bases,Dominio,Bancos,Documentos,Movimentos,Liquidacao,Reversao,Cheques,Adiantamentos,DRE,Cobranca,TransacaoFilial,Consultas,Repositories,DTOs}`, `Infrastructure/{Mappings,Repositories}`, `DependencyInjection`.
- **Constituição:** Artigos III, IV.
- **Branch:** `feat/mod-05-financeira-project` · **Commit:** `setup(mod-05): cria projeto Versatus.GestaoFinanceira e estrutura` · **Depende de:** S-T01

### S-T03 · DbContexts Write/Read + RepositorioBase · tipo: dbcontext
- **Objetivo:** `GestaoFinanceiraDbContext` (Write) + `GestaoFinanceiraReadDbContext` (Read, `NoTracking`) com `ConfigureConventions` (`Properties<bool>().HaveConversion<short>()`) e `ApplyConfigurationsFromAssembly`; `GestaoFinanceiraRepositorioBase<T>` (Context=escrita, ReadContext=leitura).
- **Cria:** `Infrastructure/GestaoFinanceiraDbContext.cs`, `...ReadDbContext.cs`, `Infrastructure/Repositories/GestaoFinanceiraRepositorioBase.cs`.
- **Constituição:** Artigo VII.4 (CQRS), Artigo III.5 (bool→short).
- **Branch:** `feat/mod-05-dbcontext-base` · **Commit:** `setup(mod-05): DbContexts Write/Read e RepositorioBase` · **Depende de:** S-T02

### S-T04 · Projeto de testes + DI + registro na WebAPI · tipo: di
- **Objetivo:** `tests/Versatus.GestaoFinanceira.Tests` (`net10.0`, InMemory, xunit); `ServiceCollectionExtensions.AddGestaoFinanceira()`; registrar 2 DbContexts + `AddSharedKernel()` + `AddGestaoFinanceira()` + `AddApplicationPart(...)` no `src/Versatus.WebAPI/Program.cs`.
- **Cria:** `.Tests.csproj`, `DependencyInjection/ServiceCollectionExtensions.cs`; altera `Program.cs`.
- **Pronto quando:** `dotnet build` + `dotnet test` (0 testes) verdes; WebAPI sobe.
- **Branch:** `feat/mod-05-di-e-testes` · **Commit:** `setup(mod-05): projeto de testes, DI e registro na WebAPI` · **Depende de:** S-T03

---

## Épico E0 — `Versatus.SharedKernel` (escopo mínimo — CLR-02)

### E0-T01 · Inventário de enums financeiros · tipo: analysis
- **Objetivo:** listar todos os enums de `Projeto.Geral.Enumerado`/`.EnumeradoObjeto` usados pelo MOD-05 com **valores inteiros** e `[TipoEnumerado(idPai)]`; classificar as colunas `ID*` do `data-model.md` como enum vs. FK.
- **Refs legados:** `projeto_tag_1906/Geral/{tipoenumerado.cs, TipoEnumeradoObjeto.cs, EnumDescriptor.cs}`, `Projeto.Geral.Enumerado*`; `research.md §2`.
- **Cria:** `specs/modulos/MOD-05/enums.md`.
- **Cobre:** RN-05 (enums de situação/tipo), suporte a `data-model.md`.
- **Branch:** `feat/mod-05-e0-analise-enums` · **Commit:** `docs(mod-05): inventário de enums (E0-T01)` · **Depende de:** S-T01

### E0-T02 · Enums no SharedKernel · tipo: domain
- **Objetivo:** criar os enums de `enums.md` em `Versatus.SharedKernel/Enums/`, valores inteiros preservados, comentário de origem.
- **Cria:** `src/Versatus.SharedKernel/Enums/*.cs`.
- **Cobre:** E0-T01.
- **Constituição:** Artigo V.5, Artigo II.6.
- **Branch:** `feat/mod-05-e0-enums` · **Commit:** `feat(mod-05): enums financeiros no SharedKernel (E0-T02)` · **Depende de:** E0-T01

### E0-T03 · Interfaces transversais + container de rateio · tipo: domain
- **Objetivo:** `IMovimentoPeriodo`, `IDadosPeriodoFormaPagto`, `IDadosRateioFinanceiro`, `IDadosComissao` (CLR-05) e o container de acumulação de rateio (`RateioContainer`) em `Versatus.SharedKernel`.
- **Refs legados:** `Interface.GestaoFinanceira`, `Projeto.Geral` `RateioMovto`/`ValidationRateioContainer`.
- **Cria:** `src/Versatus.SharedKernel/Abstractions/*.cs`, `.../Rateio/RateioContainer.cs`.
- **Cobre:** RN-05-004, RN-05-002.
- **Branch:** `feat/mod-05-e0-abstracoes` · **Commit:** `feat(mod-05): interfaces transversais e container de rateio (E0-T03)` · **Depende de:** E0-T02

### E0-T04 · Testes do SharedKernel + `AddSharedKernel()` · tipo: service
- **Objetivo:** testes de valor de enum (paridade com o legado) e registro DI `AddSharedKernel()`.
- **Cria:** `tests/.../SharedKernel/EnumValoresTests.cs`, `src/Versatus.SharedKernel/DependencyInjection/ServiceCollectionExtensions.cs`.
- **Cobre:** E0-T02.
- **Pronto quando:** `dotnet test` verde.
- **Branch:** `feat/mod-05-e0-testes` · **Commit:** `test(mod-05): valores de enum do SharedKernel (E0-T04)` · **Depende de:** E0-T03

---

## Épico E1 — Bases do módulo (CLR-04: POCO abstrata + serviço)

### E1-T01 · Auditoria das bases legadas (RTV + ROT) · tipo: analysis
- **Objetivo:** rodar `legacy-validation-audit` + `legacy-operation-audit` em `DocumentoFinanceiroBase` (1303), `OperacaoDocumentoBase` (1098), `ItemFinanceiroBase` (456), `ParcelaGeral` (609), `ParcelaBase` (772), `FormaMovInfo` (146), `FechamentoCaixaBase` (448) — subindo a cadeia até `ObjectBase`. Separar **dados compartilhados** (→ POCO abstrata) de **comportamento** (→ serviço).
- **Refs legados:** os 7 arquivos + `servidor/framework/servidor.framework/ObjetoNegocio.cs`.
- **Cria:** `matriz-rtv.md#E1`, `matriz-rot.md#E1`, `specs/modulos/MOD-05/analysis/E1-bases.md`.
- **Cobre:** RN-05-001, RN-05-004, RN-05-007, RN-05-008.
- **Branch:** `feat/mod-05-e1-analise-bases` · **Commit:** `docs(mod-05): auditoria das bases legadas — RTV/ROT E1 (E1-T01)` · **Depende de:** E0-T04

### E1-T02 · POCOs abstratas de base · tipo: domain
- **Objetivo:** classes abstratas **só de dados** em `Domain/Bases/`: `DocumentoFinanceiroBase`, `ItemFinanceiroBase`, `ParcelaGeralBase`, `ParcelaBase`, `FormaMovInfoBase`, `FechamentoCaixaBase`. Sem herança de framework, sem atributos.
- **Cria:** `src/Versatus.GestaoFinanceira/Domain/Bases/*.cs`.
- **Cobre:** E1-T01; RN-05-001, RN-05-008.
- **Constituição:** Artigo III, §3.3 da spec.
- **Branch:** `feat/mod-05-e1-pocos-base` · **Commit:** `feat(mod-05): POCOs abstratas de base (E1-T02)` · **Depende de:** E1-T01

### E1-T03 · Serviços de comportamento de base · tipo: service
- **Objetivo:** serviços/handlers compartilhados em `Application/Bases/` que substituem o comportamento das bases (ex.: `RateioServiceBase`, `PersistenciaDocumentoBase`, `OperacaoDocumentoBaseHandler` abstrato). Só o esqueleto + contratos; a lógica concreta entra nos épicos que herdam.
- **Cria:** `src/Versatus.GestaoFinanceira/Application/Bases/*.cs`.
- **Cobre:** E1-T01 (OP-xx de base); RN-05-004.
- **Constituição:** Artigo VI, Artigo VII.
- **Branch:** `feat/mod-05-e1-servicos-base` · **Commit:** `feat(mod-05): serviços de comportamento de base (E1-T03)` · **Depende de:** E1-T02

### E1-T04 · Testes de base (RTV) · tipo: parity
- **Objetivo:** 1 `[Fact]` por linha `VAL-xx#E1`; testes de cálculo de base → `CALC-xx#E1` (conversão por índice).
- **Cria:** `tests/.../E1/*Tests.cs`, `golden/CALC-E1-*.csv` (origem conforme `research.md §5`).
- **Cobre:** todas as `VAL-xx#E1`, `CALC-xx#E1`.
- **Pronto quando:** `dotnet test` verde.
- **Branch:** `feat/mod-05-e1-testes` · **Commit:** `test(mod-05): RTV e paridade das bases (E1-T04)` · **Depende de:** E1-T03

---

## Épico E3 — Caixa e Banco *(executa antes do E2 — `plan.md §3`)*

### E3-T01 · Auditoria RTV/ROT + schema · tipo: analysis
- **Objetivo:** `legacy-validation-audit` + `legacy-operation-audit` em `CaixaBanco` (869), `ContaBancaria` (1704), `CaixaBancoUsuario`, `SaldoCaixaBanco`, `SaldoRateio`, `Cobrador`, `IndiceConversor`. Confirmar colunas contra `legacy-schema/fin_columns.txt`. **Marcar em `ContaBancaria` as ~40 colunas de integração (E14).**
- **Cria:** `matriz-rtv.md#E3`, `matriz-rot.md#E3`, `analysis/E3-caixa-banco.md` (mapa coluna→propriedade de cada tabela).
- **Cobre:** RN-05-005, RN-05-006, RN-05-019.
- **Branch:** `feat/mod-05-e3-analise` · **Commit:** `docs(mod-05): auditoria E3 Caixa/Banco (E3-T01)` · **Depende de:** E1-T04

### E3-T02 · Entidades de domínio E3 · tipo: domain
- **Objetivo:** `CaixaBanco`, `CaixaBancoUsuario`, `ContaBancaria` (55 props — todas, Regra 4), `SaldoCaixaBanco`, `SaldoRateio`, `Cobrador`, `IndiceConversor` como POCOs puras em `Domain/Bancos/`. Coleções de agregado = `List<T>` privada + `IReadOnlyList<T>`.
- **Cria:** `src/Versatus.GestaoFinanceira/Domain/Bancos/*.cs`.
- **Cobre:** E3-T01; RN-05-006.
- **Constituição:** Artigo III, Artigo V.1.
- **Branch:** `feat/mod-05-e3-entidades` · **Commit:** `feat(mod-05): entidades de Caixa/Banco (E3-T02)` · **Depende de:** E3-T01

### E3-T03 · Mappings Fluent API E3 · tipo: domain
- **Objetivo:** 1 arquivo por entidade em `Infrastructure/Mappings/`: `ToTable` (nome MAIÚSCULO real), `HasKey(new {...})` na ordem física, `HasColumnName`, `HasColumnType("text")`, `HasPrecision(23,8)`/`(17,2)`, nulidade coluna a coluna do extract, `.ValueGeneratedNever()` nas PKs. `ContaBancaria`: `HasOne<CaixaBanco>().WithOne().HasForeignKey<ContaBancaria>(x => new {x.IdCaixaBanco, x.IdFilial})`.
- **Cria:** `Infrastructure/Mappings/{CaixaBanco,CaixaBancoUsuario,ContaBancaria,SaldoCaixaBanco,SaldoRateio,Cobrador,IndiceConversor}Mapping.cs`.
- **Cobre:** E3-T01; RN-05-006.
- **Constituição:** Artigo II.1/II.5, Artigo III.2.
- **Branch:** `feat/mod-05-e3-mappings` · **Commit:** `feat(mod-05): mappings Fluent API E3 (E3-T03)` · **Depende de:** E3-T02

### E3-T04 · DbSets E3 + teste de mapeamento · tipo: dbcontext
- **Objetivo:** registrar DbSets no `GestaoFinanceiraDbContext`; teste InMemory materializa/persiste cada entidade e valida PK composta.
- **Cria:** altera `GestaoFinanceiraDbContext.cs`; `tests/.../E3/MapeamentoE3Tests.cs`.
- **Cobre:** E3-T03.
- **Pronto quando:** `dotnet test` verde.
- **Branch:** `feat/mod-05-e3-dbcontext` · **Commit:** `feat(mod-05): DbSets e teste de mapeamento E3 (E3-T04)` · **Depende de:** E3-T03

### E3-T05 · Serviços + repositórios E3 (CRUD + RTV) · tipo: service
- **Objetivo:** `ICaixaBancoService`/`CaixaBancoService` (CRUD + usuários em lote), `IContaBancariaService` (núcleo), `ICobradorService`; repositórios CQRS; paginação materializada. 1 `[Fact]` por `VAL-xx#E3`.
- **Cria:** `Domain/Services/*`, `Domain/Repositories/*`, `Infrastructure/Repositories/*`, `tests/.../E3/*ServiceTests.cs`.
- **Cobre:** todas as `VAL-xx#E3`; RN-05-005, RN-05-019.
- **Constituição:** Artigo IV, VI, VII.4/VII.5.
- **Branch:** `feat/mod-05-e3-servicos` · **Commit:** `feat(mod-05): serviços E3 + testes RTV (E3-T05)` · **Depende de:** E3-T04

### E3-T06 · Controllers E3 · tipo: contract
- **Objetivo:** `CaixaBancoController`, `ContaBancariaController`, `CobradorController` conforme `contracts/caixa-banco.md`; finos; `Result<T>`→`400`.
- **Cria:** `Api/Controllers/*`, `Domain/DTOs/*` (records).
- **Cobre:** `contracts/caixa-banco.md`.
- **Constituição:** Artigo IV, Regra 17.
- **Branch:** `feat/mod-05-e3-controllers` · **Commit:** `feat(mod-05): controllers E3 (E3-T06)` · **Depende de:** E3-T05

### E3-T07 · Frontend Caixa/Banco · tipo: frontend
- **Objetivo:** `src/pages/Financeiro/FCaixaBanco/` (types, schema, `CaixaBancoCadastroConfig.tsx` estendendo `BaseCadastroConfig<T>`, `index.tsx`) + `schema.test.ts` (Vitest) por regra RTV. Campos obrigatórios com `required` (Lei 10).
- **Cria:** `src/Versatus.Frontend/src/pages/Financeiro/FCaixaBanco/*`.
- **Cobre:** `VAL-xx#E3` visuais.
- **Pronto quando:** `npm run build` + `npm test` verdes.
- **Branch:** `feat/mod-05-e3-frontend` · **Commit:** `feat(mod-05): tela Caixa/Banco React (E3-T07)` · **Depende de:** E3-T06

### E3-T08 · Migration EF Core E3 · tipo: migration
- **Objetivo:** primeira migration do `GestaoFinanceiraDbContext` cobrindo E3; validar contra o banco real (schema-first — a migration não recria tabelas, só sincroniza o modelo).
- **Cria:** `Infrastructure/Migrations/*`.
- **Branch:** `feat/mod-05-e3-migration` · **Commit:** `setup(mod-05): migration inicial E3 (E3-T08)` · **Depende de:** E3-T04

---

## Épico E2 — Domínio e Período

### E2-T01 · Auditoria RTV/ROT + máquina de estados · tipo: analysis
- **Objetivo:** `legacy-validation-audit` + `legacy-operation-audit` em `Dominio` (1717), `DominioPeriodo` (1017), `DominioPeriodoLacto` (823), `DominioPeriodoFechamento`(+Detalhe/+Lista), `DominioPeriodoFormaPagto`, `DominioPeriodoLog`, `DominioResponsavel`, `DominioUsuario`, `PeriodosAbertos` (551). Operações: **AbrirPeriodo**, **FecharPeriodo**, **FecharTesouraria**, **CalcularTotalFormasPagto**. Máquina de estados do período. Reconfirmar `FINLOGDOMINIOPERIODO` no banco (R-1).
- **Cria:** `matriz-rtv.md#E2`, `matriz-rot.md#E2` (+ máquina de estados), `analysis/E2-dominio.md`.
- **Cobre:** RN-05-002, RN-05-013, RN-05-019.
- **Branch:** `feat/mod-05-e2-analise` · **Commit:** `docs(mod-05): auditoria E2 Domínio/Período (E2-T01)` · **Depende de:** E3-T08

### E2-T02 · Entidades + mappings E2 · tipo: domain
- **Objetivo:** POCOs em `Domain/Dominio/` (`Dominio` com FK `IdCaixaBanco`; `DominioPeriodo` com colunas de data redundantes; `DominioPeriodoFormaPagto.Hora` como `string`) + mappings Fluent API (PK composta, `smallint→bool`).
- **Cria:** `Domain/Dominio/*.cs`, `Infrastructure/Mappings/Dominio*.cs`.
- **Cobre:** E2-T01; RN-05-002.
- **Branch:** `feat/mod-05-e2-entidades` · **Commit:** `feat(mod-05): entidades e mappings E2 (E2-T02)` · **Depende de:** E2-T01

### E2-T03 · DbSets E2 + teste de mapeamento · tipo: dbcontext
- **Cria:** altera `GestaoFinanceiraDbContext`; `tests/.../E2/MapeamentoE2Tests.cs`.
- **Cobre:** E2-T02. **Branch:** `feat/mod-05-e2-dbcontext` · **Commit:** `feat(mod-05): DbSets e mapeamento E2 (E2-T03)` · **Depende de:** E2-T02

### E2-T04 · `PeriodosAbertos` + `DominioService` (CRUD + RTV) · tipo: service
- **Objetivo:** serviço de consulta `PeriodosAbertos`; CRUD de `Dominio` e cadastros filhos; 1 `[Fact]` por `VAL-xx#E2`.
- **Cria:** `Domain/Services/DominioService.cs`, `.../PeriodosAbertosService.cs`, repos, testes.
- **Cobre:** `VAL-xx#E2`; RN-05-002.
- **Branch:** `feat/mod-05-e2-servicos` · **Commit:** `feat(mod-05): serviços E2 + RTV (E2-T04)` · **Depende de:** E2-T03

### E2-T05 · Handlers de período (ROT) · tipo: operation
- **Objetivo:** `AbrirPeriodoHandler`, `FecharPeriodoHandler`, `FecharTesourariaHandler`, `FecharCaixaHandler` — 1 transação cada; ordem de persistência de `matriz-rot.md#E2`; `CalcularTotalFormasPagto` (conferido × calculado). Testes de integração + golden por `OP-xx#E2`/`CALC-xx#E2`.
- **Cria:** `Application/Handlers/Periodo/*.cs`, `tests/.../E2/*OperationTests.cs`, `golden/CALC-E2-*.csv`.
- **Cobre:** todas as `OP-xx#E2`, `CALC-xx#E2`; RN-05-013.
- **Constituição:** Artigo VII.1/VII.2/VII.3, Artigo IX.2/IX.3.
- **Branch:** `feat/mod-05-e2-handlers` · **Commit:** `feat(mod-05): handlers de período + ROT (E2-T05)` · **Depende de:** E2-T04

### E2-T06 · Controllers E2 + `contracts/dominio-periodo.md` · tipo: contract
- **Cria:** `contracts/dominio-periodo.md`, `Api/Controllers/Dominio*Controller.cs`, DTOs.
- **Cobre:** `contracts/dominio-periodo.md`. **Branch:** `feat/mod-05-e2-controllers` · **Commit:** `feat(mod-05): contratos e controllers E2 (E2-T06)` · **Depende de:** E2-T05

### E2-T07 · Migration E2 · tipo: migration
- **Branch:** `feat/mod-05-e2-migration` · **Commit:** `setup(mod-05): migration E2 (E2-T07)` · **Depende de:** E2-T03

---

## Épico E4 — Documento e Parcela

### E4-T01 · Análise de `Documento.cs` (2803) · tipo: analysis
- **Objetivo:** leitura integral + `legacy-validation-audit` + `legacy-operation-audit` + `legacy-calc-parity`. Cobrir `ExecutarPersistir`, `ExecutarExcluir`, `AplicarRateioTributos`, `ValidarOperacao`, `PersistirRateio`, geração de parcelas pela condição de pagamento.
- **Cria:** `analysis/E4-documento.md`, `matriz-rtv.md#E4-documento`, `matriz-rot.md#E4-documento`, `golden/CALC-E4-doc-*.md`.
- **Cobre:** RN-05-001, RN-05-007, RN-05-008, RN-05-015, RN-05-020.
- **Branch:** `feat/mod-05-e4-analise-documento` · **Commit:** `docs(mod-05): análise de Documento.cs (E4-T01)` · **Depende de:** E2-T07

### E4-T02 · Análise de `DocumentoParcela.cs` (2111) · tipo: analysis
- **Objetivo:** idem para a parcela (título): situação, prazo, portador, item financeiro acumulado, colunas de boleto (marcar E14). PK com **filial 1º**.
- **Cria:** `analysis/E4-parcela.md`, `matriz-rtv.md#E4-parcela`, `matriz-rot.md#E4-parcela`, `golden/CALC-E4-parc-*.md`.
- **Cobre:** RN-05-001, RN-05-020.
- **Branch:** `feat/mod-05-e4-analise-parcela` · **Commit:** `docs(mod-05): análise de DocumentoParcela.cs (E4-T02)` · **Depende de:** E4-T01

### E4-T03 · Entidades + mappings E4 · tipo: domain
- **Objetivo:** `Documento` (agregado: `DocumentoParcela[]`, `DocumentoMovto[]`, `DoctoItemFinanceiro[]`, `DocumentoTributo[]`, `DocumentoComissionado[]`, `DocumentoCartao?`), `DoctoCancelado`(+Parcela), `DoctoMovtoItemFinanceiro` — POCOs + mappings (PK composta variável, `numeric(23,8)`, `text` em `HISTORICO`, nulidade não-uniforme de auditoria).
- **Cria:** `Domain/Documentos/*.cs`, `Infrastructure/Mappings/*.cs`.
- **Cobre:** E4-T01, E4-T02; RN-05-006.
- **Branch:** `feat/mod-05-e4-entidades` · **Commit:** `feat(mod-05): entidades e mappings E4 (E4-T03)` · **Depende de:** E4-T02

### E4-T04 · DbSets E4 + teste de mapeamento · tipo: dbcontext
- **Cobre:** E4-T03. **Branch:** `feat/mod-05-e4-dbcontext` · **Commit:** `feat(mod-05): DbSets e mapeamento E4 (E4-T04)` · **Depende de:** E4-T03

### E4-T05 · `DocumentoService` (consulta) + RTV · tipo: service
- **Objetivo:** consultas paginadas de documento e parcela (`contracts/documento.md`), detalhe completo com `Include`; 1 `[Fact]` por `VAL-xx#E4`.
- **Cobre:** `VAL-xx#E4`; RN-05-001. **Branch:** `feat/mod-05-e4-servico-consulta` · **Commit:** `feat(mod-05): consultas de documento/parcela + RTV (E4-T05)` · **Depende de:** E4-T04

### E4-T06 · Cálculo de itens financeiros (paridade) · tipo: parity
- **Objetivo:** `CalculadoraItemFinanceiro` (juros a.m., multa %, desconto, conversão por índice) transcrita **sem refatorar** (Regra 5); golden tests com igualdade exata de `decimal`.
- **Cria:** `Application/Bases/CalculadoraItemFinanceiro.cs`, `tests/.../E4/Parity/*ParityTests.cs`, `golden/CALC-E4-*.csv`.
- **Cobre:** todas as `CALC-xx#E4`; RN-05-020.
- **Constituição:** Artigo IX.3.
- **Branch:** `feat/mod-05-e4-paridade-calculo` · **Commit:** `test(mod-05): paridade de cálculo de itens financeiros (E4-T06)` · **Depende de:** E4-T05

### E4-T07 · Handlers de documento (ROT) · tipo: operation
- **Objetivo:** `IncluirDocumentoHandler` (gera parcelas pela condição de pagamento + itens + rateio), `AlterarDocumentoHandler`, `CancelarDocumentoHandler`, `ManutencaoParcelaHandler`, `AprovarParcelaHandler`. 1 transação cada; testes integração + rollback por `OP-xx#E4`.
- **Cobre:** todas as `OP-xx#E4`; RN-05-001, RN-05-007, RN-05-015.
- **Branch:** `feat/mod-05-e4-handlers` · **Commit:** `feat(mod-05): handlers de documento + ROT (E4-T07)` · **Depende de:** E4-T06

### E4-T08 · Controllers E4 · tipo: contract
- **Cobre:** `contracts/documento.md`. **Branch:** `feat/mod-05-e4-controllers` · **Commit:** `feat(mod-05): controllers E4 (E4-T08)` · **Depende de:** E4-T07

### E4-T09 · Frontend Documento/Parcela · tipo: frontend
- **Objetivo:** `src/pages/Financeiro/FDocumento/` + `FConsultaParcelaFinanceiro/` (consulta, inclusão, manutenção) + `schema.test.ts`. `required` nos obrigatórios.
- **Cobre:** `VAL-xx#E4` visuais.
- **Pronto quando:** `npm run build` + `npm test` verdes.
- **Branch:** `feat/mod-05-e4-frontend` · **Commit:** `feat(mod-05): telas de Documento/Parcela React (E4-T09)` · **Depende de:** E4-T08

### E4-T10 · Migration E4 · tipo: migration
- **Branch:** `feat/mod-05-e4-migration` · **Commit:** `setup(mod-05): migration E4 (E4-T10)` · **Depende de:** E4-T04

---

## Épico E5 — Movimento e Formas de Pagamento

> **Pré-requisito externo (CLR-01):** MOD-02 deve ter migrado `RateioMovto`,
> `RateioMovtoItem`, `ManutencaoRateio` (+ tabelas `FinRateio`/`FinClasse`) antes de
> E5-T03. Rastrear como dependência bloqueante no board do projeto.

### E5-T01 · Análise de `MovimentoFinanceiro.cs` (2136) · tipo: analysis
- **Objetivo:** `legacy-*-audit` cobrindo `ExecutarPersistir/Excluir`, `ValidarRateio`, `ValidarControleSaldo`, `ValidarValorMovto`, `PersistirConciliacao`. + análise das 11 `FormaMovInfo*` e `formapagamentomov`.
- **Cria:** `analysis/E5-movimento.md`, `matriz-rtv.md#E5`, `matriz-rot.md#E5`, `golden/CALC-E5-*.md`.
- **Cobre:** RN-05-003, RN-05-004, RN-05-005.
- **Branch:** `feat/mod-05-e5-analise` · **Commit:** `docs(mod-05): análise de MovimentoFinanceiro e formas (E5-T01)` · **Depende de:** E4-T10

### E5-T02 · Entidades + mappings E5 · tipo: domain
- **Objetivo:** `MovimentoFinanceiro` (+ `MovtoFinanceiroCheque` especialização), `MovtoFinanceiroRateio` (herda base do SharedKernel/MOD-02), `formapagamentomov`, `FormaMovInfo*` (11 + `TrocoDinheiro`), `ItemFinanceiro` (global, sem filial), `ItemFinanceiroOperacao`, `AplicacaoItemFin`(+Geral), `ContraPartidaRateioDoctoFinanceiro`, `ManutencaoRateioFinanceiro`.
- **Cobre:** E5-T01; RN-05-003, RN-05-006.
- **Branch:** `feat/mod-05-e5-entidades` · **Commit:** `feat(mod-05): entidades e mappings E5 (E5-T02)` · **Depende de:** E5-T01 **+ MOD-02 rateio**

### E5-T03 · DbSets E5 + mapeamento · tipo: dbcontext
- **Cobre:** E5-T02. **Branch:** `feat/mod-05-e5-dbcontext` · **Commit:** `feat(mod-05): DbSets e mapeamento E5 (E5-T03)` · **Depende de:** E5-T02

### E5-T04 · `ItemFinanceiro` CRUD + serviços de forma (RTV) · tipo: service
- **Cobre:** `VAL-xx#E5`. **Branch:** `feat/mod-05-e5-servicos` · **Commit:** `feat(mod-05): serviços E5 + RTV (E5-T04)` · **Depende de:** E5-T03

### E5-T05 · Cálculo/validação de saldo (paridade) · tipo: parity
- **Objetivo:** `ValidadorSaldoCaixaBanco` + `CalculadoraSaldo` (conciliado × normal) — golden exatos.
- **Cobre:** `CALC-xx#E5`; RN-05-005.
- **Branch:** `feat/mod-05-e5-paridade-saldo` · **Commit:** `test(mod-05): paridade de saldo (E5-T05)` · **Depende de:** E5-T04

### E5-T06 · Handlers de movimento (ROT) · tipo: operation
- **Objetivo:** `IncluirMovimentoHandler`, `ExcluirMovimentoHandler`, `ConciliarMovimentoHandler` — transação + rateio + saldo + período.
- **Cobre:** `OP-xx#E5`; RN-05-004, RN-05-005.
- **Branch:** `feat/mod-05-e5-handlers` · **Commit:** `feat(mod-05): handlers de movimento + ROT (E5-T06)` · **Depende de:** E5-T05

### E5-T07 · Controllers E5 + `contracts/movimento.md` · tipo: contract
- **Branch:** `feat/mod-05-e5-controllers` · **Commit:** `feat(mod-05): contratos e controllers E5 (E5-T07)` · **Depende de:** E5-T06

### E5-T08 · Migration E5 · tipo: migration
- **Branch:** `feat/mod-05-e5-migration` · **Commit:** `setup(mod-05): migration E5 (E5-T08)` · **Depende de:** E5-T03

---

## Épico E6 — Liquidação

### E6-T01 · Análise de `Liquidacao.cs` + `OperacaoDocumentoBase` · tipo: analysis
- **Objetivo:** `legacy-operation-audit` + `legacy-calc-parity` cobrindo `ValidarLiquidacao`, `PersistirLiquidacao`, `AtualizarRateio`, `CriarItemFinanceiroLista`; ordem exata de persistência de `plan.md §5`.
- **Cria:** `analysis/E6-liquidacao.md`, `matriz-rtv.md#E6`, `matriz-rot.md#E6` (OP-Liquidar), `golden/CALC-E6-*.md`.
- **Cobre:** RN-05-001, RN-05-003, RN-05-020.
- **Branch:** `feat/mod-05-e6-analise` · **Commit:** `docs(mod-05): análise de Liquidação (E6-T01)` · **Depende de:** E5-T08

### E6-T02 · Entidades + mappings E6 · tipo: domain
- **Objetivo:** `Liquidacao` (não tem tabela própria — opera sobre `FinMovimento`/`FinLiquidacaoFormaMovimento`), `LiquidacaoFormaMovimento` (PK 3 col.).
- **Cobre:** E6-T01. **Branch:** `feat/mod-05-e6-entidades` · **Commit:** `feat(mod-05): entidades e mappings E6 (E6-T02)` · **Depende de:** E6-T01

### E6-T03 · DbSets + mapeamento E6 · tipo: dbcontext
- **Branch:** `feat/mod-05-e6-dbcontext` · **Commit:** `feat(mod-05): DbSets e mapeamento E6 (E6-T03)` · **Depende de:** E6-T02

### E6-T04 · Serviço de simulação + RTV · tipo: service
- **Objetivo:** `LiquidacaoSimulacaoService` (`parcelas-abertas`, `simular` de `contracts/liquidacao.md`); 1 `[Fact]` por `VAL-xx#E6` (soma de formas = líquido, período aberto, forma habilitada).
- **Cobre:** `VAL-xx#E6`; RN-05-003.
- **Branch:** `feat/mod-05-e6-simulacao` · **Commit:** `feat(mod-05): simulação de liquidação + RTV (E6-T04)` · **Depende de:** E6-T03

### E6-T05 · Paridade de cálculo de liquidação · tipo: parity
- **Cobre:** `CALC-xx#E6`; RN-05-020. **Branch:** `feat/mod-05-e6-paridade` · **Commit:** `test(mod-05): paridade de cálculo de liquidação (E6-T05)` · **Depende de:** E6-T04

### E6-T06 · `LiquidarParcelaHandler` (ROT) · tipo: operation
- **Objetivo:** 1 transação; ordem de persistência de `plan.md §5`; integração com cheque recebido (E9) via porta/interface; teste integração + rollback por `OP-xx#E6`.
- **Cobre:** todas as `OP-xx#E6`; RN-05-001, RN-05-002, RN-05-005.
- **Constituição:** Artigo VII.1/VII.2/VII.3.
- **Branch:** `feat/mod-05-e6-handler` · **Commit:** `feat(mod-05): LiquidarParcelaHandler + ROT (E6-T06)` · **Depende de:** E6-T05

### E6-T07 · Controller E6 · tipo: contract
- **Cobre:** `contracts/liquidacao.md`. **Branch:** `feat/mod-05-e6-controller` · **Commit:** `feat(mod-05): controller de liquidação (E6-T07)` · **Depende de:** E6-T06

### E6-T08 · Frontend Liquidação · tipo: frontend
- **Objetivo:** `src/pages/Financeiro/FLiquidacaoDocumento/` (seleção de parcelas, formas múltiplas, simulação, confirmação) + `schema.test.ts`.
- **Cobre:** `VAL-xx#E6` visuais.
- **Branch:** `feat/mod-05-e6-frontend` · **Commit:** `feat(mod-05): tela de Liquidação React (E6-T08)` · **Depende de:** E6-T07

---

## Épico E7 — Estorno de Liquidação

### E7-T01 · Análise de `LiquidacaoEstorno.cs` (2617) + `LiquidacaoEstornoFormaPagto` (1074) · tipo: analysis
- **Objetivo:** `legacy-operation-audit`; documentar diferenças para a liquidação normal; `ValidarRateio`, `ValidarDominio`, `ExecutarPersistir`.
- **Cria:** `analysis/E7-estorno.md`, `matriz-rtv.md#E7`, `matriz-rot.md#E7`.
- **Cobre:** RN-05-002, RN-05-009.
- **Branch:** `feat/mod-05-e7-analise` · **Commit:** `docs(mod-05): análise de LiquidacaoEstorno (E7-T01)` · **Depende de:** E6-T08

### E7-T02 · Entidades + mappings E7 · tipo: domain
- **Cobre:** E7-T01. **Branch:** `feat/mod-05-e7-entidades` · **Commit:** `feat(mod-05): entidades e mappings E7 (E7-T02)` · **Depende de:** E7-T01

### E7-T03 · DbSets + mapeamento E7 · tipo: dbcontext
- **Branch:** `feat/mod-05-e7-dbcontext` · **Commit:** `feat(mod-05): DbSets e mapeamento E7 (E7-T03)` · **Depende de:** E7-T02

### E7-T04 · `EstornarLiquidacaoHandler` (ROT) + RTV + paridade · tipo: operation
- **Objetivo:** inverso de `OP-Liquidar` — reabre parcela, ajusta saldo/período, valida domínio; testes integração + rollback + golden por `OP-xx#E7`/`VAL-xx#E7`/`CALC-xx#E7`.
- **Cobre:** todas as `OP-xx#E7`, `VAL-xx#E7`, `CALC-xx#E7`; RN-05-009.
- **Branch:** `feat/mod-05-e7-handler` · **Commit:** `feat(mod-05): EstornarLiquidacaoHandler + ROT/RTV (E7-T04)` · **Depende de:** E7-T03

### E7-T05 · Controller + `contracts/estorno.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e7-contrato` · **Commit:** `feat(mod-05): contrato, controller e migration E7 (E7-T05)` · **Depende de:** E7-T04

---

## Épico E8 — Reversão

### E8-T01 · Análise de `Reversao.cs` (1985) · tipo: analysis
- **Objetivo:** `ValidarOrigemGerador`, `GerarRateioItemFinanceiro`, `ValidarRateio`, `PersistirRateio`, `ExecutarPersistir`, `PersistirCorrecaoRateioReversao`.
- **Cria:** `analysis/E8-reversao.md`, `matriz-rtv.md#E8`, `matriz-rot.md#E8`.
- **Cobre:** RN-05-004, RN-05-010.
- **Branch:** `feat/mod-05-e8-analise` · **Commit:** `docs(mod-05): análise de Reversão (E8-T01)` · **Depende de:** E7-T05

### E8-T02 · Entidades + mappings + DbSets E8 · tipo: domain
- **Objetivo:** `Reversao`, `ReversaoDoctoParcela`, `ReversaoItemFinanceiro`, `AplicacaoItemFinReversao` + mappings + DbSets + teste de mapeamento.
- **Cobre:** E8-T01. **Branch:** `feat/mod-05-e8-entidades` · **Commit:** `feat(mod-05): entidades/mappings/DbSets E8 (E8-T02)` · **Depende de:** E8-T01

### E8-T03 · `ReverterHandler` (ROT) + RTV + paridade · tipo: operation
- **Objetivo:** lançamento espelho + correção de rateio; testes por `OP-xx#E8`/`VAL-xx#E8`/`CALC-xx#E8`.
- **Cobre:** todas as `OP-xx#E8`, `VAL-xx#E8`, `CALC-xx#E8`; RN-05-010.
- **Branch:** `feat/mod-05-e8-handler` · **Commit:** `feat(mod-05): ReverterHandler + ROT/RTV (E8-T03)` · **Depende de:** E8-T02

### E8-T04 · Controller + `contracts/reversao.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e8-contrato` · **Commit:** `feat(mod-05): contrato, controller e migration E8 (E8-T04)` · **Depende de:** E8-T03

---

## Épico E9 — Cheques

### E9-T01 · Análise (RTV/ROT + máquina de estados) · tipo: analysis
- **Objetivo:** `legacy-*-audit` em `ChequeRecebido` (1105), `ChequeRecebidoMovto` (1692), `ChequeEmitidoMovto` (1667), `TalaoCheque` (1103), `Cheque` (460), `SuprimentoCheque`, `MotivoDevolucaoCheque`, `ChequeEmitidoMovtoUpdate`. **Máquina de estados `SituacaoCheque`** (custódia/depósito/compensação/devolução por alínea/repasse/cancelamento). PK de `FINCHEQUERECEBIDOMOVTO` com **filial 1º**; `FINTALAOCHEQUE` PK 3 col.
- **Cria:** `analysis/E9-cheques.md`, `matriz-rtv.md#E9`, `matriz-rot.md#E9` (+ máquina de estados).
- **Cobre:** RN-05-011.
- **Branch:** `feat/mod-05-e9-analise` · **Commit:** `docs(mod-05): análise de Cheques + máquina de estados (E9-T01)` · **Depende de:** E8-T04

### E9-T02 · Entidades + mappings + DbSets E9 · tipo: domain
- **Objetivo:** entidades de `Domain/Cheques/` (`Cheque` como VO/owned), agregados `ChequeRecebido`→`ChequeRecebidoMovto[]`, `ChequeEmitidoMovto`, `TalaoCheque` (folhas), `MotivoDevolucaoCheque` (global) + mappings + DbSets + teste de mapeamento.
- **Cobre:** E9-T01; RN-05-011.
- **Branch:** `feat/mod-05-e9-entidades` · **Commit:** `feat(mod-05): entidades/mappings/DbSets E9 (E9-T02)` · **Depende de:** E9-T01

### E9-T03 · `MotivoDevolucaoCheque` + `TalaoCheque` (CRUD + RTV) · tipo: service
- **Cobre:** `VAL-xx#E9` de cadastro. **Branch:** `feat/mod-05-e9-cadastros` · **Commit:** `feat(mod-05): cadastros de cheque + RTV (E9-T03)` · **Depende de:** E9-T02

### E9-T04 · Handlers de movimento de cheque (ROT) · tipo: operation
- **Objetivo:** `MovimentarChequeRecebidoHandler` (custódia/depósito/devolução/repasse/baixa), `MovimentarChequeEmitidoHandler` (emissão/compensação/cancelamento), `SuprimentoChequeHandler`, `RenumeracaoChequeEmitidoHandler` — transição de estado + `MovtoFinanceiroCheque` + saldo. Testes integração + rollback por `OP-xx#E9`.
- **Cobre:** todas as `OP-xx#E9`, `VAL-xx#E9` de movimento; RN-05-011.
- **Branch:** `feat/mod-05-e9-handlers` · **Commit:** `feat(mod-05): handlers de movimento de cheque + ROT (E9-T04)` · **Depende de:** E9-T03

### E9-T05 · Controllers + `contracts/cheques.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e9-contrato` · **Commit:** `feat(mod-05): contrato, controllers e migration E9 (E9-T05)` · **Depende de:** E9-T04

### E9-T06 · Frontend Cheques (recebido/emitido/movimento/consulta) · tipo: frontend
- **Objetivo:** `src/pages/Financeiro/FChequeRecebido/`, `FChequeEmitido/`, `FMovimentoChequeRecebido/`, `FMovimentoChequeEmitido/`, `FChequeConsulta*/` + `schema.test.ts`. **Escopo CLR-07** (talão/suprimento/impressão fora).
- **Cobre:** `VAL-xx#E9` visuais.
- **Branch:** `feat/mod-05-e9-frontend` · **Commit:** `feat(mod-05): telas de Cheques React (E9-T06)` · **Depende de:** E9-T05

---

## Épico E10 — Adiantamentos e Acertos

### E10-T01 · Análise de `AdtoAcerto.cs` (1527) + `Adiantamento.cs` (749) · tipo: analysis
- **Objetivo:** `Adiantamento` = CRUD; `AdtoAcerto` = **operação** (CLR-12) — `ExecutarPersistir/Excluir`, `AtualizarRateio`, distribuição.
- **Cria:** `analysis/E10-adiantamento.md`, `matriz-rtv.md#E10`, `matriz-rot.md#E10`, `golden/CALC-E10-*.md`.
- **Cobre:** RN-05-012.
- **Branch:** `feat/mod-05-e10-analise` · **Commit:** `docs(mod-05): análise de Adiantamento/Acerto (E10-T01)` · **Depende de:** E9-T06

### E10-T02 · Entidades + mappings + DbSets E10 · tipo: domain
- **Objetivo:** `Adiantamento`, `AdtoAcerto`, `AdtoAcertoDistribuicao`(+Rateio), `AdtoAcertoMovto`, `AdtoLanctoEntidade`.
- **Cobre:** E10-T01. **Branch:** `feat/mod-05-e10-entidades` · **Commit:** `feat(mod-05): entidades/mappings/DbSets E10 (E10-T02)` · **Depende de:** E10-T01

### E10-T03 · `AdiantamentoService` (CRUD + RTV) · tipo: service
- **Cobre:** `VAL-xx#E10`. **Branch:** `feat/mod-05-e10-servico` · **Commit:** `feat(mod-05): serviço de Adiantamento + RTV (E10-T03)` · **Depende de:** E10-T02

### E10-T04 · `AcertarAdiantamentoHandler` (ROT) + paridade · tipo: operation
- **Objetivo:** distribui saldo, rateio, movimento, participa do período; testes integração + rollback + golden por `OP-xx#E10`/`CALC-xx#E10`.
- **Cobre:** todas as `OP-xx#E10`, `CALC-xx#E10`; RN-05-012.
- **Branch:** `feat/mod-05-e10-handler` · **Commit:** `feat(mod-05): AcertarAdiantamentoHandler + ROT (E10-T04)` · **Depende de:** E10-T03

### E10-T05 · Controllers + `contracts/adiantamentos.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e10-contrato` · **Commit:** `feat(mod-05): contrato, controllers e migration E10 (E10-T05)` · **Depende de:** E10-T04

---

## Épico E11 — DRE, Projeção e Seleção

### E11-T01 · Análise DRE + `SelecaoDocumento` (1226) + Projeção · tipo: analysis
- **Objetivo:** `DRE`/`DRETitulo`/`DRETituloClasse`/`DRETituloOperacao` (apuração), `SelecaoDocumento` (serviço de seleção em lote — CLR-06), `ProjecaoFluxoCaixa`(+Lacto). **Reconfirmar `FINPROJECAOFLUXOCAIXA*` no banco (R-1)** — se ausente, solicitar dump de produção.
- **Cria:** `analysis/E11-dre-projecao.md`, `matriz-rtv.md#E11`, `matriz-rot.md#E11`, `golden/CALC-E11-*.md`.
- **Cobre:** RN-05-014.
- **Branch:** `feat/mod-05-e11-analise` · **Commit:** `docs(mod-05): análise DRE/Projeção/Seleção (E11-T01)` · **Depende de:** E10-T05

### E11-T02 · Entidades + mappings + DbSets E11 · tipo: domain
- **Cobre:** E11-T01. **Branch:** `feat/mod-05-e11-entidades` · **Commit:** `feat(mod-05): entidades/mappings/DbSets E11 (E11-T02)` · **Depende de:** E11-T01

### E11-T03 · `SelecaoDocumentoService` (sem entidade) + RTV · tipo: service
- **Objetivo:** serviço de consulta/filtro exposto por interface; consumido por E6/E12/E14.
- **Cobre:** `VAL-xx#E11` de seleção. **Branch:** `feat/mod-05-e11-selecao` · **Commit:** `feat(mod-05): SelecaoDocumentoService (E11-T03)` · **Depende de:** E11-T02

### E11-T04 · Apuração de DRE + Projeção (serviço + paridade) · tipo: parity
- **Objetivo:** `DreApuracaoService` (soma classes/operações por título) + `ProjecaoFluxoCaixaService`; golden exatos. Impressão → **record de dados** (CLR-08).
- **Cobre:** `CALC-xx#E11`; RN-05-014.
- **Branch:** `feat/mod-05-e11-apuracao` · **Commit:** `feat(mod-05): apuração de DRE e projeção + paridade (E11-T04)` · **Depende de:** E11-T03

### E11-T05 · Controllers + `contracts/dre-projecao.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e11-contrato` · **Commit:** `feat(mod-05): contrato, controllers e migration E11 (E11-T05)` · **Depende de:** E11-T04

---

## Épico E12 — Programação de Cobrança e Transação entre Filiais

### E12-T01 · Análise · tipo: analysis
- **Objetivo:** `ProgramacaoCobranca`(+Parcela) (régua), `TransacaoFilial/{TransacaoFilial, TransacaoFinanceira, FilialMovimento}` (lançamentos casados, sequencial **global**).
- **Cria:** `analysis/E12-cobranca-transacao.md`, `matriz-rtv.md#E12`, `matriz-rot.md#E12`.
- **Cobre:** RN-05-017, RN-05-018.
- **Branch:** `feat/mod-05-e12-analise` · **Commit:** `docs(mod-05): análise E12 (E12-T01)` · **Depende de:** E11-T05

### E12-T02 · Entidades + mappings + DbSets E12 · tipo: domain
- **Cobre:** E12-T01. **Branch:** `feat/mod-05-e12-entidades` · **Commit:** `feat(mod-05): entidades/mappings/DbSets E12 (E12-T02)` · **Depende de:** E12-T01

### E12-T03 · Serviço de programação (CRUD + RTV) · tipo: service
- **Cobre:** `VAL-xx#E12`. **Branch:** `feat/mod-05-e12-servico` · **Commit:** `feat(mod-05): serviço de programação de cobrança + RTV (E12-T03)` · **Depende de:** E12-T02

### E12-T04 · `ExecutarTransacaoFilialHandler` (ROT) · tipo: operation
- **Objetivo:** lançamentos casados nas 2 filiais, sequencial global; teste integração + rollback por `OP-xx#E12`.
- **Cobre:** `OP-xx#E12`; RN-05-017.
- **Branch:** `feat/mod-05-e12-handler` · **Commit:** `feat(mod-05): ExecutarTransacaoFilialHandler + ROT (E12-T04)` · **Depende de:** E12-T03

### E12-T05 · Controllers + `contracts/cobranca-transacao-filial.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e12-contrato` · **Commit:** `feat(mod-05): contrato, controllers e migration E12 (E12-T05)` · **Depende de:** E12-T04

---

## Épico E13 — Consultas

### E13-T01 · Análise das ~30 views `Consultas/*` · tipo: analysis
- **Objetivo:** para cada `Consultas/*Consulta.cs` mapear a view `vw*` real (`fin_meta.txt`), colunas, filtros, e a qual endpoint `GET` / read-model pertence. Herança (`ChequeEmitidoConsultaBase`, `DocumentoParcConsulta`).
- **Cria:** `analysis/E13-consultas.md`, `contracts/consultas.md`.
- **Cobre:** RN-05-001 (consulta de parcela), suporte aos demais épicos.
- **Branch:** `feat/mod-05-e13-analise` · **Commit:** `docs(mod-05): análise das consultas (E13-T01)` · **Depende de:** E12-T05

### E13-T02 · Read-models + query-methods + endpoints `GET` · tipo: service
- **Objetivo:** `record`s de leitura em `Domain/Consultas/`; query-methods nos repositórios de leitura (`ReadContext`, `NoTracking`, paginação materializada); controllers `GET`.
- **Cobre:** `contracts/consultas.md`.
- **Constituição:** Artigo VII.4/VII.5.
- **Branch:** `feat/mod-05-e13-readmodels` · **Commit:** `feat(mod-05): read-models e endpoints de consulta (E13-T02)` · **Depende de:** E13-T01

### E13-T03 · Testes de consulta · tipo: service
- **Objetivo:** testes de integração das consultas paginadas (filtros, total, `NoTracking`).
- **Branch:** `feat/mod-05-e13-testes` · **Commit:** `test(mod-05): consultas paginadas (E13-T03)` · **Depende de:** E13-T02

---

## Épico E14 — Integração Bancária (fase dedicada final)

### E14-T01 · Análise + decisão de libs · tipo: analysis
- **Objetivo:** `legacy-*-audit` em `View/ArquivoRemessaView` (706), `View/ArquivoRetorno{Base (421), BaseView (313), 240View (270), 400View (227), ViewOutro (250)}`, `View/{AprovacaoDoctoPagarView (411), LiberacaoDoctoPagarView (528)}`, conciliação OFX. **Decisão final** `BoletoNetCore` vs. parser próprio (`research.md §1`); tabelas `FINCNAB`/`FINCNABDETALHE`.
- **Cria:** `analysis/E14-integracao-bancaria.md`, `matriz-rtv.md#E14`, `matriz-rot.md#E14`, `research.md` (atualiza §1 com a decisão).
- **Cobre:** RN-05-016, RN-05-015.
- **Branch:** `feat/mod-05-e14-analise` · **Commit:** `docs(mod-05): análise de integração bancária + decisão de libs (E14-T01)` · **Depende de:** E13-T03

### E14-T02 · Entidades CNAB + mappings + DbSets · tipo: domain
- **Objetivo:** `Cnab`, `CnabDetalhe` (armazenamento) + mappings + DbSets.
- **Cobre:** E14-T01. **Branch:** `feat/mod-05-e14-entidades` · **Commit:** `feat(mod-05): entidades CNAB (E14-T02)` · **Depende de:** E14-T01

### E14-T03 · Geração de remessa CNAB (240/400) + boleto · tipo: operation
- **Objetivo:** `GerarRemessaHandler` (seleciona parcelas via `SelecaoDocumentoService`, gera arquivo, grava `FinCnab`, atualiza nosso-número/remessa na parcela, incrementa `ContaBancaria.UltimoNossoNumero/UltimoNumeroArquivo`); linha digitável/código de barras (string). Golden: arquivo gerado = arquivo do legado byte a byte para os mesmos dados.
- **Cobre:** `OP-xx#E14` de remessa, `CALC-xx#E14` (nosso-número, dígitos); RN-05-016.
- **Branch:** `feat/mod-05-e14-remessa` · **Commit:** `feat(mod-05): geração de remessa CNAB e boleto (E14-T03)` · **Depende de:** E14-T02

### E14-T04 · Processamento de retorno CNAB · tipo: operation
- **Objetivo:** `ProcessarRetornoHandler` (lê 240/400, por ocorrência baixa/atualiza parcela, dispara `LiquidarParcelaHandler` quando liquidação). Golden: mesmo arquivo de retorno → mesmos efeitos que o legado.
- **Cobre:** `OP-xx#E14` de retorno; RN-05-016.
- **Branch:** `feat/mod-05-e14-retorno` · **Commit:** `feat(mod-05): processamento de retorno CNAB (E14-T04)` · **Depende de:** E14-T03

### E14-T05 · Conciliação OFX · tipo: operation
- **Objetivo:** `ConciliacaoBancariaService` (lê OFX, cruza extrato × `FinMovimento`, marca conciliado).
- **Cobre:** `OP-xx#E14` de conciliação; RN-05-005.
- **Branch:** `feat/mod-05-e14-ofx` · **Commit:** `feat(mod-05): conciliação bancária OFX (E14-T05)` · **Depende de:** E14-T04

### E14-T06 · Aprovação/liberação de contas a pagar · tipo: operation
- **Objetivo:** `AprovarDoctoPagarHandler`, `LiberarDoctoPagarHandler` (lote); RN-05-015.
- **Cobre:** `OP-xx#E14` de aprovação/liberação; RN-05-015.
- **Branch:** `feat/mod-05-e14-aprovacao` · **Commit:** `feat(mod-05): aprovação/liberação de contas a pagar (E14-T06)` · **Depende de:** E14-T05

### E14-T07 · Controllers + `contracts/integracao-bancaria.md` + migration · tipo: contract
- **Branch:** `feat/mod-05-e14-contrato` · **Commit:** `feat(mod-05): contrato, controllers e migration E14 (E14-T07)` · **Depende de:** E14-T06

---

## Fecho do módulo

### Z-T01 · Migration consolidada + DI final + registro de rotas · tipo: migration
- **Objetivo:** revisar todas as migrations, `AddGestaoFinanceira()` completo, rotas React registradas em `App.tsx` + menu.
- **Pronto quando:** `dotnet build`/`dotnet test` e `npm run build`/`npm test` **100% verdes** (Lei 13).
- **Branch:** `feat/mod-05-fecho` · **Commit:** `feat(mod-05): fecho do módulo — migrations, DI e rotas` · **Depende de:** E14-T07

### Z-T02 · `/analyze MOD-05` — gate de cobertura · tipo: analysis
- **Objetivo:** rodar `sdd-analyze`; veredito verde obrigatório (todas as `VAL-xx`/`OP-xx`/`CALC-xx` cobertas; V1–V7).
- **Cria/atualiza:** `analyze-report.md`.
- **Branch:** `docs/mod-05-analyze` · **Commit:** `docs(mod-05): relatório de análise de cobertura` · **Depende de:** Z-T01

### Z-T03 · `/handoff` — Log de Progresso · tipo: —
- Atualiza `specs/04-CONTRATO-DA-IA.md` §5 e `specs/00-INDICE-GERAL.md`.

---

## Resumo de cobertura (verificação `sdd-tasks §5`)

- **RN-05-001..020:** todas referenciadas em ≥1 tarefa (E1, E3–E14).
- **VAL-xx / OP-xx / CALC-xx:** criados nas tarefas `analysis` de cada épico (E?-T01) e
  consumidos pelas tarefas `service`/`operation`/`parity`/`frontend` do mesmo épico — o
  `sdd-analyze` (Z-T02) valida a ligação 1:1.
- **Épicos do `plan.md §3`:** E0, E1, E3, E2, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13,
  E14 — todos com tarefas. Ordem de execução conforme dependências declaradas.
- **Frontend (CLR-07):** E3, E4, E6, E9 (escopo núcleo) — demais sem tarefa `frontend`.
- **Pré-requisito externo:** MOD-02 rateio antes de E5-T02 (marcado na abertura do E5).

# Matriz RTV — Rastreabilidade Total de Validações · MOD-05 Gestão Financeira

> Gerada pela skill `legacy-validation-audit` dentro do `/implement` (tarefa `analysis` de
> cada épico). Uma linha `VAL-<épico>-<nn>` por validação/regra de negócio legada.
> Colunas: ID · Origem legada (arquivo:método) · Camada/Classe · Regra/Condição · Mensagem
> legada · Destino backend (`Result<T>`) · Destino frontend (Zod+MUI) · Cobre (RN / tarefa).
>
> `sdd-analyze` V3 exige que toda validação legada tenha linha aqui; V4 exige que toda linha
> `VAL-xx` apareça em ≥1 tarefa de `tasks.md`.

---

## `#E1` — Bases do módulo (E1-T01)

**Classes auditadas** (subindo a herança até `ObjetoNegocio.cs`):
`DocumentoFinanceiroBase` (1303) · `OperacaoDocumentoBase` (1098) · `ItemFinanceiroBase` (456) ·
`ParcelaGeral` (609, só dados — sem validação) · `ParcelaBase` (772) · `FormaMovInfo` (146) ·
`FechamentoCaixaBase` (448).

| ID | Origem legada | Camada / Classe | Regra / Condição | Mensagem legada | Destino backend | Destino frontend | Cobre |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **VAL-E1-01** | `DocumentoFinanceiroBase.cs:ValidarOperacao / IdOperacao.set` | Domínio / Base | A operação informada deve ser compatível com `PagarReceber` (Receber↔`OperacaoTipo.Receber`, Pagar↔`Pagar`, MovimentoCartao↔`MovimentoCartao`), via `AcessoGlobalFactory.ValidarTipoOperacao`. | *"A operação '{0}' informada não é válida para este documento."* | `PersistenciaDocumentoBase` valida `Result.Fail` (chama serviço de operação do AcessoGlobal por `int`) | N/A (só backend) | RN-05-001, RN-05-007 |
| **VAL-E1-02** | `DocumentoFinanceiroBase.cs:ValidarEntidade` | Domínio / Base | A entidade do documento deve existir. | *"Entidade informada não existe na API principal."* | `if (entidade is null) → Result.Fail` | N/A | RN-05-001 |
| **VAL-E1-03** | `DocumentoFinanceiroBase.cs:ValidarEntidade` | Domínio / Base | `PagarReceber == Pagar` → entidade deve ser **Fornecedor**. | *"Para documento do tipo '{tipo}', a entidade deve ser do tipo 'Fornecedor'."* | `if (!entidade.IsFornecedor) → Result.Fail` | Select filtra por papel | RN-05-001 |
| **VAL-E1-04** | `DocumentoFinanceiroBase.cs:ValidarEntidade` | Domínio / Base | `PagarReceber == Receber` → entidade deve ser **Cliente**. | *"Para documento do tipo '{tipo}', a entidade deve ser do tipo 'Cliente'."* | `if (!entidade.IsCliente) → Result.Fail` | Select filtra por papel | RN-05-001 |
| **VAL-E1-05** | `DocumentoFinanceiroBase.cs:ValidarEntidade` | Domínio / Base | `PagarReceber == MovimentoCartao` → entidade deve ser **Instituição financeira**. | *"Para documento do tipo '{tipo}', a entidade deve ser do tipo 'Instituição financeira'."* | `if (!entidade.IsInstituicaoFinanceira) → Result.Fail` | Select filtra por papel | RN-05-001 |
| **VAL-E1-06** | `DocumentoFinanceiroBase.cs:ValidarNumeroDocumento` | Domínio / Base + Banco | Só quando `IdProcessoOrigem == ProcessoOrigem.Documento`, não persistido, parâmetro `VerificarDocumentoExiste` ligado e `TipoDocumento` **não** sequencial: não pode existir outro documento **não cancelado** com o mesmo (`IdFilial`, `IdEntidade`, `IdTipoDocumento`, `NumeroDocumento`, `PagarReceber`). | *"O número do documento informado '({0})' já está cadastrado. Verifique."* | consulta de unicidade no `ReadContext` → `Result.Fail` | `zod.superRefine` (assíncrona opcional) | RN-05-001, RN-05-020 |
| **VAL-E1-07** | `DocumentoFinanceiroBase.cs:ValidarTipoDocumento` | Domínio / Base (herda regra do AcessoGlobal) | `TipoDocumento.ValidarTipoDocumento(id, PagarReceber)` — regra do cadastro de Tipo de Documento (MOD-02); aqui apenas propaga o erro e zera `IdTipoDocumento`. | (variável — vem do `TipoDocumento`) | delega ao serviço de `TipoDocumento` (AcessoGlobal) por `int`; propaga `Result.Fail` | Select filtra tipos válidos | RN-05-001 |
| **VAL-E1-08** | `OperacaoDocumentoBase.cs:GeraRateioSelecionado` | Domínio / Base de operação | Transferência entre filiais (`sd.IdFilialOrigem != IdFilial`) exige os parâmetros `ClasseTransferenciaFiliais` / `CentroCustoTransferenciaFiliais` / `ProjetosTranferenciaFiliais` configurados (conforme `UsaClasse`/`UsaCentroCusto`/`UsaProjeto`). | *"Deve ser configurado o(s) parâmetro(s) abaixo para transferência entre filiais: Parâmetro de classe. / centro de custo. / projeto."* | serviço de rateio de base → `Result.Fail` acumulando parâmetros ausentes | N/A | RN-05-004, RN-05-017 |
| **VAL-E1-09** | `OperacaoDocumentoBase.cs:CarregarItemFinanceiroRateio` | Domínio / Base de operação | Quando `UsaClasse`, todo item financeiro da seleção deve ter **classe** definida para a operação (`ItensOperacao` com `IdOperacao` correspondente). | *"Deve ser informado Classe para o item financeiro '{id} - {desc}' da filial ({filial}) [para operação '{op}']."* | serviço de rateio de base → `Result.Fail` | N/A | RN-05-004 |
| **VAL-E1-10** | `OperacaoDocumentoBase.cs:ValidarOperacao(int)` | Domínio / Base de operação | Idem VAL-E1-01, aplicada na cadeia de **operação de documento** (Liquidação/Reversão) via `AcessoGlobal.Operacao.ValidarTipoOperacao`. | (lançada pelo serviço de operação) | `OperacaoDocumentoBaseHandler` valida antes de abrir a transação → `Result.Fail` | N/A | RN-05-001, RN-05-007 |
| **VAL-E1-11** | `ItemFinanceiroBase.cs:ValidarItemFinanceiro` | Domínio / Base | Se o item financeiro tem operações vinculadas (`ItensOperacao.Count > 0`), a operação informada deve estar entre elas. | (`ItemFinanceiroOperacaoException(idItem, idOperacao)`) | `RateioServiceBase` → `Result.Fail(new ValidationError("IdOperacao", ...))` | N/A | RN-05-001, RN-05-020 |
| **VAL-E1-12** | `ItemFinanceiroBase.cs:Valor.set / (faixa)` (linhas 282, 320) | Domínio / Base | Valor / dias do item financeiro dentro da faixa permitida. | (`ValorInvalidoException(ValorForaFaixa)`) | guard no serviço → `Result.Fail` | `zod.number().min(...).max(...)` | RN-05-020 |
| **VAL-E1-13** | `ParcelaBase.cs:ValidarVencimento` | Domínio / Base (parcela) | Se a condição de pagamento é `FaixaDias` ou `Semanal`, o vencimento **não pode ser alterado**. | *(LanguageManager `pbl` item 1 — "condição de pagamento não pode ser alterada")* | `GeracaoParcelasService` → `Result.Fail` | campo desabilitado | RN-05-001, RN-05-020 |
| **VAL-E1-14** | `ParcelaBase.cs:ValidarVencimento` | Domínio / Base (parcela) | 1ª parcela com `PrimeiraParcelaAVista`: vencimento deve ser **igual à data de emissão**. | *(LanguageManager `pbl` item 1)* | `Result.Fail` | `zod.superRefine` | RN-05-001 |
| **VAL-E1-15** | `ParcelaBase.cs:ValidarVencimento` | Domínio / Base (parcela) | Alteração de vencimento não pode exceder `DiasLiberado` da parcela da condição. | *(LanguageManager `pbl` item 1)* | `Result.Fail` | `zod.superRefine` | RN-05-001 |
| **VAL-E1-16** | `ParcelaBase.cs:ValidarVenctoParcelaAlterada` | Domínio / Base (parcela) | Condição com `AlteraParcelas`: 1ª parcela → vencimento ≥ data de emissão; demais → vencimento > vencimento da parcela anterior. | *"…deve ser maior/igual a {data}."* (`pbl` itens 11/13) | `Result.Fail` | `zod.superRefine` (ordem crescente) | RN-05-001 |
| **VAL-E1-17** | `ParcelaBase.cs:ValidarNumeroParcela` | Domínio / Base (parcela) | Número da parcela > 0. | *"O número da parcela deve ser maior que zero (0)."* | `Result.Fail` | `zod.number().int().positive()` + `required` | RN-05-001 |
| **VAL-E1-18** | `ParcelaBase.cs:ValidarNumeroParcela` | Domínio / Base (parcela) | Condição com `!AlteraNroParcela` → número da parcela não pode ser alterado. | *(LanguageManager `pbl` item 2)* | `Result.Fail` | campo desabilitado | RN-05-001 |
| **VAL-E1-19** | `ParcelaBase.cs:ValidarNumeroParcela` | Domínio / Base (parcela) | `0 ≤ número < 1000`. | *(LanguageManager `pbl` item 14)* | `Result.Fail` | `zod.number().min(0).max(999)` | RN-05-001 |
| **VAL-E1-20** | `ParcelaBase.cs:ValidarNumeroParcela` | Domínio / Base (parcela) | Número deve ser maior que o da parcela anterior. | *"O número da parcela '{0}' informada, deve ser maior que o da parcela anterior ({1})."* | `Result.Fail` | `zod.superRefine` | RN-05-001 |
| **VAL-E1-21** | `ParcelaBase.cs:ValidarNumeroParcela` | Domínio / Base (parcela) | Número da parcela não pode repetir na lista. | *"Este número de parcela já foi informado."* | `Result.Fail` | `zod.superRefine` (unicidade) | RN-05-001 |
| **VAL-E1-22** | `ParcelaBase.cs:ValidarValorMinimo` | Domínio / Base (parcela) | Condição com `PercentualValorMinimo > 0` e `!AlteraParcelas`: valor da parcela ≥ valor mínimo calculado (ver CALC-E1-07). | *(LanguageManager `pbl` item 3, com o valor mínimo)* | `Result.Fail` | `zod.superRefine` | RN-05-001, RN-05-020 |
| **VAL-E1-23** | `ParcelaBase.cs:ValidarValor` | Domínio / Base (parcela) | Valor da parcela ≥ 0. | *"O valor da parcela deve ser maior que zero (0)."* | `Result.Fail` | `zod.number().min(0)` + `required` | RN-05-001 |
| **VAL-E1-24** | `ParcelaBase.cs:CalcularTotalParcelas` | Domínio / Base (parcela) | Valor de uma parcela não pode exceder o valor parcelado. | *(LanguageManager `pbl` item 4)* | `Result.Fail` | `zod.superRefine` | RN-05-001, RN-05-020 |
| **VAL-E1-25** | `ParcelaBase.cs:CalcularTotalParcelas` | Domínio / Base (parcela) | Soma das parcelas não pode exceder o valor parcelado. | *(LanguageManager `pbl` item 5)* | `Result.Fail` | `zod.superRefine` (soma) | RN-05-001, RN-05-020 |
| **VAL-E1-26** | `ParcelaBase.cs:CalcularTotalParcelas` | Domínio / Base (parcela) | Se a soma atingir o valor parcelado e ainda restarem parcelas, erro. | *(LanguageManager `pbl` item 6)* | `Result.Fail` | `zod.superRefine` | RN-05-001, RN-05-020 |
| **VAL-E1-27** | `ParcelaBase.cs:ValidarCaixaBanco` | Domínio / Base (parcela) | Se o Caixa/Banco da parcela é do tipo **Banco**, a `ContaBancaria` deve ser do tipo **Conta corrente**; senão zera `IdCaixaBancoParcela`. | *"Para a conta deve ser informado conta bancária do tipo 'Conta corrente'."* | `Result.Fail` (consulta E3) | Select filtra contas corrente | RN-05-006, RN-05-019 |
| **VAL-E1-28** | `ParcelaBase.cs:` (setter linha 609) | Domínio / Base (parcela) | Regra de propriedade de parcela (`ValidacaoValidar`) — a confirmar detalhe no `E1-T02` ao portar o setter. | *(a extrair do setter)* | `Result.Fail` | — | RN-05-001 |
| **VAL-E1-29** | `FechamentoCaixaBase.cs:Quantidade.set` | Domínio / Base (fechamento) | Só permite digitar **quantidade** se `PermiteDigitarQtde()` — não edita coluna de complemento **e** não é (PDV + `ChequeCliente`). | *(LanguageManager item 1)* | `FechamentoCaixaService` → `Result.Fail` | célula somente-leitura conforme regra | RN-05-013 |
| **VAL-E1-30** | `FechamentoCaixaBase.cs:ValorInformado.set` | Domínio / Base (fechamento) | Só permite digitar **valor** se `PermiteDigitarValor()` — não edita coluna **e** forma ≠ `ChequeEmpresa`. | *(LanguageManager item 1 — `DominioValidar`)* | `Result.Fail` | célula somente-leitura conforme regra | RN-05-013 |
| **VAL-E1-31** | `FormaMovInfo.cs:DefinirValor` | Domínio / Base (forma de movimento) | Se `LimitarValorDisponivel`: o valor informado é **limitado** a `ValorDisponivel` (retorna 0 se `ValorDisponivel ≤ 0`). Ajuste, não erro. | *(sem mensagem — ajusta o valor)* | `FormaMovInfoBase` (serviço) aplica o teto | `zod`+aviso de teto | RN-05-003, RN-05-005 |

> **VAL-xx#E1 = 31.** Cobertura por `[Fact]`:
> - **VAL-E1-29, -30, -31** — regras puras já concretas (`FechamentoCaixaService.PermiteDigitar*`,
>   `FormaMovInfoServiceBase.AplicarTetoValorDisponivel`): testadas em **`E1-T04`**
>   (`tests/.../E1/FechamentoCaixaRegrasTests.cs`, `FormaMovInfoTetoTests.cs`).
> - **VAL-E1-01..28** — regras em métodos `abstract` de `Application/Bases/`
>   (`PersistenciaDocumentoBase`, `GeracaoParcelasService`, `OperacaoDocumentoBaseHandler`,
>   `RateioServiceBase`). O `[Fact]` de cada uma acompanha a **implementação concreta**, na
>   tarefa `service` do épico dono: **E4-T05** (documento/parcela — VAL-E1-01..07, 12..26),
>   **E3-T05** (`ValidarCaixaBanco` — VAL-E1-27, depende de `CaixaBanco`/`ContaBancaria`),
>   **E5/E6** (operação e rateio — VAL-E1-08..11), **E1-T02** (setter de `ParcelaBase` — VAL-E1-28).
>   `E1-T04` deixa a assinatura/mensagem esperada de cada uma documentada na matriz; o gate
>   completo (`Z-T02`) confere que nenhuma ficou sem `[Fact]`.
>
> **CALC-xx#E1** (ver `matriz-rot.md#E1`): golden tests em **`E1-T04`**
> (`tests/.../E1/CalculoItemFinanceiroParityTests.cs`, `GeracaoParcelasCalcTests.cs`,
> `ArredondamentoFinanceiroTests.cs` + `golden/CALC-E1-*.csv`).

---

## `#E3` — Caixa e Banco (E3-T01)

**Classes auditadas:** `CaixaBanco` (869) · `ContaBancaria` (1704) · `CaixaBancoUsuario` (350) ·
`SaldoCaixaBanco` (322) · `SaldoRateio` (382) · `Cobrador` (367) · `IndiceConversor` (222,
**sem tabela** — vira serviço). Colunas confirmadas contra `legacy-schema/fin_columns.txt`.
**~40 colunas de integração de `FINCONTABANCARIA` marcadas `[E14]`** (CLR-03).

| ID | Origem legada | Camada / Classe | Regra / Condição | Mensagem legada | Destino backend | Destino frontend | Cobre |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **VAL-E3-01** | `CaixaBanco.cs:ValidarUsuario` | Domínio | Se parâmetro `VinculaCaixaBancoUsuario` ligado: não pode haver usuário repetido na lista `Usuarios` do caixa. | *(LanguageManager item 1)* | `CaixaBancoService` → `Result.Fail` | `zod.superRefine` (unicidade na grade) | RN-05-019 |
| **VAL-E3-02** | `CaixaBanco.cs:ValidarContaBancaria` | Domínio | Conta Banco com `EnviarSped` → instituição financeira fiscal obrigatória. | *"Para conta bancária, a instituição financeira para SPED, deve ser informado a instituição financeira fiscal."* | `Result.Fail` | `required` condicional | RN-05-006 |
| **VAL-E3-03** | `CaixaBanco.cs:ValidarContaBancaria` | Domínio | `EnviarSped` **e** `ContaTerceiro` não podem estar ambos marcados. | *"...ao MARCAR 'Enviar SPED (Bloco 1601)', deve ser DESMARCADO 'Conta de terceiro'."* | `Result.Fail` | `zod.superRefine` | RN-05-006 |
| **VAL-E3-04** | `CaixaBanco.cs:ValidarContaBancaria` | Domínio (cross MOD-02) | Instituição financeira do SPED deve ser Pessoa **Jurídica**. | *"...deve ser do tipo 'Jurídica'."* | `Result.Fail` (consulta AcessoGlobal por `int`) | N/A | RN-05-006 |
| **VAL-E3-05** | `CaixaBanco.cs:ValidarContaBancaria` | Domínio (cross MOD-02) | Instituição financeira do SPED deve ter **CNPJ** preenchido. | *"...deve ser preenchido o CNPJ."* | `Result.Fail` | N/A | RN-05-006 |
| **VAL-E3-06** | `CaixaBanco.cs:ValidarContaBancaria` | Domínio | `ContaBancariaTipo == Investimento` → `IdContaBancariaVinculada` obrigatória. | *"Para conta bancária do tipo 'Investimento', deve ser informada a conta vinculada."* | `Result.Fail` | `required` condicional | RN-05-006 |
| **VAL-E3-07** | `CaixaBanco.cs:ValidarContaBancaria` | Domínio | Conta vinculada de investimento deve ser **diferente** do próprio `IdCaixaBanco`. | *"...a conta vinculada deve ser DIFERENTE do ID da conta bancária."* | `Result.Fail` | `zod.superRefine` | RN-05-006 |
| **VAL-E3-08** | `CaixaBanco.cs:ValidarPeriodoCaixa` | Domínio / Estado (OnBeforeExecutarPersistir) | Para **inativar** um caixa (`TipoConta == Caixa`, persistido, `Ativo == false`, era ativo): o domínio-período pertencente deve estar **fechado** (`DataFechamento > MinValue`). | *"Para inativar este caixa deve ser fechado seu domínio período pertencente ao domínio '{0}'."* | `CaixaBancoService` consulta E2 → `Result.Fail` | N/A | RN-05-013, RN-05-019 |
| **VAL-E3-09** | `CaixaBanco.cs:ValidarControleCaixaBanco` (static) | Domínio / Parâmetro | `VinculaCaixaBancoUsuario` e `TrabalhaComDominio` são **mutuamente exclusivos**. | *"Os parâmetros 'Vincular caixa/banco por usuário' e 'Trabalha com domínio financeiro' somente um deles pode estar marcado."* | serviço de parâmetro → `Result.Fail` | N/A | RN-05-013, RN-05-019 |
| **VAL-E3-10** | `CaixaBanco.cs:ValidarCaixaPeriodo` | Domínio / Consulta | Com `TrabalhaComDominio`: caixa=Banco exige domínio do usuário com `MovimentoBanco`; caixa vinculado a domínio só é movimentável por usuário que trabalha com domínio. | *"O usuário logado não possui um domínio financeiro cadastrado." / "...configurado para não efetuar movimento de banco." / "O caixa informado '({0})' está vinculado a um domínio..."* | `CaixaBancoService.ValidarCaixaPeriodoAsync` → `Result.Fail` | N/A | RN-05-013, RN-05-019 |
| **VAL-E3-11** | `CaixaBancoUsuario.cs:ValidarUsuarioUnico` | Domínio | Usuário não pode repetir na lista de usuários do caixa. | *"Este usuário já foi informado."* | `Result.Fail` | `zod.superRefine` | RN-05-019 |
| **VAL-E3-12** | `CaixaBancoUsuario.cs:IdUsuario.set` | Domínio / Estado | Não é permitido alterar o usuário depois de salvo. | *"NÃO é permitido alterar usuário após ter sido salvo."* | `Result.Fail` | campo desabilitado em edição | RN-05-019 |
| **VAL-E3-13** `[E14]` | `ContaBancaria.cs:Validate` | Domínio | `!geraBoleto && (geraRemessa \|\| processaRetorno)` → erro. | *(LanguageManager item 2)* | **E14** | **E14** | RN-05-016 |
| **VAL-E3-14** `[E14]` | `ContaBancaria.cs:Validate` | Domínio | `geraBoleto` + `BoletoBeneficiarioDiferente` → entidade beneficiária obrigatória. | *"Deve ser informado na conta bancária o beneficiário do boleto."* | **E14** | **E14** | RN-05-016 |
| **VAL-E3-15** `[E14]` | `ContaBancaria.cs:Validate` | Domínio | `geraBoleto` + `BoletoSacadoAvalista` → entidade sacado/avalista obrigatória. | *"Deve ser informado na conta bancária o sacado/avalista do boleto."* | **E14** | **E14** | RN-05-016 |
| **VAL-E3-16** `[E14]` | `ContaBancaria.cs:Validate` | Domínio | `processaRetorno && !geraRemessa` → erro. | *(LanguageManager item 3)* | **E14** | **E14** | RN-05-016 |
| **VAL-E3-17** | `ContaBancaria.cs:Validate` | Domínio | `ContaTerceiro` → `Titular` **e** `CpfCnpj` obrigatórios. | *(LanguageManager item 4)* | `Result.Fail` (núcleo — `ContaTerceiro`/`Titular` são E3; `CpfCnpj` da conta fica no bloco E14, mas a regra é núcleo) | `required` condicional | RN-05-006 |
| **VAL-E3-18** `[E14]` | `ContaBancaria.cs:ValidaDiasProtesto` | Domínio | `DiasParaProtesto` entre 5 e 55 (quando `> 0`; máx. 55 só se banco usa negativação). | *"...deve ser informado no MÍNIMO 5 (cinco) dias." / "...no MÁXIMO 55 (cinquenta e cinco) dias."* | **E14** | **E14** | RN-05-016 |
| **VAL-E3-19** `[E14]` | `ContaBancaria.cs:CpfCnpj.set` (linha 877) | Domínio | Dígito verificador de CPF/CNPJ da conta bancária. | *(`ValorInvalidoException.CnpjCpfInvalido`)* | **E14** | **E14** | RN-05-016 |
| **VAL-E3-20** | `IndiceConversor.cs:ConverterIndice` (198/201) + `RetornarIndice` (94) | Serviço | Índice de origem/destino não nulos; e deve existir valor do índice para a data (lista não vazia e valor ≠ 0). | *"IndiceOrigem/IndiceDestino" (`ValorNulo`)* · *(`DataSemIndiceEconomico`, sigla + data)* | `ConversorIndiceService` — guardas + `Result.Fail` (`DataSemIndiceEconomico`); ver CALC-E3-05..07 | N/A | RN-05-008 |
| **VAL-E3-21** `[E14]` | `ContaBancaria.cs:GeraBoleto.set` / `GeraRemessa.set` / `ProcessaRetorno.set` (1058/1080/1102) | Domínio | Ao ligar boleto / remessa / retorno, a **agência** deve estar informada. | *"Para gerar boleto/remessa / processar arquivo de retorno, deve ser informado a agência."* | **E14** | **E14** | RN-05-016 |

> **VAL-xx#E3 = 21** (6 marcadas `[E14]`, tratadas no épico E14 — CLR-03). Cobertura por
> `[Fact]`: **E3-T05** (`service`) — 1 por `VAL-E3-01..12, 17, 20`; **E14-T0x** —
> `VAL-E3-13..16, 18, 19, 21`.

# Matriz ROT — Rastreabilidade de Operações e Transações · MOD-05 Gestão Financeira

> Gerada pela skill `legacy-operation-audit` dentro do `/implement` (tarefa `analysis` de
> cada épico). Uma linha `OP-<épico>-<nn>` por operação/transação de negócio legada
> (persistência orquestrada, máquina de estados, efeitos colaterais, ordem de gravação,
> rollback). `CALC-<épico>-<nn>` para as fórmulas que precisam de golden test de paridade
> (`legacy-calc-parity`).
>
> `sdd-analyze` V3 exige que toda operação legada tenha linha aqui; V4 exige que toda
> `OP-xx`/`CALC-xx` apareça em ≥1 tarefa de `tasks.md` (e `CALC-xx` numa tarefa `parity`).

---

## `#E1` — Bases do módulo (E1-T01)

### Operações / persistência orquestrada

| ID | Origem legada | Tipo | Ordem de persistência / efeitos | Rollback | Destino no novo sistema | Cobre |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **OP-E1-01** | `ObjetoNegocio.cs:ObjectPersist.ExecutarPersistir` (herdado por todas as bases) | Persistência-base (1 transação) | `Validate()` → `OnBeforeExecutarPersistir` (em `DocumentoFinanceiroBase`: seta `dataInclusao`/`horaInclusao` se novo) → **gerar sequencial** se novo (`GeradorSequencialService` — RN-05-006) → `OnAfterGeneratedId` → gravar o registro → `OnAfterExecutarPersistir` → `PersistirRateio` (se `IDadosRateio`) → `PersistirCampoEspecifico` → `PersistirPeriodoFormaPagto` (se `IDadosPeriodoFormaPagto`) → registro de sincronização → invalidar cache. | Transação legada única (`TransacaoBase`); erro → rollback total. | `PersistenciaDocumentoBase` / `OperacaoDocumentoBaseHandler` abstratos em `Application/Bases/` — 1 `IDbContextTransaction` por Handler (Artigo VII). Sincronização e cache **fora de escopo**. | RN-05-001, RN-05-004, RN-05-006, RN-05-008 |
| **OP-E1-02** | `ObjetoNegocio.cs:ExecutarExcluir` | Exclusão-base (1 transação) | `ExcluirRateio` (se `IDadosRateio`) → `ExcluirCampoEspecifico` → registro de sincronização (Exclusão) → remover o registro → invalidar cache. | Transação única; erro → rollback. | `PersistenciaDocumentoBase.ExcluirAsync` abstrato; ordem preservada. | RN-05-001, RN-05-004 |
| **OP-E1-03** | `ObjetoNegocio.cs:ValidarRateio` + `RateioMovto.Validar` | Validação de rateio (pré-persistência) | Se `IDadosRateio`: soma do rateio por dimensão (classe / centro de custo / projeto) deve igualar o valor rateável — resultado agregado em `ValidationRateioContainer`. | — (falha impede o commit) | `RateioServiceBase.ValidarAsync` → `ValidationRateioContainer` (E0-T03). | RN-05-002, RN-05-004 |
| **OP-E1-04** | `ObjetoNegocio.cs:ObjectGenerated.ValidarOrigemGerador` | Validação de origem (pré-persistência/exclusão) | Objetos gerados por processo externo (Documento por Faturamento/Compra/Contrato/OS/…) validam a origem (`IdOrigem` + `IdProcessoOrigem`) antes de persistir/excluir. | — | `OperacaoDocumentoBaseHandler` chama `ValidarOrigemGeradorAsync` (virtual); implementação concreta por épico. | RN-05-007 |
| **OP-E1-05** | `OperacaoDocumentoBase.cs:AtualizarRateio` | Recálculo de rateio (in-memory) | `RateioMovto.Limpar()` → se `usaClasse\|usaCentroCusto\|usaProjeto`: `GeraRateioSelecionado` (rateio por documento selecionado via `FinanceiroUtil.GeraRateioLiquidacao` + rateio de transferência entre filiais quando `IdFilialOrigem != IdFilial`) → `AposGeraRateioSelecionado` → para cada `ItemFinanceiroRateio` acumulado: `AcumuladorAdd(Classe, …)` (+ centro de custo/projeto via `UpdateCentroCustoProjetoItemFinanceiro`) → `AcumuladorAplicar()`. Natureza devedora inverte o sinal. | — (recálculo total; sem persistência) | `RateioServiceBase.AtualizarRateio(RateioContainer)` — usa `RateioContainer` (E0-T03) + porta para o motor de rateio do MOD-02 (CLR-01, consumido em E5). | RN-05-002, RN-05-004 |
| **OP-E1-06** | `OperacaoDocumentoBase.cs:GerarRateioItemFinanceiro` (virtual) | Gancho (subclasse decide) | Retorna se o rateio deve ser gerado a partir dos itens financeiros da seleção — implementado por `Liquidacao`/`Reversao` (E6/E8). | — | método abstrato em `OperacaoDocumentoBaseHandler`. | RN-05-004 |
| **OP-E1-07** | `DocumentoFinanceiroBase.cs:AplicarOperacao` | Aplicação de operação ao documento | Seta histórico padrão (`or.HistoricoPadrao.TextoPadrao`), `IdFormaCobranca` (parâmetro `FormaCobrancaGeracaoDoctoMovCartao` se `MovimentoCartao`), `IdTipoDocumento` da operação; `RateioMovto.Limpar()`. | **Rollback local:** em erro zera `IdOperacao`/`Historico`/`IdTipoDocumento` e re-lança. | `PersistenciaDocumentoBase.AplicarOperacaoAsync` — mesma ordem; o "rollback local" vira `Result.Fail` sem mutação parcial. | RN-05-001, RN-05-007 |
| **OP-E1-08** | `DocumentoFinanceiroBase.cs:AplicarCondicaoPagamento` | Geração de parcelas pela condição | `recalcularValorConvertido = true` → `CalcularValorConvertido()` (CALC-E1-09 — conversão por índice) → se `DataEmissao > MinValue`: `SetParcelasCondicaoPagto(refazer)` (gera/refaz as parcelas conforme a condição de pagamento — `CondicaoPagtoTipo` Parcelada/FaixaDias/Semanal). | — (em memória; commit só no persist) | `GeracaoParcelasService` (`Application/Bases/`) — consome `CondicaoPagamento` do AcessoGlobal por `int`. | RN-05-001, RN-05-008, RN-05-020 |
| **OP-E1-09** | `DocumentoFinanceiroBase.cs:CalcularValorConvertido` | Conversão de valor por índice | Se `recalcularValorConvertido` e `IdIndiceEconomico != 0`: `IndiceConversor.ConverterIndice(IndiceEconomico, IndiceConversao, Valor, DataEmissao)` → `valorConvertido`; limpa a flag. | erro → propaga. | `ConversorIndiceService` (base) — golden test em CALC-E1-09. | RN-05-008 |
| **OP-E1-10** | `ParcelaBase.cs:RecalcularParcelas / RecalcularParcelasRestante / CorrigirDiferenca` | Rebalanceamento de parcelas | Ao alterar valor/vencimento de uma parcela: recalcula as demais; a diferença de arredondamento vai para a **1ª ou a última** parcela conforme `cp.IdParcelaArredondamento` (`ParcelamentoArredondamento`). | — (em memória) | `GeracaoParcelasService.Rebalancear` — determinístico; golden em CALC-E1-07/08. | RN-05-001, RN-05-020 |
| **OP-E1-11** | `FechamentoCaixaBase.cs:CalcularQtdeValorEditado / SetQtdeValor` | Contagem/edição de fechamento de caixa | Acumula quantidade × valor por forma de pagamento no fechamento (contagem de dinheiro por moeda, cheques recebidos); respeita `PermiteDigitarQtde`/`PermiteDigitarValor`/`PermiteEditarColuna`. | — (em memória) | `FechamentoCaixaService` (`Application/Bases/`) — a máquina de estados do período fica no E2. | RN-05-013 |
| **OP-E1-12** | `DocumentoFinanceiroBase.cs:` ganchos `virtual` vazios — `RecalcularTributoRateio` (169), `SetParcelasCondicaoPagto` (148), `CarregarComissionadoPadrao` (176), `CarregaItemFinanceiroOperacao` (183), `RetornarValorRateio`/`GetValorValidacaoRateio`/`GetValorDigitadoRateio` | Ganchos de extensão (base não faz nada) | A base só declara os pontos de extensão; o comportamento concreto (rateio de tributos, geração de parcelas pela condição, comissionado padrão, item financeiro da operação) é das subclasses. | — | métodos `abstract`/`virtual` em `PersistenciaDocumentoBase`; implementação em **E4** (`Documento` / `DocumentoParcela`). | RN-05-001, RN-05-004, RN-05-015 |
| **OP-E1-13** | `OperacaoDocumentoBase.cs:AplicarOperacao()` (189), `CarregarDefault` (166), `CarregarPortadorDefault` (214), `CarregarOperacoesDefault` (560) | Carga de defaults da operação | Ao definir a ação (Pagar/Receber/MovimentoCartao) ou a operação, a base carrega operação/portador/históricos padrão e limpa os dados dependentes (`LimparDados`). `SetPagarReceber` faz rollback do tipo anterior em erro. | rollback local do `PagarReceber` anterior | `OperacaoDocumentoBaseHandler.CarregarDefaultsAsync` (virtual); implementação por E6/E8. | RN-05-001, RN-05-007 |

### Fórmulas — golden tests de paridade (`CALC-xx#E1`, tarefa `E1-T04` / `legacy-calc-parity`)

| ID | Origem legada | Fórmula (transcrever **sem refatorar** — Regra 5) | Cobre |
| :--- | :--- | :--- | :--- |
| **CALC-E1-01** | `ItemFinanceiroBase.cs:ExecutarCalculo` | Por `CalculoItemFinanceiro`: `Somar`/`Subtrair` → `Arredondar((valorBase * Valor) / 100, 2)`; `MultiplicarSomar`/`MultiplicarDiminuir` → `valorBase * Abs(Valor)`; `DividirSomar`/`DividirSubtrair` → `valorBase / Abs(Valor)`; `PercentualSomar`/`PercentualSubtrair` → `valorBase * (Abs(Valor) / 100)`; default `0`. | RN-05-020 |
| **CALC-E1-02** | `ItemFinanceiroBase.cs:CalcularComposto` | Blocos de 30 dias: `while d>30 { vc = ExecutarCalculo(vb); total += vc; vb += vc; d -= 30 }`; resto `d>0`: `total += (ExecutarCalculo(vb) / 30) * d`. | RN-05-020 |
| **CALC-E1-03** | `ItemFinanceiroBase.cs:CalcularDias` | Por `ItemFinanceiroAplicar`: `AntesVencimento` → `(dataInicial - dataFinal).Days`; `DepoisVencimento` → `(dataFinal - dataInicial).Days`; `NaoAplicar` → `0`. | RN-05-020 |
| **CALC-E1-04** | `ItemFinanceiroBase.cs:ConsiderarDiasParaCalculo` (2 sobrecargas) | Se `!item.Composto && item.ConsideraDiasCalculo`: `valorCalculo / 30` (sem dias) ou `valorCalculo / 30 * dias`. | RN-05-020 |
| **CALC-E1-05** | `ItemFinanceiroBase.cs:RetornarIndiceConvertido` | Não converte se cálculo percentual ou `IdIndiceEconomico` nulo/igual ao default; senão `IndiceConversor.ConverterIndice(item.IndiceEconomico, default, valor, dataIndice)`. `DataSemIndiceEconomico` → `valor = 0`. | RN-05-008, RN-05-020 |
| **CALC-E1-06** | `ItemFinanceiroBase.cs:DefinirValor` | `if item.Desconto: valorCalculo = -valorCalculo` → `if item.Composto && dias>0: CalcularComposto` → `ConsiderarDiasParaCalculo(…, dias)` → `RetornarIndiceConvertido`. | RN-05-020 |
| **CALC-E1-07** | `ParcelaBase.cs:CalcularValorMinimo` | `valorParcela = valorParcelamento * PercentualDivisao / 100`; retorno `Arredondar(valorParcela * PercentualValorMinimo / 100, 2)`. | RN-05-020 |
| **CALC-E1-08** | `ParcelaBase.cs:CalcularPercentualNovo` | `total = GetValorParcelamento()`; `total == 0 → 0`; senão `Arredondar(valorParcela * 100 / total, 2)`. | RN-05-020 |
| **CALC-E1-09** | `DocumentoFinanceiroBase.cs:CalcularValorConvertido` | `valor` × índice via `IndiceConversor.ConverterIndice(IndiceEconomico, IndiceConversao, Valor, DataEmissao)` → `valorConvertido`. | RN-05-008 |
| **CALC-E1-10** | `ItemFinanceiroBase.cs:CalcularItem` / `AplicarItemFinanceiro` / `GetValorItemFinanceiroBoleto` | Entradas públicas que compõem as fórmulas acima: `CalcularItem` = `DefinirValor(valorBase, ExecutarCalculo(valorBase, item.IdCalculo), dias, dataIndice, item)`; `AplicarItemFinanceiro(dataInicial[, dataFinal])` = `CalcularDias` → `CalcularItem`; `GetValorItemFinanceiroBoleto` idem para desconto/juros/multa de boleto. | RN-05-020 |

> **OP-xx#E1 = 13 · CALC-xx#E1 = 10.** `OP` cobertas por `E1-T03` (serviços/handlers de base) e
> `E1-T04` (testes de integração + rollback). `CALC` cobertas por `E1-T04` (`parity`, igualdade
> exata de `decimal`, origem conforme `research.md §5`). Gate: `/analyze MOD-05 --epico E1`.

---

## `#E3` — Caixa e Banco (E3-T01)

### Operações / persistência orquestrada

| ID | Origem legada | Tipo | Ordem de persistência / efeitos | Rollback | Destino no novo sistema | Cobre |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **OP-E3-01** | `CaixaBanco.cs:ExecutarPersistir` | Persistência (1 transação) | `base.ExecutarPersistir` (Validate → sequencial se novo → gravar `FINCAIXABANCO`) → `PersistirContaBancaria`: se `TipoConta == Caixa` **remove** a `ContaBancaria`; se `Banco` **persiste** a `ContaBancaria` com `IdCaixaBanco`/`IdFilial` do caixa. Auditoria em `OnBeforeExecutarPersistir` (OP-E3-03). Lista de `Usuarios` (`FINCAIXABANCOUSUARIO`) gravada como agregado. | 1 transação; erro → rollback total. | `CaixaBancoService` + `GestaoFinanceiraRepositorioBase`; 1 `IDbContextTransaction` (Artigo VII). PK composta `(IdCaixaBanco, IdFilial)` — `.ValueGeneratedNever()`. | RN-05-006, RN-05-019 |
| **OP-E3-02** | `CaixaBanco.cs:ExecutarExcluir` | Exclusão (1 transação) | Remove `ContaBancaria` (se existir) → `base.ExecutarExcluir` (remove `FINCAIXABANCO` + usuários). | 1 transação; erro → rollback. | `CaixaBancoService.ExcluirAsync`. | RN-05-006, RN-05-019 |
| **OP-E3-03** | `CaixaBanco.cs:OnBeforeExecutarPersistir` | Gancho pré-persistência | `ValidarPeriodoCaixa` (VAL-E3-08) → seta usuário/data/hora de inclusão (novo) ou alteração. | — | ganchos do `CaixaBancoService`; auditoria via `IContextoExecucao`. | RN-05-019 |
| **OP-E3-04** | `CaixaBanco.cs:PersistirContaBancaria` / `RemoverContaBancaria` | Máquina de estados (tipo de conta) | Alterar `TipoConta` de `Banco` → `Caixa` **remove** a `FINCONTABANCARIA` associada; `Caixa` → `Banco` cria/persiste a `FINCONTABANCARIA`. | dentro da transação do persist. | lógica no `CaixaBancoService` — 1:1 `FINCAIXABANCO`↔`FINCONTABANCARIA` (`HasOne...WithOne`). | RN-05-006 |
| **OP-E3-05** | `ContaBancaria.cs:ExecutarPersistir` | Persistência da conta bancária | `base.ExecutarPersistir` (grava `FINCONTABANCARIA` núcleo) → `UpdateDadosContaVinculada` (propaga dados para contas que a referenciam) → `AdicionarSequencial(remessa=false)` + `AdicionarSequencial(remessa=true)` **`[E14]`** (sequenciais de arquivo). | 1 transação. | `ContaBancariaService` (núcleo E3). Sequenciais de arquivo → **E14**. | RN-05-006 |
| **OP-E3-06** | `ContaBancaria.cs:ExecutarExcluir` | Exclusão da conta bancária | `RemoverSequencial(false)` + `RemoverSequencial(true)` **`[E14]`** → `base.ExecutarExcluir`. | 1 transação. | idem; remoção dos sequenciais → **E14**. | RN-05-006 |
| **OP-E3-07** | `CaixaBanco.cs:Usuarios` (agregado) / `CaixaBancoUsuario` | Persistência em lote | Ao salvar o caixa, a coleção de usuários é sincronizada (inserir novos, remover ausentes) — respeitando VAL-E3-11 (unicidade) e VAL-E3-12 (não altera usuário salvo). | dentro da transação do caixa. | `CaixaBancoService` — `List<CaixaBancoUsuario>` privada + `IReadOnlyList<>` (Artigo V). PK `(IdCaixaBanco, IdFilial, IdUsuario)`. | RN-05-019 |
| **OP-E3-08** | `SaldoCaixaBanco.cs:RetornarSaldoCaixaBanco` / `RetornarCaixaBanco` (static) | Consulta de saldo por data/tipo | Busca 1 registro de `FINSALDOCAIXABANCO` por `(IdFilial, IdCaixaBanco)` filtrando `DataSaldo`: `Inicial` → `< dataSaldo`; `Atual`/`Final` → `<= dataSaldo`; ordena `DataSaldo` desc, top 1. Retorna 0 se não achar ou (`Inicial` + data vazia). | — (leitura) | `SaldoCaixaBancoService` no `ReadContext`; paginação/top-1 materializados (Artigo VII.5). | RN-05-005 |

### Fórmulas — golden tests de paridade (`CALC-xx#E3`, tarefa `E3-T05` / `legacy-calc-parity`)

| ID | Origem legada | Fórmula (transcrever sem refatorar — Regra 5) | Cobre |
| :--- | :--- | :--- | :--- |
| **CALC-E3-01** | `SaldoCaixaBanco.cs:Saldo` | `Arredondar( saldoAnterior + (totalCredito − totalDebito) , 2)`. | RN-05-005 |
| **CALC-E3-02** | `SaldoCaixaBanco.cs:SaldoConciliado` | `Arredondar( saldoAnteriorConciliado + (totalCreditoConciliado − totalDebitoConciliado) , 2)`. | RN-05-005 |
| **CALC-E3-03** | `SaldoCaixaBanco.cs:RetornarSaldoCaixaBanco` | Seleciona o saldo por `(dataSaldo, TipoSaldo)` (ver OP-E3-08) e devolve `Arredondar(Saldo, 2)` ou `Arredondar(SaldoConciliado, 2)` conforme `saldoConciliado`; `0` quando `Inicial` + `dataSaldo == MinValue` ou sem registro. | RN-05-005 |
| **CALC-E3-04** | `SaldoRateio.cs:SaldoEconomico` / `SaldoFinanceiro` | `Arredondar( SaldoAnterior{Economico\|Financeiro} + (TotalCredito{...} − TotalDebito{...}) , 8)` — **8 casas** (`numeric(23,8)`). | RN-05-004, RN-05-005 |
| **CALC-E3-05** | `IndiceConversor.cs:CalcularConversao` | Mesmo índice → valor inalterado. Ambos ≠ padrão → `v1 = Arredondar(valor / valorIndiceOrigem, dec)`; `Arredondar(v1 * valorIndiceDestino, dec)`. Só destino ≠ padrão → `Arredondar(valor / valorIndiceDestino, dec)`. Senão → `Arredondar(valor * valorIndiceOrigem, dec)`. `dec` = 2 por padrão. | RN-05-008 |
| **CALC-E3-06** | `IndiceConversor.cs:RetornarDataIndiceValida` | Se `d` é dia útil → `d`; senão avança (`+1`) ou retrocede (`-1` quando `IndiceModoCorrecao == UsarDataAnterior`) até achar dia útil (`Feriado.DiaUtil`). | RN-05-008 |
| **CALC-E3-07** | `IndiceConversor.cs:RetornarIndice` | Índice padrão → `1.00`. Senão carrega a lista de valores do índice para o ano; posição = `mês − 1` (`Mensal`) ou `dia − 1` (`Diário`); se lista vazia ou valor `0` → `ObjetoNegocioException(DataSemIndiceEconomico)`. | RN-05-008 |

> **OP-xx#E3 = 8 · CALC-xx#E3 = 7.** `OP` cobertas por `E3-T04` (DbSets/mapeamento) e `E3-T05`
> (serviços + testes de integração). `CALC` cobertas por **`E3-T09`** (`parity` — saldos +
> conversão por índice; o `IndiceConversor` vira `ConversorIndiceService` e realiza o gancho
> `ConverterIndice` deixado em `CalculadoraItemFinanceiroBase` no E1-T03). Gate:
> `/analyze MOD-05 --epico E3`.

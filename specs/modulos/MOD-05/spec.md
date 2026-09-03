# SPEC — MOD-05: Gestão Financeira

> **Versão:** 2.3 (SDD etapa 4 aplicada) | **Data:** 2026-09-03 | **Fase de migração:** 4
> **Status:** 🔄 Tarefas geradas — pronta para `/implement` após `/analyze` verde · ver
> [`plan.md`](./plan.md), [`tasks.md`](./tasks.md), [`data-model.md`](./data-model.md),
> [`research.md`](./research.md), [`contracts/`](./contracts/)
> **Constituição ratificada:** [`specs/memory/constitution.md`](../../memory/constitution.md) v1.0
> **Substitui:** `specs/modulos/MOD-05-GESTAO-FINANCEIRA.md` (rascunho v1.0 de 2026-04-27,
> subdimensionado e citando `net8.0`)
> **Fonte legada:** `projeto_tag_1906/servidor/objeto de negócio/gestao.financeira/`
> (180 arquivos `.cs`, ~76.269 linhas) + `cliente/cliente.aplicativo/aplicativo.gestao.financeira/`
> (~90 formulários e user controls) + subconjunto de `projeto_tag_1906/Geral/` e
> `servidor/framework/`.

---

## 1. Visão Geral e Fronteiras

### 1.1 O que o módulo faz
Controla **contas a pagar e a receber**, **movimentação de caixa e banco**, **cheques**
(recebidos e emitidos), **adiantamentos e acertos**, **liquidação** de títulos e seu
**estorno**, **reversão** de movimentos, **rateio financeiro** por centro de custo/classe,
**DRE** (Demonstrativo de Resultado), **projeção de fluxo de caixa**, **programação de
cobrança**, **transação financeira entre filiais** e **integração bancária** (arquivo
remessa/retorno CNAB 240/400, boleto, conciliação).

### 1.2 Quem alimenta / quem consome
- **Alimentado por:** Faturamento (gera contas a receber), Gestão de Compra (gera contas a
  pagar), Gestão de Contrato, Gestão de Frota, Gestão de OS, NFSe, Gestão de Transporte
  (todos geram documentos financeiros via `ProcessoOrigem`).
- **Consumido por:** SPED Fiscal/PisCofins (parcelas liquidadas, formas de pagamento
  cartão), Gestão Contábil (exportação), relatórios.

### 1.3 Em escopo (nesta conversão)
- 100% de `Domain` / `Application (Services/Handlers)` / `Infrastructure (EF Core)` /
  `Api (Controllers)` das entidades e operações do módulo.
- Testes: Matriz RTV (validações) + Matriz ROT (operações/transações) + golden tests de
  paridade de cálculo, com 100% de cobertura (Artigo IX da constituição).
- **Épico E0 — `Versatus.SharedKernel` (projeto novo, `net10.0`, escopo mínimo — CLR-02):**
  enums financeiros + `Lookup` + container de rateio + interfaces transversais
  (`IMovimentoPeriodo`, `IDadosPeriodoFormaPagto`, `IDadosRateioFinanceiro`,
  `IDadosComissao` — CLR-05). `Result`/`ValidationResult`/`IContextoExecucao` **reusados de
  `Versatus.Framework`**, não duplicados. Ampliar só sob necessidade comprovada de um épico.

### 1.4 Fora de escopo (nesta conversão)
- Reescrever/alterar o legado (o legado continua ligado — Strangler Fig).
- Classes `*Lista` (eliminadas — Artigo V da constituição).
- Módulos dependentes (Faturamento, Compra, Contrato, Frota, OS, NFSe, Transporte,
  Contábil): referência apenas por **`int` lógico**, sem navegação EF cross-projeto.
  Em particular **não há `ProjectReference` para `Versatus.Faturamento`** (que ainda não
  existe) — ligação por `IdOrigem` + `ProcessoOrigem` (CLR-09).
- Migração da infraestrutura de rateio genérica (`RateioMovto`, `RateioMovtoItem`,
  `ManutencaoRateio`) — pertence ao **MOD-02 (AcessoGlobal)**.

### 1.4.1 Pré-requisito externo (CLR-01)
Antes do **épico E5**, o **MOD-02 (AcessoGlobal)** deve expor `RateioMovto`,
`RateioMovtoItem` e `ManutencaoRateio` migrados (hoje só existem no legado
`servidor/objeto de negócio/acesso.global/`). `MovtoFinanceiroRateio` e
`ManutencaoRateioFinanceiro` herdam dessas bases. Essa migração é uma **pré-tarefa no
MOD-02**, fora do MOD-05, e entra como dependência explícita no `plan.md`.

### 1.4.2 Estrangulamento existente (CLR-10)
`Servidor.Strangler/Gestao.Financeira/DTOs/` contém apenas `EntidadeDto`, `ClienteDto`,
`FornecedorDto`, `ParametroDto` — todos **entidades do AcessoGlobal**. O estrangulamento do
MOD-05 até aqui só trocou *lookups* de AcessoGlobal dentro de `Documento.cs` /
`DocumentoFinanceiroBase.cs` por chamadas HTTP à API nova do AcessoGlobal. **Nenhum
contrato financeiro é publicado** — nada a reconciliar do lado financeiro; o legado
continua chamando a API do AcessoGlobal como está.

### 1.5 Alcance do frontend React nesta rodada — **backend-first, React só no núcleo** (confirmado no /clarify)
| Recebe tela React agora | Não recebe tela agora (só backend + endpoint) |
| :--- | :--- |
| E4 Documento financeiro / parcela | E2 Domínio/Período (`FCentralDominio`, `FDominioFinanceiro*`) |
| E6 Liquidação (`FLiquidacaoDocumento`) | E7 Estorno de liquidação |
| E3 Caixa e Banco (`FCaixaBanco`, `FConsultaMovtoCaixa/Banco`) | E8 Reversão |
| E9 Cheques — **só** recebido, emitido, movimento de cheque e consultas (CLR-07) | E9 talão, suprimento, exclusão/manutenção, impressão de cheque |
| | E10 Adiantamentos / Acertos |
| | E11 DRE, Projeção de fluxo de caixa, Seleção de documento |
| | E12 Programação de cobrança, Transação entre filiais |
| | E13 Consultas avulsas · E14 Integração bancária |

> **Impressão (CLR-08):** DRE, cheque e recibo de liquidação — o endpoint devolve o
> **modelo de dados**; renderização/impressão é do frontend. Sem geração de PDF no backend.

> **Padrões de tela e specs de referência:** telas de cadastro = **Padrão A**
> (`BaseCadastroConfig<T>`); telas de operação (Liquidar/Estornar/Reverter/Acertar/Fechar,
> a maioria do MOD-05) = **Padrão C — Operação/Assistente**
> (`.agents/skills/migrate-crud/references/padrao-c-operacao.md`, `BaseOperacaoConfig`).
> Templates criados: [`docs/spec_fcaixabanco.md`](../../../docs/spec_fcaixabanco.md) (A) e
> [`docs/spec_fliquidacaodocumento.md`](../../../docs/spec_fliquidacaodocumento.md) (C).
> As demais `docs/spec_f*.md` são geradas na tarefa `frontend` do épico, seguindo esses
> templates. `BaseOperacaoConfig` é criada na tarefa `E6-T08` (1ª tela de operação).

---

## 2. Inventário de Classes

Legenda de **Tipo**: `Base` (abstrata/base) · `Entidade` (tabela `Fin*`) · `Agregado-raiz`
· `Filho` (parte de agregado) · `Operação` (transação/UseCase) · `Consulta` (view `vw*`) ·
`Lista` (eliminada — Artigo V) · `View/Auxiliar` (helper de apresentação/processo) ·
`Language` (strings i18n — não migra como entidade).

Legenda de **Seq.**: `Filial` = `AutoSequencial(..., SequencialTipo.Filial)` ·
`Global` = `AutoSequencial(...)` sem filial · `—` = sem sequencial próprio.

### 2.1 Kernel e bases (E0 / E1)

> **Regra das bases legadas (CLR-04):** nenhuma das classes abaixo herda do framework no
> novo sistema. Cada uma vira **classe abstrata POCO só com os campos compartilhados**; o
> **comportamento** (validação, sequência de persistência, rateio) migra para
> **serviço/handler compartilhado do módulo** e é rastreado na Matriz ROT. A coluna
> "Herda de" registra a cadeia **legada** (contexto de auditoria — Seção 3), não o desenho novo.

| Classe | Arq. (linhas) | Herda de (legado) | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `DocumentoFinanceiroBase` | 1303 | `ObjectGenerator` | — | Base | — | Campos comuns de conta a pagar/receber (número, histórico, valor, valor convertido, datas, `PagarReceberTipo`, lookups de entidade/operação/tipo doc/condição pagto/índice, rateio) |
| `OperacaoDocumentoBase` | 1098 | `ObjectGenerated` | — | Base | — | Base de operações sobre documento (`AplicarOperacao`, `GerarRateioItemFinanceiro`, `AtualizarRateio`); pai de `Liquidacao` e `Reversao` |
| `ItemFinanceiroBase` | 456 | `ObjectMaster` | — | Base | — | Base de item financeiro (parcela/movimento); pai de `ItemFinanceiro`, `DoctoItemFinanceiro`, `AplicacaoItemFin` |
| `ParcelaGeral` | 609 | `ObjectMaster` | — | Base | — | Base geral de parcela; pai de `ParcelaBase` |
| `ParcelaBase` | 772 | `ParcelaGeral` | — | Base | — | Base de título/parcela; pai de `DocumentoParcela` |
| `FechamentoCaixaBase` | 448 | `ObjectPersist` | — | Base | — | Base de fechamento de caixa; pai de `ResumoDominioPeriodoFormaPagtoConsulta` |
| `FormaMovInfo` | 146 | `ObjectPersist` | — | Base (abstract) | — | Base das informações específicas por forma de pagamento/recebimento |
| `DominioUtil` | 51 | — | — | Auxiliar | — | Helpers de domínio financeiro |
| `View/FinanceiroUtil` | 118 | — | — | Auxiliar | — | Helpers financeiros de apresentação |

### 2.2 Domínio Financeiro e Período (E2)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Dominio` | 1717 | `ObjectMaster` | `FinDominio` | Agregado-raiz | Filial | Domínio financeiro (agrupa caixas/bancos e períodos) |
| `DominioPeriodo` | 1017 | `ObjectMaster` | `FinDominioPeriodo` | Agregado-raiz | Filial | Período do domínio (ex.: "Caixa — Abril/2026"); controla abertura/fechamento (`CalcularTotalFormasPagto`) |
| `DominioPeriodoFechamento` | 331 | `ObjectPersist` | `FinDominioPeriodoFechamento` | Filho | Filial | Fechamento do período |
| `DominioPeriodoFechamentoDetalhe` | 112 | `ObjectBase` | (parte de fechamento) | Filho | — | Detalhe do fechamento |
| `DominioPeriodoFormaPagto` | 421 | `ObjectGenerated` | `FinDominioPeriodoFormaPagto` | Filho | Filial | Formas de pagamento habilitadas no período |
| `DominioPeriodoLacto` | 823 | `ObjectPersist` | `FinDominioPeriodoLancto` | Filho | Filial (`IdDominioPeriodoLacto`) | Lançamento do período |
| `DominioPeriodoLog` | 143 | `ObjectGenerated` | `FinLogDominioPeriodo` | Filho | Global | Log de alterações do período |
| `DominioResponsavel` | 356 | `ObjectPersist` | `FinDominioResponsavel` | Filho | — | Responsável pelo domínio |
| `DominioUsuario` | 508 | `ObjectPersist` | `FinDominioUsuario` | Filho | — | Usuários autorizados no domínio |
| `PeriodosAbertos` | 551 | (POCO, sem base) | — | Consulta/serviço | — | Resolve os períodos financeiros abertos para um contexto |

### 2.3 Caixa e Banco (E3)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `CaixaBanco` | 869 | `ObjectMaster` | `FinCaixaBanco` | Agregado-raiz | Filial | Caixa ou conta bancária; `ValidarControleCaixaBanco` |
| `CaixaBancoUsuario` | 350 | `ObjectPersist` | `FinCaixaBancoUsuario` | Filho | — | Usuários do caixa/banco |
| `ContaBancaria` | 1704 | `ObjectPersist` | `FinContaBancaria` | Agregado-raiz | — | **CLR-03:** entidade `ContaBancaria` (dados bancários, carteira, cedente, nosso-número) fica em **E3**; a lógica de **remessa/retorno CNAB e conciliação OFX** vira serviço em **E14** |
| `SaldoCaixaBanco` | 322 | `ObjectPersist` | `FinSaldoCaixaBanco` | Entidade | — | Saldo por caixa/banco/data |
| `SaldoRateio` | 382 | `ObjectPersist` | `FinSaldoRateio` | Entidade | — | Saldo de rateio |
| `Cobrador` | 367 | `ObjectPersist` | `FinCobrador` | Entidade | Filial | Cobrador (cadastro) |
| `IndiceConversor` | 222 | `ObjectPersist` | (índice econômico) | Entidade | — | Conversão de valor por índice econômico |

### 2.4 Documento Financeiro e Parcela (E4)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Documento` | 2803 | `DocumentoFinanceiroBase` | `FinDocumento` | Agregado-raiz | Filial | Conta a pagar/receber (`ExecutarPersistir`, `ExecutarExcluir`, `AplicarRateioTributos`, `ValidarOperacao`, `PersistirRateio`) |
| `DocumentoManutencao` | 490 | `ObjectPersist` | `FinDocumento` | Operação | — | Manutenção pontual de campos do documento |
| `DocumentoParcela` | 2111 | `ParcelaBase` | `FinDocumentoParcela` | Agregado-raiz (título) | Filial (`IdDocumentoParcela`) | Parcela = título independente a pagar/receber |
| `DocumentoParcelaManutencao` | 855 | `ObjectPersist` | `FinDocumentoParcela` | Operação | — | Manutenção de parcelas (vencimento, valor, portador, boleto) |
| `DocumentoParcelaImage` | 139 | `ObjectPersist` | `FinDocumentoParcelaImagem` | Filho | — | Imagem/boleto anexado à parcela |
| `DocumentoMovto` | 790 | `ObjectGenerated` | `FinDocumentoMovto` | Filho | Filial | Movimento do documento (histórico de alterações de valor/estado) |
| `DoctoItemFinanceiro` | 409 | `ItemFinanceiroBase` | `FinDoctoItemFinanceiro` | Filho | Filial | Item financeiro do documento (juros, multa, desconto, tarifa…) |
| `DoctoMovtoItemFinanceiro` | 273 | `ObjectPersist` | `FinDoctoMovtoItemFinanceiro` | Filho | — | Movimento do item financeiro |
| `DocumentoTributo` | 405 | `ObjectPersist` | `FinDocumentoTributo` | Filho | — | Tributos retidos/incidentes no documento |
| `DocumentoCartao` | 203 | `ObjectPersist` | `FinDocumentoCartao` | Filho | — | Dados de cartão no documento (movimento de cartão) |
| `DocumentoComissionado` | 300 | `ObjectPersist` | `FinDocumentoComissionado` | Filho | — | Comissionado vinculado ao documento |
| `DoctoCancelado` | 360 | `ObjectGenerator` | `FinDoctoCancelado` | Entidade | Filial | Documento cancelado (registro histórico) |
| `DoctoCanceladoParcela` | 173 | `ObjectPersist` | `FinDoctoCanceladoParcela` | Filho | Filial | Parcela do documento cancelado |

### 2.5 Movimento Financeiro e Formas de Pagamento (E5)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `MovimentoFinanceiro` | 2136 | `ObjectGenerator` | `FinMovimento` | Agregado-raiz | Filial | Movimento financeiro (entrada/saída em caixa/banco); `ValidarControleSaldo`, `ValidarValorMovto`, `PersistirConciliacao`, `ValidarRateio` |
| `MovimentoFinanceiroUpdate` | 109 | `ObjectPersist` | `FinMovimento` | Operação | — | Atualização pontual de movimento |
| `MovtoFinanceiroCheque` | 67 | `MovimentoFinanceiro` | `FinMovimento` | Especialização | Filial | Movimento financeiro que representa um cheque emitido |
| `MovtoFinanceiroRateio` | 384 | `RateioMovtoItem` (MOD-02) | `FinMovimentoRateio` | Filho | Filial (`IdRateioMovtoItem`) | Rateio do movimento por centro de custo/classe |
| `formapagamentomov` (`FormaPagamentoMov`) | 507 | `ObjectPersist` | (forma no movimento) | Filho | — | Forma de pagamento/recebimento aplicada no movimento |
| `FormaMovInfoDinheiro` | 260 | `FormaMovInfo` | — | Filho | — | Info — Dinheiro |
| `FormaMovInfoCartao` | 266 | `FormaMovInfo` | — | Filho | — | Info — Cartão (bandeira, NSU, autorização, parcelas) |
| `FormaMovInfoPix` | 342 | `FormaMovInfo` | — | Filho | — | Info — PIX (txid, chave, e2e) |
| `FormaMovInfoChequeCliente` | 135 | `FormaMovInfo` | — | Filho | — | Info — Cheque de cliente |
| `FormaMovInfoChequeEmpresa` | 133 | `FormaMovInfo` | — | Filho | — | Info — Cheque da empresa |
| `FormaMovInfoCredito` | 140 | `FormaMovInfo` | — | Filho | — | Info — Crédito (adiantamento/vale) |
| `FormaMovInfoDeposito` | 77 | `FormaMovInfoDinheiro` | — | Filho | — | Info — Depósito |
| `FormaMovInfoAbatimento` | 98 | `FormaMovInfo` | — | Filho | — | Info — Abatimento |
| `FormaMovInfoCrediario` | 41 | `FormaMovInfo` | — | Filho | — | Info — Crediário |
| `FormaMovInfoFinanceira` | 41 | `FormaMovInfo` | — | Filho | — | Info — Financeira |
| `FormaMovInfoOutros` | 43 | `FormaMovInfo` | — | Filho | — | Info — Outros |
| `TrocoDinheiro` | 117 | `FormaPagamentoMov` | — | Filho | — | Troco em dinheiro |
| `ItemFinanceiro` | 529 | `ItemFinanceiroBase` | `FinItemFinanceiro` | Entidade | Global | Cadastro de item financeiro (tipo de juros/multa/desconto/tarifa) |
| `ItemFinanceiroOperacao` | 428 | `ObjectPersist` | `FinItemFinanceiroOperacao` | Filho | — | Vínculo item financeiro × operação |
| `AplicacaoItemFin` | 434 | `ItemFinanceiroBase` | — | Operação | — | Aplicação de item financeiro num título |
| `AplicacaoItemFinGeral` | 290 | `ObjectPersist` | — | Operação | — | Aplicação geral de item financeiro |
| `AplicacaoItemFinReversao` | 144 | `AplicacaoItemFinGeral` | — | Operação | — | Aplicação de item financeiro na reversão |
| `ContraPartidaRateioDoctoFinanceiro` | 296 | `ObjectBase` | — | Filho | — | Contrapartida de rateio do documento financeiro |
| `ManutencaoRateioFinanceiro` | 408 | `ManutencaoRateio` (MOD-02) | `GloManutencaoRateio` | Operação | Global | Manutenção de rateio financeiro |

### 2.6 Liquidação (E6)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Liquidacao` | 735 | `OperacaoDocumentoBase` | — (opera sobre `FinMovimento`, `FinLiquidacaoFormaMovimento`) | Operação | — | Liquidar parcela(s): `ValidarLiquidacao`, `PersistirLiquidacao`, `AtualizarRateio` |
| `LiquidacaoFormaMovimento` | 183 | `ObjectPersist` | `FinLiquidacaoFormaMovimento` | Filho | — | Forma de movimento aplicada na liquidação |

### 2.7 Estorno de Liquidação (E7)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `LiquidacaoEstorno` | 2617 | `ObjectGenerator` | `FinLiquidacaoEstorno` | Operação/Agregado | Filial | Estornar liquidação: `ValidarRateio`, `ValidarDominio`, `ExecutarPersistir` |
| `LiquidacaoEstornoFormaPagto` | 1074 | `ObjectMaster` | `FinLiquidacaoEstornoFormaPagto` | Filho | Filial | Forma de pagamento do estorno |
| `LiquidacaoEstornoItemFin` | 80 | `ObjectBase` | (parte do estorno) | Filho | — | Item financeiro do estorno |

### 2.8 Reversão (E8)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Reversao` | 1985 | `OperacaoDocumentoBase` | `FinReversao` | Operação/Agregado | Filial | Reverter movimentos/parcelas: `ValidarOrigemGerador`, `GerarRateioItemFinanceiro`, `ValidarRateio`, `PersistirRateio`, `ExecutarPersistir`, `PersistirCorrecaoRateioReversao` |
| `ReversaoDoctoParcela` | 233 | `ObjectPersist` | `FinReversaoDoctoParcela` | Filho | Filial | Parcela revertida |
| `ReversaoItemFinanceiro` | 248 | `ObjectPersist` | `FinReversaoItemFinanceiro` | Filho | Filial | Item financeiro revertido |

### 2.9 Cheques (E9)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Cheque` | 460 | `ObjectPersist` | (dados de cheque) | Filho/VO | — | Dados de um cheque (número, banco, agência, conta, valor, bom para) |
| `ChequeRecebido` | 1105 | `ObjectPersist` | `FinChequeRecebido` | Agregado-raiz | Filial | Cheque recebido de cliente (`IMovimentoPeriodo`) |
| `ChequeRecebidoMovto` | 1692 | `ObjectGenerator` | `FinChequeRecebidoMovto` | Agregado-raiz | Filial | Movimentos do cheque recebido (custódia, depósito, devolução, repasse, baixa) |
| `ChequeEmitidoMovto` | 1667 | `ObjectGenerator` | `FinChequeEmitidoMovto` | Agregado-raiz | Filial | Movimentos do cheque emitido (emissão, compensação, cancelamento) |
| `ChequeEmitidoMovtoUpdate` | 104 | `ObjectPersist` | `FinChequeEmitidoMovto` | Operação | — | Atualização pontual de movimento de cheque emitido |
| `TalaoCheque` | 1103 | `ObjectPersist` | `FinTalaoCheque` | Agregado-raiz | — | Talão de cheque (faixa de folhas, conta, status por folha) |
| `SuprimentoCheque` | 277 | `ObjectBase` | — | Operação | — | Suprimento de folhas de cheque |
| `MotivoDevolucaoCheque` | 284 | `ObjectPersist` | `FinMotivoDevolucaoCheque` | Entidade | — | Cadastro de motivo de devolução (alíneas) |

### 2.10 Adiantamentos e Acertos (E10)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Adiantamento` | 749 | `ObjectGenerator` | `FinAdiantamento` | Agregado-raiz | Filial | Adiantamento a fornecedor/funcionário/cliente |
| `AdtoAcerto` | 1527 | `ObjectMaster` | `FinAdtoAcerto` | Operação/Agregado | Filial | Acerto (prestação de contas) do adiantamento (`IMovimentoPeriodo`); `ExecutarPersistir`, `ExecutarExcluir`, `AtualizarRateio` |
| `AdtoAcertoDistribuicao` | 477 | `ObjectPersist` | `FinAdtoAcertoDistribuicao` | Filho | Filial | Distribuição do acerto por documento/despesa |
| `AdtoAcertoDistribuicaoRateio` | 307 | `ObjectBase` | — | Filho | — | Rateio da distribuição do acerto |
| `AdtoAcertoMovto` | 621 | `ObjectPersist` | `FinAdtoAcertoMovto` | Filho | Filial | Movimento do acerto |
| `AdtoLanctoEntidade` | 149 | `ObjectBase` | — | Filho | — | Lançamento do adiantamento por entidade |

### 2.11 DRE, Projeção e Seleção (E11)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `DRE` | 739 | `ObjectMaster` | `FinDRE` | Agregado-raiz | Filial | Estrutura do Demonstrativo de Resultado |
| `DRETitulo` | 514 | `ObjectMaster` | `FinDRETitulo` | Filho | Filial | Título (linha) do DRE |
| `DRETituloClasse` | 362 | `ObjectPersist` | `FinDRETituloClasse` | Filho | — | Classe vinculada ao título do DRE |
| `DRETituloOperacao` | 294 | `ObjectPersist` | `FinDRETituloOperacao` | Filho | — | Operação vinculada ao título do DRE |
| `View/ClasseAvulsaDRE` | 143 | `ObjectBase` | — | Auxiliar | — | Classe avulsa no DRE (montagem) |
| `View/DREImpressaoView` | 189 | `ObjectMaster` | — | Auxiliar | — | View de impressão do DRE |
| `ProjecaoFluxoCaixa` | 771 | `ObjectMaster` | `FinProjecaoFluxoCaixa` | Agregado-raiz | Filial | Projeção de fluxo de caixa (cenário) |
| `ProjecaoFluxoCaixaLacto` | 484 | `ObjectGenerator` | `FinProjecaoFluxoCaixaLacto` | Filho | Filial | Lançamento avulso da projeção |
| `SelecaoDocumento` | 1226 | `ObjetoBaseSAO` | — | Consulta/serviço | — | Seleção de documentos/parcelas para operações em lote (liquidação, cobrança, remessa) |

### 2.12 Programação de Cobrança e Transação entre Filiais (E12)

| Classe | Arq. (linhas) | Herda de | Tabela legada | Tipo | Seq. | Propósito |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `ProgramacaoCobranca` | 401 | `ObjectMaster` | `FinProgramacaoCobranca` | Agregado-raiz | Filial | Programação de cobrança (régua) |
| `ProgramacaoCobrancaParcela` | 249 | `ObjectPersist` | `FinProgramacaoCobrancaParcela` | Filho | Filial | Parcela dentro da programação |
| `TransacaoFilial/TransacaoFilial` | 640 | `ObjectPersist` | `FinTransacaoFilial` | Agregado-raiz | Global | Transação financeira entre filiais |
| `TransacaoFilial/TransacaoFinanceira` | 326 | `ObjectGenerated` | `FinTransacaoFinanceira` | Filho | Global | Lançamento financeiro da transação entre filiais |
| `TransacaoFilial/FilialMovimento` | 278 | `ObjectGenerated` | `FinFilialMovto` | Filho | Global | Movimento por filial da transação |

### 2.13 Consultas (E13) — views legadas `vw*` → read-models / query-methods

| Classe | Arq. (linhas) | View legada | Propósito |
| :--- | :--- | :--- | :--- |
| `Consultas/DocumentoConsulta` | 855 | `vwDocumento` | Consulta de documentos financeiros |
| `Consultas/DocumentoParcConsulta` | 1508 | `vwDocumentoParcela` | Consulta de parcelas (base de várias telas) |
| `Consultas/DocumentoParcConsultaRetornoBoleto` | 289 | `vwDocumentoParcela` (herda de `DocumentoParcConsulta`) | Parcelas para retorno de boleto |
| `Consultas/MovimentoFinanceiroConsulta` | 346 | `vwlMovimentoFinanceiro` | Consulta de movimentos financeiros |
| `Consultas/LiquidacaoEstornoConsulta` | 551 | `vwLiquidacaoEstorno` | Consulta de estornos |
| `Consultas/LiqEstornoConsulta` | 213 | `vwlConsultaLiquidacaoEstorno` | Consulta resumida de estorno |
| `Consultas/LiquidacaoFormaPagtoCartaoSpedConsulta` | 153 | `vwoLiquidacaoFormaPagtoCartaoSped` | Formas de pagto cartão para SPED |
| `Consultas/CaixaBancoConsulta` | 258 | `vwlCaixaBancoUsuario` | Caixas/bancos por usuário |
| `Consultas/CaixaBancoContaConsulta` | 680 | `vwCaixaBancoContaBancaria` | Caixa/banco × conta bancária |
| `Consultas/CaixaBancoTalaoConsulta` | 320 | `vwCaixaBancoCheque` | Caixa/banco × talão/cheque |
| `Consultas/RelacaoContaTalaoConsulta` (`RelacaoContaBancariaComTalaoConsulta`) | 291 | `vwRelacaoContaBancariaComTalao` | Contas com talão |
| `Consultas/ChequeEmitidoConsultaBase` | 552 | (base) | Base das consultas de cheque emitido |
| `Consultas/ChequeEmitidoConsulta` | 95 | `vwFinMovimentoChequeEmitido` | Cheques emitidos |
| `Consultas/ChequeEmitidoMovtoTalaoConsulta` | 60 | `vwoFinMovimentoChequeEmitidoTalao` | Cheques emitidos por talão |
| `Consultas/ChequeRecebidoConsulta` | 505 | `vwChequeRecebidoAberto` | Cheques recebidos em aberto |
| `Consultas/ChequeRecebidoMovtoFinanceiroConsulta` | 287 | `vwoChequeRecebidoMovtoFinanceiro` | Movto financeiro de cheque recebido |
| `Consultas/ChequeRecEmitConsulta` | 327 | `vwChequeEmitidoRecebido` | Cheques emitidos+recebidos |
| `Consultas/PrimeiroMovtoChequeEmitidoConsulta` | 336 | `vwPrimeiroMovtoChequeEmitido` | Primeiro movimento do cheque emitido |
| `Consultas/UltimoMovtoChequeRecConsulta` | 385 | `vwUltimoMovtoChequeRecebido` | Último movimento do cheque recebido |
| `Consultas/AdiantamentoConsulta` | 139 | `vwlAdiantamento` | Adiantamentos |
| `Consultas/AdtoAcertoConsulta` | 131 | `VWLFinAdtoAcerto` | Acertos de adiantamento |
| `Consultas/DominioPeriodoFormaPagtoConsulta` | 166 | `vwDominioPeriodoFormaPagto` | Formas de pagto do período |
| `Consultas/DominioUsuarioSuprimentoSangriaConsulta` | 280 | `vwlDominioUsuarioSuprimentoSangria` | Suprimento/sangria por usuário |
| `Consultas/ResumoPeriodoFormaPagtoConsulta` (`ResumoDominioPeriodoFormaPagtoConsulta`) | 286 | `vwResumoDominioPeriodoFormaPagto` (herda `FechamentoCaixaBase`) | Resumo do fechamento por forma de pagto |
| `Consultas/ItemFinanceiroOperacaoFilialConsulta` | 177 | `vwoItemFinanceiroOperacaoFilial` | Item financeiro × operação × filial |
| `Consultas/ProgramacaoCobrancaParcelaConsulta` | 424 | `vwoProgramacaoCobrancaParcela` | Parcelas na programação de cobrança |
| `Consultas/ReversaoDoctoParcelaChequeConsulta` | 269 | `vwoFinReversaoDoctoParcelaCheque` | Reversão de parcela/cheque |
| `Consultas/ParcelaLiquidadaExportacao` | 319 | `VWParcelaLiquidadaExportacao` | Exportação de parcelas liquidadas |
| `Consultas/ParcelaLiquidadaExportacaoConsulta` | 377 | `VWPARCELALIQUIDADAEXPORTACAO` | Idem (consulta) |

### 2.14 Integração Bancária (E14 — fase dedicada final)

| Classe | Arq. (linhas) | Herda de | Tipo | Propósito |
| :--- | :--- | :--- | :--- | :--- |
| `View/ArquivoRemessaView` | 706 | `ObjectBase` | Operação | Geração de arquivo remessa CNAB (usa `BoletoNet`) |
| `View/ArquivoRetornoBase` | 421 | `ObjectBase` | Base | Base de leitura de arquivo retorno |
| `View/ArquivoRetornoBaseView<T,I>` | 313 | `ArquivoRetornoBase` | Base genérica | Base genérica de retorno CNAB |
| `View/ArquivoRetorno240View` | 270 | `ArquivoRetornoBaseView<ArquivoRetornoCNAB240, DetalheRetornoCNAB240>` | Operação | Processa retorno CNAB 240 |
| `View/ArquivoRetorno400View` | 227 | `ArquivoRetornoBaseView<ArquivoRetornoCNAB400, DetalheRetorno>` | Operação | Processa retorno CNAB 400 |
| `View/ArquivoRetornoViewOutro` | 250 | `ObjectBase` | Operação | Retorno de outros formatos |
| `View/AprovacaoDoctoPagarView` | 411 | `ObjectBase` | Operação | Aprovação de documentos a pagar |
| `View/LiberacaoDoctoPagarView` | 528 | `ObjectBase` | Operação | Liberação de documentos a pagar |
| `View/TransacaoFinanceiraView` | 267 | `ObjectBase` | Auxiliar | View da transação financeira entre filiais |

### 2.15 Classes `*Lista` (eliminadas — Artigo V) — não migram

`AdtoAcertoDistribuicaoLista`, `AdtoAcertoMovtoLista`, `AdtoLanctoEntidadeLista`,
`CaixaBancoUsuarioLista`, `ChequeEmitidoMovtoLista`, `ChequeLista`,
`ChequeRecebidoMovtoLista`, `DRETituloClasseLista`, `DRETituloLista`,
`DRETituloOperacaoLista`, `DoctoCanceladoParcelaLista`, `DoctoItemFinanceiroLista`,
`DoctoMovtoItemFinanceiroLista`, `DocumentoComissionadoLista`, `DocumentoMovtoLista`,
`DocumentoParcelaImageLista`, `DocumentoParcelaLista`, `DominioPeriodoFechamentoLista`,
`DominioPeriodoLista`, `DominioResponsavelLista`, `DominioUsuarioLista`,
`ItemFinanceiroOperacaoLista`, `LiquidacaoEstornoFormaPagtoLista`,
`LiquidacaoEstornoItemFinLista`, `LiquidacaoFormaMovimentoLista`,
`ProgramacaoCobrancaParcelaLista`, `ProjecaoChequeEmitidoLista`,
`ProjecaoChequeRecebidoLista`, `ProjecaoFluxoCaixaLactoLista`, `ProjecaoRecPagarLista`,
`ReversaoDoctoParcelaLista`, `ReversaoItemFinanceiroLista`, `SaldoCaixaBancoLista`,
`SelecaoDocumentoLista`, `TransacaoFilialLista`, `TrocoFormaLista`,
`aplicacaoitemfingerallista`, `aplicacaoitemfinlista`, `documentotributolista`,
`formapagamentomovlista`, `View/ClasseAvulsaDRELista`.

→ Substituídas por `IReadOnlyList<T>` na coleção do agregado ou `PagedResult<T>` na
consulta. A lógica útil eventualmente contida nelas (ordenação, totalização) migra para o
serviço/agregado correspondente e é rastreada na Matriz RTV/ROT.

### 2.16 `*Language` (i18n) — não migram como entidade

`AcertoLanguage`, `DocumentoParcelaListaLanguage`, `FormaMovInfoLanguage`,
`FormaPagamentoMovLanguage`, `MovimentoFinanceiroLanguage`, `ParcelaBaseLanguage`.
→ Tratados pela estratégia de i18n do frontend/serviço; strings exatas de mensagem entram
na Matriz RTV.

---

## 3. Árvore de Herança

### 3.1 Framework legado (`servidor/framework/servidor.framework/`)
```
MarshalByRefObject                       (removido — era Remoting)
└─ ObjectBase (abstract)                 ObjetoNegocio.cs:30
   ├─ ObjectPersist (abstract)           ObjetoNegocio.cs:187   (IPersistent)
   │  └─ ObjectMaster (abstract)         ObjetoNegocio.cs:1964
   │     └─ ObjectGenerated (abstract)   ObjetoNegocio.cs:2295
   │        └─ ObjectGenerator (abstract)ObjetoNegocio.cs:2434
   ├─ ObjetoBaseSAO                       ObjetoBaseSAO.cs:9
   └─ ListBase → ListPersist             ListaBase.cs / ListaBasePersistencia.cs  (eliminados — Artigo V)
```

### 3.2 Cadeias do MOD-05
```
ObjectGenerator
├─ DocumentoFinanceiroBase ─ Documento
├─ OperacaoDocumentoBase (abstract) ─ Liquidacao
│                                   └ Reversao
├─ Adiantamento
├─ ChequeEmitidoMovto
├─ ChequeRecebidoMovto
├─ DoctoCancelado
├─ LiquidacaoEstorno
├─ MovimentoFinanceiro ─ MovtoFinanceiroCheque
└─ ProjecaoFluxoCaixaLacto

ObjectGenerated (direto)
├─ DocumentoMovto
├─ DominioPeriodoFormaPagto
├─ DominioPeriodoLog
├─ TransacaoFilial/FilialMovimento
└─ TransacaoFilial/TransacaoFinanceira

ObjectMaster (direto)
├─ ItemFinanceiroBase ─ ItemFinanceiro
│                     ├ DoctoItemFinanceiro
│                     └ AplicacaoItemFin
├─ ParcelaGeral ─ ParcelaBase (abstract) ─ DocumentoParcela
├─ AdtoAcerto
├─ CaixaBanco
├─ DRE
├─ DRETitulo
├─ Dominio
├─ DominioPeriodo
├─ LiquidacaoEstornoFormaPagto
├─ ProgramacaoCobranca
├─ ProjecaoFluxoCaixa
└─ View/DREImpressaoView

ObjectPersist (direto)  — ~35 classes (Cheque, ChequeRecebido, Cobrador, ContaBancaria,
  todas as Consultas/*, DocumentoCartao, DocumentoComissionado, DocumentoManutencao,
  DocumentoParcelaImage, DocumentoParcelaManutencao, DominioPeriodoFechamento,
  DominioResponsavel, DominioUsuario, DoctoMovtoItemFinanceiro, documentotributo,
  FechamentoCaixaBase, formapagamentomov, IndiceConversor, ItemFinanceiroOperacao,
  LiquidacaoFormaMovimento, MotivoDevolucaoCheque, MovimentoFinanceiroUpdate,
  ProgramacaoCobrancaParcela, ReversaoDoctoParcela, ReversaoItemFinanceiro,
  SaldoCaixaBanco, SaldoRateio, TalaoCheque, TransacaoFilial/TransacaoFilial,
  aplicacaoitemfingeral, CaixaBancoUsuario, AdtoAcertoDistribuicao, AdtoAcertoMovto)

FormaMovInfo (abstract, : ObjectPersist)
├─ FormaMovInfoDinheiro ─ FormaMovInfoDeposito
├─ FormaMovInfoCartao
├─ FormaMovInfoPix
├─ FormaMovInfoChequeCliente
├─ FormaMovInfoChequeEmpresa
├─ FormaMovInfoCredito
├─ FormaMovInfoAbatimento
├─ FormaMovInfoCrediario
├─ FormaMovInfoFinanceira
└─ FormaMovInfoOutros

ObjectBase (direto)
├─ AdtoAcertoDistribuicaoRateio
├─ AdtoLanctoEntidade
├─ ContraPartidaRateioDoctoFinanceiro
├─ DominioPeriodoFechamentoDetalhe
├─ LiquidacaoEstornoItemFin
├─ SuprimentoCheque
└─ View/{AprovacaoDoctoPagarView, ArquivoRemessaView, ArquivoRetornoBase,
         ClasseAvulsaDRE, LiberacaoDoctoPagarView, TransacaoFinanceiraView,
         ArquivoRetornoViewOutro}

ObjetoBaseSAO
└─ SelecaoDocumento

Bases externas (não no framework, não neste módulo)
├─ RateioMovtoItem  (servidor/objeto de negócio/acesso.global/RateioMovtoItem.cs) ─ MovtoFinanceiroRateio
├─ ManutencaoRateio (servidor/objeto de negócio/acesso.global/ManutencaoRateio.cs) ─ ManutencaoRateioFinanceiro
├─ ArquivoRetornoCNAB240 / ArquivoRetornoCNAB400 / DetalheRetorno* (BoletoNet) ─ ArquivoRetorno*View
└─ POCO puro (sem base): PeriodosAbertos, DominioUtil, View/FinanceiroUtil
```

> **Regra:** ao migrar cada classe, a auditoria (`legacy-validation-audit` +
> `legacy-operation-audit`) sobe a cadeia inteira até `ObjectBase`, lendo cada nível por
> completo (Artigo IX; skill §1).

### 3.3 Como a cadeia legada mapeia para o novo sistema (CLR-04)

A cadeia `ObjectBase → ObjectPersist → ObjectMaster → ObjectGenerated → ObjectGenerator`
**não existe** no novo sistema (confirmado: `Versatus.AcessoGlobal` e
`Versatus.GestaoTributo` usam entidade POCO pura, sem classe-base; `Versatus.Framework` não
tem base de entidade). Cada responsabilidade da cadeia tem destino próprio:

| Responsabilidade na cadeia legada | Destino no novo sistema |
| :--- | :--- |
| `MarshalByRefObject` (.NET Remoting) | Removido (DEC-002) |
| `ObjectPersist.Persist()` / `ExecutarPersistir` / `OnBefore*Persist*` (persistência no objeto) | `DbContext` + Repository; orquestração no **Handler/Service** (DEC-003) — coberto pela Matriz ROT |
| `ObjectGenerator` + `[AutoSequencial]` (geração de ID) | `GeradorSequencialService` + `.ValueGeneratedNever()` no mapping (RN-05-006) |
| `ObjectMaster` (tracking de agregado/filhos) | Change tracking do EF Core + agregado `List<T>` privada / `IReadOnlyList<T>` público (Artigo V) |
| `IAmbiente` carregado pela cadeia (usuário/filial/transação) | `IContextoExecucao` injetado por DI (reusado de `Versatus.Framework`) |
| `TransacaoBase` passada entre objetos | `IDbContextTransaction` aberta no Handler (DEC-003) |

**Bases do próprio módulo** (`DocumentoFinanceiroBase`, `OperacaoDocumentoBase`,
`ItemFinanceiroBase`, `ParcelaGeral`/`ParcelaBase`, `FormaMovInfo`, `FechamentoCaixaBase`):
→ **classe abstrata POCO só com os campos compartilhados** (sem herdar nada) +
**serviço/handler compartilhado** para o comportamento. A decisão POCO-abstrata vs.
serviço para cada base é aplicada individualmente no `/plan`, guiada por esta regra.

---

## 4. Mapa de Dependências

Detalhe completo em [`dependency-graph.md`](./dependency-graph.md). Resumo:

### 4.1 Cross-módulo (por volume de `using` no legado)
| Módulo/Namespace legado | Peso | Natureza da dependência | Tratamento nesta conversão |
| :--- | :--- | :--- | :--- |
| `Interface.AcessoGlobal` + `ObjetosNegocio.AcessoGlobal` | alto (128 + 72) | Entidade (Entidade/Filial/Operação/CondicaoPagamento/Banco/Portador/CentroCusto/Classe/Usuario), rateio (`RateioMovto`, `RateioMovtoItem`, `ManutencaoRateio`), saldo (`ObjetosNegocio.AcessoGlobal.Saldo`) | `int` lógico para entidades; **`RateioMovtoItem`/`ManutencaoRateio` são classes-base** → **pré-tarefa no MOD-02** para expô-las antes do E5 (CLR-01, §1.4.1) |
| `Interface.Ambiente` + `ObjetosNegocio.Ambiente` | alto (125) | `IAmbiente` (usuário, filial, data/hora do sistema, transação) | Substituído por `IContexto` injetado (Glossário da constituição) |
| `Servidor.Framework` + `Interface.Framework` | alto (173 + 106) | `ObjectBase` e cadeia, `TransacaoBase`, `Factory.Instanciar`, `Lookup`, `AutoSequencial`, `SequencialTipo` | `TransacaoBase`→`DbContext`; `Factory`→DI; `Lookup`→cache/repo; `AutoSequencial`→`GeradorSequencialService` + `ValueGeneratedNever()` |
| `Projeto.Geral` + `.Enumerado` + `.EnumeradoObjeto` | alto (141 + 128 + 69) | Enums, `ValidationResult`, helpers, `RateioMovto` container | Subconjunto **mínimo** → `Versatus.SharedKernel` (E0, CLR-02): enums + `Lookup` + container de rateio + interfaces transversais. `ValidationResult` reusado de `Versatus.Framework`. |
| `Interface.BaseDistribuicao` | médio (18) | Base de documento com distribuição por filial | `DocumentoBase` com `IdDistribuicao` (Glossário) — coordenar com MOD-04 |
| `Interface.Impressao` | baixo (8) | Contratos de impressão (DRE, cheque, recibo) | **CLR-08:** fora do domínio; endpoint retorna o modelo de dados, renderização/impressão é do frontend. Sem PDF no backend. |
| `Interface.Faturamento` | baixo (8) | Documento de venda que originou a conta a receber | `int` lógico (`IdOrigem` + `ProcessoOrigem`); **sem `ProjectReference`** (CLR-09) |
| `Interface.NFSe` / `GestaoTransporte` / `GestaoOS` / `GestaoMaterial` / `GestaoFrota` / `GestaoContrato` / `GestaoContabil` / `GestaoEval` | baixo (1–2 cada) | Origem de documento / consumo de dados financeiros | `int` lógico; nenhum `Include` cross-projeto |
| `BoletoNet` | baixo (6) | Geração remessa / leitura retorno CNAB | Épico **E14**; substituto .NET 10 avaliado em `research.md` |
| `Versatus.Eval` | baixo (1) | Licenciamento/avaliação | Fora de escopo |
| `Servidor.Strangler.GestaoFinanceira.DTOs` + `.Common` | baixo (2 + 2) | Só `EntidadeDto`/`ClienteDto`/`FornecedorDto`/`ParametroDto` — **entidades do AcessoGlobal** | **CLR-10:** nenhum contrato financeiro publicado; nada a reconciliar do lado financeiro. `plan.md §4` = nota curta. |
| `Gentle.Framework` | alto (131) | ORM legado | → EF Core 10.x (DEC-001) |

### 4.2 Interfaces implementadas com significado de domínio (destino — CLR-05)
| Interface | Implementada por | Destino no novo sistema |
| :--- | :--- | :--- |
| `IMovimentoPeriodo` | `MovimentoFinanceiro`, `ChequeRecebido`, `AdtoAcerto` | `Versatus.SharedKernel` (transversal: controle de período) |
| `IDadosPeriodoFormaPagto` | `LiquidacaoEstorno` | `Versatus.SharedKernel` |
| `IDadosRateioFinanceiro` | `ContraPartidaRateioDoctoFinanceiro` | `Versatus.SharedKernel` |
| `IDadosComissao` | `Documento`, `LiquidacaoEstorno`, `Reversao` | `Versatus.SharedKernel` (futuro Faturamento consome sem depender do MOD-05) |
| `IItemList` | várias | Conceito de UI legada — **não migra** (era grade editável) |
| `ILanguageStrings` | várias | i18n — não é domínio |

---

## 5. Enums

Enums de `Projeto.Geral.Enumerado` / `.EnumeradoObjeto` usados pelo módulo (contagem de
ocorrências no legado). **Valores inteiros a preservar** — extração exata é tarefa do
`/plan` (`research.md`) com base em `docs/analise_integracao_enumerados.md` e no código de
`Projeto.Geral`.

| Enum | Ocorr. | Uso |
| :--- | :--- | :--- |
| `ProcessoOrigem` | 563 | Identifica o processo que gerou o registro (Liquidacao, Reversao, AdtoAcerto, Faturamento, Compra, Contrato, Frota, OS, NFSe…) |
| `PagarReceberTipo` | 177 | Pagar / Receber / MovimentoCartão |
| `SituacaoDocumento` | 69 | Aberto / Liberado / Liquidado / Cancelado / (parcial) |
| `OperacaoTipo` | 66 | Pagar / Receber / MovimentoCartao (classifica a operação) |
| `TipoMovimento` | 21 | Tipo do movimento financeiro |
| `SituacaoMovimento` | 19 | Situação do movimento (normal / conciliado / estornado) |
| `TipoLancamento` | 16 | Tipo de lançamento do período |
| `SituacaoParcela` | 16 | Aberta / Parcial / Liquidada / Cancelada |
| `PeriodoTipo` | 16 | Tipo do período do domínio |
| `FormaMovimento` | 15 | Forma de movimento (dinheiro/cartão/cheque/pix/…) |
| `SituacaoCheque` | 11 | Situação do cheque (carteira/custódia/depositado/compensado/devolvido/…) |
| `SequencialTipo` | — | Filial / Global (parâmetro do `AutoSequencial`) |

> Enums adicionais (bandeira de cartão, alínea de devolução, tipo de conta bancária, tipo
> de talão, etc.) serão inventariados por épico durante `/plan`.

---

## 6. Épicos (ordem topológica de execução)

| Épico | Nome | Classes-núcleo | Depende de | Análise prévia obrigatória (>1.500 l) | Tela React nesta rodada |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **E0** | Kernel compartilhado (`Versatus.SharedKernel`) — escopo **mínimo** (CLR-02) | enums financeiros + `Lookup` + container de rateio + interfaces `IMovimentoPeriodo`, `IDadosPeriodoFormaPagto`, `IDadosRateioFinanceiro`, `IDadosComissao` (CLR-05). Reusa `Result`/`ValidationResult`/`IContextoExecucao` de `Versatus.Framework`. | Framework | — | Não |
| **E1** | Bases do módulo | `DocumentoFinanceiroBase`, `OperacaoDocumentoBase`, `ItemFinanceiroBase`, `ParcelaGeral`, `ParcelaBase`, `FormaMovInfo`, `FechamentoCaixaBase` | E0 | `DocumentoFinanceiroBase` (1303) | Não |
| **E2** | Domínio e Período | `Dominio`, `DominioPeriodo`, `DominioPeriodoFechamento(+Detalhe)`, `DominioPeriodoFormaPagto`, `DominioPeriodoLacto`, `DominioPeriodoLog`, `DominioResponsavel`, `DominioUsuario`, `PeriodosAbertos` | E1 | `Dominio` (1717), `DominioPeriodo` (1017) | Não |
| **E3** | Caixa e Banco | `CaixaBanco`, `CaixaBancoUsuario`, `ContaBancaria` (**só a entidade** — CLR-03), `SaldoCaixaBanco`, `SaldoRateio`, `Cobrador`, `IndiceConversor` | E2 | `ContaBancaria` (1704) — análise marca o que é E14 | **Sim** |
| **E4** | Documento e Parcela | `Documento`, `DocumentoParcela`, `DocumentoMovto`, `DoctoItemFinanceiro`, `DoctoMovtoItemFinanceiro`, `DocumentoTributo`, `DocumentoCartao`, `DocumentoComissionado`, `DoctoCancelado(+Parcela)`, `DocumentoManutencao`, `DocumentoParcelaManutencao`, `DocumentoParcelaImage` | E1, E3 | `Documento` (2803), `DocumentoParcela` (2111) | **Sim** |
| **E5** | Movimento e Formas de Pagamento | `MovimentoFinanceiro`, `MovimentoFinanceiroUpdate`, `MovtoFinanceiroCheque`, `MovtoFinanceiroRateio`, `formapagamentomov`, `FormaMovInfo*` (11), `TrocoDinheiro`, `ItemFinanceiro`, `ItemFinanceiroOperacao`, `AplicacaoItemFin(+Geral)`, `ContraPartidaRateioDoctoFinanceiro`, `ManutencaoRateioFinanceiro` | E2, E3, **+ MOD-02: rateio migrado** (CLR-01, §1.4.1) | `MovimentoFinanceiro` (2136) | Não |
| **E6** | Liquidação | `Liquidacao`, `LiquidacaoFormaMovimento` | E4, E5 | — | **Sim** |
| **E7** | Estorno de Liquidação | `LiquidacaoEstorno`, `LiquidacaoEstornoFormaPagto`, `LiquidacaoEstornoItemFin` | E6 | `LiquidacaoEstorno` (2617), `LiquidacaoEstornoFormaPagto` (1074) | Não |
| **E8** | Reversão | `Reversao`, `ReversaoDoctoParcela`, `ReversaoItemFinanceiro`, `AplicacaoItemFinReversao` | E4, E6 | `Reversao` (1985) | Não |
| **E9** | Cheques | `Cheque`, `ChequeRecebido`, `ChequeRecebidoMovto`, `ChequeEmitidoMovto`, `ChequeEmitidoMovtoUpdate`, `TalaoCheque`, `SuprimentoCheque`, `MotivoDevolucaoCheque` | E3, E5 | `ChequeRecebidoMovto` (1692), `ChequeEmitidoMovto` (1667), `TalaoCheque` (1103), `ChequeRecebido` (1105) | **Sim — só** recebido/emitido/movimento/consulta (CLR-07) |
| **E10** | Adiantamentos e Acertos | `Adiantamento` (CRUD), `AdtoAcerto` (**operação** — Handler + ROT, CLR-12), `AdtoAcertoDistribuicao(+Rateio)`, `AdtoAcertoMovto`, `AdtoLanctoEntidade` | E4, E5 | `AdtoAcerto` (1527) | Não |
| **E11** | DRE, Projeção, Seleção | `DRE`, `DRETitulo(+Classe/+Operacao)`, `ProjecaoFluxoCaixa`, `ProjecaoFluxoCaixaLacto`, **`SelecaoDocumento` → serviço de consulta sem entidade** (CLR-06), `View/{ClasseAvulsaDRE, DREImpressaoView}` | E4, E5 | `SelecaoDocumento` (1226) | Não |
| **E12** | Programação de Cobrança e Transação entre Filiais | `ProgramacaoCobranca(+Parcela)`, `TransacaoFilial/{TransacaoFilial, TransacaoFinanceira, FilialMovimento}`, `View/TransacaoFinanceiraView` | E4 | — | Não |
| **E13** | Consultas | ~30 classes `Consultas/*` → read-models / query-methods + `contracts/` | épicos correspondentes | `Consultas/DocumentoParcConsulta` (1508) | Não |
| **E14** | Integração Bancária (fase dedicada final) | `View/ArquivoRemessaView`, `View/ArquivoRetorno{Base, BaseView, 240View, 400View, ViewOutro}`, `View/{AprovacaoDoctoPagarView, LiberacaoDoctoPagarView}`, integração de boleto/OFX | E3, E6, E9 | — | Não |

> **Ordem de execução real** (achado do `data-model.md`): `FINDOMINIO.IDFINCAIXABANCO` é
> `NOT NULL`, então **E3 (Caixa/Banco) executa antes de E2 (Domínio/Período)**. Os números
> de épico são identificadores, não sequência — a sequência definitiva está em
> [`plan.md §3`](./plan.md).

---

## 7. Regras de Negócio Macro (RN-05-NNN)

> Nível de módulo. A extração linha-a-linha de validações (Matriz RTV) e operações
> (Matriz ROT) é feita por `legacy-validation-audit` e `legacy-operation-audit` por épico,
> durante `/plan`.

| ID | Regra | Origem legada (macro) |
| :--- | :--- | :--- |
| **RN-05-001** | **A parcela é o título.** Todo pagar/receber é por `DocumentoParcela`; a liquidação, o estorno e a reversão operam sobre a parcela, não sobre o `Documento`. | `DocumentoParcela.cs`, `Liquidacao.cs`, `ParcelaBase.cs` |
| **RN-05-002** | **Todo movimento pertence a um `DominioPeriodo`.** Um período pode ser fechado; período fechado bloqueia inclusão/alteração/estorno de lançamentos (`IMovimentoPeriodo`). | `DominioPeriodo.cs`, `MovimentoFinanceiro.cs`, `LiquidacaoEstorno.cs:ValidarDominio` |
| **RN-05-003** | **Liquidação com múltiplas formas de pagamento.** A soma das `FormaMovInfo`/`LiquidacaoFormaMovimento` deve fechar com o valor liquidado (+ juros/multa − desconto); cada forma carrega dados específicos (cartão, PIX, cheque, depósito…). | `Liquidacao.cs:ValidarLiquidacao`, `formapagamentomov.cs`, `FormaMovInfo*.cs` |
| **RN-05-004** | **Rateio financeiro.** Movimentos e documentos podem ser rateados por centro de custo/classe; a soma do rateio deve igualar o valor rateável; há contrapartida (`ContraPartidaRateioDoctoFinanceiro`). Rateio é recalculado em `AtualizarRateio`/`PersistirRateio`. | `MovimentoFinanceiro.cs:ValidarRateio/PersistirRateio`, `OperacaoDocumentoBase.cs:AtualizarRateio`, `MovtoFinanceiroRateio.cs` |
| **RN-05-005** | **Controle de saldo de caixa/banco.** Toda operação que movimenta caixa/banco valida e atualiza `SaldoCaixaBanco`; movimento conciliado tem regra própria (`ValidarControleSaldo`, `ValidarValorMovto`). `ValidarControleCaixaBanco` decide se o controle está ativo. | `MovimentoFinanceiro.cs:1958/2005`, `CaixaBanco.cs:774`, `SaldoCaixaBanco.cs` |
| **RN-05-006** | **Sequencial por filial.** IDs de quase todas as entidades `Fin*` são `AutoSequencial(..., SequencialTipo.Filial)` — nunca `IDENTITY`/`GUID` (Artigo II.5). | `[AutoSequencial]` em ~45 classes |
| **RN-05-007** | **`ProcessoOrigem` + `IdOrigem` amarram o documento à sua origem.** Documento financeiro gerado por Faturamento/Compra/Contrato/Frota/OS/NFSe guarda a origem; regras de exclusão/edição dependem dela (`validarProcessoOrigem`). | `Documento.cs`, `DocumentoFinanceiroBase.cs` |
| **RN-05-008** | **Conversão por índice econômico.** `valor` × índice → `valorConvertido`; recalculado quando `recalcularValorConvertido`. | `DocumentoFinanceiroBase.cs`, `IndiceConversor.cs` |
| **RN-05-009** | **Estorno ≠ exclusão.** `LiquidacaoEstorno` reverte a liquidação preservando rastro (formas de pagto do estorno, itens financeiros), reabre a parcela e ajusta saldo/período; validações de domínio/período aplicáveis. | `LiquidacaoEstorno.cs` |
| **RN-05-010** | **Reversão de movimentos.** `Reversao` desfaz movimentos/parcelas gerando lançamentos espelho e correção de rateio (`PersistirCorrecaoRateioReversao`), validando a origem geradora (`ValidarOrigemGerador`). | `Reversao.cs` |
| **RN-05-011** | **Ciclo de vida do cheque.** Cheque recebido e emitido têm máquina de estados própria (`SituacaoCheque`) dirigida por `ChequeRecebidoMovto` / `ChequeEmitidoMovto` (custódia, depósito, compensação, devolução por alínea, repasse, cancelamento). Talão controla faixa de folhas e status por folha. | `ChequeRecebidoMovto.cs`, `ChequeEmitidoMovto.cs`, `TalaoCheque.cs`, `MotivoDevolucaoCheque.cs` |
| **RN-05-012** | **Adiantamento e acerto.** Adiantamento gera crédito; o acerto (`AdtoAcerto`) presta contas distribuindo o valor entre documentos/despesas (`AdtoAcertoDistribuicao`), com rateio e movimento próprios, participando do período. | `Adiantamento.cs`, `AdtoAcerto.cs` |
| **RN-05-013** | **Fechamento de caixa.** `DominioPeriodoFechamento` totaliza por forma de pagamento (`CalcularTotalFormasPagto`) e compara conferido × calculado; divergência é registrada em detalhe. | `DominioPeriodo.cs:972`, `DominioPeriodoFechamento.cs`, `FechamentoCaixaBase.cs` |
| **RN-05-014** | **DRE configurável.** `DRE` → `DRETitulo` → (`DRETituloClasse` | `DRETituloOperacao`): a apuração soma classes/operações por título; classes avulsas permitidas. | `DRE.cs`, `DRETitulo*.cs` |
| **RN-05-015** | **Aprovação/liberação de contas a pagar.** Documento a pagar pode exigir liberação (`documentoLiberado`, `usuarioLiberacao`, `dataLiberacao`) e passar por aprovação em lote antes da liquidação. | `Documento.cs`, `View/LiberacaoDoctoPagarView.cs`, `View/AprovacaoDoctoPagarView.cs` |
| **RN-05-016** | **Cobrança bancária (E14).** Parcela com boleto tem nosso-número/carteira/cedente da `ContaBancaria`; remessa CNAB gera o arquivo, retorno CNAB baixa/atualiza parcela conforme ocorrência; conciliação OFX cruza extrato × movimentos. | `ContaBancaria.cs`, `View/ArquivoRemessaView.cs`, `View/ArquivoRetorno*View.cs` |
| **RN-05-017** | **Transação entre filiais.** Movimento financeiro entre filiais gera lançamentos casados nas duas filiais (`TransacaoFilial` → `TransacaoFinanceira` + `FilialMovimento`), mantendo sequencial global. | `TransacaoFilial/*.cs` |
| **RN-05-018** | **Programação de cobrança (régua).** `ProgramacaoCobranca` define a régua; `ProgramacaoCobrancaParcela` materializa as parcelas a cobrar por evento/data. | `ProgramacaoCobranca*.cs` |
| **RN-05-019** | **Paridade de parâmetros de configuração.** Regras que dependem de parâmetros (controle de caixa/banco ativo, obrigatoriedade de domínio, permissão de liquidar período, etc.) devem replicar o comportamento condicional (Artigo II.7 / Regra 16). | diversas (`ValidarControle*`, checagens de `IAmbiente`/parâmetro) |
| **RN-05-020** | **Cálculo de juros/multa/desconto.** Item financeiro aplica juros (a.m.), multa (%) e desconto sobre a parcela, com regra de arredondamento e ordem de operações específicas do legado — migrar **sem refatorar** (Regra 5) e cobrir com golden tests (`legacy-calc-parity`). | `Documento.cs:CalcularJurosMulta` (e correlatos), `DoctoItemFinanceiro.cs`, `ItemFinanceiro.cs` |

---

## 8. Artefatos SDD subsequentes (checklist)

- [x] `clarify.md` — 13 clarificações resolvidas (2026-09-03); nenhuma pendência bloqueante
- [x] `plan.md` + `research.md` + `data-model.md` + `contracts/` (2026-09-03) + `legacy-schema/` (extract real do banco)
- [x] `tasks.md` (2026-09-03) — Setup + E0..E14 + fecho; ~90 tarefas atômicas
- [~] `analyze-report.md` — **parcial** (V1/V2/V6/V7 executáveis agora; V3/V4/V5 aguardam matrizes por épico)
- [ ] `matriz-rtv.md` (por épico — `legacy-validation-audit`, nas tarefas `E?-T01`)
- [ ] `matriz-rot.md` + máquina de estados (por épico — `legacy-operation-audit`, nas tarefas `E?-T01`)
- [ ] `golden/CALC-*.{md,csv}` (`legacy-calc-parity`, nas tarefas `analysis`/`parity`)

---

## 9. Pendências e DÚVIDAS

Todas as 13 DÚVIDAS foram resolvidas na rodada de clarificação de 2026-09-03 — ver
[`clarify.md`](./clarify.md) (linhas CLR-01..CLR-13). **Nenhuma pendência bloqueante para o
`/plan`.**

| DÚVIDA | Resolução | CLR |
| :--- | :--- | :--- |
| 01 SharedKernel escopo | Mínimo: enums + `Lookup` + container de rateio + interfaces transversais; reusa Framework | CLR-02 |
| 02 Referência a Faturamento | Não — `int` lógico, sem `ProjectReference` (Artigo VIII) | CLR-09 |
| 03 Bases de rateio | Pré-tarefa no **MOD-02** migra `RateioMovto`/`RateioMovtoItem`/`ManutencaoRateio` antes do E5 | CLR-01 |
| 04 Interfaces de domínio | `IMovimentoPeriodo`/`IDadosPeriodoFormaPagto`/`IDadosRateioFinanceiro`/`IDadosComissao` → SharedKernel | CLR-05 |
| 05 Estrangulamento | Só DTOs do AcessoGlobal; nenhum contrato financeiro; nada a reconciliar | CLR-10 |
| 06 Acesso ao banco | Confirmado; `INFORMATION_SCHEMA` + materialização do 1º registro | CLR-11 |
| 07 `ContaBancaria` | Entidade em E3; lógica remessa/retorno/OFX em E14 | CLR-03 |
| 08 Telas React | Backend-first; E9 só recebido/emitido/movimento/consulta | CLR-07 |
| 09 `SelecaoDocumento` | Serviço de consulta sem entidade em E11 | CLR-06 |
| 10 Impressão | Endpoint devolve dados; frontend renderiza; sem PDF no backend | CLR-08 |
| 11 `AdtoAcerto` | Operação transacional (Handler + Matriz ROT), não CRUD | CLR-12 |
| 12 Pastas `Domain/` | Proposta aceita; ajuste fino no `/plan §1` | CLR-13 |
| 13 Bases legadas | Regra fixada: POCO abstrata só de dados + comportamento em serviço/handler | CLR-04 |

**A detalhar dentro do `/plan`** (não bloqueia): enums finos por épico, campos de cada
entidade (`data-model.md`), decisão POCO-abstrata vs. serviço por base individual.

---

## Histórico de Alterações

| Data | Autor | Alteração |
| :--- | :--- | :--- |
| 2026-04-27 | Análise inicial | Criação do rascunho v1.0 (`MOD-05-GESTAO-FINANCEIRA.md`) |
| 2026-09-03 | SDD etapa 1 (`sdd-specify`) | Reescrita v2.0: inventário completo (180 classes), árvore de herança real, mapa de dependências, 14 épicos em ordem topológica, RN-05-001..020 macro, 13 DÚVIDAS. Fixa `net10.0` / constituição v1.0. |
| 2026-09-03 | SDD etapa 2 (`sdd-clarify`) | 13 clarificações resolvidas (`clarify.md`, CLR-01..13): SharedKernel mínimo, pré-tarefa de rateio no MOD-02, `ContaBancaria` E3/E14, regra das bases legadas (POCO abstrata + serviço), interfaces transversais no SharedKernel, `SelecaoDocumento` como serviço, escopo React de cheques, impressão no frontend. §1.3–1.5, §2.1, §2.3, §3.3 (nova), §4.1–4.2, §6 (E0/E3/E5/E9/E10/E11), §8–9 atualizados. Sem pendência bloqueante. |
| 2026-09-03 | SDD etapa 3 (`sdd-plan`) | `plan.md` (estrutura de projeto, E0 SharedKernel, 15 passos de execução, 11 operações com ordem de persistência, 10 riscos), `research.md` (BoletoNet/OFX, enums, GeradorSequencial, Lookup, golden com banco vazio, tipos `text`/`datetime`), `data-model.md` (extract real de `localhost\SQLEXPRESS2008` — PKs compostas por filial, `numeric(23,8)`, nulidade não-uniforme, detalhamento de E-Caixa/E-Domínio/E-Documento núcleo), `contracts/` (README + caixa-banco/documento/liquidacao), `legacy-schema/` (fin_columns.txt + fin_meta.txt). Achado: banco de dev não tem 100% do schema (falta `FINPROJECAOFLUXOCAIXA*`, `FINLOGDOMINIOPERIODO`) e `FINDOMINIO`→`FINCAIXABANCO` inverte a ordem E2/E3. |
| 2026-09-03 | SDD etapa 4 (`sdd-tasks`) | `tasks.md`: fase Setup (S-T01..04) + E0..E14 + fecho (Z-T01..03), ~90 tarefas atômicas (1 tarefa = 1 branch `feat/mod-05-*` = 1 commit), cada uma com objetivo, refs legados, arquivos, `Cobre:` (RN/VAL/OP/CALC), Artigos da constituição, `Pronto quando:` e `Depende de:`. `analyze-report.md` parcial (V1/V2/V6/V7). Data-model: nota reforçando que Fluent API vai no Mapping, não na entidade (igual `acesso.global`). |
| 2026-09-03 | Frontend — Padrão C + templates | Criado **Padrão C — Operação/Assistente** (`.agents/skills/migrate-crud/references/padrao-c-operacao.md`; classificação adicionada em `spec-generator` e `migrate-crud`; `MANUAL-SKILLS.md` atualizado — Lei 12). Templates de spec de tela: `docs/spec_fcaixabanco.md` (Padrão A) e `docs/spec_fliquidacaodocumento.md` (Padrão C). Tarefas `frontend` (E3-T07, E4-T09, E6-T08, E9-T06) atualizadas: E6-T08 cria `BaseOperacaoConfig`. Demais `spec_f*.md` geradas por épico no `/implement`. |
